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

namespace AgCubio
{
   public partial class PlayerConsole : Form
   {

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

         world = new World(1000, 1000, ourName, ourServer);

         //temporary data test method
         buildWorld();

         InitializeComponent();
         DoubleBuffered = true;
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
   }
}
