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
        public const int BufferSize = 1024;
        /// <summary>
        /// Receive buffer.
        /// </summary>
        public byte[] buffer = new byte[BufferSize];
        /// <summary>
        /// Received data string.
        /// </summary>
        public StringBuilder sb = new StringBuilder();
        /// <summary>
        /// Callback method
        /// </summary>
        public Action<StateObject> CallbackAction;
      /// <summary>
      /// State Object Identifier
      /// </summary>
       public String ID = "";
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
            StateObject ClientStateObject = new StateObject();
            ClientStateObject.CallbackAction = callback;
            // It will need to open a socket and then use the BeginConnect method.Note this method take the 
            // "state" object and "regurgitates" it back to you when a connection is made, thus allowing
            // "communication" between this function and the Connected_to_Server function.

            try
            {
                // Establish the remote endpoint for the socket.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(hostname);
                IPAddress ipAddress = ipHostInfo.AddressList[ipHostInfo.AddressList.Length - 1];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                ClientStateObject.workSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                ClientStateObject.workSocket.BeginConnect(remoteEP, Connected_to_Server, ClientStateObject);
                //Receive((IAsyncResult)MainStateObject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return ClientStateObject.workSocket;
        }
        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <param name="ar"></param>
        public static void Connected_to_Server(IAsyncResult ar)
        {
            // create our state object
            StateObject state = ((StateObject)ar.AsyncState);

            try
            {
                // finalize the initial socket connection
                state.workSocket.EndConnect(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // run callback defined in view - send Player name to server
            state.CallbackAction(state);

        }

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

                // using UTF8 encoding, append buffer contents to our string buffer
                state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                // run the Callback defined in the View - process JSON strings
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
            // Convert the string data to byte data using UTF8 encoding.
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            try
            {
                // Begin sending the data to the remote device.
                socket.BeginSend(byteData, 0, byteData.Length, 0, SendCallBack, socket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        /// <summary>
        /// Assists 'Send'. Do nothing if all data has been sent, otherwise arrange to send remaining data
        /// </summary>
        static void SendCallBack(IAsyncResult ar)
        {

            //If all the data has been sent, then life is good and nothing needs to be done 
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);

                // Signal that all bytes have been sent.
                //sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Close the Asyc Socket
        /// </summary>
        /// <param name="socket"></param>
        public static void Stop(Socket socket)
        {
            socket.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public static void Server_Awaiting_Client(Action<StateObject> callback)
        {
            StateObject serverStateObject = new StateObject();
            serverStateObject.CallbackAction = callback;


            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".


            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            listener.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

            serverStateObject.workSocket = listener;

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(new IPEndPoint(IPAddress.IPv6Any, port));
                listener.Listen(100);


                // Start an asynchronous socket to listen for connections.
                Console.WriteLine("Waiting for a connection...");
                listener.BeginAccept(new AsyncCallback(Accept_a_New_Client), serverStateObject);

                // Wait until a connection is made before continuing.
                //callback();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }


        private static void Accept_a_New_Client(IAsyncResult ar)
        {
            //get socket to handle client requests
            StateObject listenerState = (StateObject)ar.AsyncState;
            Socket clientSocket = listenerState.workSocket.EndAccept(ar);

            //create the client state object
            StateObject clientState = new StateObject();
            clientState.workSocket = clientSocket;
            clientState.CallbackAction = listenerState.CallbackAction; // not sure if this is needed

            clientSocket.BeginReceive(clientState.buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, clientState);

            listenerState.workSocket.BeginAccept(new AsyncCallback(Accept_a_New_Client), listenerState);
        }





        //public static void ReadCallback(IAsyncResult ar)
        //{
        //    String content = String.Empty;

        //    // Retrieve the state object and the handler socket
        //    // from the asynchronous state object.
        //    StateObject state = (StateObject)ar.AsyncState;
        //    Socket handler = state.workSocket;

        //    // Read data from the client socket. 
        //    int bytesRead = handler.EndReceive(ar);

        //    if (bytesRead > 0)
        //    {
        //        // There  might be more data, so store the data received so far.
        //        state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));

        //        // Check for end-of-file tag. If it is not there, read more data.
        //        content = state.sb.ToString();
        //        if (content.IndexOf("<EOF>") > -1)
        //        {
        //            // All the data has been read from the client. Display it on the console.
        //            Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
        //            // Echo the data back to the client.
        //            Send(handler, content);
        //        }
        //        else
        //        {
        //            // Not all data received. Get more.
        //            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
        //            new AsyncCallback(ReadCallback), state);
        //        }
        //    }
        //}
    }
}
