using AgCubio;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Timers;

namespace Server
{
   class GameServer
   {

      private static Dictionary<string, StateObject> clientStates;
      private static Random rand = new Random();
      private static World mainWorld;
      private static int uid = 0;
      private static Dictionary<string, Tuple<int, int>> mousePoints 
                              = new Dictionary<string, Tuple<int, int>>();
      private static Dictionary<string, Tuple<int, int>> splitPoints 
                              = new Dictionary<string, Tuple<int, int>>();
      private static WorldParams mainWorldParams;
      private static Timer heartbeat;

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

         
         mainWorld = new World(mainWorldParams.height, mainWorldParams.width);

         //GameServer server = new GameServer(11000);
         clientStates = new Dictionary<string, StateObject>();

         // add food cubes to world
         for (int i = 0; i < mainWorldParams.maxFood; i++)
         {
            GenerateFoodCube();
         }

         Network.Server_Awaiting_Client(NameReceived);

         SetTimer();
         Console.Read();
      }

      private static void SetTimer()
      {
    
         // Create a timer based on the heartbeatsPerSecond parameter
         heartbeat = new Timer(1000.0/mainWorldParams.heartbeatsPerSecond);
         // Hook up the Elapsed event for the timer. 
         heartbeat.Elapsed += OnHeartbeat;
         heartbeat.AutoReset = true;
         heartbeat.Enabled = true;
      
   }
      /// <summary>
      /// TODO: rather than return JSON strings, it would be better to create a HashSet 
      /// of cube UID's that need to be updated. We then add a function towards the end 
      /// that creats the JSON string builder and then removes all cubes of zero mass.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private static void OnHeartbeat(object sender, ElapsedEventArgs e)
      {
         // String Builder to hold all cubes needing an update.
         StringBuilder jsonCubes = new StringBuilder();
         HashSet<Cube> modifiedCubes = new HashSet<Cube>();
         // stop the heartbeat
         heartbeat.Stop();
         // grow new food if needed and append to modifiedCubes set
         modifiedCubes.UnionWith(createFood());
         // shrink players
         attrition();
         // randomly increate the mass of food cubes
         modifiedCubes.Add(randomFoodGrowth());
         // process cube movements
         processMoves();
         // process cube splits
         // TODO: processSplits();
         // virus mechanics - append any created or destroyed virus cubes to the json string builder
         // TODO: modifieCubes.UnionWith(spawnVirus());
         // players eating food and players eating players, players hitting  
         // and remove them from the world (players and food)
         absorb();
         // add all player cubes to the modifiedCubes set
         modifiedCubes.UnionWith(addPlayers());
         // remove all zero mass cubes
         modifiedCubes.UnionWith(removeDead());
         // send update to all players of all added and eaten food along will all players
         foreach (var cube in modifiedCubes)
         {
            jsonCubes.Append(JsonConvert.SerializeObject(cube) + "\n");
         }
            sendUpdates(jsonCubes);
         //start the heartbeat back up
         heartbeat.Start();

      }

      private static HashSet<Cube> removeDead()
      {
         HashSet<Cube> deadCubes = new HashSet<Cube>();

         lock (mainWorld)
         {
            // find all cubes with zero mass
            foreach (var cube in mainWorld.worldCubes)
            {
               if (cube.Value.Mass < 1.0)
                  deadCubes.Add(cube.Value);
            }
            // remove cubes from world
            foreach (var cube in deadCubes)
            {
               mainWorld.removeCube(cube);
            }
         }
         return deadCubes;
      }

      private static HashSet<Cube> createFood()
      {
         HashSet<Cube> newCubes = new HashSet<Cube>();
         int foodCount = 0;

         lock (mainWorld)
         {
            if (mainWorld.worldCubes.Count > 0)
            {
               foreach (var cube in mainWorld.worldCubes)
               {
                  if (cube.Value.food)
                     foodCount++;
               }
            }
         }
         if (foodCount < mainWorldParams.maxFood)
            newCubes.Add(GenerateFoodCube());

         return newCubes;
      }

