using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Reflection;

namespace DownLoad
{
    /// <summary>
    /// C#操作Excel类
    /// </summary>
    class ExcelOperate
    {
        //法一
        //public bool DataSetToExcel(DataSet dataSet, bool isShowExcle)
        //{
        //    DataTable dataTable = dataSet.Tables[0];
        //    int rowNumber = dataTable.Rows.Count;
        //    int columnNumber = dataTable.Columns.Count;

        //    if (rowNumber == 0)
        //    {
        //        MessageBox.Show("没有任何数据可以导入到Excel文件！");
        //        return false;
        //    }

        //    //建立Excel对象
        //    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
        //    excel.Application.Workbooks.Add(true);
        //    excel.Visible = isShowExcle;//是否打开该Excel文件

        //    //填充数据
        //    for (int c = 0; c < rowNumber; c++)
        //    {
        //        for (int j = 0; j < columnNumber; j++)
        //        {
        //            excel.Cells[c + 1, j + 1] = dataTable.Rows[c].ItemArray[j];
        //        }
        //    }

        //    return true;
        //}


        //法二
        //public bool DataSetToExcel(DataSet dataSet, bool isShowExcle)
        //{
        //    DataTable dataTable = dataSet.Tables[0];
        //    int rowNumber = dataTable.Rows.Count;

        //    int rowIndex = 1;
        //    int colIndex = 0;


        //    if (rowNumber == 0)
        //    {
        //        return false;
        //    }

        //    //建立Excel对象
        //    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
        //    excel.Application.Workbooks.Add(true);
        //    excel.Visible = isShowExcle;

        //    //生成字段名称
        //    foreach (DataColumn col in dataTable.Columns)
        //    {
        //        colIndex++;
        //        excel.Cells[1, colIndex] = col.ColumnName;
        //    }

        //    //填充数据
        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        rowIndex++;
        //        colIndex = 0;
        //        foreach (DataColumn col in dataTable.Columns)
        //        {
        //            colIndex++;
        //            excel.Cells[rowIndex, colIndex] = row[col.ColumnName];
        //        }
        //    }

        //    return true;
        //}

        //法三（速度最快）
        /// <summary>
        /// 将数据集中的数据导出到EXCEL文件
        /// </summary>
        /// <param name="dataSet">输入数据集</param>
        /// <param name="isShowExcle">是否显示该EXCEL文件</param>
        /// <returns></returns>
        public bool DataSetToExcel(DataSet dataSet, bool isShowExcle)
        {
            DataTable dataTable = dataSet.Tables[0];
            int rowNumber = dataTable.Rows.Count;//不包括字段名
            int columnNumber = dataTable.Columns.Count;
            int colIndex = 0;

            if (rowNumber == 0)
            {
                return false;
            }

            //建立Excel对象 
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            //excel.Application.Workbooks.Add(true);
            Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            excel.Visible = isShowExcle;
            //Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)excel.Worksheets[1];
            Microsoft.Office.Interop.Excel.Range range;

            //生成字段名称 
            foreach (DataColumn col in dataTable.Columns)
            {
                colIndex++;
                excel.Cells[1, colIndex] = col.ColumnName;
            }

            object[,] objData = new object[rowNumber, columnNumber];

            for (int r = 0; r < rowNumber; r++)
            {
                for (int c = 0; c < columnNumber; c++)
                {
                    objData[r, c] = dataTable.Rows[r][c];
                }
                //Application.DoEvents();
            }

            // 写入Excel 
            range = worksheet.get_Range(excel.Cells[2, 1], excel.Cells[rowNumber + 1, columnNumber]);
            //range.NumberFormat = "@";//设置单元格为文本格式
            range.Value2 = objData;
            worksheet.get_Range(excel.Cells[2, 1], excel.Cells[rowNumber + 1, 1]).NumberFormat = "yyyy-m-d h:mm";

            return true;
        }

