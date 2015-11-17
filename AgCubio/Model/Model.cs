using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;


namespace AgCubio
{
    /// <summary>
    /// A cube contains:
    /// * A unique id
    /// * The position in space(x, y)
    /// * A color
    /// * A name -- if this is a player cube
    /// * A mass
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
        /// true = Food
        /// false = Player
        /// </summary>
        public bool food { get; }

        /// <summary>
        /// The name on the cube - if food, the name will be an empty string
        /// </summary>
        public String Name { get; }


        /// <summary>
        /// mass of the cube - if set attempts a value less than 1, we'll default to 1
        /// </summary>
        public Double Mass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int team_id { get; set; }

        /// <summary>
        /// Width is the square root of the mass. We will drop all decimal values.
        /// Width is a read only property
        /// </summary>
        public Double Width
        {
            get { return Math.Sqrt(Mass); }
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
    /// </summary>
    public class World
    {
        /// <summary>
        /// 
        /// </summary>
        public int worldHieght { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int worldWidth { get; set; }
        /// <summary>
        /// Our players UID
        /// </summary>
        public int ourID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, Cube> worldCubes { get; }
        /// <summary>
        /// List of our worldCubes - 
        /// </summary>
        public Dictionary<int, Cube> ourCubes { get; }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="hieght"></param>
        /// <param name="width"></param>
        public World(int hieght, int width)
        {
            this.worldHieght = hieght;
            this.worldWidth = width;
            worldCubes = new Dictionary<int, Cube>();
            ourCubes = new Dictionary<int, Cube>();
        }

        /// <summary>
        /// Add worldCubes to our cube dictionary. Key is the cube UID. Also check the cube team ID.
        /// IF the team ID is the same as our UID, add it to the ourCubes HashSet
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
            else if (cube.team_id == ourID)
                ourCubes.Add(cube.uid, cube);

            worldCubes.Add(cube.uid, cube);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="cube"></param>
        public void moveCube(Cube cube)
        {
            if (worldCubes.ContainsKey(cube.uid))
            {
                worldCubes[cube.uid].Mass = cube.Mass;
                worldCubes[cube.uid].loc_x = cube.loc_x;
                worldCubes[cube.uid].loc_y = cube.loc_y;
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
        /// 
        /// </summary>
        /// <param name="cube"></param>
        public void removeCube(Cube cube)
        {
            if (worldCubes.ContainsKey(cube.uid))
                worldCubes.Remove(cube.uid);
            if (ourCubes.ContainsKey(cube.uid))
                ourCubes.Remove(cube.uid);
        }

        /// <summary>
        /// 
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
                    // check if our mass is zero - end game if true
                    if (cube.uid == ourID)
                    {
                        //?
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
        /// <returns>Tuple</returns>
        public Tuple<Double, Double, Double> getOurInfo()
        {

            List<Double> xValues = new List<Double>();
            List<Double> yValues = new List<Double>();
            Double x, y, width, height;

            foreach (KeyValuePair<int, Cube> cube in ourCubes)
            {
                xValues.Add(cube.Value.loc_x + cube.Value.Width / 2);
                yValues.Add(cube.Value.loc_y + cube.Value.Width / 2);
            }

            x = xValues.Average() - ourCubes[ourID].Width;
            y = yValues.Average() - ourCubes[ourID].Width;

            width = (xValues.Max() - xValues.Min() + ourCubes[ourID].Width);
            height = (yValues.Max() - yValues.Min() + ourCubes[ourID].Width);

            // return the average x, y , and max width of our cube(s) 
            return Tuple.Create(x, y, Math.Max(width, height));
        }


    }

}
