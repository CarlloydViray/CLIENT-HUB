using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using DataTable = System.Data.DataTable;
using ICell = NPOI.SS.UserModel.ICell;

namespace BPOI_HUB.modules.windows
{
    public class ExcelOps
    {
        public enum OUTPUT_TYPE
        {
            DICTIONARY,
            DATATABLE
        }

        public static IWorkbook ReadWorkBook(string path)
        {

            IWorkbook? book;

            if (Path.GetExtension(path) == ".xls")
            {
                try
                {
                    FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    book = new HSSFWorkbook(fs);
                }
                catch
                {
                    book = null;
                }
            }
            else
            {
                try
                {
                    FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    book = new XSSFWorkbook(fs);

                }
                catch
                {
                    book = null;
                }
            }

            return book;
        }

        public static DataTable LoadConfigFile(string file, int startIndex = 0)
        {
            DataTable config_file = new();

            IWorkbook book = ReadWorkBook(file);

            ISheet ws = book.GetSheetAt(0);

            int maxColumn = GetMaxColumn(ws);

            for (int rowIndex = startIndex; rowIndex <= ws.LastRowNum; rowIndex++)
            {
                IRow row = ws.GetRow(rowIndex);


                if (row != null)
                {

                    if (rowIndex == startIndex)
                    {
                        for (int colIndex = 0; colIndex < maxColumn; colIndex++)
                            config_file.Columns.Add(GetCellText(row.GetCell(colIndex)));
                    }
                    else
                    {
                        object[] data = new object[maxColumn];

                        for (int colIndex = 0; colIndex < maxColumn; colIndex++)
                            data[colIndex] = GetCellText(row.GetCell(colIndex));

                        config_file.Rows.Add(data);
                    }


                }
            }


            return config_file;
        }

        public static List<string> GetSheetList(IWorkbook workbook, bool visibleOnly = true)
        {
            int sheetCount = workbook.NumberOfSheets;

            List<string> sheetList = new();


            if (visibleOnly)
            {
                for (int i = 0; i < sheetCount; i++)
                {
                    if (workbook.IsSheetHidden(i) == false && workbook.IsSheetVeryHidden(i) == false)
                    {
                        ISheet sheet = workbook.GetSheetAt(i);

                        sheetList.Add(sheet.SheetName);
                    }
                }
            }
            else
            {
                for (int i = 0; i < sheetCount; i++)
                {
                    ISheet sheet = workbook.GetSheetAt(i);

                    sheetList.Add(sheet.SheetName);
                }
            }

            return sheetList;
        }

        public static int GetMaxColumn(ISheet ws)
        {
            int maxCol = 0;
            for (int i = 0; i <= ws.LastRowNum; i++)
            {
                if (ws.GetRow(i) != null)
                    if (ws.GetRow(i).LastCellNum > maxCol)
                    {
                        maxCol = ws.GetRow(i).LastCellNum;
                    }
            }

            return maxCol;
        }

        public static void WriteExcelData(string file, string sheet, Dictionary<string, Dictionary<string, string>> data)
        {
            Dictionary<string, int> headerConfig = new();
            IWorkbook wb = ReadWorkBook(file);
            wb.MissingCellPolicy = MissingCellPolicy.RETURN_NULL_AND_BLANK;

            string sheetName = FindSheet(wb, sheet);

            ISheet ws = wb.GetSheet(sheetName) ?? throw new Exception(sheet + " : Sheet Not Found.");
            IRow sheetRow = ws.GetRow(0);

            if (sheetRow != null)
            {
                for (int col = 0; col <= sheetRow.LastCellNum; col++)
                {
                    ICell cell = sheetRow.GetCell(col);
                    if (cell != null)
                    {
                        headerConfig[cell.StringCellValue.Trim()] = col;
                    }
                }
            }
            else
                throw new Exception("Invalid Template. Missing Headers.");

            int rowIndex = 1;

            foreach (string id in data.Keys)
            {

				sheetRow = ws.GetRow(rowIndex);

                sheetRow ??= ws.CreateRow(rowIndex);

                foreach (string key in data[id].Keys)
                {


					ICell cell = sheetRow.GetCell(headerConfig[key]);

                    if (cell != null)
                    {
                        cell.SetCellValue(data[id][key]);
                    }
                    else
                    {
                        cell = sheetRow.CreateCell(headerConfig[key]);
                        cell.SetCellValue(data[id][key]);
                    }

                }

                rowIndex++;

            }



            using (FileStream stream = new(file, FileMode.Create, FileAccess.Write))
            {
                wb.Write(stream);
            }

            wb.Close();


        }

        public static object ReadExcelData(string file, string sheet, string range = "", string key = "", OUTPUT_TYPE type = OUTPUT_TYPE.DICTIONARY, int maxBlankRow = 1)
        {
            IWorkbook wb = ReadWorkBook(file);
            wb.MissingCellPolicy = MissingCellPolicy.RETURN_NULL_AND_BLANK;

