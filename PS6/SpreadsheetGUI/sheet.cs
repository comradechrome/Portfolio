using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using System.IO;

namespace SS
{
   /// <summary>
   /// Example of using a SpreadsheetPanel object
   /// </summary>
   public partial class Form1 : Form
   {
      Spreadsheet mainSpreadsheet;
      String activeCell = "A1";
      private int col, row;
      /// <summary>
      /// Default Constructor for the Spreadsheet Form
      /// </summary>
      public Form1()
      {
         mainSpreadsheet = new Spreadsheet(s => Regex.IsMatch(s, "[A-Z][1-9][0-9]?$"), s => s.ToUpper(), "ps6");

         InitializeComponent();

         spreadsheetPanel1.SelectionChanged += selectCell;
         spreadsheetPanel1.SetSelection(0, 0);
      }

      /// <summary>
      /// Default Constructor for the Spreadsheet Form
      /// </summary>
      public Form1(String openfile)
      {
         mainSpreadsheet = new Spreadsheet(openfile, s => Regex.IsMatch(s, "[A-Z][1-9][0-9]?$"), s => s.ToUpper(), "ps6");

         InitializeComponent();

         spreadsheetPanel1.SelectionChanged += selectCell;
         spreadsheetPanel1.SetSelection(0, 0);

         refreshCells(mainSpreadsheet.GetNamesOfAllNonemptyCells());
      }


      private void selectCell(SpreadsheetPanel ssPanel)
      {
         ssPanel.GetSelection(out col, out row);
         activeCell = getCellName(col, row);
         cellNameTextBox.Text = activeCell;
         cellValueTextBox.Text = mainSpreadsheet.GetCellValue(activeCell).ToString();
         cellContentsTextBox.Text = mainSpreadsheet.GetCellContents(activeCell).ToString();
         cellContentsTextBox.Focus();
      }

      // Deals with the New menu
      private void newToolStripMenuItem_Click(object sender, EventArgs e)
      {
         // Tell the application context to run the form on the same
         // thread as the other forms.
         DemoApplicationContext.getAppContext().RunForm(new Form1());
      }

      private void openToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Stream myStream = null;
         OpenFileDialog openFileDialog1 = new OpenFileDialog();

         openFileDialog1.InitialDirectory = "c:\\";
         openFileDialog1.Filter = 
            "sprd files (*.sprd)|*.sprd|All files (*.*)|*.*";
         openFileDialog1.FilterIndex = 1;
         openFileDialog1.RestoreDirectory = true;
         openFileDialog1.InitialDirectory =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

         if (openFileDialog1.ShowDialog() == DialogResult.OK)
         {
            try
            {
                DemoApplicationContext.getAppContext().RunForm(new Form1(openFileDialog1.FileName));
               
            }
            catch (Exception ex)
            {
               MessageBox.Show("Error: Could not read file: " + ex.Message, "Open File Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
         }
      }

      private void saveToolStripMenuItem_Click(object sender, EventArgs e)
      {
         // Set the properties on SaveFileDialog1 so the user is 
         // prompted to create the file if it doesn't exist 
         // or overwrite the file if it does exist.
         SaveFileDialog saveFileDialog1 = new SaveFileDialog();

         saveFileDialog1.CreatePrompt = true;
         saveFileDialog1.OverwritePrompt = true;

         // Set the file name to myText.txt, set the type filter
         // to sprd files, and set the initial directory to the 
         // MyDocuments folder.
         saveFileDialog1.FileName = "spreadsheet";
         // DefaultExt is only used when "All files" is selected from 
         // the filter box and no extension is specified by the user.
         saveFileDialog1.DefaultExt = "sprd";
         saveFileDialog1.Filter =
             "sprd files (*.sprd)|*.sprd|All files (*.*)|*.*";
         saveFileDialog1.InitialDirectory =
             Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

         // Call ShowDialog and check for a return value of DialogResult.OK,
         // which indicates that the file was saved. 
         DialogResult result = saveFileDialog1.ShowDialog();

         // save the spreadsheet
         mainSpreadsheet.Save(saveFileDialog1.FileName);

      }


      // Deals with the Close menu
      private void closeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Close();
      }

      private static String getCellName(int col, int row)
      {
         const String letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

         String column = letters.Substring(col, 1);
         // Increment Row so we start with Row 1
         row++;
         String newRow = row.ToString();
         return column + newRow;
      }

      private static Tuple<int, int> getColRow(String cell)
      {
         int col, row;
         col = (int)(cell[0] - 65);
         int.TryParse(cell.Substring(1), out row);

         return new Tuple<int, int>(col, --row);
      }


      private void input_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.Return)
         {
            enterButton_Click(sender, e);
            e.SuppressKeyPress = true; // stop the annoying bing that Windows makes when you press a key
         }
      }

      private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
      {

      }

      private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
      {
         String aboutText = "SpreadSheet version 0.6\n Developed by team ellefsakishev";
         MessageBox.Show(aboutText, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);

      }

      private void spreadsheetUsageToolStripMenuItem_Click(object sender, EventArgs e)
      {
         String aboutText = "To use the spreadsheet: ";
         MessageBox.Show(aboutText, "Usage", MessageBoxButtons.OK, MessageBoxIcon.Question);

      }

      private void refreshCells(IEnumerable<String> toUpdate)
      {
         Tuple<int, int> temp;

         foreach (string el in toUpdate)
         {

            temp = getColRow(el);
            object cellValue = mainSpreadsheet.GetCellValue(el);

            if (cellValue is FormulaError)
            {
               FormulaError cellError = (FormulaError)cellValue;
               spreadsheetPanel1.SetValue(temp.Item1, temp.Item2, "*Cell Error*");
            }
            else
               spreadsheetPanel1.SetValue(temp.Item1, temp.Item2, mainSpreadsheet.GetCellValue(el).ToString());
         }
      }
    
      private void enterButton_Click(object sender, EventArgs e)
      {
         Tuple<int, int> temp;
         try
         {

            ISet<string> ToUpdate = mainSpreadsheet.SetContentsOfCell(activeCell, cellContentsTextBox.Text);
            cellValueTextBox.Text = mainSpreadsheet.GetCellValue(activeCell).ToString();
            foreach (string el in ToUpdate)
            {

               temp = getColRow(el);
               object cellValue = mainSpreadsheet.GetCellValue(el);

               if (cellValue is FormulaError)
               {
                  FormulaError cellError = (FormulaError)cellValue;
                  spreadsheetPanel1.SetValue(temp.Item1, temp.Item2, "*Cell Error*");
                  MessageBox.Show(cellError.Reason,"Cell Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
               }
               else
                  spreadsheetPanel1.SetValue(temp.Item1, temp.Item2, mainSpreadsheet.GetCellValue(el).ToString());
            }
         }
         catch (Exception ex)
         {
            MessageBox.Show("Invalid entry in " + activeCell + " \n Error:" + ex, "Cell Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
         }
      }
   }
}