        //法四
        //public bool DataSetToExcel(DataSet dataSet, bool isShowExcle)
        //{
        //    DataTable dataTable = dataSet.Tables[0];
        //    int rowNumber = dataTable.Rows.Count;
        //    int columnNumber = dataTable.Columns.Count;
        //    String stringBuffer = "";

        //    if (rowNumber == 0)
        //    {
        //        MessageBox.Show("没有任何数据可以导入到Excel文件！");
        //        return false;
        //    }

        //    //建立Excel对象
        //    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
        //    excel.Application.Workbooks.Add(true);
        //    excel.Visible = isShowExcle;//是否打开该Excel文件

        //    //填充数据
        //    for (int i = 0; i < rowNumber; i++)
        //    {
        //        for (int j = 0; j < columnNumber; j++)
        //        {
        //            stringBuffer += dataTable.Rows[i].ItemArray[j].ToString();
        //            if (j < columnNumber - 1)
        //            {
        //                stringBuffer += "\t";
        //            }
        //        }
        //        stringBuffer += "\n";
        //    }
        //    Clipboard.Clear();
        //    Clipboard.SetDataObject(stringBuffer);
        //    ((Microsoft.Office.Interop.Excel.Range)excel.Cells[1, 1]).Select();
        //    ((Microsoft.Office.Interop.Excel.Worksheet)excel.ActiveWorkbook.ActiveSheet).Paste(Missing.Value, Missing.Value);
        //    Clipboard.Clear();

        //    return true;
        //}

        //public bool DataSetToExcel(DataSet dataSet, string fileName, bool isShowExcle)
        //{
        //    DataTable dataTable = dataSet.Tables[0];
        //    int rowNumber = dataTable.Rows.Count;
        //    int columnNumber = dataTable.Columns.Count;

        //    if (rowNumber == 0)
        //    {
        //        MessageBox.Show("没有任何数据可以导入到Excel文件！");
        //        return false;
        //    }

        //    //建立Excel对象
        //    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
        //    Microsoft.Office.Interop.Excel.Workbook workBook = excel.Application.Workbooks.Add(true);
        //    excel.Visible = false;//是否打开该Excel文件

        //    //填充数据
        //    for (int i = 0; i < rowNumber; i++)
        //    {
        //        for (int j = 0; j < columnNumber; j++)
        //        {
        //            excel.Cells[i + 1, j + 1] = dataTable.Rows[i].ItemArray[j];
        //        }
        //    }

        //    //string fileName = path + "\\" + DateTime.Now.ToString().Replace(':', '_') + ".xls";
        //    workBook.SaveAs(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

        //    try
        //    {
        //        workBook.Saved = true;
        //        excel.UserControl = false;
        //        //excelapp.Quit();
        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show(exception.Message);
        //    }
        //    finally
        //    {
        //        workBook.Close(Microsoft.Office.Interop.Excel.XlSaveAction.xlSaveChanges, Missing.Value, Missing.Value);
        //        excel.Quit();
        //    }

        //    if (isShowExcle)
        //    {
        //        System.Diagnostics.Process.Start(fileName);
        //    }
        //    return true;
        //}

        //public bool DataSetToExcel(DataSet dataSet, string fileName, bool isShowExcle)
        //{
        //    DataTable dataTable = dataSet.Tables[0];
        //    int rowNumber = dataTable.Rows.Count;//不包括字段名
        //    int columnNumber = dataTable.Columns.Count;
        //    int colIndex = 0;

        //    if (rowNumber == 0)
        //    {
        //        MessageBox.Show("没有任何数据可以导入到Excel文件！");
        //        return false;
        //    }

        //    //建立Excel对象
        //    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
        //    //excel.Application.Workbooks.Add(true);
        //    Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
        //    Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
        //    excel.Visible = isShowExcle;
        //    //Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)excel.Worksheets[1];
        //    worksheet.Name = "挠度数据";
        //    Microsoft.Office.Interop.Excel.Range range;

        //    //生成字段名称
        //    foreach (DataColumn col in dataTable.Columns)
        //    {
        //        colIndex++;
        //        excel.Cells[1, colIndex] = col.ColumnName;
        //    }

