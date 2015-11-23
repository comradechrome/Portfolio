AgCubio Client
Authors: Darin Stoker, Austin Payne
Date Last Modified: 11/18/15

Design Decisions: We chose to split the login window and the gameView window into two seperate forms to make it easier to draw the gameView. 
Because of this we had to pass the userName and ServerName entered into the gameView form and open the socket from that form. The gameView 
is layed out using a table layout so that game stats can be uniformly displayed along the bottom of the game window. A picture box was used 
to draw game elements because it is defaulty double buffered.

The world class contains a JsonToCubes method that, in one operation, splits and json string into sepearte json objects, deserializes them, 
and then adds them to a world "cubes" HashSet. This allowed for a one line operation to build and update the world as necessary. Due to the 
use of a HashSet, the Cube object implemented both .Equals and .GetHashCode. The HashCode was generated based on the cube ID as it is unique 
across server.

Features: A max scaling factor was set so that if a player splits several times, the window would not zoom in too much.  