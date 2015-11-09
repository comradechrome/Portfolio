using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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
      public Double Mass

      { get; set; }


      /// <summary>
      /// Width is the square root of the mass. We will drop all decimal values.
      /// Width is a read only property
      /// </summary>
      public Double Width
      {
         get { return Math.Sqrt(Mass); }
      }

      /// <summary>
      /// Radius is half the square root of the mass. We will drop all decimal values.
      /// Radius is a read only property
      /// </summary>
      public Double Radius
      {
         get { return Math.Sqrt(Mass) / 2; }
      }

      /// <summary>
      /// Default contructor of the cube object
      /// </summary>
      /// <param name="loc_x"></param>
      /// <param name="loc_y"></param>
      /// <param name="argb_color"></param>
      /// <param name="uid"></param>
      /// <param name="food"></param>
      /// <param name="name"></param>
      /// <param name="mass"></param>
      public Cube(Double loc_x, Double loc_y, int argb_color, int uid, bool food, String name, Double mass)
      {
         this.loc_x = loc_x;
         this.loc_y = loc_y;
         this.argb_color = argb_color;
         this.uid = uid;
         this.food = food;
         Name = name;
         Mass = mass;
      }
   }

   /// <summary>
   /// Represents 'State' of world. Responsible for tracking:
   /// * the world Width and Height (read only 'constants')
   /// * all the cubes in the game.
   /// </summary>
   public class World
   {
      /// <summary>
      /// 
      /// </summary>
      public int worldHieght { get; }
      /// <summary>
      /// 
      /// </summary>
      public int worldWidth { get; }
      /// <summary>
      /// 
      /// </summary>
      public String playerName { get; }
      /// <summary>
      /// 
      /// </summary>
      public String Server { get; }
      /// <summary>
      /// 
      /// </summary>
      public HashSet<Cube> cubes { get; }


      /// <summary>
      /// 
      /// </summary>
      /// <param name="hieght"></param>
      /// <param name="width"></param>
      /// <param name="name"></param>
      /// <param name="server"></param>
      public World(int hieght, int width, String name, String server)
      {
         this.worldHieght = hieght;
         this.worldWidth = width;
         this.playerName = name;
         this.Server = server;
         cubes = new HashSet<Cube>();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <param name="color"></param>
      /// <param name="uid"></param>
      /// <param name="food"></param>
      /// <param name="name"></param>
      /// <param name="mass"></param>
      public void newCube(Double x, Double y, int color, int uid, bool food, String name, Double mass)
      {
         Cube cube = new Cube(x, y, color, uid, food, name, mass);
         cubes.Add(cube);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="cube"></param>
      public void addCube(Cube cube)
      {
            cubes.Add(cube);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="uid"></param>
      /// <param name="x"></param>
      /// <param name="y"></param>
      /// <param name="mass"></param>
    //  public void moveCube(int uid, Double x, Double y, Double mass)
     // {
         //if (cubes.ContainsKey(uid))
         //{
         //   cubes[uid].Mass = mass;
         //   cubes[uid].loc_x = x;
          //  cubes[uid].loc_y = y;
        // }
         // do nothing if uid doesn't exist -
         // TODO: may need to handle this, but we'll ignore for now 

   //   }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="uid"></param>
      //public void removeCube(int uid)
      //{
      //   if (cubes.ContainsKey(uid))
      //      cubes.Remove(uid);
      //}

   }

}
