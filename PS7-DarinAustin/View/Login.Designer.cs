namespace AgCubio
{
    partial class Login
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.userNameBox = new System.Windows.Forms.TextBox();
            this.serverNameBox = new System.Windows.Forms.TextBox();
            this.userNameLabel = new System.Windows.Forms.Label();
            this.serverNameLabel = new System.Windows.Forms.Label();
            this.loginButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // userNameBox
            // 
            this.userNameBox.Location = new System.Drawing.Point(174, 78);
            this.userNameBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.userNameBox.Name = "userNameBox";
            this.userNameBox.Size = new System.Drawing.Size(277, 26);
            this.userNameBox.TabIndex = 0;
            this.userNameBox.Text = "Chuck Norris";
            this.userNameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.userNameBox_KeyDown);
            // 
            // serverNameBox
            // 
            this.serverNameBox.Location = new System.Drawing.Point(174, 158);
            this.serverNameBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.serverNameBox.Name = "serverNameBox";
            this.serverNameBox.Size = new System.Drawing.Size(277, 26);
            this.serverNameBox.TabIndex = 1;
            this.serverNameBox.Text = "localhost";
            this.serverNameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.serverNameBox_KeyDown);
            // 
            // userNameLabel
            // 
            this.userNameLabel.AutoSize = true;
            this.userNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userNameLabel.Location = new System.Drawing.Point(75, 78);
            this.userNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.userNameLabel.Name = "userNameLabel";
            this.userNameLabel.Size = new System.Drawing.Size(64, 25);
            this.userNameLabel.TabIndex = 2;
            this.userNameLabel.Text = "Name";
            // 
            // serverNameLabel
            // 
            this.serverNameLabel.AutoSize = true;
            this.serverNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverNameLabel.Location = new System.Drawing.Point(75, 158);
            this.serverNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.serverNameLabel.Name = "serverNameLabel";
            this.serverNameLabel.Size = new System.Drawing.Size(70, 25);
            this.serverNameLabel.TabIndex = 3;
            this.serverNameLabel.Text = "Server";
            // 
            // loginButton
            // 
            this.loginButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginButton.Location = new System.Drawing.Point(339, 222);
            this.loginButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(112, 40);
            this.loginButton.TabIndex = 4;
            this.loginButton.Text = "Login";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 303);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.serverNameLabel);
            this.Controls.Add(this.userNameLabel);
            this.Controls.Add(this.serverNameBox);
            this.Controls.Add(this.userNameBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AgCubio Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox userNameBox;
        private System.Windows.Forms.TextBox serverNameBox;
        private System.Windows.Forms.Label userNameLabel;
        private System.Windows.Forms.Label serverNameLabel;
        private System.Windows.Forms.Button loginButton;
    }
}

