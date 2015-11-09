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
      public PlayerConsole()
      {




         world = new World(1000, 1000, "name", "server");


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
       
            foreach (Cube cube1 in world.cubes)
            {

               //MessageBox.Show("cube:" + cube1.uid);
               Color color = Color.FromArgb(cube1.argb_color);
               myBrush = new System.Drawing.SolidBrush(color);
               //textColor = new System.Drawing.SolidBrush(Color.Black);
               Rectangle rectangle = new Rectangle((int)cube1.loc_x, (int)cube1.loc_y, (int)cube1.Width, (int)cube1.Width);
               e.Graphics.FillRectangle(myBrush, rectangle);
               //Font myFont = new Font("Arial", 10);
               //e.Graphics.DrawString(cube1.Name, myFont, textColor, (int)cube1.loc_x, (int)cube1.loc_y);
               Invalidate();
            }
            //Invalidate();
         
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

      private void PlayerConsole_Load(object sender, EventArgs e)
      {

      }

      private void width_TextChanged(object sender, EventArgs e)
      {

      }

      private void fps_TextChanged(object sender, EventArgs e)
      {

      }

      private void food_TextChanged(object sender, EventArgs e)
      {

      }

      private void mass_TextChanged(object sender, EventArgs e)
      {

      }
   }
}
