Team: Andrey Myakishev, Randall Ellefsen, and Kyeunghye Park

We installed Installed NewtonSoft Json.NET 7.0.1

** Server **
Server can be started with an argument of a file path to an XML parameter file of the following format:
	<?xml version="1.0" encoding="utf-8" ?>
		<parameters>
		  <width>1000</width>
		  <height>1000</height>
		  <max_split_distance>150</max_split_distance>  
		  <top_speed>5</top_speed>
		  <low_speed>1</low_speed>
		  <attrition_rate>200</attrition_rate>
		  <food_value>1</food_value>
		  <player_start_mass>1000</player_start_mass>
		  <max_food>5000</max_food>
		  <min_split_mass>100</min_split_mass>
		  <absorb_constant>1.25</absorb_constant>
		  <max_view_range>10000</max_view_range>
		  <heartbeats_per_second>25</heartbeats_per_second>
		</parameters>

** Model **
Our cube and world objects are defined here:
Cubes have loc_x, loc_y, argb_color, uid, team_id, food, name, and mass properties
We're using 'cube Width = Mass ^ .65' to closer approximate the cube size size so food is eaten on the edge of our cube
The world object contains a hight, width, our UID, a dictionary of 'world' cubes, and a dictionary of 'our' cubes.
Our primary method is the 'processCubes' method which takes a JSON string and creates a cube. It checks for cube mass, 
team_id values, and cube existence to determine if cube should be added, removed, or moved. If the team_id is equal to 
our ID it will add that cube to the ourCubes dictionary (in the case of a split).
World Paramters are setup to include parameters for game mechanics.

** Network Project **
Server name is sent to the network to create an Async Socket connection
Generic network functions are done in this Project
This method sends data to the view to be processed via a calback method

** View **
Callback methods used to send and process network data are defined in this project
The world object is instantiated and contained here.

The Network process receives JSON. Through a callback method, the raw JSON contained in a string buffer is processed
and sent to the model project for processing.

This project continually itterates through the world of cubes and draws them on the screen. We transform the cubes
position and size so that our cube is always in the center of the screen. As our cube gets larger we zoom out so see
more of the world.

** Model Test **
We test the model project. 100% code coverage has been acheived with our tests