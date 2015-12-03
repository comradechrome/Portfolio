using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Diagnostics;

namespace AgCubio
{
    public partial class PlayerConsole : Form
    {

        private System.Drawing.SolidBrush myBrush;
        private System.Drawing.SolidBrush textColor;

        World mainWorld;
        /// <summary>
        /// 
        /// </summary>
        ///public int foodCount = 0;

        /// <summary>
        /// the mass of our current cube
        /// </summary>
        ///public Double ourMass;
        /// <summary>
        /// String to contain the reaminder of incomplete JSON text after each buffer read
        /// </summary>
        private String remainingJson = "";

        private Socket worldSocket;

        //various boolean's
        private bool isRunning;
        private bool isConnected;
        private bool hasCubes;
        private bool isDead;

        // variables used for FPS calculation
        private static int lastTick;
        private static int lastFrameRate;
        private static int frameRate;

        //used during scaling
        private double scaleFactor;

        //stopwatch variables
        private static Stopwatch stopwatch = new Stopwatch();
        private static long elapsedSecs;

        private double biggestMass = 0;
        Font myFont = new Font("Arial", 8);


        /// <summary>
        /// Create our world at 700 X 700 and begin drawing our form
        /// </summary>
        public PlayerConsole()
        {

            mainWorld = new World(700, 700);

            InitializeComponent();
            DoubleBuffered = true;

        }



        /// <summary>
        /// Draw our world. Send mouse positions
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {

            drawWorld(e);

            if (isRunning)
            {
                Invalidate();
                if (isConnected && !isDead)
                {
                    var pointerLocation = getPointer();
                    try
                    {
                        Network.Send(worldSocket, "(move, " + pointerLocation.Item1 + ", " + pointerLocation.Item2 + ")");
                    }
                    catch
                    {
                        MessageBox.Show("Could not connect to server: " + textBox_serverName.Text);
                    }
                }
            }
        }

        /// <summary>
        /// Get our pointer location and return the 'transformed' x,y coordinates 
        /// </summary>
        /// <returns></returns>
        private Tuple<int, int> getPointer()
        {
            Tuple<Double, Double, Double> mainCubeInfo;

            int x;
            int y;

            lock (mainWorld)
            {
                mainCubeInfo = mainWorld.getOurCubesAverage();
            }
            // store our cube position and size 
            int mainCubeX = (int)mainCubeInfo.Item1;
            int mainCubeY = (int)mainCubeInfo.Item2;
            int mainCubeWidth = (int)mainCubeInfo.Item3;

            // adjust pointer location relative to where our cube is drawn          
            x = this.PointToClient(Cursor.Position).X + (mainCubeX - this.Width / 2) + mainCubeWidth / 2;
            y = this.PointToClient(Cursor.Position).Y + (mainCubeY - this.Height / 2) + mainCubeWidth / 2;

            //return a Tuple containing our cubes x,y coordinates
            return Tuple.Create(x, y);
        }

        /// <summary>
        /// start game if player name and server name are not empty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_playerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                if (!String.IsNullOrEmpty(textBox_playerName.Text) && !String.IsNullOrEmpty(textBox_serverName.Text))
                    startGame();
        }

