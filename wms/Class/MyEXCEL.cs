using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Excel = Microsoft.Office.Interop.Excel;
using VB = Microsoft.VisualBasic;
using Office = Microsoft.Office.Core;
using MyDs = System.Data.DataSet;
using MyDt = System.Data.DataTable;
using MyDr = System.Data.DataRow;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;

using TMS.Framework.Business.DataAccess;
using TMS.Framework.Publics.Data;

using System.Data;
using System.Configuration;
using System.IO;
using System.Net;

namespace UniversalAnalyse
{
    public class MyEXCEL
    {
        ///// <summary>
        ///// 
        ///// </summary>
        //public static void 取页面清单数据(string URL)
        //{

        //}


        DBUtil DB = new DBUtil();

        public static void 取消和并并赋值(string URL,int Rows,int Cells)//,int Norm)
        {

            Excel.Application app = new Excel.ApplicationClass();

            //打开模板文件，得到WorkBook对象
            Excel.Workbook workBook = app.Workbooks.Open(URL, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            app.Visible = true;

            //Excel._Worksheet WSt = (Excel._Worksheet)workBook.Sheets[1];
            //WSt.Select(Type.Missing);

            //Excel.Range oRngt = (Excel.Range)WSt.get_Range(WSt.Cells[1, 1], WSt.Cells[Rows, 1]); ;

            for (int i =0;i<workBook.Sheets.Count;i++)
            {
                Excel._Worksheet WS = (Excel._Worksheet)workBook.Sheets[i+1];
                WS.Select(Type.Missing);

                //Excel.Range oRngNorm = (Excel.Range)WS.get_Range(WS.Cells[1, Norm], WS.Cells[Rows, Norm]); ;
                //object[,] Normobject = (object[,])oRngt.Value2;

                for (int cellindex = 0; cellindex < Cells; cellindex++)
                {
                    //Excel.Range oRngt = (Excel.Range)WS.get_Range(WS.Cells[1, cellindex+1], WS.Cells[Rows, cellindex+1]); ;
                    ////string[] excelValue = Convert.toa oRngt.Value2;
                    //object[,] eobject = (object[,])oRngt.Value2;

                    for (int rowindex = 0; rowindex < Rows; rowindex++)
                    {
                        try
                        {
                            Excel.Range oRng = (Excel.Range)WS.Cells[rowindex + 1, cellindex + 1];
                            oRng.Select();
                            bool isMerge = (bool)oRng.MergeCells;
                            
                            if (isMerge)
                            {
                                if (oRng.Value2 != null)
                                {
                                    String Rvalue = oRng.Value2.ToString();
                                    Excel.Range TepRang;

                                    int oRngRowsCount = oRng.MergeArea.Rows.Count;
                                    int oRngCellsCount = oRng.MergeArea.Columns.Count;
                                    oRng.UnMerge();
                                    for (int Rindex = 0; Rindex < oRngRowsCount - 1; Rindex++)
                                    {
                                        for (int Cindex = 0; Cindex < oRngCellsCount; Cindex++)
                                        {
                                            TepRang = (Excel.Range)WS.Cells[rowindex + 1 + Rindex + 1, cellindex + 1 + Cindex];
                                            TepRang.Value2 = Rvalue;
                                        }
                                    }
                                }

                                #region "20081114"

                                ////oRng.MergeArea.Cells.Count 
                                //String Rvalue = oRng.Value2.ToString();
                                //Excel.Range TepRang = (Excel.Range)WS.Cells[rowindex + 1, cellindex + 1];
                                //int MergeCount = 1;
                                //while ((bool)TepRang.MergeCells)
                                //{

                                //    //TepRang = (Excel.Range)WS.get_Range(WS.Cells[rowindex + 1, cellindex + 1], WS.Cells[rowindex + 1 + MergeCount, cellindex + 1]);
                                //    TepRang = (Excel.Range)WS.Cells[rowindex + 1 + MergeCount, cellindex + 1];
                                //    if (TepRang.Value2 == null || TepRang.Value2.ToString() == "")
                                //    {
                                //        MergeCount++;
                                //    }
                                //    else
                                //    { break; }
                                //    //oRng = (Excel.Range)WS.Cells[rowindex + 1+MergeCount, cellindex + 1];
                                //}

                                //int oRngRowsCount = MergeCount - 1;//oRng.CurrentRegion.Rows.Count;
                                ////int oRngCellsCount = oRng.CurrentRegion.Columns.Count;
                                //oRng.UnMerge();
                                ////MessageBox.Show(isMerge.ToString());  
                                //for (int Rindex = 0; Rindex < oRngRowsCount; Rindex++)
                                //{
                                //    //for (int Cindex = 0; Cindex < oRngCellsCount; Cindex++)
                                //    //{
                                //    TepRang = (Excel.Range)WS.Cells[rowindex + 1 + Rindex + 1, cellindex + 1];
                                //    TepRang.Value2 = Rvalue;
                                //    //}
                                //}
                                #endregion
                            }
                        }
                        catch
                        {
                            MessageBox.Show(rowindex.ToString() + "rows" + cellindex.ToString() + "cells");
                        }
                    }
                }
            }
        }

        public static void 基础数据设计文档NEW(MyDt ExcelDt, string URL, MyDt dt)
        {
            Excel.Application app = new Excel.ApplicationClass();

            //打开模板文件，得到WorkBook对象
            Excel.Workbook workBook = app.Workbooks.Open(URL, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            //Excel.Worksheet ws = (Excel.Worksheet)workBook.Sheets[3];
            app.Visible = true;

            #region "详细信息"
            Excel.Worksheet ws模板1 = (Excel.Worksheet)workBook.Sheets[4];
            Excel.Worksheet ws模板2 = (Excel.Worksheet)workBook.Sheets[5];
            for (int i = 0; i < ExcelDt.Rows.Count; i++)
            {
                string[] strlist = new string[9] { "OrderNo", "ContexID", "Remark", "FCODE", "CREATE_BY", "CREATE_DATE", "LAST_UPDATE_BY", "LAST_UPDATE_DATE", "Status" };
                ws模板1.Copy(Type.Missing, workBook.Sheets[6 + i * 2]);
                ws模板2.Copy(Type.Missing, workBook.Sheets[6 + i * 2 + 1]);
                Excel.Worksheet wsTep1 = (Excel.Worksheet)workBook.Sheets[6 + i * 2 + 1];
                Excel.Worksheet wsTep2 = (Excel.Worksheet)workBook.Sheets[6 + i * 2 + 2];
                wsTep1.Name = (1 + i).ToString() + ".1";
                wsTep2.Name = (1 + i).ToString() + ".2";

                #region"基础数据说明书-结构说明"
                wsTep1.Select(Type.Missing);

                wsTep1.get_Range("B2", Type.Missing).Value2 = ExcelDt.Rows[i][3].ToString();
                wsTep1.get_Range("B3", Type.Missing).Value2 = ExcelDt.Rows[i][5].ToString();
                if (ExcelDt.Rows[i][7].ToString().Length == 0)
                {
                    wsTep1.get_Range("F3", Type.Missing).Value2 = "固定值";
                }
                else
                {
                    wsTep1.get_Range("F3", Type.Missing).Value2 = "基础数据";


                    //MyDr[] drl = dt.Select("TableName='" + alist[i].ToString().Split(' ')[2].ToString().Trim()+"'");
                    MyDt tepDt = 取得表的字段信息(ExcelDt.Rows[i][7].ToString().Trim(), dt);

                    for (int index = 0; index < tepDt.Rows.Count; index++)
                    {
                        bool tepflg = false;
                        foreach (string tepstr in strlist)
                        {
                            if (tepDt.Rows[index]["ColumnName"].ToString().Trim().ToUpper() == tepstr.Trim().ToUpper())
                            {
                                tepflg = true;
                                break;
                            }
                        }
                        if (!tepflg)
                        {

                            指定位置插入行(wsTep1.get_Range("A" + (6 + index + 1).ToString(), Type.Missing));
                            //if (index > 0)
                            //{
                            //    Excel.Range range = wsTep1.get_Range("A6", "E6");
                            //    Excel.Range range1 = wsTep1.get_Range("A" + (6 + index + 1).ToString(), System.Type.Missing);
                            //    复制单元格(range, range1);
                            //}

                            wsTep1.get_Range("A" + (6 + index).ToString(), Type.Missing).Value2 = tepDt.Rows[index]["ColumnDesc"].ToString();
                            wsTep1.get_Range("B" + (6 + index).ToString(), Type.Missing).Value2 = 取得字符类型(tepDt.Rows[index]["Type"].ToString(), tepDt.Rows[index]["Length"].ToString());
                            wsTep1.get_Range("C" + (6 + index).ToString(), Type.Missing).Value2 = tepDt.Rows[index]["Length"].ToString();
                            wsTep1.get_Range("D" + (6 + index).ToString(), Type.Missing).Value2 = "";
                            wsTep1.get_Range("E" + (6 + index).ToString(), Type.Missing).Value2 = tepDt.Rows[index]["ColumnDesc"].ToString();
                        }
                    }
                #endregion
                #region"基础数据说明书-数据实例"
                    wsTep2.Select(Type.Missing);
                    for (int index = 0; index < tepDt.Rows.Count; index++)
                    {
                        bool tepflg = false;
                        foreach (string tepstr in strlist)
                        {
                            if (tepDt.Rows[index]["ColumnName"].ToString().Trim().ToUpper() == tepstr.Trim().ToUpper())
                            {
                                tepflg = true;
                                break;
                            }
                        }
                        if (!tepflg)
                        {
                            if (index > 0)
                            {
                                Excel.Range range = wsTep2.get_Range("A2", "A14");
                                Excel.Range range1 = wsTep2.get_Range(wsTep2.Cells[2, index + 1], wsTep2.Cells[14, index + 1]);
                                复制单元格(range, range1);
                            }
                            wsTep2.Cells[2, index + 1] = tepDt.Rows[index]["ColumnDesc"].ToString();
                        }
                    }
                    wsTep2.get_Range(wsTep2.Cells[1, 1], wsTep2.Cells[1, tepDt.Rows.Count - 9]).Borders.LineStyle = 1;
                    wsTep2.get_Range(wsTep2.Cells[1, 1], wsTep2.Cells[1, tepDt.Rows.Count - 9]).Merge(Type.Missing);

                    wsTep2.Cells[1, 1] = "基础数据说明书-数据实例";
                #endregion
                }

            }
            #endregion

            #region "添加链接"
            Excel.Worksheet ws = (Excel.Worksheet)workBook.Sheets[3];
            ws.Select(Type.Missing);
            int linkCount = 0;
            Excel.Range LinkRange = ws.get_Range("D4", Type.Missing);
            Excel.Range linkRange1 = ws.get_Range("B4", Type.Missing);

            while (LinkRange != null)
            {
                if (LinkRange.Value2.ToString().Trim().Length > 0)
                {
                    //Excel.Range range = ws页面清单.get_Range("C" + (4 + i).ToString(), Type.Missing);
                    Excel.Hyperlink link = (Excel.Hyperlink)LinkRange.Hyperlinks.Add(LinkRange, "", linkRange1.Value2.ToString().Trim() + "!A1", Type.Missing, Type.Missing);
                    linkCount++;
                    LinkRange = ws.get_Range("D" + (4 + linkCount).ToString(), Type.Missing);
                    linkRange1 = ws.get_Range("B" + (4 + linkCount).ToString(), Type.Missing);
                }
                else
                    break;
            }
            #endregion
        }


        /// <summary>
        /// 读取EXCEL模板，并插入数据
        /// </summary>
        /// <param name="alist"></param>
        public static  void 基础数据设计文档(ArrayList alist ,string URL ,MyDt dt)
        {
            Excel.Application app = new Excel.ApplicationClass();

            //打开模板文件，得到WorkBook对象
            Excel.Workbook workBook = app.Workbooks.Open(URL, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing , Type.Missing , Type.Missing , Type.Missing , Type.Missing , Type.Missing , Type.Missing , Type.Missing , Type.Missing );

            Excel.Worksheet ws = (Excel.Worksheet)workBook.Sheets[3];
            app.Visible = true;
            #region "基础数据目录"
            ws.Select(Type.Missing );

            for (int i = 0; i < alist.Count; i++)
            {
                //结构序号	实例序号	内容编号	对应功能点	内容名称	复用引用	内容描述
                string 结构序号 = (i + 1).ToString() + ".1";
                string 实例序号 = (i + 1).ToString() + ".2";
                string 内容编号 = "B_" + get内容编号((i + 1).ToString());
                string 对应功能点 = "";
                string 内容名称 = alist[i].ToString().Split(' ')[0].ToString();
                string 复用引用 = "";
                string 内容描述 = "";
                if (i > 0)
                {
                  

                    指定位置插入行(ws.get_Range("A" + (4 + i).ToString(), Type.Missing));

                    Excel.Range range = ws.get_Range("A4", "G4");
                    Excel.Range range1 = ws.get_Range("A" + (4 + i).ToString(), System.Type.Missing);
                    复制单元格(range ,range1 );

                }

                ws.Cells[4 + i, 1] = 结构序号;
                ws.Cells[4 + i, 2] = 实例序号;
                ws.Cells[4 + i, 3] = 内容编号;
                ws.Cells[4 + i, 4] = 对应功能点;
                ws.Cells[4 + i, 5] = 内容名称;
                ws.Cells[4 + i, 6] = 复用引用;
                ws.Cells[4 + i, 7] = 内容描述;

            }               
            #endregion
            //app.Visible = true;
            #region "详细信息"
            Excel.Worksheet ws模板1 = (Excel.Worksheet)workBook.Sheets[4];
            Excel.Worksheet ws模板2 = (Excel.Worksheet)workBook.Sheets[5];
            for (int i = 0; i < alist.Count; i++)
            {
                string[] strlist = new string[9] { "OrderNo", "ContexID",  "Remark", "FCODE", "CREATE_BY", "CREATE_DATE", "LAST_UPDATE_BY", "LAST_UPDATE_DATE", "Status" };
                ws模板1.Copy(Type.Missing, workBook.Sheets[6 + i*2]);
                ws模板2.Copy(Type.Missing, workBook.Sheets[6 + i*2+1]);
                Excel.Worksheet wsTep1 = (Excel.Worksheet)workBook.Sheets[6 + i*2+1];
                Excel.Worksheet wsTep2 = (Excel.Worksheet)workBook.Sheets[6 + i*2+2];
                wsTep1.Name = (1 + i).ToString() + ".1";
                wsTep2.Name = (1 + i).ToString() + ".2";

                #region"基础数据说明书-结构说明"
                wsTep1.Select(Type.Missing);

                wsTep1.get_Range("B2", Type.Missing).Value2 = alist[i].ToString().Split(' ')[2].ToString();
                wsTep1.get_Range("B3", Type.Missing).Value2 = alist[i].ToString().Split(' ')[0].ToString();
                wsTep1.get_Range("F3", Type.Missing).Value2 = "基础数据";

                //MyDr[] drl = dt.Select("TableName='" + alist[i].ToString().Split(' ')[2].ToString().Trim()+"'");
                MyDt tepDt = 取得表的字段信息(alist[i].ToString().Split(' ')[2].ToString().Trim(), dt);
                
                for (int index = 0; index < tepDt.Rows.Count; index++)
                {
                    bool tepflg=false;
                    foreach (string tepstr in strlist)
                    {
                        if (tepDt.Rows[index]["ColumnName"].ToString().Trim().ToUpper() == tepstr.Trim().ToUpper())
                        {
                            tepflg = true;
                            break;
                        }
                    }
                    if (!tepflg)
                    {

                        指定位置插入行(wsTep1.get_Range("A" + (6 + index + 1).ToString(), Type.Missing));
                        //if (index > 0)
                        //{
                        //    Excel.Range range = wsTep1.get_Range("A6", "E6");
                        //    Excel.Range range1 = wsTep1.get_Range("A" + (6 + index + 1).ToString(), System.Type.Missing);
                        //    复制单元格(range, range1);
                        //}

                        wsTep1.get_Range("A" + (6 + index).ToString(), Type.Missing).Value2 = tepDt.Rows[index]["ColumnDesc"].ToString();
                        wsTep1.get_Range("B" + (6 + index).ToString(), Type.Missing).Value2 = 取得字符类型(tepDt.Rows[index]["Type"].ToString(), tepDt.Rows[index]["Length"].ToString());
                        wsTep1.get_Range("C" + (6 + index).ToString(), Type.Missing).Value2 = tepDt.Rows[index]["Length"].ToString();
                        wsTep1.get_Range("D" + (6 + index).ToString(), Type.Missing).Value2 = "";
                        wsTep1.get_Range("E" + (6 + index).ToString(), Type.Missing).Value2 = tepDt.Rows[index]["ColumnDesc"].ToString();
                    }
                }
                #endregion
                #region"基础数据说明书-数据实例"
                wsTep2.Select(Type.Missing);
                for (int index = 0; index < tepDt.Rows.Count; index++)
                {
                    bool tepflg=false;
                    foreach (string tepstr in strlist)
                    {
                        if (tepDt.Rows[index]["ColumnName"].ToString().Trim().ToUpper() == tepstr.Trim().ToUpper())
                        {
                            tepflg = true;
                            break;
                        }
                    }
                    if (!tepflg)
                    {
                        if (index > 0)
                        {
                            Excel.Range range = wsTep2.get_Range("A2", "A14");
                            Excel.Range range1 = wsTep2.get_Range(wsTep2.Cells[2, index + 1], wsTep2.Cells[14, index + 1]);
                            复制单元格(range, range1);
                        }
                        wsTep2.Cells[2, index + 1] = tepDt.Rows[index]["ColumnDesc"].ToString();
                    }
                }
                wsTep2.get_Range(wsTep2.Cells[1, 1], wsTep2.Cells[1, tepDt.Rows.Count - 9]).Borders.LineStyle = 1;
                wsTep2.get_Range(wsTep2.Cells[1, 1], wsTep2.Cells[1, tepDt.Rows.Count-9]).Merge(Type.Missing);
                
                wsTep2.Cells[1, 1] = "基础数据说明书-数据实例";
                #endregion
            }
            #endregion
        }

        private static string 取得字符类型(string str, string length)
        {
            switch (str)
            {
                case "int":
                    return "数值";

                case "bigint":
                    return "数值";


                //break;
                case "nvarchar":
                    if (length == "1")
                        return "布尔值";
                    else
                        return "文本";
                case "varchar":
                    if (length == "1")
                        return "布尔值";
                    else
                        return "文本";
                //break;
                case "datetime":
                    return "日期";
                //break;
                case "text":
                    return "文本";
                //break;
                case "decimal":
                    return "数值";
                //break;
                default:
                    return "文本";
                //break;
            }
        }

        private static MyDt 取得表的字段信息(string tablename,MyDt dt)
        {
            MyDt tepDt = dt.Clone();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["TableName"].ToString().Trim() == tablename.Trim())
                {
                    MyDr dr = tepDt.NewRow();
                    dr.ItemArray = dt.Rows[i].ItemArray;
                    tepDt.Rows.Add(dr);
                    i++;
                    while (dt.Rows[i]["TableName"].ToString().Trim().Length == 0)
                    {
                        //
                        dr = tepDt.NewRow();
                        dr.ItemArray = dt.Rows[i].ItemArray;
                        tepDt.Rows.Add(dr);
                        i++;
                    }
                    break;
                }
            }
            return tepDt;
        }


