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
            heartbeat = new Timer(1000.0 / mainWorldParams.heartbeatsPerSecond);
            // Hook up the Elapsed event for the timer. 
            heartbeat.Elapsed += OnHeartbeat;
            heartbeat.AutoReset = true;
            heartbeat.Enabled = true;

        }
        /// <summary>
        /// 
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
            modifiedCubes.UnionWith(processMoves());
            // process cube splits
            modifiedCubes.UnionWith(processSplits());
            // virus mechanics - append any created or destroyed virus cubes to the json string builder
            modifiedCubes.UnionWith(spawnVirus());
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
                //if (cube == null) continue;
                jsonCubes.Append(JsonConvert.SerializeObject(cube) + "\n");
            }
            sendUpdates(jsonCubes);
            //start the heartbeat back up
            //Console.WriteLine("heartbeat");
            heartbeat.Start();

        }
        /// <summary>
        /// Every heartbeat we'll 'roll the dice' - if we are lucky we'll attempt to create a virus
        /// Chances of creation are determined by the virusProbably parameter
        /// If we find available spot, we create virus (the more empty spaces the higher the probability)
        /// </summary>
        /// <returns></returns>
        private static HashSet<Cube> spawnVirus()
        {
            HashSet<Cube> cubes = new HashSet<Cube>();
            Cube cube;
            lock (mainWorld)
            {
                if (rand.Next(10000) < mainWorldParams.virusProbability * 100 / mainWorldParams.heartbeatsPerSecond)
                {

                    int newRadius = (int)Math.Ceiling(Cube.getWidth(mainWorldParams.virusMass) / 2.0); // round up
                    Tuple<bool, int, int> results = tryPosition(newRadius, false);
                    if (results.Item1)
                    {
                        int green = Color.LawnGreen.ToArgb();
                        cube = new Cube(results.Item2, results.Item3, green, uid++, 0, true, virusName(3), mainWorldParams.virusMass);
                        mainWorld.addCube(cube);
                        mainWorld.virusList.Add(cube.uid);
                        cubes.Add(cube);
                    }
                }
                foreach (var virusID in mainWorld.virusList)
                {
                    mainWorld.worldCubes[virusID].Name = virusName(3);
                    cubes.Add(mainWorld.worldCubes[virusID]);
                }

            }
            return cubes;
        }

        /// <summary>
        /// Generates a random virus name 'length' special characters long
        /// </summary>
        /// <returns></returns>
        private static string virusName(int length)
        {
            var chars = "~@#$%^&*+-:;<>?}{[]|";
            // Name format is 'length' random characters + VIRUS + 'length' random characters (in reverse)
            var stringChars = new char[length * 2 + 1];
            var random = new Random();

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
                stringChars[stringChars.Length - i - 1] = stringChars[i];
            }

            stringChars[length] = 'V';
            //stringChars[length + 1] = 'I';
            //stringChars[length + 2] = 'R';
            //stringChars[length + 3] = 'U';
            //stringChars[length + 4] = 'S';


            var name = new String(stringChars);
            return name;
        }

        private static HashSet<Cube> removeDead()
        {
            HashSet<Cube> deadCubes = new HashSet<Cube>();
            lock (mainWorld)
            {
                // find all cubes with zero mass
                foreach (Cube cube in mainWorld.worldCubes.Values)
                {
                    if (cube.Mass < 1.0)
                        deadCubes.Add(cube);
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
            HashSet<Cube> cubes = new HashSet<Cube>();
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
                cubes.Add(GenerateFoodCube());

            return cubes;
        }

        /// <summary>
        /// At each heartbeat of the game every player cube above the minMass value will lose mass.
        /// Minumum mass is 2 times the minSplitMass
        /// below the acceleratedMass value a player will lose 1% per second with an attritionRate of 200
        /// Above the acceleratedMass value a player will lose 2% per second with an attritionRate of 200
        /// </summary>
        private static void attrition()
        {
            int minMass = 2 * mainWorldParams.minSplitMass;

            if (mainWorld.playerCubes.Count > 0)
            {
                lock (mainWorld)
                {
                    HashSet<Cube> teamCubes = new HashSet<Cube>();


                    foreach (int playerID in mainWorld.playerCubes.Values)
                    {
                        // add player and any split cubes to HashSet
                        teamCubes.Add(mainWorld.worldCubes[playerID]);
                        if (mainWorld.teams.ContainsKey(playerID))
                        {
                            foreach (Cube cube in mainWorld.teams[playerID])
                            {
                                teamCubes.Add(cube);
                            }
                        }

                        foreach (Cube cube in teamCubes)
                        {
                            // decrease mass if cube is above minimum mass - 2%/sec if attrition rate is 200
                            if (cube.Mass > mainWorldParams.acceleratedAttrition)
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
                            // decay the cubes momentum if greater than two, otherwise set it to one
                            double momentum = cube.momentum;
                            if (momentum < 2)
                                cube.momentum = (1);
                            else
                            {
                                momentum -= momentum * mainWorldParams.splitDecayRate / (100 * mainWorldParams.heartbeatsPerSecond);
                                cube.momentum = (momentum);
                            }
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
            Cube foodCube = null;

            int randomFoodID = rand.Next(0, mainWorldParams.foodRandomFactor * mainWorldParams.maxFood);

            lock (mainWorld)
            {
                if (mainWorld.worldCubes.ContainsKey(randomFoodID) && mainWorld.worldCubes[randomFoodID].food)
                {
                    mainWorld.worldCubes[randomFoodID].Mass = mainWorld.worldCubes[randomFoodID].Mass * mainWorldParams.foodGrowthFactor;
                    foodCube = mainWorld.worldCubes[randomFoodID];
                }
            }
            return foodCube;
        }

        private static HashSet<Cube> processMoves()
        {
            HashSet<Cube> teamCubes = new HashSet<Cube>();

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
                            int playerID = mainWorld.playerCubes[playerName];

                            // Add the player main cube to our hashSet
                            teamCubes.Add(mainWorld.worldCubes[playerID]);
                            // iterate through the world and find any cubes with a matching team id
                            foreach (Cube cube in mainWorld.worldCubes.Values)
                            {
                                // Console.WriteLine("team ID: " + cube.Value.team_id + " PlayerID: " + playerID);
                                if (cube.team_id == playerID)
                                {
                                    teamCubes.Add(cube);
                                }

                            }
                            // move our cube and any 'team' cubes if any
                            foreach (Cube cube in teamCubes)
                            {
                                double cubeX = cube.loc_x;
                                double cubeY = cube.loc_y;
                                double mass = cube.Mass;

                                // calculate the distance from our mouse to the cube
                                double distX = x - cubeX;
                                double distY = y - cubeY;

                                // make sure distace is greater than 1
                                double distance = Math.Sqrt(distX * distX + distY * distY);

                                if (distance > 1.0)
                                {
                                    cube.loc_x += distX * smoothingFactor(mass);
                                    cube.loc_y += distY * smoothingFactor(mass);
                                }

                                if (cube.team_id != 0)
                                    foreach (Cube friend in mainWorld.teams[cube.team_id])
                                    {

                                    }
                            }
                        }
                    }
                }
            }
            return teamCubes;
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
            // this adjusts the amount of speed factor
            double scaleConst = mainWorldParams.scaleConst;
            // this adjusts how much the factor affects speed as mass increases
            double smoothingIncrement = (double)mainWorldParams.smoothingIncrement;

            double minFactor = 3 * scaleConst + mainWorldParams.lowSpeed * scaleConst;
            double maxFactor = 3 * scaleConst + mainWorldParams.topSpeed * scaleConst;

            double factor = 3 * mainWorldParams.splitMomentum * scaleConst + mainWorldParams.topSpeed +
                                 ((mainWorldParams.minSplitMass - mass) / smoothingIncrement) * scaleConst;

            if (factor < minFactor) { return minFactor; }
            if (factor > maxFactor) { return maxFactor; }
            return factor;
        }

        /// <summary>
        /// TODO: Split cubes when space bar has been hit. create team_id
        /// TODO: handle cube ID when main cube is eaten and there are additional cubes
        /// TODO: remove player and disconnect socket when last cube has been eaten (maybe handle in absorb mehtod)
        /// TODO: gather statisics for PS9 - Play Time, Max mass, mass at death, etc
        /// </summary>
        private static HashSet<Cube> processSplits()
        {
            HashSet<Cube> updatedCubes = new HashSet<Cube>();
            if (splitPoints.Count > 0)
            {
                lock (mainWorld)
                {
                    foreach (string name in splitPoints.Keys)
                    {
                        Cube originalCube = mainWorld.worldCubes[mainWorld.playerCubes[name]];
                        if (originalCube.team_id == 0)
                        {
                            int teamID = originalCube.uid;
                            mainWorld.teams.Add(teamID, new HashSet<Cube>());
                            splitHelper(teamID, name, originalCube, updatedCubes, splitPoints[name]);
                        }
                        else
                        {
                            foreach (Cube cube in mainWorld.teams[originalCube.team_id].ToArray())
                            {
                                if (cube.Mass > mainWorldParams.minSplitMass)
                                    splitHelper(originalCube.team_id, name, cube, updatedCubes, splitPoints[name]);
                            }
                        }
                    }
                    splitPoints = new Dictionary<string, Tuple<int, int>>();
                }
            }

            return updatedCubes;
        }

        private static void splitHelper(int teamID, string name, Cube splitCube, HashSet<Cube> updatedCubes, Tuple<int, int> pointer)
        {
            // calculate the distance from our mouse to the cube
            double distX = pointer.Item1 - splitCube.loc_x;
            double distY = pointer.Item2 - splitCube.loc_y;

            // make sure distace is greater than 1
            double distance = Math.Sqrt(distX * distX + distY * distY);

            double unitX = distX / distance;
            double unitY = distY / distance;

            double newX = splitCube.loc_x + unitX * splitCube.Width * mainWorldParams.splitDistance;
            double newY = splitCube.loc_y + unitY * splitCube.Width * mainWorldParams.splitDistance;

            double newWidth = Math.Sqrt(splitCube.Mass / 2);
            Cube newCube = new Cube(newX, newY, splitCube.argb_color, uid++, teamID, false, name, splitCube.Mass / 2);
            newCube.momentum = mainWorldParams.splitMomentum;

            newCube.mergeDecay += 10;
            splitCube.mergeDecay += 10;

            splitCube.Mass /= 2;
            //splitCube.setMomentum(mainWorldParams.splitMomentum);
            splitCube.team_id = teamID;
            lock (mainWorld)
            {
                //mainWorld.playerCubes[name] = newCube.uid;
                mainWorld.addCube(newCube);
                mainWorld.teams[teamID].Add(newCube);
                mainWorld.teams[teamID].Add(splitCube);
            }
            updatedCubes.Add(newCube);
            updatedCubes.Add(splitCube);
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
        /// Itterates through all Player and virus cubes.
        /// Detects cube collisions and takes appropriate action. Cubes that need to be updated are added to the 
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
                    // create a list of all Player and virus ID's so we can itterate through them
                    HashSet<int> playersViruses = new HashSet<int>(mainWorld.playerCubes.Values);
                    playersViruses.UnionWith(mainWorld.virusList);
                    foreach (HashSet<Cube> team in mainWorld.teams.Values)
                        foreach (Cube cube in team)
                            playersViruses.Add(cube.uid);

                    foreach (int cubeID in playersViruses)
                    {
                        Cube playerCube = mainWorld.worldCubes[cubeID];

                        // only run loop if player/virus is not dead
                        if (playerCube.Mass > 0)
                        {
                            // get the x,y coordinates of the upper left and lower right of the player/virus cube
                            Tuple<int, int, int, int> playerCorners = playerCube.edges;

                            int playerCubeX1 = playerCorners.Item1;
                            int playerCubeY1 = playerCorners.Item2;
                            int playerCubeX2 = playerCorners.Item3;
                            int playerCubeY2 = playerCorners.Item4;

                            foreach (Cube cube in mainWorld.worldCubes.Values)
                            {
                                // make sure we're not comparing playerCube to itself
                                if (cube.uid != playerCube.uid)
                                {
                                    // get the x,y coordinates of the upper left and lower right of the checked cube
                                    Tuple<int, int, int, int> cubeCorners = cube.edges;
                                    double overlap = cube.Width * mainWorldParams.allowedOverlap;

                                    int cubeX1 = cubeCorners.Item1 + (int)overlap;
                                    int cubeY1 = cubeCorners.Item2 + (int)overlap;
                                    int cubeX2 = cubeCorners.Item3 - (int)overlap;
                                    int cubeY2 = cubeCorners.Item4 - (int)overlap;

                                    // this algorithm checks to see if player cube [(x1,y1),(x2,y2)] overlaps current cube [(x1,y1),(x2,y2)]
                                    // more specifically, it's checking 4 conditions where the cubes cannot overlap - if any are true, the cubes do not overlap

                                    if (!(playerCubeY2 < cubeY1 || playerCubeY1 > cubeY2 ||
                                          playerCubeX2 < cubeX1 || playerCubeX1 > cubeX2))
                                    // cubes overlap; figure out cube type and take appropriate action
                                    {
                                        // check if encountered cube is food
                                        if (cube.food && cube.Name == "")
                                        {
                                            playerCube.Mass += cube.Mass;
                                            cube.Mass = 0;
                                            cubeUpdates.Add(cube);
                                        }
                                        // check if encountered cube is a virus
                                        else if (cube.food)
                                        {
                                            // remove virus and add player to infected HashSet
                                            cube.Mass = 0;
                                            infectedCubes.Add(playerCube);
                                        }
                                        else if (cube.team_id != 0)
                                        {
                                            if (playerCubeY2 > cubeY1)
                                            {
                                                playerCube.loc_y += 1;
                                                merge(playerCube, cube);
                                            }
                                            if (playerCubeY1 < cubeY2)
                                            {
                                                playerCube.loc_y -= 1;
                                                merge(playerCube, cube);
                                            }

                                            if (playerCubeX2 > cubeX1)
                                            {
                                                playerCube.loc_x += 1;
                                                merge(playerCube, cube);
                                            }

                                            if (playerCubeX1 < cubeX2)
                                            {
                                                playerCube.loc_x -= 1;
                                                merge(playerCube, cube);
                                            }
                                        }
                                        // cube mass is greater than player so we remove player cube (don't check if we're a virus)
                                        else if (cube.Mass > playerCube.Mass && !playerCube.food)
                                        {
                                            cube.Mass += playerCube.Mass;
                                            killPlayer(playerCube);
                                            playerCube.Mass = 0;
                                        }
                                        else if (!playerCube.food)
                                        // cube is smaller (or equal) than the player cube so we will remove the cube (and we're not a virus)
                                        {
                                            playerCube.Mass += cube.Mass;
                                            killPlayer(cube);
                                            cube.Mass = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    processInfected(infectedCubes);
                }
            }
        }

        private static void merge(Cube cube1, Cube cube2)
        {
            if (cube1.mergeDecay > 0)
                cube1.mergeDecay--;
            else
            {
                if (cube1.uid == cube1.team_id)
                {
                    cube1.Mass += cube2.Mass;
                    cube2.Mass = 0;
                }
                else
                {
                    cube2.Mass += cube1.Mass;
                    cube1.Mass = 0;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="infectedCubes"></param>
        private static void processInfected(HashSet<Cube> infectedCubes)
        {
            foreach (Cube cube in infectedCubes)
            {

                Console.WriteLine(cube.Name + " was infected!!");
            }
        }

        private static void killPlayer(Cube cube)
        {
            // TODO: once we have figured out splitting, we need to add logic here to figure out if this is last of the players cubes - close socket connection
            // TODO: just as the case above, we need method to determine if this is the last of a players cubes - close socket connection

            if (cube.team_id == 0 || cube.team_id == cube.uid)
            {
                // Network.Send(clientStates[cube.Name].workSocket,"");
                //  Network.Stop(clientStates[cube.Name].workSocket);
            }

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
                    foreach (int playerID in mainWorld.playerCubes.Values)
                    {
                        playerCubes.Add(mainWorld.worldCubes[playerID]);
                        if (mainWorld.teams.ContainsKey(playerID))
                        {
                            foreach (Cube cube in mainWorld.teams[playerID])
                            {
                                playerCubes.Add(cube);
                            }
                        }
                        playerCubes.Add(mainWorld.worldCubes[playerID]);



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

        }

        private static string GeneratePlayerCube(string playerName)
        {
            double width = Cube.getWidth(mainWorldParams.playerStartMass);
            int radius = (int)Math.Ceiling(width / 2.0); // round up
            Cube playerCube = new Cube(rand.Next(radius, mainWorldParams.height - radius), rand.Next(radius, mainWorldParams.width - radius),
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
        public static Tuple<double, double> availablePosition(double mass)
        {
            double width = Cube.getWidth(mass);
            int x = 0;
            int y = 0;
            bool available = false;
            int newRadius = (int)Math.Ceiling(width / 2.0); // round up



            while (!available)
            {
                Tuple<bool, int, int> results = tryPosition(newRadius, true);
                available = results.Item1;
                x = results.Item2;
                y = results.Item3;
            }

            return Tuple.Create((double)x, (double)y);
        }

        /// <summary>
        /// generates a random x,y coordinate. With a supplied radius, checks if there is an overlap.
        /// foodFlag determines if we check for overlapping food or not. True: check food, False: only check players
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="foodFlag"></param>
        /// <returns></returns>
        public static Tuple<bool, int, int> tryPosition(int radius, bool foodFlag)
        {
            // generate random coordiantes that fall withing the world. Then generate the cube diagonal points.
            int x = rand.Next(radius, mainWorldParams.height - radius);
            int y = rand.Next(radius, mainWorldParams.width - radius);
            bool available = true;

            int x1 = x - radius;
            int y1 = y - radius;
            int x2 = x + radius;
            int y2 = y + radius;

            if (mainWorld.worldCubes.Count > 0)
            {

                // we have cubes so we'll need to check for overlaps
                lock (mainWorld)
                {
                    foreach (var cube in mainWorld.worldCubes)
                    {
                        // don't check food cubes
                        if (!foodFlag)
                        {
                            if (cube.Value.food)
                                break;
                        }

                        Tuple<int, int, int, int> corners = cube.Value.edges;

                        int x3 = corners.Item1;
                        int y3 = corners.Item2;
                        int x4 = corners.Item3;
                        int y4 = corners.Item4;

                        // this algorithm checks to see if new cube [(x1,y1),(x2,y2)] overlaps current cube [(x3,y3),(x4,y4)]
                        // more specifically, it's checking 4 conditions where the cubes cannot overlap - if any are true, the cubes do not overlap

                        if (!(y2 < y3 || y1 > y4 || x2 < x3 || x1 > x4))
                        // cubes overlap, set flag to false and break out of loop
                        {
                            available = false;
                            break;
                        }
                    }
                }
            }
            return Tuple.Create(available, x, y);
        }

        /// <summary>
        /// ensures that our stored X values fall within the range of the world 
        /// </summary>
        /// <param name="xValue"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static int widthRange(int xValue, double width)
        {
            int radius = (int)(width / 4); // if we set this to 'width / 2' cube is too slow when it gets near the edges
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
        public static int heightRange(int yValue, double width)
        {
            int radius = (int)(width / 4); // if we set this to 'width / 2' cube is too slow when it gets near the edges
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
            if (mainWorld.playerCubes.ContainsKey(playerName))
            {
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
                        Console.WriteLine(playerName + splitPoints[playerName].ToString());
                    }
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