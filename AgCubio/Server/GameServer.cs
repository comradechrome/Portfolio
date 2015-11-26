using AgCubio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace Server
{
   class GameServer
   {

      private static Dictionary<string, StateObject> clientStates;
      private static Random rand = new Random();
      private static World mainWorld = new World(1000, 1000);
      private static int uid = 100000;
      private static StringBuilder jsonCubes = new StringBuilder();
      private static Dictionary<string, Tuple<int, int>> mousePoints;
      private static Dictionary<string, Tuple<int, int>> splitPoints;
      private static WorldParams mainWorldParams;


      //private TcpListener server;

      public static void Main(string[] args)
      {

         if (args.Length == 1)
         {
            String fileName = args[0];
            mainWorldParams = new WorldParams(fileName);

         }
         else
         {
            mainWorldParams = new WorldParams();
         }

         //GameServer server = new GameServer(11000);
         clientStates = new Dictionary<string, StateObject>();

         Server.GameServer.buildWorld();


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
         // save the player name into the State object
         state.ID = playerName;

         clientStates.Add(playerName, state);
         Console.WriteLine(playerName + " connected to: " + state.workSocket.RemoteEndPoint.ToString());

         //TODO process player name

         Network.Send(state.workSocket, GeneratePlayerCube(playerName));
         //Network.Send(state.workSocket, "{\"loc_x\":279.0,\"loc_y\":458.0,\"argb_color\":-7381092,\"uid\":21,\"food\":true,\"Name\":\"\",\"Mass\":1000.0}\n");
         sendWorld(state.workSocket);

         state.CallbackAction = ActionReceived;


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
         Network.i_want_more_data(state);
         //worldSocket = state.workSocket;

      }

      private static string GeneratePlayerCube(string playerName)
      {
         
         Cube playerCube = new Cube(rand.Next(mainWorldParams.height), rand.Next(mainWorldParams.width),
                                     randomColor(), uid++, 0, false, playerName, mainWorldParams.playerStartMass);
         mainWorld.addCube(playerCube);
         return JsonConvert.SerializeObject(playerCube) + "\n";
      }

      private static string GenerateFoodCube()
      {
         Cube foodCube = new Cube(rand.Next(mainWorldParams.height), rand.Next(mainWorldParams.width), 
                             randomColor(), uid++, 0, true, "", mainWorldParams.foodValue);
         mainWorld.addCube(foodCube);
         return JsonConvert.SerializeObject(foodCube) + "\n";
      }

      private static int randomColor()
      {
         return Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255)).ToArgb();
      }

      private static void ActionReceived(StateObject state)
      {
         // save our state string buffer to a new String
         String actionString = state.sb.ToString();
         // clear out the state buffer
         state.sb.Clear();
         int x = 0;
         int y = 0;

         if (actionString.StartsWith("(move"))
         {
            Regex pattern = new Regex(@"\(move,\s*(\-?\d+),\s*(\-?\d+).*");
            Match match = pattern.Match(actionString);
            x = int.Parse(match.Groups[1].Value);
            y = int.Parse(match.Groups[2].Value);

            // add to our mousePoints dictionary. should we Lock this ??
            lock (mousePoints)
            {
               mousePoints.Add(state.ID, Tuple.Create(x, y));
            }
            

            Console.WriteLine(actionString + "\nX: " + x + " Y: " + y);
         }
         else if (actionString.StartsWith("(split"))
         {
            Regex pattern = new Regex(@"\(split,\s*(\-?\d+),\s*(\-?\d+).*");
            Match match = pattern.Match(actionString);
            x = int.Parse(match.Groups[1].Value);
            y = int.Parse(match.Groups[2].Value);

            // add to our mousePoints dictionary. should we Lock this ??
            lock (splitPoints)
            {
               splitPoints.Add(state.ID, Tuple.Create(x, y));
            }

         }
         Network.i_want_more_data(state);
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
