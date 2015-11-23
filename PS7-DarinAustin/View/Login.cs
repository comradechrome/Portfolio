using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgCubio
{
    /// <summary>
    /// Login form.
    /// </summary>
    public partial class Login : Form
    {
        /// <summary>
        /// Initialize login form
        /// </summary>
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Starts gameView form.
        /// </summary>
        /// <param name="sender">Sender object (button)</param>
        /// <param name="e">Event arguments</param>
        private void loginButton_Click(object sender, EventArgs e)
        {
            DoLogin();
        }

        /// <summary>
        /// Allows user to simply press enter to login
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Key event args</param>
        private void serverNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                DoLogin();
            }
        }

        /// <summary>
        /// Handles simple form validation and creating new gameView form
        /// </summary>
        private void DoLogin()
        {
            string userName = userNameBox.Text;
            string serverName = serverNameBox.Text;

            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(serverName))
            {
                MessageBox.Show("Please input a name and server.", "Login Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            else
            {
                // creates gameView
                gameView gameView = new gameView(userName, serverName);
                gameView.Show(); // shows gameView
                this.Hide(); // hides login screen
            }
            return;
        }

        /// <summary>
        /// Allows user to simply press enter to login
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">Key event args</param>
        private void userNameBox_KeyDown(object sender, KeyEventArgs e)
        {   
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                DoLogin();
            }  
        }
    }
}