            string sheetName = FindSheet(wb, sheet);

            ISheet ws = wb.GetSheet(sheetName) ?? throw new Exception(sheet + " : Sheet Not Found.");

            if (type == OUTPUT_TYPE.DICTIONARY)
            {
                return ReadExcelDataToDictionary(wb, ws, range, key, maxBlankRow);

            }


            return null;
        }

        private static string FindSheet(IWorkbook wb, string sheetName)
        {
            string result = "";
            List<string> sheets;


            if (sheetName.StartsWith("*") && sheetName.EndsWith("*"))
            {
                sheetName = sheetName.Replace("*", "");
                sheets = GetSheetList(wb);

                foreach (string sheet in sheets)
                {
                    if (sheet.Contains(sheetName))
                    {
                        result = sheet;
                        break;
                    }
                }
            }
            else if (sheetName.StartsWith("*") && sheetName.EndsWith("*") == false)
            {

                Regex regex = new(Regex.Escape(sheetName).Replace("\\*", ".*?"), RegexOptions.IgnoreCase);
                sheets = GetSheetList(wb);

                foreach (string sheet in sheets)
                {
                    if (regex.IsMatch(sheet))
                    {
                        result = sheet;
                        break;
                    }
                }
            }
            else if (sheetName.StartsWith("*") == false && sheetName.EndsWith("*"))
            {
                Regex regex = new("^" + Regex.Escape(sheetName).Replace(@"\*", ".*") + "$", RegexOptions.IgnoreCase);
                sheets = GetSheetList(wb);

                foreach (string sheet in sheets)
                {
                    if (regex.IsMatch(sheet))
                    {
                        result = sheet;
                        break;
                    }
                }

            }
            else
                result = sheetName;


            return result;
        }

        private static Dictionary<string, Dictionary<string, string>> ReadExcelDataToDictionary(IWorkbook wb, ISheet ws, string range, string key = "", int maxBlankRow = 1)
        {
            Dictionary<string, Dictionary<string, string>> result = new();
            int blankRowCount = 0;
            int keyIndex = -1;
            int dictIndex = 0;

            key ??= "";

            if (!IsValidExcelRange(range.Replace("*", "")))
                throw new Exception(range + " : Invalid Excel Range.");

            if (range.Contains('*') == false)
            {
                /*CellRangeAddress cellRange = CellRangeAddress.ValueOf(range);
                int startRow = cellRange.FirstRow;
                int endRow = cellRange.LastRow;

                for (int row = startRow; row <= endRow; row++)
                {
                    IRow sheetRow = ws.GetRow(row);

                    if (sheetRow != null)
                    {
                        int startColumn = cellRange.FirstColumn;
                        int endColumn = cellRange.LastColumn;

                        for (int column = startColumn; column <= endColumn; column++)
                        {
                            ICell cell = sheetRow.GetCell(column);
                            string cellValue;

                            if (cell != null)
                            {
                                cellValue = cell.ToString();
                            }

                        }
                    }
                }*/
            }
            else if (range.Contains('*'))
            {
                range = range.Replace("*", "");
                CellRangeAddress cellRange = CellRangeAddress.ValueOf(range);
                int startRow = cellRange.FirstRow;
                string cellKeyValue = "";
                Dictionary<int, string> columnConfig = new();

                for (int row = startRow; row <= ws.LastRowNum; row++)
                {

                    IRow sheetRow = ws.GetRow(row);

                    if (sheetRow != null)
                    {
                        blankRowCount = 0;
                        int startColumn = cellRange.FirstColumn;
                        int endColumn = cellRange.LastColumn;

                        if (row > startRow)
                        {
                            if (keyIndex == -1)
                            {
                                dictIndex++;
                                result[dictIndex.ToString()] = new Dictionary<string, string>();
                            }
                            else
                            {
                                ICell cellKey = sheetRow.GetCell(keyIndex);

                                cellKeyValue = GetCellValueString(cellKey, wb);

                                if (cellKeyValue.Trim() != "")
                                    result[cellKeyValue.Trim()] = new Dictionary<string, string>();
                                else
                                    continue;
                            }
                        }


                        for (int column = startColumn; column <= endColumn; column++)
                        {
                            ICell cell = sheetRow.GetCell(column);
                            string cellValue;


                            cellValue = GetCellValueString(cell, wb);

                            if (row == startRow)
                            {
                                if (key != "")
                                {
                                    if (cellValue == key)
                                        keyIndex = column;
                                }

                                columnConfig[column] = cellValue.Trim();
                            }
                            else
                            {
                                if (keyIndex != -1)
                                    result[cellKeyValue.Trim()][columnConfig[column]] = cellValue.Trim();
                                else
                                    result[dictIndex.ToString()][columnConfig[column]] = cellValue.Trim();

                            }

                        }

                        if (row == startRow && key != "" && keyIndex == -1)
                            throw new Exception(key + " : Key Not Found.");
                    }
                    else
                    {
                        if (row == startRow)
                            throw new Exception(range + " - Header Row Not Found!");
                        else
                        {
                            blankRowCount++;

                            if (blankRowCount >= maxBlankRow)
                                break;
                        }
                    }
                }
            }




            return result;
        }

