using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;
using System.Timers;
using System.Xml;
using Microsoft.SqlServer.Server;

namespace AgCubio
{
   /// <summary>
   /// A cube contains:
   /// * A unique id
   /// * The position in space(x, y)
   /// * A color
   /// * A name -- if this is a player cube
   /// * A mass
   /// * A team ID (if the cube has been split)
   /// * Food status (is this cube food or a player)
   /// 
   /// These should be derived properties based on mass:
   /// * Width, Top, Left, Right, Bottom
   /// </summary>
   public class Cube
   {
      /// <summary>
      /// x coordinate of the center of the cube
      /// </summary>
      public Double loc_x { get; set; }

      /// <summary>
      /// y coordinate of the center of the cube
      /// </summary>
      public Double loc_y { get; set; }

      /// <summary>
      /// integer representation of the ARGB color value
      /// </summary>
      public int argb_color { get; }

      /// <summary>
      /// Cube unique ID
      /// </summary>
      public int uid { get; }

      /// <summary>
      /// true = Food (or if it has a name, it's a virus)
      /// false = Player
      /// </summary>
      public bool food { get; }

      /// <summary>
      /// The name on the cube - if food, the name will be an empty string
      /// </summary>
      public String Name { get; set; }


      /// <summary>
      /// mass of the cube - if set attempts a value less than 1, we'll default to 1
      /// </summary>
      public Double Mass { get; set; }

      /// <summary>
      /// If cube has been split, this will be the origial cube ID on all split cube instances
      /// </summary>
      public int team_id { get; set; }

      /// <summary>
      /// On splits, the momentum will be > 0 and cause the cube to move faster. Default value is 0
      /// </summary>
      private int momentum { get; set; } = 0;

      /// <summary>
      /// Width is  mass ^ .65 which gets us close to the supplied client
      /// Width is a read only property
      /// </summary>
      public Double Width
      {
         get { return getWidth(Mass); }
      }

      /// <summary>
      /// returns the x,y coordinates of the top left and bottom right of the cube (in that order)
      /// x1,y1 are the top left and x2,y2 are the bottom right
      /// </summary>
      public Tuple<int, int, int, int> corners
      {
         get
         {
            int x1 = (int)(loc_x - Math.Ceiling(Width / 2.0)); // round down to the smallest integer
            int y1 = (int)(loc_y - Math.Ceiling(Width / 2.0)); // round down to the smallest integer
            int x2 = (int)(loc_x + Math.Ceiling(Width / 2.0)); // round up to the largest integer
            int y2 = (int)(loc_y + Math.Ceiling(Width / 2.0)); // round up to the largest integer

            return Tuple.Create(x1, y1, x2, y2);
}
      }



