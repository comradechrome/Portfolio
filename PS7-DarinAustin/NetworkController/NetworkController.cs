using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AgCubio
{
    /// <summary>
    /// Delegate to use for callback
    /// </summary>
    public delegate void Callback();

    /// <summary>
    /// State class represents the state of the socket connection
    /// at all times.
    /// </summary>
    public class State
    {
        /// <summary>
        /// Callback to use
        /// </summary>
        public Callback Callback;
        /// <summary>
        /// Socket information
        /// </summary>
        public Socket socket = null;
        /// <summary>
        /// Size of receive buffer
        /// </summary>
        public const int bufferSize = 1024;
        /// <summary>
        /// Recieve buffer
        /// </summary>
        public byte[] buffer = new byte[bufferSize];
        /// <summary>
        /// Recieved data
        /// </summary>
        public StringBuilder sb = new StringBuilder();

        /// <summary>
        /// Constructor sets callback
        /// </summary>
        /// <param name="Callback">Call back to use upon construction</param>
        public State(Callback Callback)
        {
            this.Callback = Callback;
        }
    }

    /// <summary>
    /// NetworkController is used to open socket connection. It also
    /// facilitates sending and recieving data between the client and 
    /// end point using the socket.
    /// </summary>
    public static class NetworkController
    {
        const int port = 11000;
        
        /// <summary>
        /// Initiate connection to server using provided hostname.
        /// </summary>
        /// <param name="Callback">Callback function to use</param>
        /// <param name="hostName">Hostname to connect to</param>
        /// <returns>Socket connection</returns>
        public static State ConnectToServer(Callback Callback, string hostName)
        {
            // gets ip address from a domain name
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint server = new IPEndPoint(ipAddress, port);

            // initializes socket and state
            State state = new State(Callback);
            state.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // callback ConnectedToServer once connection is established
            state.socket.BeginConnect(server, ConnectedToServer, state);
            return state;
        }

        /// <summary>
        /// Callback once a successful socket connection has been made
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectedToServer(IAsyncResult ar)
        {
            State state = (State)ar.AsyncState;
            // catches any socket connection exceptions and allows caller to handle it 
            // in their callback method by checking state.socket.Connected
            try
            {
                state.socket.EndConnect(ar);
            }
            catch(SocketException)
            {
                state.Callback();
            }
            // runs caller callback and begins receiving data
            state.Callback();
            state.socket.BeginReceive(state.buffer, 0, State.bufferSize, 0, ReceiveCallback, state);
        }

        /// <summary>
        /// Callback funtion that is called when data comes in from the end point
        /// </summary>
        /// <param name="ar">Asynce result from end point</param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            State state = (State) ar.AsyncState;
            int bytesRead = state.socket.EndReceive(ar);

            // if bytesRead == 0 then the connection was cloesd.
            if(bytesRead > 0)
            {
                lock (state.sb)
                {
                    // append data recieved onto the string builder
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                }
                state.Callback();
                //state.socket.BeginReceive(state.buffer, 0, State.bufferSize, 0, ReceiveCallback, state);
            }

            //If bytesRead == 0 then the server has closed the connection
            else
            {
                state.socket.Close();
            }
        }

        /// <summary>
        /// Function used once a connection is made and caller wants to recieve more data
        /// </summary>
        /// <param name="state">State to use</param>
        public static void IWantMoreData(State state)
        {
            state.socket.BeginReceive(state.buffer, 0, State.bufferSize, 0, ReceiveCallback, state);
        }

        /// <summary>
        /// Function used to send data to end point.
        /// </summary>
        /// <param name="state">State to use</param>
        /// <param name="data">Data to send</param>
        public static void Send(State state, string data)
        {
            byte[] bytesData = Encoding.UTF8.GetBytes(data);
            state.socket.BeginSend(bytesData, 0, bytesData.Length, 0, SendCallback, state);
        }

        /// <summary>
        /// Callback to use once data has been sent. Used to end send.
        /// </summary>
        /// <param name="ar">Asynce result from end point</param>
        private static void SendCallback(IAsyncResult ar)
        {
            State state = (State)ar.AsyncState;
            int bytesSent = state.socket.EndSend(ar);

            // The socket has been closed
            if (bytesSent == 0)
            {
                state.socket.Close();
            }
        }
    }
}

   
