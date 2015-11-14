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
      public int foodCount = 0;

      /// <summary>
      /// the mass of our current cube
      /// </summary>
      public Double ourMass;
      /// <summary>
      /// String to contain the reaminder of incomplete JSON text after each buffer read
      /// </summary>
      private String remainingJson = "";

      private Socket worldSocket;

      private bool isRunning;
      private bool isConnected;
      private bool hasCubes;
      /// <summary>
      /// 
      /// </summary>
      public PlayerConsole()
      {

         //TODO: get playername and server name - then hide textbox and labels

         // TODO: may not have UID yet
         mainWorld = new World(772, 772);
         //temporary data test method
         //buildWorld();

         InitializeComponent();
         DoubleBuffered = true;

      }


      ///// <summary>
      ///// Temporary method to read test data and populate our world
      ///// </summary>
      //private void buildWorld()
      //{
      //    string[] lines = System.IO.File.ReadAllLines(@"..\..\..\Resources\Libraries\sample.data");
      //    foreach (string line in lines)
      //    {
      //        Cube cube = JsonConvert.DeserializeObject<Cube>(line);
      //        mainWorld.addCube(cube);
      //    }

      //}

      /// <summary>
      /// TODO: add this.Invalidate(); to end of method
      /// </summary>
      /// <param name="e"></param>
      protected override void OnPaint(PaintEventArgs e)
      {

         drawWorld(e);
         //this.textBox_playerName.Focused;

         //send mouse location to server

         if (isRunning)
         {
            Invalidate();
            if (isConnected)
            {
               var pointerLocation = getPointer();
               Network.Send(worldSocket, "(move, " + pointerLocation.Item1 + ", " + pointerLocation.Item2 + ")");
            }
         }
      }

      private Tuple<int,int> getPointer()
      {
         Tuple<Double, Double, Double> mainCubeInfo;

         int x;
         int y;

         lock (mainWorld)
         {
            mainCubeInfo = mainWorld.getOurInfo();
         }
         int mainCubeX = (int)mainCubeInfo.Item1;
         int mainCubeY = (int)mainCubeInfo.Item2;

         // adjust pointer location relative to where our cube is drawn
         x = this.PointToClient(Cursor.Position).X + (mainCubeX - mainWorld.worldWidth / 2);
         y = this.PointToClient(Cursor.Position).Y + (mainCubeY - mainWorld.worldHieght / 2);

         return Tuple.Create(x,y);
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
            Network.Send(worldSocket, "(split, " + pointerLocation.Item1 + ", " + pointerLocation.Item2 + ")");
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

         Network.Send(state.workSocket, textBox_playerName.Text + "\n");

         //drawWorld();

         state.CallbackAction = ReceiveData;

         Network.i_want_more_data(state);
      }


      private void drawWorld(PaintEventArgs e)
      {
         lock (mainWorld)
         {
            if (!hasCubes) return;
            // Cube mainCube = mainWorld.worldCubes[mainWorld.ourID];
            var mainCubeInfo = mainWorld.getOurInfo();
            Double mainCubeX = mainCubeInfo.Item1;
            Double mainCubeY = mainCubeInfo.Item2;
            Double mainCubeWidth = mainCubeInfo.Item3;
            int transformX,transformY;

            foreach (KeyValuePair<int, Cube> cube in mainWorld.worldCubes)
            {

               transformX = (int)((cube.Value.loc_x - mainCubeX) + (mainWorld.worldWidth - mainCubeWidth) / 2);
               transformY = (int)((cube.Value.loc_y - mainCubeY) + (mainWorld.worldHieght - mainCubeWidth) / 2);

               Color color = Color.FromArgb(cube.Value.argb_color);
               myBrush = new System.Drawing.SolidBrush(color);
               textColor = new System.Drawing.SolidBrush(Color.Black);


               Rectangle rectangle = new Rectangle(transformX,transformY,(int)cube.Value.Width,(int)cube.Value.Width );

               e.Graphics.FillRectangle(myBrush, rectangle);
               Font myFont = new Font("Arial", 8);
               SizeF size = e.Graphics.MeasureString(cube.Value.Name, myFont);
               // Draw text in our cube only if it can fit - center text in the cube
               if ( (size.Width < rectangle.Size.Width) && (size.Height < rectangle.Size.Height))
               {
                  StringFormat sf = new StringFormat();
                  sf.LineAlignment = StringAlignment.Center;
                  sf.Alignment = StringAlignment.Center;
                  e.Graphics.DrawString(cube.Value.Name, myFont, textColor, transformX+(int)(cube.Value.Width/2), transformY + (int)(cube.Value.Width / 2), sf);
               }
            }
         }
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
            lock (mainWorld)
            {
               if (hasCubes && mainWorld.worldCubes[mainWorld.ourID].Mass == 0.0)
                  gameOver();
               //MessageBox.Show(line);
               mainWorld.processCube(line);
               hasCubes = true;
            }

         }
         isConnected = true;
         //MessageBox.Show("Get a new buffer");
         Network.i_want_more_data(state);
         worldSocket = state.workSocket;

      }

      private void gameOver()
      {
         worldSocket.Close();
         gameOverLabel.Show();
         //Invalidate();
         MessageBox.Show("WHAT");
      }

      /// <summary>
      /// TODO: process our json - extra brackets - incomplete lines etc. - return a String Array of clean JSON
      /// TODO: save the remainder in a 'remainingJson' string to prepend to our next set of incoming data
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
               //TODO: may want ot use an out parameter with this 
               remainingJson = line;
            }
            // ignore anything else
         }

         return cleanJson;
      }


   }
}
