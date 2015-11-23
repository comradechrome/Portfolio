using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace AgCubio
{
    /// <summary>
    /// Represents an AgCube
    /// </summary>
    public class Cube
    {
        /// <summary>
        /// Unique cube id
        /// </summary>
        public long id { get; private set; }
        /// <summary>
        /// Team id (for when player splits)
        /// </summary>
        public long teamid { get; private set; }
        /// <summary>
        /// X position of cube
        /// </summary>
        public int x { get; private set; }
        /// <summary>
        /// Y position of cube
        /// </summary>
        public int y { get; private set; }
        /// <summary>
        /// Cube color
        /// </summary>
        public int color { get; private set; }
        /// <summary>
        /// Cube name (if its a player cube)
        /// </summary>
        public string name { get; private set; }
        /// <summary>
        /// Mass of the cube
        /// </summary>
        public int mass { get; private set; }
        /// <summary>
        /// Whether or not this is a food cube
        /// </summary>
        public bool isFood { get; private set; }
        /// <summary>
        /// Width of cube
        /// </summary>
        public int width { get; private set; }

        /// <summary>
        /// Cube constructor
        /// </summary>
        /// <param name="loc_x">X position</param>
        /// <param name="loc_y">Y position</param>
        /// <param name="argb_color">Cube color</param>
        /// <param name="uid">Unique cube ID</param>
        /// <param name="team_id">Team ID</param>
        /// <param name="food">Whether this is a food cube</param>
        /// <param name="Name">Name of cube (if player cube)</param>
        /// <param name="Mass">Mass of the cube</param>
        public Cube(double loc_x, double loc_y, int argb_color, long uid, long team_id, bool food, string Name, double Mass)
        {
            color = argb_color;
            id = uid;
            teamid = team_id;
            isFood = food;
            name = Name;
            mass = (int)Mass;

            //width = (int)Math.Sqrt(this.mass);
            width = (int)Math.Pow(mass,0.65);
            x = (int)loc_x - (width/2);
            y = (int)loc_y - (width/2);
        }

        /// <summary>
        /// String representation of cube
        /// </summary>
        /// <returns></returns>
        //public override string ToString()
        //{
        //    return "Name: " + name + " ID: " + id + " Location: (" + x + "," + y + ") " + " isFood: " + isFood + " Mass: " + mass + " Width: " + width;
        //}

        /// <summary>
        /// Overloaded equals method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Cube cube = obj as Cube;

            if (obj == null)
                return false;

            if (this.id == cube.id)
                return true;
            else
                return false;
        }

        /// <summary>
        /// HashCode based on unique ID
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)this.id;
        }
    }
}