        /// <summary>
        /// start game if player name and server name are not empty (default server name is 'localhost')
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_serverName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                if (!String.IsNullOrEmpty(textBox_playerName.Text) && !String.IsNullOrEmpty(textBox_serverName.Text))
                    startGame();
        }

        /// <summary>
        /// Send plit command to server when the space bar is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerConsole_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && isConnected)
            {
                var pointerLocation = getPointer();

                try
                {
                    Network.Send(worldSocket, "(split, " + pointerLocation.Item1 + ", " + pointerLocation.Item2 + ")");
                }
                catch
                {
                    MessageBox.Show("Could not connect to server: " + textBox_serverName.Text);
                }
            }

        }

        /// <summary>
        /// Begin our connection to the server by sending our server name and the SendPlayerInfo 
        /// callback method to the network_controller
        /// </summary>
        private void startGame()
        {
            isDead = false;
            isRunning = true;
            worldSocket = Network.Connect_to_Server(SendPlayerInfo, textBox_serverName.Text);
            stopwatch.Start();

        }

        /// <summary>
        /// run the endConnect on the socket startup
        /// </summary>
        /// <param name="ar"></param>
        private void EndConnectCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;

            state.workSocket.EndConnect(ar);


        }

        /// <summary>
        /// Callback function that hides the initial server and player name text boxes then
        /// sends the player name to the server
        /// </summary>
        /// <param name="state"></param>
        private void SendPlayerInfo(StateObject state)
        {

            this.Invoke(new Action(() =>
            {
                textBox_playerName.Hide();
                label_playerName.Hide();
                textBox_serverName.Hide();
                label_serverName.Hide();


            }));
            try
            {
                // send player name
                Network.Send(state.workSocket, textBox_playerName.Text + "\n");
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not connect to server: " + textBox_serverName.Text + "\nError: " + e);
            }

            // change the callback method to Receive Data
            state.CallbackAction = ReceiveData;
            // request more data
            Network.i_want_more_data(state);
        }

        /// <summary>
        /// transform the x,y coordinates and size of all cubes ... then draw
        /// </summary>
        /// <param name="e"></param>
        private void drawWorld(PaintEventArgs e)
        {
            int food = 0;
            lock (mainWorld)
            {
                if (!hasCubes) return;
                // Cube mainCube = mainWorld.worldCubes[mainWorld.ourID];
                var mainCubeInfo = mainWorld.getOurCubesAverage();
                Double mainCubeX = mainCubeInfo.Item1;
                Double mainCubeY = mainCubeInfo.Item2;
                Double mainCubeWidth = mainCubeInfo.Item3;
                int transformX, transformY, transformWidth;
                //cubeFactor = (int)(this.Height / 2 mainCubeWidth);

                foreach (Cube cube in mainWorld.worldCubes.Values)
                {

                    if (cube.food)
                        food++;

                    //      from (int)((cube.Value.loc_x - mainCubeX) + (mainWorld.worldWidth - mainCubeWidth) / 2 - cube.Value.Width * scaleFactor / 2);
                    transformX = (int)((cube.loc_x - mainCubeX) + (this.Width - cube.Width - mainCubeWidth) / 2);
                    transformY = (int)((cube.loc_y - mainCubeY) + (this.Height - cube.Width - mainCubeWidth) / 2);

                    // set scaleFact to be a function of cube width
                    scaleFactor = (300 - mainCubeWidth) / 150;
                    //scaleFactor = 0.5;

                    //         starting point    dist from center to point    exaggerated by main width
                    transformX += (int)((transformX - this.Width / 2) * scaleFactor);
                    transformY += (int)((transformY - this.Height / 2) * scaleFactor);

                    transformWidth = (int)((cube.Width) * (1 + scaleFactor));

                    Color color = Color.FromArgb(cube.argb_color);
                    myBrush = new System.Drawing.SolidBrush(color);
                    // set text color in box to a color contrasting the cube color
                    textColor = new System.Drawing.SolidBrush(ContrastColor(color));

                    // create rectangle ... minimum cube size is 3
                    Rectangle rectangle = new Rectangle(transformX, transformY, (transformWidth > 3 ? transformWidth : 3),
                                                                                (transformWidth > 3 ? transformWidth : 3));
                    // draw cube
                    e.Graphics.FillRectangle(myBrush, rectangle);
                    SizeF size = e.Graphics.MeasureString(cube.Name, myFont);
                    // Draw text in our cube only if it can fit - center text in the cube
                    if ((size.Width < rectangle.Size.Width) && (size.Height < rectangle.Size.Height))
                    {
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        e.Graphics.DrawString(cube.Name, myFont, textColor, transformX + (int)(transformWidth / 2), transformY + (int)(transformWidth / 2), sf);

                    }
                }
                // game statistics
                if (mainWorld.worldCubes[mainWorld.ourID].Mass > biggestMass)
                    biggestMass = mainWorld.worldCubes[mainWorld.ourID].Mass;
                textBox_mass.Text = "mass: " + (int)mainWorld.worldCubes[mainWorld.ourID].Mass;
                textBox_width.Text = "width: " + (int)mainWorld.worldCubes[mainWorld.ourID].Width;
                textBox_food.Text = "food: " + food.ToString();
                textBox_fps.Text = "fps: " + CalcFrameRate();
                refreshTextBoxes();
            }
        }

        /// <summary>
        /// method to refresh game statistics 
        /// </summary>
        private void refreshTextBoxes()
        {
            textBox_mass.Update();
            textBox_mass.Refresh();
            textBox_width.Update();
            textBox_width.Refresh();
            textBox_food.Update();
            textBox_food.Refresh();
            textBox_fps.Update();
            textBox_fps.Refresh();
        }

        /// <summary>
        /// Nice color contrast algorithm I found on source forge
        /// http://stackoverflow.com/questions/1855884/determine-font-color-based-on-background-color
        /// This will be used for creating the text color on the cubes
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        Color ContrastColor(Color color)
        {
            int d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return Color.FromArgb(d, d, d);
        }


        /// <summary>
        /// Calculate the framerate
        /// </summary>
        /// <returns></returns>
        public static int CalcFrameRate()
        {

            if (System.Environment.TickCount - lastTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = System.Environment.TickCount;
            }
            frameRate++;
            return lastFrameRate;
        }

        /// <summary>
        /// Receive JSON data from server. Process data and send to model for Cube creation.
        /// If our mass is zero, end the game
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveData(StateObject state)
        {
            // save our state string buffer to a new String
            String newJson = state.sb.ToString();
            // clear out the state buffer
            state.sb.Clear();
            // process the raw JSON data and return a string array of clean JSON
            List<String> jsonLines = processJson(remainingJson + newJson);
            // JSON is now clean, we can process line by line
            foreach (String line in jsonLines)
            {
                Double ourMass;
                if (hasCubes)
                {
                    lock (mainWorld)
                    {
                        ourMass = mainWorld.worldCubes[mainWorld.ourID].Mass;
                    }
                    if (ourMass == 0.0)
                    {
                        // our mass is zero ... end game 
                        this.Invoke(new Action(() =>
                        {
                            isDead = true;
                            gameOver();
                        }));
                        return;
                    }
                }

                // create cubes in world
                lock (mainWorld)
                {
                    mainWorld.processCube(line);
                }

                hasCubes = true;


            }
            isConnected = true;

            // request more cube data from server
            Network.i_want_more_data(state);
            worldSocket = state.workSocket;

        }

        /// <summary>
        /// End the game and close the socket
        /// </summary>
        private void gameOver()
        {
            stopwatch.Stop();
            elapsedSecs = stopwatch.ElapsedMilliseconds / 1000;
            gameOverLabel.Text = "YOU DIED!! \nSeconds elapsed: " + elapsedSecs
                + "\nGreatest Mass: " + (int)biggestMass
                + "\nMass at Death: " + textBox_mass.Text;
            gameOverLabel.Show();
            gameOverLabel.Update();
            

            Network.Stop(worldSocket);


            //MessageBox.Show("Gamestats: /nSeconds Elapsed: " + elapsedSecs);
        }

        /// <summary>
        /// process raw JSON. return a list of valid JSON cubes. 
        /// </summary>
        /// <param name="rawJson"></param>
        /// <returns></returns>
        private List<String> processJson(String rawJson)
        {

            // clear the remainingJson string
            remainingJson = "";
            List<String> cleanJson = new List<string>();
            String[] lines = rawJson.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (String line in lines)
            {
                if (line.StartsWith("{") & line.EndsWith("}"))
                {
                    cleanJson.Add(line);
                }
                else if (line.StartsWith("{"))
                {
                    remainingJson = line;
                }
                // ignore anything else
            }

            return cleanJson;
        }

        /// <summary>
        /// New game menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameOver();
            this.Invoke(new Action(() =>
            {
                gameOverLabel.Hide();
                textBox_playerName.Show();
                label_playerName.Show();
                textBox_serverName.Show();
                label_serverName.Show();
            }));

            Invalidate();
        }

        /// <summary>
        /// If there is no connection, then closes window
        /// Otherwise, kills player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (worldSocket == null || isDead)
            {
                this.Close();
            }
            else
            {
                Invalidate();
                isDead = true;
                gameOver();
            }
        }

        //private void aboutAgCubioToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //   this.Invoke(new Action(() =>
        //   {
        //      MessageBox.Show("AgCubio version 0.1\nCreated by + ellefsakishev");
        //   }));
        //}
    }
}
