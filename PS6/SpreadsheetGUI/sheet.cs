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
   /// Creating a SpreadsheetPanel object
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

         this.FormClosing += Form1_FormClosing;

         spreadsheetPanel1.SelectionChanged += selectCell;
         spreadsheetPanel1.SetSelection(0, 0);
      }

      /// <summary>
      /// Constructor for the Spreadsheet Form when opening an existing file
      /// </summary>
      /// <param name="openfile"></param>
      public Form1(String openfile)
      {
         // create our spreadsheet using an existing file, normalize all variables to use uppercase,
         //    validate variables start with a single letter followed by 1-99, set version to ps6
         mainSpreadsheet = new Spreadsheet(openfile, s => Regex.IsMatch(s, "[A-Z][1-9][0-9]?$"), s => s.ToUpper(), "ps6");

         InitializeComponent();

         spreadsheetPanel1.SelectionChanged += selectCell;
         //set our default cell to be A1
         spreadsheetPanel1.SetSelection(0, 0);

         // refresh all cells in the spreadsheet to display non-empty cells of imported spreadsheet file
         refreshCells(mainSpreadsheet.GetNamesOfAllNonemptyCells());
      }

      /// <summary>
      /// Get the current cell and refresh menu values
      /// </summary>
      /// <param name="ssPanel"></param>
      private void selectCell(SpreadsheetPanel ssPanel)
      {
         ssPanel.GetSelection(out col, out row);
         activeCell = getCellName(col, row);
         refreshMenu(activeCell);
      }

      /// <summary>
      /// catches the form close event - promts to save existing spreadsheet before we close
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void Form1_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (e.CloseReason == CloseReason.UserClosing)
            saveChange("closing");
      }

      /// <summary>
      /// New menu item - open a new spreadsheet
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void newToolStripMenuItem_Click(object sender, EventArgs e)
      {
         // Tell the application context to run the form on the same
         // thread as the other forms.
         SSApplicationContext.getAppContext().RunForm(new Form1());
      }

      /// <summary>
      /// Open Menu item - opens a previously saved spreadsheet
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void openToolStripMenuItem_Click(object sender, EventArgs e)
      {
         // make sure we save the existing spreadsheet 1st
         saveChange("overwriting");

         // open file dialog - including all defaults
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
               //get a list of nonEmpty cells
               IEnumerable<String> oldCells = mainSpreadsheet.GetNamesOfAllNonemptyCells();

               // create new spreadsheet using the file
               mainSpreadsheet = new Spreadsheet(openFileDialog1.FileName, s => Regex.IsMatch(s, "[A-Z][1-9][0-9]?$"), s => s.ToUpper(), "ps6");

               // CLear old form after successfuly opening new spreadsheet
               clearCells(oldCells);

               //update the spreadsheet and menu with the new values
               spreadsheetPanel1.SelectionChanged += selectCell;
               spreadsheetPanel1.SetSelection(0, 0);
               refreshCells(mainSpreadsheet.GetNamesOfAllNonemptyCells());
               refreshMenu("A1");

            }
            catch (Exception ex)
            {
               MessageBox.Show("Error: Could not read file: " + ex.Message, "Open File Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
         }
      }

      /// <summary>
      /// Save Menu item - saves the existing spreadsheet
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void saveToolStripMenuItem_Click(object sender, EventArgs e)
      {
         saveSpreadsheet();
      }


      /// <summary>
      /// Close menu item
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void closeToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Close();
      }

      /// <summary>
      /// converts column/row coordinates to a String cell name
      /// </summary>
      /// <param name="col"></param>
      /// <param name="row"></param>
      /// <returns></returns>
      private static String getCellName(int col, int row)
      {
         const String letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

         String column = letters.Substring(col, 1);
         // Increment Row so we start with Row 1
         row++;
         String newRow = row.ToString();
         return column + newRow;
      }

      /// <summary>
      /// converts a cell name to column/row coordinates
      /// </summary>
      /// <param name="cell"></param>
      /// <returns></returns>
      private static Tuple<int, int> getColRow(String cell)
      {
         int col, row;
         col = (int)(cell[0] - 65);
         int.TryParse(cell.Substring(1), out row);

         return new Tuple<int, int>(col, --row);
      }

      /// <summary>
      /// catches keyboard input events - return and arro keys
      /// Return behaives the same as the spreadsheet enter button
      /// Arrow keys can navigate the spreadsheet cells - logic prevents us from going outside the spreadsheet
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void input_KeyDown(object sender, KeyEventArgs e)
      {
         // return key is simply executes the enterButton click
         if (e.KeyCode == Keys.Return)
         {
            enterButton_Click(sender, e);
            e.SuppressKeyPress = true; // stop the annoying bing that Windows makes when you press a key
         }
         else if (e.KeyCode == Keys.Down)
         {
            if (spreadsheetPanel1.SetSelection(col, row + 1))
            {
               row++;
               refreshMenu(getCellName(col, row));
            }
         }
         else if (e.KeyCode == Keys.Up)
         {
            if (spreadsheetPanel1.SetSelection(col, row - 1))
            {
               row--;
               refreshMenu(getCellName(col, row));
            }

         }
         else if (e.KeyCode == Keys.Right)
         {
            if (spreadsheetPanel1.SetSelection(col + 1, row))
            {
               col++;
               refreshMenu(getCellName(col, row));
            }

         }
         else if (e.KeyCode == Keys.Left)
         {
            if (spreadsheetPanel1.SetSelection(col - 1, row))
            {
               col--;
               refreshMenu(getCellName(col, row));
            }

         }

      }

      /// <summary>
      /// 'Help -> About' menu item - provides information about the spreadheet
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
      {
         String aboutText = "SpreadSheet version 0.6\nDeveloped by team ellefsakishev\n\n" +
                            "Extra Functionality:\n" + "- Arrow key navigation\n" +
                            "- Cells with a FormulaError show '*Error*' in the cell\n" +
                            "- FormulaError popup error messages display the FormulaError reason\n" +
                            "- MSI Installer package";
         MessageBox.Show(aboutText, "About", MessageBoxButtons.OK, MessageBoxIcon.Question);

      }

      /// <summary>
      /// 'Help -> Using' menu item - provides information on how to use the spreadheet
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void spreadsheetUsageToolStripMenuItem_Click(object sender, EventArgs e)
      {
         // a better design would be to use the Help Class and compiled Help files (.chm)
         String aboutText = "To use the spreadsheet:\n- Use the mouse or arrow keys to navigate the spreadsheet cells.\n" +
                            "- Enter a String, Double, or Formula into the contents box and either click the 'Enter' button or " +
                            "enter key on the keyboard. (Note: formulas start with a '=')\n" +
                            "- The upper tools bar displays the current cell name, value, and contents.\n" +
                            "- The menu has 'new', 'open', 'save', and 'close' functions.\n- The default file extension for " +
                            "spreadsheets is '.sprd'";

         MessageBox.Show(aboutText, "Usage", MessageBoxButtons.OK, MessageBoxIcon.Question);

      }

      /// <summary>
      /// saves the spreadsheet - default file extension is sprd
      /// Much of this code and comments were pulled from MSDN
      /// </summary>
      private void saveSpreadsheet()
      {
         // Set the properties on SaveFileDialog1 so the user is 
         // prompted to create the file if it doesn't exist 
         // or overwrite the file if it does exist.
         SaveFileDialog saveFileDialog1 = new SaveFileDialog();

         saveFileDialog1.CreatePrompt = true;
         saveFileDialog1.OverwritePrompt = true;

         // Set the type filter to sprd files and set the
         // initial directory to the MyDocuments folder.
         // DefaultExt is only used when "All files" is selected from 
         // the filter box and no extension is specified by the user.
         //saveFileDialog1.DefaultExt = "sprd";
         saveFileDialog1.Filter =
             "sprd files (*.sprd)|*.sprd|All files (*.*)|*.*";
         saveFileDialog1.InitialDirectory =
             Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

         // Call ShowDialog and check for a return value of DialogResult.OK,
         // which indicates that the file was saved. 
         DialogResult result = saveFileDialog1.ShowDialog();

         // save the spreadsheet
         try
         {
            mainSpreadsheet.Save(saveFileDialog1.FileName);
         }
         catch (Exception ex)
         {
            MessageBox.Show("Error saving spreadsheet.\nInfo: " + ex, "Save Error",
               MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      /// <summary>
      /// Check to see if spreadsheet has been changed, if so open a dialog box asking to save
      /// </summary>
      /// <param name="closeType"></param>
      private void saveChange(String closeType)
      {
         if (mainSpreadsheet.Changed)
         {
            DialogResult result = MessageBox.Show("Existing Spreadsheet has been modified. " +
               "Would you like to save before " + closeType + " the existing spreadsheet?",
               "Unsaved Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (result == DialogResult.Yes)
            {
               saveSpreadsheet();
            }

         }
      }

      /// <summary>
      /// update the menu information after a cell change. Also update the active cell
      /// </summary>
      /// <param name="cell"></param>
      private void refreshMenu(String cell)
      {
         cellNameTextBox.Text = cell;
         cellValueTextBox.Text = mainSpreadsheet.GetCellValue(cell).ToString();
         cellContentsTextBox.Text = mainSpreadsheet.GetCellContents(cell).ToString();
         cellContentsTextBox.Focus();
         activeCell = getCellName(col, row);
      }

      /// <summary>
      /// Clear all nonEmpty cells in the spreadsheet form
      /// </summary>
      /// <param name="toUpdate"></param>
      private void clearCells(IEnumerable<String> toUpdate)
      {
         Tuple<int, int> temp;

         foreach (string el in toUpdate)
         {
            temp = getColRow(el);
            spreadsheetPanel1.SetValue(temp.Item1, temp.Item2, "");
         }
      }

      /// <summary>
      /// Update the form with all nonEmpty spreadsheet cells 
      /// </summary>
      /// <param name="toUpdate"></param>
      private void refreshCells(IEnumerable<String> toUpdate)
      {
         Tuple<int, int> temp;

         foreach (string el in toUpdate)
         {

            temp = getColRow(el);
            object cellValue = mainSpreadsheet.GetCellValue(el);

            // special handling if the value is FormulaError - display *Cell Error*
            if (cellValue is FormulaError)
            {
               FormulaError cellError = (FormulaError)cellValue;
               spreadsheetPanel1.SetValue(temp.Item1, temp.Item2, "*Cell Error*");
            }
            else
               spreadsheetPanel1.SetValue(temp.Item1, temp.Item2, mainSpreadsheet.GetCellValue(el).ToString());
         }
      }

      /// <summary>
      /// handle the 'Enter' button click
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void enterButton_Click(object sender, EventArgs e)
      {
         Tuple<int, int> temp;
         try
         {
            // set the spreadsheet cell with contents, get all cell dependents, then itterate through the 
            // list updating cells as needed
            ISet<string> ToUpdate = mainSpreadsheet.SetContentsOfCell(activeCell, cellContentsTextBox.Text);
            cellValueTextBox.Text = mainSpreadsheet.GetCellValue(activeCell).ToString();
            foreach (string el in ToUpdate)
            {

               temp = getColRow(el);
               object cellValue = mainSpreadsheet.GetCellValue(el);

               // special FOrmulaError handling
               if (cellValue is FormulaError)
               {
                  FormulaError cellError = (FormulaError)cellValue;
                  // set form cell to display *Cell Error*
                  spreadsheetPanel1.SetValue(temp.Item1, temp.Item2, "*Cell Error*");
                  // open a message box displaying the FormulaError reason
                  MessageBox.Show("Error in cell " + el + "\nError Info: " +
                     cellError.Reason, "Cell Error", MessageBoxButtons.OK,
                     MessageBoxIcon.Warning);
               }
               else
                  spreadsheetPanel1.SetValue(temp.Item1, temp.Item2, mainSpreadsheet.GetCellValue(el).ToString());
            }
            // refresh the menu
            refreshMenu(activeCell);
         }
         catch (Exception ex)
         {
            MessageBox.Show("Invalid entry in " + activeCell + " \n Error:" + ex, "Cell Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
         }
      }
   }
}
