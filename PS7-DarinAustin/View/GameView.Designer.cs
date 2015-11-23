namespace AgCubio
{
    partial class gameView
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
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.statsTable = new System.Windows.Forms.TableLayoutPanel();
            this.nameLabel = new System.Windows.Forms.Label();
            this.massLabel = new System.Windows.Forms.Label();
            this.fpsLabel = new System.Windows.Forms.Label();
            this.xPosLabel = new System.Windows.Forms.Label();
            this.yPosLabel = new System.Windows.Forms.Label();
            this.viewportPanel = new System.Windows.Forms.PictureBox();
            this.mainTableLayout.SuspendLayout();
            this.statsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewportPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.ColumnCount = 1;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Controls.Add(this.statsTable, 0, 1);
            this.mainTableLayout.Controls.Add(this.viewportPanel, 0, 0);
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayout.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.RowCount = 2;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 97F));
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.mainTableLayout.Size = new System.Drawing.Size(984, 962);
            this.mainTableLayout.TabIndex = 0;
            // 
            // statsTable
            // 
            this.statsTable.ColumnCount = 5;
            this.statsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.statsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.statsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.statsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.statsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.statsTable.Controls.Add(this.nameLabel, 0, 0);
            this.statsTable.Controls.Add(this.massLabel, 1, 0);
            this.statsTable.Controls.Add(this.fpsLabel, 2, 0);
            this.statsTable.Controls.Add(this.xPosLabel, 3, 0);
            this.statsTable.Controls.Add(this.yPosLabel, 4, 0);
            this.statsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsTable.Location = new System.Drawing.Point(2, 935);
            this.statsTable.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.statsTable.Name = "statsTable";
            this.statsTable.RowCount = 1;
            this.statsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.statsTable.Size = new System.Drawing.Size(980, 25);
            this.statsTable.TabIndex = 1;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameLabel.Location = new System.Drawing.Point(2, 0);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(192, 25);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "nameLabel";
            this.nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // massLabel
            // 
            this.massLabel.AutoSize = true;
            this.massLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.massLabel.Location = new System.Drawing.Point(198, 0);
            this.massLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.massLabel.Name = "massLabel";
            this.massLabel.Size = new System.Drawing.Size(192, 25);
            this.massLabel.TabIndex = 1;
            this.massLabel.Text = "massLabel";
            this.massLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = true;
            this.fpsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fpsLabel.Location = new System.Drawing.Point(394, 0);
            this.fpsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(192, 25);
            this.fpsLabel.TabIndex = 2;
            this.fpsLabel.Text = "fpsLabel";
            this.fpsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // xPosLabel
            // 
            this.xPosLabel.AutoSize = true;
            this.xPosLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xPosLabel.Location = new System.Drawing.Point(590, 0);
            this.xPosLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.xPosLabel.Name = "xPosLabel";
            this.xPosLabel.Size = new System.Drawing.Size(192, 25);
            this.xPosLabel.TabIndex = 3;
            this.xPosLabel.Text = "xPosLabel";
            this.xPosLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // yPosLabel
            // 
            this.yPosLabel.AutoSize = true;
            this.yPosLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.yPosLabel.Location = new System.Drawing.Point(786, 0);
            this.yPosLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.yPosLabel.Name = "yPosLabel";
            this.yPosLabel.Size = new System.Drawing.Size(192, 25);
            this.yPosLabel.TabIndex = 4;
            this.yPosLabel.Text = "yPosLabel";
            this.yPosLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // viewportPanel
            // 
            this.viewportPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewportPanel.Location = new System.Drawing.Point(2, 2);
            this.viewportPanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.viewportPanel.Name = "viewportPanel";
            this.viewportPanel.Size = new System.Drawing.Size(980, 929);
            this.viewportPanel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.viewportPanel.TabIndex = 2;
            this.viewportPanel.TabStop = false;
            this.viewportPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.viewportPanel_Paint);
            // 
            // gameView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 962);
            this.Controls.Add(this.mainTableLayout);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 1000);
            this.MinimumSize = new System.Drawing.Size(1000, 1000);
            this.Name = "gameView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "GameView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.gameView_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gameView_KeyDown);
            this.mainTableLayout.ResumeLayout(false);
            this.mainTableLayout.PerformLayout();
            this.statsTable.ResumeLayout(false);
            this.statsTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.viewportPanel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.TableLayoutPanel statsTable;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label massLabel;
        private System.Windows.Forms.Label fpsLabel;
        private System.Windows.Forms.Label xPosLabel;
        private System.Windows.Forms.Label yPosLabel;
        private System.Windows.Forms.PictureBox viewportPanel;
    }
}