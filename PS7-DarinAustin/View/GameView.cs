using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AgCubio;
using System.Net.Sockets;

namespace AgCubio
{
    /// <summary>
    /// Game view form represents viewport into AgCubio world.
    /// Includes viewport and status bar at the bottom of the form.
    /// </summary>
    public partial class gameView : Form
    {
        readonly string playerName;
        readonly string serverName;
        private double scalingFactor;
        private double maxScalingFactor;
        private double translationFactorX;
        private double translationFactorY;
        private World world = null;
        private int viewportWidth;
        private int viewportHeight;
        private State state = null;
        private SolidBrush cubeBrush;
        private SolidBrush stringBrush = new SolidBrush(Color.White);
        private Timer timer = null;
        private long fpsCounter;

        /// <summary>
        /// Initializes GameView form. UserName and ServerName are passed in 
        /// from the login scrren (see Login.cs)
        /// </summary>
        /// <param name="userName">userName to use</param>
        /// <param name="serverName">Server to connect to</param>
        public gameView(string userName, string serverName)
        {
            InitializeComponent();
            world = new World();
            viewportWidth = this.Width;
            viewportHeight = this.Height;
            this.playerName = userName;
            this.serverName = serverName;
            nameLabel.Text = "Player: " + playerName;
            scalingFactor = 2;
            maxScalingFactor = 3;
            translationFactorX = 0;
            translationFactorY = 0;
            // time for use in calculating FPS
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(TimerTick);
            // FPS counter
            fpsCounter = 0;
            // begins socket connection
            Connect();
        }

        /// <summary>
        /// Closes application when user clicks "X" in the upper right.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Closing arguments</param>
        private void gameView_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Initiates a socket connection to serverName.
        /// </summary>
        private void Connect()
        {
            try
            {
                state = NetworkController.ConnectToServer(SendName, serverName);
            }
            catch(SocketException)
            {
                // if there are any errors, gracefully notify user and allow restart
                HandleSocketException();
            }
            
        }

        /// <summary>
        /// Callback function used when a successful socket connection is made.
        /// </summary>
        private void SendName()
       {
            if(!state.socket.Connected)
            {
                HandleSocketException();
            }
            // updates socket callback to GetPlayer (first cube sent by the server)
            state.Callback = GetPlayer;
            // sends the user entered playerName to the server
            NetworkController.Send(state, playerName);
        }

        /// <summary>
        /// Gets player information from Server. According to the 
        /// server protocol, the first piece of data it sends back to the client 
        /// is the json object representing our player cube.
        /// </summary>
        private void GetPlayer()
        {
            string json;
            lock (state.sb)
            {
                json = state.sb.ToString();
                state.sb.Clear();   //We know that there is only one cube, so this is fine
            }
            lock (world)
            {
                world.JsonToCubes(json); // deserialize cube
                world.SetPlayer(world.cubes.ElementAt(0)); // player cube is only object sent back
            }
            // updates callback for more data
            state.Callback = FetchData;
            try
            {
                NetworkController.IWantMoreData(state);
            }
            catch (SocketException)
            {
                HandleSocketException();
            }
            // asks GUI to invalidate and cause a re-paint of newly recieved cubes
            viewportPanel.Invalidate();
        }

        /// <summary>
        /// Fetches more data from the server using the NetworkController method 
        /// IWantMoreData.
        /// </summary>
        private void FetchData()
        {
            string json;
            // lock state.sb in case another thread is accessing state
            lock (state.sb)
            {
                json = state.sb.ToString(); // gets current string stored in string builder
                state.sb.Clear(); // clears string builder so more data can be put into it
                // finds partial string after last complete json object was sent
                int end = json.LastIndexOf('\n');
                string remaining = json.Substring(end);
                state.sb.Append(remaining); // store partial back in string builder
                json = json.Substring(0, end); // process only complete json objects
            }
            // again locks world in case another thread is trying to access it
            lock (world)
            {
                world.JsonToCubes(json); // deserialize cubes
            }
            // sets callback to FetchData so program continually gets data from server
            state.Callback = FetchData;
            try
            {
                NetworkController.IWantMoreData(state);
            }
            catch (SocketException)
            {
                HandleSocketException();
            }
            // asks GUI to invalidate and cause a re-paint of newly recieved cubes
            viewportPanel.Invalidate();
        }

        /// <summary>
        /// Helper method that updates stats bar on every re-paint
        /// </summary>
        private void updateStatusBar()
        {
            massLabel.Text = "Mass: " + world.player.mass.ToString();
            xPosLabel.Text = "X Position: " + world.player.x.ToString();
            yPosLabel.Text = "Y Position:" + world.player.y.ToString();
        }