      /// <summary>
      /// Overloaded equals method
      /// </summary>
      /// <param name="obj"></param>
      /// <returns></returns>
      public override bool Equals(object obj)
      {
         
         if (obj == null)
            return false;

         Cube cube = obj as Cube;

            if ((object)cube == null)
            return false;

         if (this.uid == cube.uid)
            return true;
         else
            return false;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="c1"></param>
      /// <param name="c2"></param>
      /// <returns></returns>
      public static bool operator ==(Cube c1, Cube c2)
      {
         if (c1 == null && c2 == null)
            return true;
         else
            return Equals(c1.uid, c2.uid);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="c1"></param>
      /// <param name="c2"></param>
      /// <returns></returns>
      public static bool operator !=(Cube c1, Cube c2)
      {
         return !(c1 == c2);
      }

      /// <summary>
      /// HashCode based on unique ID
      /// </summary>
      /// <returns></returns>
      public override int GetHashCode()
      {
         return this.uid.GetHashCode();
      }

      /// <summary>
      /// Calculates the width of a cube
      /// </summary>
      /// <param name="mass"></param>
      /// <returns></returns>
      public static double getWidth(double mass)
      {
         return Math.Sqrt(mass);
      }

      /// <summary>
      /// Gets the momentum of a cube
      /// </summary>
      /// <returns></returns>
      public int getMomentum()
      {
         return this.momentum;
      }
      /// <summary>
      /// Sets the momentum of a cube
      /// </summary>
      /// <param name="momentum"></param>
      public void setMomentum(int momentum)
      {
         this.momentum = momentum;
      }


      /// <summary>
      /// Default contructor of the cube object
      /// </summary>
      /// <param name="loc_x"></param>
      /// <param name="loc_y"></param>
      /// <param name="argb_color"></param>
      /// <param name="uid"></param>
      /// <param name="team_id"></param>
      /// <param name="food"></param>
      /// <param name="name"></param>
      /// <param name="mass"></param>
      public Cube(Double loc_x, Double loc_y, int argb_color, int uid, int team_id, bool food, String name, Double mass)
      {
         this.loc_x = loc_x;
         this.loc_y = loc_y;
         this.argb_color = argb_color;
         this.uid = uid;
         this.team_id = team_id;
         this.food = food;
         this.Name = name;
         this.Mass = mass;
      }
   }

   /// <summary>
   /// Represents 'State' of world. Responsible for tracking:
   /// * the world Width and Height (read only 'constants')
   /// * all the worldCubes in the game.
   /// * all split cubes for our player
   /// </summary>
   public class World
   {
      /// <summary>
      /// total height of the world
      /// </summary>
      public int worldHeight { get; set; }
      /// <summary>
      /// total width of the world
      /// </summary>
      public int worldWidth { get; set; }
      /// <summary>
      /// Our players UID
      /// </summary>
      public int ourID { get; set; }
      /// <summary>
      /// Dictionary of all the worlds cubes. Index by Cube ID
      /// </summary>
      public Dictionary<int, Cube> worldCubes { get; }
      /// <summary>
      /// Dictionary of our split cubes - will always contain our initial cube - used by the client
      /// As cube splits, it will be indexed by Cube_id, but all team_id's should be our original UID
      /// </summary>
      public Dictionary<int, Cube> ourCubes { get; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, HashSet<Cube>> teams { get; }

      /// <summary>
        /// Dictionary of player cubes used by the server. Key: PlayerName, Value: Player ID
      /// </summary>
        public Dictionary<string, int> playerCubes { get; }

      public HashSet<int> virusList { get; set; } 



      /// <summary>
      /// Default contructor for our World object
      /// </summary>
      /// <param name="hieght"></param>
      /// <param name="width"></param>
      public World(int hieght, int width)
      {
         this.worldHeight = hieght;
         this.worldWidth = width;
         worldCubes = new Dictionary<int, Cube>();
         ourCubes = new Dictionary<int, Cube>();
         playerCubes = new Dictionary<string, int>();
         virusList = new HashSet<int>();
            teams = new Dictionary<int, HashSet<Cube>>();
      }

      /// <summary>
      /// Add worldCubes to our cube dictionary. Key is the cube UID. Also check the cube team ID.
      /// If the team ID is the same as our UID, add it to the ourCubes Dictionary
      /// </summary>
      /// <param name="cube"></param>
      public void addCube(Cube cube)
      {
         if (worldCubes.Count == 0)
         {
            ourID = cube.uid;
            ourCubes.Add(cube.uid, cube);
         }
         // add our split cubes to ourCubes dictionary


         worldCubes.Add(cube.uid, cube);
      }



      /// <summary>
      /// move the position and weight of a cube
      /// </summary>
      /// <param name="cube"></param>
      public void moveCube(Cube cube)
      {
         if (worldCubes.ContainsKey(cube.uid))
         {
            worldCubes[cube.uid].Mass = cube.Mass;
            worldCubes[cube.uid].loc_x = cube.loc_x;
            worldCubes[cube.uid].loc_y = cube.loc_y;
            worldCubes[cube.uid].Name = cube.Name;
         }
         // check if ourCubes contains cube, if so modify it
         if (ourCubes.ContainsKey(cube.uid))
         {
            ourCubes[cube.uid].Mass = cube.Mass;
            ourCubes[cube.uid].loc_x = cube.loc_x;
            ourCubes[cube.uid].loc_y = cube.loc_y;
         }

         // do nothing if uid doesn't exist -
      }

      /// <summary>
      /// remove a cube from the world object
      /// </summary>
      /// <param name="cube"></param>
      public void removeCube(Cube cube)
      {
         if (worldCubes.ContainsKey(cube.uid))
            worldCubes.Remove(cube.uid);
         if (ourCubes.ContainsKey(cube.uid))
            ourCubes.Remove(cube.uid);
         if (playerCubes.ContainsKey(cube.Name))
            playerCubes.Remove(cube.Name);
         if (virusList.Contains(cube.uid))
            virusList.Remove(cube.uid);
      }

      /// <summary>
      /// Main worker method for our world object. Processing begins by converting a JSON string to a cube object.
      /// Check mass - if zero delete cube. (we have already cecked if our cube is mass=zero in the view as this will end the game).
      /// If the cube has mass and already eists, we move ... otherwise we add the cube to the world.
      /// </summary>
      /// <param name="jsonCube"></param>
      public void processCube(String jsonCube)
      {

         Cube cube = JsonConvert.DeserializeObject<Cube>(jsonCube);
         // check if cube exists
         if (worldCubes.ContainsKey(cube.uid))
         {
            // check if mass equals zero
            if (cube.Mass == 0.0)
            {
               // check if our mass is zero - end game if true (check is done in the view)
               if (cube.uid == ourID)
               {
                  moveCube(cube);
               }
               else
                  removeCube(cube);
            }
            // cube exists and is not zero mass
            else
               moveCube(cube);
         }
         // cube doesn't exist so we will add it
         else
         {
            // add the cube
            addCube(cube);
         }
      }

      /// <summary>
      /// Determines the average x,y coordiantes of our cube(s)
      /// Determines the maximum of our cube(s) height and width
      /// These three values are returned as a Tuple
      /// </summary>
      /// <returns>Tuple of the average x, y, and max width of our cube(s) </returns>
      public Tuple<Double, Double, Double> getOurCubesAverage()
      {
         List<Double> xValues = new List<Double>();
         List<Double> yValues = new List<Double>();
         Double x, y, width, height;

         foreach (Cube cube in ourCubes.Values)
         {
            // get the cube centers and add to X and Y lists
            xValues.Add(cube.loc_x + cube.Width / 2);
            yValues.Add(cube.loc_y + cube.Width / 2);
         }

         x = xValues.Average() - ourCubes[ourID].Width;
         y = yValues.Average() - ourCubes[ourID].Width;

         width = (xValues.Max() - xValues.Min() + ourCubes[ourID].Width);
         height = (yValues.Max() - yValues.Min() + ourCubes[ourID].Width);

         return Tuple.Create(x, y, Math.Max(width, height));
      }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="splitPoints"></param>
        //public void splitPlayer(Dictionary<string, Tuple<int, int>> splitPoints)
        //{
        //    foreach(string name in splitPoints.Keys)
        //    {

        //    }
        //}
    }

   /// <summary>
   /// 
   /// </summary>
   public class WorldParams
   {
      /// <summary>
      /// 
      /// </summary>
      public int width { get; } = 1000;
      /// <summary>
      /// 
      /// </summary>
      public int height { get; } = 1000;
      /// <summary>
      /// 
      /// </summary>
      public int maxSplitDistance { get; } = 150;
      /// <summary>
      /// 
      /// </summary>
      public int topSpeed { get; } = 5;
      /// <summary>
      /// 
      /// </summary>
      public int lowSpeed { get; } = 1;
      /// <summary>
      /// 
      /// </summary>
      public int attritionRate { get; } = 200;
      /// <summary>
      /// 
      /// </summary>
      public int foodValue { get; } = 1;
      /// <summary>
      /// 
      /// </summary>
      public int playerStartMass { get; } = 1000;
      /// <summary>
      /// 
      /// </summary>
      public int maxFood { get; } = 5000;
      /// <summary>
      /// 
      /// </summary>
      public int minSplitMass { get; } = 100;
      /// <summary>
      /// 
      /// </summary>
      public double absorbConstant { get; } = 1.25;
      /// <summary>
      /// 
      /// </summary>
      public int maxViewRange { get; } = 10000;
      /// <summary>
      /// 
      /// </summary>
      public int heartbeatsPerSecond { get; } = 25;
      /// <summary>
      /// The mass of a virus
      /// </summary>
      public int virusMass { get; } = 800;
      /// <summary>
      /// percent per second chance that a virus will be generated
      /// </summary>
      public int virusProbability { get; } = 5;

      public int acceleratedAttrition { get; } = 800;
      public int foodRandomFactor { get; } = 100;
      public int foodGrowthFactor { get; } = 5;
      /// <summary>
      /// the percentage of cube overlap allowed before a cube can be 'eaten'
      /// </summary>
      public double allowedOverlap { get; } = .25;

      /// <summary>
      /// this adjusts the amount of speed factor
      /// </summary>
      public double scaleConst { get; } = .00125;

      /// <summary>
      /// this adjusts how much the factor affects speed as mass increases
      /// </summary>
      public int smoothingIncrement { get; } = 1500;
      public double splitDistance { get; } = 1.5;
      public int splitDecayRate { get; } = 10;
      public int splitMomentum { get; } = 10;

      /// <summary>
      /// Default World Parameters construtor
      /// </summary>
      /// <param name="width"></param>
      /// <param name="height"></param>
      /// <param name="maxSplitDistance"></param>
      /// <param name="topSpeed"></param>
      /// <param name="lowSpeed"></param>
      /// <param name="attritionRate"></param>
      /// <param name="foodValue"></param>
      /// <param name="playerStartMass"></param>
      /// <param name="maxFood"></param>
      /// <param name="minSplitMass"></param>
      /// <param name="absorbConstant"></param>
      /// <param name="maxViewRange"></param>
      /// <param name="heartbeatsPerSecond"></param>
      //public WorldParams(int width, int height, int maxSplitDistance, int topSpeed, int lowSpeed,
      //   int attritionRate, int foodValue, int playerStartMass, int maxFood, int minSplitMass,
      //   double absorbConstant, int maxViewRange, int heartbeatsPerSecond) : base()
      //{
      //   this.width = width;
      //   this.height = height;
      //   this.maxSplitDistance = maxSplitDistance;
      //   this.topSpeed = topSpeed;
      //   this.lowSpeed = lowSpeed;
      //   this.attritionRate = attritionRate;
      //   this.foodValue = foodValue;
      //   this.playerStartMass = playerStartMass;
      //   this.maxFood = maxFood;
      //   this.minSplitMass = minSplitMass;
      //   this.absorbConstant = absorbConstant;
      //   this.maxViewRange = maxViewRange;
      //   this.heartbeatsPerSecond = heartbeatsPerSecond;
      //}

      /// <summary>
      /// Contructor that uses XML Properties file
      /// </summary>
      /// <param name="paramFile"></param>
      public WorldParams(string paramFile)
      {
         try
         {
            using (XmlReader reader = XmlReader.Create(paramFile))
            {
               while (reader.Read())
               {
                  if (reader.IsStartElement())
                  {
                     switch (reader.Name)
                     {
                        case "width":
                           reader.Read();
                           this.width = reader.ReadContentAsInt();
                           break;
                        case "height":
                           reader.Read();
                           this.height = reader.ReadContentAsInt();
                           break;
                        case "max_split_distance":
                           reader.Read();
                           this.maxSplitDistance = reader.ReadContentAsInt();
                           break;
                        case "top_speed":
                           reader.Read();
                           this.topSpeed = reader.ReadContentAsInt();
                           break;
                        case "low_speed":
                           reader.Read();
                           this.lowSpeed = reader.ReadContentAsInt();
                           break;
                        case "attrition_rate":
                           reader.Read();
                           this.attritionRate = reader.ReadContentAsInt();
                           break;
                        case "food_value":
                           reader.Read();
                           this.foodValue = reader.ReadContentAsInt();
                           break;
                        case "player_start_mass":
                           reader.Read();
                           this.playerStartMass = reader.ReadContentAsInt();
                           break;
                        case "max_food":
                           reader.Read();
                           this.maxFood = reader.ReadContentAsInt();
                           break;
                        case "min_split_mass":
                           reader.Read();
                           this.minSplitMass = reader.ReadContentAsInt();
                           break;
                        case "absorb_constant":
                           reader.Read();
                           this.absorbConstant = reader.ReadContentAsDouble();
                           break;
                        case "max_view_range":
                           reader.Read();
                           this.maxViewRange = reader.ReadContentAsInt();
                           break;
                        case "heartbeats_per_second":
                           reader.Read();
                           this.heartbeatsPerSecond = reader.ReadContentAsInt();
                           break;
                        case "virus_mass":
                           reader.Read();
                           this.virusMass = reader.ReadContentAsInt();
                           break;
                        case "virus_probability":
                           reader.Read();
                           this.virusProbability = reader.ReadContentAsInt();
                           break;
                        case "accelerated_attrition":
                           reader.Read();
                           this.acceleratedAttrition = reader.ReadContentAsInt();
                           break;
                        case "food_random_factor":
                           reader.Read();
                           this.foodRandomFactor = reader.ReadContentAsInt();
                           break;
                        case "food_growth_factor":
                           reader.Read();
                           this.foodGrowthFactor = reader.ReadContentAsInt();
                           break;
                        case "allowed_overlap":
                           reader.Read();
                           this.allowedOverlap = reader.ReadContentAsInt();
                           break;
                        case "scale_constant":
                           reader.Read();
                           this.scaleConst = reader.ReadContentAsInt();
                           break;
                        case "smoothing_increment":
                           reader.Read();
                           this.smoothingIncrement = reader.ReadContentAsInt();
                           break;
                        case "split_distance":
                           reader.Read();
                           this.splitDistance = reader.ReadContentAsInt();
                           break;
                        case "split_decay_rate":
                           reader.Read();
                           this.splitDecayRate = reader.ReadContentAsInt();
                           break;
                        case "split_momentum":
                           reader.Read();
                           this.splitMomentum = reader.ReadContentAsInt();
                           break;
                        default:
                           reader.Read();
                           break;
                     }
                  }
               }
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine("Error reading paramerter file; Using default values.\n Error: " + ex);
         }
      }

      /// <summary>
      /// Contructor that uses default property values
      /// </summary>
      public WorldParams()
      {
      }

   }


}
