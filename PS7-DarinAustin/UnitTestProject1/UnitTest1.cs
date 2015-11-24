﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AgCubio;
using System.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCubeCreation()
        {
            Cube cube = new Cube(1, 1, 255, 1, 1, false, "Bobby", 10);
            Assert.AreEqual(-1, cube.x); // cube position is placed at the center of cube (see Cube constructor)
            Assert.AreEqual(-1, cube.y);
            Assert.AreEqual(255, cube.color);
            Assert.AreEqual(1, cube.id);
            Assert.AreEqual(1, cube.teamid);
            Assert.AreEqual(false, cube.isFood);
            Assert.AreEqual("Bobby", cube.name);
            Assert.AreEqual(10, cube.mass);
        }

        [TestMethod]
        public void TestCubeEquality()
        {
            Cube cube1 = new Cube(1, 1, 255, 1, 1, false, "Bobby", 10);
            Cube cube2 = new Cube(1, 1, 255, 1, 1, false, "Bobby", 10);
            Assert.AreEqual(cube1, cube2);
        }

        [TestMethod]
        public void TestCubeInEquality()
        {
            Cube cube1 = new Cube(1, 1, 255, 1, 1, false, "Bobby", 10);
            Cube cube2 = new Cube(1, 1, 255, 2, 2, false, "Bobby", 10);
            Assert.AreNotEqual(cube1, cube2);
        }

        [TestMethod]
        public void TestCubeHashEquality()
        {
            Cube cube1 = new Cube(1, 1, 255, 1, 1, false, "Bobby", 10);
            Cube cube2 = new Cube(1, 1, 255, 1, 1, false, "Bobby", 10);
            Assert.AreEqual(cube1.GetHashCode(), cube2.GetHashCode());
        }

        [TestMethod]
        public void TestWorldCreation()
        {
            World world = new World();
            string json = 
          @"{'loc_x':926.0,'loc_y':682.0,'argb_color':-65536,'uid':5571,'food':false,'Name':'3500 is love','Mass':1000.0}
            { 'loc_x':116.0,'loc_y':350.0,'argb_color':-8243084,'uid':5002,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':193.0,'loc_y':523.0,'argb_color':-4759773,'uid':5075,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':267.0,'loc_y':55.0,'argb_color':-7502725,'uid':2,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':998.0,'loc_y':580.0,'argb_color':-16481514,'uid':3,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':69.0,'loc_y':895.0,'argb_color':-5905052,'uid':4,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':387.0,'loc_y':506.0,'argb_color':-2505812,'uid':5,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':687.0,'loc_y':152.0,'argb_color':-9834450,'uid':6,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':395.0,'loc_y':561.0,'argb_color':-2210515,'uid':7,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':585.0,'loc_y':222.0,'argb_color':-11930702,'uid':8,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':49.0,'loc_y':614.0,'argb_color':-4232190,'uid':9,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':809.0,'loc_y':452.0,'argb_color':-9234755,'uid':10,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':286.0,'loc_y':666.0,'argb_color':-11083980,'uid':11,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':252.0,'loc_y':869.0,'argb_color':-8317209,'uid':12,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':748.0,'loc_y':364.0,'argb_color':-1845167,'uid':13,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':185.0,'loc_y':406.0,'argb_color':-2364104,'uid':14,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':324.0,'loc_y':62.0,'argb_color':-13328918,'uid':5015,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':962.0,'loc_y':884.0,'argb_color':-12198033,'uid':16,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':64.0,'loc_y':392.0,'argb_color':-15736963,'uid':5056,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':280.0,'loc_y':662.0,'argb_color':-14308540,'uid':18,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':663.0,'loc_y':549.0,'argb_color':-4577953,'uid':19,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':475.0,'loc_y':742.0,'argb_color':-10962961,'uid':20,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':279.0,'loc_y':458.0,'argb_color':-7381092,'uid':21,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':360.0,'loc_y':823.0,'argb_color':-2848730,'uid':5098,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':881.0,'loc_y':629.0,'argb_color':-6724733,'uid':23,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':510.0,'loc_y':561.0,'argb_color':-6326708,'uid':24,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':201.0,'loc_y':913.0,'argb_color':-7373343,'uid':5046,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':630.0,'loc_y':359.0,'argb_color':-3829330,'uid':26,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':459.0,'loc_y':579.0,'argb_color':-9519582,'uid':5367,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':822.0,'loc_y':981.0,'argb_color':-16113991,'uid':28,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':806.0,'loc_y':172.0,'argb_color':-10185411,'uid':29,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':844.0,'loc_y':40.0,'argb_color':-11329073,'uid':5055,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':957.0,'loc_y':848.0,'argb_color':-7554557,'uid':31,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':391.0,'loc_y':490.0,'argb_color':-9442438,'uid':32,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':594.0,'loc_y':869.0,'argb_color':-10116250,'uid':33,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':367.0,'loc_y':669.0,'argb_color':-6626356,'uid':34,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':140.0,'loc_y':347.0,'argb_color':-8316193,'uid':35,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':885.0,'loc_y':634.0,'argb_color':-15560314,'uid':36,'food':true,'Name':'','Mass':1.0}";
            world.JsonToCubes(json);
            Assert.AreEqual(world.cubes.Count, 38);
        }

        [TestMethod]
        public void TestCubeDeletion()
        {
            World world = new World();
            string json =
          @"{'loc_x':926.0,'loc_y':682.0,'argb_color':-65536,'uid':5571,'food':false,'Name':'3500 is love','Mass':1000.0}
            { 'loc_x':116.0,'loc_y':350.0,'argb_color':-8243084,'uid':5002,'food':true,'Name':'','Mass':1.0}
            { 'loc_x':116.0,'loc_y':350.0,'argb_color':-8243084,'uid':5002,'food':true,'Name':'','Mass':0.0}";
            world.JsonToCubes(json);
            Assert.AreEqual(world.cubes.Count, 1);
        }

        [TestMethod]
        public void TestPlayerDead()
        {
            World world = new World();
            string json = @"{'loc_x':926.0,'loc_y':682.0,'argb_color':-65536,'uid':5571,'food':false,'Name':'3500 is love','Mass':1000.0}";
            world.JsonToCubes(json);
            world.SetPlayer(world.cubes.ElementAt(0));
            json = @"{'loc_x':926.0,'loc_y':682.0,'argb_color':-65536,'uid':5571,'food':false,'Name':'3500 is love','Mass':0.0}";
            world.JsonToCubes(json);
            Assert.AreEqual(world.cubes.Count, 0);
            Assert.IsTrue(world.playerDead);
        }
    }
}