        private static string GetCellValueString(ICell cell, IWorkbook wb)
        {
            string cellValue = "";

            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.String: cellValue = cell.ToString(); break;
                    case CellType.Numeric: cellValue = cell.NumericCellValue.ToString(); break;
                    case CellType.Formula:
                        try
                        {
                            var evaluator = wb.GetCreationHelper().CreateFormulaEvaluator();
                            ICell evaluatedCell = evaluator.EvaluateInCell(cell);
                            if (evaluatedCell.CellType == CellType.String)
                                cellValue = evaluatedCell.StringCellValue;
                            else if (evaluatedCell.CellType == CellType.Numeric)
                                cellValue = evaluatedCell.NumericCellValue.ToString();
                        }
                        catch (Exception)
                        {
                            try { cellValue = cell.StringCellValue; } catch (Exception) { cellValue = cell.NumericCellValue.ToString(); }
                        }

                        break;
                }
            }

            return cellValue;
        }

        public static bool IsValidExcelRange(string range)
        {
            int maxRows = 1048576;
            int maxColumns = 16384;

            string pattern = @"^[A-Za-z]+\d+:[A-Za-z]+\d+$";

            if (!Regex.IsMatch(range, pattern))
                return false;

            string[] cells = range.Split(':');
            string startCell = cells[0];
            string endCell = cells[1];

            // Calculate the row and column indexes from the cell addresses
            int startRow = int.Parse(Regex.Match(startCell, @"\d+").Value);
            int endRow = int.Parse(Regex.Match(endCell, @"\d+").Value);
            int startColumn = ConvertColumnLetterToIndex(Regex.Match(startCell, @"[A-Za-z]+").Value);
            int endColumn = ConvertColumnLetterToIndex(Regex.Match(endCell, @"[A-Za-z]+").Value);

            // Check if the row and column indexes are within the allowed range
            if (startRow > maxRows || endRow > maxRows || startColumn > maxColumns || endColumn > maxColumns)
                return false;

            return true;
        }

        private static bool IsValidExcelCell(string cell)
        {
            int maxRows = 1048576;
            int maxColumns = 16384;

            string pattern = @"^[A-Za-z]+[0-9]+$";

            if (!Regex.IsMatch(cell, pattern))
                return false;

            int startRow = 0, startColumn = 0;

            GetCellIndex(cell, ref startRow, ref startColumn);

            if (startRow > maxRows || startColumn > maxColumns)
                return false;

            return true;
        }

        public static void GetCellIndex(string cell, ref int row, ref int col)
        {
            row = int.Parse(Regex.Match(cell, @"\d+").Value);
            col = ConvertColumnLetterToIndex(Regex.Match(cell, @"[A-Za-z]+").Value);
        }

        private static void GetNPOICellIndex(string cell, ref int row, ref int col)
        {
            row = int.Parse(Regex.Match(cell, @"\d+").Value) - 1;
            col = ConvertColumnLetterToIndex(Regex.Match(cell, @"[A-Za-z]+").Value) - 1;
        }

        public static int ConvertColumnLetterToIndex(string columnLetters)
        {
            int index = 0;
            int pow = 1;

            for (int i = columnLetters.Length - 1; i >= 0; i--)
            {
                char c = columnLetters[i];
                index += (c - 'A' + 1) * pow;
                pow *= 26;
            }

            return index;
        }

        public static string GetCellText(ICell cell)
        {

            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        return cell.NumericCellValue.ToString();
                    case CellType.String:
                        return cell.StringCellValue.ToString();
                    case CellType.Boolean:
                        return cell.BooleanCellValue.ToString();
                    case CellType.Formula:
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.Numeric: return cell.NumericCellValue.ToString();
                            case CellType.String: return cell.StringCellValue.ToString();
                            case CellType.Boolean: return cell.BooleanCellValue.ToString();
                        }; break;
                    default:
                        return "";
                }

            }
            else
                return "";


            return "";
        }

        public static double GetCellNumeric(ISheet ws, string range)
        {

            IsValidExcelCell(range);

            int row = 0, column = 0;

            GetNPOICellIndex(range, ref row, ref column);

            ICell cell = CheckCell(ws, row, column);


            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.Numeric:
                        return cell.NumericCellValue;
                    case CellType.String:
                        return double.Parse(cell.StringCellValue.ToString());
                    case CellType.Boolean:
                        return 0;
                    case CellType.Formula:
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.Numeric: return cell.NumericCellValue;
                            case CellType.String: return double.Parse(cell.StringCellValue.ToString());
                            case CellType.Boolean: return 0;
                        }; break;
                    default:
                        return 0;
                }

            }
            else
                return 0;


            return 0;
        }

        public static void SetCellText(ICell cell, string text)
        {
            cell?.SetCellValue(text);
        }

        public static void TerminateExcel()
        {
            Process cmd = new();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine("taskkill /F /IM excel.exe");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
        }

        public static string ConvertCSVToXLSX(string file)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Sheet1");

            // Read the CSV file and write data to Excel sheet
            using (StreamReader reader = new(file))
            {
                int rowIndex = 0;
                while (!reader.EndOfStream)
                {
                    string[] values = reader.ReadLine().Split(',');

                    IRow row = sheet.CreateRow(rowIndex++);
                    for (int i = 0; i < values.Length; i++)
                    {
                        row.CreateCell(i).SetCellValue(values[i]);
                    }
                }
            }

            string newFileName = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".xlsx";

            // Save the workbook to a file
            using (FileStream fs = new(newFileName, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            return newFileName;

        }

        public static string GetNextColumn(string currentColumn)
        {
            if (currentColumn == "XFD")
                return "XFD";


            char[] columnLetters = currentColumn.ToCharArray();
            int length = columnLetters.Length;

            for (int i = length - 1; i >= 0; i--)
            {
                char currentChar = columnLetters[i];

                if (currentChar < 'Z')
                {
                    columnLetters[i] = (char)(currentChar + 1);
                    break;
                }
                else
                {
                    columnLetters[i] = 'A';
                    if (i == 0)
                    {
                        Array.Resize(ref columnLetters, length + 1);
                        Array.Copy(columnLetters, 0, columnLetters, 1, length);
                        columnLetters[0] = 'A';
                    }
                }
            }

            return new string(columnLetters);
        }

        public static int GetMaxColumnIndex(ISheet sheet)
        {
            int maxColumnIndex = -1;

            foreach (IRow row in sheet)
            {
                if (row != null)
                {
                    foreach (ICell cell in row.Cells)
                    {
                        if (cell != null)
                        {
                            maxColumnIndex = Math.Max(maxColumnIndex, cell.ColumnIndex);
                        }
                    }
                }
            }

            return maxColumnIndex;
        }

        public static void SetCellValue(ISheet ws, int row, int column, string value)
        {
            ICell cell = CheckCell(ws, row, column);

            cell.SetCellValue(value);

        }

        public static void SetCellValue(ISheet ws, string range, string value, ICellStyle? style = null)
        {

            IsValidExcelCell(range);

            int row = 0, column = 0;

            GetNPOICellIndex(range, ref row, ref column);

            ICell cell = CheckCell(ws, row, column);

            cell.SetCellValue(value);

            if(style != null)
                cell.CellStyle = style;

        }

        public static void SetCellFormula(ISheet ws, string range, string formula, ICellStyle? style = null)
        {

            IsValidExcelCell(range);

            int row = 0, column = 0;

            GetNPOICellIndex(range, ref row, ref column);

            ICell cell = CheckCell(ws, row, column);

            cell.SetCellType(CellType.Formula);
            cell.SetCellFormula(formula);

            if (style != null)
                cell.CellStyle = style;
        }

        public static void SetCellValueNumeric(ISheet ws, string range, string value, ICellStyle? style = null)
        {

            IsValidExcelCell(range);

            int row = 0, column = 0;

            GetNPOICellIndex(range, ref row, ref column);

            ICell cell = CheckCell(ws, row, column);

            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(double.Parse(value));

            if (style != null)
                cell.CellStyle = style;
        }

        public static void SetCellValueNumeric(ISheet ws, string range, decimal value, ICellStyle? style = null)
        {

            IsValidExcelCell(range);

            int row = 0, column = 0;

            GetNPOICellIndex(range, ref row, ref column);

            ICell cell = CheckCell(ws, row, column);

            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(double.Parse(value.ToString()));

            if (style != null)
                cell.CellStyle = style;
        }

        private static ICell CheckCell(ISheet ws, int row, int column)
        {
            IRow irow = ws.GetRow(row);

            irow ??= ws.CreateRow(row);

            ICell cell = irow.GetCell(column);

            cell ??= irow.CreateCell(column);

            return cell;
        }

        public static void SetCellStyle(ISheet ws, string range, ICellStyle style)
        {
            IsValidExcelCell(range);

            int row = 0, column = 0;

            GetNPOICellIndex(range, ref row, ref column);

            ICell cell = CheckCell(ws, row, column);
            cell.CellStyle = style;

        }

    }
}
