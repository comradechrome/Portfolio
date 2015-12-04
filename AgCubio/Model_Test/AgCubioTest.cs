using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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
         Assert.AreEqual(20, (int)cube.Width);

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

         Assert.AreEqual(0, world.worldCubes[123].team_id);
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

         world.playerCubes[cube1.Name] = cube1.uid;
         world.virusList.Add(cube1.uid);

         HashSet<Cube> cubes = new HashSet<Cube>();
         cubes.Add(cube1);
         cubes.Add(cube2);


         world.addCube(cube1);
         world.addCube(cube2);

         world.teams[cube1.uid] = cubes;
         world.removeCube(cube2);
         world.removeCube(cube1);



         Assert.AreEqual(0, world.ourCubes.Count);




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

         Tuple<Double, Double, Double> cubeAvg = world.getOurCubesAverage();

         Assert.AreEqual(12, (int)cubeAvg.Item1);
         Assert.AreEqual(12, (int)cubeAvg.Item2);
         Assert.AreEqual(15, (int)cubeAvg.Item3);


      }

      [TestMethod]
      public void momentum()
      {
         World world = new World(100, 200);
         Cube cube1 = new Cube(10, 10, -124342, 123, 0, false, "cube1", 34);
         cube1.momentum = 5;

         Assert.AreEqual(5, cube1.momentum);
      }

      [TestMethod]
      public void mergeDecay()
      {
         World world = new World(100, 200);
         Cube cube1 = new Cube(10, 10, -124342, 123, 0, false, "cube1", 34);
         cube1.mergeDecay = 5;

         Assert.AreEqual(5, cube1.mergeDecay);
      }

      [TestMethod]
      public void edges()
      {
         Cube cube1 = new Cube(100, 100, -124342, 123, 0, false, "cube1", 100);

         Tuple<int, int, int, int> edges = cube1.edges;

         Assert.AreEqual(95, edges.Item1);
         Assert.AreEqual(95, edges.Item2);
         Assert.AreEqual(105, edges.Item3);
         Assert.AreEqual(105, edges.Item4);

      }

      [TestMethod]
      public void equalsOverloads()
      {
         Cube cube1 = new Cube(100, 100, -124342, 123, 0, false, "cube1", 100);
         Cube cube2 = new Cube(100, 100, -124342, 123, 0, false, "cube1", 100);
         Cube cube3 = new Cube(100, 100, -124342, 124, 0, false, "cube3", 100);
         Cube cube4 = null;
         Cube cube5 = null;



         Assert.AreEqual(cube1, cube2);
         Assert.AreNotEqual(cube1, cube3);
         Assert.AreEqual(cube4, cube5);
         Assert.AreNotEqual(cube3, cube5);

         Assert.IsTrue(cube1 == cube2);
         Assert.IsTrue(cube1 != cube3);
         Assert.IsFalse(cube1.Equals(cube4));

         //Assert.IsTrue(cube4.Equals(cube5));

         Assert.AreEqual(123, cube1.GetHashCode());


      }

      [TestMethod]
      public void worldParams()
      {
         WorldParams worldParams = new WorldParams(@"..\..\..\Resources\Libraries\world_parameters.xml");

         WorldParams worldParams2 = new WorldParams();


         Assert.AreEqual(1000, worldParams.width);
         Assert.AreEqual(1000, worldParams.height);
         Assert.AreEqual(150, worldParams.maxSplitDistance);
         Assert.AreEqual(5, worldParams.topSpeed);
         Assert.AreEqual(1, worldParams.lowSpeed);
         Assert.AreEqual(200, worldParams.attritionRate);
         Assert.AreEqual(1, worldParams.foodValue);
         Assert.AreEqual(1000, worldParams.playerStartMass);
         Assert.AreEqual(5000, worldParams.maxFood);
         Assert.AreEqual(100, worldParams.minSplitMass);
         Assert.AreEqual(1.25, worldParams.absorbConstant);
         Assert.AreEqual(10000, worldParams.maxViewRange);
         Assert.AreEqual(25, worldParams.heartbeatsPerSecond);
         Assert.AreEqual(800, worldParams.virusMass);
         Assert.AreEqual(5, worldParams.virusProbability);
         Assert.AreEqual(800, worldParams.acceleratedAttrition);
         Assert.AreEqual(100, worldParams.foodRandomFactor);
         Assert.AreEqual(5, worldParams.foodGrowthFactor);
         Assert.AreEqual(.25, worldParams.allowedOverlap);
         Assert.AreEqual(.00125, worldParams.scaleConst);
         Assert.AreEqual(1500, worldParams.smoothingIncrement);
         Assert.AreEqual(20, worldParams.splitDistance);
         Assert.AreEqual(10, worldParams.splitDecayRate);
         Assert.AreEqual(50, worldParams.splitMomentum);
         Assert.AreEqual(10, worldParams.splitDecay);
         Assert.AreEqual(100, worldParams.infectedDecay);
      }

      
      /// <summary>
      /// SHould not throw an exception
      /// </summary>
      [TestMethod]
      //[ExpectedException(typeof(Exception))]
      public void worldParamsBadFile()
      {
         WorldParams worldParams = new WorldParams("badfile");

      }
   }
}
