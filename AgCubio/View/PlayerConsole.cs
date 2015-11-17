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

        private bool isRunning;
        private bool isConnected;
        private bool hasCubes;
        private int scaleFactor = 3;

        /// <summary>
        /// 
        /// </summary>
        public PlayerConsole()
        {

            mainWorld = new World(700, 700);

            InitializeComponent();
            DoubleBuffered = true;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {

            drawWorld(e);
            //this.textBox_playerName.Focused;


            //send mouse location to server?

            if (isRunning)
            {
                Invalidate();
                if (isConnected)
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

        private Tuple<int, int> getPointer()
        {
            Tuple<Double, Double, Double> mainCubeInfo;

            int x;
            int y;

            lock (mainWorld)
            {
                mainCubeInfo = mainWorld.getOurCubesAverage();
            }
            int mainCubeX = (int)mainCubeInfo.Item1;
            int mainCubeY = (int)mainCubeInfo.Item2;
            int mainCubeWidth = (int)mainCubeInfo.Item3;

            // adjust pointer location relative to where our cube is drawn          
            x = this.PointToClient(Cursor.Position).X + (mainCubeX - this.Width / 2); //removed + mainCubeWidth / 2
            y = this.PointToClient(Cursor.Position).Y + (mainCubeY - this.Height / 2);

            return Tuple.Create(x, y);
        }


        private void textBox_playerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                if (!String.IsNullOrEmpty(textBox_playerName.Text) && !String.IsNullOrEmpty(textBox_serverName.Text))
                    startGame();
        }


        private void textBox_serverName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                if (!String.IsNullOrEmpty(textBox_playerName.Text) && !String.IsNullOrEmpty(textBox_serverName.Text))
                    startGame();
        }

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

        private void startGame()
        {
            isRunning = true;
            worldSocket = Network.Connect_to_Server(SendPlayerInfo, textBox_serverName.Text);
        }

        private void EndConnectCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;

            state.workSocket.EndConnect(ar);
        }

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
                Network.Send(state.workSocket, textBox_playerName.Text + "\n");
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not connect to server: " + textBox_serverName.Text + "\nError: " + e);
            }



            //drawWorld();

            state.CallbackAction = ReceiveData;

            Network.i_want_more_data(state);
        }



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
                    transformX = (int)((cube.loc_x - mainCubeX) + (this.Width - cube.Width * scaleFactor) / 2) ;
                    transformY = (int)((cube.loc_y - mainCubeY) + (this.Height - cube.Width * scaleFactor) / 2 );
                    transformWidth = (int)(cube.Width * scaleFactor);

                    //  starting point      dist from center to point       exaggerated by main width
                    //transformX = (int)(transformX + ((transformX - this.Width / 2) * mainCubeWidth / 30));
                    //transformY = (int)(transformY + ((transformY - this.Height / 2) * mainCubeWidth / 30));

                    Color color = Color.FromArgb(cube.argb_color);
                    myBrush = new System.Drawing.SolidBrush(color);
                    // set text color in box to a color contrasting the cube color
                    textColor = new System.Drawing.SolidBrush(ContrastColor(color));


                    Rectangle rectangle = new Rectangle(transformX, transformY, transformWidth, transformWidth);

                    e.Graphics.FillRectangle(myBrush, rectangle);
                    Font myFont = new Font("Arial", 10);
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

                textBox_mass.Text = "mass: " + (int)mainWorld.worldCubes[mainWorld.ourID].Mass;
                textBox_width.Text = "width: " + (int)mainWorld.worldCubes[mainWorld.ourID].Width;
                textBox_fps.Text = "fps: ";

                //TODO: This is not refreshing
                this.Invoke(new Action(() =>
                {
                    // textBox_food.Text = "food: " + food.ToString();
                    
                    
                }));

            }

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

        private void ReceiveData(StateObject state)
        {
            // save our state string buffer to a new String
            String newJson = new StringBuilder(state.sb.ToString()).ToString();
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
                        //not working yet
                        gameOver(state);
                }

                //MessageBox.Show(line);
                lock (mainWorld)
                {
                    mainWorld.processCube(line);
                }

                hasCubes = true;


            }
            isConnected = true;
            //MessageBox.Show("Get a new buffer");
            Network.i_want_more_data(state);
            worldSocket = state.workSocket;

        }

        /// <summary>
        /// TODO: This is not working
        /// </summary>
        /// <param name="state"></param>
        private void gameOver(StateObject state)
        {
            Network.Stop(state.workSocket);
            gameOverLabel.Show();
            //Invalidate();
            MessageBox.Show("WHAT");
        }

        /// <summary>
        ///
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
                    //TODO: may want to use an out parameter with this 
                    remainingJson = line;
                }
                // ignore anything else
            }

            return cleanJson;
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.Invoke(new Action(() =>
            {
                textBox_playerName.Show();
                label_playerName.Show();
                textBox_serverName.Show();
                label_serverName.Show();

            }));

            Invalidate();
        }

        /// <summary>
        /// TODO: This is not working
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Network.Stop(worldSocket);
            gameOverLabel.Show();
            //Invalidate();
            MessageBox.Show("WHAT");

        }

    }
}
