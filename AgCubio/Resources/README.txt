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
		  <virus_mass>800</virus_mass>
          <virus_probability>5</virus_probability>
          <accelerated_attrition>800</accelerated_attrition>
          <food_random_factor>100</food_random_factor>
          <food_growth_factor>5</food_growth_factor>
          <allowed_overlap>.25</allowed_overlap>
          <scale_constant>.00125</scale_constant>
          <smoothing_increment>1500</smoothing_increment>
          <split_distance>20</split_distance>
          <split_decay>10</split_decay>
          <infected_decay>100</infected_decay>
		</parameters>

	Gameplay features :
		-Speed of player is selected from 5 tiers depending on size
		-Players can split as often as the size of thier cubes allows
		-Cubes shift towards the location of the player's mouse after split
		-Virus cubes force a split for the player and move cubes towards last mouse location
		-Cubes merge quickly after a split but slowly after a virus split
		-Viruses are easily distinguishable from players because their names fluctuate menacingly
		-Food Cubes grow randomly to give players goals during play
		-Speed decreases near edges to eliminate 'invisible wall' quality of world edge
		-Movement smoothing optimizes visual experience
		-Only the last mouse location is used from each player on each heart beat to prevent an unfair advantage from 'modified' clients

	Known Bugs:
		-Occasion food artifacts on startup
		-Occasional player mouse conflation in multiplayer games
		-split cubes can be sent outside of game borders
		-Viruses can occasionally add to mass (also a gameplay feature)
		-known iPV4/V6 compatibility issues
		-Very rare screen centering on other players/food cubes (possibly client issue)

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
We test the model project. >99% code coverage has been acheived with our tests