      /// <summary>
      /// At each heartbeat of the game every player cube above the minMass value will lose mass
      /// below the acceleratedMass value a player will lose 1% per second with an attritionRate of 200
      /// Above the acceleratedMass value a player will lose 2% per second with an attritionRate of 200
      /// </summary>
      private static void attrition()
      {
         int minMass = 200;
         int acceleratedMass = 800;

         if (mainWorld.playerCubes.Count > 0)
         {
            lock (mainWorld)
            {
               foreach (var player in mainWorld.playerCubes)
               {
                  Cube cube = mainWorld.worldCubes[player.Value];
                  // decrease mass if cube is above minimum mass - 2%/sec if attrition rate is 200
                  if (cube.Mass > acceleratedMass)
                  {
                     cube.Mass -= cube.Mass * mainWorldParams.attritionRate /
                                        (10000 * mainWorldParams.heartbeatsPerSecond);
                  }
                  else if (cube.Mass > minMass)
                  {
                     // decrease mass by 1%/sec if attrition rate is 200
                     cube.Mass -= cube.Mass * mainWorldParams.attritionRate /
                                        (20000 * mainWorldParams.heartbeatsPerSecond);
                  }
               }
            }
         }
      }

      /// <summary>
      /// Randomly genrates a number. If a food cube exists with that number we grow the food
      /// If growth was successful, we return the food ID, otherwise we return 0
      /// </summary>
      private static Cube randomFoodGrowth()
      {
         int randomFactor = 100; // TODO: add this to parameter file
         int growthFactor = 5;
         Cube foodCube = null;

         int randomFoodID = rand.Next(0, randomFactor * mainWorldParams.maxFood);

         lock (mainWorld)
         {
            if (mainWorld.worldCubes.ContainsKey(randomFoodID) && mainWorld.worldCubes[randomFoodID].food)
            {
               mainWorld.worldCubes[randomFoodID].Mass = mainWorld.worldCubes[randomFoodID].Mass*growthFactor;
               foodCube = mainWorld.worldCubes[randomFoodID];
            }  
         }
         return foodCube;
      }

      private static void processMoves()
      {
         lock (mainWorld)
         {
            if (mousePoints.Count > 0)
            {
               foreach (var coordinates in mousePoints)
               {
                  // player name and mouse coordinates
                  string playerName = coordinates.Key;
                  int x = coordinates.Value.Item1;
                  int y = coordinates.Value.Item2;

                  // get players cube current position and mass - 1st check if player exists
                  if (mainWorld.playerCubes.ContainsKey(playerName))
                  {


                     Cube cube = mainWorld.worldCubes[mainWorld.playerCubes[playerName]];
                     double cubeX = cube.loc_x;
                     double cubeY = cube.loc_y;
                     double mass = cube.Mass;

                     // calculate the distance from our mouse to the cube
                     double distX = x - cubeX;
                     double distY = y - cubeY;

                     // make sure distace is greater than 1
                     double distance = Math.Sqrt(distX*distX + distY*distY);

                     if (distance > 1.0)
                     {
                        cube.loc_x += distX*smoothingFactor(mass);
                        cube.loc_y += distY*smoothingFactor(mass);
                     }
                  }
               }
            }
         }
      }

      /// <summary>
      /// f(mass) = factor
      /// f(200) = .01 (top_speed = 5)
      /// f(2700) = .00875
      /// f(3200) = .0075
      /// f(4700) = .00625
      /// f(6200) = .005 (low_speed = 1)
      /// </summary>
      /// <param name="mass"></param>
      /// <returns></returns>
      private static double smoothingFactor(double mass)
      {
         double scaleConst = .00125;
         double smoothingIncrement = 1500;
         double minFactor = 3 * scaleConst + mainWorldParams.lowSpeed * scaleConst;
         double maxFactor = 3 * scaleConst + mainWorldParams.topSpeed * scaleConst;

         double factor = 3 * scaleConst + mainWorldParams.topSpeed + 
                              ((mainWorldParams.minSplitMass - mass) / smoothingIncrement) * scaleConst;

         if (factor < minFactor) { return minFactor; }
         if (factor > maxFactor) { return maxFactor; }
         return factor;

      }

