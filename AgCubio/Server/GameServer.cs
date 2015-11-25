﻿using AgCubio;
using Newtonsoft.Json;
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

        private static Dictionary<string, StateObject> clientStates;
        private static Random rand = new Random();
        private static World mainWorld = new World(1000, 1000);
        private static int uid = 100000;
        private static StringBuilder jsonCubes = new StringBuilder();
        //private TcpListener server;

        public static void Main(string[] args)
        {
            //GameServer server = new GameServer(11000);
            clientStates = new Dictionary<string, StateObject>();
            
            buildWorld();

            
            Network.Server_Awaiting_Client(NameReceived);
            Console.Read();
        }

        private static void buildWorld()
        {
            string[] lines = System.IO.File.ReadAllLines(@"..\..\..\Resources\Libraries\sample.data");
            foreach (string line in lines)
            {
                Cube cube = JsonConvert.DeserializeObject<Cube>(line);
                mainWorld.addCube(cube);
            }
        }

        private static void sendWorld(Socket socket)
        {
            foreach (Cube cube in mainWorld.worldCubes.Values)
            {
                jsonCubes.Append(JsonConvert.SerializeObject(cube) + "\n");
            }
            Network.Send(socket, jsonCubes.ToString());
            
        }
        //public GameServer(int port)
        //{
        //    worldSockets = new Dictionary<string, Socket>();
        //    Network.Server_Awaiting_Client(NameReceived);
        //}

        private static void NameReceived(StateObject state)
        {

            //Network.i_want_more_data(state);
            // save our state string buffer to a new String
            String playerName = state.sb.ToString().Trim();
            // clear out the state buffer
            state.sb.Clear();

            clientStates.Add(playerName, state);
            Console.WriteLine(playerName + " connected to: " + state.workSocket.RemoteEndPoint.ToString());
            
            //TODO process player name

            Network.Send(state.workSocket, GeneratePlayerCube(playerName));
            sendWorld(state.workSocket);

            //state.CallbackAction = ActionReceived;


            //{ "loc_x":395.0,"loc_y":561.0,"argb_color":-2210515,"uid":7,"food":true,"Name":"","Mass":1.0}


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

        private static string GeneratePlayerCube(string playerName)
        {
            Cube playerCube = new Cube(rand.NextDouble()*mainWorld.worldHeight, rand.NextDouble() * mainWorld.worldWidth, 
                                        (int)(rand.NextDouble() * -3000000), uid++, 0, false, playerName, 100.0);
            mainWorld.addCube(playerCube);
            return JsonConvert.SerializeObject(playerCube) +"\n";
        }

        private static void ActionReceived(StateObject state)
        {
            Network.i_want_more_data(state);

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
