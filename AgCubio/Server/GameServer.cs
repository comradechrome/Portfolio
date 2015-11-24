using AgCubio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GameServer
    {

        private static Dictionary<string, Socket> worldSockets;

        //private TcpListener server;

        static void Main(string[] args)
        {
            //GameServer server = new GameServer(11000);
            worldSockets = new Dictionary<string, Socket>();
            Network.Server_Awaiting_Client(NameReceived);
            Console.Read();
        }

        
        //public GameServer(int port)
        //{
        //    worldSockets = new Dictionary<string, Socket>();
        //    Network.Server_Awaiting_Client(NameReceived);
        //}

        private static void NameReceived(StateObject state)
        {

            Network.Receive(state);
            // save our state string buffer to a new String
            String playerName = state.sb.ToString().Trim();
            // clear out the state buffer
            state.sb.Clear();

            worldSockets.Add(playerName, state.workSocket);
            Console.WriteLine(playerName + "WEEEEEEEEEEEEEEEEEEEEE");

            state.CallbackAction = ActionReceived;


            //    // create cubes in world
            //    lock (mainWorld)
            //    {
            //        mainWorld.processCube(line);
            //    }

            //    hasCubes = true;


            //}
            //isConnected = true;

            //// request more cube data from server
            //Network.i_want_more_data(state);
            //worldSocket = state.workSocket;
     
        }

        private static void ActionReceived(StateObject state)
        {
            Network.Receive(state);

            //process moves and splits
        }



        //private void ConnectionReceived(IAsyncResult ar)
        //{
        //    Socket socket = server.EndAcceptSocket(ar);
        //    socket.Listen(45);
        //    //StringSocket ss = new StringSocket(socket, UTF8Encoding.Default);
        //    //ss.BeginReceive(NameReceived, ss);
        //    server.BeginAcceptSocket(ConnectionReceived, null);
        //}
    }
}