        /// <summary>
        /// This function handles all painting done on the form. Every time more data 
        /// comes in from the server, this method is invoked and thus the player can always 
        /// see the most up to date scene.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Paint arguments</param>
        private void viewportPanel_Paint(object sender, PaintEventArgs e)
    {
            timer.Start(); // starts timer for FPS calculation

            lock (world)
            {
                //e.Graphics.ScaleTransform(2f, 2f);

                if (world.player != null)
                {
                    scalingFactor = viewportWidth/(world.player.width*3);
                    if (scalingFactor > maxScalingFactor) // prevent too much zoom if player splits multiple times
                        scalingFactor = maxScalingFactor;
                    if (scalingFactor == 0) // prevents int casting to 0 and causing cubes to disappear
                        scalingFactor = 1;
                    // translation factor to get player cube in the center of the view and other cubes drawn in relation to it
                    translationFactorX = (viewportWidth / 3) - world.player.x * scalingFactor;
                    translationFactorY = (viewportHeight / 3) - world.player.y * scalingFactor;
                }
                // draws each cube in the world
                foreach (Cube cube in world.cubes)
                {
                    if (cube.id == world.player.id)
                        world.SetPlayer(cube); // updates player cube mass, postion, etc.

                    Color color = Color.FromArgb(cube.color);
                    cubeBrush = new SolidBrush(color);
                    // translates all cubes in relation to the player cube (at the center of the view)
                    int cubeXPos = (int)((cube.x * scalingFactor) + translationFactorX);
                    int cubeYPos = (int)((cube.y * scalingFactor) + translationFactorY);
                    // scales cube
                    int scaledCubeWidth = (int)(cube.width * scalingFactor);
                    // draws cube on screen
                    e.Graphics.FillRectangle(cubeBrush, new Rectangle(cubeXPos, cubeYPos, scaledCubeWidth, scaledCubeWidth));
                    // if this is a player cube, draw a player name label on the cube
                    if (!cube.isFood)
                    {
                        Font font = new Font("Calibri", (int)(10 * scalingFactor));
                        SizeF size = e.Graphics.MeasureString(cube.name, font); // need for centering
                        // centers player name label on cube
                        int stringPosX = (int)(cubeXPos + (scaledCubeWidth / 2) - (size.Width / 2));
                        int stringPosY = (int)(cubeYPos + (scaledCubeWidth / 2) - (size.Height / 2));

                        Point point = new Point(stringPosX, stringPosY);
                        e.Graphics.DrawString(cube.name, font, stringBrush, point);
                    }
                }

                //End of game
                if (world != null)
                {
                    if (world.playerDead)
                    {
                        // show dialog box with final mass stat
                        DialogResult diagResult = MessageBox.Show("Game over! Would you like to play again?\n\nFinal Mass: " + world.player.mass, "You died!",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                        switch (diagResult)
                        {
                            // if user wants to play again, restart application
                            case (DialogResult.Yes):
                                Application.Restart();
                                this.Close();
                                break;
                            // if not, initate form_closing
                            case (DialogResult.No):
                                this.Close();
                                break;
                        }
                    }
                }
            }
            updateStatusBar();
            sendCurrentMousePosition(); //Send mouse data for next server update

            fpsCounter++; // update FPS counter
        }

        /// <summary>
        /// Gracefully handles any socket expceptions, notifies the user and allows 
        /// them to re-enter the game
        /// </summary>
        private void HandleSocketException()
        {
            state.socket.Close();   //Release the socket resources

            DialogResult diagResult = MessageBox.Show("Server rejected the connection. "
                + "Verify the server is running on the specified port.", "Connection Rejected",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

            Application.Restart();  
        }

        /// <summary>
        /// Sends mouse position to server which initiates player cube movement in the 
        /// direction of the mouse
        /// </summary>
        private void sendCurrentMousePosition()
        {
            // translates mouse coordinates back into world coordinates
            int dest_x = (int)((MousePosition.X  - translationFactorX) / scalingFactor);
            int dest_y = (int)((MousePosition.Y  - translationFactorY) / scalingFactor);

            NetworkController.Send(state, "(move, " + dest_x + ", " + dest_y + ")\n");
        }

        /// <summary>
        /// Sends a split command, by pressing the spacebar, to the server that causes the player cube to split 
        /// into smaller cubes
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Key arguments</param>
        private void gameView_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Space)
            {
                // translates mouse coordinates back into world coordinates
                int dest_x = (int)((MousePosition.X - translationFactorX) / scalingFactor);
                int dest_y = (int)((MousePosition.Y - translationFactorY) / scalingFactor);

                e.SuppressKeyPress = true;
                NetworkController.Send(state, "(split, " + dest_x + ", " + dest_y + ")\n");
            }
        }

        /// <summary>
        /// Updates FPS counter everty 1 second.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void TimerTick(Object obj, EventArgs args)
        {
            // updates FPS label
            fpsLabel.Text = "FPS: " + fpsCounter.ToString();
            fpsCounter = 0;
        }
    }
}
