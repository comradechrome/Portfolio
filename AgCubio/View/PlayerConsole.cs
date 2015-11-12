﻿using System;
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

        World mainWorld;
        /// <summary>
        /// 
        /// </summary>
        public int foodCount = 0;
        /// <summary>
        /// 
        /// </summary>
      public String ourServer;
        /// <summary>
      /// the mass of our current cube
        /// </summary>
      public Double ourMass;
        /// <summary>
        /// 
        /// </summary>
        public PlayerConsole()
        {

            //TODO: get playername and server name - then hide textbox and labels

         // TODO: may not have UID yet
         mainWorld = new World(1000, 1000);

            //temporary data test method
            //buildWorld();

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
                mainWorld.addCube(cube);
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
         lock (mainWorld)
            {
                foreach (KeyValuePair<int, Cube> cube in mainWorld.cubes)
                {
                    //Need to reset food count to zero

                    if (cube.Value.food)
                        foodCount++;
       
               if (mainWorld.ourID == cube.Value.uid)
                        ourMass = cube.Value.Mass;

                    Color color = Color.FromArgb(cube.Value.argb_color);
                    myBrush = new System.Drawing.SolidBrush(color);
                    textColor = new System.Drawing.SolidBrush(Color.Black);
                    Rectangle rectangle = new Rectangle((int)cube.Value.loc_x, (int)cube.Value.loc_y, (int)cube.Value.Width, (int)cube.Value.Width);
                    e.Graphics.FillRectangle(myBrush, rectangle);
                    Font myFont = new Font("Arial", 10);
                    e.Graphics.DrawString(cube.Value.Name, myFont, textColor, (int)cube.Value.loc_x, (int)cube.Value.loc_y);
                }

                //send mouse location to server

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
            Network.Connect_to_Server(SendPlayerInfo, textBox_serverName.Text);
        }

        private static void EndConnectCallback(IAsyncResult ar)
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
           
            Network.Send(state.workSocket, textBox_playerName.Text + "\n");

            state.CallbackAction = ReceiveData;
            Network.i_want_more_data(state);

        }

        private void ReceiveData (StateObject state)
        {
            StringBuilder test = state.sb;
            MessageBox.Show(state.sb.ToString());
            mainWorld.firstCube(state.sb.ToString());
            Network.i_want_more_data(state);
        }

      private void processCube(Cube cube)
      {
         // check if cube exists
         if(mainWorld.cubes.ContainsKey(cube.uid))
         {
            // check if mass equals zero
            if(cube.Mass == 0.0)
            {
               // check if our mass is zero - end game if true
               if(cube.uid == mainWorld.ourID)
               {
                  // TODO: run code to end game
               }
               else
                  mainWorld.removeCube(cube);
            }
            // cube exists and is not zero mass
            else
               mainWorld.moveCube(cube);
         }
         // cube doesn't exist so we will add it
         else
            mainWorld.addCube(cube);

      }


    }
}
