using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio
{
    /// <summary>
    /// When data arrives on a socket, keep track of:
    ///   * What previous data has arrived
    /// Decide:
    ///   * Wwhat function to call in the Client when data arrives.
    /// </summary>
    public class StateObject
    {
      /// <summary>
      /// Client socket.
      /// </summary>
      public Socket workSocket = null;
      /// <summary>
      /// Size of receive buffer.
      /// </summary>
      public const int BufferSize = 2048;
      /// <summary>
      /// Receive buffer.
      /// </summary>
      public byte[] buffer = new byte[BufferSize];
      /// <summary>
      /// Received data string.
      /// </summary>
      public StringBuilder sb = new StringBuilder();
      /// <summary>
      /// 
      /// </summary>
        public Action<StateObject> CallbackAction;


    }

    /// <summary>
    /// Helper functions to handle networking.
    /// 
    /// Network Protocol Summary:
    ///  - Establish a socket connection to the server at port number 11000.
    ///  - Upon connection, send a single '\n' terminated string representing the player name.
    ///  - The server will then send a cube object in JSON representing the player.
    ///  - The server will then continually send the current state of the system. (All cube objects)
    ///  - The client can at anytime send a move or split request of the form([move|split], x, y)
    /// </summary>
    public static class Network
    {
        private const int port = 11000;

        /// <summary>
        /// Attempt to connect to the server via a provided hostname. Save the callback function (in a state object)
        ///  for use when data arrives.
        /// </summary>
        /// <param name="callback">function inside the View to be called when a connection is made</param>
        /// <param name="hostname">name of the server to connect to</param>
        /// <returns></returns>
        public static Socket Connect_to_Server(Action<StateObject> callback, String hostname)
        {
            StateObject MainStateObject = new StateObject();
            MainStateObject.CallbackAction = callback;
            // It will need to open a socket and then use the BeginConnect method.Note this method take the 
            // "state" object and "regurgitates" it back to you when a connection is made, thus allowing
            // "communication" between this function and the Connected_to_Server function.

            try
            {
                // Establish the remote endpoint for the socket.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(hostname);
                IPAddress ipAddress = ipHostInfo.AddressList[1];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                MainStateObject.workSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                MainStateObject.workSocket.BeginConnect(remoteEP, Connected_to_Server, MainStateObject);
                //Receive((IAsyncResult)MainStateObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return MainStateObject.workSocket;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        public static void Connected_to_Server(IAsyncResult ar)
        {
            StateObject state = ((StateObject)ar.AsyncState);

         try
         {
            state.workSocket.EndConnect(ar);
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }
         
            state.CallbackAction(state);
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public static void Receive(StateObject state)
        {
            try
            {

                // Begin receiving the data from the remote device.
                state.workSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        ///// <summary>
        ///// Referenced by the BeginConnect method above and is "called" by the OS when the socket connects to the server.
        ///// 
        ///// </summary>
        ///// <param name="state_in_an_ar_object">contains a field "AsyncState" which contains the "state" object saved in 'BeginConnect'</param>
        //static void Connected_to_Server(IAsyncResult state_in_an_ar_object)
        //{
        //    // Once a connection is established the "saved away" callback function needs to called.
        //    // Additionally, the network connection should "BeginReceive" expecting more data to arrive 
        //    // (and provide the ReceiveCallback function for this purpose)
        //}

        /// <summary>
        /// Called by the OS when new data arrives.
        /// This method should check to see how much data has arrived. 
        /// If 0, the connection has been closed (presumably by the server). 
        /// On greater than zero data, this method should call the callback function provided above.
        /// </summary>
        /// <param name="ar"></param>
        static void ReceiveCallback(IAsyncResult ar)
        {

            //For our purposes, this function should not request more data. 
            //It is up to the code in the callback function above to request more data.

            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

                state.CallbackAction(state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        /// <summary>
        /// Called when the View code wants more data.
        /// </summary>
        /// <param name="state"></param>
        public static void i_want_more_data(StateObject state)
        {
            //Note: the client will probably want more data every time it gets data.
            try
            {
                // Begin receiving the data from the remote device.
                state.workSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// With the help of 'SendCallBack', allow a program to send data over a socket.
        /// Convert to bytes then send using socket.BeginSend
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        public static void Send(Socket socket, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.UTF8.GetBytes(data);
         try
         {
            // Begin sending the data to the remote device.
            socket.BeginSend(byteData, 0, byteData.Length, 0, SendCallBack, socket);
         }
         catch (Exception e)
         {
            throw e;
         }
            
        }

        /// <summary>
        /// Assists 'Send'. Do nothing if all data has been sent, otherwise arrange to send remaining data
        /// </summary>
        static void SendCallBack(IAsyncResult ar)
        {

            //If all the data has been sent, then life is good and nothing needs to be done 
            //(note: you may, when first prototyping your program, put a WriteLine in here to see when data goes out).

            //(see the ChatClient example program).

            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to server.", bytesSent);
                //Console.Read();

                // Signal that all bytes have been sent.
                //sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

      public static void Stop(Socket socket)
      {
         socket.Close();
      }
   }
}
