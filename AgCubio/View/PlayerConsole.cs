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
        private Delegate ConnectionDelegate;

        private System.Drawing.SolidBrush myBrush;
        private System.Drawing.SolidBrush textColor;

        World world;
        /// <summary>
        /// 
        /// </summary>
        public int foodCount = 0;
        /// <summary>
        /// 
        /// </summary>
        public Double ourMass = 0;
        /// <summary>
        /// 
        /// </summary>
        public String ourName;
        /// <summary>
        /// 
        /// </summary>
        public String ourServer;

        /// <summary>
        /// 
        /// </summary>
        public PlayerConsole()
        {

            //TODO: get playername and server name - then hide textbox and labels

            world = new World(1000, 1000, ourName, ourServer);

            //temporary data test method
            buildWorld();

            InitializeComponent();
            DoubleBuffered = true;

            Random rand = new Random(0);
            //while (true)
            //{
            //    Cube cube = new Cube(rand.NextDouble()*1000, rand.NextDouble() * 1000, (int)rand.NextDouble() * -32000000, (int)rand.NextDouble() * 5000, true, "", 1.0);

            //}
        }


        /// <summary>
        /// Temporary method to read test data and populate our world
        /// </summary>
        private void buildWorld()
        {
            string[] lines = System.IO.File.ReadAllLines(@"..\..\..\Resources\Libraries\sample.data");
            foreach (string line in lines)
            {
                Cube cube = JsonConvert.DeserializeObject<Cube>(line);
                world.addCube(cube);
            }

        }

        /// <summary>
        /// TODO: add this.Invalidate(); to end of method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerConsole_Paint(object sender, PaintEventArgs e)
        {
            foodCount = 0;
            lock(world)
            {
                foreach (KeyValuePair<int, Cube> cube in world.cubes)
                {
                    //Need to reset food count to zero

                    if (cube.Value.food)
                        foodCount++;
                    if (world.playerName == cube.Value.Name)
                        ourMass = cube.Value.Mass;

                    Color color = Color.FromArgb(cube.Value.argb_color);
                    myBrush = new System.Drawing.SolidBrush(color);
                    textColor = new System.Drawing.SolidBrush(Color.Black);
                    Rectangle rectangle = new Rectangle((int)cube.Value.loc_x, (int)cube.Value.loc_y, (int)cube.Value.Width, (int)cube.Value.Width);
                    e.Graphics.FillRectangle(myBrush, rectangle);
                    Font myFont = new Font("Arial", 10);
                    e.Graphics.DrawString(cube.Value.Name, myFont, textColor, (int)cube.Value.loc_x, (int)cube.Value.loc_y);
                }
            }
        }

        private void aboutAgCubioToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void usingAgCubioToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void width_TextChanged(object sender, EventArgs e)
        {
            //TODO: might want to use the cube method to get this
            width.Text = Math.Sqrt(ourMass).ToString();
        }

        private void fps_TextChanged(object sender, EventArgs e)
        {

        }

        private void food_TextChanged(object sender, EventArgs e)
        {
            food.Text = foodCount.ToString();
        }

        private void mass_TextChanged(object sender, EventArgs e)
        {
            mass.Text = ourMass.ToString();
        }

        private void textBox_playerName_TextChanged(object sender, EventArgs e)
        {
            ourName = textBox_playerName.ToString();

        }

        private void textBox_serverName_TextChanged(object sender, EventArgs e)
        {
            ourServer = textBox_serverName.ToString();
            
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

        private void startGame()
        {
            textBox_playerName.Hide();
            label_playerName.Hide();
            textBox_serverName.Hide();
            label_serverName.Hide();

            Network.Connect_to_Server(SendPlayerInfo, textBox_serverName.Text);
        }

        private void SendPlayerInfo(StateObject s)
        {
            Network.Send(s.workSocket, textBox_playerName.Text + "\n");

            s.ConnectionDelegate = ReceivePlayer;
        }

        private static void ReceivePlayer (StateObject s)
        {
            try
            {
                // Begin receiving the data from the remote device.
                //s.workSocket.BeginReceive(s.buffer, 0, StateObject.BufferSize, 0, s.ConnectionDelegate, s);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        private void textBox_serverName_KeyDown_1(object sender, KeyEventArgs e)
        {
            //removelater
        }
    }
}
