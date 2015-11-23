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

        private Dictionary<string, Socket> worldSockets;

        private TcpListener server;

        static void Main(string[] args)
        {
            GameServer server = new GameServer(11000);
        }

        
        public GameServer(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            worldSockets = new Dictionary<string, Socket>();
            server.Start();
            server.BeginAcceptSocket(ConnectionReceived, null);
        }

        private void ConnectionReceived(IAsyncResult ar)
        {
            Socket socket = server.EndAcceptSocket(ar);
            //StringSocket ss = new StringSocket(socket, UTF8Encoding.Default);
            //ss.BeginReceive(NameReceived, ss);
            server.BeginAcceptSocket(ConnectionReceived, null);
        }
    }
}
