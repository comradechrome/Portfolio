using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgCubio
{
   [TestClass]
   public class AgCubioTest
   {
      [TestMethod]
      public void createCube()
      {
         Cube cube = new Cube(1, 1, -124342, 123, 124, false, "cube1", 400);
         
         Assert.AreEqual(1, cube.loc_x);
         Assert.AreEqual(1, cube.loc_y);
         Assert.AreEqual(-124342, cube.argb_color);
         Assert.AreEqual(124, cube.team_id);
         Assert.AreEqual(123, cube.uid);
         Assert.IsFalse(cube.food);
         Assert.AreEqual("cube1", cube.Name);
         Assert.AreEqual(400, cube.Mass);
         Assert.AreEqual(49, (int)cube.Width);

      }

      [TestMethod]
      public void createWorld()
      {
         World world = new World(100, 200);
         world.ourID = 123;
         Assert.AreEqual(100, world.worldHeight);
         Assert.AreEqual(200, world.worldWidth);
         Assert.AreEqual(123, world.ourID);
      }

      [TestMethod]
      public void addCube()
      {
         World world = new World(100, 200);
         Cube cube1 = new Cube(1, 1, -124342, 123, 0, false, "cube1", 400);
         Cube cube2 = new Cube(1, 1, -124342, 125, 123, false, "cube2", 400);
         world.addCube(cube1);
         world.addCube(cube2);

         Assert.AreEqual(0,world.worldCubes[123].team_id);
         Assert.AreEqual(123, world.ourCubes[125].team_id);
         Assert.AreEqual(2, world.ourCubes.Count);


      }

      [TestMethod]
      public void moveCube()
      {
         World world = new World(100, 200);
         Cube cube1 = new Cube(1, 1, -124342, 123, 0, false, "cube1", 400);
         Cube cube2 = new Cube(1, 1, -124342, 125, 123, false, "cube2", 400);
         world.addCube(cube1);
         world.addCube(cube2);
         Cube cube3 = new Cube(2, 3, -124342, 125, 123, false, "cube2", 500);
         world.moveCube(cube3);

         Assert.AreEqual(2, world.worldCubes[125].loc_x);
         Assert.AreEqual(3, world.worldCubes[125].loc_y);
         Assert.AreEqual(500, world.worldCubes[125].Mass);
         Assert.AreEqual(3, world.ourCubes[125].loc_y);
         Assert.AreEqual(2, world.ourCubes[125].loc_x);
         Assert.AreEqual(500, world.ourCubes[125].Mass);
         Assert.AreEqual(2, world.ourCubes.Count);

      }

      [TestMethod]
      public void removeCube()
      {
         World world = new World(100, 200);
         Cube cube1 = new Cube(1, 1, -124342, 123, 0, false, "cube1", 400);
         Cube cube2 = new Cube(1, 1, -124342, 125, 123, false, "cube2", 400);
         world.addCube(cube1);
         world.addCube(cube2);
         world.removeCube(cube2);

         Assert.AreEqual(1, world.ourCubes.Count);


      }

      [TestMethod]
      public void processCube()
      {
         World world = new World(100, 200);
         world.processCube("{ \"loc_x\":926.0,\"loc_y\":682.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":0,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":1000.0}");
         world.processCube("{ \"loc_x\":300.0,\"loc_y\":40.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":0,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":2000.0}");
         world.processCube("{ \"loc_x\":96.0,\"loc_y\":82.0,\"argb_color\":-45536,\"uid\":5,\"team_id\":0,\"food\":true,\"Name\":\"\",\"Mass\":1.0}");
         Assert.AreEqual(2, world.worldCubes.Count);
         Assert.AreEqual(1, world.ourCubes.Count);

         world.processCube("{ \"loc_x\":9.0,\"loc_y\":2.0,\"argb_color\":-45536,\"uid\":5,\"team_id\":0,\"food\":true,\"Name\":\"\",\"Mass\":0.0}");
         world.processCube("{ \"loc_x\":6.0,\"loc_y\":82.0,\"argb_color\":-65536,\"uid\":5571,\"team_id\":0,\"food\":false,\"Name\":\"3500 is love\",\"Mass\":0.0}");

         Assert.AreEqual(1, world.worldCubes.Count);
         Assert.AreEqual(1, world.ourCubes.Count);
      }

      [TestMethod]
      public void getAverage()
      {
         World world = new World(100, 200);
         Cube cube1 = new Cube(10, 10, -124342, 123, 0, false, "cube1", 34);
         Cube cube2 = new Cube(20, 20, -124342, 125, 123, false, "cube2", 34);
         world.addCube(cube1);
         world.addCube(cube2);

         Tuple<Double,Double,Double> cubeAvg = world.getOurCubesAverage();
         
         Assert.AreEqual(10,(int)cubeAvg.Item1);
         Assert.AreEqual(10, (int)cubeAvg.Item2);
         Assert.AreEqual(19, (int)cubeAvg.Item3);


      }

   }
}