        //    object[,] objData = new object[rowNumber, columnNumber];

        //    for (int r = 0; r < rowNumber; r++)
        //    {
        //        for (int c = 0; c < columnNumber; c++)
        //        {
        //            objData[r, c] = dataTable.Rows[r][c];
        //        }
        //        //Application.DoEvents();
        //    }

        //    // 写入Excel
        //    range = worksheet.get_Range(excel.Cells[2, 1], excel.Cells[rowNumber + 1, columnNumber]);
        //    //range.NumberFormat = "@";//设置单元格为文本格式
        //    range.Value2 = objData;
        //    worksheet.get_Range(excel.Cells[2, 1], excel.Cells[rowNumber + 1, 1]).NumberFormat = "yyyy-m-d h:mm";

        //    //string fileName = path + "\\" + DateTime.Now.ToString().Replace(':', '_') + ".xls";
        //    workbook.SaveAs(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

        //    try
        //    {
        //        workbook.Saved = true;
        //        excel.UserControl = false;
        //        //excelapp.Quit();
        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show(exception.Message);
        //    }
        //    finally
        //    {
        //        workbook.Close(Microsoft.Office.Interop.Excel.XlSaveAction.xlSaveChanges, Missing.Value, Missing.Value);
        //        excel.Quit();
        //    }

        //    //if (isShowExcle)
        //    //{
        //    //    System.Diagnostics.Process.Start(fileName);
        //    //}
        //    return true;
        //}

        /// <summary>
        /// 将数据集中的数据保存到EXCEL文件
        /// </summary>
        /// <param name="dataSet">输入数据集</param>
        /// <param name="fileName">保存EXCEL文件的绝对路径名</param>
        /// <param name="isShowExcle">是否打开EXCEL文件</param>
        /// <returns></returns>
        public bool DataSetToExcel(DataGridView dv, string fileName, bool isShowExcle)
        {
            //DataTable dataTable = dataSet.Tables[0];
            int rowNumber = dv.Rows.Count;//不包括字段名
            int columnNumber = dv.Columns.Count;
            int colIndex = 0;

            if (rowNumber == 0)
            {
                MessageBox.Show("没有任何数据可以导入到Excel文件！");
                return false;
            }

            //建立Excel对象 
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            //excel.Application.Workbooks.Add(true);
            Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            excel.Visible = false;
            //Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)excel.Worksheets[1];
            Microsoft.Office.Interop.Excel.Range range;

            //生成字段名称 
            foreach (DataGridViewColumn col in dv.Columns)
            {
                colIndex++;
                excel.Cells[1, colIndex] = col.Name;
            }

            object[,] objData = new object[rowNumber, columnNumber];

            for (int r = 0; r < rowNumber; r++)
            {
                for (int c = 0; c < columnNumber; c++)
                {
                    objData[r, c] = dv.Rows[r].Cells[c].Value == null ? "" : dv.Rows[r].Cells[c].Value.ToString();
                }
                //Application.DoEvents();
            }

            // 写入Excel 
            range = worksheet.get_Range(excel.Cells[2, 1], excel.Cells[rowNumber + 1, columnNumber]);
            //range.NumberFormat = "@";//设置单元格为文本格式
            range.Value2 = objData;
            worksheet.get_Range(excel.Cells[2, 1], excel.Cells[rowNumber + 1, 1]).NumberFormat = "yyyy-m-d h:mm";

            //string fileName = path + "\\" + DateTime.Now.ToString().Replace(':', '_') + ".xls"; 
            workbook.SaveAs(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            try
            {
                workbook.Saved = true;
                excel.UserControl = false;
                //excelapp.Quit();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                workbook.Close(Microsoft.Office.Interop.Excel.XlSaveAction.xlSaveChanges, Missing.Value, Missing.Value);
                excel.Quit();
            }

            if (isShowExcle)
            {
                System.Diagnostics.Process.Start(fileName);
            }
            return true;
        }
    }
}