        public static void f_barcode(string URL)
        {
            Excel.Application app = new Excel.ApplicationClass();
            Excel.Workbook workBook = app.Workbooks.Open(URL, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Excel.Worksheet ws_bc1 = (Excel.Worksheet)workBook.Sheets[1];

            app.Visible = true;

            string ls_sql = "select id,barcode,style,color,size,qty,ln from t_url_barcode";
            DataGroup dp_sku = new DataGroup();
            dp_sku = null;
            //DB.GetDataTable(ls_sql);

            ws_bc1.Select(Type.Missing);

            //复制模板区域
            int rowIndex = 0;
            while (rowIndex < 17)
            {
                指定位置插入行(ws_bc1.get_Range("A18", Type.Missing));
                rowIndex++;
            }

            Excel.Range range1 = ws_bc1.get_Range("A1:M17", Type.Missing);
            Excel.Range range = ws_bc1.get_Range("A18:M34", Type.Missing);
            
            复制单元格(range1, range);

            ws_bc1.get_Range("A18", Type.Missing).Value2 = "00152";
            ws_bc1.get_Range("B18", Type.Missing).Value2 = "152";
            ws_bc1.get_Range("A19", Type.Missing).Value2 = "MANGO-21 BLACK";
            ws_bc1.get_Range("B19", Type.Missing).Value2 = "7.5";


        }

        public static void 数据库概要设计文档(MyDt dt, string URL,MyDt FiledDt)
        {
            Excel.Application app = new Excel.ApplicationClass();

            //打开模板文件，得到WorkBook对象
            Excel.Workbook workBook = app.Workbooks.Open(URL, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            Excel.Worksheet ws表清单 = (Excel.Worksheet)workBook.Sheets[3];
            Excel.Worksheet ws表设计说明 = (Excel.Worksheet)workBook.Sheets[4];
            Excel.Worksheet ws存储过程清单 = (Excel.Worksheet)workBook.Sheets[5];
            Excel.Worksheet wsER图 = (Excel.Worksheet)workBook.Sheets[6];
            //dr["value"].ToString() +"  "+ dr["objname"].ToString()

            app.Visible = true;

            #region "表清单"
            ws表清单.Select(Type.Missing);
            for (int i =dt.Rows.Count ; i >0 ; i--)
            {
                插入表清单空行并赋值(ws表清单, i-1, dt,dt.Rows.Count-i,ws表设计说明.Name);
            }
            #endregion

            #region "表设计说明"
            ws表设计说明.Select(Type.Missing);

            //计数
            int RowCount = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //复制模板区域
                int rowIndex = 0;
                while (rowIndex < 7)
                {
                    指定位置插入行(ws表设计说明.get_Range("A3", Type.Missing));
                    rowIndex++;
                }

                Excel.Range range = ws表设计说明.get_Range("A" + (RowCount + 3 + (i + 1) * 7).ToString() + ":I" + (RowCount + 9 + (i + 1) * 7).ToString(), Type.Missing);
                Excel.Range range1 = ws表设计说明.get_Range("A3", Type.Missing);
                复制单元格(range, range1);

                for (int index = 0; index < FiledDt.Rows.Count; index++)
                {
                    if (FiledDt.Rows[index]["TableName"].ToString() == dt.Rows[i]["objname"].ToString())
                    {
                        //填充数据    
                        ws表设计说明.get_Range("B4", Type.Missing).Value2 = "表序号： " + (dt.Rows.Count - i).ToString();
                        ws表设计说明.get_Range("D4", Type.Missing).Value2 = "表名称： " + dt.Rows[i]["value"].ToString();
                        Excel.Range TabRange=ws表设计说明.get_Range("I4", Type.Missing);
                        TabRange.Value2 = dt.Rows[i]["objname"].ToString();
                        TabRange.Name = dt.Rows[i]["objname"].ToString();

                        //ws表设计说明.get_Range("K4", Type.Missing).Value2 = dt.Rows[i]["objname"].ToString().Split('_')[1].ToString();
                        //ws表设计说明.get_Range("K5", Type.Missing).Value2 = dt.Rows[i]["objname"].ToString().Split('_')[1].ToString();
                        if (dt.Rows[i]["objname"].ToString().Substring(0, 2) == "T_")
                        {
                            ws表设计说明.get_Range("K4", Type.Missing).Value2 = dt.Rows[i]["objname"].ToString().Split('_')[1].ToString();
                            ws表设计说明.get_Range("K5", Type.Missing).Value2 = dt.Rows[i]["objname"].ToString().Split('_')[1].ToString();
                        }
                        else
                        {
                            ws表设计说明.get_Range("K4", Type.Missing).Value2 = dt.Rows[i]["objname"].ToString().Split('_')[0].ToString();
                            ws表设计说明.get_Range("K5", Type.Missing).Value2 = dt.Rows[i]["objname"].ToString().Split('_')[0].ToString();
                        }
                        //ws表设计说明.get_Range("B5", Type.Missing).Value2 = "说明： " ;
                       

                        表设计说明插入一行并填充数据(ws表设计说明, index, FiledDt, 0);
                        if (dt.Rows[i]["objname"].ToString().Substring(0, 2) == "T_")
                        {
                            ws表设计说明.get_Range("K8", Type.Missing).Value2 = dt.Rows[i]["objname"].ToString().Split('_')[1].ToString();
                        }
                        else
                        {
                            ws表设计说明.get_Range("K8", Type.Missing).Value2 = dt.Rows[i]["objname"].ToString().Split('_')[0].ToString();
                        }
                        RowCount++;

                        for (int tabIndex = index + 1; tabIndex < FiledDt.Rows.Count; tabIndex++)
                        {
                            if (FiledDt.Rows[tabIndex]["TableName"].ToString().Trim().Length == 0)
                            {
                                表设计说明插入一行并填充数据(ws表设计说明, tabIndex, FiledDt, tabIndex - index);
                                string sTStr = dt.Rows[i]["objname"].ToString();
                                if (sTStr.Substring(0, 2).ToUpper() == "T_")
                                {
                                    ws表设计说明.get_Range("K" + (8 + tabIndex - index).ToString(), Type.Missing).Value2 = dt.Rows[i]["objname"].ToString().Split('_')[1].ToString();
                                }
                                else
                                {
                                    ws表设计说明.get_Range("K" + (8 + tabIndex - index).ToString(), Type.Missing).Value2 = dt.Rows[i]["objname"].ToString().Split('_')[0].ToString();
                                }
                                RowCount++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            #endregion

       }

        private static void 表设计说明插入一行并填充数据(Excel.Worksheet ws,int i,MyDt dt,int RowCount)
        {
            string[] strlist = new string[9] { "OrderNo", "ContexID", "Remark", "FCODE", "CREATE_BY", "CREATE_DATE", "LAST_UPDATE_BY", "LAST_UPDATED_DATE", "Status" };
            //"ORGID",

            指定位置插入行(ws.get_Range("A" + (9 + RowCount).ToString(), Type.Missing));
            string sColDesc = dt.Rows[i]["ColumnDesc"].ToString();
            if (sColDesc.IndexOf("#") > 0)
            {
                sColDesc = sColDesc.Split('#')[0];
            }
            ws.get_Range("B" + (8 + RowCount).ToString(), Type.Missing).Value2 = sColDesc;//dt.Rows[i]["ColumnDesc"].ToString();
            ws.get_Range("C" + (8 + RowCount).ToString(), Type.Missing).Value2 = dt.Rows[i]["ColumnName"].ToString().ToUpper();
            ws.get_Range("D" + (8 + RowCount).ToString(), Type.Missing).Value2 = dt.Rows[i]["Type"].ToString();
            if (dt.Rows[i]["Type"].ToString().ToLower() == "nvarchar")
            {
                if (dt.Rows[i]["Length"].ToString() == "-1")
                {
                    ws.get_Range("E" + (8 + RowCount).ToString(), Type.Missing).Value2 = "MAX";
                }
                else
                {
                    Int32 intLen = int.Parse(dt.Rows[i]["Length"].ToString());
                    ws.get_Range("E" + (8 + RowCount).ToString(), Type.Missing).Value2 = (intLen / 2).ToString();
                }

            }
            else
            {
                ws.get_Range("E" + (8 + RowCount).ToString(), Type.Missing).Value2 = dt.Rows[i]["Length"].ToString();
            }

            if (dt.Rows[i]["PrimaryKey"].ToString().Trim() == "√")
            {
                ws.get_Range("F" + (8 + RowCount).ToString(), Type.Missing).Value2 = "PK";

                //----------------------------------------------------------------
                if (dt.Rows[i]["identity"].ToString().Trim() == "√")
                {
                    ws.get_Range("A" + (8 + RowCount).ToString(), Type.Missing).Value2 = "Y";
                }
                else
                {
                    ws.get_Range("A" + (8 + RowCount).ToString(), Type.Missing).Value2 = "N";
                }
                //----------------------------------------------------------------

            }


            string str约束 = dt.Rows[i]["fk_name"].ToString() + "," + dt.Rows[i]["fk_r_name"].ToString() + "," + dt.Rows[i]["fk_r_c_name"].ToString();
            if (str约束 == ",,")
            {
                str约束 = ""; //dt.Rows[i]["ColumnName"].ToString();
            }
            else
            {
                ws.get_Range("F" + (8 + RowCount).ToString(), Type.Missing).Value2 = "FK";
            }

            ws.get_Range("G" + (8 + RowCount).ToString(), Type.Missing).Value2 = str约束;
            ws.get_Range("H" + (8 + RowCount).ToString(), Type.Missing).Value2 = dt.Rows[i]["Default"].ToString();
            ws.get_Range("I" + (8 + RowCount).ToString(), Type.Missing).Value2 = dt.Rows[i]["ColumnDesc"].ToString();
            ws.get_Range("L" + (8 + RowCount).ToString(), Type.Missing).Value2 =dt.Rows[i]["IndexName"].ToString(); //dt.Rows[i]["ColumnName"].ToString();



            foreach (string str in strlist)
            {
                if (dt.Rows[i]["ColumnName"].ToString().Trim().ToUpper() == str.ToUpper())
                    ws.get_Range("B" + (8 + RowCount).ToString(), "I" + (8 + RowCount).ToString()).Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkGray );
                        //.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DeepPink);
            }
            
        }

        //private static string 获取索引信息(MyDt indexDt,)

        /// <summary>
        /// i_name 索引名
        /// i_t_name 表名
        /// i_c_name 字段名
        /// </summary>
        /// <returns></returns>
        public static string 获取所有索引()
        {
            return @"select i_name=i.name ,i_t_name=o.name,
                        i_c_name=(select name from sys.all_columns ac 
                        where ac.object_id=i.object_id and ac.column_id=ic.column_id)
                        from sys.indexes i
                        left outer join sys.objects o on o.object_id=i.object_id and o.is_ms_shipped=0
                        left outer join sys.index_columns ic on ic.object_id=i.object_id and ic.index_id=i.index_id";
        }

        /// <summary>
        /// fk_name FK名称
        /// fk_t_name 表名
        /// fk_r_name 关联表名
        /// fk_t_c_name 字段名
        /// fk_r_c_name 关联字段名
        /// </summary>
        /// <returns></returns>
        public static string 获取所有FK()
        {
            return @"select fk_name=fk.name,fk_t_name= o.name,fk_r_name=o1.name,
                        fk_t_c_name =(select name from sys.all_columns ac 
                        where fkc.parent_column_id=ac.column_id and ac.object_id=fk.parent_object_id),
                        fk_r_c_name =(select name from sys.all_columns ac 
                        where fkc.referenced_column_id=ac.column_id and ac.object_id=fk.referenced_object_id)
                        --,fkc.parent_column_id
                        --,fkc.referenced_column_id,fk.object_id
                        from  sys.foreign_keys  fk
                        left outer join sys.objects o on fk.parent_object_id=o.object_id
                        left outer join sys.objects o1 on fk.referenced_object_id=o1.object_id
                        left outer join sys.foreign_key_columns fkc on fkc.constraint_object_id=fk.object_id";
        }

        /// <summary>
        ///TableName:表名
        ///TableDesc:表说明
        ///Column_id:字段序号
        ///ColumnName:字段名
        ///PrimaryKey:是否主键
        ///IDENTITY:是否自增型
        ///Computed:
        ///Type:字段类型
        ///Length:长度
        ///Precision:
        ///Scale:
        ///NullAble:是否可以为空
        ///Default:默认值
        ///ColumnDesc:字段说明
        ///IndexName:索引
        ///IndexSort:排序
        ///Create_Date:创建时间
        ///Modify_Date:修改时间
        /// </summary>
        /// <returns></returns>
        public static string 获取所有字段信息的SQL语句( )
        {
            string sqlstr = @"SELECT 
                                TableName=CASE WHEN C.column_id=1 THEN O.name ELSE N'' END,
                                TableDesc=ISNULL(CASE WHEN C.column_id=1 THEN PTB.[value] END,N''),
                                Column_id=C.column_id,
                                ColumnName=C.name,
                                PrimaryKey=ISNULL(IDX.PrimaryKey,N''),
                                [IDENTITY]=CASE WHEN C.is_identity=1 THEN N'√'ELSE N'' END,
                                Computed=CASE WHEN C.is_computed=1 THEN N'√'ELSE N'' END,
                                Type=T.name,
                                Length=C.max_length,
                                Precision=C.precision,
                                Scale=C.scale,
                                NullAble=CASE WHEN C.is_nullable=1 THEN N'√'ELSE N'' END,
                                [Default]=ISNULL(D.definition,N''),
                                ColumnDesc=ISNULL(PFD.[value],N''),
                                IndexName=ISNULL(IDX.IndexName,N''),
                                IndexSort=ISNULL(IDX.Sort,N''),
                                Create_Date=O.Create_Date,
                                Modify_Date=O.Modify_date
                                FROM sys.columns C
                                INNER JOIN sys.objects O
                                ON C.[object_id]=O.[object_id]
                                AND O.type='U'
                                AND O.is_ms_shipped=0
                                INNER JOIN sys.types T
                                ON C.user_type_id=T.user_type_id
                                LEFT JOIN sys.default_constraints D
                                ON C.[object_id]=D.parent_object_id
                                AND C.column_id=D.parent_column_id
                                AND C.default_object_id=D.[object_id]
                                LEFT JOIN sys.extended_properties PFD
                                ON PFD.class=1 
                                AND C.[object_id]=PFD.major_id 
                                AND C.column_id=PFD.minor_id
                                -- AND PFD.name='Caption' -- 字段说明对应的描述名称(一个字段可以添加多个不同name的描述)
                                LEFT JOIN sys.extended_properties PTB
                                ON PTB.class=1 
                                AND PTB.minor_id=0 
                                AND C.[object_id]=PTB.major_id
                                -- AND PFD.name='Caption' -- 表说明对应的描述名称(一个表可以添加多个不同name的描述) bitsCN.Com网管联盟 

                                LEFT JOIN -- 索引及主键信息
                                (
                                SELECT 
                                IDXC.[object_id],
                                IDXC.column_id,
                                Sort=CASE INDEXKEY_PROPERTY(IDXC.[object_id],IDXC.index_id,IDXC.index_column_id,'IsDescending')
                                WHEN 1 THEN 'DESC' WHEN 0 THEN 'ASC' ELSE '' END,
                                PrimaryKey=CASE WHEN IDX.is_primary_key=1 THEN N'√'ELSE N'' END,
                                IndexName=IDX.Name
                                FROM sys.indexes IDX
                                INNER JOIN sys.index_columns IDXC
                                ON IDX.[object_id]=IDXC.[object_id]
                                AND IDX.index_id=IDXC.index_id
                                LEFT JOIN sys.key_constraints KC
                                ON IDX.[object_id]=KC.[parent_object_id]
                                AND IDX.index_id=KC.unique_index_id
                                INNER JOIN -- 对于一个列包含多个索引的情况,只显示第1个索引信息
                                (
                                SELECT [object_id], Column_id, index_id=MIN(index_id)
                                FROM sys.index_columns
                                GROUP BY [object_id], Column_id 


                                ) IDXCUQ
                                ON IDXC.[object_id]=IDXCUQ.[object_id]
                                AND IDXC.Column_id=IDXCUQ.Column_id
                                AND IDXC.index_id=IDXCUQ.index_id
                                ) IDX
                                ON C.[object_id]=IDX.[object_id]
                                AND C.column_id=IDX.column_id
                                WHERE --C.name NOT IN ( 'ContexID', 'FCODE', 'CREATE_BY', 'CREATE_DATE', 'LAST_UPDATE_BY', 'LAST_UPDATE_DATE') 
                                --and 
                                O.name<>'sysdiagrams'
                                 --WHERE O.name=N'{0}' -- 如果只查询指定表,加上此条件
                                ORDER BY O.name,C.column_id ";
            //return string.Format(sqlstr, sTableName);
            return sqlstr;
        }

        private  static MyDt 获取所有表的字段信息()
        {
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();

            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();

            string sqlstr = 获取所有字段信息的SQL语句();
                //"SELECT objtype, objname, name, value FROM fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', '" +
                //sTableName + "', 'column', NULL);";
            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
            MyDt  dtColumnExtendedPropert = new MyDt(); // 列的扩展属性
            adapter.Fill(dtColumnExtendedPropert);

            return dtColumnExtendedPropert;
        }

        private static void 插入表清单空行并赋值(Excel.Worksheet ws,int i,MyDt dt,int space,string linkSheetName)
        {
            指定位置插入行(ws.get_Range("A" + (5 + space).ToString(), Type.Missing));
            ws.get_Range("B" + (4 + space).ToString(), Type.Missing).Value2 = (space + 1).ToString();
            ws.get_Range("C" + (4 + space).ToString(), Type.Missing).Value2 = dt.Rows[i]["value"].ToString();
            
            Excel.Range range = ws.get_Range("D" + (4 + space).ToString(), Type.Missing);
            range.Value2 = dt.Rows[i]["objname"].ToString();
            Excel.Hyperlink link = (Excel.Hyperlink)range.Hyperlinks.Add(range, "", linkSheetName + "!" + dt.Rows[i]["objname"].ToString(), Type.Missing, Type.Missing);
        }

        /// <summary>
        /// 把range复制到range1
        /// </summary>
        /// <param name="range"></param>
        /// <param name="range1"></param>
        private static void 复制单元格(Excel.Range range, Excel.Range range1)
        {
            range.Copy(Type.Missing);
            range1.PasteSpecial(Excel.XlPasteType.xlPasteAll, Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, System.Type.Missing, System.Type.Missing);
        }

        private static string get内容编号(string str)
        {
            if (str.Length == 1)
            {
                str = "00" + str;
            }
            else if (str.Length == 2)
            {
                str = "0" + str;
            }
            return str;
        }
        
        /// <summary>
        /// 在指定的行上插入一整行   
        /// </summary>
        public static  void 指定位置插入行(Excel.Range range)
        {
            range.Select();
            range.EntireRow.Insert(Type.Missing,Type.Missing);   
        }

        /// <summary>
        /// 生成外部设计文档
        /// </summary>
        /// <param name="dt">外部设计资料数据</param>
        /// <param name="URL">外部设计模板路径</param>
        public static void 功能外部设计文档(MyDt dt, string URL,MyDt allFiledDt)
        {
            Excel.Application app = new Excel.ApplicationClass();

            //打开模板文件，得到WorkBook对象
            Excel.Workbook workBook = app.Workbooks.Open(URL, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            Excel.Worksheet ws功能清单 = (Excel.Worksheet)workBook.Sheets[3];
            Excel.Worksheet ws页面清单 = (Excel.Worksheet)workBook.Sheets[4];
            Excel.Worksheet ws模板 = (Excel.Worksheet)workBook.Sheets[5];
            
            app.Visible = true;

            #region "功能清单"

            ws功能清单.Select(Type.Missing);

            MyDt dt功能清单 = 获取功能清单的dt(dt, 2);

            for (int i = 0; i < dt功能清单.Rows.Count; i++)
            {
                在功能清单页插入一行并给单元格赋值(ws功能清单, i, dt功能清单);

                Int32 index = i;

                if (i < dt功能清单.Rows.Count - 1)
                {

                    if (dt功能清单.Rows[i + 1]["F1"].ToString().Trim().Length == 0)
                    {
                        在功能清单页插入一行并给单元格赋值(ws功能清单, i + 1, dt功能清单);

                        index += 2;
                        while (index < dt功能清单.Rows.Count)
                        {
                            if (dt功能清单.Rows[index]["F1"].ToString().Trim().Length == 0)
                            {
                                在功能清单页插入一行并给单元格赋值(ws功能清单, index, dt功能清单);
                            }
                            else
                            {
                                break;
                            }
                            index++;
                        }
                        ws功能清单.get_Range(ws功能清单.Cells[4 + i, 1], ws功能清单.Cells[4 + index - 1, 1]).Merge(Type.Missing);
                        ws功能清单.get_Range(ws功能清单.Cells[4 + i, 2], ws功能清单.Cells[4 + index - 1, 2]).Merge(Type.Missing);
                        i = index - 1;
                    }
                }
            }
            #endregion

            #region"页面清单"

            ws页面清单.Select(Type.Missing);

            MyDt dt页面清单 = 获取页面清单的dt(dt, 2);

            for (int i = 0; i < dt页面清单.Rows.Count; i++)
            {
                在页面清单页插入一行并给单元格赋值(ws页面清单, i, dt页面清单);

                Int32 index = i;

                if (i < dt页面清单.Rows.Count - 1)
                {

                    if (dt页面清单.Rows[i + 1]["F3"].ToString().Trim().Length == 0)
                    {
                        在页面清单页插入一行并给单元格赋值(ws页面清单, i + 1, dt页面清单);

                        index += 2;
                        while (index < dt页面清单.Rows.Count)
                        {
                            if (dt页面清单.Rows[index]["F3"].ToString().Trim().Length == 0)
                            {
                                在页面清单页插入一行并给单元格赋值(ws页面清单, index, dt页面清单);
                            }
                            else
                            {
                                break;
                            }
                            index++;
                        }
                        ws页面清单.get_Range(ws页面清单.Cells[4 + i, 1], ws页面清单.Cells[4 + index - 1, 1]).Merge(Type.Missing);
                        ws页面清单.get_Range(ws页面清单.Cells[4 + i, 2], ws页面清单.Cells[4 + index - 1, 2]).Merge(Type.Missing);
                        i = index - 1;
                    }
                }
            }

            #endregion

            #region "添加页面清单中的链接 "
            ws页面清单.Select(Type.Missing);
            for (int i = 0; i < dt页面清单.Rows.Count; i++)
            {
                Excel.Range range = ws页面清单.get_Range("C" + (4 + i).ToString(), Type.Missing);
                Excel.Hyperlink link = (Excel.Hyperlink)range.Hyperlinks.Add(range, "", range.Value2.ToString().Trim() + "!A1", Type.Missing, Type.Missing);
            }
            //Excel.Range range;
            //range.Hyperlinks= 
            #endregion

            #region"功能设计"

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //一级模块：F1   二级模块：F3   三级模块 NULL 				
            //
            //1、项目:      功能编码：F2 功能名称：F3
            //2、业务概要   执行者：F6 概要：F7 约束：F8
            //3、处理时机   前置条件：F9
            //4、业务流程   ＮＵＬＬ
            //5、处理步骤   基本路径流程描述：F11　扩展路径流程描述：无
            //6、异常处理   业务规则校验：F10
            //              常规数据校验：
            //              1、	界面上所有红色标出的输入框不能为空。											
            //              2、输入的所有的扣减金额必须为数字。	
            //              一般性处理：
            //              1、当指定条件查询没有记录时，系统显示消息：“没有找到符合指定条件的查询记录！”
            //              系统错误处理：
            //              1、当系统执行操作过程中出现程序错误时，将错误信息写入日志，并提示：“系统出现异常无法进行操作，请通知系统管理员检查！”
            //              2、	当金额输入错误时，提示：“***扣减费用输入错误，请重新输入！”					
            //
            //7、输入输出
            //8、画面样式
            //9、页面初始化描述
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            ws模板.Select(Type.Missing);

            //for (int index = 0; index < dt页面清单.Rows.Count - 220; index++)
            for (int index = 0; index < dt页面清单.Rows.Count ; index++)
            {
                ws模板.Copy(Type.Missing, workBook.Sheets[5 + index]);
                Excel.Worksheet TepSw = (Excel.Worksheet)workBook.Sheets[5 + index + 1];
                TepSw.Name = dt页面清单.Rows[index]["F5"].ToString();
                TepSw.Select(Type.Missing);

                //计数变量
                Int32 RowIndex = 0;

                //一级模块
                TepSw.get_Range("K1", Type.Missing).Value2 = 获取一级模块名称(dt页面清单, index);
                //二级模块
                TepSw.get_Range("W1", Type.Missing).Value2 = 获取二级模块名称(dt页面清单, index);
                //三级模块
                TepSw.get_Range("AJ1", Type.Missing).Value2 = dt页面清单.Rows[index]["F6"].ToString();

                Excel.Range tepRange;

                //插入一行
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("B" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "1、项目";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "功能编码：";
                tepRange = TepSw.get_Range("L" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = dt页面清单.Rows[index]["F5"].ToString();
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "功能名称：";
                tepRange = TepSw.get_Range("L" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = dt页面清单.Rows[index]["F6"].ToString();

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("B" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "2、业务概要";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "执行者：";
                tepRange = TepSw.get_Range("K" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = dt页面清单.Rows[index]["F7"].ToString();
                tepRange.Font.ColorIndex = 5;
                tepRange.Font.Italic = true;

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "概  要：";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = dt页面清单.Rows[index]["F8"].ToString();
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "约  束：";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = dt页面清单.Rows[index]["F9"].ToString();

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("B" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "3、处理时机";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "前置条件：";

                RowIndex = 根据回车符分割数据并填充数据(dt页面清单.Rows[index]["F10"].ToString(), TepSw, RowIndex);

                //if (dt页面清单.Rows[index]["F9"].ToString().Trim().Length != 0)
                //{
                //    string[] strRange = Regex.Split(dt页面清单.Rows[index]["F9"].ToString(), "\n");
                //    for (int arlindex = 0; arlindex < strRange.Length; arlindex++)
                //    {
                //        RowIndex++;
                //        在设计文档中插入一行(TepSw, RowIndex);
                //        tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                //        tepRange.Value2 = strRange[arlindex ].ToString();
                //    }
                //}




                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("B" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "4、业务流程";

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("B" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "5、处理步骤";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "基本路径流程描述：";
                RowIndex = 根据回车符分割数据并填充数据(dt页面清单.Rows[index]["F12"].ToString(), TepSw, RowIndex);
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "扩展路径流程描述：";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "无";

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                //6、异常处理
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("B" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "6、异常处理";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "常规数据校验：";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "1、界面上所有红色标出的输入框不能为空。";
                //RowIndex++;
                //在设计文档中插入一行(TepSw, RowIndex);
                //tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                //tepRange.Value2 = "2、输入的所有的扣减金额必须为数字。";

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "业务规则校验：";
                RowIndex = 根据回车符分割数据并填充数据(dt页面清单.Rows[index]["F11"].ToString(), TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "一般性处理：";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "1、当指定条件查询没有记录时，系统显示消息：“没有找到符合指定条件的查询记录！”";

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("H" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "系统错误处理：";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "1、当系统执行操作过程中出现程序错误时，将错误信息写入日志，并提示：“系统出现异常无法进行操作，请通知系统管理员检查！”";
                //RowIndex++;
                //在设计文档中插入一行(TepSw, RowIndex);
                //tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                //tepRange.Value2 = "2、	当金额输入错误时，提示：“***扣减费用输入错误，请重新输入！”";

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                //7、输入输出
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("B" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "7、输入输出";
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "输入：";
                //////////////添加输入输出
                //字段,类型,长度,输入约束,默认值,下拉框数据源,为空,说明	
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                定义输入的表头(tepRange, TepSw, RowIndex);
                MyDs OIDs = 获取输入输出字段(dt页面清单.Rows[index]["F5"].ToString().Trim());
                
                              
                MyDt TepAllFiledDt = allFiledDt.Copy();
                for (int i = 1; i < TepAllFiledDt.Rows.Count; i++)
                {
                    if (TepAllFiledDt.Rows[i]["TableName"].ToString().Trim().Length == 0)
                    {
                        TepAllFiledDt.Rows[i]["TableName"] = TepAllFiledDt.Rows[i - 1]["TableName"].ToString();
                    }
                }
                插入输入输出的值(OIDs.Tables[0], TepAllFiledDt, tepRange, TepSw, ref RowIndex,true );


                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "输出：";
                //////////////添加输入输出
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                定义输出表头(tepRange, TepSw, RowIndex);
                插入输入输出的值(OIDs.Tables[1], TepAllFiledDt, tepRange, TepSw, ref RowIndex, false );


                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);

                //8、画面样式
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("B" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "8、画面样式";

                for (int stlyindex = 0; stlyindex < 30; stlyindex++)
                {
                    RowIndex++;
                    在设计文档中插入一行(TepSw, RowIndex);
                }
                   

                //9、页面初始化描述
                RowIndex++;
                在设计文档中插入一行(TepSw, RowIndex);
                tepRange = TepSw.get_Range("B" + (2 + RowIndex).ToString(), Type.Missing);
                tepRange.Value2 = "9、页面初始化描述";

            }
            #endregion

            
                //app.Visible = true;
        }

        #region "定义输出表头"
        private static void 定义表头(Excel.Range tepRange, string RangeName)
        {
            tepRange.Merge(Type.Missing);
            tepRange.HorizontalAlignment = HorizontalAlignment.Center;
            //tepRange.Font.Bold = true;
            tepRange.Borders.LineStyle = 1;
            tepRange.Font.Size = 10;
            tepRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
            tepRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkGray);
            tepRange.Value2 = RangeName;
        }

        private static void 定义输出表头(Excel.Range tepRange, Excel.Worksheet TepSw, int RowIndex)
        {
            tepRange = TepSw.get_Range("K" + (2 + RowIndex).ToString(), "R" + (2 + RowIndex).ToString());
            定义表头(tepRange, "字段");
            tepRange = TepSw.get_Range("S" + (2 + RowIndex).ToString(), "V" + (2 + RowIndex).ToString());
            定义表头(tepRange, "类型");
            tepRange = TepSw.get_Range("W" + (2 + RowIndex).ToString(), "Y" + (2 + RowIndex).ToString());
            定义表头(tepRange, "长度");
            tepRange = TepSw.get_Range("Z" + (2 + RowIndex).ToString(), "AX" + (2 + RowIndex).ToString());
            定义表头(tepRange, "说明");
        }

        #endregion

        #region "定义输出数值的行格式并赋值"
        private static void 定义输出数值的行格式并赋值(Excel.Range tepRange, Excel.Worksheet TepSw, int RowIndex, MyDr dr)
        {
            tepRange = TepSw.get_Range("K" + (2 + RowIndex).ToString(), "R" + (2 + RowIndex).ToString());
            定义行格式并赋值(tepRange, dr["ColumnDesc"].ToString());
            tepRange = TepSw.get_Range("S" + (2 + RowIndex).ToString(), "V" + (2 + RowIndex).ToString());
            tepRange.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop, Type.Missing, "文本,数字,日期,布尔值", Type.Missing);
            定义行格式并赋值(tepRange, 外部设计取得字符类型(dr["Type"].ToString(), dr["Length"].ToString()));
            tepRange = TepSw.get_Range("W" + (2 + RowIndex).ToString(), "Y" + (2 + RowIndex).ToString());
            定义行格式并赋值(tepRange, dr["Length"].ToString());
            tepRange = TepSw.get_Range("Z" + (2 + RowIndex).ToString(), "AX" + (2 + RowIndex).ToString());
            定义行格式并赋值(tepRange, "");
        }
        #endregion

        private static void 插入输入输出的值(MyDt filedDt,MyDt dt,Excel.Range tepRange,Excel.Worksheet TepSw,ref int RowIndex,bool flg)
        {
            for (int i = 0; i < filedDt.Rows.Count; i++)
            {
                string tableName = filedDt.Rows[i]["table_id"].ToString();
                string filedName = filedDt.Rows[i]["filed_id"].ToString();
                MyDr[] dr = dt.Select("ColumnName='" + filedName + "' and TableName='" + tableName + "'");
                if (dr.Length > 0)
                {
                    RowIndex++;  
                    在设计文档中插入一行(TepSw, RowIndex);
                    if (flg)
                        定义输入数值的行格式并赋值(tepRange, TepSw, RowIndex, dr[0]);
                    else
                        定义输出数值的行格式并赋值(tepRange, TepSw, RowIndex, dr[0]);
                }
            }
        }

        #region "定义输入数值的行格式并赋值"

        private static void 定义行格式并赋值(Excel.Range tepRange, string RangeName)
        {
            tepRange.Merge(Type.Missing);
            //tepRange.HorizontalAlignment = HorizontalAlignment.Left ;
            //tepRange.Font.Bold = true;
            tepRange.Borders.LineStyle = 1;
            tepRange.Font.Size = 10;
            //tepRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
            //tepRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkGray);
            tepRange.Value2 = RangeName;
        }

        private static void 定义输入数值的行格式并赋值(Excel.Range tepRange, Excel.Worksheet TepSw, int RowIndex, MyDr dr)
        {
            tepRange = TepSw.get_Range("K" + (2 + RowIndex).ToString(), "R" + (2 + RowIndex).ToString());
            定义行格式并赋值(tepRange, dr["ColumnDesc"].ToString());
            tepRange = TepSw.get_Range("S" + (2 + RowIndex).ToString(), "V" + (2 + RowIndex).ToString());
            tepRange.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop, Type.Missing, "文本,数字,日期,布尔值", Type.Missing);
            定义行格式并赋值(tepRange, 外部设计取得字符类型(dr["Type"].ToString(), dr["Length"].ToString()));
            tepRange = TepSw.get_Range("W" + (2 + RowIndex).ToString(), "Y" + (2 + RowIndex).ToString());
            定义行格式并赋值(tepRange, dr["Length"].ToString());
            tepRange = TepSw.get_Range("Z" + (2 + RowIndex).ToString(), "AD" + (2 + RowIndex).ToString());
            //tepRange.Validation.Modify(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop, Type.Missing, "1,2,3", Type.Missing);
            tepRange.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop, Type.Missing, "文本输入,只读字段,控制输入", Type.Missing);
            定义行格式并赋值(tepRange, 输入约束(dr["PrimaryKey"].ToString().Trim(), dr["Type"].ToString().Trim()));
            tepRange = TepSw.get_Range("AE" + (2 + RowIndex).ToString(), "AI" + (2 + RowIndex).ToString());
            定义行格式并赋值(tepRange, dr["Default"].ToString());
            tepRange = TepSw.get_Range("AJ" + (2 + RowIndex).ToString(), "AN" + (2 + RowIndex).ToString());
            定义行格式并赋值(tepRange, "");
            tepRange = TepSw.get_Range("AO" + (2 + RowIndex).ToString(), "AP" + (2 + RowIndex).ToString());//"√"
            tepRange.Validation.Add(Excel.XlDVType.xlValidateList, Excel.XlDVAlertStyle.xlValidAlertStop, Type.Missing, "否,是", Type.Missing);
            //if (dr["PrimaryKey"].ToString().Trim() == "√")
            if (dr["Type"].ToString().Trim() == "bigint")
                //upbylzh
                定义行格式并赋值(tepRange, "否");
            else
                定义行格式并赋值(tepRange, "");
            tepRange = TepSw.get_Range("AQ" + (2 + RowIndex).ToString(), "AX" + (2 + RowIndex).ToString());
            定义行格式并赋值(tepRange, "");
        }

        //private static string Get为空(string key, string type)
        //{
        //    if (key == "√")
        //        return "只读字段";
        //    if (type == "int")
        //        return "控制输入";
        //    else
        //        return "文本输入";
        //}

        private static string 输入约束(string key,string type)
        {
            if (key == "√")
                return "只读字段";
            if (type == "int")
                return "控制输入";
            else
                return "文本输入";
        }

        private static string 外部设计取得字符类型(string str,string length)
        {
            switch (str)
            {
                case "int":
                    return "数值";
                case "bigint":
                    return "数值";

                //break;
                case "nvarchar":
                    if (length == "1")
                        return "布尔值";
                    else
                        return "文本";
                case "varchar":
                    if (length == "1")
                        return "布尔值";
                    else
                        return "文本";
                //break;
                case "datetime":
                    return "日期";
                //break;
                case "text":
                    return "文本";
                //break;
                case "decimal":
                    return "数值";
                //break;
                default:
                    return "文本";
                //break;
            }
        }

        #endregion

        #region "定义输入的表头"

        private static void 定义输入的表头(Excel.Range tepRange, Excel.Worksheet TepSw,int RowIndex)
        {
            tepRange = TepSw.get_Range("K" + (2 + RowIndex).ToString(), "R" + (2 + RowIndex).ToString());
            定义表头(tepRange, "字段");
            tepRange = TepSw.get_Range("S" + (2 + RowIndex).ToString(), "V" + (2 + RowIndex).ToString());
            定义表头(tepRange, "类型");
            tepRange = TepSw.get_Range("W" + (2 + RowIndex).ToString(), "Y" + (2 + RowIndex).ToString());
            定义表头(tepRange, "长度");
            tepRange = TepSw.get_Range("Z" + (2 + RowIndex).ToString(), "AD" + (2 + RowIndex).ToString());
            定义表头(tepRange, "输入约束");
            tepRange = TepSw.get_Range("AE" + (2 + RowIndex).ToString(), "AI" + (2 + RowIndex).ToString());
            定义表头(tepRange, "默认值");
            tepRange = TepSw.get_Range("AJ" + (2 + RowIndex).ToString(), "AN" + (2 + RowIndex).ToString());
            定义表头(tepRange, "下拉框数据源");
            tepRange = TepSw.get_Range("AO" + (2 + RowIndex).ToString(), "AP" + (2 + RowIndex).ToString());
            定义表头(tepRange, "为空");
            tepRange = TepSw.get_Range("AQ" + (2 + RowIndex).ToString(), "AX" + (2 + RowIndex).ToString());
            定义表头(tepRange, "说明");
        }
        #endregion

        private static MyDs  获取输入输出字段(string PAGEID)
        {
            MyDs ds = new MyDs();
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            //string connstr = configurationAppSettings.GetValue("get_filed_connstr", typeof(string)).ToString();
            string connstr = configurationAppSettings.GetValue("get_TOOL_connstr", typeof(string)).ToString();

            SqlConnection  myConn = new SqlConnection(connstr);
            //string strSql = "select * from 页面中Key区域使用到的表及字段 ; select * from 页面Grid区域使用到的表和字段 ; ";
            string strSql = "select * from PAGE_INFO where page_id='" + PAGEID + "' and  oi_flg='1' ; select * from PAGE_INFO where page_id='" + PAGEID + "' and  oi_flg='0' ;";

            SqlDataAdapter da = new SqlDataAdapter(strSql, myConn);
            da.Fill(ds);
            return ds;
        }

        private static Int32  根据回车符分割数据并填充数据(string str,Excel.Worksheet TepSw,Int32 RowIndex)
        {
            if (str.Trim().Length != 0)
            {
                string[] strRange = Regex.Split(str, "\n");
                for (int arlindex = 0; arlindex < strRange.Length; arlindex++)
                {
                    RowIndex++;
                    在设计文档中插入一行(TepSw, RowIndex);
                    Excel.Range tepRange;
                    tepRange = TepSw.get_Range("I" + (2 + RowIndex).ToString(), Type.Missing);
                    tepRange.Value2 = strRange[arlindex].ToString();
                }
            }
            return RowIndex;
        }

        private static string 获取二级模块名称(MyDt dt, int index)
        {
            if (dt.Rows[index]["F4"].ToString().Trim().Length == 0)
            {
                index--;
                while (dt.Rows[index]["F4"].ToString().Trim().Length == 0)
                {
                    if (index == 0)
                        return "";
                    index--;
                }
                return dt.Rows[index]["F4"].ToString();
            }
            else
            {
                return dt.Rows[index]["F4"].ToString();
            }
        }

        private static string 获取一级模块名称(MyDt dt,int index)
        {
            if (dt.Rows[index]["F1"].ToString().Trim().Length == 0)
            {
                index--;
                while (dt.Rows[index]["F1"].ToString().Trim().Length == 0)
                {
                    if (index == 0)
                        return "";
                    index--;
                }
                string[] strValue = Regex.Split(dt.Rows[index]["F1"].ToString().Replace("\r\n", "\n"), "\n");
                if (strValue.Length > 1)
                {
                    return strValue[1].ToString();
                }
                else
                {
                    return strValue[0].ToString();
                }
            }
            else
            {
                string[] strValue = Regex.Split(dt.Rows[index]["F1"].ToString().Replace("\r\n", "\n"), "\n");
                if (strValue.Length > 1)
                {
                    return strValue[1].ToString();
                }
                else
                {
                    return strValue[0].ToString();
                }
                
            }
        }

        private static MyDt 获取页面清单的dt(MyDt dt, int starRow)
        {
            MyDt tpDt = dt.Clone();
            for (int i = starRow; i < dt.Rows.Count; i++)
            {
                //if (dt.Rows[i]["F2"].ToString().Trim().Length != 0)
                //{
                    MyDr dr = tpDt.NewRow();
                    dr.ItemArray = dt.Rows[i].ItemArray;
                    tpDt.Rows.Add(dr);
                //}
            }
            return tpDt;
        }

        private static MyDt  获取功能清单的dt(MyDt  dt,int starRow)
        {
            MyDt tpDt = dt.Clone();
            for (int i = starRow; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["F3"].ToString().Trim().Length != 0)
                {
                    MyDr dr = tpDt.NewRow();
                    dr.ItemArray = dt.Rows[i].ItemArray;
                    tpDt.Rows.Add(dr);
                }
            }
            return tpDt;
        }

        private static void 在设计文档中插入一行(Excel.Worksheet TepSw,int RowIndex)
        {
            指定位置插入行(TepSw.get_Range("A" + (3 + RowIndex).ToString(), Type.Missing));
        }

        /// <summary>
        /// 在功能清单页插入一行并给单元格赋值
        /// 以便复用循环
        /// </summary>
        /// <param name="i"></param>
        private static void 在功能清单页插入一行并给单元格赋值(Excel.Worksheet ws功能清单, int i,MyDt dt)
        {
            指定位置插入行(ws功能清单.get_Range("A" + (5 + i ).ToString(), Type.Missing));
            string[] strValue = Regex.Split(dt.Rows[i]["F1"].ToString().Replace("\r\n", "\n"), "\n");
            if (strValue.Length > 1)
            {
                ws功能清单.Cells[4 + i, 1] = strValue[0].ToString();
                // dt.Rows[i]["F1"].ToString();
                ws功能清单.Cells[4 + i, 2] = strValue[1].ToString();
            }
            else
            {
                ws功能清单.Cells[4 + i, 2] = dt.Rows[i]["F1"].ToString();
            }
            ws功能清单.Cells[4 + i , 3] = dt.Rows[i]["F3"].ToString();
            ws功能清单.Cells[4 + i , 4] = dt.Rows[i]["F4"].ToString();
        }

        /// <summary>
        /// 在页面清单页插入一行并给单元格赋值
        /// 以便复用循环
        /// </summary>
        /// <param name="i"></param>
        private static void 在页面清单页插入一行并给单元格赋值(Excel.Worksheet ws页面清单, int i, MyDt dt)
        {
            指定位置插入行(ws页面清单.get_Range("A" + (5 + i).ToString(), Type.Missing));
            ws页面清单.Cells[4 + i, 1] = dt.Rows[i]["F3"].ToString();
            ws页面清单.Cells[4 + i, 2] = dt.Rows[i]["F4"].ToString();
            ws页面清单.Cells[4 + i, 3] = dt.Rows[i]["F5"].ToString();
            ws页面清单.Cells[4 + i, 4] = dt.Rows[i]["F6"].ToString();
        }

        public static string 把报价导入数据库(string URL)
        {
            Excel.Application app = new Excel.ApplicationClass();
            Excel.Workbook workBook = app.Workbooks.Open(URL, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Excel.Worksheet ws表设计 = (Excel.Worksheet)workBook.Sheets[1];

            try
            {
                ws表设计.Select(Type.Missing);
                int indexCount = 0;
                string SqlStr = string.Empty;


                string ls_工厂名 = ws表设计.get_Range("A" + (2 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim();
                string ls_网址 = "";
                if (ws表设计.get_Range("B" + (2 + indexCount).ToString(), Type.Missing).Value2 != null)
                {
                    ls_网址 = ws表设计.get_Range("B" + (2 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim();
                }
                string ls_联系信息1 = "";
                if (ws表设计.get_Range("C" + (2 + indexCount).ToString(), Type.Missing).Value2 != null)
                {
                    ls_联系信息1 = ws表设计.get_Range("C" + (2 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim();
                }
                string ls_联系信息2 = "";
                if (ws表设计.get_Range("D" + (2 + indexCount).ToString(), Type.Missing).Value2 != null)
                {
                    ls_联系信息2 = ws表设计.get_Range("D" + (2 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim();
                }
                string ls_联系信息3 = "";
                if (ws表设计.get_Range("E" + (2 + indexCount).ToString(), Type.Missing).Value2 != null) 
                {
                    ls_联系信息3 = ws表设计.get_Range("E" + (2 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim();
                }
                string ls_联系信息4 = "";
                if (ws表设计.get_Range("F" + (2 + indexCount).ToString(), Type.Missing).Value2 != null)
                {
                    ls_联系信息4 = ws表设计.get_Range("F" + (2 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim();
                }
                string ls_联系信息5 = "";
                if (ws表设计.get_Range("G" + (2 + indexCount).ToString(), Type.Missing).Value2 != null)
                {
                    ls_联系信息5 = ws表设计.get_Range("G" + (2 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim();
                }


                string ls_条款 = "";

                while (ws表设计.get_Range("A" + (4 + indexCount).ToString(), Type.Missing).Value2 !=null)
                {
                    ls_条款 += ws表设计.get_Range("A" + (4 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim() + "<br>";

                    indexCount++;
                    if (ws表设计.get_Range("A" + (4 + indexCount).ToString(), Type.Missing).Value2 == null)
                        break;

                }



                //A15-X15
                //向下读取到
                //A??-X??

                //再次初始化indexCount
                indexCount = 0;
                int index = 0;
                //string[] STRING24 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X" };

                while (ws表设计.get_Range("A" + (15 + indexCount).ToString(), Type.Missing).Value2 !=null)
                {


                    indexCount++;
                    if (ws表设计.get_Range("A" + (15 + indexCount).ToString(), Type.Missing).Value2 == null)
                        break;

                }

                //A15-X15
                //向下读取到
                //A??-X??     得到??的值 
                index = 15 + indexCount - 1;

                Excel.Range range = ws表设计.get_Range("A15", "AA" + index.ToString());

                Object[,] saRet;

                saRet = (System.Object[,])range.get_Value(Type.Missing);


                DataTable dt = new DataTable();
                DataRow dr;
                for (int i = 0; i < saRet.GetLength(1); i++)
                {
                    dt.Columns.Add(i.ToString());
                }
                for (int ii = 1; ii <= saRet.GetLength(0); ii++)
                {
                    dr = dt.NewRow();
                    for (int j = 1; j <= saRet.GetLength(1); j++)
                    {
                        dr[j-1] = saRet[ii, j];
                    }
                    dt.Rows.Add(dr);
                }

                string LS_SQL = "";

                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    LS_SQL += "INSERT INTO G_BASE_IMP_FORMXLS " +
                                   "(SHEET           ,CLASSID           ,CLASS           ,IMP_STEP_FLAG           ,ITEMNO" +
                                   ",NAME           ,MINORDER           ,MIN_DESCRIPTION      ,DESCRIPTION        ,KEY_WORDS" +
                                   ",S1           ,S2           ,S3           ,S4           ,S5" +
                                   ",S6           ,S7           ,S8           ,S9           ,S10" +
                                   ",S11           ,S12           ,S13           ,S14           ,S15" +
                                   ",N1           ,N2           ,N3           ,N4           ,N5,PIC" +
                                   ",REMARK           ,CREATE_BY           ,STATUS) VALUES ( " +
                                   "	'" + ws表设计.Name.ToString() + "' ,null,null,0,	" +
                                    "	'" + dt.Rows[x][0].ToString().Trim().Replace("'", "''").Trim() + "'," +//.Replace("<", "&lt;").Replace(">", "&gt;")

                                    "	'" + dt.Rows[x][1].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][2].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][3].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][4].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][5].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][6].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][7].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][8].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][9].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][10].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][11].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][12].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][13].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][14].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][15].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][16].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][17].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][18].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][19].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][20].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][21].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][22].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][23].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][24].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][25].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][26].ToString().Trim().Replace("'", "''").Trim() + "',   null,0,1 ); ";
                }


                                    LS_SQL += "INSERT INTO G_BASE_IMP_FORMXLS_FACT" +
                                    "(SHEET,FACT_NAME,FACT_URL" +
                                    ",CONN1,CONN2,CONN3,CONN4,CONN5,TERM" +
                                    ",REMARK,CREATE_BY,STATUS) VALUES ( " +
                                     "	'" + ws表设计.Name.ToString() + "' ," +
                                     "	'" + ls_工厂名.Replace("'", "''").Trim().Replace("<", "&lt;").Replace(">", "&gt;") + "'," +
                                     "	'" + ls_网址.Replace("'", "''").Trim().Replace("<", "&lt;").Replace(">", "&gt;") + "'," +
                                     "	'" + ls_联系信息1.Replace("'", "''").Trim().Replace("<", "&lt;").Replace(">", "&gt;") + "'," +
                                     "	'" + ls_联系信息2.Replace("'", "''").Trim().Replace("<", "&lt;").Replace(">", "&gt;") + "'," +
                                     "	'" + ls_联系信息3.Replace("'", "''").Trim().Replace("<", "&lt;").Replace(">", "&gt;") + "'," +
                                     "	'" + ls_联系信息4.Replace("'", "''").Trim().Replace("<", "&lt;").Replace(">", "&gt;") + "'," +
                                     "	'" + ls_联系信息5.Replace("'", "''").Trim().Replace("<", "&lt;").Replace(">", "&gt;") + "'," +
                                     "	'" + ls_条款.Replace("'", "''").Trim().Replace("<", "&lt;").Replace(">", "&gt;") + "',null,0,1 );";

                return LS_SQL;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return string.Empty;
            }
            finally
            {
                ws表设计 = null;
                workBook = null;
                app.Quit();
                app = null;
                System.GC.Collect();
            }
        }

        
        public static string 把商品导入数据库(string URL)
        {
            Excel.Application app = new Excel.ApplicationClass();
            Excel.Workbook workBook = app.Workbooks.Open(URL, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Excel.Worksheet ws表设计 = (Excel.Worksheet)workBook.Sheets[1];

            try
            {
                ws表设计.Select(Type.Missing);
                //int indexCount = 0;
                string SqlStr = string.Empty;

                TMS.Framework.Business.DataAccess.DBUtil dbs = new TMS.Framework.Business.DataAccess.DBUtil();

                

                //Excel.Range range = ws表设计.get_Range("A1", "BF15830");

                //从a1到bf28830
                Excel.Range range = ws表设计.get_Range("A15831", "BF28830");

                Object[,] saRet;

                saRet = (System.Object[,])range.get_Value(Type.Missing);


                DataTable dt = new DataTable();
                DataRow dr;
                for (int i = 0; i < saRet.GetLength(1); i++)
                {
                    dt.Columns.Add(i.ToString());
                }
                for (int ii = 1; ii <= saRet.GetLength(0); ii++)
                {
                    dr = dt.NewRow();
                    for (int j = 1; j <= saRet.GetLength(1); j++)
                    {
                        dr[j - 1] = saRet[ii, j];
                    }
                    dt.Rows.Add(dr);
                }

                string LS_SQL = "";

                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    LS_SQL = "INSERT INTO  REGNOW ( "+
                                    " ProductID,ProductName,VendorID,VendorName,VendorSupportEmail "+
                                    " ,VendorHomepageURL,USDPrice,EuroPrice,GBPPrice,AUDPrice "+
                                    " ,CADPrice,CategoryID,CategoryName,ShortDesc,LongDesc "+
                                    " ,TrialURL,DirectPurchaseURL,Platform1,Platform2,Boxshot "+
                                    " ,Screenshot,Icon,Banner125x125,Banner468x60,Banner120x90 "+
                                    " ,Banner728x90,Banner300x250,Banner392x72,Banner234x60,Banner120x240 "+
                                    " ,Banner120x60,Banner88x31,OtherImg1,OtherImg2,OtherImg3 "+
                                    " ,TextLink1,TextLink2,TextLink3,PromoText,EncodingCharSet "+
                                    " ,Commission,Add_Date,FileSize,VendorContactEmail,AllowAffiliates "+
                                    " ,Disabled,CertificationStatus,ShortDesc80,ShortDesc250,ShortDesc450 "+
                                    " ,MedDesc,OperatingSystems,Keywords,SystemRequirements,Update_Date " +
                                    " ,SoftwareVersion,RelationshipStatus,AutoApproved) VALUES ( " +
                                    "	'" + dt.Rows[x][0].ToString().Trim().Replace("'", "''").Trim() + "'," +//.Replace("<", "&lt;").Replace(">", "&gt;")
                                    "	'" + dt.Rows[x][1].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][2].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][3].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][4].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][5].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][6].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][7].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][8].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][9].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][10].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][11].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][12].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][13].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][14].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][15].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][16].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][17].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][18].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][19].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][20].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][21].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][22].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][23].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][24].ToString().Trim().Replace("'", "''").Trim() + "'," +


                                    "	'" + dt.Rows[x][25].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][26].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][27].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][28].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][29].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][30].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][31].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][32].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][33].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][34].ToString().Trim().Replace("'", "''").Trim() + "'," +


                                    "	'" + dt.Rows[x][35].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][36].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][37].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][38].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][39].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][40].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][41].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][42].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][43].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][44].ToString().Trim().Replace("'", "''").Trim() + "'," +


                                    "	'" + dt.Rows[x][45].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][46].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][47].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][48].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][49].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][50].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][51].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][52].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][53].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][54].ToString().Trim().Replace("'", "''").Trim() + "'," +

                                    "	'" + dt.Rows[x][55].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][56].ToString().Trim().Replace("'", "''").Trim() + "'," +
                                    "	'" + dt.Rows[x][57].ToString().Trim().Replace("'", "''").Trim() + "'       ); ";


                    dbs.ExecuteSQL(LS_SQL);



                }


                return LS_SQL;



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return string.Empty;
            }
            finally
            {
                ws表设计 = null;
                workBook = null;
                app.Quit();
                app = null;
                System.GC.Collect();
            }
        }



        public static  DataTable ConvertToDataTable(object[,] arr)   
       {   
  
           DataTable dataSouce = new DataTable();   
           for (int i = 0; i < arr.GetLength(1); i++)   
           {   
               DataColumn newColumn = new DataColumn(i.ToString(), arr[0, 0].GetType());   
               dataSouce.Columns.Add(newColumn);   
           }   
           for (int i = 0; i < arr.GetLength(0); i++)   
           {   
               DataRow newRow = dataSouce.NewRow();   
               for (int j = 0; j < arr.GetLength(1); j++)   
              {   
                   newRow[j.ToString()] = arr[i, j];   
              }   
              dataSouce.Rows.Add(newRow);   
          }   
          return dataSouce;   
 
      }




        public static string 把概要设计的数据导入数据库(string URL)
        {
            //ArrayList fkAlist = new ArrayList();
            //ArrayList tAlist = new ArrayList();//用于表更新排序
            Excel.Application app = new Excel.ApplicationClass();

            //打开模板文件，得到WorkBook对象
            Excel.Workbook workBook = app.Workbooks.Open(URL, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            Excel.Worksheet ws表设计 = (Excel.Worksheet)workBook.Sheets[4];

            try
            {               

                ws表设计.Select(Type.Missing);
                //app.Visible = true;

                int indexCount = 0;//计数

                string SqlStr = string.Empty;//SQL

                //string indexStr = string.Empty;

                //string FKStr = string.Empty;

                //Excel.Range range = ws表设计.get_Range("I" + (4 + indexCount).ToString(), Type.Missing);
                //Excel.Range range1 = ws表设计.get_Range("H" + (4 + indexCount).ToString(), Type.Missing);

                while (ws表设计.get_Range("I" + (4 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim().Length > 0 && ws表设计.get_Range("H" + (4 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim() == "表编码：")
                {
                    SqlStr += "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'U')) DROP TABLE [dbo].[{0}]";
                    string tepTabStr = ws表设计.get_Range("I" + (4 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim();
                    string tepTabNameStr = ws表设计.get_Range("D" + (4 + indexCount).ToString(), Type.Missing).Value2.ToString().Replace("表名称：", "").Trim();
                    string tepStr = string.Empty;
                    string tepkeyStr = string.Empty;
                    SqlStr += "CREATE TABLE [dbo].[{0}]( ";

                    while (ws表设计.get_Range("B" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim().Length != 0)
                    {
                        if (ws表设计.get_Range("F" + (8 + indexCount).ToString(), Type.Missing).Value2 != null)
                        {
                            if (ws表设计.get_Range("F" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim().ToUpper() == "PK")
                            {
                                tepkeyStr = ws表设计.get_Range("C" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString();
                            }
                        }

                        ////FK
                        //if (ws表设计.get_Range("G" + (8 + indexCount).ToString(), Type.Missing).Value2 != null)
                        //{
                        //    string[] TEP_FK_STR = ws表设计.get_Range("G" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim().Split(',');
                        //    FKStr += @" ALTER TABLE {0} ADD CONSTRAINT {4} FOREIGN KEY({1})  REFERENCES {2}({3}) ;";
                        //    FKStr = string.Format(FKStr, tepTabStr, ws表设计.get_Range("C" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString(), TEP_FK_STR[1].ToString(), TEP_FK_STR[2].ToString(), TEP_FK_STR[0].ToString());
                        //    string[] fkstr = new string[2] { tepTabStr, TEP_FK_STR[1] };
                        //    fkAlist.Add(fkstr);
                        //}

                        ////INDEX
                        //if (ws表设计.get_Range("L" + (8 + indexCount).ToString(), Type.Missing).Value2 != null)
                        //{
                        //    string TEP_INDEX_STR = ws表设计.get_Range("L" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim();
                        //    if (TEP_INDEX_STR.Substring(0,3) != "PK_")
                        //    {
                        //        indexStr += @" create index {2} on {1} ( {0} ASC ) ;";
                        //        indexStr = string.Format(indexStr, ws表设计.get_Range("C" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString(), tepTabStr,TEP_INDEX_STR );
                        //    }                            
                        //}

                        string bolStr = 获取SQL表达式(indexCount, ws表设计);

                        if (bolStr == "CREATE_DATE datetime null, ")
                        { 
                            bolStr ="CREATE_DATE datetime null DEFAULT (getdate()), ";
                        }

                        if (bolStr == "LAST_UPDATED_DATE datetime null, ")
                        {
                            bolStr ="LAST_UPDATED_DATE datetime null DEFAULT (getdate()), ";
                        }

                        if (bolStr == "STATUS nvarchar(1) COLLATE Chinese_PRC_CI_AS NULL , ")
                        {
                            bolStr = "STATUS nvarchar(1) COLLATE Chinese_PRC_CI_AS NULL DEFAULT ((1)), ";
                        }



                        if (bolStr == string.Empty)
                            return string.Empty ;
                        SqlStr += bolStr;
                        tepStr += 获取扩展属性(indexCount, ws表设计);
                        indexCount++;
                        if (ws表设计.get_Range("B" + (8 + indexCount).ToString(), Type.Missing).Value2 == null)
                            break;
                    }
                    SqlStr += "constraint PK_{0} primary key ({1}))  ON [PRIMARY]    ";
                    SqlStr += tepStr;
                    SqlStr += "EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{2}' ,@level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{0}'  ";

                    //"constraint PK_T_KNOWLEDGE_DISCUSS_TYPE primary key (KNOWLEDGE_DISCUSS_TYPE_ID"
                    SqlStr = string.Format(SqlStr, tepTabStr, tepkeyStr, tepTabNameStr);
                    //string[] tStr = new string[2] { SqlStr, tepTabStr };
                    //tAlist.Add(tStr);
                    //SqlStr = string.Empty;
                    SqlStr += "\r\n";
                    indexCount += 7;
                    if (ws表设计.get_Range("I" + (4 + indexCount).ToString(), Type.Missing).Value2 == null)
                        break;







                }
                //tAlist = 排列表更新顺序(tAlist, fkAlist);

                //for (int i = 0; i < tAlist.Count; i++)
                //{
                //    SqlStr +=" "+ ((string[])tAlist[i])[1].ToString();
                //}
                    //ws表设计 = null;
                    //workBook.Close(Type.Missing );
                    //app = null; 
                return SqlStr;// + indexStr + FKStr;
                //return FKStr;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return string.Empty;
            }
            finally
            {
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(ws表设计);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(workBook);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                ws表设计 = null;
                workBook = null;
                app.Quit();
                app = null;
                System.GC.Collect();
            }


            //app.Visible = true;
        }

        private static ArrayList 排列表更新顺序(ArrayList tAlist,ArrayList fkAlist)
        {
            for (int i = 0; i < fkAlist.Count; i++)
            {
                string t_name = ((string[])fkAlist[i])[0].ToString();
                string r_name = ((string[])fkAlist[i])[1].ToString();
                tAlist = 更改顺序(tAlist, t_name, r_name, fkAlist);
            }
            return tAlist;
        }

        private static ArrayList 更改顺序(ArrayList tAlist,string t_name,string r_name,ArrayList fkAlist)
        {
            int tIndex = 0;
            int rIndex = 0;
            for (int i = 0; i < tAlist.Count; i++)
            {
                if (((string[])tAlist[i])[1].ToString() == t_name)
                    tIndex = i;
                if (((string[])tAlist[i])[1].ToString() == r_name)
                    rIndex = i;
            }
            if (rIndex > tIndex)
            {
                string[] tStr = (string[])tAlist[rIndex];
                tAlist.RemoveAt(rIndex);
                tAlist.Insert(tIndex, tStr);
                string[] tepStr = 查找关联表的外键(fkAlist, r_name);
                if (r_name != null)
                {
                   tAlist= 更改顺序(tAlist, tepStr[0].ToString(), tepStr[1].ToString(), fkAlist);
                }
            }
            return tAlist;
        }

        private static string[] 查找关联表的外键(ArrayList fkAlist,string r_name)
        {
            string[] tStr = null;
            for (int i = 0; i < fkAlist.Count; i++)
            {
                if (((string[])fkAlist[i])[1].ToString() == r_name)
                    return (string[])fkAlist[i];
            }
            return null;
        }

        private static string 获取扩展属性(int indexCount, Excel.Worksheet ws表设计)
        {
            return "            EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'"+ws表设计.get_Range("B" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString()+"' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{0}', @level2type=N'COLUMN',@level2name=N'"+ws表设计.get_Range("C" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString()+"'     ";

        }

        private static string 获取SQL表达式(int indexCount, Excel.Worksheet ws表设计)
        {

           


                String STR = ws表设计.get_Range("C" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString() + " ";
 try
            {
                if (ws表设计.get_Range("D" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim().ToLower() == "bigint" && ws表设计.get_Range("F" + (8 + indexCount).ToString(), Type.Missing).Value2 != null)
                {
                    string f = "";
                    try
                    {
                        f = ws表设计.get_Range("A" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim();
                    }
                    catch
                    {
                        f = "Y";
                    }


                    if ((ws表设计.get_Range("F" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim() == "PK")
                        && (f == "Y"))
                    {
                        STR += " bigint  IDENTITY(50000,1) NOT NULL, ";
                    }
                    else
                    {
                        STR += "bigint NOT NULL,  ";
                    }
                }
                else
                {
                    switch (ws表设计.get_Range("D" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim().ToLower())
                    {
                        case "int":
                            if (ws表设计.get_Range("H" + (8 + indexCount).ToString(), Type.Missing).Value2 != null)
                            {
                                STR += "int null default " + ws表设计.get_Range("H" + (8 + indexCount).ToString(), Type.Missing).Value2 + ",  ";
                            }
                            else
                            {
                                STR += "int null,  ";
                            }
                            break;

                        case "bigint":
                            // STR += "bigint null,  ";
                            if (ws表设计.get_Range("H" + (8 + indexCount).ToString(), Type.Missing).Value2 != null)
                            {
                                STR += "bigint null default " + ws表设计.get_Range("H" + (8 + indexCount).ToString(), Type.Missing).Value2 + ",  ";
                            }
                            else
                            {
                                STR += "bigint null,  ";
                            }
                            break;

                        case "nvarchar":
                            if (ws表设计.get_Range("E" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim().ToUpper() == "MAX")
                            {
                                STR += "nvarchar(MAX) COLLATE Chinese_PRC_CI_AS NULL , ";
                            }
                            else
                            {
                                if (ws表设计.get_Range("H" + (8 + indexCount).ToString(), Type.Missing).Value2 != null)
                                {
                                    STR += "nvarchar(" + (Convert.ToInt32(ws表设计.get_Range("E" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim())).ToString() + ")  default" + ws表设计.get_Range("H" + (8 + indexCount).ToString(), Type.Missing).Value2 + ",  ";
                                }
                                else
                                {
                                    STR += "nvarchar(" + (Convert.ToInt32(ws表设计.get_Range("E" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim())).ToString() + ") ,  ";
                                }


                                // STR += "nvarchar(" + (Convert.ToInt32(ws表设计.get_Range("E" + (8 + indexCount).ToString(), Type.Missing).Value2.ToString().Trim())).ToString() + ") COLLATE Chinese_PRC_CI_AS NULL , ";
                            }
                            break;
                        case "datetime":
                            // STR += "datetime null, ";
                            if (ws表设计.get_Range("H" + (8 + indexCount).ToString(), Type.Missing).Value2 != null)
                            {
                                STR += "datetime null default " + ws表设计.get_Range("H" + (8 + indexCount).ToString(), Type.Missing).Value2 + ",  ";
                            }
                            else
                            {
                                STR += "datetime null,  ";
                            }

                            break;
                        case "text":
                            STR += "text COLLATE Chinese_PRC_CI_AS NULL, ";
                            break;
                        case "ntext":
                            STR += "ntext COLLATE Chinese_PRC_CI_AS NULL, ";
                            break;
                        case "decimal":
                            //STR += "[decimal](18, 3) NULL, ";
                            if (ws表设计.get_Range("H" + (8 + indexCount).ToString(), Type.Missing).Value2 != null)
                            {
                                STR += "[decimal](18, 3) null default " + ws表设计.get_Range("H" + (8 + indexCount).ToString(), Type.Missing).Value2 + ",  ";
                            }
                            else
                            {
                                STR += "[decimal](18, 3) null,  ";
                            }

                            break;
                        default:
                            if (MessageBox.Show("第" + (8 + indexCount).ToString() + "行类型不对!是否继续运行程序？", "MY PROJECT", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.Cancel)
                            {
                                return string.Empty;
                            }
                            break;
                    }
                }

            }

            catch
            {
                MessageBox.Show("Excel第"+indexCount.ToString()+"行出错");
            }


                return STR;

            
        }

        public static void 测试(string URL)
        {
            Excel.Application app = new Excel.ApplicationClass();

            //打开模板文件，得到WorkBook对象
            Excel.Workbook workBook = app.Workbooks.Open(URL, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            Excel.Worksheet ws表设计 = (Excel.Worksheet)workBook.Sheets[4];

            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(ws表设计);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(workBook);
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                ws表设计 = null;
                workBook = null;
                app.Quit();
                app = null;
                System.GC.Collect();
            }
        }
    }
}
