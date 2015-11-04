I certify that the work to create this GUI was done entirely by myself and my partner - Andrey Myakishev, Randall Ellefsen

- SpreadsheetPannel.dll was modified to use AnyCPU. This eliminated the CPU warning. 
	This DLL was built from PSon 10/30/2015.
- Formula.dll was built from Andreys PS3 master branch on 10/30/2015.
- SpreadsheetUtilities.dll was built from Andreys PS2 master branch on 10/30/2015.
- Spreadsheet.dll was built from Andreys PS4 branch PS6 on 10/30/2015.

Required Features:
- One or more independent spreadsheets can be opened
- Application closes when last spreadsheet is closes
- Grid of 26 columns (A-Z: case insensitive) X 99 Rows with scroll bar
- Cell value is displayed in Grid cell (String, Double, FormulaError) - cell is not editable
- One cell is always highlighted (default cell is A1)
- Mouse pointer selects cells
- Menu contains these non-editable boxes:
	- cell name
	- cell value
- Menu contains editable contents box (this is how data is entered and modified)
- Popup box is displayed on cell errors (formula errors, bad lookups, circular, etc.)
- Save and Open functionality - default file extention is .sprd
- warning if operation will result in data loss (close before save)
- Help menu contains info on how to use spreadsheet as well as extra features
- Added "Save As..." menu item wich will always prompt for a filename
- Cell updates are done in the background worker thread

Extra Features:
- arrow key navigation
- cells with a FormulaError show '*Error*' in the cell
- FormulaError popup error messages display the FormulaError reason
- MSI Installer package - modified file types to have this program open up .sprd files. If an existing .sprd file is double
	clicked, that spreadsheet will open up.

UI Testing:
- UI Testing acheived 98.45 % code coverage - we were not able to test 3 items. This prevented us from having 100% code coverage.
	* Catch block in the openToolStripMenuItem_Click method was not covered. This is checking for a difficult to 
		test exception where a file would be attempted to be opened but could not be read by the spreadsheet constructor
	* A portion of the Dispose method was not able to be covered by existing tests
	* The code required to open up existing spreadsheets was not tested. This would be dificult to test on a remote machine as the file may or may not exist.
	* I noticed that UI testing would not work on the lab machines. I received a COM Exception error. Please contact me if you have questions
		on the UI Testing. It works perfectly on my laptop. Posibly an issue with the networked "My Documents". I also noticed that when running the tests,
		VS tried to install some kind of package.