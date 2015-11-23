using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AgCubio
{
    /// <summary>
    /// Represents the AgCubio world. Contains cubes, player cube 
    /// and whether or not the player is dead
    /// </summary>
    public class World
    {
        /// <summary>
        /// All cubes in the world
        /// </summary>
        public HashSet<Cube> cubes { get; private set; }
        /// <summary>
        /// Represents this clients player cube
        /// </summary>
        public Cube player { get; private set; }
        /// <summary>
        /// Whether the player has died or not
        /// </summary>
        public bool playerDead { get; private set; } = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        public World()
        {
            cubes = new HashSet<Cube>();
        }

        /// <summary>
        /// Converts a json string stream to objects
        /// </summary>
        /// <param name="json"></param>
        public void JsonToCubes(string json)
        {
            string[] jsonObjects = json.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string obj in jsonObjects)
            {
                Cube cube = JsonConvert.DeserializeObject<Cube>(obj);
                if (!cube.isFood)   //Player cube
                {
                    //Delete old cube and redraw
                    DeleteCube(cube);
                    AddCube(cube);
                    
                }
                if (cube.mass != 0)
                    AddCube(cube);
                else
                {
                    DeleteCube(cube);
                    if (cube.Equals(player)) // find out if player is dead
                        playerDead = true;
                }

            }
        }

        /// <summary>
        /// Adds cube to the world
        /// </summary>
        /// <param name="cube">Cube to add</param>
        private void AddCube(Cube cube)
        {
            cubes.Add(cube);
        }

        /// <summary>
        /// Deletes cube from the world
        /// </summary>
        /// <param name="cube">Cube to delete</param>
        private void DeleteCube(Cube cube)
        {
            cubes.Remove(cube);
        }

        /// <summary>
        /// Sets the client player cube
        /// </summary>
        /// <param name="cube">The player cube to use</param>
        public void SetPlayer(Cube cube)
        {
            player = cube;
        }
    }
}
