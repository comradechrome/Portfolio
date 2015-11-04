namespace SS
{
   partial class Form1
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
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.spreadsheetUsageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.fileMenuStrip = new System.Windows.Forms.MenuStrip();
         this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
         this.cellNameTextBox = new System.Windows.Forms.ToolStripTextBox();
         this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
         this.cellValueTextBox = new System.Windows.Forms.ToolStripTextBox();
         this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
         this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
         this.cellContentsTextBox = new System.Windows.Forms.ToolStripTextBox();
         this.toolStrip1 = new System.Windows.Forms.ToolStrip();
         this.spreadsheetPanel1 = new SS.SpreadsheetPanel();
         this.enterButton = new System.Windows.Forms.Button();
         this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
         this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.fileMenuStrip.SuspendLayout();
         this.toolStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // fileToolStripMenuItem
         // 
         this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.closeToolStripMenuItem});
         this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
         this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
         this.fileToolStripMenuItem.Text = "File";
         // 
         // newToolStripMenuItem
         // 
         this.newToolStripMenuItem.Name = "newToolStripMenuItem";
         this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
         this.newToolStripMenuItem.Text = "New";
         this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
         // 
         // openToolStripMenuItem
         // 
         this.openToolStripMenuItem.Name = "openToolStripMenuItem";
         this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
         this.openToolStripMenuItem.Text = "Open";
         this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
         // 
         // saveToolStripMenuItem
         // 
         this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
         this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
         this.saveToolStripMenuItem.Text = "Save";
         this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
         // 
         // closeToolStripMenuItem
         // 
         this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
         this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
         this.closeToolStripMenuItem.Text = "Close";
         this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
         // 
         // helpToolStripMenuItem
         // 
         this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spreadsheetUsageToolStripMenuItem,
            this.aboutToolStripMenuItem});
         this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
         this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
         this.helpToolStripMenuItem.Text = "Help";
         // 
         // spreadsheetUsageToolStripMenuItem
         // 
         this.spreadsheetUsageToolStripMenuItem.Name = "spreadsheetUsageToolStripMenuItem";
         this.spreadsheetUsageToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
         this.spreadsheetUsageToolStripMenuItem.Text = "Spreadsheet Usage";
         this.spreadsheetUsageToolStripMenuItem.Click += new System.EventHandler(this.spreadsheetUsageToolStripMenuItem_Click);
         // 
         // aboutToolStripMenuItem
         // 
         this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
         this.aboutToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
         this.aboutToolStripMenuItem.Text = "About Spreadsheet";
         this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
         // 
         // fileMenuStrip
         // 
         this.fileMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
         this.fileMenuStrip.Location = new System.Drawing.Point(0, 0);
         this.fileMenuStrip.Name = "fileMenuStrip";
         this.fileMenuStrip.Size = new System.Drawing.Size(797, 24);
         this.fileMenuStrip.TabIndex = 1;
         this.fileMenuStrip.Text = "menuStrip1";
         // 
         // toolStripLabel1
         // 
         this.toolStripLabel1.Name = "toolStripLabel1";
         this.toolStripLabel1.Size = new System.Drawing.Size(62, 22);
         this.toolStripLabel1.Text = "Cell Name";
         // 
         // cellNameTextBox
         // 
         this.cellNameTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.cellNameTextBox.Name = "cellNameTextBox";
         this.cellNameTextBox.ReadOnly = true;
         this.cellNameTextBox.Size = new System.Drawing.Size(25, 25);
         this.cellNameTextBox.Text = "A1";
         // 
         // toolStripLabel2
         // 
         this.toolStripLabel2.Name = "toolStripLabel2";
         this.toolStripLabel2.Size = new System.Drawing.Size(59, 22);
         this.toolStripLabel2.Text = "Cell Value";
         // 
         // cellValueTextBox
         // 
         this.cellValueTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.cellValueTextBox.Name = "cellValueTextBox";
         this.cellValueTextBox.ReadOnly = true;
         this.cellValueTextBox.Size = new System.Drawing.Size(200, 25);
         // 
         // toolStripSeparator
         // 
         this.toolStripSeparator.Name = "toolStripSeparator";
         this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
         // 
         // toolStripLabel3
         // 
         this.toolStripLabel3.Name = "toolStripLabel3";
         this.toolStripLabel3.Size = new System.Drawing.Size(78, 22);
         this.toolStripLabel3.Text = "Cell Contents";
         // 
         // cellContentsTextBox
         // 
         this.cellContentsTextBox.Name = "cellContentsTextBox";
         this.cellContentsTextBox.Size = new System.Drawing.Size(100, 25);
         this.cellContentsTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_KeyDown);
         // 
         // toolStrip1
         // 
         this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
         this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.cellNameTextBox,
            this.toolStripLabel2,
            this.cellValueTextBox,
            this.toolStripSeparator,
            this.toolStripLabel3,
            this.cellContentsTextBox});
         this.toolStrip1.Location = new System.Drawing.Point(0, 24);
         this.toolStrip1.Name = "toolStrip1";
         this.toolStrip1.Size = new System.Drawing.Size(797, 25);
         this.toolStrip1.TabIndex = 2;
         this.toolStrip1.Text = "toolStrip1";
         // 
         // spreadsheetPanel1
         // 
         this.spreadsheetPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.spreadsheetPanel1.Location = new System.Drawing.Point(0, 52);
         this.spreadsheetPanel1.Name = "spreadsheetPanel1";
         this.spreadsheetPanel1.Size = new System.Drawing.Size(797, 414);
         this.spreadsheetPanel1.TabIndex = 3;
         // 
         // enterButton
         // 
         this.enterButton.Location = new System.Drawing.Point(540, 24);
         this.enterButton.Name = "enterButton";
         this.enterButton.Size = new System.Drawing.Size(75, 23);
         this.enterButton.TabIndex = 4;
         this.enterButton.Text = "Enter";
         this.enterButton.UseVisualStyleBackColor = true;
         this.enterButton.Click += new System.EventHandler(this.enterButton_Click);
         // 
         // backgroundWorker1
         // 
         this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
         // 
         // saveAsToolStripMenuItem
         // 
         this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
         this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.saveAsToolStripMenuItem.Text = "Save As ...";
         this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(797, 467);
         this.Controls.Add(this.enterButton);
         this.Controls.Add(this.spreadsheetPanel1);
         this.Controls.Add(this.toolStrip1);
         this.Controls.Add(this.fileMenuStrip);
         this.MainMenuStrip = this.fileMenuStrip;
         this.Name = "Form1";
         this.Text = "Sheet";
         this.fileMenuStrip.ResumeLayout(false);
         this.fileMenuStrip.PerformLayout();
         this.toolStrip1.ResumeLayout(false);
         this.toolStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
      private System.Windows.Forms.MenuStrip fileMenuStrip;
      private System.Windows.Forms.ToolStripLabel toolStripLabel1;
      private System.Windows.Forms.ToolStripTextBox cellNameTextBox;
      private System.Windows.Forms.ToolStripLabel toolStripLabel2;
      private System.Windows.Forms.ToolStripTextBox cellValueTextBox;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
      private System.Windows.Forms.ToolStripLabel toolStripLabel3;
      private System.Windows.Forms.ToolStripTextBox cellContentsTextBox;
      private System.Windows.Forms.ToolStrip toolStrip1;
      private SpreadsheetPanel spreadsheetPanel1;
      private System.Windows.Forms.Button enterButton;
      private System.Windows.Forms.ToolStripMenuItem spreadsheetUsageToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
      private System.ComponentModel.BackgroundWorker backgroundWorker1;
      private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
   }

}