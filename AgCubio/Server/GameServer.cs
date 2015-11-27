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
      private static World mainWorld = new World(mainWorldParams.height, mainWorldParams.width);
      private static int uid = 0;
      private static StringBuilder jsonCubes = new StringBuilder();
      private static Dictionary<string, Tuple<int, int>> mousePoints;
      private static Dictionary<string, Tuple<int, int>> splitPoints;
      private static WorldParams mainWorldParams;


      //private TcpListener server;

      public static void Main(string[] args)
      {
         // check if parameter file was supplied as an argument
         if (args.Length == 1)
         {
            String fileName = args[0];
            mainWorldParams = new WorldParams(fileName);

         }
         // no argument so create world using defaults
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
      /// <summary>
      /// Temporary functino to build our worls cubes from sample data
      /// </summary>
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

      /// <summary>
      /// Given the width of a new Cube, finds an unoccupied x,y coordinate
      /// x1,y1,x2,y2 will be the upper left and lower right coordiantes of the random 'proposed' newcube
      /// x3,y3,x4,y4 will be the upper left and lower right coordiantes of the existing cube we are checking
      /// </summary>
      /// <param name="width"></param>
      /// <returns></returns>
      private Tuple<double,double> availablePosition(double width)
      {
         int x = 0;
         int y = 0;
         bool available = false;
         int newRadius = (int)Math.Ceiling(width/2.0); // round up
         int cubeRadius;


         while (!available)
         {
            // generate random coordiantes that fall withing the world. Then generate the cube diagonal points.
            x = rand.Next(newRadius, mainWorldParams.height - newRadius);
            y = rand.Next(newRadius, mainWorldParams.width - newRadius);
            int x1 = x - newRadius;
            int y1 = y - newRadius;
            int x2 = x + newRadius;
            int y2 = y + newRadius;


            foreach (var cube in mainWorld.worldCubes)
            { 
               cubeRadius = (int)Math.Ceiling(cube.Value.Width / 2.0); // round up
               // get diagonal corners of current cube
               int x3 = (int)cube.Value.loc_x - cubeRadius;
               int y3 = (int)cube.Value.loc_y - cubeRadius;
               int x4 = (int)cube.Value.loc_x + cubeRadius;
               int y4 = (int)cube.Value.loc_y + cubeRadius;

               // this algorithm checks to see if new cube [(x1,y1),(x2,y2)] overlaps current cube [(x3,y3),(x4,y4)]
               // more specifically, it's checking 4 conditions where the cubes cannot overlap - if any are true, the cubes do not overlap

               if (!(y2 < y3 || y1 > y4 || x2 < x3 || x1 > x4))
                  // cubes overlap; break, generate a new cube, check again
               {
                  available = false;
                  break;
               }
               else
               // cubes do not overlap, set flag to true and check another cube
               {
                  available = true;
               }
            }
         }
         
         return Tuple.Create((double)x, (double)y);
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
