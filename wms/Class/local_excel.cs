using System;
using System.Data;
using System.Threading;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelOptions
{
    //class local_excel
    //{
    //}

    public class ExcelOptions
    {
        private Stopwatch wath = new Stopwatch();

        /// <summary>
        /// 使用COM读取Excel
        /// </summary>
        /// <param name="excelFilePath">路径</param>
        /// <returns>DataTabel</returns>
        public System.Data.DataTable GetExcelData(string excelFilePath)
        {
            Excel.Application app = new Excel.Application();
            Excel.Sheets sheets;
            Excel.Workbook workbook = null;
            object oMissiong = System.Reflection.Missing.Value;
            System.Data.DataTable dt = new System.Data.DataTable();

            wath.Start();

            try
            {
                if (app == null)
                {
                    return null;
                }

                workbook = app.Workbooks.Open(excelFilePath, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);

                //将数据读入到DataTable中——Start   

                sheets = workbook.Worksheets;
                Excel.Worksheet worksheet = (Excel.Worksheet)sheets.get_Item(1);//读取第一张表
                if (worksheet == null)
                    return null;

                string cellContent;
                int iRowCount = worksheet.UsedRange.Rows.Count;
                int iColCount = worksheet.UsedRange.Columns.Count;
                Excel.Range range;

                //负责列头Start
                DataColumn dc;
                int ColumnID = 1;
                range = (Excel.Range)worksheet.Cells[1, 1];
                while (range.Text.ToString().Trim() != "")
                {
                    dc = new DataColumn();
                    dc.DataType = System.Type.GetType("System.String");
                    dc.ColumnName = range.Text.ToString().Trim();
                    dt.Columns.Add(dc);

                    range = (Excel.Range)worksheet.Cells[1, ++ColumnID];
                }


                for (int iRow = 1; iRow <= iRowCount; iRow++)
                {
                    Excel.Range range_is_get;
                    range_is_get=(Excel.Range)worksheet.Cells[iRow, 1];

                    if (range_is_get.Text.ToString() == "Order Status" || range_is_get.Text.ToString() == "Unshipped" )
                    {
                        DataRow dr = dt.NewRow();
                        for (int iCol = 1; iCol <= iColCount; iCol++)
                        {
                            range = (Excel.Range)worksheet.Cells[iRow, iCol];

                            cellContent = (range.Value2 == null) ? "" : range.Text.ToString();

                            dr[iCol - 1] = cellContent;
                        }
                        dt.Rows.Add(dr);
                    }
                }

                wath.Stop();
                TimeSpan ts = wath.Elapsed;

                //将数据读入到DataTable中——End
                return dt;
            }
            catch
            {

                return null;
            }
            finally
            {
                workbook.Close(false, oMissiong, oMissiong);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                workbook = null;
                app.Workbooks.Close();
                app.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                app = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /*
        /// <summary>
        /// 使用COM，多线程读取Excel（1 主线程、4 副线程）
        /// </summary>
        /// <param name="excelFilePath">路径</param>
        /// <returns>DataTabel</returns>
        public System.Data.DataTable ThreadReadExcel(string excelFilePath)
        {
            Excel.Application app = new Excel.Application();
            Excel.Sheets sheets = null;
            Excel.Workbook workbook = null;
            object oMissiong = System.Reflection.Missing.Value;
            System.Data.DataTable dt = new System.Data.DataTable();

            wath.Start();

            try
            {
                if (app == null)
                {
                    return null;
                }

                workbook = app.Workbooks.Open(excelFilePath, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);

                //将数据读入到DataTable中——Start   
                sheets = workbook.Worksheets;
                Excel.Worksheet worksheet = (Excel.Worksheet)sheets.get_Item(1);//读取第一张表
                if (worksheet == null)
                    return null;

                string cellContent;
                int iRowCount = worksheet.UsedRange.Rows.Count;
                int iColCount = worksheet.UsedRange.Columns.Count;
                Excel.Range range;

                //负责列头Start
                DataColumn dc;
                int ColumnID = 1;
                range = (Excel.Range)worksheet.Cells[1, 1];
                //while (range.Text.ToString().Trim() != "")
                while (iColCount >= ColumnID)
                {
                    dc = new DataColumn();
                    dc.DataType = System.Type.GetType("System.String");

                    string strNewColumnName = range.Text.ToString().Trim();
                    if (strNewColumnName.Length == 0) strNewColumnName = "_1";
                    //判断列名是否重复
                    for (int i = 1; i < ColumnID; i++)
                    {
                        if (dt.Columns[i - 1].ColumnName == strNewColumnName)
                            strNewColumnName = strNewColumnName + "_1";
                    }

                    dc.ColumnName = strNewColumnName;
                    dt.Columns.Add(dc);

                    range = (Excel.Range)worksheet.Cells[1, ++ColumnID];
                }
                //End

                //数据大于500条，使用多进程进行读取数据
                if (iRowCount - 1 > 500)
                {
                    //开始多线程读取数据
                    //新建线程
                    int b2 = (iRowCount - 1) / 10;
                    DataTable dt1 = new DataTable("dt1");
                    dt1 = dt.Clone();
                    SheetOptions sheet1thread = new SheetOptions(worksheet, iColCount, 2, b2 + 1, dt1);
                    Thread othread1 = new Thread(new ThreadStart(sheet1thread.SheetToDataTable));
                    othread1.Start();

                    //阻塞 1 毫秒，保证第一个读取 dt1
                    Thread.Sleep(1);

                    DataTable dt2 = new DataTable("dt2");
                    dt2 = dt.Clone();
                    SheetOptions sheet2thread = new SheetOptions(worksheet, iColCount, b2 + 2, b2 * 2 + 1, dt2);
                    Thread othread2 = new Thread(new ThreadStart(sheet2thread.SheetToDataTable));
                    othread2.Start();

                    DataTable dt3 = new DataTable("dt3");
                    dt3 = dt.Clone();
                    SheetOptions sheet3thread = new SheetOptions(worksheet, iColCount, b2 * 2 + 2, b2 * 3 + 1, dt3);
                    Thread othread3 = new Thread(new ThreadStart(sheet3thread.SheetToDataTable));
                    othread3.Start();

                    DataTable dt4 = new DataTable("dt4");
                    dt4 = dt.Clone();
                    SheetOptions sheet4thread = new SheetOptions(worksheet, iColCount, b2 * 3 + 2, b2 * 4 + 1, dt4);
                    Thread othread4 = new Thread(new ThreadStart(sheet4thread.SheetToDataTable));
                    othread4.Start();

                    //主线程读取剩余数据
                    for (int iRow = b2 * 4 + 2; iRow <= iRowCount; iRow++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int iCol = 1; iCol <= iColCount; iCol++)
                        {
                            range = (Excel.Range)worksheet.Cells[iRow, iCol];
                            cellContent = (range.Value2 == null) ? "" : range.Text.ToString();
                            dr[iCol - 1] = cellContent;
                        }
                        dt.Rows.Add(dr);
                    }

                    othread1.Join();
                    othread2.Join();
                    othread3.Join();
                    othread4.Join();

                    //将多个线程读取出来的数据追加至 dt1 后面
                    foreach (DataRow dr in dt.Rows)
                        dt1.Rows.Add(dr.ItemArray);
                    dt.Clear();
                    dt.Dispose();

                    foreach (DataRow dr in dt2.Rows)
                        dt1.Rows.Add(dr.ItemArray);
                    dt2.Clear();
                    dt2.Dispose();

                    foreach (DataRow dr in dt3.Rows)
                        dt1.Rows.Add(dr.ItemArray);
                    dt3.Clear();
                    dt3.Dispose();

                    foreach (DataRow dr in dt4.Rows)
                        dt1.Rows.Add(dr.ItemArray);
                    dt4.Clear();
                    dt4.Dispose();

                    return dt1;
                }
                else
                {
                    for (int iRow = 2; iRow <= iRowCount; iRow++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int iCol = 1; iCol <= iColCount; iCol++)
                        {
                            range = (Excel.Range)worksheet.Cells[iRow, iCol];
                            cellContent = (range.Value2 == null) ? "" : range.Text.ToString();
                            dr[iCol - 1] = cellContent;
                        }
                        dt.Rows.Add(dr);
                    }
                }

                wath.Stop();
                TimeSpan ts = wath.Elapsed;
                //将数据读入到DataTable中——End
                return dt;
            }
            catch
            {

                return null;
            }
            finally
            {
                workbook.Close(false, oMissiong, oMissiong);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheets);
                workbook = null;
                app.Workbooks.Close();
                app.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                app = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                /*
                object objmissing = System.Reflection.Missing.Value;
 
Excel.ApplicationClass application = new ApplicationClass();
Excel.Workbook book = application.Workbooks.Add(objmissing);
Excel.Worksheet sheet = (Excel.Worksheet)book.Worksheets.Add（objmissing,objmissing,objmissing,objmissing);
 
//操作过程 ^&%&×&……&%&&……
 
//释放
sheet.SaveAs(path,objmissing,objmissing,objmissing,objmissing,objmissing,objmissing,objmissing,objmissing);
System.Runtime.InteropServices.Marshal.ReleaseComObject((object)sheet);
System.Runtime.InteropServices.Marshal.ReleaseComObject((object)book);
application.Quit();
System.Runtime.InteropServices.Marshal.ReleaseComObject((object)application);
System.GC.Collect();
                 //
            }
        }
        */

        /// <summary>
        /// 删除Excel行
        /// </summary>
        /// <param name="excelFilePath">Excel路径</param>
        /// <param name="rowStart">开始行</param>
        /// <param name="rowEnd">结束行</param>
        /// <param name="designationRow">指定行</param>
        /// <returns></returns>
        public string DeleteRows(string excelFilePath, int rowStart, int rowEnd, int designationRow)
        {
            string result = "";
            Excel.Application app = new Excel.Application();
            Excel.Sheets sheets;
            Excel.Workbook workbook = null;
            object oMissiong = System.Reflection.Missing.Value;
            try
            {
                if (app == null)
                {
                    return "分段读取Excel失败";
                }

                workbook = app.Workbooks.Open(excelFilePath, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);
                sheets = workbook.Worksheets;
                Excel.Worksheet worksheet = (Excel.Worksheet)sheets.get_Item(1);//读取第一张表
                if (worksheet == null)
                    return result;
                Excel.Range range;

                //先删除指定行，一般为列描述
                if (designationRow != -1)
                {
                    range = (Excel.Range)worksheet.Rows[designationRow, oMissiong];
                    range.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                }
                Stopwatch sw = new Stopwatch();
                sw.Start();

                int i = rowStart;
                for (int iRow = rowStart; iRow <= rowEnd; iRow++, i++)
                {
                    range = (Excel.Range)worksheet.Rows[rowStart, oMissiong];
                    range.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                }

                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                workbook.Save();

                //将数据读入到DataTable中——End
                return result;
            }
            catch
            {

                return "分段读取Excel失败";
            }
            finally
            {
                workbook.Close(false, oMissiong, oMissiong);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                workbook = null;
                app.Workbooks.Close();
                app.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                app = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public void ToExcelSheet(DataSet ds, string fileName)
        {
            Excel.Application appExcel = new Excel.Application();
            Excel.Workbook workbookData = null;
            Excel.Worksheet worksheetData;
            Excel.Range range;
            try
            {
                workbookData = appExcel.Workbooks.Add(System.Reflection.Missing.Value);
                appExcel.DisplayAlerts = false;//不显示警告
                //xlApp.Visible = true;//excel是否可见
                //
                //for (int i = workbookData.Worksheets.Count; i > 0; i--)
                //{
                //    Microsoft.Office.Interop.Excel.Worksheet oWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbookData.Worksheets.get_Item(i);
                //    oWorksheet.Select();
                //    oWorksheet.Delete();
                //}

                for (int k = 0; k < ds.Tables.Count; k++)
                {
                    worksheetData = (Excel.Worksheet)workbookData.Worksheets.Add(System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                    // testnum--;
                    if (ds.Tables[k] != null)
                    {
                        worksheetData.Name = ds.Tables[k].TableName;
                        //写入标题
                        for (int i = 0; i < ds.Tables[k].Columns.Count; i++)
                        {
                            worksheetData.Cells[1, i + 1] = ds.Tables[k].Columns[i].ColumnName;
                            range = (Excel.Range)worksheetData.Cells[1, i + 1];
                            //range.Interior.ColorIndex = 15;
                            range.Font.Bold = true;
                            range.NumberFormatLocal = "@";//文本格式 
                            range.EntireColumn.AutoFit();//自动调整列宽 
                            // range.WrapText = true; //文本自动换行   
                            range.ColumnWidth = 15;
                        }
                        //写入数值
                        for (int r = 0; r < ds.Tables[k].Rows.Count; r++)
                        {
                            for (int i = 0; i < ds.Tables[k].Columns.Count; i++)
                            {
                                worksheetData.Cells[r + 2, i + 1] = ds.Tables[k].Rows[r][i];
                                //Range myrange = worksheetData.get_Range(worksheetData.Cells[r + 2, i + 1], worksheetData.Cells[r + 3, i + 2]);
                                //myrange.NumberFormatLocal = "@";//文本格式 
                                //// myrange.EntireColumn.AutoFit();//自动调整列宽 
                                ////   myrange.WrapText = true; //文本自动换行   
                                //myrange.ColumnWidth = 15;
                            }
                            //  rowRead++;
                            //System.Windows.Forms.Application.DoEvents();
                        }
                    }
                    worksheetData.Columns.EntireColumn.AutoFit();
                    workbookData.Saved = true;
                }
            }
            catch (Exception ex) { }
            finally
            {
                workbookData.SaveCopyAs(fileName);
                workbookData.Close(false, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                appExcel.Quit();
                GC.Collect();
            }
        }

    }
}
