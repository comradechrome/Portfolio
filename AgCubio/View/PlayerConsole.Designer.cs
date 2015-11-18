namespace AgCubio
{
    /// <summary>
    /// 
    /// </summary>
    partial class PlayerConsole
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
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.agCubioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.aboutAgCubioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.usingAgCubioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.textBox_fps = new System.Windows.Forms.TextBox();
         this.textBox_food = new System.Windows.Forms.TextBox();
         this.textBox_mass = new System.Windows.Forms.TextBox();
         this.textBox_width = new System.Windows.Forms.TextBox();
         this.textBox_playerName = new System.Windows.Forms.TextBox();
         this.textBox_serverName = new System.Windows.Forms.TextBox();
         this.label_playerName = new System.Windows.Forms.Label();
         this.label_serverName = new System.Windows.Forms.Label();
         this.gameOverLabel = new System.Windows.Forms.Label();
         this.menuStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.agCubioToolStripMenuItem,
            this.helpToolStripMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(756, 24);
         this.menuStrip1.TabIndex = 0;
         this.menuStrip1.Text = "menuStrip1";
         // 
         // agCubioToolStripMenuItem
         // 
         this.agCubioToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameToolStripMenuItem,
            this.quitToolStripMenuItem});
         this.agCubioToolStripMenuItem.Name = "agCubioToolStripMenuItem";
         this.agCubioToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
         this.agCubioToolStripMenuItem.Text = "AgCubio";
         // 
         // newGameToolStripMenuItem
         // 
         //this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
         //this.newGameToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
         //this.newGameToolStripMenuItem.Text = "New Game";
         //this.newGameToolStripMenuItem.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click);
         // 
         // quitToolStripMenuItem
         // 
         this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
         this.quitToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
         this.quitToolStripMenuItem.Text = "Quit";
         this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
         // 
         // helpToolStripMenuItem
         // 
         //this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
         //   this.aboutAgCubioToolStripMenuItem,
         //   this.usingAgCubioToolStripMenuItem});
         //this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
         //this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
         //this.helpToolStripMenuItem.Text = "Help";
         // 
         // aboutAgCubioToolStripMenuItem
         // 
         //this.aboutAgCubioToolStripMenuItem.Name = "aboutAgCubioToolStripMenuItem";
         //this.aboutAgCubioToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
         //this.aboutAgCubioToolStripMenuItem.Text = "About AgCubio";
         //this.aboutAgCubioToolStripMenuItem.Click += new System.EventHandler(this.aboutAgCubioToolStripMenuItem_Click);
         // 
         // usingAgCubioToolStripMenuItem
         // 
         //this.usingAgCubioToolStripMenuItem.Name = "usingAgCubioToolStripMenuItem";
         //this.usingAgCubioToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
         //this.usingAgCubioToolStripMenuItem.Text = "Using AgCubio";
         // 
         // textBox_fps
         // 
         this.textBox_fps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.textBox_fps.BackColor = System.Drawing.SystemColors.ButtonFace;
         this.textBox_fps.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.textBox_fps.ForeColor = System.Drawing.SystemColors.WindowText;
         this.textBox_fps.Location = new System.Drawing.Point(670, 27);
         this.textBox_fps.Name = "textBox_fps";
         this.textBox_fps.ReadOnly = true;
         this.textBox_fps.Size = new System.Drawing.Size(80, 13);
         this.textBox_fps.TabIndex = 1;
         this.textBox_fps.Text = "fps: ";
         // 
         // textBox_food
         // 
         this.textBox_food.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.textBox_food.BackColor = System.Drawing.SystemColors.ButtonFace;
         this.textBox_food.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.textBox_food.Location = new System.Drawing.Point(670, 47);
         this.textBox_food.Name = "textBox_food";
         this.textBox_food.ReadOnly = true;
         this.textBox_food.Size = new System.Drawing.Size(80, 13);
         this.textBox_food.TabIndex = 2;
         this.textBox_food.Text = "food: ";
         // 
         // textBox_mass
         // 
         this.textBox_mass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.textBox_mass.BackColor = System.Drawing.SystemColors.ButtonFace;
         this.textBox_mass.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.textBox_mass.Location = new System.Drawing.Point(670, 67);
         this.textBox_mass.Name = "textBox_mass";
         this.textBox_mass.ReadOnly = true;
         this.textBox_mass.Size = new System.Drawing.Size(80, 13);
         this.textBox_mass.TabIndex = 3;
         this.textBox_mass.Text = "mass: ";
         // 
         // textBox_width
         // 
         this.textBox_width.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.textBox_width.BackColor = System.Drawing.SystemColors.ButtonFace;
         this.textBox_width.BorderStyle = System.Windows.Forms.BorderStyle.None;
         this.textBox_width.Location = new System.Drawing.Point(670, 87);
         this.textBox_width.Name = "textBox_width";
         this.textBox_width.ReadOnly = true;
         this.textBox_width.Size = new System.Drawing.Size(80, 13);
         this.textBox_width.TabIndex = 4;
         this.textBox_width.Text = "width: ";
         // 
         // textBox_playerName
         // 
         this.textBox_playerName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
         this.textBox_playerName.Location = new System.Drawing.Point(335, 222);
         this.textBox_playerName.Name = "textBox_playerName";
         this.textBox_playerName.Size = new System.Drawing.Size(100, 20);
         this.textBox_playerName.TabIndex = 5;
         this.textBox_playerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_playerName_KeyDown);
         // 
         // textBox_serverName
         // 
         this.textBox_serverName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
         this.textBox_serverName.Location = new System.Drawing.Point(335, 261);
         this.textBox_serverName.Name = "textBox_serverName";
         this.textBox_serverName.Size = new System.Drawing.Size(100, 20);
         this.textBox_serverName.TabIndex = 6;
         this.textBox_serverName.Text = "localHost";
         this.textBox_serverName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_serverName_KeyDown);
         // 
         // label_playerName
         // 
         this.label_playerName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
         this.label_playerName.AutoSize = true;
         this.label_playerName.Location = new System.Drawing.Point(262, 222);
         this.label_playerName.Name = "label_playerName";
         this.label_playerName.Size = new System.Drawing.Size(67, 13);
         this.label_playerName.TabIndex = 7;
         this.label_playerName.Text = "Player Name";
         // 
         // label_serverName
         // 
         this.label_serverName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
         this.label_serverName.AutoSize = true;
         this.label_serverName.Location = new System.Drawing.Point(275, 264);
         this.label_serverName.Name = "label_serverName";
         this.label_serverName.Size = new System.Drawing.Size(38, 13);
         this.label_serverName.TabIndex = 8;
         this.label_serverName.Text = "Server";
         // 
         // gameOverLabel
         // 
         this.gameOverLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
         this.gameOverLabel.AutoSize = true;
         this.gameOverLabel.BackColor = System.Drawing.SystemColors.WindowText;
         this.gameOverLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.gameOverLabel.ForeColor = System.Drawing.Color.Red;
         this.gameOverLabel.Location = new System.Drawing.Point(193, 114);
         this.gameOverLabel.Name = "gameOverLabel";
         this.gameOverLabel.Size = new System.Drawing.Size(342, 73);
         this.gameOverLabel.TabIndex = 9;
         this.gameOverLabel.Text = "You Died!!";
         this.gameOverLabel.Visible = false;
         // 
         // PlayerConsole
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(756, 733);
         this.Controls.Add(this.gameOverLabel);
         this.Controls.Add(this.label_serverName);
         this.Controls.Add(this.label_playerName);
         this.Controls.Add(this.textBox_serverName);
         this.Controls.Add(this.textBox_playerName);
         this.Controls.Add(this.textBox_width);
         this.Controls.Add(this.textBox_mass);
         this.Controls.Add(this.textBox_food);
         this.Controls.Add(this.textBox_fps);
         this.Controls.Add(this.menuStrip1);
         this.ForeColor = System.Drawing.SystemColors.AppWorkspace;
         this.KeyPreview = true;
         this.MainMenuStrip = this.menuStrip1;
         this.Margin = new System.Windows.Forms.Padding(2);
         this.Name = "PlayerConsole";
         this.Text = "PlayerConsole";
         this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PlayerConsole_KeyDown);
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem agCubioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutAgCubioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usingAgCubioToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox_fps;
        private System.Windows.Forms.TextBox textBox_food;
        private System.Windows.Forms.TextBox textBox_mass;
        private System.Windows.Forms.TextBox textBox_width;
        private System.Windows.Forms.TextBox textBox_playerName;
        private System.Windows.Forms.TextBox textBox_serverName;
        private System.Windows.Forms.Label label_playerName;
        private System.Windows.Forms.Label label_serverName;
        private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
        private System.Windows.Forms.Label gameOverLabel;
    }
}