      /// <summary>
      /// 
      /// </summary>
      private static void processSplits()
      {
         
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      private static string virusUpdates()
      {
         StringBuilder jsonCubes = new StringBuilder();

         return jsonCubes.ToString();
      }
      /// <summary>
      /// Detect cube collisions and take appropriate action. Cubes that need to be updated are added to the 
      /// cubes HashSet and are returned to the caller
      /// </summary>
      /// <returns></returns>
      private static void absorb()
      {
         HashSet<Cube> cubeUpdates = new HashSet<Cube>();
         HashSet<Cube> infectedCubes = new HashSet<Cube>();


         if (mainWorld.playerCubes.Count > 0)
         {
            lock (mainWorld)
            {
               foreach (var playerID in mainWorld.playerCubes)
               {
                  Cube playerCube = mainWorld.worldCubes[playerID.Value];
                  // only run loop if player is not dead
                  if (playerCube.Mass > 0)
                  {
                     // get the x,y coordinates of the upper left and lower right of the player cube
                     Tuple<int,int,int,int> playerCorners = playerCube.corners;

                     int playerCubeX1 = playerCorners.Item1;
                     int playerCubeY1 = playerCorners.Item2;
                     int playerCubeX2 = playerCorners.Item3;
                     int playerCubeY2 = playerCorners.Item4;

                     foreach (var cube in mainWorld.worldCubes)
                     {
                        // make sure we're not comparing playerCube to itself
                        if (cube.Value.uid != playerCube.uid) 
                        {
                           // get the x,y coordinates of the upper left and lower right of the checked cube
                           Tuple<int, int, int, int> cubeCorners = cube.Value.corners;

                           int cubeX1 = cubeCorners.Item1;
                           int cubeY1 = cubeCorners.Item2;
                           int cubeX2 = cubeCorners.Item3;
                           int cubeY2 = cubeCorners.Item4;

                           // this algorithm checks to see if player cube [(x1,y1),(x2,y2)] overlaps current cube [(x1,y1),(x2,y2)]
                           // more specifically, it's checking 4 conditions where the cubes cannot overlap - if any are true, the cubes do not overlap

                           if (!(playerCubeY2 < cubeY1 || playerCubeY1 > cubeY2 ||
                                 playerCubeX2 < cubeX1 || playerCubeX1 > cubeX2))
                              // cubes overlap; figure out cube type and take appropriate action
                           {
                              // check if encountered cube is food
                              if (cube.Value.food && cube.Value.Name == "")
                              {
                                 playerCube.Mass += cube.Value.Mass;
                                 cube.Value.Mass = 0;
                                 cubeUpdates.Add(cube.Value);
                              }
                              // check if encountered cube is a virus and larger than the player cube
                              else if (cube.Value.food && cube.Value.Mass >= playerCube.Mass)
                              {
                                 // remove virus and add player to infected HashSet
                                 cube.Value.Mass = 0;
                                 infectedCubes.Add(playerCube);
                              }
                              // check if cube is a virus and smaller than the player cube
                              else if (cube.Value.food)
                              {
                                 //TODO:  move cube so it doesn't overlap virus

                              }
                              // cube mass is greater than player so we remove player cube
                              else if (cube.Value.Mass > playerCube.Mass)
                              {
                                 //TODO: once we have figured out splitting, we need to add logic here to figure out if this is last of the players cubes - close socket connection
                                 cube.Value.Mass += playerCube.Mass;
                                 playerCube.Mass = 0;
                              }
                              else
                              // cube is smaller (or equal) than the player cube so we will remove the cube
                              {
                                 // TODO: just as the case above, we need method to determine if this is the last of a players cubes - close socket connection
                                 playerCube.Mass += cube.Value.Mass;
                                 cube.Value.Mass = 0;
                              }
                           }
                        }
                     }
                  }
               }

               // TODO: process infected cubes
               // processInfected(infectedCubes);
            }
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      private static HashSet<Cube> virusCollision()
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Generate a HashSet of all player cubes
      /// </summary>
      /// <returns></returns>
      private static HashSet<Cube> addPlayers()
      {
         HashSet<Cube> playerCubes = new HashSet<Cube>();

         if (mainWorld.playerCubes.Count > 0)
         {
            lock (mainWorld)
            {
               //append all player cubes to the string builder
               foreach (var playerID in mainWorld.playerCubes)
               {
                  playerCubes.Add(mainWorld.worldCubes[playerID.Value]);
               }
            }
         }

         return playerCubes;
      }

      /// <summary>
      /// receives a string in JSON format of all food cubes that have ben updated
      /// We convert all Player cubes to JSON and append to the passed in string
      /// </summary>
      /// <param name="jsonCubes"></param>
      private static void sendUpdates(StringBuilder jsonCubes)
      {
         if (clientStates.Count > 0)
         {
            lock (mainWorld)
            {
               //send all player and updated food cubes to all clients in JSON format
               foreach (var clients in clientStates)
               {
                  Network.Send(clients.Value.workSocket, jsonCubes.ToString());
               }
            }
         }
      }

      /// <summary>
      /// Temporary functino to build our worls cubes from sample data
      /// </summary>
      //private static void buildWorld()
      //{
      //   string[] lines = System.IO.File.ReadAllLines(@"..\..\..\Resources\Libraries\sample.data");
      //   foreach (string line in lines)
      //   {
      //      Cube cube = JsonConvert.DeserializeObject<Cube>(line);
      //      mainWorld.addCube(cube);
      //   }
      //}



      private static void sendWorld(Socket socket)
      {
         StringBuilder jsonCubes = new StringBuilder();
         lock (mainWorld)
         {
            foreach (Cube cube in mainWorld.worldCubes.Values)
            {
               jsonCubes.Append(JsonConvert.SerializeObject(cube) + "\n");
            }
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
         lock (mainWorld)
         {
            clientStates.Add(playerName, state);
         }
         Console.WriteLine(playerName + " connecting from: " + state.workSocket.RemoteEndPoint.ToString());

         Network.Send(state.workSocket, GeneratePlayerCube(playerName));
         // absorb and remove any cubes we landed on before sending anything else to client
         absorb();
         removeDead();
         // send world cubes to client
         sendWorld(state.workSocket);

         state.CallbackAction = ActionReceived;

         Network.i_want_more_data(state);
         

         //    // create cubes in world
         //    lock (mainWorld)
         //    {
         //        mainWorld.processCube(line);
         //    }

         //    hasCubes = true;


         //}
         //isConnected = true;

         //// request more cube data from server

         //worldSocket = state.workSocket;

      }

      private static string GeneratePlayerCube(string playerName)
      {
         double width = Cube.getWidth(mainWorldParams.playerStartMass);
         int radius = (int)Math.Ceiling(width / 2.0); // round up
         Cube playerCube = new Cube(rand.Next(radius,mainWorldParams.height - radius), rand.Next(radius, mainWorldParams.width - radius),
                                     randomColor(), uid++, 0, false, playerName, mainWorldParams.playerStartMass);
         lock (mainWorld)
         {
            mainWorld.addCube(playerCube);
            mainWorld.playerCubes[playerName] = playerCube.uid;
         }
         return JsonConvert.SerializeObject(playerCube) + "\n";
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      private static Cube GenerateFoodCube()
      {
         Tuple<double, double> coordinates = availablePosition(mainWorldParams.foodValue);
         Cube foodCube = new Cube(coordinates.Item1, coordinates.Item2, randomColor(),
                                   uid++, 0, true, "", mainWorldParams.foodValue);
         lock (mainWorld)
         {
            mainWorld.addCube(foodCube);
         }
         return foodCube;
      }

      private static int randomColor()
      {
         return Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255)).ToArgb();
      }

      /// <summary>
      /// Given the mass of a new Cube, finds an unoccupied x,y coordinate
      /// x1,y1,x2,y2 will be the upper left and lower right coordiantes of the random 'proposed' newcube
      /// x3,y3,x4,y4 will be the upper left and lower right coordiantes of the existing cube we are checking
      /// </summary>
      /// <param name="width"></param>
      /// <returns></returns>
      public static Tuple<double,double> availablePosition(double mass)
      {
         double width = Cube.getWidth(mass);
         int x = 0;
         int y = 0;
         bool available = false;
         int newRadius = (int)Math.Ceiling(width/2.0); // round up



         while (!available)
         {
            // generate random coordiantes that fall withing the world. Then generate the cube diagonal points.
            x = rand.Next(newRadius, mainWorldParams.height - newRadius);
            y = rand.Next(newRadius, mainWorldParams.width - newRadius);
            int x1 = x - newRadius;
            int y1 = y - newRadius;
            int x2 = x + newRadius;
            int y2 = y + newRadius;

            if (mainWorld.worldCubes.Count > 0)
            {
               // we have cubes so we'll need to check for overlaps
               lock (mainWorld)
               {


                  foreach (var cube in mainWorld.worldCubes)
                  {
                     int cubeRadius = (int) Math.Ceiling(cube.Value.Width/2.0); // round up
                     // get diagonal corners of current cube
                     // get the x,y coordinates of the upper left and lower right of the player cube
                     Tuple<int, int, int, int> corners = cube.Value.corners;

                     int x3 = corners.Item1;
                     int y3 = corners.Item2;
                     int x4 = corners.Item3;
                     int y4 = corners.Item4;

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
            }
            else
            {
               // no cubes yet, so we'll set flag to true
               available = true;
            }
         }
         
         return Tuple.Create((double)x, (double)y);
      }

      /// <summary>
      /// ensures that our stored X values fall within the range of the world 
      /// </summary>
      /// <param name="xValue"></param>
      /// <param name="width"></param>
      /// <returns></returns>
      public static int widthRange(int xValue,double width)
      {
         int radius = (int)(width / 4); // if we set this to 'width / 2' cube is too slo when it gets near the edges
         if (xValue < radius) { return radius; }
         if (xValue > mainWorldParams.width - radius) { return mainWorldParams.width - radius; }
         return xValue;
      }
      
      /// <summary>
      /// ensures that our stored Y values fall within the range of the world 
      /// </summary>
      /// <param name="yValue"></param>
      /// <param name="width"></param>
      /// <returns></returns>
      public static int heightRange(int yValue,double width)
      {
         int radius = (int)(width / 4); // if we set this to 'width / 2' cube is too slo when it gets near the edges
         if (yValue < radius) { return radius; }
         if (yValue > mainWorldParams.height - radius) { return mainWorldParams.height - radius; }
         return yValue;
      }

      private static void ActionReceived(StateObject state)
      {

         // save our state string buffer to a new String
         string actionString = state.sb.ToString();
         string playerName = state.ID;
         // clear out the state buffer
         state.sb.Clear();
         int x = 0;
         int y = 0;

         if (actionString.StartsWith("(move"))
         {
            // 'Groups[1]' is the x coordinate, 'Groups[2]' is the y coordinate
            Regex pattern = new Regex(@"\(move,\s*(\-?\d+),\s*(\-?\d+).*");
            Match match = pattern.Match(actionString);
            // get the width of player cube
            double width = mainWorld.worldCubes[mainWorld.playerCubes[playerName]].Width;
            // save x,y values but ensure that they fall within valide ranges
            x = widthRange(int.Parse(match.Groups[1].Value), width);
            y = heightRange(int.Parse(match.Groups[2].Value), width);
            
            // add to our mousePoints dictionary. should we Lock this ??
            lock (mainWorld)
            {
               mousePoints[playerName] = Tuple.Create(x, y);
            }
            
         }
         else if (actionString.StartsWith("(split"))
         {
            // TODO: repeat code, can combine this with 'move'
            // 'Groups[1]' is the x coordinate, 'Groups[2]' is the y coordinate
            Regex pattern = new Regex(@"\(split,\s*(\-?\d+),\s*(\-?\d+).*");
            Match match = pattern.Match(actionString);
            // get the width of player cube
            double width = mainWorld.worldCubes[mainWorld.playerCubes[playerName]].Width;
            // save x,y values but ensure that they fall within valide ranges
            x = widthRange(int.Parse(match.Groups[1].Value), width);
            y = heightRange(int.Parse(match.Groups[2].Value), width);

            // add to our mousePoints dictionary. should we Lock this ??
            lock (mainWorld)
            {
               splitPoints[playerName] = Tuple.Create(x, y);
               //splitPoints.Add(playerName, Tuple.Create(x, y));
            }

         }

        // Console.WriteLine(actionString + "\nX: " + x + " Y: " + y);

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
