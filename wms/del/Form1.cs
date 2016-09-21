using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Collections;
//using C1.C1Excel;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

using System.Net;
using TMS.Framework.Business.DataAccess;
using TMS.Framework.Publics.Data;

using System.Web;
using System.Threading;
using System.ServiceProcess;
using System.Diagnostics;

using System.Collections.Specialized;


using System.Globalization;
using System.Security.Cryptography;
using System.Timers;

using System.IO.Compression;

using System.Linq;



namespace UniversalAnalyse
{
    public partial class Form1 : Form
    {

        DBUtil DB = new DBUtil();

        DataSet excelds = new DataSet();
        DataTable dtTableExtendedPropert;
        DataTable dtViewExtendedPropert;

        //获取所有表的字段信息
        DataTable FiledDt;

        public Form1()
        {
            InitializeComponent();

            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();

            OleDbConnection myConn = new OleDbConnection(connstr);

            myConn.Open();
            //获取所有表的字段信息
            OleDbDataAdapter adapter = new OleDbDataAdapter(MyEXCEL.获取所有字段信息的SQL语句(), myConn);
            FiledDt = new System.Data.DataTable();
            adapter.Fill(FiledDt);
        }

        private void button1_Click(object sender, EventArgs e)
        {


            OFDScript.Title = "打开(Open)";
            OFDScript.FileName = "";
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.Filter = "sql脚本文件(*.sql)|*.sql|文本文件(*.txt)|*.txt";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    StreamReader sr = new StreamReader(OFDScript.FileName, System.Text.Encoding.Default);
                    this.richTextBox1.Text = sr.ReadToEnd();
                }
                //dataGridView1.DataSource = AnalyseScript(richTextBox1.Text);
                AnalyseScript(richTextBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void AnalyseScript(string str)
        {
            //DataSet ScriptDS = new DataSet();
            string[] ScriptStr = str.Split('\n');
            for (int i = 0; i < ScriptStr.Length; i++)
            {
                if (ScriptStr[i].ToLower().IndexOf("create table") > -1)
                {
                    //string dropStr = ScriptStr[i].Substring(ScriptStr[i].ToLower().IndexOf("create table")+12, ScriptStr[i].ToLower().IndexOf("(")-12).Trim();
                    //ScriptStr[i] = "IF  EXISTS  (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[DBO].[" + dropStr + "]') AND TYPE IN (N'U')) DROP TABLE [DBO].[" + dropStr + "]  " + ScriptStr[i];
                    if (i + 2 < ScriptStr.Length)
                    {
                        while (ScriptStr[i + 2].ToLower().IndexOf("identity,") > 0)
                        {
                            if (ScriptStr[i + 2].ToLower().IndexOf("create table") > 0)
                            {
                                i++;
                                break;
                            }
                            ScriptStr[i + 2] = ScriptStr[i + 2].Replace("identity,", "not null,");
                            i++;
                        }
                    }
                }
            }

            String TepStr = @"OrderNo              int                  null,                                       
                                       ContexID             varchar(50)          null,
                                        Remark              varchar(200)        null,
                                       FCODE                int                  null,
                                       CREATE_BY            int                  null,
                                       CREATE_DATE          datetime             null DEFAULT (getdate()),
                                       LAST_UPDATE_BY       int                  null,
                                       LAST_UPDATE_DATE     datetime             null DEFAULT (getdate()),
                                       Status               varchar(1)           null DEFAULT ((1)) ,";

            String TepStr1 = @"execute sp_addextendedproperty 'MS_Description', 
                               '顺序ID',
                               'user', '', 'table', '{0}', 'column', 'OrderNo'
                            go

                            execute sp_addextendedproperty 'MS_Description', 
                               '上下文ID',
                               'user', '', 'table', '{0}', 'column', 'ContexID'
                            go

                            execute sp_addextendedproperty 'MS_Description', 
                               '备注',
                               'user', '', 'table', '{0}', 'column', 'Remark'
                            go

                            execute sp_addextendedproperty 'MS_Description', 
                               '组织ID',
                               'user', '', 'table', '{0}', 'column', 'FCODE'
                            go

                            execute sp_addextendedproperty 'MS_Description', 
                               '建立人',
                               'user', '', 'table', '{0}', 'column', 'CREATE_BY'
                            go

                            execute sp_addextendedproperty 'MS_Description', 
                               '建立日期',
                               'user', '', 'table', '{0}', 'column', 'CREATE_DATE'
                            go

                            execute sp_addextendedproperty 'MS_Description', 
                               '修改人',
                               'user', '', 'table', '{0}', 'column', 'LAST_UPDATE_BY'
                            go

                            execute sp_addextendedproperty 'MS_Description', 
                               '修改日期',
                               'user', '', 'table', '{0}', 'column', 'LAST_UPDATE_DATE'
                            go

                            execute sp_addextendedproperty 'MS_Description', 
                               '状态',
                               'user', '', 'table', '{0}', 'column', 'Status'
                            go";

            string strTab = string.Empty;

            for (int i = 0; i < ScriptStr.Length; i++)
            {
                if (ScriptStr[i].IndexOf("create table ") > -1)
                {
                    strTab = ScriptStr[i].Replace("create table ", "").Replace("(", "").Trim();
                }
                if (ScriptStr[i].IndexOf("constraint PK") > -1)
                {
                    ScriptStr[i] = TepStr + ScriptStr[i];
                    ScriptStr[i + 10] = string.Format(TepStr1, strTab) + ScriptStr[i + 10];
                }
                //if (i > 10)
                //{
                //    if (ScriptStr[i].IndexOf("create table ") > -1)
                //    {
                //        ScriptStr[i-4] = TepStr + ScriptStr[i-4];
                //    }
                //}
            }

            //for (int i = 0; i < ScriptStr.Length; i++)
            //{
            //    if (ScriptStr[i].ToLower().IndexOf("create table") > -1)
            //    {
            //        string dropStr = ScriptStr[i].Substring(ScriptStr[i].ToLower().IndexOf("create table") + 12, ScriptStr[i].ToLower().IndexOf("(") - 12).Trim();
            //        ScriptStr[i] = "IF  EXISTS  (SELECT * FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'[DBO].[" + dropStr + "]') AND TYPE IN (N'U')) DROP TABLE [DBO].[" + dropStr + "]  " + ScriptStr[i];
            //    }
            //}
            for (int i = 0; i < ScriptStr.Length; i++)
            {
                if (ScriptStr[i].ToLower().IndexOf("'user', '',") > -1)
                {
                    ScriptStr[i] = ScriptStr[i].Replace("'user', '',", "'user', 'dbo',");
                }
                if (ScriptStr[i].ToLower().IndexOf("user_name()") > -1)
                {
                    ScriptStr[i] = ScriptStr[i].Replace("user_name()", "'dbo'");
                }
                richTextBox2.Text += ScriptStr[i] + '\n'.ToString();
            }
            //return ScriptDS;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();

            OleDbConnection myConn = new OleDbConnection(connstr);


            if (tabControl1.SelectedIndex == 1)
            {
                MyDataSet myDataSetDif = new MyDataSet();
                myConn.Open();
                //
                string sqlstr =
                    "SELECT objtype, objname, name, value FROM ::fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', default, NULL, NULL) where objname<>'sysdiagrams' order by objname desc;";
                OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
                dtTableExtendedPropert = new System.Data.DataTable(); // 表的扩展属性,
                adapter.Fill(dtTableExtendedPropert);

                checkedListBox1.Items.Clear();

                ////获取所有表的字段信息
                //adapter = new OleDbDataAdapter(MyEXCEL.获取所有字段信息的SQL语句(), myConn);
                //FiledDt = new System.Data.DataTable();
                //adapter.Fill(FiledDt);

                foreach (DataRow dr in dtTableExtendedPropert.Rows)
                {
                    checkedListBox1.Items.Add(dr["value"].ToString() + "  " + dr["objname"].ToString());
                }
            }
            else if (tabControl1.SelectedIndex == 4)
            {
                //OleDbDataAdapter adapter=new OleDbDataAdapter(MyEXCEL.获取所有字段信息的SQL语句(), myConn);
                //FiledDt=new DataTable();
                //adapter.Fill(FiledDt );
                //checkedListBox2 
                string tableFlg = string.Empty;
                for (int i = 0; i < FiledDt.Rows.Count; i++)
                {
                    if (FiledDt.Rows[i][0].ToString().Trim().Length != 0)
                    {
                        if (tableFlg != FiledDt.Rows[i][0].ToString().Split('_')[1].ToString())
                        {
                            tableFlg = FiledDt.Rows[i][0].ToString().Split('_')[1].ToString();
                            checkedListBox3.Items.Add(tableFlg);
                        }
                        checkedListBox2.Items.Add(FiledDt.Rows[i][1].ToString() + "::" + FiledDt.Rows[i][0].ToString());
                    }
                }
            }
            else if (tabControl1.SelectedIndex == 6)
            {
                try
                {
                    this.Cursor = Cursors.AppStarting;
                    treeView2.Nodes.Clear();
                    MyDataSet myDataSetDif = new MyDataSet();
                    myConn.Open();
                    //
                    string sqlstr =
                        "SELECT objtype, objname, name, value FROM ::fn_listextendedproperty (NULL, 'schema', 'dbo', 'table', default, NULL, NULL) where objname<>'sysdiagrams' order by objname desc; SELECT objtype, objname, name, value FROM ::fn_listextendedproperty (NULL, 'schema', 'dbo', 'view', default, NULL, NULL) where objname<>'sysdiagrams' order by objname desc;";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dtTableExtendedPropert = ds.Tables[0]; // 表的扩展属性,
                    dtViewExtendedPropert = ds.Tables[1]; // 视图的扩展属性,

                    // 合成视图和表扩展属性
                    for (int i = 0; i < dtViewExtendedPropert.Rows.Count; i++)
                    {
                        DataRow dr = dtTableExtendedPropert.NewRow();
                        dr.ItemArray = dtViewExtendedPropert.Rows[i].ItemArray;

                        dtTableExtendedPropert.Rows.Add(dr);
                    }

                    string DBStr = configurationAppSettings.GetValue("DataBase", typeof(string)).ToString();

                    object[] restrictions = new object[] { DBStr, "dbo", null, "TABLE" };
                    DataTable dtTable = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions);

                    restrictions = new object[] { DBStr, "dbo", null, "VIEW" };
                    DataTable dtView = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions);

                    // 合成视图和表
                    for (int i = 0; i < dtView.Rows.Count; i++)
                    {
                        DataRow dr = dtTable.NewRow();
                        dr.ItemArray = dtView.Rows[i].ItemArray;

                        dtTable.Rows.Add(dr);
                    }

                    dtTable.Columns.Add("E_objname");

                    for (int i = 0; i < dtTable.Rows.Count; i++)
                    {
                        foreach (DataRow dr in dtTableExtendedPropert.Rows)
                        {
                            if (dr["objtype"].ToString() == dtTable.Rows[i][3].ToString() && dr["objname"].ToString() == dtTable.Rows[i][2].ToString() && dr["name"].ToString().ToUpper() == "MS_DESCRIPTION")
                            {
                                dtTable.Rows[i]["E_objname"] = dr["value"].ToString();
                                break;
                            }
                        }
                    }

                    //绑定TREEVIEW数据
                    TreeNode rtn = new TreeNode();
                    rtn.Text = DBStr;
                    treeView2.Nodes.Add(rtn);

                    restrictions = new object[] { DBStr, "dbo", null };
                    DataTable dtFK = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, restrictions);

                    for (int i = 0; i < dtTable.Rows.Count; i++)
                    {
                        TreeNode tn = new TreeNode();
                        tn.Text = dtTable.Rows[i][3].ToString() + ":" + dtTable.Rows[i][2].ToString() + ":" + dtTable.Rows[i]["E_objname"].ToString();

                        //添加列
                        DataTable dtColumnEP = 获取列扩展属性(dtTable.Rows[i][3].ToString().ToLower(), dtTable.Rows[i][2].ToString(), myConn);

                        if (dtTable.Rows[i][3].ToString().ToLower() == "view")
                        {
                            string v_name = dtTable.Rows[i][2].ToString();
                            //restrictions = new object[] { DBStr, "dbo", dtTable.Rows[i][2].ToString() };
                            //DataTable dttest = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.View_Column_Usage, restrictions);
                            sqlstr = "select VIEW_CATALOG,VIEW_SCHEMA,VIEW_NAME,TABLE_CATALOG,TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME " +
                                " FROM INFORMATION_SCHEMA.VIEW_COLUMN_USAGE WHERE VIEW_NAME='" + v_name + "' ORDER BY TABLE_NAME ; ";
                            adapter = new OleDbDataAdapter(sqlstr, myConn);
                            DataTable dtViewCol = new DataTable();
                            adapter.Fill(dtViewCol);
                            string tableName = string.Empty;
                            DataTable tepDT = null;

                            for (int index = 0; index < dtViewCol.Rows.Count; index++)
                            {
                                bool tepFlg = false;
                                for (int cIndex = 0; cIndex < dtColumnEP.Rows.Count; cIndex++)
                                {
                                    if (dtColumnEP.Rows[cIndex]["objname"].ToString() == dtViewCol.Rows[index]["COLUMN_NAME"].ToString())
                                    {
                                        tepFlg = true;
                                        break;
                                    }
                                }
                                if (!tepFlg)
                                {
                                    DataRow TepDr = 从表中查找视图里字段的扩展属性(dtViewCol.Rows[index]["COLUMN_NAME"].ToString(), ref  tepDT, tableName, dtViewCol.Rows[index]["TABLE_NAME"].ToString(), myConn);
                                    if (TepDr != null)
                                    {
                                        DataRow CEPdr = dtColumnEP.NewRow();
                                        CEPdr.ItemArray = TepDr.ItemArray;
                                        dtColumnEP.Rows.Add(CEPdr);
                                    }
                                }
                            }
                        }

                        restrictions = new object[] { DBStr, "dbo", dtTable.Rows[i][2].ToString() };
                        DataTable dtPK = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, restrictions);


                        restrictions = new object[] { DBStr, "dbo", null, null, dtTable.Rows[i][2].ToString() };
                        DataTable dtIndex = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.Indexes, restrictions);

                        restrictions = new object[] { DBStr, "dbo", dtTable.Rows[i][2].ToString(), null };
                        DataTable dtColumn = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, restrictions);

                        DataTable TNDt = INIT列结构();
                        //添加标题行
                        TreeNode ctn = new TreeNode();
                        ctn.Text = 添加标题行(TNDt, false); //dtTable.Rows[i][3].ToString().ToLower() == "view");
                        tn.Nodes.Add(ctn);

                        for (int j = 0; j < dtColumn.Rows.Count; j++)
                        {
                            string ctnStr = string.Empty;
                            DataRow drTN = TNDt.NewRow();
                            drTN = 添加行值(dtColumnEP, dtPK, dtFK, dtIndex, dtColumn.Rows[j], drTN, myConn);
                            TNDt.Rows.Add(drTN);

                            ctn = new TreeNode();
                            ctn.Text = 添加内容行(TNDt, false);//dtTable.Rows[i][3].ToString().ToLower() == "view"); //dtColumn.Rows[j][3].ToString();
                            if (TNDt.Rows[TNDt.Rows.Count - 1][9].ToString() == "1")
                            {
                                ctn.ImageIndex = 1;
                                ctn.BackColor = System.Drawing.Color.Red;
                            }
                            if (TNDt.Rows[TNDt.Rows.Count - 1][7].ToString().Length > 0)
                                ctn.ImageIndex = 0;
                            tn.Nodes.Add(ctn);
                        }
                        rtn.Nodes.Add(tn);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }

            }
        }

        private string 取得字段类型(OleDbConnection myConn, string tabName, string colName)
        {
            string sqlstr = "select DATA_TYPE,ISNULL(CHARACTER_MAXIMUM_LENGTH,0) AS CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" + tabName + "' AND COLUMN_NAME='" + colName + "'; ";
            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["CHARACTER_MAXIMUM_LENGTH"].ToString() != "0")
                    return dt.Rows[0]["DATA_TYPE"].ToString() + "(" + dt.Rows[0]["CHARACTER_MAXIMUM_LENGTH"].ToString() + ")";
                else
                    return dt.Rows[0]["DATA_TYPE"].ToString();
            }
            return string.Empty;
        }

        private DataRow 添加行值(DataTable dtColumnEP, DataTable dtPK, DataTable dtFK, DataTable dtIndex, DataRow dr, DataRow drTN, OleDbConnection myConn)
        {
            //string cnStr = string.Empty;
            string colName = dr["COLUMN_NAME"].ToString();
            string lenStr = dr["CHARACTER_MAXIMUM_LENGTH"].ToString();
            String tabName = dr["TABLE_NAME"].ToString();
            //string DataType = 取得字段类型(myConn,tabName,colName );// dr["DATA_TYPE"].ToString();

            //DataRow drTN = dtTN.NewRow();
            drTN[0] = tabName;
            drTN[2] = colName;
            drTN[3] = 取得字段类型(myConn, tabName, colName);
            drTN[4] = dr["IS_NULLABLE"].ToString();
            drTN[6] = dr["COLUMN_DEFAULT"].ToString();
            foreach (DataRow tepDr in dtColumnEP.Rows)
            {
                if (tepDr["objname"].ToString() == colName && tepDr["name"].ToString().ToUpper() == "MS_DESCRIPTION")
                {
                    drTN[1] = tepDr["value"].ToString();
                    break;
                }
            }
            foreach (DataRow tepDr in dtPK.Rows)
            {
                if (tepDr["COLUMN_NAME"].ToString() == colName && tepDr["TABLE_NAME"].ToString() == tabName)
                {
                    drTN[9] = "1";
                }
            }
            foreach (DataRow tepDr in dtFK.Rows)
            {
                if (tepDr["FK_COLUMN_NAME"].ToString() == colName && tepDr["FK_TABLE_NAME"].ToString() == tabName)
                {
                    drTN[8] = tepDr["PK_COLUMN_NAME"].ToString();
                    drTN[7] = tepDr["PK_TABLE_NAME"].ToString();
                    break;
                }
            }
            foreach (DataRow tepDr in dtIndex.Rows)
            {
                if (tepDr["COLUMN_NAME"].ToString() == colName && tepDr["TABLE_NAME"].ToString() == tabName)
                {
                    drTN[5] = "√";
                    break;
                }
            }

            //dtTN.Rows.Add(drTN);
            return drTN;
        }

        private string 添加内容行(DataTable dt, bool flg)
        {
            string tepStr = string.Empty;
            if (flg)
            {
                for (int i = 0; i < dt.Columns.Count - 1; i++)
                {
                    tepStr += 设置宽度(dt.Rows[dt.Rows.Count - 1][i].ToString(), dt.Columns[i].MaxLength);
                }
            }
            else
            {
                for (int i = 1; i < dt.Columns.Count - 1; i++)
                {
                    tepStr += 设置宽度(dt.Rows[dt.Rows.Count - 1][i].ToString(), dt.Columns[i].MaxLength);
                }
            }
            return tepStr;
        }

        private string 添加标题行(DataTable dt, bool flg)
        {
            string tepStr = string.Empty;
            if (flg)
            {
                for (int i = 0; i < dt.Columns.Count - 1; i++)
                {
                    tepStr += 设置宽度(dt.Columns[i].ColumnName, dt.Columns[i].MaxLength);
                }
            }
            else
            {
                for (int i = 1; i < dt.Columns.Count - 1; i++)
                {
                    tepStr += 设置宽度(dt.Columns[i].ColumnName, dt.Columns[i].MaxLength);
                }
            }
            return tepStr;
        }

        private string 设置宽度(string colText, int colMaxLen)
        {
            byte[] byteFactNum = Encoding.Default.GetBytes(colText);
            for (int i = 0; i < colMaxLen - byteFactNum.Length; i++)
            {
                colText += " ";
            }
            return colText;
        }

        private DataTable INIT列结构()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("表名");//using view
            dt.Columns[0].MaxLength = 30;
            //
            dt.Columns.Add("列说明");
            dt.Columns[1].MaxLength = 30;
            dt.Columns.Add("列名称");
            dt.Columns[2].MaxLength = 30;
            dt.Columns.Add("字段类型");
            dt.Columns[3].MaxLength = 20;
            dt.Columns.Add("是否为空");
            dt.Columns[4].MaxLength = 12;
            dt.Columns.Add("索引");
            dt.Columns[5].MaxLength = 10;
            dt.Columns.Add("默认值");
            dt.Columns[6].MaxLength = 20;
            dt.Columns.Add("外键关联表");
            dt.Columns[7].MaxLength = 30;
            dt.Columns.Add("外键关联字段");
            dt.Columns[8].MaxLength = 30;

            dt.Columns.Add("isKey");
            dt.Columns[9].DefaultValue = "0";
            return dt;
        }

        private DataRow 从表中查找视图里字段的扩展属性(string ColumnName, ref DataTable tepDT, string tableName, string ViewCol_tableName, OleDbConnection myConn)
        {

            if (tableName != ViewCol_tableName)
            {
                tableName = ViewCol_tableName;
                tepDT = 获取列扩展属性("table", tableName, myConn);
            }

            for (int i = 0; i < tepDT.Rows.Count; i++)
            {
                if (tepDT.Rows[i]["objname"].ToString() == ColumnName)
                {
                    return tepDT.Rows[i];
                }
            }
            return null;
        }

        private DataTable 获取列扩展属性(string TypeStr, string NameStr, OleDbConnection myConn)
        {
            string sqlstr =
                    "SELECT objtype, objname, name, value FROM fn_listextendedproperty (NULL, 'schema', 'dbo', '" +
                    TypeStr + "', '" +
                    NameStr + "', 'column', NULL) ;";
            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataTable dtColumnEP = new DataTable(); // 列的扩展属性
            adapter.Fill(dtColumnEP);
            return dtColumnEP;
        }

        /// <summary>
        /// 基础数据设计文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            ArrayList alist = new ArrayList();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                    alist.Add(checkedListBox1.GetItemText(checkedListBox1.Items[i]));
            }
            if (alist.Count > 0)
            {
                //读取EXCEL模板，并插入数据
                基础数据设计文档(alist);
            }
        }

        /// <summary>
        /// 读取EXCEL模板，并插入数据
        /// </summary>
        private void 基础数据设计文档(ArrayList alist)
        {

            OFDScript.Title = "数据库基础数据设计文档模板";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {

                    MyEXCEL.基础数据设计文档(alist, OFDScript.FileName, FiledDt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }



            //C1XLBook c1XLBookTemp = new C1XLBook();
            //c1XLBookTemp.Clear();
            //c1XLBookTemp.Sheets.RemoveAt(0);

            //c1XLBook1.Load("CRM10-0802_基础数据说明书.xls");

            //导出原始Sheet(c1XLBookTemp, c1XLBook1, "封面", 47, 35);
            //导出原始Sheet(c1XLBookTemp, c1XLBook1, "修订", 18, 4);
            //导出原始Sheet(c1XLBookTemp, c1XLBook1, "目录", 3, 7);

            ////结构序号	实例序号	内容编号	对应功能点	内容名称	复用引用	内容描述
            //DataTable dt目录 = new DataTable();
            //dt目录.Columns.Add("结构序号", typeof(string));
            //dt目录.Columns.Add("实例序号", typeof(string));
            //dt目录.Columns.Add("内容编号", typeof(string));
            //dt目录.Columns.Add("对应功能点", typeof(string));
            //dt目录.Columns.Add("内容名称", typeof(string));
            //dt目录.Columns.Add("复用引用", typeof(string));
            //dt目录.Columns.Add("内容描述", typeof(string));

            //XLSheet xlSheet目录 = c1XLBookTemp.Sheets["目录"];
            //for (int i = 2; i < xlSheet目录.Rows.Count; i++)
            //{
            //    DataRow dr = dt目录.NewRow();

            //    for (int j = 0; j < xlSheet目录.Columns.Count; j++)
            //    {
            //        XLCell xlCell = xlSheet目录.GetCell(i, j);
            //        dr[j] = xlCell.Value;
            //    }

            //    dt目录.Rows.Add(dr);
            //}

            //c1XLBookTemp.Save(@"CRM10-0802_基础数据说明书_V1.0.1.xls");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            excelds.Clear();

            OFDScript.Title = "选择外部设计文档资料";
            OFDScript.FileName = "";
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    String strConn = "Provider=Microsoft.Jet.OleDb.4.0; Data Source=" + OFDScript.FileName + "; Extended Properties=Excel 8.0;";

                    //调用方法如下：
                    DataTable excelDt = GetAllTableInfo(strConn);
                    if (excelDt != null && excelDt.Rows.Count > 0)
                    {
                        for (int j = 0; j < excelDt.Rows.Count; j++)
                        {
                            if (excelDt.Rows[j][2].ToString().IndexOf("_") == -1)
                            {
                                string sql = "select * from [" + excelDt.Rows[j][2].ToString() + "]";
                                DataSet infods = getExcelInfoDs(strConn, sql);
                                for (int m = 0; m < infods.Tables.Count; m++)
                                {
                                    if (infods.Tables[m] != null)
                                    {
                                        excelds.Tables.Add(infods.Tables[m].Copy());
                                    }
                                }
                            }
                        }
                        dataGridView1.DataSource = excelds.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            ////Regex.Split( str , "\r\n" )
            //MessageBox.Show(Regex.Split( excelds.Tables[0].Rows[2]["F1"].ToString(),"\n")[0].ToString());
            //MessageBox.Show(Regex.Split(excelds.Tables[0].Rows[2]["F1"].ToString(), "\n")[1].ToString());
        }

        /// <summary>
        /// 生成功能外部设计文档
        /// </summary>
        /// <param name="dt"></param>
        private void 功能外部设计文档(DataTable dt)
        {
            OFDScript.Title = "选择外部设计文档模板";
            OFDScript.FileName = "";
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    MyEXCEL.功能外部设计文档(excelds.Tables[0], OFDScript.FileName, FiledDt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }


        //如下给出的是c#读取excel
        //查询excel，得到worksheet，即类似数据库中的table
        public static DataTable GetAllTableInfo(string connstring)
        {
            OleDbConnection conn = new OleDbConnection(connstring);
            try
            {
                conn.Open();
                DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                return schemaTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return null;
            }
        }

        ///查询excel，返回DataSet
        public static DataSet getExcelInfoDs(string strConn, string sqlstring)
        {
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            OleDbDataAdapter adp = new OleDbDataAdapter(sqlstring, conn);
            DataSet ds = new DataSet();
            try
            {
                adp.Fill(ds);
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return null;
            }
            return ds;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataTable dt = excelds.Tables[0];
            //dt.Rows.Remove(excelds.Tables[0].Rows[0]);
            //dt.Rows.Remove(excelds.Tables[0].Rows[1]);
            功能外部设计文档(dt);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //Intercept_Map obj = new Intercept_Map();
            // obj.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OFDScript.Title = "数据库概要设计文档模板";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {

                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    MyEXCEL.数据库概要设计文档(dtTableExtendedPropert, OFDScript.FileName, 概要设计字段DT());//FiledDt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private DataTable 概要设计字段DT()
        {
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();

            OleDbConnection myConn = new OleDbConnection(connstr);

            OleDbCommand cmd = new OleDbCommand(MyEXCEL.获取所有FK(), myConn);
            OleDbDataAdapter oda = new OleDbDataAdapter(cmd);
            DataTable FKDt = new DataTable();
            oda.Fill(FKDt);

            cmd = new OleDbCommand(MyEXCEL.获取所有索引(), myConn);
            oda = new OleDbDataAdapter(cmd);
            DataTable indexDt = new DataTable();
            oda.Fill(indexDt);

            DataTable TepDt = FiledDt.Copy();
            TepDt.Columns.Add("fk_name");//fk名称
            TepDt.Columns.Add("fk_r_name");//关联表名
            TepDt.Columns.Add("fk_r_c_name");//关联字段

            string tabname = string.Empty;
            string colname = string.Empty;

            DataRow[] tepIndexDr = null;
            DataRow[] tepFkDr = null;

            for (int i = 0; i < TepDt.Rows.Count; i++)
            {
                colname = TepDt.Rows[i][3].ToString().Trim();
                if (TepDt.Rows[i][0].ToString().Trim() != "")
                {
                    tabname = TepDt.Rows[i][0].ToString();
                    tepIndexDr = indexDt.Select("i_t_name='" + tabname + "'");
                    tepFkDr = FKDt.Select("fk_t_name='" + tabname + "'");
                }
                if (tepIndexDr != null)
                {
                    if (tepIndexDr.Length > 0)
                    {
                        for (int indexi = 0; indexi < tepIndexDr.Length; indexi++)
                        {
                            if (tepIndexDr[indexi]["i_c_name"].ToString().Trim() == colname)
                            {
                                TepDt.Rows[i]["IndexName"] = tepIndexDr[indexi][0].ToString();
                            }
                        }
                    }
                }

                if (tepFkDr != null)
                {
                    if (tepFkDr.Length > 0)
                    {
                        for (int indexf = 0; indexf < tepFkDr.Length; indexf++)
                        {
                            if (tepFkDr[indexf]["fk_t_c_name"].ToString().Trim() == colname)
                            {
                                TepDt.Rows[i]["fk_name"] = tepFkDr[indexf][0].ToString();
                                TepDt.Rows[i]["fk_r_name"] = tepFkDr[indexf][2].ToString();
                                TepDt.Rows[i]["fk_r_c_name"] = tepFkDr[indexf][4].ToString();
                            }
                        }
                    }
                }
            }

            return TepDt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (FiledDt != null)
            {
                //配置GetDataById
                #region 获取配置文件
                //for (int i = 1; i < FiledDt.Rows.Count; i++)
                //{
                //    if (FiledDt.Rows[i][0].ToString() == "")
                //        FiledDt.Rows[i][0] = FiledDt.Rows[i - 1][0].ToString();
                //}

                //StringBuilder sb = new StringBuilder();
                //string tbName = string.Empty;
                //for (int i = 0; i < FiledDt.Rows.Count; i++)
                //{
                //    if (tbName != FiledDt.Rows[i][0].ToString())
                //    {
                //        string colName = string.Empty;
                //        ArrayList keyList = new ArrayList();
                //        string tbDesc = string.Empty;
                //        tbName = FiledDt.Rows[i][0].ToString();
                //        tbDesc = FiledDt.Rows[i][1].ToString();
                //        while (tbName == FiledDt.Rows[i][0].ToString())
                //        {
                //            if (FiledDt.Rows[i][5].ToString() == "√")
                //                keyList.Add(FiledDt.Rows[i][3].ToString());
                //            colName += "," + FiledDt.Rows[i][3].ToString();
                //            i++;
                //            if (i == FiledDt.Rows.Count)
                //                break;
                //        }
                //        string keyName = string.Empty;
                //        for (int j = 0; j < keyList.Count; j++)
                //        {
                //            keyName += " AND " + keyList[j].ToString() + " = #" + keyList[j].ToString() + "#";
                //        }

                //        sb.AppendLine("<!--" + tbDesc + "-->");
                //        sb.AppendLine(" <" + tbName + ">");
                //        sb.AppendLine("SELECT " + colName.Substring(1, colName.Length - 1) + " FROM " + tbName + " WHERE 1=1 " + keyName);
                //        sb.AppendLine(" </" + tbName + ">");
                //    }
                //    i--;
                //}
                #endregion

                #region 获取接口文件
                //StringBuilder sb = new StringBuilder();
                //foreach (DataRow dr in FiledDt.Rows)
                //{
                //    if (dr[0].ToString() != "")
                //    {
                //        string tbName = dr[0].ToString();
                //        string colName = dr[3].ToString();
                //        string tbDesc = dr[1].ToString();
                //        string colDesc = dr[13].ToString();
                //        tbName = tbName.Substring(2, tbName.Length - 2);
                //        string[] sStrs = tbName.Split('_');
                //        tbName = string.Empty;
                //        for (int i = 0; i < sStrs.Length; i++)
                //        {
                //            tbName += sStrs[i].Substring(0, 1).ToUpper() + sStrs[i].Substring(1, sStrs[i].Length - 1).ToLower();
                //        }
                //        sStrs = colName.Split('_');
                //        colName = string.Empty;
                //        for (int i = 0; i < sStrs.Length; i++)
                //        {
                //            if (i == 0)
                //            {
                //                colName += sStrs[i].ToLower();
                //            }
                //            else
                //            {
                //                colName += sStrs[i].Substring(0, 1).ToUpper() + sStrs[i].Substring(1, sStrs[i].Length - 1).ToLower();
                //            }
                //        }
                //        sb.AppendLine("/// <summary>");
                //        sb.AppendLine("/// 类型：方法");
                //        sb.AppendLine("/// 方法名称：获取" + tbDesc + "的信息");
                //        sb.AppendLine("/// 方法说明：根据" + colDesc + "获取" + tbDesc + "的信息");
                //        sb.AppendLine("/// </summary>");
                //        sb.AppendLine("/// <param name=\"" + colName + "\">" + colDesc + "</param>");
                //        sb.AppendLine("/// <returns></returns>");
                //        sb.AppendLine("DataTable Get" + tbName + "DataById(string " + colName + ");");
                //        sb.AppendLine("");
                //    }
                //}
                #endregion

                #region 获取服务文件
                //StringBuilder sb = new StringBuilder();
                //foreach (DataRow dr in FiledDt.Rows)
                //{
                //    if (dr[0].ToString() != "")
                //    {
                //        string tbName = dr[0].ToString();
                //        string xmlNodeName = dr[0].ToString();
                //        string colName = dr[3].ToString();
                //        string tbDesc = dr[1].ToString();
                //        string colDesc = dr[13].ToString();
                //        tbName = tbName.Substring(2, tbName.Length - 2);
                //        string[] sStrs = tbName.Split('_');
                //        tbName = string.Empty;
                //        for (int i = 0; i < sStrs.Length; i++)
                //        {
                //            tbName += sStrs[i].Substring(0, 1).ToUpper() + sStrs[i].Substring(1, sStrs[i].Length - 1).ToLower();
                //        }
                //        sStrs = colName.Split('_');
                //        string paramName = string.Empty;
                //        for (int i = 0; i < sStrs.Length; i++)
                //        {
                //            if (i == 0)
                //            {
                //                paramName += sStrs[i].ToLower();
                //            }
                //            else
                //            {
                //                paramName += sStrs[i].Substring(0, 1).ToUpper() + sStrs[i].Substring(1, sStrs[i].Length - 1).ToLower();
                //            }
                //        }
                //        sb.AppendLine("");
                //        sb.AppendLine("/// <summary>");
                //        sb.AppendLine("/// 类型：方法");
                //        sb.AppendLine("/// 方法名称：获取" + tbDesc + "的信息");
                //        sb.AppendLine("/// 方法说明：根据" + colDesc + "获取" + tbDesc + "的的信息");
                //        sb.AppendLine("/// </summary>");
                //        sb.AppendLine("/// <param name=\"" + paramName + "\">" + colDesc + "</param>");
                //        sb.AppendLine("/// <returns></returns>");
                //        sb.AppendLine("public DataTable Get" + tbName + "DataById(string " + paramName + ")");
                //        sb.AppendLine("{");
                //        sb.AppendLine("Dictionary<string, string> dicKeyValue = new Dictionary<string, string>();");
                //        sb.AppendLine("dicKeyValue.Add(\"#" + colName + "#\", " + paramName + ");");
                //        sb.AppendLine("IDataAccess dataAccess = ContainerContext.Container.Resolve<IDataAccess>();");
                //        sb.AppendLine("try");
                //        sb.AppendLine("{");
                //        sb.AppendLine("using (DbConnection con = dataAccess.GetCurrentConnection())");
                //        sb.AppendLine("{");
                //        sb.AppendLine("return dataAccess.QueryTableData(con, \"Portal/GetDataById/" + xmlNodeName + "\", dicKeyValue);");
                //        sb.AppendLine("}");
                //        sb.AppendLine("    }");
                //        sb.AppendLine("  catch (Exception ex)");
                //        sb.AppendLine("  {");
                //        sb.AppendLine("LogError(Operate.查看, \"根据" + colDesc + "获取" + tbDesc + "的信息失败!\", ex);");
                //        sb.AppendLine("throw new DataAccessException(ex.Message);");
                //        sb.AppendLine("}");
                //        sb.AppendLine("}");
                //    }
                //}

                #endregion

                #region 获取清单
                //StringBuilder sb = new StringBuilder();
                //foreach (DataRow dr in FiledDt.Rows)
                //{
                //    if (dr[0].ToString() != "")
                //    {
                //        string tbName = dr[0].ToString();
                //        string tbDesc = dr[1].ToString();
                //        string tbName1 = tbName.Substring(2, tbName.Length - 2);
                //        string[] sStrs = tbName1.Split('_');
                //        tbName1 = string.Empty;
                //        for (int i = 0; i < sStrs.Length; i++)
                //        {
                //            tbName1 += sStrs[i].Substring(0, 1).ToUpper() + sStrs[i].Substring(1, sStrs[i].Length - 1).ToLower();
                //        }
                //        sb.AppendLine(tbDesc + ",");
                //        sb.AppendLine(tbName + ",");
                //        sb.AppendLine("Get" + tbName1 + "DataById,");
                //        sb.AppendLine("");
                //    }
                //}
                #endregion

                //配置逻辑组件
                #region 从概要设计书到列表
                //OFDScript.Title = "打开(Open)";
                //OFDScript.FileName = "";
                ////为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
                //OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
                //OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
                //OFDScript.CheckFileExists = true;  //验证路径有效性
                //OFDScript.CheckPathExists = true; //验证文件有效性
                //try
                //{
                //    StringBuilder sb = new StringBuilder();
                //    sb.AppendLine("逻辑名称,逻辑说明,表名,表说明,字段名,字段说明,复合从属关系,表关键字段");
                //    if (OFDScript.ShowDialog() == DialogResult.OK)
                //    {
                //        String strConn = "Provider=Microsoft.Jet.OleDb.4.0; Data Source=" + OFDScript.FileName + "; Extended Properties=Excel 8.0;";
                //        string sql = "select * from [1.1、表设计说明$]";
                //        DataSet infods = getExcelInfoDs(strConn, sql);
                //        string sTabName = string.Empty;
                //        string sTabDesc = string.Empty;
                //        string sColName = string.Empty;
                //        string sColDesc = string.Empty;
                //        string sKeyColName = string.Empty;
                //        //StringBuilder sb = new StringBuilder();

                //        for (int i = 0; i < infods.Tables[0].Rows.Count; i++)
                //        {
                //            DataRow dr = infods.Tables[0].Rows[i];
                //            if (dr["F6"].ToString().Trim().ToUpper() == "PK")
                //                sKeyColName = dr["F3"].ToString().Trim();
                //            if (dr["F12"].ToString().Trim().ToUpper() == "X")
                //            {
                //                if (dr["F8"].ToString().Trim().ToUpper() == "表编码：")
                //                {
                //                    sTabName = dr["F9"].ToString().Trim();
                //                    sTabDesc = dr["F4"].ToString().Trim().Split('：')[1];
                //                }
                //                else
                //                {
                //                    sColName = dr["F3"].ToString().Trim();
                //                    sColDesc = dr["F2"].ToString().Trim();
                //                    sb.AppendLine("," + "," + sTabName + "," + sTabDesc + "," + sColName + "," + sColDesc + "," + sKeyColName);
                //                }
                //            }
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message.ToString());
                //}
                #endregion

                #region 从列表到配置文件

                //OFDScript.Title = "打开(Open)";
                //OFDScript.FileName = "";
                ////为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
                //OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
                //OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
                //OFDScript.CheckFileExists = true;  //验证路径有效性
                //OFDScript.CheckPathExists = true; //验证文件有效性

                //StringBuilder sb = new StringBuilder();
                //try
                //{
                //    if (OFDScript.ShowDialog() == DialogResult.OK)
                //    {
                //        String strConn = "Provider=Microsoft.Jet.OleDb.4.0; Data Source=" + OFDScript.FileName + "; Extended Properties=Excel 8.0;";
                //        string sql = "select * from [逻辑组件列表$]";
                //        DataSet infods = getExcelInfoDs(strConn, sql);

                //        for (int i = 1; i < FiledDt.Rows.Count; i++)
                //        {
                //            if (FiledDt.Rows[i][0].ToString() == "")
                //                FiledDt.Rows[i][0] = FiledDt.Rows[i - 1][0].ToString();
                //        }

                //        for (int i = 0; i < infods.Tables[0].Rows.Count; i++)
                //        {
                //            DataRow dr = infods.Tables[0].Rows[i];
                //            DataRow[] drs = FiledDt.Select("TableName='" + dr[2].ToString().Trim() + "' and ColumnName='" + dr[4].ToString().Trim() + "'");
                //            if (drs.Length > 0)
                //            {
                //                dr[6] = drs[0]["Type"].ToString();
                //            }
                //            drs = FiledDt.Select("TableName='" + dr[2].ToString().Trim() + "' and IDENTITY='√'");
                //            if (drs.Length > 0)
                //            {
                //                dr[7] = drs[0]["ColumnName"].ToString();
                //            }
                //        }

                //        string functionName = string.Empty;
                //        for (int i = 0; i < infods.Tables[0].Rows.Count; i++)
                //        {
                //            if (functionName != infods.Tables[0].Rows[i][0].ToString())
                //            {
                //                string colName = string.Empty;
                //                ArrayList keyList = new ArrayList();
                //                ArrayList keyTypeList = new ArrayList();
                //                string tbName = string.Empty;
                //                string tbDesc = string.Empty;
                //                string functionDesc = string.Empty;
                //                string sColKeyName = string.Empty;
                //                functionName = infods.Tables[0].Rows[i][0].ToString();
                //                functionDesc = infods.Tables[0].Rows[i][1].ToString();
                //                tbName = infods.Tables[0].Rows[i][2].ToString();
                //                tbDesc = infods.Tables[0].Rows[i][3].ToString();
                //                sColKeyName = infods.Tables[0].Rows[i][7].ToString();
                //                while (functionName == infods.Tables[0].Rows[i][0].ToString())
                //                {
                //                    //if (infods.Tables[0].Rows[i][6].ToString() == "1")
                //                    //    colName = infods.Tables[0].Rows[i][4].ToString();
                //                    keyList.Add(infods.Tables[0].Rows[i][4].ToString());
                //                    keyTypeList.Add(infods.Tables[0].Rows[i][6].ToString());
                //                    i++;
                //                    if (i == infods.Tables[0].Rows.Count)
                //                        break;
                //                }
                //                string keyName = string.Empty;
                //                for (int j = 0; j < keyList.Count; j++)
                //                {
                //                    if (keyTypeList[j].ToString().ToLower() == "int")
                //                        keyName += " AND " + keyList[j].ToString() + " = #" + keyList[j].ToString() + "#";
                //                    else
                //                        keyName += " AND " + keyList[j].ToString() + " = '#" + keyList[j].ToString() + "#'";
                //                }

                //                sb.AppendLine("<!--" + tbDesc + ":" + functionDesc + "-->");
                //                sb.AppendLine(" <" + functionName + ">");
                //                sb.AppendLine("<![CDATA[");
                //                sb.AppendLine("SELECT COUNT(" + sColKeyName + ") FROM " + tbName + " WHERE 1=1 " + keyName + " AND STATUS = '#STATUS#' AND " + sColKeyName + " <> #" + sColKeyName + "#");
                //                sb.AppendLine("]]>");
                //                sb.AppendLine(" </" + functionName + ">");
                //            }
                //            i--;
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}               
                #endregion

                #region 获取接口文件
                //OFDScript.Title = "打开(Open)";
                //OFDScript.FileName = "";
                ////为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
                //OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
                //OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
                //OFDScript.CheckFileExists = true;  //验证路径有效性
                //OFDScript.CheckPathExists = true; //验证文件有效性

                //StringBuilder sb = new StringBuilder();
                //try
                //{
                //    if (OFDScript.ShowDialog() == DialogResult.OK)
                //    {
                //        String strConn = "Provider=Microsoft.Jet.OleDb.4.0; Data Source=" + OFDScript.FileName + "; Extended Properties=Excel 8.0;";
                //        string sql = "select * from [逻辑组件列表$]";
                //        DataSet infods = getExcelInfoDs(strConn, sql);

                //        for (int i = 1; i < FiledDt.Rows.Count; i++)
                //        {
                //            if (FiledDt.Rows[i][0].ToString() == "")
                //                FiledDt.Rows[i][0] = FiledDt.Rows[i - 1][0].ToString();
                //        }

                //        for (int i = 0; i < infods.Tables[0].Rows.Count; i++)
                //        {
                //            DataRow dr = infods.Tables[0].Rows[i];
                //            DataRow[] drs = FiledDt.Select("TableName='" + dr[2].ToString().Trim() + "' and ColumnName='" + dr[4].ToString().Trim() + "'");
                //            if (drs.Length > 0)
                //            {
                //                dr[6] = drs[0]["Type"].ToString();
                //            }
                //            drs = FiledDt.Select("TableName='" + dr[2].ToString().Trim() + "' and IDENTITY='√'");
                //            if (drs.Length > 0)
                //            {
                //                dr[7] = drs[0]["ColumnName"].ToString();
                //            }
                //        }

                //        string functionName = string.Empty;
                //        for (int i = 0; i < infods.Tables[0].Rows.Count; i++)
                //        {
                //            if (functionName != infods.Tables[0].Rows[i][0].ToString())
                //            {
                //                string colName = string.Empty;
                //                ArrayList keyList = new ArrayList();
                //                ArrayList keyNameList = new ArrayList();
                //                string tbName = string.Empty;
                //                string tbDesc = string.Empty;
                //                string functionDesc = string.Empty;
                //                string sColKeyName = string.Empty;
                //                functionName = infods.Tables[0].Rows[i][0].ToString();
                //                functionDesc = infods.Tables[0].Rows[i][1].ToString();
                //                tbName = infods.Tables[0].Rows[i][2].ToString();
                //                tbDesc = infods.Tables[0].Rows[i][3].ToString();
                //                sColKeyName = infods.Tables[0].Rows[i][7].ToString();
                //                while (functionName == infods.Tables[0].Rows[i][0].ToString())
                //                {
                //                    keyList.Add(infods.Tables[0].Rows[i][4].ToString());
                //                    keyNameList.Add(infods.Tables[0].Rows[i][5].ToString());
                //                    i++;
                //                    if (i == infods.Tables[0].Rows.Count)
                //                        break;
                //                }
                //                string keyName = string.Empty;
                //                for (int j = 0; j < keyList.Count; j++)
                //                {
                //                    keyName += " AND " + keyList[j].ToString() + " = #" + keyList[j].ToString() + "#";
                //                }

                //                string keyDesc = string.Empty;
                //                for (int j = 0; j < keyNameList.Count; j++)
                //                {
                //                    keyDesc += "、" + keyNameList[j].ToString();
                //                }
                //                keyDesc = keyDesc.Substring(1, keyDesc.Length - 1);
                //                //修改时判断
                //                sb.AppendLine("/// <summary>");
                //                sb.AppendLine("/// 类型：方法");
                //                sb.AppendLine("/// 方法名称：根据" + keyDesc + "获取表" + tbDesc + "中的记录数");
                //                sb.AppendLine("/// 方法说明：修改时用，" + functionDesc);
                //                sb.AppendLine("/// </summary>");
                //                string colKeyParam = getParam(sColKeyName);
                //                sb.AppendLine("/// <param name=\"" + colKeyParam + "\"> 表" + tbDesc + "当前修改记录的自增编号</param>");
                //                string paramNames = string.Empty;
                //                for (int j = 0; j < keyList.Count; j++)
                //                {
                //                    //string[] sStrs = keyList[j].ToString().Split('_');
                //                    //string paramName = string.Empty;
                //                    //for (int index = 0; index < sStrs.Length; index++)
                //                    //{
                //                    //    if (index == 0)
                //                    //    {
                //                    //        paramName += sStrs[index].ToLower();
                //                    //    }
                //                    //    else
                //                    //    {
                //                    //        paramName += sStrs[index].Substring(0, 1).ToUpper() + sStrs[index].Substring(1, sStrs[index].Length - 1).ToLower();
                //                    //    }
                //                    //}
                //                    string paramName = getParam(keyList[j].ToString());
                //                    paramNames += "string " + paramName + ",";
                //                    sb.AppendLine("/// <param name=\"" + paramName + "\">" + keyNameList[j].ToString() + "</param>");
                //                }
                //                sb.AppendLine("/// <returns></returns>");
                //                sb.AppendLine("int " + functionName + "(string " + colKeyParam + "," + paramNames.Substring(0, paramNames.Length - 1) + ");");
                //                sb.AppendLine("");

                //                //新增时判断
                //                sb.AppendLine("/// <summary>");
                //                sb.AppendLine("/// 类型：方法");
                //                sb.AppendLine("/// 方法名称：根据" + keyDesc + "获取表" + tbDesc + "中的记录数");
                //                sb.AppendLine("/// 方法说明：新增时用，" + functionDesc);
                //                sb.AppendLine("/// </summary>");
                //                paramNames = string.Empty;
                //                for (int j = 0; j < keyList.Count; j++)
                //                {
                //                    //string[] sStrs = keyList[j].ToString().Split('_');
                //                    //string paramName = string.Empty;
                //                    //for (int index = 0; index < sStrs.Length; index++)
                //                    //{
                //                    //    if (index == 0)
                //                    //    {
                //                    //        paramName += sStrs[index].ToLower();
                //                    //    }
                //                    //    else
                //                    //    {
                //                    //        paramName += sStrs[index].Substring(0, 1).ToUpper() + sStrs[index].Substring(1, sStrs[index].Length - 1).ToLower();
                //                    //    }
                //                    //}
                //                    string paramName = getParam(keyList[j].ToString());
                //                    paramNames += "string " + paramName + ",";
                //                    sb.AppendLine("/// <param name=\"" + paramName + "\">" + keyNameList[j].ToString() + "</param>");
                //                }
                //                sb.AppendLine("/// <returns></returns>");
                //                sb.AppendLine("int " + functionName + "(" + paramNames.Substring(0, paramNames.Length - 1) + ");");
                //                sb.AppendLine("");
                //            }
                //            i--;
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //} 
                #endregion

                #region 获取服务文件

                OFDScript.Title = "打开(Open)";
                OFDScript.FileName = "";
                //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
                OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
                OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
                OFDScript.CheckFileExists = true;  //验证路径有效性
                OFDScript.CheckPathExists = true; //验证文件有效性

                StringBuilder sb = new StringBuilder();
                try
                {
                    if (OFDScript.ShowDialog() == DialogResult.OK)
                    {
                        String strConn = "Provider=Microsoft.Jet.OleDb.4.0; Data Source=" + OFDScript.FileName + "; Extended Properties=Excel 8.0;";
                        string sql = "select * from [逻辑组件列表$]";
                        DataSet infods = getExcelInfoDs(strConn, sql);

                        for (int i = 1; i < FiledDt.Rows.Count; i++)
                        {
                            if (FiledDt.Rows[i][0].ToString() == "")
                                FiledDt.Rows[i][0] = FiledDt.Rows[i - 1][0].ToString();
                        }

                        for (int i = 0; i < infods.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = infods.Tables[0].Rows[i];
                            DataRow[] drs = FiledDt.Select("TableName='" + dr[2].ToString().Trim() + "' and ColumnName='" + dr[4].ToString().Trim() + "'");
                            if (drs.Length > 0)
                            {
                                dr[6] = drs[0]["Type"].ToString();
                            }
                            drs = FiledDt.Select("TableName='" + dr[2].ToString().Trim() + "' and IDENTITY='√'");
                            if (drs.Length > 0)
                            {
                                dr[7] = drs[0]["ColumnName"].ToString();
                            }
                        }

                        string functionName = string.Empty;
                        for (int i = 0; i < infods.Tables[0].Rows.Count; i++)
                        {
                            if (functionName != infods.Tables[0].Rows[i][0].ToString())
                            {
                                string colName = string.Empty;
                                ArrayList keyList = new ArrayList();
                                ArrayList keyNameList = new ArrayList();
                                string tbName = string.Empty;
                                string tbDesc = string.Empty;
                                string functionDesc = string.Empty;
                                string sColKeyName = string.Empty;
                                functionName = infods.Tables[0].Rows[i][0].ToString();
                                functionDesc = infods.Tables[0].Rows[i][1].ToString();
                                tbName = infods.Tables[0].Rows[i][2].ToString();
                                tbDesc = infods.Tables[0].Rows[i][3].ToString();
                                sColKeyName = infods.Tables[0].Rows[i][7].ToString();
                                while (functionName == infods.Tables[0].Rows[i][0].ToString())
                                {
                                    keyList.Add(infods.Tables[0].Rows[i][4].ToString());
                                    keyNameList.Add(infods.Tables[0].Rows[i][5].ToString());
                                    i++;
                                    if (i == infods.Tables[0].Rows.Count)
                                        break;
                                }

                                string keyDesc = string.Empty;
                                for (int j = 0; j < keyNameList.Count; j++)
                                {
                                    keyDesc += "、" + keyNameList[j].ToString();
                                }
                                keyDesc = keyDesc.Substring(1, keyDesc.Length - 1);

                                sb.AppendLine("/// <summary>");
                                sb.AppendLine("/// 类型：修改时用，方法");
                                sb.AppendLine("/// 方法名称：根据" + keyDesc + "获取表" + tbDesc + "中的记录数");
                                sb.AppendLine("/// 方法说明：修改时用，" + functionDesc);
                                sb.AppendLine("/// </summary>");
                                ArrayList paramList = new ArrayList();

                                string colKeyParam = getParam(sColKeyName);
                                sb.AppendLine("/// <param name=\"" + colKeyParam + "\"> 表" + tbDesc + "当前修改记录的自增编号</param>");

                                string paramNames = string.Empty;
                                for (int j = 0; j < keyList.Count; j++)
                                {
                                    string paramName = getParam(keyList[j].ToString());
                                    paramList.Add(paramName);
                                    paramNames += "string " + paramName + ",";
                                    sb.AppendLine("/// <param name=\"" + paramName + "\">" + keyNameList[j].ToString() + "</param>");
                                }
                                sb.AppendLine("/// <returns></returns>");
                                //sb.AppendLine("int " + functionName + "(" + paramNames.Substring(0, paramNames.Length - 1) + ");");
                                sb.AppendLine("public int " + functionName + "(string " + colKeyParam + ", " + paramNames.Substring(0, paramNames.Length - 1) + ")");
                                sb.AppendLine("{");
                                sb.AppendLine("Dictionary<string, string> dicKeyValue = new Dictionary<string, string>();");
                                sb.AppendLine("if (!string.IsNullOrEmpty(" + colKeyParam + "))");
                                sb.AppendLine("dicKeyValue.Add(\"#" + sColKeyName + "#\", " + colKeyParam + ");");
                                for (int j = 0; j < keyList.Count; j++)
                                {
                                    string paramName = getParam(keyList[j].ToString());
                                    sb.AppendLine("dicKeyValue.Add(\"#" + keyList[j].ToString() + "#\", " + paramName + ");");
                                }
                                sb.AppendLine("IDataAccess dataAccess = ContainerContext.Container.Resolve<IDataAccess>();");
                                sb.AppendLine("try");
                                sb.AppendLine("{");
                                sb.AppendLine("using (DbConnection con = dataAccess.GetCurrentConnection())");
                                sb.AppendLine("{");
                                sb.AppendLine("DataTable dt= dataAccess.QueryCustomTableData(con, \"Portal/PublicBIZ/" + functionName + "\", dicKeyValue);");
                                sb.AppendLine("return (int)dt.Rows[0][0];");
                                sb.AppendLine("}");
                                sb.AppendLine("    }");
                                sb.AppendLine("  catch (Exception ex)");
                                sb.AppendLine("  {");
                                sb.AppendLine("LogError(Operate.查看, \"根据" + keyDesc + "获取表" + tbDesc + "中的记录数失败!\", ex);");
                                sb.AppendLine("throw new DataAccessException(ex.Message);");
                                sb.AppendLine("}");
                                sb.AppendLine("}");
                                sb.AppendLine("");


                                //新增时

                                sb.AppendLine("/// <summary>");
                                sb.AppendLine("/// 类型：新增时用，方法");
                                sb.AppendLine("/// 方法名称：根据" + keyDesc + "获取表" + tbDesc + "中的记录数");
                                sb.AppendLine("/// 方法说明：新增时用，" + functionDesc);
                                sb.AppendLine("/// </summary>");
                                paramList = new ArrayList();

                                paramNames = string.Empty;
                                for (int j = 0; j < keyList.Count; j++)
                                {
                                    string paramName = getParam(keyList[j].ToString());
                                    paramList.Add(paramName);
                                    paramNames += "string " + paramName + ",";
                                    sb.AppendLine("/// <param name=\"" + paramName + "\">" + keyNameList[j].ToString() + "</param>");
                                }
                                sb.AppendLine("/// <returns></returns>");
                                //sb.AppendLine("int " + functionName + "(" + paramNames.Substring(0, paramNames.Length - 1) + ");");
                                sb.AppendLine("public int " + functionName + "(" + paramNames.Substring(0, paramNames.Length - 1) + ")");
                                sb.AppendLine("{");
                                sb.AppendLine("return " + functionName + "(string.Empty ," + paramNames.Substring(0, paramNames.Length - 1).Replace("string ", " ") + ");");
                                sb.AppendLine("}");
                                sb.AppendLine("");
                            }
                            i--;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                #endregion
            }
        }

        private string getParam(string str)
        {
            string[] sStrs = str.ToString().Split('_');
            string paramName = string.Empty;
            for (int index = 0; index < sStrs.Length; index++)
            {
                if (index == 0)
                {
                    paramName += sStrs[index].ToLower();
                }
                else
                {
                    paramName += sStrs[index].Substring(0, 1).ToUpper() + sStrs[index].Substring(1, sStrs[index].Length - 1).ToLower();
                }
            }
            return paramName;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            excelds.Clear();
            OFDScript.Title = "打开(Open)";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    //取页面清单数据
                    String strConn = "Provider=Microsoft.Jet.OleDb.4.0; Data Source=" + OFDScript.FileName + "; Extended Properties=Excel 8.0;";

                    //调用方法如下：
                    DataTable excelDt = GetAllTableInfo(strConn);
                    if (excelDt != null && excelDt.Rows.Count > 0)
                    {
                        for (int j = 0; j < excelDt.Rows.Count; j++)
                        {
                            if (excelDt.Rows[j][2].ToString().IndexOf("_") == -1)
                            {
                                string sql = "select * from [" + excelDt.Rows[j][2].ToString() + "]";
                                DataSet infods = getExcelInfoDs(strConn, sql);
                                for (int m = 0; m < infods.Tables.Count; m++)
                                {
                                    if (infods.Tables[m] != null)
                                    {
                                        excelds.Tables.Add(infods.Tables[m].Copy());
                                    }
                                }
                            }
                        }
                        //dataGridView1.DataSource = excelds.Tables[0];
                        excelds.Tables[0].Rows.RemoveAt(0);
                        excelds.Tables[0].Rows.RemoveAt(0);

                        邦定TV(TVPageList, excelds.Tables[0]);
                        设定TV颜色(TVPageList);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void 设定TV颜色(TreeView TV)
        {
            //TV.ExpandAll();
            if (TV.VisibleCount > 0)
            {
                System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
                string connstr = configurationAppSettings.GetValue("get_TOOL_connstr", typeof(string)).ToString();

                SqlConnection myConn = new SqlConnection(connstr);
                string strSql = "select * from page_info ";
                DataTable DT = new DataTable();
                SqlDataAdapter DA = new SqlDataAdapter(strSql, myConn);
                DA.Fill(DT);
                if (DT.Rows.Count > 0)
                {
                    //一级
                    for (int i = 0; i < TV.Nodes.Count; i++)
                    {
                        //二级
                        foreach (TreeNode tn in TV.Nodes[i].Nodes)
                        {
                            //三级
                            foreach (TreeNode ctn in tn.Nodes)
                            {
                                if (ctn.Text.IndexOf("::") > -1)
                                {
                                    string strpageid = ctn.Text.Split("::".ToCharArray())[0].ToString();//页面名称
                                    DataRow[] drl = DT.Select("page_id='" + strpageid + "' and oi_flg='0'");
                                    DataRow[] drl1 = DT.Select("page_id='" + strpageid + "' and oi_flg='1'");
                                    if (drl.Length > 0 && drl1.Length > 0)
                                    {
                                        ctn.ForeColor = System.Drawing.Color.Green;
                                        ctn.BackColor = System.Drawing.Color.White;
                                    }
                                    else if (drl.Length > 0 || drl1.Length > 0)
                                    {
                                        ctn.BackColor = System.Drawing.Color.Green;
                                        ctn.ForeColor = System.Drawing.Color.Red;
                                    }
                                    else
                                    {
                                        ctn.ForeColor = System.Drawing.Color.Red;
                                        ctn.BackColor = System.Drawing.Color.White;
                                    }
                                }
                            }
                        }
                        //foreach(TreeNode tn in TV.Nodes[i].

                    }
                }
            }
        }

        private void 绑定三级菜单(TreeNode tn, DataTable dt, int index)
        {
            for (int i = index; i < dt.Rows.Count; i++)
            {
                if (i == index)
                {
                    TreeNode ctn = new TreeNode();//三级菜单
                    ctn.Text = dt.Rows[i][4].ToString() + "::" + dt.Rows[i][5].ToString();
                    tn.Nodes.Add(ctn);
                }
                else
                {
                    if (dt.Rows[i][0].ToString().Trim().Length == 0 && dt.Rows[i][1].ToString().Trim().Length == 0)
                    {
                        TreeNode ctn = new TreeNode();//三级菜单
                        ctn.Text = dt.Rows[i][4].ToString() + "::" + dt.Rows[i][5].ToString();
                        tn.Nodes.Add(ctn);
                    }
                    else
                    { break; }
                }
            }
        }


        private void 绑定二级菜单(TreeNode tn, DataTable dt, int index)
        {
            for (int i = index; i < dt.Rows.Count; i++)
            {
                TreeNode ctn = new TreeNode();//二级菜单
                if (i == index)
                {
                    if (dt.Rows[i][1].ToString().Trim().Length != 0)
                    {
                        ctn.Text = dt.Rows[i][1].ToString();
                    }
                    else
                    {
                        ctn.Text = dt.Rows[i][0].ToString();
                    }
                    tn.Nodes.Add(ctn);
                    绑定三级菜单(ctn, dt, i);
                }
                else
                {
                    if (dt.Rows[i][0].ToString().Trim().Length == 0)
                    {
                        if (dt.Rows[i][1].ToString().Trim().Length != 0)
                        {
                            ctn.Text = dt.Rows[i][1].ToString();
                            tn.Nodes.Add(ctn);
                            绑定三级菜单(ctn, dt, i);
                        }
                    }
                    else
                    { break; }
                }
            }
        }

        private void 邦定TV(TreeView TV, DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString().Trim().Length != 0)
                {
                    TreeNode tn = new TreeNode();//一级菜单
                    tn.Text = dt.Rows[i][0].ToString();
                    TV.Nodes.Add(tn);

                    绑定二级菜单(tn, dt, i);
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //checkedListBox2.SetItemChecked(5, true);
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.SetItemChecked(i, false);
            }
            //checkedListBox3.Items.Clear();
            if (CHKFiledDt != null)
            {
                CHKFiledDt.Rows.Clear();
                dataGridView2.DataSource = CHKFiledDt; Set底色(dataGridView2);

            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                //checkedListBox3.SetItemChecked(i, false);
            }
        }

        private void checkedListBox2_Click(object sender, EventArgs e)
        {

        }

        private DataTable CHKFiledDt;

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBox2.GetItemChecked(checkedListBox2.SelectedIndex))
            {
                string tablename = checkedListBox2.Items[checkedListBox2.SelectedIndex].ToString().Split(("::").ToCharArray())[2].ToString();

                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    if (dataGridView2.Rows[i].Cells[2].Value.ToString().Trim() == tablename.ToString().Trim())
                    {
                        return;
                    }
                }

                for (int i = 0; i < FiledDt.Rows.Count; i++)
                {
                    if (FiledDt.Rows[i][0].ToString() == tablename)
                    {
                        //checkedListBox3.Items.Add(FiledDt.Rows[i][1].ToString() + "::" + FiledDt.Rows[i][0].ToString() + " | " + FiledDt.Rows[i][13].ToString() + "::" + FiledDt.Rows[i][3].ToString());
                        //i++;

                        if (CHKFiledDt == null)
                        {
                            CHKFiledDt = new DataTable();
                            CHKFiledDt.TableName = "CHKTABLE";
                            CHKFiledDt.Columns.Add("chk");
                            CHKFiledDt.Columns.Add("TABLE_NAME");
                            CHKFiledDt.Columns.Add("TABLE_CODE");
                            CHKFiledDt.Columns.Add("FILED_NAME");
                            CHKFiledDt.Columns.Add("FILED_CODE");
                        }

                        DataRow DR = CHKFiledDt.NewRow();
                        DR[1] = FiledDt.Rows[i][1].ToString();
                        DR[2] = FiledDt.Rows[i][0].ToString();
                        DR[3] = FiledDt.Rows[i][13].ToString();
                        DR[4] = FiledDt.Rows[i][3].ToString();
                        CHKFiledDt.Rows.Add(DR);

                        i++;

                        while (FiledDt.Rows[i][0].ToString().Trim().Length == 0)
                        {
                            //checkedListBox3.Items.Add(checkedListBox2.Items[checkedListBox2.SelectedIndex].ToString() + " | " + FiledDt.Rows[i][13].ToString() + "::" + FiledDt.Rows[i][3].ToString());

                            DR = CHKFiledDt.NewRow();
                            DR[1] = checkedListBox2.Items[checkedListBox2.SelectedIndex].ToString().Split(("::").ToCharArray())[0].ToString();
                            DR[2] = checkedListBox2.Items[checkedListBox2.SelectedIndex].ToString().Split(("::").ToCharArray())[2].ToString();
                            DR[3] = FiledDt.Rows[i][13].ToString();
                            DR[4] = FiledDt.Rows[i][3].ToString();
                            CHKFiledDt.Rows.Add(DR);

                            i++;
                            if (i == FiledDt.Rows.Count)
                                break;
                        }
                        break;
                    }
                }
            }
            else
            {
                if (CHKFiledDt != null)
                {
                    //string str = checkedListBox2.Items[checkedListBox2.SelectedIndex].ToString();
                    string str = checkedListBox2.Items[checkedListBox2.SelectedIndex].ToString().Split(("::").ToCharArray())[2].ToString(); ;
                    ArrayList alist = new ArrayList();
                    for (int i = 0; i < CHKFiledDt.Rows.Count; i++)
                    {
                        if (CHKFiledDt.Rows[i]["TABLE_CODE"].ToString().IndexOf(str.Trim()) > -1)
                        {
                            alist.Add(i);
                        }
                    }
                    for (int i = 0; i < alist.Count; i++)
                    {
                        CHKFiledDt.Rows.RemoveAt(Convert.ToInt32(alist[i]) - i);
                    }
                }
            }
            if (CHKFiledDt != null)
            {
                dataGridView2.DataSource = CHKFiledDt; Set底色(dataGridView2);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //输入
            插入页面信息("1");

        }

        private void 插入页面信息(string OIFLG)
        {
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("get_TOOL_connstr", typeof(string)).ToString();

            SqlConnection myConn = new SqlConnection(connstr);
            //

            string strpageid = TVPageList.SelectedNode.Text.Split("::".ToCharArray())[0].ToString();//页面名称
            string strtableid = string.Empty;
            string strfiledid = string.Empty;

            string sqlstr = "DELETE FROM PAGE_INFO WHERE PAGE_ID='" + strpageid + "' AND OI_FLG='" + OIFLG + "' ; ";

            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                DataGridViewCheckBoxCell dgdchk = (DataGridViewCheckBoxCell)dataGridView2.Rows[i].Cells[0];
                if (dgdchk != null && (bool)dgdchk.FormattedValue) //(dgdchk.Value==dgdchk.TrueValue)
                {
                    strtableid = dataGridView2.Rows[i].Cells[2].ToString(); //checkedListBox3.Items[i].ToString().Split('|')[0].Split("::".ToCharArray())[2].ToString();
                    strfiledid = dataGridView2.Rows[i].Cells[4].ToString(); //checkedListBox3.Items[i].ToString().Split('|')[1].Split("::".ToCharArray())[2].ToString();
                    sqlstr += "insert into page_info(page_id,table_id,filed_id ,OI_FLG) values ('" + strpageid + "','" + strtableid + "','" + strfiledid + "','" + OIFLG + "'); ";
                }
            }

            try
            {
                myConn.Open();
                SqlCommand cmd = new SqlCommand(sqlstr, myConn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("更新成功！");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                myConn.Close();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //输出
            if (radioButton1.Checked)
            {
                插入页面信息("1");
                设定TV颜色(TVPageList);
            }
            else
            {
                插入页面信息("0");
                设定TV颜色(TVPageList);
            }
        }

        private void 取得字段信息(string OIFLG, String PAGEID)
        {
            //clear data for checkboxlist
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                checkedListBox2.SetItemChecked(i, false);
            }
            //checkedListBox3.Items.Clear();
            if (CHKFiledDt != null)
            {
                CHKFiledDt.Rows.Clear();
                dataGridView2.DataSource = CHKFiledDt; Set底色(dataGridView2);

            }


            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("get_TOOL_connstr", typeof(string)).ToString();

            SqlConnection myConn = new SqlConnection(connstr);
            string strSql = "select * from page_info where page_id='" + PAGEID + "' and OI_FLG='" + OIFLG + "'";
            DataTable DT = new DataTable();
            SqlDataAdapter DA = new SqlDataAdapter(strSql, myConn);
            DA.Fill(DT);
            if (DT.Rows.Count > 0)
            {
                //选择已有被选择字段的表
                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    string tablename = checkedListBox2.Items[i].ToString().Split(("::").ToCharArray())[2].ToString();
                    for (int index = 0; index < DT.Rows.Count; index++)
                    {
                        if (tablename.Trim() == DT.Rows[index]["table_id"].ToString().Trim())
                        {
                            checkedListBox2.SetItemChecked(i, true);
                        }
                    }
                }

                //列出相关字段，并选择已选择的字段
                for (int index = 0; index < checkedListBox2.Items.Count; index++)
                {
                    if (checkedListBox2.GetItemChecked(index))
                    {
                        string tablename = checkedListBox2.Items[index].ToString().Split(("::").ToCharArray())[2].ToString();
                        for (int i = 0; i < FiledDt.Rows.Count; i++)
                        {
                            if (FiledDt.Rows[i][0].ToString() == tablename)
                            {
                                DataRow[] drl = DT.Select("table_id='" + tablename + "' and filed_id='" + FiledDt.Rows[i][3].ToString() + "'");
                                if (drl.Length > 0)
                                {
                                    if (CHKFiledDt == null)
                                    {
                                        CHKFiledDt = new DataTable();
                                        CHKFiledDt.Columns.Add("chk");
                                        CHKFiledDt.Columns.Add("TABLE_NAME");
                                        CHKFiledDt.Columns.Add("TABLE_CODE");
                                        CHKFiledDt.Columns.Add("FILED_NAME");
                                        CHKFiledDt.Columns.Add("FILED_CODE");
                                    }

                                    DataRow DR = CHKFiledDt.NewRow();
                                    DR[0] = true;
                                    DR[1] = FiledDt.Rows[i][1].ToString();
                                    DR[2] = FiledDt.Rows[i][0].ToString();
                                    DR[3] = FiledDt.Rows[i][13].ToString();
                                    DR[4] = FiledDt.Rows[i][3].ToString();
                                    CHKFiledDt.Rows.Add(DR);

                                    //checkedListBox3.Items.Add(FiledDt.Rows[i][1].ToString() + "::" + FiledDt.Rows[i][0].ToString() + " | " + FiledDt.Rows[i][13].ToString() + "::" + FiledDt.Rows[i][3].ToString(), true);
                                }
                                else
                                {
                                    if (CHKFiledDt == null)
                                    {
                                        CHKFiledDt = new DataTable();
                                        CHKFiledDt.Columns.Add("chk");
                                        CHKFiledDt.Columns.Add("TABLE_NAME");
                                        CHKFiledDt.Columns.Add("TABLE_CODE");
                                        CHKFiledDt.Columns.Add("FILED_NAME");
                                        CHKFiledDt.Columns.Add("FILED_CODE");
                                    }

                                    DataRow DR = CHKFiledDt.NewRow();
                                    DR[0] = false;
                                    DR[1] = FiledDt.Rows[i][1].ToString();
                                    DR[2] = FiledDt.Rows[i][0].ToString();
                                    DR[3] = FiledDt.Rows[i][13].ToString();
                                    DR[4] = FiledDt.Rows[i][3].ToString();
                                    CHKFiledDt.Rows.Add(DR);

                                    //checkedListBox3.Items.Add(FiledDt.Rows[i][1].ToString() + "::" + FiledDt.Rows[i][0].ToString() + " | " + FiledDt.Rows[i][13].ToString() + "::" + FiledDt.Rows[i][3].ToString(), false );
                                }

                                i++;


                                while (FiledDt.Rows[i][0].ToString().Trim().Length == 0)
                                {
                                    drl = DT.Select("table_id='" + tablename + "' and filed_id='" + FiledDt.Rows[i][3].ToString() + "'");
                                    if (drl.Length > 0)
                                    {
                                        //if (CHKFiledDt == null)
                                        //{
                                        //    CHKFiledDt = new DataTable();
                                        //    CHKFiledDt.Columns.Add("chk");
                                        //    CHKFiledDt.Columns.Add("TABLE_NAME");
                                        //    CHKFiledDt.Columns.Add("TABLE_CODE");
                                        //    CHKFiledDt.Columns.Add("FILED_NAME");
                                        //    CHKFiledDt.Columns.Add("FILED_CODE");
                                        //}

                                        DataRow DR = CHKFiledDt.NewRow();
                                        DR[0] = true;
                                        DR[1] = checkedListBox2.Items[index].ToString().Split(("::").ToCharArray())[0].ToString();
                                        DR[2] = checkedListBox2.Items[index].ToString().Split(("::").ToCharArray())[2].ToString();
                                        DR[3] = FiledDt.Rows[i][13].ToString();
                                        DR[4] = FiledDt.Rows[i][3].ToString();
                                        CHKFiledDt.Rows.Add(DR);
                                        //checkedListBox3.Items.Add(checkedListBox2.Items[index].ToString() + " | " + FiledDt.Rows[i][13].ToString() + "::" + FiledDt.Rows[i][3].ToString() ,true );
                                    }
                                    else
                                    {
                                        DataRow DR = CHKFiledDt.NewRow();
                                        DR[0] = false;
                                        DR[1] = checkedListBox2.Items[index].ToString().Split(("::").ToCharArray())[0].ToString();
                                        DR[2] = checkedListBox2.Items[index].ToString().Split(("::").ToCharArray())[2].ToString();
                                        DR[3] = FiledDt.Rows[i][13].ToString();
                                        DR[4] = FiledDt.Rows[i][3].ToString();
                                        CHKFiledDt.Rows.Add(DR);
                                        //checkedListBox3.Items.Add(checkedListBox2.Items[index].ToString() + " | " + FiledDt.Rows[i][13].ToString() + "::" + FiledDt.Rows[i][3].ToString(),false );
                                    }
                                    i++;
                                    if (i == FiledDt.Rows.Count)
                                        break;
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (TVPageList.SelectedNode.Level == 2)
            {
                设定TV颜色(TVPageList);
                TVPageList.SelectedNode.BackColor = System.Drawing.Color.Blue;
                TVPageList.SelectedNode.ForeColor = System.Drawing.Color.White;
            }

            if (CHKFiledDt != null)
            {
                dataGridView2.DataSource = CHKFiledDt; Set底色(dataGridView2);
            }
        }

        private void Set底色(DataGridView dgv)
        {
            if (dgv.Rows.Count > 0)
            {
                System.Drawing.Color color1 = System.Drawing.Color.White;
                System.Drawing.Color color2 = System.Drawing.Color.DarkGreen;
                System.Drawing.Color color = color1;
                string tableName = dgv.Rows[0].Cells[1].Value.ToString();
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    if (dgv.Rows[i].Cells[1].Value.ToString() != tableName)
                    {
                        tableName = dgv.Rows[i].Cells[1].Value.ToString();
                        if (color == color1)
                            color = color2;
                        else
                            color = color1;
                    }
                    for (int j = 0; j < dgv.ColumnCount; j++)
                    {
                        dgv.Rows[i].Cells[j].Style.BackColor = color;
                    }
                    //dgv.Rows[i].Cells[1].Style.BackColor = color;
                }
            }
        }

        private void TVPageList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //
            string strpageid = TVPageList.SelectedNode.Text.Split("::".ToCharArray())[0].ToString();//页面名称
            if (radioButton1.Checked)
            {
                取得字段信息("1", strpageid);
            }
            else
            {
                取得字段信息("0", strpageid);
            }
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            OFDScript.Title = "打开(Open)";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    //取页面清单数据
                    String strConn = "Provider=Microsoft.Jet.OleDb.4.0; Data Source=" + OFDScript.FileName + "; Extended Properties=Excel 8.0;";

                    //调用方法如下：
                    DataTable excelDt = GetAllTableInfo(strConn);
                    if (excelDt != null && excelDt.Rows.Count > 0)
                    {
                        for (int j = 0; j < excelDt.Rows.Count; j++)
                        {
                            string sql = "select * from [" + excelDt.Rows[j][2].ToString() + "]";
                            DataSet infods = getExcelInfoDs(strConn, sql);
                            for (int m = 0; m < infods.Tables.Count; m++)
                            {
                                if (infods.Tables[m] != null)
                                {
                                    excelds.Tables.Add(infods.Tables[m].Copy());
                                }
                            }
                        }
                        //dataGridView1.DataSource = excelds.Tables[0];
                        excelds.Tables[0].Rows.RemoveAt(0);
                        excelds.Tables[0].Rows.RemoveAt(0);

                        邦定TV(treeView1, excelds.Tables[0]);
                        设定TV颜色(treeView1);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            string strpageid = TVPageList.SelectedNode.Text.Split("::".ToCharArray())[0].ToString();//页面名称
            if (radioButton1.Checked)
            {
                取得字段信息("1", strpageid);
                设定TV颜色(TVPageList);
            }
            else
            {
                取得字段信息("0", strpageid);
                设定TV颜色(TVPageList);
            }
        }

        private TreeNode FindNode(TreeNode tnParent, string strValue)
        {

            if (tnParent == null) return null;

            if (tnParent.Text == strValue) return tnParent;



            TreeNode tnRet = null;

            foreach (TreeNode tn in tnParent.Nodes)
            {

                tnRet = FindNode(tn, strValue);

                if (tnRet != null) break;

            }

            return tnRet;

        }


        private void 拷贝输入输出数据(string PAGEID, String OIFLG)
        {
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("get_TOOL_connstr", typeof(string)).ToString();

            SqlConnection myConn = new SqlConnection(connstr);
            string strSql = "select * from page_info where page_id='" + PAGEID + "' and OI_FLG='" + OIFLG + "'";
            DataTable DT = new DataTable();
            SqlDataAdapter DA = new SqlDataAdapter(strSql, myConn);
            DA.Fill(DT);
            if (DT.Rows.Count > 0)
            {
                if (OIFLG == "1")
                {
                    OIFLG = "0";
                }
                else
                {
                    OIFLG = "1";
                }
                string sqlstr = "DELETE FROM PAGE_INFO WHERE PAGE_ID='" + PAGEID + "' AND OI_FLG='" + OIFLG + "' ; ";

                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    sqlstr += "insert into page_info(page_id,table_id,filed_id ,OI_FLG) values ('" + DT.Rows[i]["PAGE_ID"].ToString() + "','" + DT.Rows[i]["TABLE_ID"].ToString() + "','" + DT.Rows[i]["FILED_ID"].ToString() + "','" + OIFLG + "'); ";
                }
                try
                {
                    myConn.Open();
                    SqlCommand cmd = new SqlCommand(sqlstr, myConn);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("OK！");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                finally
                {
                    myConn.Close();
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string strpageid = TVPageList.SelectedNode.Text.Split("::".ToCharArray())[0].ToString();//页面名称
            ///输出到输入
            if (TVPageList.SelectedNode.Level == 2)
            {
                if (MessageBox.Show("你确定要把" + TVPageList.SelectedNode.Text + "页面的输出字段覆盖输入字段?", "My Application", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
                {
                    radioButton1.Checked = true;
                    radioButton2.Checked = false;
                    拷贝输入输出数据(strpageid, "0");
                    取得字段信息("1", strpageid);
                    设定TV颜色(TVPageList);
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //输入到输出
            string strpageid = TVPageList.SelectedNode.Text.Split("::".ToCharArray())[0].ToString();//页面名称
            if (TVPageList.SelectedNode.Level == 2)
            {
                if (MessageBox.Show("你确定要把" + TVPageList.SelectedNode.Text + "页面的输入字段覆盖输出字段?", "My Application", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
                {
                    radioButton2.Checked = true;
                    radioButton1.Checked = false;
                    拷贝输入输出数据(strpageid, "1");
                    取得字段信息("0", strpageid);
                    设定TV颜色(TVPageList);
                }
            }
        }

        private void checkedListBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string strpageid = TVPageList.SelectedNode.Text.Split("::".ToCharArray())[0].ToString();//页面名称
            //string  strtableid = checkedListBox3.Items[checkedListBox3.SelectedIndex].ToString().Split('|')[0].Split("::".ToCharArray())[2].ToString();
            //string  strfiledid = checkedListBox3.Items[checkedListBox3.SelectedIndex].ToString().Split('|')[1].Split("::".ToCharArray())[2].ToString();
            //String OIFLG = String.Empty;
            //String sqlstr = String.Empty;

            //if (radioButton1.Checked)
            //{
            //    OIFLG = "1";
            //}
            //else
            //{
            //    OIFLG = "0";
            //}
            //if (checkedListBox3.GetItemChecked(checkedListBox3.SelectedIndex))
            //{

            //     sqlstr += "DELETE FROM page_info WHERE page_id='" + strpageid + "' AND table_id='" + strtableid + "' AND filed_id ='" + strfiledid + "' AND OI_FLG='" + OIFLG + "'; ";
            //   sqlstr += "insert into page_info(page_id,table_id,filed_id ,OI_FLG) values ('" + strpageid + "','" + strtableid + "','" + strfiledid + "','" + OIFLG + "'); ";
            //}
            //else
            //{
            //     sqlstr += "DELETE FROM page_info WHERE page_id='" + strpageid + "' AND table_id='" + strtableid + "' AND filed_id ='" + strfiledid + "' AND OI_FLG='" + OIFLG + "'; ";
            //}

            //System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            //string connstr = configurationAppSettings.GetValue("get_TOOL_connstr", typeof(string)).ToString();

            //SqlConnection myConn = new SqlConnection(connstr);

            //try
            //{
            //    myConn.Open();
            //    SqlCommand cmd = new SqlCommand(sqlstr, myConn);
            //    cmd.ExecuteNonQuery();
            //    //MessageBox.Show("更新成功！");
            //}
            //catch (Exception eX)
            //{
            //    MessageBox.Show(eX.ToString());
            //}
            //finally
            //{
            //    myConn.Close();
            //}

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewCheckBoxCell c = (DataGridViewCheckBoxCell)dataGridView2.CurrentCell;


                string strpageid = TVPageList.SelectedNode.Text.Split("::".ToCharArray())[0].ToString();//页面名称
                string strtableid = dataGridView2.CurrentRow.Cells[2].Value.ToString();//checkedListBox3.Items[checkedListBox3.SelectedIndex].ToString().Split('|')[0].Split("::".ToCharArray())[2].ToString();
                string strfiledid = dataGridView2.CurrentRow.Cells[4].Value.ToString();//checkedListBox3.Items[checkedListBox3.SelectedIndex].ToString().Split('|')[1].Split("::".ToCharArray())[2].ToString();
                String OIFLG = String.Empty;
                String sqlstr = String.Empty;

                if (radioButton1.Checked)
                {
                    OIFLG = "1";
                }
                else
                {
                    OIFLG = "0";
                }

                if (c != null && !(bool)c.EditedFormattedValue)
                {

                    sqlstr += "DELETE FROM page_info WHERE page_id='" + strpageid + "' AND table_id='" + strtableid + "' AND filed_id ='" + strfiledid + "' AND OI_FLG='" + OIFLG + "'; ";
                    sqlstr += "insert into page_info(page_id,table_id,filed_id ,OI_FLG) values ('" + strpageid + "','" + strtableid + "','" + strfiledid + "','" + OIFLG + "'); ";
                }
                else
                {
                    sqlstr += "DELETE FROM page_info WHERE page_id='" + strpageid + "' AND table_id='" + strtableid + "' AND filed_id ='" + strfiledid + "' AND OI_FLG='" + OIFLG + "'; ";
                }

                System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
                string connstr = configurationAppSettings.GetValue("get_TOOL_connstr", typeof(string)).ToString();

                SqlConnection myConn = new SqlConnection(connstr);

                try
                {
                    myConn.Open();
                    SqlCommand cmd = new SqlCommand(sqlstr, myConn);
                    cmd.ExecuteNonQuery();
                    //MessageBox.Show("更新成功！");
                }
                catch (Exception eX)
                {
                    MessageBox.Show(eX.ToString());
                }
                finally
                {
                    myConn.Close();
                }

            }
            catch
            {
                return;
            }


        }

        private void dataGridView2_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    DataGridViewCheckBoxCell c = (DataGridViewCheckBoxCell)dataGridView2.CurrentCell;

            //}
            //catch
            //{
            //    return;
            //}

            //MessageBox.Show("fsdafds");
        }


        private void button20_Click(object sender, EventArgs e)
        {
            OFDScript.Title = "分析Excel";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    String sqlStr = MyEXCEL.把报价导入数据库(OFDScript.FileName);
                    if (sqlStr != string.Empty)
                    {

                        sqlStr = sqlStr.ToUpper();
                        System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
                        string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();


                        try
                        {

                            using (OleDbConnection myConn = new OleDbConnection(connstr))
                            {
                                OleDbTransaction ts = null;

                                myConn.Open();
                                ts = myConn.BeginTransaction();
                                OleDbCommand cmd = new OleDbCommand(sqlStr, myConn, ts);
                                cmd.ExecuteNonQuery();
                                ts.Commit();
                                MessageBox.Show("更新成功！");
                            }

                        }
                        catch (Exception eX)
                        {
                            MessageBox.Show(eX.ToString());

                            string strPath = @"c:\excel_log.txt";

                            using (StreamWriter sw = new StreamWriter(strPath, false, Encoding.UTF8))
                            {
                                sw.Write(sqlStr);
                            }


                        }
                        finally
                        {
                            //myConn.Close();
                        }


                    } /* */
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }


        }


        private void button15_Click(object sender, EventArgs e)
        {

            OFDScript.Title = "数据库概要设计文档模板";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    String sqlStr = MyEXCEL.把概要设计的数据导入数据库(OFDScript.FileName);
                    if (sqlStr != string.Empty)
                    {
                        sqlStr = sqlStr.ToUpper();
                        System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
                        string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();

                        //OleDbConnection myConn = new OleDbConnection(connstr);

                        try
                        {

                            using (OleDbConnection myConn = new OleDbConnection(connstr))
                            {
                                OleDbTransaction ts = null;

                                myConn.Open();
                                ts = myConn.BeginTransaction();
                                OleDbCommand cmd = new OleDbCommand(sqlStr, myConn, ts);
                                cmd.ExecuteNonQuery();
                                ts.Commit();
                                MessageBox.Show("更新成功！");
                            }

                        }
                        catch (Exception eX)
                        {
                            MessageBox.Show(eX.ToString());

                            string strPath = @"c:\excel_log.txt";

                            using (StreamWriter sw = new StreamWriter(strPath, false, Encoding.UTF8))
                            {
                                sw.Write(sqlStr);
                            }


                        }
                        finally
                        {
                            //myConn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void button16_Click(object sender, EventArgs e)
        {
            OFDScript.Title = "测试";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    MyEXCEL.测试(OFDScript.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void checkedListBox3_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (checkedListBox3.GetItemChecked(checkedListBox3.SelectedIndex))
            {
                for (int i = 0; i < checkedListBox3.Items.Count; i++)
                {
                    if (i != checkedListBox3.SelectedIndex)
                        checkedListBox3.SetItemChecked(i, false);
                }
            }
            string str = checkedListBox3.SelectedItem.ToString();
            ArrayList alist = new ArrayList();
            ArrayList chkalist = new ArrayList();

            ArrayList alist1 = new ArrayList();
            ArrayList chkalist1 = new ArrayList();

            for (int j = 0; j < checkedListBox2.Items.Count; j++)
            {
                if (checkedListBox2.Items[j].ToString().Split('_')[1].ToString() == str)
                {
                    alist.Add(checkedListBox2.Items[j].ToString());
                    chkalist.Add(checkedListBox2.GetItemChecked(j));
                }
                else
                {
                    alist1.Add(checkedListBox2.Items[j].ToString());
                    chkalist1.Add(checkedListBox2.GetItemChecked(j));
                }
            }
            checkedListBox2.Items.Clear();
            for (int index = 0; index < alist.Count; index++)
            {
                checkedListBox2.Items.Add(alist[index], (bool)chkalist[index]);
            }

            for (int index = 0; index < alist1.Count; index++)
            {
                checkedListBox2.Items.Add(alist1[index], (bool)chkalist1[index]);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            OFDScript.Title = "打开(Open)";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    //取页面清单数据
                    String strConn = "Provider=Microsoft.Jet.OleDb.4.0; Data Source=" + OFDScript.FileName + "; Extended Properties=Excel 8.0;";

                    //调用方法如下：
                    //DataTable excelDt = GetAllTableInfo(strConn);

                    string sql = "select * from [目录＄]";
                    DataSet infods = getExcelInfoDs(strConn, sql);
                    //dataGridView1.DataSource = excelds.Tables[0];
                    //excelds.Tables[0].Rows.RemoveAt(0);
                    infods.Tables[0].Rows.RemoveAt(0);
                    MyEXCEL.基础数据设计文档NEW(infods.Tables[0], OFDScript.FileName, FiledDt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            //取消和并并赋值

            OFDScript.Title = "打开(Open)";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    //取页面清单数据
                    //String strConn = "Provider=Microsoft.Jet.OleDb.4.0; Data Source=" + OFDScript.FileName + "; Extended Properties=Excel 8.0;";

                    ////调用方法如下：
                    //DataTable excelDt = GetAllTableInfo(strConn);

                    //for (int i = 0; i < excelDt.Rows.Count; i++)
                    //{

                    //}
                    MyEXCEL.取消和并并赋值(OFDScript.FileName, Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
                    //string sql = "select * from [目录＄]";
                    //DataSet infods = getExcelInfoDs(strConn, sql);
                    //dataGridView1.DataSource = excelds.Tables[0];
                    //excelds.Tables[0].Rows.RemoveAt(0);
                    //infods.Tables[0].Rows.RemoveAt(0);
                    //MyEXCEL.基础数据设计文档NEW(infods.Tables[0], OFDScript.FileName, FiledDt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            //frm_map obj = new frm_map();
            //obj.Show();
        }

        private void treeView2_Click(object sender, EventArgs e)
        {
            //if (treeView2.SelectedNode != null)
            //{
            //    TreeNode tn = treeView2.SelectedNode;
            //    if (tn.Level == 0)
            //    {
            //        contextMenuStrip1.Items[0].Visible = true;
            //        contextMenuStrip1.Items[1].Visible = false;
            //        contextMenuStrip1.Items[2].Visible = false;
            //    }
            //    else if (tn.Level == 1)
            //    {
            //        contextMenuStrip1.Items[0].Visible = false;
            //        contextMenuStrip1.Items[1].Visible = true;
            //        contextMenuStrip1.Items[2].Visible = true;
            //    }
            //    else if(tn.Level==2)
            //    {
            //        contextMenuStrip1.Items[0].Visible = false;
            //        contextMenuStrip1.Items[1].Visible = false;
            //        contextMenuStrip1.Items[2].Visible = true;
            //    }
            //}
        }

        private void treeView2_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button = MouseButtons.Right)
            //{
            if (treeView2.SelectedNode != null)
            {
                TreeNode tn = treeView2.SelectedNode;
                if (tn.Level == 0)
                {
                    contextMenuStrip1.Items[0].Visible = true;
                    contextMenuStrip1.Items[1].Visible = false;
                    contextMenuStrip1.Items[2].Visible = false;
                }
                else if (tn.Level == 1)
                {
                    contextMenuStrip1.Items[0].Visible = false;
                    contextMenuStrip1.Items[1].Visible = true;
                    contextMenuStrip1.Items[2].Visible = true;
                }
                else if (tn.Level == 2)
                {
                    contextMenuStrip1.Items[0].Visible = false;
                    contextMenuStrip1.Items[1].Visible = false;
                    contextMenuStrip1.Items[2].Visible = true;
                }
            }
            else
            {
                contextMenuStrip1.Items[0].Visible = false;
                contextMenuStrip1.Items[1].Visible = false;
                contextMenuStrip1.Items[2].Visible = false;
            }
            //}
        }

        private void 添加表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAddTable obj = new FrmAddTable();
            obj.ShowDialog();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void button21_Click(object sender, EventArgs e)
        {
            OFDScript.Title = "分析Excel";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    String sqlStr = MyEXCEL.把商品导入数据库(OFDScript.FileName);


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            OFDScript.Title = "Get a SQL file";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "txt文件(*.txt)|*.txt";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {
                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    String sqlStr = "";

                    using (StreamReader sr = new StreamReader(OFDScript.FileName))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            sqlStr += line;
                        }
                    }





                    //using (System.IO.StreamWriter sw = System.IO.File.ReadAllLines(OFDScript.FileName))
                    //{
                    //    sqlStr = "";
                    //}


                    //MyEXCEL.把商品导入数据库(OFDScript.FileName);  

                    if (sqlStr != string.Empty)
                    {

                        sqlStr = sqlStr.ToUpper();
                        System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
                        string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();


                        try
                        {

                            using (OleDbConnection myConn = new OleDbConnection(connstr))
                            {
                                OleDbTransaction ts = null;

                                myConn.Open();
                                ts = myConn.BeginTransaction();
                                OleDbCommand cmd = new OleDbCommand(sqlStr, myConn, ts);
                                cmd.ExecuteNonQuery();
                                ts.Commit();
                                MessageBox.Show("更新商品成功！");
                            }

                        }
                        catch (Exception eX)
                        {
                            MessageBox.Show(eX.ToString());

                            string strPath = @"c:\excel_log2.txt";

                            using (StreamWriter sw = new StreamWriter(strPath, false, Encoding.UTF8))
                            {
                                sw.Write(sqlStr);
                            }


                        }
                        finally
                        {
                            //myConn.Close();
                        }


                    } /* */
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void tabPage9_Click(object sender, EventArgs e)
        {

        }

        private void button23_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(xxxxxxxxxxxxxx);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;


            thread.Start();
        }


        private void button121_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(xxxxxxxxxxxxxx222);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;


            thread.Start();
        }

        /// <summary>
        /// 最大允许线程数
        /// </summary>
        private int ThreadCount = 5;

        /// <summary>
        /// 采集进程记录
        /// </summary>
        private ArrayList alMsgList = new ArrayList();

        /// <summary>
        /// 线程
        /// </summary>
        private Thread thread;

        /// <summary>
        /// 线程计数器
        /// </summary>
        private int Count = 0;

        #region 多线程采集

        /// <summary>
        /// 创建线程
        /// </summary>
        private void CreateManageThreads()
        {
            thread = new Thread(new ThreadStart(ManageThreads));

            thread.Start();
        }


        /// <summary>
        /// 循环线程
        /// </summary>
        private void ManageThreads()
        {
            while (thread.ThreadState == System.Threading.ThreadState.Running)
            {
                GetCircleStart();

                Count = 0;

                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// 根据采集周期判定获取当前需要采集的记录
        /// </summary>
        /// <param name="obj"></param>
        private void GetCircleStart()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;


            string ls_sql = "SELECT ID,URL FROM URL_BASE WHERE STATUS IS NULL OR STATUS !=1 order by ID";
            group = DB.GetDataGroup(ls_sql);



            Thread[] subTread = new Thread[alMsgList.Count];

            for (int i = 0; i < group.Table.Rows.Count; i++)
            {
                //采集进程配置实体
                //Entity_GatherThread obj = (Entity_GatherThread)alMsgList[i];

                //if (obj.StateId == "2")
                //{
                //obj.PeriodGather.ToString() != "0"表示定时采集；obj.PeriodGather.ToString() == ""表示循环采集
                //if (obj.PeriodGather.ToString() == "0")
                //{
                //if (DateTime.Now >= obj.NextTime && obj.OneYesOrNo)
                //{
                Count++;
                if (Count <= this.ThreadCount)
                {
                    //obj.OneFlag = false;
                    //obj.NextTime = DateTime.Now;
                    //subTread[i] = new Thread(new ParameterizedThreadStart(StartCreateGetData));
                    //subTread[i].Start(obj);


                    string pig = getUrlSource(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");
                    string ls_up = "UPDATE URL_BASE SET HTML='" + pig + "' ,STATUS=1 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                    DB.ExecuteSQL(ls_up);
                    pig = "";

                }
                //}
                //}
                //else
                //{
                //    if (DateTime.Now <= obj.EndTime && DateTime.Now >= obj.NextTime)
                //    {
                //        Count++;
                //        if (Count <= this.ThreadCount)
                //        {
                //            obj.NextTime = DateTime.Now.AddMinutes(obj.PeriodGather);
                //            subTread[i] = new Thread(new ParameterizedThreadStart(StartCreateGetData));
                //            subTread[i].Start(obj);
                //        }
                //    }
                //}
                //}
            }
        }


        /// <summary>
        /// 开始数据采集操作(按每天为周期)
        /// </summary>
        /// <param name="obj"></param>
        /*
        private void StartCreateGetData(object obj)
        {
            Entity_GatherThread entity = (Entity_GatherThread)obj;

            //更改Arrylist状态：正在采集
            entity.StateId = "1";
            entity.StateName = "正在采集";

            //启动数据采集函数
            GatherOper(entity.DataSourceId, entity.MgId);

            //更改当前列表和Arrylist状态
            //entity.OneFlag == true表示定时采集的当天未采集,false表示已采集
            if (entity.OneFlag == false)
            {
                entity.StateName = "完成采集";
                entity.StateId = "3";
            }
            else
            {
                entity.StateName = "等待采集";
                entity.StateId = "2";
            }


            //采集完毕
        }

        */


        #endregion



        public void xxxxxxxxxxxxxx()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;



            string ls_sql = "SELECT ID, URL FROM plan425_b1    WHERE STATUS IS NULL   order by ID";//
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                //L_M2.Text = group.Table.Rows.Count.ToString();
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = "";
                    //pig = getUrlSource(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312
                    //pig = getUrlSource_Adv(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312
                    
                    SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
                    foreach (SHDocVw.InternetExplorer ie in shellWindows)
                    {
                        string filename = System.IO.Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
                        if (filename.Equals("iexplore"))
                        {
                            object url = group.Table.Rows[i]["URL"].ToString();
                            object oEmpty = "";
                            ie.Navigate2(ref url, ref oEmpty, ref oEmpty, ref oEmpty, ref oEmpty);
                            mshtml.IHTMLDocument2 htmlDoc = ie.Document as mshtml.IHTMLDocument2;

                            System.Threading.Thread.Sleep(20000);

                            try
                            {
                                pig = htmlDoc.body.outerHTML.Substring(0, htmlDoc.body.outerHTML.Length);
                            }
                            catch
                            {
                                pig = "";
                            }

                            pig = pig.Replace("\"", "").Replace("'", "");
                        }
                     }
/**/
                    //pig = HttpUtility.HtmlEncode(pig);
                    //L_M1.Text = i.ToString();
                    if (pig != "")
                    {
                        string ls_up = "UPDATE plan425_b1 SET HTML='" + pig + "' ,STATUS=1  WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);
                    }
                    pig = "";
                    //抓google
                    //System.Threading.Thread.Sleep(2000);
                    //UPDATE CMS_B2 SET kw=substring(name2,1,CHARINDEX(' ',name2))
                }
            }
            MessageBox.Show("ok!");
        }

        public void xxxxxxxxxxxxxx222()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;
            string ls_sql = "SELECT ID,  url_product as URL FROM plan425_b2    WHERE STATUS   =1 order by ID";//
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = "";
                    //pig = getUrlSource(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312
                    pig = getUrlSource_Adv(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312
                    if (pig != "")
                    {
                        string ls_up = "UPDATE plan425_b2 SET HTML='" + pig + "' ,STATUS=2  WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);
                    }
                    pig = "";
                }
            }
            MessageBox.Show("ok!");
        }

        /// <summary>
        /// 根据采集url地址以及编码方式获得网页源代码
        /// </summary>
        /// <param name="strUrl">信息采集地址 eg:www.sina.com</param>
        /// <param name="strEncoding">编码方式 eg:gb2312,utf-8 etc</param>
        /// <returns>网页源代码字符串</returns>
        public string getUrlSource(string strUrl, string strEncoding)
        {
            string lsResult;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strUrl);
                HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(rep.GetResponseStream(), Encoding.GetEncoding(strEncoding));

                lsResult = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                lsResult = "";
                Console.WriteLine(ex.Message);
            }
            return lsResult;
        }


        public string getUrlSource_proxy(string strUrl, string strEncoding)
        {
            string IP = "127.0.0.1";
            int PORRT = 7070;

            DataGroup group = new DataGroup();
            group = null;
            string ls_proxy = "SELECT ID,PROXY,IP,PORT FROM A_PROXY where status=1 and ok >0 and err <10 order by err,ok,id";
            group = DB.GetDataGroup(ls_proxy);
            if (group.Table.Rows.Count > 0)
            {
                IP = group.Table.Rows[0]["IP"].ToString();
                PORRT = int.Parse(group.Table.Rows[0]["PORT"].ToString().Trim());
            }


            string lsResult;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strUrl);


                //WebProxy myproxy = new WebProxy("61.220.57.86",3128);
                WebProxy myproxy = new WebProxy(IP, PORRT);
                req.Proxy = myproxy;
                //myProxy.Credentials = new NetworkCredential("username "," password "," domainname ");


                HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(rep.GetResponseStream(), Encoding.GetEncoding(strEncoding));

                lsResult = sr.ReadToEnd();


                string ls_sql = "update A_PROXY set ok=ok+1 where id='" + group.Table.Rows[0]["ID"].ToString() + "'";
                DB.ExecuteSQL(ls_sql);
                group = null;

            }

            catch (Exception ex)
            {
                string ls_sql = "update A_PROXY set err=err+1 where id='" + group.Table.Rows[0]["ID"].ToString() + "'";
                DB.ExecuteSQL(ls_sql);
                group = null;

                lsResult = "";
                Console.WriteLine(ex.Message);
            }
            return lsResult;
        }




        /// <summary>
        /// 根据采集url地址以及编码方式获得网页源代码
        /// </summary>
        /// <param name="strUrl">信息采集地址 eg:www.sina.com</param>
        /// <param name="strEncoding">编码方式 eg:gb2312,utf-8 etc</param>
        /// <returns>网页源代码字符串</returns>
        public string getUrlSource_Adv(string strUrl, string strEncoding)
        {
            string lsResult;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strUrl);
                req.Timeout = 50000;
                req.Headers.Set("Pragma", "no-cache");

                //req.Method = "Post";
                req.Method = "GET";

                //req.KeepAlive = false;

                /* list.Add("Opera/7.51 (Windows NT 5.1; U) [en]");
                list.Add("Opera/7.50 (Windows XP; U)");
                list.Add("Avant Browser/1.2.789rel1 (http://www.avantbrowser.com)");
                list.Add("Mozilla/5.0 (Windows; U; Win98; en-US; rv:1.4) Gecko Netscape/7.1 (ax)");
                list.Add("Mozilla/5.0 (Windows; U; Windows XP) Gecko MultiZilla/1.6.1.0a");
                list.Add("Opera/7.50 (Windows ME; U) [en]");
                list.Add("Mozilla/3.01Gold (Win95; I)");
                list.Add("Mozilla/2.02E (Win95; U)");
                list.Add("Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.9.0.19) Gecko/2010031422 Firefox/3.0.19 (.NET CLR 3.5.30729)");
                list.Add("Mozilla/5.0 (Macintosh; U; PPC Mac OS X; en) AppleWebKit/125.2 (KHTML, like Gecko) Safari/125.8");
                list.Add("Mozilla/5.0 (Macintosh; U; PPC Mac OS X; en) AppleWebKit/125.2 (KHTML, like Gecko) Safari/85.8");
                list.Add("Mozilla/4.0 (compatible; MSIE 5.15; Mac_PowerPC)");
                list.Add("Mozilla/5.0 (Macintosh; U; PPC Mac OS X Mach-O; en-US; rv:1.7a) Gecko/20050614 Firefox/0.9.0+");
                list.Add("Mozilla/5.0 (Macintosh; U; PPC Mac OS X; en-US) AppleWebKit/125.4 (KHTML, like Gecko, Safari) OmniWeb/v563.15");
                list.Add("Mozilla/5.0 (X11; U; Linux; i686; en-US; rv:1.6) Gecko Debian/1.6-7");
                list.Add("Mozilla/5.0 (X11; U; Linux; i686; en-US; rv:1.6) Gecko Epiphany/1.2.5");*/

                //req.ContentType = "application/octet-stream";

                req.ContentType = "application/x-www-form-urlencoded";
                req.Accept = "*/*";
                req.KeepAlive = true;
                //req.Headers.Add("Accept-Language", "zh-cn,en-us;q=0.5");
                req.AllowAutoRedirect = true;

                string f = req.Address.ToString();

                //"Googlebot";//
                req.UserAgent = "Googlebot";
                HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(rep.GetResponseStream(), Encoding.GetEncoding(strEncoding));

                lsResult = sr.ReadToEnd();


            }
            catch (Exception ex)
            {
                lsResult = "";
                Console.WriteLine(ex.Message);
            }
            return lsResult;
        }

        public static string getUrlSource2(string url, string Encodingstr)
        {

            WebBrowser myie = new WebBrowser();


            //webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);

            myie.Navigate(url);


            while (myie.ReadyState != WebBrowserReadyState.Complete)
            {

            }

            StreamReader sr = new StreamReader(myie.DocumentStream, Encoding.GetEncoding(Encodingstr));
            string html = sr.ReadToEnd();
            return html;
        }

        public string getUrlSource_google(string strUrl, string strEncoding)
        {
            //string PostUrl = strUrl.Replace("http://www.google.com/search" + "?", "");
            //strUrl = "http://www.google.com/search";

            string lsResult;
            try
            {
                ArrayList list = new ArrayList();
                list.Add("Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)");
                list.Add("Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)");
                list.Add("Mozilla/4.0 (compatible; MSIE 5.5; Windows NT 5.0 )");
                list.Add("Mozilla/4.0 (compatible; MSIE 5.5; Windows 98; Win 9x 4.90)");
                list.Add("Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.0.1) Gecko/2008070208 Firefox/3.0.1");
                list.Add("Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.14) Gecko/20080404 Firefox/2.0.0.14");
                list.Add("Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/525.13 (KHTML, like Gecko) Chrome/0.2.149.29 Safari/525.13");
                list.Add("Mozilla/4.8 [en] (Windows NT 6.0; U)");
                list.Add("Mozilla/4.8 [en] (Windows NT 5.1; U)");
                list.Add("Opera/9.25 (Windows NT 6.0; U; en)");
                list.Add("Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.0; en) Opera 8.0");
                list.Add("Opera/7.51 (Windows NT 5.1; U) [en]");
                list.Add("Opera/7.50 (Windows XP; U)");
                list.Add("Avant Browser/1.2.789rel1 (http://www.avantbrowser.com)");
                list.Add("Mozilla/5.0 (Windows; U; Win98; en-US; rv:1.4) Gecko Netscape/7.1 (ax)");
                list.Add("Mozilla/5.0 (Windows; U; Windows XP) Gecko MultiZilla/1.6.1.0a");
                list.Add("Opera/7.50 (Windows ME; U) [en]");
                list.Add("Mozilla/3.01Gold (Win95; I)");
                list.Add("Mozilla/2.02E (Win95; U)");
                list.Add("Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US; rv:1.9.0.19) Gecko/2010031422 Firefox/3.0.19 (.NET CLR 3.5.30729)");
                list.Add("Mozilla/5.0 (Macintosh; U; PPC Mac OS X; en) AppleWebKit/125.2 (KHTML, like Gecko) Safari/125.8");
                list.Add("Mozilla/5.0 (Macintosh; U; PPC Mac OS X; en) AppleWebKit/125.2 (KHTML, like Gecko) Safari/85.8");
                list.Add("Mozilla/4.0 (compatible; MSIE 5.15; Mac_PowerPC)");
                list.Add("Mozilla/5.0 (Macintosh; U; PPC Mac OS X Mach-O; en-US; rv:1.7a) Gecko/20050614 Firefox/0.9.0+");
                list.Add("Mozilla/5.0 (Macintosh; U; PPC Mac OS X; en-US) AppleWebKit/125.4 (KHTML, like Gecko, Safari) OmniWeb/v563.15");
                list.Add("Mozilla/5.0 (X11; U; Linux; i686; en-US; rv:1.6) Gecko Debian/1.6-7");
                list.Add("Mozilla/5.0 (X11; U; Linux; i686; en-US; rv:1.6) Gecko Epiphany/1.2.5");
                list.Add("Mozilla/5.0 (X11; U; Linux i586; en-US; rv:1.7.3) Gecko/20050924 Epiphany/1.4.4 (Ubuntu)");
                list.Add("Mozilla/5.0 (compatible; Konqueror/3.5; Linux) KHTML/3.5.10 (like Gecko) (Kubuntu)");
                list.Add("Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.8.1.19) Gecko/20081216 Ubuntu/8.04 (hardy) Firefox/2.0.0.19");
                list.Add("Mozilla/5.0 (X11; U; Linux; i686; en-US; rv:1.6) Gecko Galeon/1.3.14");
                list.Add("Konqueror/3.0-rc4; (Konqueror/3.0-rc4; i686 Linux;;datecode)");
                list.Add("Mozilla/5.0 (compatible; Konqueror/3.3; Linux 2.6.8-gentoo-r3; X11;");
                list.Add("Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.6) Gecko/20050614 Firefox/0.8");
                list.Add("ELinks/0.9.3 (textmode; Linux 2.6.9-kanotix-8 i686; 127x41)");
                list.Add("ELinks (0.4pre5; Linux 2.6.10-ac7 i686; 80x33)");
                list.Add("Links (2.1pre15; Linux 2.4.26 i686; 158x61)");
                list.Add("Links/0.9.1 (Linux 2.4.24; i386;)");
                list.Add("MSIE (MSIE 6.0; X11; Linux; i686) Opera 7.23");
                list.Add("Opera/9.52 (X11; Linux i686; U; en)");
                list.Add("Lynx/2.8.5rel.1 libwww-FM/2.14 SSL-MM/1.4.1 GNUTLS/0.8.12");
                list.Add("w3m/0.5.1");
                list.Add("Links (2.1pre15; FreeBSD 5.3-RELEASE i386; 196x84)");
                list.Add("Mozilla/5.0 (X11; U; FreeBSD; i386; en-US; rv:1.7) Gecko");
                list.Add("Mozilla/4.77 [en] (X11; I; IRIX;64 6.5 IP30)");
                list.Add("Mozilla/4.8 [en] (X11; U; SunOS; 5.7 sun4u)");
                list.Add("Mozilla/3.0 (compatible; NetPositive/2.1.1; BeOS)");

                Random ra = new Random();
                int num = 47;
                int value = ra.Next(num);


                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strUrl);

                /*
                WebRequest _HWR = WebRequest.CreateDefault(new System.Uri(URL));
                WebResponse _HRS = _HWR.GetResponse() ;
                Stream ReceiveStream = _HRS.GetResponseStream();

                 2
                 
                WebProxy _WP = new WebProxy(ProxyName, ProxyPort);
                _WP.BypassProxyOnLocal = true;
                ICredentials credentials = new NetworkCredential(UserName, UserKey, DomainName);
                _WP.Credentials = credentials;

                WebRequest _HWR = WebRequest.CreateDefault(new System.Uri(URL));
                _HWR.Proxy = _WP;
                WebResponse _HRS = _HWR.GetResponse();
                */

                WebProxy _WP = new WebProxy("127.0.0.1", 1008);
                _WP.BypassProxyOnLocal = true;
                ICredentials credentials = new NetworkCredential("user", "pwd");
                _WP.Credentials = credentials;
                req.Proxy = _WP;


                //string param = PostUrl; 
                //byte[] bs = Encoding.ASCII.GetBytes(param); 
                //req.Method = "POST";
                //req.ContentType = "application/x-www-form-urlencoded";
                //req.ContentLength = bs.Length; 
                req.UserAgent = list[value].ToString();


                HttpWebResponse rep = (HttpWebResponse)req.GetResponse();

                StreamReader sr = new StreamReader(rep.GetResponseStream(), Encoding.GetEncoding(strEncoding));

                lsResult = sr.ReadToEnd();

            }
            catch (Exception ex)
            {
                lsResult = "";
                Console.WriteLine(ex.Message);
            }
            return lsResult;
        }

        /*
                        //HttpWebResponse orsResponse = (HttpWebResponse)req.GetResponse();
                        //using (Stream responseStream = orsResponse.GetResponseStream())
                        //{
                        //    using (StreamReader streamRead = new StreamReader(responseStream, Encoding.UTF8))
                        //    {
                        //        lsResult = streamRead.ReadToEnd();
                        //    }
                        //}


                        //using (Stream reqStream = req.GetRequestStream())
                        //{
                        //    reqStream.Write(bs, 0, bs.Length);
                        //}
                        //using (WebResponse wr = req.GetResponse())
                        //{
                        //    //在这里对接收到的页面内容进行处理
                        //    lsResult = ""; ;
                        //}*/

        public string getUrl301(string strUrl, string strEncoding)
        {
            string lsResult;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strUrl);

                HttpWebResponse rep = (HttpWebResponse)req.GetResponse();

                //StreamReader sr = new StreamReader(rep.GetResponseStream(), Encoding.GetEncoding(strEncoding));

                //lsResult = sr.ReadToEnd();

                lsResult = req.Address.ToString();
            }
            catch (Exception ex)
            {
                lsResult = "";
                Console.WriteLine(ex.Message);
            }
            return lsResult;
        }


        private void label6_Click(object sender, EventArgs e)
        {

        }


        private void button24_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    l1.Text = OperateStr(t0.Text.Trim(), t1.Text.Trim(), t2.Text.Trim(), t3.Text.Trim(), int.Parse(o1.Text.Trim()), 1, 1);
            //}
            //catch { }

            //-----------------------------------------------------------------------------------------------------
            DataGroup group = new DataGroup();
            group = null;
            //DataEntity de = new DataEntity();
            //de.RemoveAll();
            string ls_sql = "SELECT RID,FILED,OID,CYCLE,STRING_S,STRING_E,STRING_NEW,INCLUDE_S,INCLUDE_E FROM URL_RULE WHERE STATUS=0 OR STATUS IS NULL";
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    PIG(group.Table.Rows[i]["RID"].ToString(), group.Table.Rows[i]["FILED"].ToString(),
                        group.Table.Rows[i]["OID"].ToString(), group.Table.Rows[i]["CYCLE"].ToString(),
                        group.Table.Rows[i]["STRING_S"].ToString(), group.Table.Rows[i]["STRING_E"].ToString(),
                        group.Table.Rows[i]["STRING_NEW"].ToString(),
                        group.Table.Rows[i]["INCLUDE_S"].ToString(), group.Table.Rows[i]["INCLUDE_E"].ToString()
                        );

                }
            }
            group = null;
            //-----------------------------------------------------------------------------------------------------



        }


        public void PIG(string RID, string FILED, string OID, string CYCLE, string STRING_S, string STRING_E, string STRING_NEW, string INCLUDE_S, string INCLUDE_E)
        {
            //-----------------------------------------------------------------------------------------------------
            DataGroup group = new DataGroup();
            group = null;
            string ls_sql = "SELECT ID,HTML FROM URL_BASE WHERE  STATUS =1 order by ID";//id=7019 and
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string dog = OperateStr(group.Table.Rows[i]["HTML"].ToString().Replace("'", "\""), STRING_S, STRING_E, STRING_NEW, int.Parse(OID), int.Parse(INCLUDE_S), int.Parse(INCLUDE_E), CYCLE);

                    string ls_up = "UPDATE URL_BASE SET " + FILED + "='" + dog + "' ,STATUS=1 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                    DB.ExecuteSQL(ls_up);

                    dog = "";

                }
            }
            group = null;
            //-----------------------------------------------------------------------------------------------------
        }


        //pig = HttpUtility.HtmlEncode(pig);
        public void yyyyyyyyyyyyyyyyyyyyy()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;


            string ls_sql = "SELECT ID,HTML,TITLE_S,TITLE_E FROM URL_BASE WHERE STATUS =1 order by ID";
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string dog = OperateStr(group.Table.Rows[i]["HTML"].ToString(), group.Table.Rows[i]["TITLE_S"].ToString(), group.Table.Rows[i]["TITLE_E"].ToString(), "", 1, 0, 0, "0");

                    string ls_up = "UPDATE URL_BASE SET TITLE='" + dog + "' ,STATUS=1 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                    DB.ExecuteSQL(ls_up);

                    dog = "";

                }
            }



        }

        /// <summary>
        /// 根据各个操作类型处理字符串 //1://截取  2://删除段  3://删除字符串  4://字符串前缀  5://段落前缀  6://字符串后缀  7://段落后缀  8://替换字符串  9://替换段落
        /// </summary>
        /// <param name="str">要处理的字符串</param>
        /// <param name="startstr">开始字符串</param>
        /// <param name="endstr">结束字符串</param>
        /// <param name="Newstr">替换/前缀/后缀的字符串</param>
        /// <param name="operate">操作类型</param>
        /// <param name="inHead">包含头1，不包含头0</param>
        /// <param name="inTail">包含尾1，不包含尾0</param>
        /// <returns>返回处理过后的字符串</returns>
        private string OperateStr(string str, string startstr, string endstr, string Newstr, int operate, int inHead, int inTail, string CYCLE)
        {
            int intLength = str.Length;//【HTML总长度】
            int startIndex = str.IndexOf(startstr);//【开始字符串】在【HTML】位置


            if (startIndex == -1)
            {
                return "";
            }

            //int endIndex = str.IndexOf(endstr);
            int x = startstr.Length;//【开始字符串】的长度
            string cat = str.Substring(0, startIndex + x);//从【开头】到【开始字符串】

            cat = str.Replace(cat, "");//【目标字符串】后到【尾巴】
            int end_len = cat.IndexOf(endstr);//【目标字符串】的长度

            int endIndex = cat.IndexOf(endstr);


            int KEY_INDEX = startIndex + startstr.Length + endIndex + endstr.Length;



            int CCC = 0;
            if (CYCLE == "1")
            {
                //Regex r = new Regex(startstr); // 定义一个Regex对象实例
                //Match m = r.Match(str); // 在字符串中匹配
                //if (m.Success)
                //{
                //    CCC = m.Length;
                //}

                CCC = (str.Length - str.Replace(startstr, String.Empty).Length) / startstr.Length;
            }




            switch (operate)
            {
                case 1://截取
                    //if (endIndex <= startIndex || startIndex == -1)
                    //    return "";

                    if (inHead == 1 && inTail == 1)//表示包含头尾,保证能找到尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex + endstr.Length);

                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        //return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length);

                        if (CCC == 0)
                        {
                            return str.Substring(startIndex + startstr.Length, end_len);
                        }
                        else
                        {
                            string wolf = "";
                            wolf = "" + str.Substring(startIndex + startstr.Length, end_len);

                            for (int i = 1; i < CCC; i++)
                            {
                                //------------重新定位【开始长度位】--------------
                                //--------------【KEY_INDEX】---------------------
                                //------------先替换掉处理的【HTML】--------------

                                //int temp_index = startIndex + startstr.Length + end_len + endstr.Length+337 ;

                                cat = str.Substring(0, KEY_INDEX);
                                cat = str.Replace(cat, "");//【新世纪】

                                startIndex = cat.IndexOf(startstr);

                                string mouse = cat.Substring(0, startIndex + startstr.Length);//从【开头】到【开始字符串】
                                mouse = cat.Replace(mouse, "");
                                endIndex = mouse.IndexOf(endstr);

                                wolf += "|" + cat.Substring(startIndex + startstr.Length, endIndex);

                                KEY_INDEX += startIndex + startstr.Length + endIndex + endstr.Length;
                            }

                            return wolf;
                        }

                    }
                    break;
                case 2://删除段
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), "");
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), "");
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), "");
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), "");
                    }
                    break;
                case 3://删除字符串
                    return str.Replace(startstr, "");
                    break;
                case 4://字符串前缀
                    return str.Replace(startstr, Newstr + startstr);
                    break;
                case 5://段落前缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;
                    if (inHead == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex));

                    if (inHead == 0)
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr + str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length));
                    break;
                case 6://字符串后缀
                    return str.Replace(startstr, startstr + Newstr);
                    break;
                case 7://段落后缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inTail == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), str.Substring(startIndex, endIndex - startIndex + endstr.Length) + Newstr);

                    if (inTail == 0)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex) + Newstr);
                    break;
                case 8://替换字符串
                    return str.Replace(startstr, Newstr);
                    break;
                case 9://替换段落
                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), Newstr);
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), Newstr);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr);
                    }

                    break;
            }
            return str;
        }




        private void button25_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(zzzzzzzzzzzz);
            thread.IsBackground = true;
            thread.Start();
        }
        public void zzzzzzzzzzzz()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;


            string ls_sql = "SELECT ID,PIC_MAIN FROM URL_BASE WHERE STATUS=1 order by ID";//STATUS IS NULL OR STATUS !=1
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                //L_M2.Text = group.Table.Rows.Count.ToString();

                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = getUrlSource(group.Table.Rows[i]["PIC_MAIN"].ToString(), "utf-8").Replace("'", "\"");

                    //pig = HttpUtility.HtmlEncode(pig);

                    //L_M1.Text = i.ToString();

                    string ls_up = "UPDATE URL_BASE SET PIC_MAIN_HTML='" + pig + "' ,STATUS=1 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                    DB.ExecuteSQL(ls_up);

                    pig = "";

                }
            }



        }

        private void button26_Click(object sender, EventArgs e)
        {
            //-----------------------------------------------------------------------------------------------------
            DataGroup group = new DataGroup();
            group = null;
            //DataEntity de = new DataEntity();
            //de.RemoveAll();
            string ls_sql = "SELECT RID,FILED,OID,CYCLE,STRING_S,STRING_E,STRING_NEW,INCLUDE_S,INCLUDE_E FROM URL_RULE WHERE siteid=2 and STATUS=0 OR STATUS IS NULL";
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    PIG2(group.Table.Rows[i]["RID"].ToString(), group.Table.Rows[i]["FILED"].ToString(),
                        group.Table.Rows[i]["OID"].ToString(), group.Table.Rows[i]["CYCLE"].ToString(),
                        group.Table.Rows[i]["STRING_S"].ToString(), group.Table.Rows[i]["STRING_E"].ToString(),
                        group.Table.Rows[i]["STRING_NEW"].ToString(),
                        group.Table.Rows[i]["INCLUDE_S"].ToString(), group.Table.Rows[i]["INCLUDE_E"].ToString()
                        );

                }
            }
            group = null;
            //-----------------------------------------------------------------------------------------------------

        }


        public void PIG2(string RID, string FILED, string OID, string CYCLE, string STRING_S, string STRING_E, string STRING_NEW, string INCLUDE_S, string INCLUDE_E)
        {
            //-----------------------------------------------------------------------------------------------------
            DataGroup group = new DataGroup();
            group = null;
            string ls_sql = "SELECT ID,PIC_MAIN_HTML FROM URL_BASE WHERE  STATUS =1 order by ID";//id=7019 and
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string dog = OperateStr(group.Table.Rows[i]["PIC_MAIN_HTML"].ToString().Replace("'", "\""), STRING_S, STRING_E, STRING_NEW, int.Parse(OID), int.Parse(INCLUDE_S), int.Parse(INCLUDE_E), CYCLE);

                    string ls_up = "UPDATE URL_BASE SET " + FILED + "='" + dog + "' ,STATUS=1 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                    DB.ExecuteSQL(ls_up);

                    dog = "";

                }
            }
            group = null;
            //-----------------------------------------------------------------------------------------------------
        }





        public void wolf(string RID, string FILED, string OID, string CYCLE, string STRING_S, string STRING_E, string STRING_NEW, string INCLUDE_S, string INCLUDE_E, string STR_AREA, string STR_AFT, string STR_BEF)
        {
            //-----------------------------------------------------------------------------------------------------
            DataGroup group = new DataGroup();
            group = null;
            string ls_sql = "SELECT ID,HTML FROM URL_JADE WHERE  STATUS =0 order by ID";//id=7019 and
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string dog = OperateStr_Adv(group.Table.Rows[i]["HTML"].ToString().Replace("'", "\""), STRING_S, STRING_E, STRING_NEW, int.Parse(OID), int.Parse(INCLUDE_S), int.Parse(INCLUDE_E), CYCLE, STR_AREA, STR_AFT, STR_BEF);

                    string[] dog_small = dog.Split('|');
                    foreach (string bb in dog_small)
                    {
                        //string ls_up = "UPDATE URL_BASE SET " + FILED + "='" + dog + "' ,STATUS=1 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        string ls_up = "insert into url_jade (" + FILED + ",levelid,p_id,status) values ('" + bb + "',1,'" + group.Table.Rows[i]["ID"].ToString() + "',1)";
                        DB.ExecuteSQL(ls_up);
                    }
                    dog = "";

                }
            }
            group = null;
            //-----------------------------------------------------------------------------------------------------
        }

        private string OperateStr_Adv_A(string str, string startstr, string endstr, string Newstr, int operate, int inHead, int inTail, string CYCLE, string STR_AREA, string STR_AFT, string STR_BEF)
        {
            //先截断字符----------------------------------------------------------------
            if (STR_AREA == "1")
            {
                int temp_left = str.IndexOf(STR_AFT);
                temp_left += STR_AFT.Length;
                str = str.Replace(str.Substring(0, temp_left), "");

            }
            //--------------------------------------------------------------------------

            //.Replace((char)13, (char)0).ToString().Replace((char)10, (char)0)

            str = str.Replace("\r\n", "");// 去掉换行符
            //.Replace((char)13, (char)0);//.ToString().Replace((char)10, (char)0);

            int intLength = str.Length;//【HTML总长度】
            int startIndex = str.IndexOf(startstr);//【开始字符串】在【HTML】位置

            //　string tempStr = newStr.Replace((char)13, (char)0);

            //return tempStr.Replace((char)10, (char)0);



            if (startIndex == -1)
            {
                return "";
            }

            //int endIndex = str.IndexOf(endstr);
            int x = startstr.Length;//【开始字符串】的长度
            string cat = str.Substring(0, startIndex + x);//从【开头】到【开始字符串】

            cat = str.Replace(cat, "");//【目标字符串】后到【尾巴】
            int end_len = cat.IndexOf(endstr);//【目标字符串】的长度

            int endIndex = cat.IndexOf(endstr);


            int KEY_INDEX = startIndex + startstr.Length + endIndex + endstr.Length;



            int CCC = 0;
            if (CYCLE == "1")
            {
                //Regex r = new Regex(startstr); // 定义一个Regex对象实例
                //Match m = r.Match(str); // 在字符串中匹配
                //if (m.Success)
                //{
                //    CCC = m.Length;
                //}

                CCC = (str.Length - str.Replace(startstr, String.Empty).Length) / startstr.Length;
            }




            switch (operate)
            {
                case 1://截取
                    //if (endIndex <= startIndex || startIndex == -1)
                    //    return "";

                    if (inHead == 1 && inTail == 1)//表示包含头尾,保证能找到尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex + endstr.Length);

                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        //return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length);

                        if (CCC == 0)
                        {
                            return str.Substring(startIndex + startstr.Length, end_len);
                        }
                        else
                        {
                            string wolf = "";
                            wolf = "" + str.Substring(startIndex + startstr.Length, end_len);

                            for (int i = 1; i < CCC; i++)
                            {
                                //------------重新定位【开始长度位】--------------
                                //--------------【KEY_INDEX】---------------------
                                //------------先替换掉处理的【HTML】--------------

                                //int temp_index = startIndex + startstr.Length + end_len + endstr.Length+337 ;

                                cat = str.Substring(0, KEY_INDEX);
                                cat = str.Replace(cat, "");//【新世纪】

                                startIndex = cat.IndexOf(startstr);

                                string mouse = cat.Substring(0, startIndex + startstr.Length);//从【开头】到【开始字符串】
                                mouse = cat.Replace(mouse, "");
                                endIndex = mouse.IndexOf(endstr);

                                wolf += "|" + cat.Substring(startIndex + startstr.Length, endIndex);

                                KEY_INDEX += startIndex + startstr.Length + endIndex + endstr.Length;
                            }

                            wolf = wolf.Replace("||", "|");
                            //wolf=wolf.Substring(1, wolf.Length-1);
                            return wolf;
                        }

                    }
                    break;
                case 2://删除段
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), "");
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), "");
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), "");
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), "");
                    }
                    break;
                case 3://删除字符串
                    return str.Replace(startstr, "");
                    break;
                case 4://字符串前缀
                    return str.Replace(startstr, Newstr + startstr);
                    break;
                case 5://段落前缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;
                    if (inHead == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex));

                    if (inHead == 0)
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr + str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length));
                    break;
                case 6://字符串后缀
                    return str.Replace(startstr, startstr + Newstr);
                    break;
                case 7://段落后缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inTail == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), str.Substring(startIndex, endIndex - startIndex + endstr.Length) + Newstr);

                    if (inTail == 0)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex) + Newstr);
                    break;
                case 8://替换字符串
                    return str.Replace(startstr, Newstr);
                    break;
                case 9://替换段落
                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), Newstr);
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), Newstr);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr);
                    }

                    break;
            }
            return str;
        }

        private string OperateStr_Adv(string str, string startstr, string endstr, string Newstr, int operate, int inHead, int inTail, string CYCLE, string STR_AREA, string STR_AFT, string STR_BEF)
        {
            //先截断字符----------------------------------------------------------------
            if (STR_AREA == "1")
            {
                str = str.Replace("\t", "");

                int temp_left = str.IndexOf(STR_AFT);

                if (temp_left == -1)
                {
                    return "";
                }
                temp_left += STR_AFT.Length;
                str = str.Replace(str.Substring(0, temp_left), "");

                int temp_right = str.IndexOf(STR_BEF);
                str = str.Substring(0, temp_right).Replace("\t", "");//////////////.Replace("</option>", "").Replace("<option ", "") + "value=";

            }
            //--------------------------------------------------------------------------


            int intLength = str.Length;//【HTML总长度】
            int startIndex = str.IndexOf(startstr);//【开始字符串】在【HTML】位置


            if (startIndex == -1)
            {
                return "";
            }

            //int endIndex = str.IndexOf(endstr);
            int x = startstr.Length;//【开始字符串】的长度
            string cat = str.Substring(0, startIndex + x);//从【开头】到【开始字符串】

            cat = str.Replace(cat, "");//【目标字符串】后到【尾巴】
            int end_len = cat.IndexOf(endstr);//【目标字符串】的长度

            int endIndex = cat.IndexOf(endstr);


            int KEY_INDEX = startIndex + startstr.Length + endIndex + endstr.Length;



            int CCC = 0;
            if (CYCLE == "1")
            {
                //Regex r = new Regex(startstr); // 定义一个Regex对象实例
                //Match m = r.Match(str); // 在字符串中匹配
                //if (m.Success)
                //{
                //    CCC = m.Length;
                //}

                CCC = (str.Length - str.Replace(startstr, String.Empty).Length) / startstr.Length;
            }




            switch (operate)
            {
                case 1://截取
                    //if (endIndex <= startIndex || startIndex == -1)
                    //    return "";

                    if (inHead == 1 && inTail == 1)//表示包含头尾,保证能找到尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex + endstr.Length);

                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        //return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length);

                        if (CCC == 0)
                        {
                            return str.Substring(startIndex + startstr.Length, end_len);
                        }
                        else
                        {
                            string wolf = "";
                            wolf = "" + str.Substring(startIndex + startstr.Length, end_len).Trim();

                            for (int i = 1; i < CCC; i++)
                            {
                                //------------重新定位【开始长度位】--------------
                                //--------------【KEY_INDEX】---------------------
                                //------------先替换掉处理的【HTML】--------------

                                //int temp_index = startIndex + startstr.Length + end_len + endstr.Length+337 ;

                                cat = str.Substring(0, KEY_INDEX);
                                cat = str.Replace(cat, "");//【新世纪】

                                startIndex = cat.IndexOf(startstr);

                                string mouse = cat.Substring(0, startIndex + startstr.Length);//从【开头】到【开始字符串】
                                mouse = cat.Replace(mouse, "");
                                endIndex = mouse.IndexOf(endstr);

                                wolf += "|" + cat.Substring(startIndex + startstr.Length, endIndex).Trim();

                                KEY_INDEX += startIndex + startstr.Length + endIndex + endstr.Length;
                            }

                            return wolf;
                        }

                    }
                    break;
                case 2://删除段
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), "");
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), "");
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), "");
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), "");
                    }
                    break;
                case 3://删除字符串
                    return str.Replace(startstr, "");
                    break;
                case 4://字符串前缀
                    return str.Replace(startstr, Newstr + startstr);
                    break;
                case 5://段落前缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;
                    if (inHead == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex));

                    if (inHead == 0)
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr + str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length));
                    break;
                case 6://字符串后缀
                    return str.Replace(startstr, startstr + Newstr);
                    break;
                case 7://段落后缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inTail == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), str.Substring(startIndex, endIndex - startIndex + endstr.Length) + Newstr);

                    if (inTail == 0)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex) + Newstr);
                    break;
                case 8://替换字符串
                    return str.Replace(startstr, Newstr);
                    break;
                case 9://替换段落
                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), Newstr);
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), Newstr);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr);
                    }

                    break;
            }
            return str;
        }

        private void button28_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(CCCCCCCC);
            thread.IsBackground = true;
            thread.Start();
        }
        public void CCCCCCCC()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;


            string ls_sql = "SELECT ID,URL FROM CC02 WHERE STATUS IS NULL OR STATUS !=1 order by ID";
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                //L_M2.Text = group.Table.Rows.Count.ToString();

                for (int i = 0; i < group.Table.Rows.Count; i++)
                {

                    string pig = "";

                    try
                    {
                        pig = getUrlSource(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");
                    }
                    catch
                    {
                        pig = "";
                    }
                    //pig = HttpUtility.HtmlEncode(pig);

                    //L_M1.Text = i.ToString();

                    string ls_up = "UPDATE CC02 SET HTML='" + pig + "' ,STATUS=1 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                    DB.ExecuteSQL(ls_up);

                    pig = "";

                }
            }



        }

        private void button29_Click(object sender, EventArgs e)
        {

        }



        private string OperateStr_Adv_html(string str, string startstr, string endstr, string Newstr, int operate, int inHead, int inTail, string CYCLE, string STR_AREA, string STR_AFT, string STR_BEF)
        {
            //先截断字符----------------------------------------------------------------
            if (STR_AREA == "1")
            {
                int temp_left = str.IndexOf(STR_AFT);
                temp_left += STR_AFT.Length;
                str = str.Replace(str.Substring(0, temp_left), "");

            }
            //--------------------------------------------------------------------------


            int intLength = str.Length;//【HTML总长度】
            int startIndex = str.IndexOf(startstr);//【开始字符串】在【HTML】位置


            if (startIndex == -1)
            {
                return "";
            }

            //int endIndex = str.IndexOf(endstr);
            int x = startstr.Length;//【开始字符串】的长度
            string cat = str.Substring(0, startIndex + x);//从【开头】到【开始字符串】

            cat = str.Replace(cat, "");//【目标字符串】后到【尾巴】
            int end_len = cat.IndexOf(endstr);//【目标字符串】的长度

            int endIndex = cat.IndexOf(endstr);


            int KEY_INDEX = startIndex + startstr.Length + endIndex + endstr.Length;



            int CCC = 0;
            if (CYCLE == "1")
            {
                //Regex r = new Regex(startstr); // 定义一个Regex对象实例
                //Match m = r.Match(str); // 在字符串中匹配
                //if (m.Success)
                //{
                //    CCC = m.Length;
                //}

                CCC = (str.Length - str.Replace(startstr, String.Empty).Length) / startstr.Length;
            }




            switch (operate)
            {
                case 1://截取
                    //if (endIndex <= startIndex || startIndex == -1)
                    //    return "";

                    if (inHead == 1 && inTail == 1)//表示包含头尾,保证能找到尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex + endstr.Length);

                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        //return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length);

                        if (CCC == 0)
                        {
                            try
                            {
                                return str.Substring(startIndex + startstr.Length, end_len);
                            }
                            catch
                            {
                                return null;
                            }

                        }
                        else
                        {
                            string wolf = "";
                            wolf = "" + str.Substring(startIndex + startstr.Length, end_len);

                            for (int i = 1; i < CCC; i++)
                            {
                                //------------重新定位【开始长度位】--------------
                                //--------------【KEY_INDEX】---------------------
                                //------------先替换掉处理的【HTML】--------------

                                //int temp_index = startIndex + startstr.Length + end_len + endstr.Length+337 ;

                                cat = str.Substring(0, KEY_INDEX);
                                cat = str.Replace(cat, "");//【新世纪】

                                startIndex = cat.IndexOf(startstr);

                                string mouse = cat.Substring(0, startIndex + startstr.Length);//从【开头】到【开始字符串】
                                mouse = cat.Replace(mouse, "");
                                endIndex = mouse.IndexOf(endstr);

                                wolf += "\r\n" + cat.Substring(startIndex + startstr.Length, endIndex);

                                KEY_INDEX += startIndex + startstr.Length + endIndex + endstr.Length;
                            }

                            wolf = wolf.Replace("\r\n\r\n", "\r\n");
                            //wolf=wolf.Substring(1, wolf.Length-1);
                            return wolf;
                        }

                    }
                    break;
                case 2://删除段
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), "");
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), "");
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), "");
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), "");
                    }
                    break;
                case 3://删除字符串
                    return str.Replace(startstr, "");
                    break;
                case 4://字符串前缀
                    return str.Replace(startstr, Newstr + startstr);
                    break;
                case 5://段落前缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;
                    if (inHead == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex));

                    if (inHead == 0)
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr + str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length));
                    break;
                case 6://字符串后缀
                    return str.Replace(startstr, startstr + Newstr);
                    break;
                case 7://段落后缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inTail == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), str.Substring(startIndex, endIndex - startIndex + endstr.Length) + Newstr);

                    if (inTail == 0)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex) + Newstr);
                    break;
                case 8://替换字符串
                    return str.Replace(startstr, Newstr);
                    break;
                case 9://替换段落
                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), Newstr);
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), Newstr);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr);
                    }

                    break;
            }
            return str;
        }

        private void B_Read_Click(object sender, EventArgs e)
        {
            return;

            //B_Read


            int cat = 0;
            //try
            //{
            //    cat = int.Parse(t_cat.Text.Trim());
            //}
            //catch
            //{
            //    cat = 0;
            //}




            DataGroup group = new DataGroup();
            group = null;
            string ls_sql = "SELECT html FROM URL_COCO WHERE STATUS=4  and id ='" + cat + "'";
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                l_html.Text = group.Table.Rows[0]["html"].ToString();
            }


        }

        private void button30_Click(object sender, EventArgs e)
        {
            l_html.Text = IsGanBr(l_html.Text);
            l_html.Text = l_html.Text.Replace("\r\n", "");
        }

        private void button31_Click(object sender, EventArgs e)
        {
            // DataGroup temp = new DataGroup();
            //temp=null;


            DataGroup group = new DataGroup();

            DataGroup group_html = new DataGroup();
            group_html = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group_html = null;

            string ls_sql = "SELECT ID,HTML FROM plan425_b1 WHERE STATUS =1 and flag is null  order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["HTML"].ToString().Replace("\r\n", "").ToString();//替换换行符
                    ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("<DL>", "|");//将分隔码 替换成“|”

                    //去掉后面的代码  
                    //int temp_right = ls_shtml.IndexOf("<!-- bof: whats_new -->");
                    //ls_shtml = ls_shtml.Substring(0, temp_right);


                    string[] dog_small = ls_shtml.Split('|');//子串

                    int pd = 0;

                    foreach (string bb in dog_small)
                    {
                        if (pd > 0)
                        {

                            string ls_FILED = "";
                            string ls_Value = "";

                            //-----------------------------------------------------------------------------------------------------
                            group = null;
                            string ls_rule = "SELECT RID,FILED,OID,CYCLE,STRING_S,STRING_E,STRING_NEW,INCLUDE_S,INCLUDE_E FROM URL_RULE WHERE siteid=15101 and STATUS=1";
                            group = DB.GetDataGroup(ls_rule);
                            if (group.Table.Rows.Count > 0)
                            {
                                for (int j = 0; j < group.Table.Rows.Count; j++)
                                {
                                    try
                                    {
                                        ls_Value += ",'" + OperateStr(bb, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, "0") + "'";
                                    }
                                    catch
                                    {
                                        ls_Value = "," + " ";
                                    }

                                    ls_FILED += "," + group.Table.Rows[j]["FILED"].ToString();


                                }

                                //picknikes   kicksonnike
                                //ls_Value = ls_Value.Replace("picknikes.com", "kicksonnike.com");
                                //temp = null;
                                //string ls_temp = "select id from KICK_N2 where url_product='" + ls_Value + "'";
                                //ls_temp = ls_temp.Replace(",'", "");
                                //ls_temp = ls_temp.Replace("''", "'");
                                //temp = DB.GetDataGroup(ls_temp);

                                //if (temp.RecCount == 0)
                                //{
                                string ls_up = "INSERT INTO plan425_b2 (PID " + ls_FILED + ",STATUS) values ('" + group_html.Table.Rows[i]["ID"].ToString() + "' " + ls_Value + ",1)";
                                DB.ExecuteSQL(ls_up);

                                //}
                                //else
                                //{


                                //    string ls_up = "update KICK_N2 set status=9 where url_product='" + ls_Value + "'";
                                //    ls_up = ls_up.Replace(",'", "");
                                //    ls_up = ls_up.Replace("''", "'");
                                //    DB.ExecuteSQL(ls_up);

                                //}

                                ls_Value = "";
                                ls_FILED = "";


                            }
                            group = null;
                            //-----------------------------------------------------------------------------------------------------
                        }
                        pd += 1;
                    }





                }

                MessageBox.Show("OK!!");
            }



        }

        private void button32_Click(object sender, EventArgs e)
        {
            try
            {
                DataGroup group = new DataGroup();

                DataGroup group_html = new DataGroup();
                group_html = null;
                DataEntity de = new DataEntity();
                de.RemoveAll();
                group_html = null;

                string ls_sql = "SELECT  ID,html as HTML FROM plan425_b2 WHERE STATUS =2    order by ID"; //and price =''
                group_html = DB.GetDataGroup(ls_sql);
                if (group_html.Table.Rows.Count > 0)
                {

                    for (int i = 0; i < group_html.Table.Rows.Count; i++)
                    {
                        string ls_shtml = group_html.Table.Rows[i]["HTML"].ToString().Replace("\r\n", "").ToString();//替换换行符
                        ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                        ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”


                        //string ls_FILED = "";
                        string ls_Value = "";
                        //-----------------------------------------------------------------------------------------------------
                        group = null;
                        string ls_rule = "SELECT RID,FILED,OID,CYCLE,STRING_S,STRING_E,STRING_NEW,INCLUDE_S,INCLUDE_E,STR_AREA,STR_AFT,STR_BEF FROM URL_RULE WHERE siteid=34710 and STATUS=1";
                        group = DB.GetDataGroup(ls_rule);
                        if (group.Table.Rows.Count > 0)
                        {
                            for (int j = 0; j < group.Table.Rows.Count; j++)
                            {
                                try
                                {

                                    ls_Value = OperateStr_Adv(ls_shtml, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, group.Table.Rows[j]["CYCLE"].ToString(), group.Table.Rows[j]["STR_AREA"].ToString(), group.Table.Rows[j]["STR_AFT"].ToString(), group.Table.Rows[j]["STR_BEF"].ToString());

                                    ls_Value = NoHTML(ls_Value).Trim();
                                    //ls_Value = OperateStr(ls_shtml, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, group.Table.Rows[j]["CYCLE"].ToString());

                                }
                                catch
                                {
                                    ls_Value = "";
                                }

                                string ls_up = "UPDATE plan425_b2 SET " + group.Table.Rows[j]["FILED"].ToString() + "='" + ls_Value + "' ,STATUS=2  WHERE ID='" + group_html.Table.Rows[i]["ID"].ToString() + "'";

                                //string ls_up = "insert into plan425_b1 (pid,url) values ('" + group_html.Table.Rows[i]["ID"].ToString() + "','" + group.Table.Rows[j]["FILED"].ToString() + "')";

                                DB.ExecuteSQL(ls_up);
                            }


                        }
                        ls_shtml = null;
                        group = null;
                        //-----------------------------------------------------------------------------------------------------


                    }

                }

            }
            catch
            { }

            MessageBox.Show("OK");

        }

        private void button33_Click(object sender, EventArgs e)
        {
            /*
             insert into Asics_pic (pid,url_pic,flag,status)
             select id,'http://www.asicsshoesmart.com/'+pic_s,'S',1 from Asics_b3
             * 
             * 
             
             insert into plan425_pic (pid,url_pic,flag,status)
             select id,pic_s,'S',0 from plan425_b2

             * 
             */
            DataGroup group = new DataGroup();

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group_html = null;

            string ls_sql = "SELECT ID,pic_group as pic_group FROM  plan425_b2 where status=2  order by ID";//WHERE STATUS =2
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["pic_group"].ToString();
                    string[] dog_small = ls_shtml.Split('|');

                    foreach (string bb in dog_small)
                    {
                        string xx = bb.Trim();
                        //string ls_up = "insert into ZY_PIC (pid,url_pic,flag,status) values ('" + group_html.Table.Rows[i]["ID"].ToString() + "','http://www.xxx.com/" + xx + "','B',1)";

                        group = null;
                        string ls_double = "select id from plan425_PIC where url_pic='" + xx + "' and status=1   ";
                        group = DB.GetDataGroup(ls_double);
                        if (group.Table.Rows.Count == 0)
                        {
                            string ls_up = "insert into plan425_PIC (pid,url_pic,flag,status) values ('" + group_html.Table.Rows[i]["ID"].ToString().Trim() + "','" + xx + "','B',0)";//http://www.xxx.com/
                            DB.ExecuteSQL(ls_up);
                        }

                    }



                }
            }

            MessageBox.Show("OK!");
        }

        private void button34_Click(object sender, EventArgs e)
        {
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    //string pig = getUrlSource(group.Table.Rows[i]["URL_PIC"].ToString(), "utf-8");
                    //string ls_up = "UPDATE JADE_PIC SET PD='" + pig + "' ,STATUS=2 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                    //DB.ExecuteSQL(ls_up);
                    //pig = "";

                    //string xx = "";

                    //xx=group.Table.Rows[i]["URL_PIC"].ToString();

                    //xx=

                    //.Replace("http://www.jades.cn/huayu/", "d:/2010/").Replace("/","\\")

                    try
                    {

                        下载文件(new Uri(group.Table.Rows[i]["URL_PIC"].ToString()), "e:/plan15/plan425-0415/", group.Table.Rows[i]["ID"].ToString());
                        
                        string ls_up = "UPDATE plan425_PIC SET status=1  WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);

                    }
                    catch
                    {

                    }

                }
            }

            MessageBox.Show("OK!");
        }




        #region 功能代码
        /// <summary>
        /// 下载文件到指定目录，并返回下载后存放的文件路径
        /// </summary>
        /// <param name="Uri">网址</param>
        /// <param name="存放目录">存放目录，如果该目录中已存在与待下载文件同名的文件，那么将自动重命名</param>
        /// <returns>下载文件存放的文件路径</returns>
        public string 下载文件(Uri Uri, string 存放目录, string 文件名)
        {

            var q = WebRequest.Create(Uri).GetResponse();
            var s = q.GetResponseStream();
            var b = new BinaryReader(s);

            if (q.ContentLength == -1)
            {
                return "";
            }



            var file = "";

            try
            {
                file = 生成下载文件存放路径(存放目录, Uri, q.ContentType, 文件名);
            }
            catch
            {
                b.Close();
                s.Close();
                return "";
            }

            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            fs.Write(b.ReadBytes((int)q.ContentLength), 0, (int)q.ContentLength);
            fs.Close();
            b.Close();
            s.Close();
            return file;

        }

        string 生成下载文件存放路径(string 存放目录, Uri Uri, string ContentType, string 文件名)
        {
            try
            {
                var ex = "jpg";//获取对应扩展名(ContentType);
                string up = null;
                string upne = null;
                if (Uri.LocalPath == "/")
                {
                    //处理Url是域名的情况
                    up = upne = Uri.Host;
                }
                else
                {
                    if (Uri.LocalPath.EndsWith("/"))
                    {
                        //处理Url是目录的情况
                        up = Uri.LocalPath.Substring(0, Uri.LocalPath.Length - 1);
                        upne = Path.GetFileName(up);
                    }
                    else
                    {
                        //处理常规Url
                        up = Uri.LocalPath;
                        upne = Path.GetFileNameWithoutExtension(up);
                    }
                }

                //var name = string.IsNullOrEmpty(ex) ? Path.GetFileName(up) : upne + "." + ex;
                var name = 文件名 + "." + ex;

                var fn = Path.Combine(存放目录, name);
                var x = 1;
                while (File.Exists(fn))
                {
                    fn = Path.Combine(存放目录, Path.GetFileNameWithoutExtension(name) + "(" + x++ + ")" + Path.GetExtension(name));
                }
                return fn;

            }
            catch
            {
                return null;
            }
        }

        //string 获取对应扩展名(string ContentType)
        //{
        //    foreach (var f in MimeDic.Keys)
        //    {
        //        if (ContentType.ToLower().IndexOf(f) >= 0) return MimeDic[f];
        //    }
        //    return null;
        //}

        //static Dictionary<string, string> MimeDic;


        #endregion

        //html = CleanWordHtml(html);

        #region Clean Word CSS


        public static string CleanWordHtml(string html)
        {
            StringCollection sc = new StringCollection();
            // get rid of unnecessary tag spans (comments and title)
            sc.Add(@"<!--(\w|\W) ?-->");
            sc.Add(@"<title>(\w|\W) ?</title>");
            // Get rid of classes and styles
            sc.Add(@"\s?class=\w ");
            sc.Add(@"\s style='[^'] '");
            // Get rid of unnecessary tags
            sc.Add(
            @"<(meta|link|/?o:|/?style|/?div|/?st\d|/?head|/?html|body|/?body|/?span|!\[)[^>]*?>");
            // Get rid of empty paragraph tags
            sc.Add(@"(<[^>] >) &nbsp;(</\w >) ");
            // remove bizarre v: element attached to <img> tag
            sc.Add(@"\s v:\w =""[^""] """);
            // remove extra lines
            sc.Add(@"(\n\r){2,}");
            foreach (string s in sc)
            {
                html = Regex.Replace(html, s, "", RegexOptions.IgnoreCase);
            }
            return html;
        }

        //public static string FixEntities(string html)
        //{
        //    NamueCollection nvc = new NamueCollection();
        //    nvc.Add("“", "&ldquo;");
        //    nvc.Add("”", "&rdquo;");
        //    nvc.Add("–", "&mdash;");
        //    foreach (string key in nvc.Keys)
        //    {
        //        html = html.Replace(key, nvc[key]);
        //    }
        //    return html;
        //}
        #endregion

        #region 清理HTML标签
        /// <summary>
        /// 清理HTML标签的多余样式；如<div style="color:#454353">示例</div>;换成<div>示例</div>
        /// </summary>
        /// <param name="str">原始文本</param>
        /// <param name="element">要清除的标签</param>
        /// <returns></returns>
        public static string ClearElement(string str, string element)
        {
            string old = @"<" + element + "[^>]+>";
            string rep = "<" + element + ">";
            str = Regex.Replace(str, old, rep, RegexOptions.IgnoreCase);
            return str;
        }
        /// <summary>
        /// 清除HTML标签；如<div style="color:#454353">示例</div>;换成：示例
        /// </summary>
        /// <param name="str">原始文本</param>
        /// <param name="element">要清除的标签</param>
        /// <returns></returns>
        public static string ReMoveElement(string str, string element)
        {
            string regFront = @"<" + element + "[^>]*>";
            string regAfter = "</" + element + ">";
            str = Regex.Replace(str, regFront, "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, regAfter, "", RegexOptions.IgnoreCase);
            return str;
        }
        /// <summary>
        /// 清理指定字符串，大小写不敏感
        /// </summary>
        /// <param name="strText">原始文本</param>
        /// <param name="strOld">要替换的字符串，支持正则表达式，大小写不敏感</param>
        /// <param name="strNew">替换后的字符串</param>
        /// <returns></returns>
        public static string RegexReplace(string strText, string strOld, string strNew)
        {
            strText = Regex.Replace(strText, strOld, strNew, RegexOptions.IgnoreCase);
            return strText;
        }
        /// <summary>
        /// 清理Word的样式，主要是一些带冒号的标签，如o:p
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static string ClearWordStyle(string strText)
        {
            string regFront = @"<\w+:[^>]*>";
            string regAfter = @"</\w+:[^>]*>";
            strText = Regex.Replace(strText, regFront, "", RegexOptions.IgnoreCase);
            strText = Regex.Replace(strText, regAfter, "", RegexOptions.IgnoreCase);
            return strText;
        }
        #endregion

        ///   <summary>
        ///   去除HTML标记
        ///   </summary>
        ///   <param   name="NoHTML">包括HTML的源码   </param>
        ///   <returns>已经去除后的文字</returns>  
        public static string NoHTML(string Htmlstring)
        {
            
            //删除脚本  
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            /*//删除HTML  
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
             * 
             * 
                html=Regex.Replace(html,@"\<a[^\>]*\>","");
                html=Regex.Replace(html,@"\</a\>","");
            */

            //<!--    -->   <--(?!/?p)[^>]+-->   <!--   -->  var result = r.replace(/<!--.*-->/gim,"")


            //<!--(.*?)-->
            Htmlstring = Regex.Replace(Htmlstring, @"<!--[^@]*-->", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<FORM[^@]*\</FORM\>", "", RegexOptions.IgnoreCase);
             

            //去掉垃圾信息，暂时注释
            //Htmlstring = Regex.Replace(Htmlstring, @"\<span.*</span>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"\<font.*</font>", "", RegexOptions.IgnoreCase);
            
            //<img >
            Htmlstring = Regex.Replace(Htmlstring, @"\<img[^\>]*\>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<table[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\<tbody[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\<tr[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\<td[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</tr\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</td\>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\</tbody\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</table\>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<blockquote[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</blockquote\>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<a[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</a\>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<FONT[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</FONT\>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<P[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</P\>", "", RegexOptions.IgnoreCase);


            Htmlstring = Regex.Replace(Htmlstring, @"\<h2[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</h2\>", "", RegexOptions.IgnoreCase);


            Htmlstring = Regex.Replace(Htmlstring, @"\<DIV[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</DIV\>", "", RegexOptions.IgnoreCase);


            Htmlstring = Regex.Replace(Htmlstring, @"\<td[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</td\>", "", RegexOptions.IgnoreCase);



            Htmlstring = Regex.Replace(Htmlstring, @"\<SPAN[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</SPAN\>", "", RegexOptions.IgnoreCase);







            Htmlstring = Regex.Replace(Htmlstring, @"^\<xml[^\>]*/xml\>", "", RegexOptions.IgnoreCase);


            //</meta>    <!--  -->
            Htmlstring = Regex.Replace(Htmlstring, @"\<meta.*</meta>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<LI[^\>]*\>", "<Li>", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<br[^\>]*\>", "<br>", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<strong[^\>]*\>", "<strong>", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\<UL[^\>]*\>", "<ul>", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\<H3[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\<H1[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\<H2[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</H3\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</H1\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</H2\>", "", RegexOptions.IgnoreCase);


            Htmlstring = Regex.Replace(Htmlstring, @"\<chmetcnv[^\>]*\>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\</chmetcnv\>", "", RegexOptions.IgnoreCase);

            //<meta http-equiv=Content-Type content=text/html; charset=utf-8><meta content=Word.Document name=ProgId><meta content=Microsoft Word 12 name=Generator><meta content=Microsoft Word 12 name=Originator><style type=text/css></style>  
            //Htmlstring = Regex.Replace(Htmlstring, @"\<meta.*</style>", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("? ", "");
            Htmlstring.Replace("?", "");

            Htmlstring.Replace("é", "e");

            Htmlstring.Replace("&nbsp;", "");



            return Htmlstring;
        }


        ///提取HTML代码中文字的C#函数
        ///   <summary>
        ///   去除HTML标记
        ///   </summary>
        ///   <param   name="strHtml">包括HTML的源码   </param>
        ///   <returns>已经去除后的文字</returns>  
        public static string StripHTML(string strHtml)
        {
            string[] aryReg ={
        @"<script[^>]*?>.*?</script>",
        @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
        @"([\r\n])[\s]+",
        @"&(quot|#34);",
        @"&(amp|#38);",
        @"&(lt|#60);",
        @"&(gt|#62);",
        @"&(nbsp|#160);",
        @"&(iexcl|#161);",
        @"&(cent|#162);",
        @"&(pound|#163);",
        @"&(copy|#169);",
        @"&#(\d+);",
        @"-->",
        @"<!--.*\n"
        };
            string[] aryRep =   {
        "",
        "",
        "",
        "\"",
        "&",
        "<",
        ">",
        "   ",
        "\xa1",//chr(161),  
                                "\xa2",//chr(162),  
                                "\xa3",//chr(163),  
                                "\xa9",//chr(169),  
                                "",
        "\r\n",
        ""
        };
            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, aryRep[i]);
            }
            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");
            return strOutput;
        }

        #region 取得文件后缀名
        /****************************************
         * 函数名称：GetPostfixStr
         * 功能说明：取得文件后缀名
         * 参    数：filename:文件名称
         * 调用示列：
         *           string filename = "aaa.aspx";        
         *           string s = EC.FileObj.GetPostfixStr(filename);         
        *****************************************/
        /// <summary>
        /// 取后缀名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>.gif|.html格式</returns>
        public static string GetPostfixStr(string filename)
        {
            int start = filename.LastIndexOf(".");
            int length = filename.Length;
            string postfix = filename.Substring(start, length - start);
            return postfix;
        }
        #endregion

        #region 写文件
        /****************************************
         * 函数名称：WriteFile
         * 功能说明：当文件不存时，则创建文件，并追加文件
         * 参    数：Path:文件路径,Strings:文本内容
         * 调用示列：
         *           string Path = Server.MapPath("Default2.aspx");       
         *           string Strings = "这是我写的内容啊";
         *           EC.FileObj.WriteFile(Path,Strings);
        *****************************************/
        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Strings">文件内容</param>
        public static void WriteFile(string Path, string Strings)
        {

            if (!System.IO.File.Exists(Path))
            {
                //Directory.CreateDirectory(Path);

                System.IO.FileStream f = System.IO.File.Create(Path);
                f.Close();
                f.Dispose();
            }
            System.IO.StreamWriter f2 = new System.IO.StreamWriter(Path, true, System.Text.Encoding.UTF8);
            f2.WriteLine(Strings);
            f2.Close();
            f2.Dispose();


        }
        #endregion

        #region 读文件
        /****************************************
         * 函数名称：ReadFile
         * 功能说明：读取文本内容
         * 参    数：Path:文件路径
         * 调用示列：
         *           string Path = Server.MapPath("Default2.aspx");       
         *           string s = EC.FileObj.ReadFile(Path);
        *****************************************/
        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <returns></returns>
        public static string ReadFile(string Path)
        {
            string s = "";
            if (!System.IO.File.Exists(Path))
                s = "不存在相应的目录";
            else
            {
                StreamReader f2 = new StreamReader(Path, System.Text.Encoding.GetEncoding("gb2312"));
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }

            return s;
        }
        #endregion

        #region 追加文件
        /****************************************
         * 函数名称：FileAdd
         * 功能说明：追加文件内容
         * 参    数：Path:文件路径,strings:内容
         * 调用示列：
         *           string Path = Server.MapPath("Default2.aspx");     
         *           string Strings = "新追加内容";
         *           EC.FileObj.FileAdd(Path, Strings);
        *****************************************/
        /// <summary>
        /// 追加文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="strings">内容</param>
        public static void FileAdd(string Path, string strings)
        {
            StreamWriter sw = File.AppendText(Path);
            sw.Write(strings);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        #endregion

        #region 【拷贝文件&并新建目录】
        /****************************************
         * 函数名称：FileCoppy
         * 功能说明：拷贝文件
         * 参    数：OrignFile:原始文件,NewFile:新文件路径
         * 调用示列：
         *           string OrignFile = Server.MapPath("Default2.aspx");     
         *           string NewFile = Server.MapPath("Default3.aspx");
         *           EC.FileObj.FileCoppy(OrignFile, NewFile);
        *****************************************/
        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="OrignFile">原始文件</param>
        /// <param name="NewFile">新文件路径</param>
        public static void FileCoppy(string OrignFile, string NewPath, string NewFile)
        {
            if (!Directory.Exists(NewPath))
                Directory.CreateDirectory(NewPath);

            //FileInfo CreateFile = new FileInfo(NewPath); //创建文件 
            //if (!CreateFile.Exists)
            //{
            //    FileStream FS = CreateFile.Create();
            //    FS.Close();
            //}


            File.Copy(OrignFile, NewPath + NewFile, true);
        }



        public static void FileCoppy2(string OrignFile, string NewPath, string NewFile)
        {
            string showNewFile = NewFile;
            //Jade-Pendant/Yellow-Jade-Pendant-3.JPG

            int temp_left = showNewFile.IndexOf('/');

            NewFile = showNewFile.Substring(temp_left + 1, showNewFile.Length - temp_left - 1);


            NewPath = NewPath + showNewFile.Replace("/" + NewFile, "") + "\\";


            if (!Directory.Exists(NewPath))
                Directory.CreateDirectory(NewPath);

            //FileInfo CreateFile = new FileInfo(NewPath); //创建文件 
            //if (!CreateFile.Exists)
            //{
            //    FileStream FS = CreateFile.Create();
            //    FS.Close();
            //}


            File.Copy(OrignFile, NewPath + NewFile, true);
        }






        #endregion

        #region 删除文件
        /****************************************
         * 函数名称：FileDel
         * 功能说明：删除文件
         * 参    数：Path:文件路径
         * 调用示列：
         *           string Path = Server.MapPath("Default3.aspx");    
         *           EC.FileObj.FileDel(Path);
        *****************************************/
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Path">路径</param>
        public static void FileDel(string Path)
        {
            File.Delete(Path);
        }
        #endregion

        #region 移动文件
        /****************************************
         * 函数名称：FileMove
         * 功能说明：移动文件
         * 参    数：OrignFile:原始路径,NewFile:新文件路径
         * 调用示列：
         *            string OrignFile = Server.MapPath("../说明.txt");    
         *            string NewFile = Server.MapPath("http://www.cnblogs.com/说明.txt");
         *            EC.FileObj.FileMove(OrignFile, NewFile);
        *****************************************/
        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="OrignFile">原始路径</param>
        /// <param name="NewFile">新路径</param>
        public static void FileMove(string OrignFile, string NewFile)
        {
            File.Move(OrignFile, NewFile);
        }
        #endregion

        #region 在当前目录下创建目录
        /****************************************
         * 函数名称：FolderCreate
         * 功能说明：在当前目录下创建目录
         * 参    数：OrignFolder:当前目录,NewFloder:新目录
         * 调用示列：
         *           string OrignFolder = Server.MapPath("test/");    
         *           string NewFloder = "new";
         *           EC.FileObj.FolderCreate(OrignFolder, NewFloder); 
        *****************************************/
        /// <summary>
        /// 在当前目录下创建目录
        /// </summary>
        /// <param name="OrignFolder">当前目录</param>
        /// <param name="NewFloder">新目录</param>
        public static void FolderCreate(string OrignFolder, string NewFloder)
        {
            Directory.SetCurrentDirectory(OrignFolder);
            Directory.CreateDirectory(NewFloder);
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="Path"></param>
        public static void FolderCreate(string Path)
        {
            // 判断目标目录是否存在如果不存在则新建之
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
        }
        #endregion

        #region 创建目录
        public static void FileCreate(string Path)
        {
            FileInfo CreateFile = new FileInfo(Path); //创建文件 
            if (!CreateFile.Exists)
            {
                FileStream FS = CreateFile.Create();
                FS.Close();
            }
        }
        #endregion

        #region 递归删除文件夹目录及文件
        /****************************************
         * 函数名称：DeleteFolder
         * 功能说明：递归删除文件夹目录及文件
         * 参    数：dir:文件夹路径
         * 调用示列：
         *           string dir = Server.MapPath("test/"); 
         *           EC.FileObj.DeleteFolder(dir);       
        *****************************************/
        /// <summary>
        /// 递归删除文件夹目录及文件
        /// </summary>
        /// <param name="dir"></param> 
        /// <returns></returns>
        public static void DeleteFolder(string dir)
        {
            if (Directory.Exists(dir)) //如果存在这个文件夹删除之 
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                        File.Delete(d); //直接删除其中的文件                        
                    else
                        DeleteFolder(d); //递归删除子文件夹 
                }
                Directory.Delete(dir, true); //删除已空文件夹                 
            }
        }

        #endregion

        #region 将指定文件夹下面的所有内容copy到目标文件夹下面 果目标文件夹为只读属性就会报错。
        /****************************************
         * 函数名称：CopyDir
         * 功能说明：将指定文件夹下面的所有内容copy到目标文件夹下面 果目标文件夹为只读属性就会报错。
         * 参    数：srcPath:原始路径,aimPath:目标文件夹
         * 调用示列：
         *           string srcPath = Server.MapPath("test/"); 
         *           string aimPath = Server.MapPath("test1/");
         *           EC.FileObj.CopyDir(srcPath,aimPath);   
        *****************************************/
        /// <summary>
        /// 指定文件夹下面的所有内容copy到目标文件夹下面
        /// </summary>
        /// <param name="srcPath">原始路径</param>
        /// <param name="aimPath">目标文件夹</param>
        public static void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加之
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                // 判断目标目录是否存在如果不存在则新建之
                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                //如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                //string[] fileList = Directory.GetFiles(srcPath);
                string[] fileList = Directory.GetFileSystemEntries(srcPath);
                //遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个目录就递归Copy该目录下面的文件

                    if (Directory.Exists(file))
                        CopyDir(file, aimPath + Path.GetFileName(file));
                    //否则直接Copy文件
                    else
                        File.Copy(file, aimPath + Path.GetFileName(file), true);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }
        }
        #endregion

        #region 获取指定文件夹下所有子目录及文件(树形)
        /****************************************
         * 函数名称：GetFoldAll(string Path)
         * 功能说明：获取指定文件夹下所有子目录及文件(树形)
         * 参    数：Path:详细路径
         * 调用示列：
         *           string strDirlist = Server.MapPath("templates");       
         *           this.Literal1.Text = EC.FileObj.GetFoldAll(strDirlist); 
        *****************************************/
        /// <summary>
        /// 获取指定文件夹下所有子目录及文件
        /// </summary>
        /// <param name="Path">详细路径</param>
        public static string GetFoldAll(string Path)
        {

            string str = "";
            DirectoryInfo thisOne = new DirectoryInfo(Path);
            str = ListTreeShow(thisOne, 0, str);
            return str;

        }


        /// <summary>
        /// 获取指定文件夹下所有子目录及文件函数
        /// </summary>
        /// <param name="theDir">指定目录</param>
        /// <param name="nLevel">默认起始值,调用时,一般为0</param>
        /// <param name="Rn">用于迭加的传入值,一般为空</param>
        /// <returns></returns>
        public static string ListTreeShow(DirectoryInfo theDir, int nLevel, string Rn)//递归目录 文件
        {
            DirectoryInfo[] subDirectories = theDir.GetDirectories();//获得目录
            foreach (DirectoryInfo dirinfo in subDirectories)
            {

                if (nLevel == 0)
                {
                    Rn += "├";
                }
                else
                {
                    string _s = "";
                    for (int i = 1; i <= nLevel; i++)
                    {
                        _s += "│&nbsp;";
                    }
                    Rn += _s + "├";
                }
                Rn += "<b>" + dirinfo.Name.ToString() + "</b><br />";
                FileInfo[] fileInfo = dirinfo.GetFiles();   //目录下的文件
                foreach (FileInfo fInfo in fileInfo)
                {
                    if (nLevel == 0)
                    {
                        Rn += "│&nbsp;├";
                    }
                    else
                    {
                        string _f = "";
                        for (int i = 1; i <= nLevel; i++)
                        {
                            _f += "│&nbsp;";
                        }
                        Rn += _f + "│&nbsp;├";
                    }
                    Rn += fInfo.Name.ToString() + " <br />";
                }
                Rn = ListTreeShow(dirinfo, nLevel + 1, Rn);


            }
            return Rn;
        }


        /****************************************
         * 函数名称：GetFoldAll(string Path)
         * 功能说明：获取指定文件夹下所有子目录及文件(下拉框形)
         * 参    数：Path:详细路径
         * 调用示列：
         *            string strDirlist = Server.MapPath("templates");      
         *            this.Literal2.Text = EC.FileObj.GetFoldAll(strDirlist,"tpl","");
        *****************************************/
        /// <summary>
        /// 获取指定文件夹下所有子目录及文件(下拉框形)
        /// </summary>
        /// <param name="Path">详细路径</param>
        ///<param name="DropName">下拉列表名称</param>
        ///<param name="tplPath">默认选择模板名称</param>
        public static string GetFoldAll(string Path, string DropName, string tplPath)
        {
            string strDrop = "<select name=\"" + DropName + "\" id=\"" + DropName + "\"><option value=\"\">--请选择详细模板--</option>";
            string str = "";
            DirectoryInfo thisOne = new DirectoryInfo(Path);
            str = ListTreeShow(thisOne, 0, str, tplPath);
            return strDrop + str + "</select>";

        }

        /// <summary>
        /// 获取指定文件夹下所有子目录及文件函数
        /// </summary>
        /// <param name="theDir">指定目录</param>
        /// <param name="nLevel">默认起始值,调用时,一般为0</param>
        /// <param name="Rn">用于迭加的传入值,一般为空</param>
        /// <param name="tplPath">默认选择模板名称</param>
        /// <returns></returns>
        public static string ListTreeShow(DirectoryInfo theDir, int nLevel, string Rn, string tplPath)//递归目录 文件
        {
            DirectoryInfo[] subDirectories = theDir.GetDirectories();//获得目录

            foreach (DirectoryInfo dirinfo in subDirectories)
            {

                Rn += "<option value=\"" + dirinfo.Name.ToString() + "\"";
                if (tplPath.ToLower() == dirinfo.Name.ToString().ToLower())
                {
                    Rn += " selected ";
                }
                Rn += ">";

                if (nLevel == 0)
                {
                    Rn += "┣";
                }
                else
                {
                    string _s = "";
                    for (int i = 1; i <= nLevel; i++)
                    {
                        _s += "│&nbsp;";
                    }
                    Rn += _s + "┣";
                }
                Rn += "" + dirinfo.Name.ToString() + "</option>";


                FileInfo[] fileInfo = dirinfo.GetFiles();   //目录下的文件
                foreach (FileInfo fInfo in fileInfo)
                {
                    Rn += "<option value=\"" + dirinfo.Name.ToString() + "/" + fInfo.Name.ToString() + "\"";
                    if (tplPath.ToLower() == fInfo.Name.ToString().ToLower())
                    {
                        Rn += " selected ";
                    }
                    Rn += ">";

                    if (nLevel == 0)
                    {
                        Rn += "│&nbsp;├";
                    }
                    else
                    {
                        string _f = "";
                        for (int i = 1; i <= nLevel; i++)
                        {
                            _f += "│&nbsp;";
                        }
                        Rn += _f + "│&nbsp;├";
                    }
                    Rn += fInfo.Name.ToString() + "</option>";
                }
                Rn = ListTreeShow(dirinfo, nLevel + 1, Rn, tplPath);


            }
            return Rn;
        }
        #endregion

        #region 获取文件夹大小
        /****************************************
         * 函数名称：GetDirectoryLength(string dirPath)
         * 功能说明：获取文件夹大小
         * 参    数：dirPath:文件夹详细路径
         * 调用示列：
         *           string Path = Server.MapPath("templates"); 
         *           Response.Write(EC.FileObj.GetDirectoryLength(Path));       
        *****************************************/
        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <returns></returns>
        public static long GetDirectoryLength(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                return 0;
            long len = 0;
            DirectoryInfo di = new DirectoryInfo(dirPath);
            foreach (FileInfo fi in di.GetFiles())
            {
                len += fi.Length;
            }
            DirectoryInfo[] dis = di.GetDirectories();
            if (dis.Length > 0)
            {
                for (int i = 0; i < dis.Length; i++)
                {
                    len += GetDirectoryLength(dis[i].FullName);
                }
            }
            return len;
        }
        #endregion

        #region 获取指定文件详细属性
        /****************************************
         * 函数名称：GetFileAttibe(string filePath)
         * 功能说明：获取指定文件详细属性
         * 参    数：filePath:文件详细路径
         * 调用示列：
         *           string file = Server.MapPath("robots.txt"); 
         *            Response.Write(EC.FileObj.GetFileAttibe(file));         
        *****************************************/
        /// <summary>
        /// 获取指定文件详细属性
        /// </summary>
        /// <param name="filePath">文件详细路径</param>
        /// <returns></returns>
        public static string GetFileAttibe(string filePath)
        {
            string str = "";
            System.IO.FileInfo objFI = new System.IO.FileInfo(filePath);
            str += "详细路径:" + objFI.FullName + "<br>文件名称:" + objFI.Name + "<br>文件长度:" + objFI.Length.ToString() + "字节<br>创建时间" + objFI.CreationTime.ToString() + "<br>最后访问时间:" + objFI.LastAccessTime.ToString() + "<br>修改时间:" + objFI.LastWriteTime.ToString() + "<br>所在目录:" + objFI.DirectoryName + "<br>扩展名:" + objFI.Extension;
            return str;
        }
        #endregion

        #region 【获取某文件大小】
        public static string GetFileSize(string filePath)
        {
            string fs = "0";
            try
            {
                System.IO.FileInfo objFI = new System.IO.FileInfo(filePath);
                fs = objFI.Length.ToString();
            }
            catch
            {
                return "";
            }
            return fs;
        }
        #endregion

        private void button35_Click(object sender, EventArgs e)
        {
            //-----------------------NO HTML-------------------------
            DataGroup group_html = new DataGroup();
            group_html = null;

            string ls_sql_html = "SELECT ID,description as remark FROM plan425_b2 order by ID";//WHERE STATUS =1   where status=3 
            group_html = DB.GetDataGroup(ls_sql_html);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["remark"].ToString().Replace("\n", "");

                    string ls_up2 = "update plan425_b2 set description='" + NoHTML(ls_shtml).Trim() + "'  where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up2);
                }
            }
            //-----------------------NO HTML-------------------------




            //update jade_pic set filename='D:\\IMGS\\JADE2010\\'+CAST( id AS nvarchar(10))+'.JPG'
            //UPDATE ZY_PIC SET FILENAME='D:\\IMGS\\ZY2010\\'+LTRIM(RTRIM( str(ID) ))+'.JPG'

            DataGroup group = new DataGroup();
            group = null;

            string ls_sql = "SELECT ID FROM plan425_PIC /*WHERE STATUS =1*/  order by ID";//and filename!='D:\\IMGS\\ZY2010\\'
            group = DB.GetDataGroup(ls_sql);

            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string ls_Value = GetFileSize("E:\\Plan15\\plan425-0415\\"+group.Table.Rows[i]["id"].ToString()+".jpg");

                    string ls_up = "UPDATE plan425_PIC SET FILESIZE='" + ls_Value + "' ,STATUS=1 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                    DB.ExecuteSQL(ls_up);

                }
            }

            string ls_flag = "update plan425_PIC set status=0 where filesize='' or filesize=0";
            DB.ExecuteSQL(ls_flag);

            MessageBox.Show("OK!");
        }

        private void button36_Click_1(object sender, EventArgs e)
        {
            DataGroup group_files = new DataGroup();

            string ls_group_files = "select  y.id as ID,x.pic_s_ename   from jade_b3 x,jade_pic y where x.id=y.pid and y.flag='S' ";
            group_files = DB.GetDataGroup(ls_group_files);
            if (group_files.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_files.Table.Rows.Count; x++)
                {

                    FileCoppy2("d:\\Imgs\\jade2010\\s\\" + group_files.Table.Rows[x]["id"].ToString().Trim() + ".jpg", "e:\\JFOK\\", group_files.Table.Rows[x]["pic_s_ename"].ToString().Trim());

                }
            }


            /*
             DataGroup group_files = new DataGroup();

             string ls_group_files = "select id,url_ename from jade_pic where flag='B' ";
             group_files = DB.GetDataGroup(ls_group_files);
             if (group_files.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_files.Table.Rows.Count; x++)
                {

                    FileCoppy2("e:\\jf\\" + group_files.Table.Rows[x]["id"].ToString().Trim() + ".jpg", "e:\\JFOK\\", group_files.Table.Rows[x]["url_ename"].ToString().Trim());



                }
            }

            */

            MessageBox.Show("OK!!");


        }

        private void button27_Click(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();

            DataGroup group_html = new DataGroup();
            group_html = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group_html = null;

            string ls_sql = "SELECT ID,c1name,c1url,c1html FROM brand1 WHERE STATUS =1 order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["c1html"].ToString().Replace("\r\n", "").ToString();//替换换行符
                    ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("<div class=box-product-list", "|");//将分隔码 替换成“|”
                    string[] dog_small = ls_shtml.Split('|');//子串

                    int pd = 0;

                    foreach (string bb in dog_small)
                    {
                        if (pd > 0)
                        {

                            string ls_FILED = "";
                            string ls_Value = "";

                            //-----------------------------------------------------------------------------------------------------
                            group = null;
                            string ls_rule = "SELECT RID,FILED,OID,CYCLE,STRING_S,STRING_E,STRING_NEW,INCLUDE_S,INCLUDE_E FROM URL_RULE WHERE siteid=6 and STATUS=1";
                            group = DB.GetDataGroup(ls_rule);
                            if (group.Table.Rows.Count > 0)
                            {
                                for (int j = 0; j < group.Table.Rows.Count; j++)
                                {
                                    try
                                    {
                                        ls_Value += ",'" + OperateStr(bb, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, "0") + "'";
                                    }
                                    catch
                                    {
                                        ls_Value = "," + " ";
                                    }

                                    ls_FILED += "," + group.Table.Rows[j]["FILED"].ToString();


                                }

                                string ls_up = "INSERT INTO popbag_B2 (PID " + ls_FILED + ",STATUS) values ('" + group_html.Table.Rows[i]["ID"].ToString() + "' " + ls_Value + ",1)";
                                DB.ExecuteSQL(ls_up);

                                ls_Value = "";
                                ls_FILED = "";


                            }
                            group = null;
                            //-----------------------------------------------------------------------------------------------------
                        }
                        pd += 1;
                    }





                }

                MessageBox.Show("OK!!");
            }
        }

        private void button29_Click_1(object sender, EventArgs e)
        {
            //"\r\n"
            string LS_html = l_html.Text;

            l_html.Text = OperateStr_Adv_html(LS_html.Replace("'", "\"")
                                        , "href=\"", "\"", ""
                                        , 1
                                        , 0, 0
                                        , "1"
                                        , ""
                                        , "", "");




        }


        private void button27_Click_1(object sender, EventArgs e)
        {
            string LS_html = l_html.Text;

            //<a href="http://en.wikipedia.org/wiki/MoinMoin" title="MoinMoin">MoinMoin</a><span 


            l_html.Text = OperateStr_Adv_html(LS_html.Replace("'", "\"")
                //, "\">", "</a>", ""
                                         , "title=", ">", ""
                                        , 1
                                        , 0, 0
                                        , "1"
                                        , ""
                                        , "", "");
        }

        private void button28_Click_1(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();
            group = null;

            string ls_sql = "SELECT ID,www FROM cms_b2 WHERE STATUS =5 order by ID";
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = getUrl301(group.Table.Rows[i]["www"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312

                    //pig = HttpUtility.HtmlEncode(pig);

                    if (pig != "")
                    {
                        string ls_up = "UPDATE cms_b2 SET WWW3='" + pig + "' ,STATUS=1  WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);

                        pig = "";
                    }

                }
            }

            MessageBox.Show("ok!");




        }

        private void button37_Click(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();
            group = null;

            string ls_sql = "SELECT ID,NAME,desc_en FROM cms_b2 order by ID";
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    //string pig = getUrl301(group.Table.Rows[i]["www"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312
                    //string pig = HttpUtility.HtmlEncode(group.Table.Rows[i]["NAME"].ToString());

                    string pig = HttpUtility.HtmlDecode(group.Table.Rows[i]["NAME"].ToString()).Trim().Replace('\'', '‘');
                    string pig2 = HttpUtility.HtmlDecode(group.Table.Rows[i]["desc_en"].ToString()).Trim().Replace('\'', '‘');

                    if (pig != "")
                    {
                        string ls_up = "UPDATE cms_b2 SET name2='" + pig + "' ,desc2='" + pig2 + "' ,STATUS=1  WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);

                        pig = "";
                    }

                }
            }

            MessageBox.Show("ok!");


        }

        private void button38_Click(object sender, EventArgs e)
        {
            //update JADE_KW_CLASS SET BIGTEXT=REPLACE(BIGTEXT,'||','|')


            DataGroup group = new DataGroup();
            group = null;

            string ls_sql = "SELECT ID,main_class,sub_class,desc_en FROM jade_b3 where desc_en is not null order by ID";
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {





                for (int i = 0; i < group.Table.Rows.Count; i++)
                {

                    //if (group.Table.Rows[i]["desc_en"].ToString().Trim().Length >10)
                    //{

                    string pig = group.Table.Rows[i]["desc_en"].ToString().Trim().Replace('\'', ' ') + '|' + group.Table.Rows[i]["sub_class"].ToString().Trim().Replace('\'', ' ');

                    if (pig != "")
                    {
                        string ls_up = "UPDATE JADE_KW_CLASS SET bigtext='|'+bigtext+'" + pig + "'  WHERE main_class='" + group.Table.Rows[i]["main_class"].ToString() + "' and  sub_class ='" + group.Table.Rows[i]["sub_class"].ToString().Replace('\'', ' ') + "'";
                        DB.ExecuteSQL(ls_up);
                        pig = "";
                    }

                    //}


                }
            }

            MessageBox.Show("ok!");

        }

        private void button39_Click(object sender, EventArgs e)
        {

            DataGroup group = new DataGroup();

            DataGroup group_html = new DataGroup();
            group_html = null;

            string ls_sql = "select id,main_class,sub_class,pnum,bigtext from JADE_KW_CLASS where status=1";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["bigtext"].ToString();
                    string[] dog_small = ls_shtml.Split('|');

                    foreach (string bb in dog_small)
                    {
                        string xx = bb.Trim();
                        //string ls_up = "insert into ZY_PIC (pid,url_pic,flag,status) values ('" + group_html.Table.Rows[i]["ID"].ToString() + "','http://www.otbags.com/" + xx + "','B',1)";


                        group = null;
                        string ls_double = "select id from JADE_KW_CLASS_SUB where skw='" + xx + "' and main_class='" + group_html.Table.Rows[i]["main_class"].ToString().Trim() + "'  and sub_class='" + group_html.Table.Rows[i]["sub_class"].ToString().Trim().Replace('\'', '‘') + "'   ";
                        group = DB.GetDataGroup(ls_double);
                        if (group.Table.Rows.Count == 0)
                        {
                            string ls_up = "insert into JADE_KW_CLASS_SUB (pid,main_class,sub_class,skw,status) " +
                                " values ('" + group_html.Table.Rows[i]["ID"].ToString().Trim() + "','" + group_html.Table.Rows[i]["main_class"].ToString().Trim() + "','" + group_html.Table.Rows[i]["sub_class"].ToString().Trim().Replace('\'', '‘') + "','" + xx + "',1)";
                            DB.ExecuteSQL(ls_up);
                        }


                    }



                }
            }

            MessageBox.Show("OK!");
        }

        private void button40_Click(object sender, EventArgs e)
        {
            DataGroup group_main_class = new DataGroup();
            string ls_group_main_class = "select distinct main_class from JADE_KW_CLASS_SUB where status=1 ";
            group_main_class = DB.GetDataGroup(ls_group_main_class);
            if (group_main_class.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_main_class.Table.Rows.Count; x++)
                {
                    DataGroup group_kw = new DataGroup();
                    DataGroup group_word = new DataGroup();
                    group_kw = null;
                    group_word = null;
                    string ls_group_kw = "select id,main_class,kw from jade_kw1 where main_class='" + group_main_class.Table.Rows[x]["main_class"].ToString().Trim() + "'";
                    string ls_group_word = "select id,main_class,sub_class,pid,skw from JADE_KW_CLASS_SUB where status=1 and main_class='" + group_main_class.Table.Rows[x]["main_class"].ToString().Trim() + "'";
                    group_kw = DB.GetDataGroup(ls_group_kw);
                    group_word = DB.GetDataGroup(ls_group_word);


                    if (group_kw.Table.Rows.Count > 0)
                    {
                        for (int k = 0; k < group_kw.Table.Rows.Count; k++)
                        {


                            if (group_word.Table.Rows.Count > 0)
                            {
                                for (int w = 0; w < group_word.Table.Rows.Count; w++)
                                {


                                    if (group_kw.Table.Rows[k]["kw"].ToString().Trim().IndexOf(group_word.Table.Rows[w]["skw"].ToString().Trim()) >= 0)
                                    {

                                        string ls_up = "insert into jade_kw2 (pid,main_class,sub_class,kw,skw)" +
                                      " values ('" + group_word.Table.Rows[w]["pid"].ToString().Trim() + "','" + group_word.Table.Rows[w]["main_class"].ToString().Trim() + "'  ,'" + group_word.Table.Rows[w]["sub_class"].ToString().Trim() + "',  '" + group_kw.Table.Rows[k]["kw"].ToString().Trim().Replace('\'', '‘') + "','" + group_word.Table.Rows[w]["skw"].ToString().Trim() + "'  )";
                                        DB.ExecuteSQL(ls_up);

                                    }





                                }
                            }


                        }
                    }




                }
            }

            MessageBox.Show("OK!");
        }

        private void button41_Click(object sender, EventArgs e)
        {
            //UPDATE jade_b3 SET PIC_S_ENAME=MAIN_CLASS+'/'+MAIN_CLASS+'-'+PIC_S_ENAME+'.JPG'
            //UPDATE jade_b3 SET PIC_S_ENAME=replace(PIC_S_ENAME,' ','-')

            DataGroup group_main_class = new DataGroup();
            string ls_group_main_class = "select distinct main_class,sub_class  from xuping_engs ";
            group_main_class = DB.GetDataGroup(ls_group_main_class);
            if (group_main_class.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_main_class.Table.Rows.Count; x++)
                {
                    DataGroup group_kw = new DataGroup();
                    group_kw = null;
                    string ls_group_kw = "select id from xuping_engs where main_class='" + group_main_class.Table.Rows[x]["main_class"].ToString().Trim() + "' and replace(sub_class,'''','-')='" + group_main_class.Table.Rows[x]["sub_class"].ToString().Trim().Replace('\'', '-') + "' order by main_class,sub_class,ID ";
                    group_kw = DB.GetDataGroup(ls_group_kw);

                    if (group_kw.Table.Rows.Count > 0)
                    {
                        for (int k = 0; k < group_kw.Table.Rows.Count; k++)
                        {

                            int pig = k + 1;

                            string ls_up = "UPDATE xuping_engs SET model='" + pig + "'  WHERE id='" + group_kw.Table.Rows[k]["id"].ToString() + "' ";
                            DB.ExecuteSQL(ls_up);

                        }
                    }

                }

            }


            MessageBox.Show("ok!");

        }

        private void button42_Click(object sender, EventArgs e)
        {
            //.Replace('\'', '-')

            DataGroup group_main_class = new DataGroup();

            DataGroup group_list = new DataGroup();

            string ls_group_main_class = "select distinct main_class from jade_b3 ";
            group_main_class = DB.GetDataGroup(ls_group_main_class);
            if (group_main_class.Table.Rows.Count > 0)
            {


                for (int x = 0; x < group_main_class.Table.Rows.Count; x++)
                {


                    string ls_kw_list = "select id,kw from jade_kw1 where  main_class='" + group_main_class.Table.Rows[x]["main_class"].ToString() + "' order by kw";
                    group_list = null;
                    group_list = DB.GetDataGroup(ls_kw_list);


                    ArrayList list = new ArrayList();

                    if (group_list.Table.Rows.Count > 0)
                    {
                        for (int y = 0; y < group_list.Table.Rows.Count; y++)
                        {
                            list.Add(group_list.Table.Rows[y]["kw"].ToString());
                        }
                    }



                    Random ra = new Random();
                    int num = group_list.Table.Rows.Count;
                    int value = ra.Next(num);
                    //string dog = list[value].ToString();


                    DataGroup group_product_images = new DataGroup();
                    string ls_group_product_images = "select x.id as ID from jade_pic x,JADE_b3 y where x.status=2 and x.pid=y.id and y.main_class='" + group_main_class.Table.Rows[x]["main_class"].ToString().Trim() + "'";
                    group_product_images = DB.GetDataGroup(ls_group_product_images);
                    if (group_product_images.Table.Rows.Count > 0)
                    {
                        for (int y = 0; y < group_product_images.Table.Rows.Count; y++)
                        {
                            value = ra.Next(num);
                            string dog = list[value].ToString().Replace('\'', '-').Replace(' ', '-');

                            string ls_up = "UPDATE jade_pic SET KW='" + dog + "' WHERE ID='" + group_product_images.Table.Rows[y]["ID"].ToString() + "'  ";
                            DB.ExecuteSQL(ls_up);

                        }
                    }







                }
            }




            MessageBox.Show("OK!");

        }

        private void button43_Click(object sender, EventArgs e)
        {
            //desc_cn  colour  weight  material  规格  pattern   objects  originalarea
            //desc_cn2
            //select 'update `jv1_products_description` set products_description="'+replace(desc_en2,'"','''')+'"  where products_name="'+main_class+'-'+replace(sub_class,'/',' ')+' '+ename+'";' from jade_b3 where mid is not null

            //select 'update `jv1_products_description` set `bigimghtml`="'+replace(bigimghtml,'"','''')+'"  where products_name="'+pname+'";' as ok from JewelOra_b3 where status=4


            /*  JewelOra_b3
            ,Color
            ,Material
            ,Style
            ,Length
            ,Weight
            ,Description
            ,Packing_Method
            */

            DataGroup group_field = new DataGroup();
            group_field = null;

            DataGroup group = new DataGroup();
            group = null;


            string ls_sql = "select id     from usa_nike_b3 where status=1 order by id";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {

                    string ls_shtml = "";
                    string ls_shtml2 = "";

                    //if (group_field.Table.Rows[i]["pname"].ToString().Trim().Length > 1)
                    //{
                    //    ls_shtml += "<p>" + group_field.Table.Rows[i]["pname"].ToString().Trim() + "</p>";
                    //}
                    //else
                    //{
                    //    //ls_shtml += "<p>" + group_field.Table.Rows[i]["ename"].ToString().Trim() + "</p>";
                    //}
                    /*
                    if (group_field.Table.Rows[i]["Material"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p><b>Material:</b>" + group_field.Table.Rows[i]["Material"].ToString().Trim() + "</p>";
                    }


                    if (group_field.Table.Rows[i]["Color"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p><b>Color:</b>" + group_field.Table.Rows[i]["Color"].ToString().Trim() + "</p>";
                    }


                    if (group_field.Table.Rows[i]["Style"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p><b>Style:</b>" + group_field.Table.Rows[i]["Style"].ToString().Trim() + "</p>";
                    }

                    if (group_field.Table.Rows[i]["Weight"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p><b>Weight:</b>" + group_field.Table.Rows[i]["Weight"].ToString().Trim() + "</p>";
                    }

                    string a = "<div style=\"float:left;width:420px;margin-top:5px;\">";

                    string ls_up2 = "update JewelOra_b3 set desc2 ='" +a+ ls_shtml.Replace('\'', '‘') + "</div>'  where id='" + group_field.Table.Rows[i]["id"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up2);

                    */
                    /**/
                    string ls_group = "select id from usa_nike_pic where pid ='" + group_field.Table.Rows[i]["id"].ToString().Trim() + "' and flag='B' ";
                    string dog = "";
                    group = null;
                    group = DB.GetDataGroup(ls_group);
                    if (group.Table.Rows.Count > 0)
                    {
                        for (int x = 0; x < group.Table.Rows.Count; x++)
                        {
                            dog += "<img src=\"images/Cheap-Nike-Shoes-" + group.Table.Rows[x]["id"].ToString().Trim() + ".jpg\" alt=\"Cheap-Nike-Shoes " + group.Table.Rows[x]["id"].ToString().Trim() + "\" title=\"Cheap-Nike-Shoes " + group.Table.Rows[x]["id"].ToString().Trim() + "\" /><br/>";

                        }
                    }

                    //if (group_field.Table.Rows[i]["Description"].ToString().Trim().Length > 1)
                    //{
                    //    ls_shtml += "<div class=\"sidebox-header-left\"><h3 class=\"leftBoxHeading\" >Product Description:</h3></div><div class=\"sideBoxContent\">" 
                    //        + group_field.Table.Rows[i]["Description"].ToString().Trim() + "</div><br>";
                    //}

                    if (dog.Length > 1)
                    {
                        ls_shtml += "<div class=\"sidebox-header-left\"><h3 class=\"leftBoxHeading\">Product Images:</h3></div><div class=\"sideBoxContent\"   align=\"center\">"
                            + dog + "</div>";
                    }


                    //if (group_field.Table.Rows[i]["Packing_Method"].ToString().Trim().Length > 1)
                    //{
                    //    ls_shtml2 += "<div class=\"sidebox-header-left\"><h3 class=\"leftBoxHeading\" >Packing Method:</h3></div><div class=\"sideBoxContent\">"
                    //        + group_field.Table.Rows[i]["Packing_Method"].ToString().Trim() + "</div>";
                    //}



                    string ls_up = "update usa_nike_b3 set bigimghtml ='" + ls_shtml.Replace('\'', '‘') + "'  where id='" + group_field.Table.Rows[i]["id"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up);

                    //string ls_up2 = "update JewelOra_b3 set desc3 ='" + ls_shtml2.Replace('\'', '‘') + "'  where id='" + group_field.Table.Rows[i]["id"].ToString().Trim() + "' ";
                    //DB.ExecuteSQL(ls_up2);




                }
            }

            MessageBox.Show("OK!");







        }

        private void button44_Click(object sender, EventArgs e)
        {

            DataGroup group_field = new DataGroup();
            group_field = null;

            string ls_sql = "select id,filename from jade_pic where flag='B' ";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {


                    Bitmap bm1 = new Bitmap(group_field.Table.Rows[i]["filename"].ToString().Trim());//得到一个bitmap 
                    Color myColor = new Color();

                    try
                    {
                        myColor = bm1.GetPixel(581, 45);//取到相应坐标的像素的颜色=255 255 248 ok
                        if (myColor.R > 218 & myColor.G > 218 & myColor.B > 218)
                        {
                            //move to type 1   ok1
                            FileCoppy(group_field.Table.Rows[i]["filename"].ToString().Trim(), "e:\\1\\", group_field.Table.Rows[i]["id"].ToString().Trim() + ".JPG");
                        }
                        else
                        {
                            myColor = bm1.GetPixel(462, 52);//取到相应坐标的像素的颜色=255 255 248 bad
                            if (myColor.R > 218 & myColor.G > 218 & myColor.B > 218)
                            {
                                //move to type2  ok2
                                FileCoppy(group_field.Table.Rows[i]["filename"].ToString().Trim(), "e:\\2\\", group_field.Table.Rows[i]["id"].ToString().Trim() + ".JPG");

                            }
                            else
                            {
                                //move to type3  what1
                                FileCoppy(group_field.Table.Rows[i]["filename"].ToString().Trim(), "e:\\3\\", group_field.Table.Rows[i]["id"].ToString().Trim() + ".JPG");

                            }


                        }
                    }
                    catch
                    {
                        //move to type4 what2
                        FileCoppy(group_field.Table.Rows[i]["filename"].ToString().Trim(), "e:\\4\\", group_field.Table.Rows[i]["id"].ToString().Trim() + ".JPG");
                    }




                    //if (myColor.R.ToString() == "255" & myColor.G.ToString() == "255" & myColor.B.ToString() == "248")
                    // MessageBox.Show(myColor.R.ToString() + '-' + myColor.G.ToString() + '-' + myColor.B.ToString());



                }
            }





            MessageBox.Show("OK!");



        }

        private void button45_Click(object sender, EventArgs e)
        {
            string f = "";
            DirectoryInfo theDir = new DirectoryInfo("c:\\j\\");
            FileInfo[] fileInfo = theDir.GetFiles();   //目录下的文件
            foreach (FileInfo fInfo in fileInfo)
            {
                f = fInfo.Name.ToString().Replace(".jpg", "");


                string ls_up = "update jade_pic set f1 ='9'  where id='" + f + "' ";
                DB.ExecuteSQL(ls_up);


            }

            MessageBox.Show("OK!");

        }

        private void button46_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(MoveFiles);
            thread.IsBackground = true;
            thread.Start();
        }

        public void MoveFiles()
        {
            DataGroup group_field = new DataGroup();
            group_field = null;

            string ls_sql = "select id from NFL_B3 /* where flag='B'*/ ";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {
                    //FileCoppy(group_field.Table.Rows[i]["filename"].ToString().Trim(), "e:\\A1\\", group_field.Table.Rows[i]["id"].ToString().Trim() + ".JPG");

                    try
                    {
                        File.Copy("e:\\imgs\\nfl\\s\\" + group_field.Table.Rows[i]["id"].ToString().Trim() + ".jpg", "e:\\imgs\\nfl\\s-ok\\wholesale-nfl-jerseys-" + group_field.Table.Rows[i]["id"].ToString().Trim() + ".jpg", true);
                    }
                    catch
                    { }

                }
            }


            MessageBox.Show("OK!");
        }

        private void button47_Click(object sender, EventArgs e)
        {
            //这个很重要，要到mysql里执行
            //select 'update `jv1_products_description` set bigimghtml="'+replace(bigimghtml,'"','''')+'"  where products_name="'+main_class+'-'+replace(sub_class,'/',' ')+' '+ename+'";' from jade_b3 where mid is not null

            /*
            DataGroup group_field = new DataGroup();
            group_field = null;

            string ls_sql = " select id,pid,kw,url_ename from jade_pic where flag='B' ";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {
                    string dog = "";
                    dog = "<img src=\"http://pic.jade-jewe.com/images/" + group_field.Table.Rows[i]["url_ename"].ToString().Trim() + "\" alt=\"" + group_field.Table.Rows[i]["kw"].ToString().Trim() + "\" title=\"" + group_field.Table.Rows[i]["kw"].ToString().Trim() + "\" /><br/><br/>";

                    string ls_up = "update jade_b3 set bigimghtml = bigimghtml+'" + dog + "'  where id='" + group_field.Table.Rows[i]["pid"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up);

                }
            }
            */


            DataGroup group_field = new DataGroup();
            group_field = null;

            string ls_sql = " select id,pid,flag from usa_nike_pic where flag='B'  order by id";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {
                    string dog = "";



                    dog = "<img src=\"http://www.cheap-nike-shoes.org/Cheap-Nike_Shoes-" + group_field.Table.Rows[i]["id"].ToString().Trim() + ".jpg\" alt=\"Cheap-Nike_Shoes-" + group_field.Table.Rows[i]["id"].ToString().Trim() + "\" title=\"Cheap-Nike_Shoes-" + group_field.Table.Rows[i]["id"].ToString().Trim() + "\" /><br/><br/>";
                    //}
                    string ls_up = "update usa_nike_b3 set bigimghtml = bigimghtml+'" + dog + "'  where id='" + group_field.Table.Rows[i]["pid"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up);

                }
            }


            MessageBox.Show("OK!");
        }

        private void button48_Click(object sender, EventArgs e)
        {



            DataGroup group_list = new DataGroup();
            DataGroup group_product_images = new DataGroup();


            string ls_kw_list = "select ID from jade_b3   order by newid() ";
            group_list = null;
            group_list = DB.GetDataGroup(ls_kw_list);



            string ls_group_product_images = "select id from jade_b3 where 产品数量=1 order by id desc ";
            group_product_images = DB.GetDataGroup(ls_group_product_images);
            if (group_product_images.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_product_images.Table.Rows.Count; x++)
                {


                    string ls_up = "UPDATE jade_b3 SET mid='" + group_list.Table.Rows[x]["ID"].ToString() + "' WHERE ID='" + group_product_images.Table.Rows[x]["ID"].ToString() + "'  ";
                    DB.ExecuteSQL(ls_up);

                }
            }



            DataGroup group_mdate = new DataGroup();
            string ls_group_mdate = "select MID from jade_b3 where 产品数量=1 order by mid ";
            group_mdate = null;
            group_mdate = DB.GetDataGroup(ls_group_mdate);
            if (group_mdate.Table.Rows.Count > 0)
            {

                DateTime ls_date = DateTime.Parse("2010-9-29");

                for (int i = 0; i < group_mdate.Table.Rows.Count; i++)
                {
                    string ls_up = "UPDATE jade_b3 SET mdate='" + ls_date.ToString() + "' WHERE MID='" + group_mdate.Table.Rows[i]["MID"].ToString() + "'  ";
                    DB.ExecuteSQL(ls_up);


                    if (i % 100 == 0)
                    {
                        ls_date = ls_date.AddDays(1);
                    }

                }
            }




            MessageBox.Show("OK!");


        }

        private void button49_Click(object sender, EventArgs e)
        {
            DataGroup group_html = new DataGroup();
            group_html = null;

            string ls_sql = "SELECT ID,URL FROM BLOG_A1";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["URL"].ToString().Trim();

                    string ls_Value = "";

                    try
                    {

                        ls_Value = OperateStr_Adv(ls_shtml, "http://", "/", "", 1, 0, 0, "", "", "", "");

                    }
                    catch
                    {
                        ls_Value = "";
                    }

                    string ls_up = "UPDATE BLOG_A1 SET f4='" + ls_Value + "'  WHERE ID='" + group_html.Table.Rows[i]["ID"].ToString() + "'";
                    DB.ExecuteSQL(ls_up);

                }

            }





            group_html = null;

            ls_sql = "SELECT ID,f4 FROM BLOG_A1";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["f4"].ToString().Trim();

                    string ls_Value = "";


                    //如果含2个. 则去掉含.前面的
                    int x = ls_shtml.Length;
                    int y = ls_shtml.Replace(".", "").Length;
                    int z = x - y;

                    if (z > 1)
                    {
                        int intLength = ls_shtml.Length;
                        int startIndex = ls_shtml.IndexOf('.');

                        ls_Value = ls_shtml.Substring(startIndex + 1, intLength - startIndex - 1);

                    }
                    else
                    {
                        ls_Value = ls_shtml;
                    }

                    string ls_up = "UPDATE BLOG_A1 SET f5='" + ls_Value + "'  WHERE ID='" + group_html.Table.Rows[i]["ID"].ToString() + "'";
                    DB.ExecuteSQL(ls_up);

                }

            }


            MessageBox.Show("ok!");
        }

        private void button50_Click(object sender, EventArgs e)
        {

            DataGroup group = new DataGroup();
            DataGroup group_files = new DataGroup();

            string ls_group = "select distinct pid from chun_pic WHERE STATUS=4 ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {



                    string ls_group_files = "select  id   from chun_pic where pid= '" + group.Table.Rows[i]["pid"].ToString().Trim() + "'  AND STATUS=4 ";
                    group_files = DB.GetDataGroup(ls_group_files);
                    if (group_files.Table.Rows.Count > 0)
                    {
                        for (int x = 0; x < group_files.Table.Rows.Count; x++)
                        {

                            FileCoppy("d:\\Imgs\\chun2010\\" + group_files.Table.Rows[x]["id"].ToString().Trim() + ".jpg", "D:\\CHUNOK3\\" + group.Table.Rows[i]["pid"].ToString().Trim() + "\\", group_files.Table.Rows[x]["id"].ToString().Trim() + ".jpg");

                        }
                    }





                }
            }



            /*


                    FileCoppy2("e:\\jf\\" + group_files.Table.Rows[x]["id"].ToString().Trim() + ".jpg", "e:\\JFOK\\", group_files.Table.Rows[x]["url_ename"].ToString().Trim());




            */

            MessageBox.Show("OK!!");
        }

        private void button51_Click(object sender, EventArgs e)
        {
            /*update chun_pic set status=3 where status !=0  and id 
              not in
              (
              SELECT MIN(id) AS id FROM chun_pic where status !=0 GROUP BY url_pic 
              )*/

            DataGroup group_field = new DataGroup();
            group_field = null;

            //string ls_sql = "select id,replace(pic_s_ename,'/','\\') as pic_s_ename from jade_b3 where mid is not null ";

            //good
            //string ls_sql = "select jade_pic.id,replace(url_ename,'/','\\') as pic_s_ename from jade_b3 ,jade_pic  where jade_b3.id=jade_pic.pid and mid is not null and flag='B' ";


            string ls_sql = "select min(id) as id from pick_pic where pid in (SELECT id from pick_if)group by pid";


            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {
                    // FileCoppy4("e:\\pickyourshoes\\" + group_field.Table.Rows[i]["id"].ToString().Trim()+".jpg", "d:\\pickyourshoes-eg\\", group_field.Table.Rows[i]["id"].ToString().Trim()+".jpg");

                    File.Copy("e:\\pickyourshoes\\" + group_field.Table.Rows[i]["id"].ToString().Trim() + ".jpg", "e:\\pickyourshoes-eg\\" + group_field.Table.Rows[i]["id"].ToString().Trim() + ".jpg", true);

                }
            }

            MessageBox.Show("OK!!");

        }

        public static void FileCoppy3(string OrignFile, string NewPath, string NewFile)
        {
            string iNewPath = NewPath;
            string iNewFile = NewFile;

            string jNewpath = NewPath + NewFile;

            //  d:\\aa\bb\\cc.jpg    str.substring(0,str.lastIndexOf(",")) 

            jNewpath = jNewpath.Substring(0, jNewpath.LastIndexOf("\\"));

            //int temp_left = showNewFile.IndexOf('\\');
            //NewFile = showNewFile.Substring(temp_left + 1, showNewFile.Length - temp_left - 1);
            //NewPath = NewPath + showNewFile.Replace("/" + NewFile, "");


            if (!Directory.Exists(jNewpath))
                Directory.CreateDirectory(jNewpath);

            File.Copy(OrignFile, iNewPath + iNewFile, true);
        }

        private void button52_Click(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;



            string ls_sql = "SELECT ID,  URL FROM kick_n1 WHERE STATUS IS NULL  order by ID";//IS NULL
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                //L_M2.Text = group.Table.Rows.Count.ToString();

                for (int i = 0; i < group.Table.Rows.Count; i++)
                {

                    string pig = getUrlSource2(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312

                    //pig = HttpUtility.HtmlEncode(pig);

                    //L_M1.Text = i.ToString();

                    if (pig != "")
                    {
                        string ls_up = "UPDATE kick_n1 SET HTML='" + pig + "' ,STATUS=1 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);

                        pig = "";
                    }

                    //抓google
                    //System.Threading.Thread.Sleep(2000);
                    //UPDATE CMS_B2 SET kw=substring(name2,1,CHARINDEX(' ',name2))


                }
            }

            MessageBox.Show("ok!");



            /*
            string PageUrl = "http://www.kicksonnike.com/air-max-air-max-90_c140";
            WebClient wc = new WebClient();
            wc.Credentials = CredentialCache.DefaultCredentials;
            ///方法一：
            Byte[] pageData = wc.DownloadData(PageUrl);
            string bbbb = Encoding.Default.GetString(pageData);  
             


            ///方法2
            WebClient wc = new WebClient();
            Stream resStream = wc.OpenRead("http://www.kicksonnike.com/air-max-air-max-90_c140");
            StreamReader sr = new StreamReader(resStream, System.Text.Encoding.Default);
            string bbbb = sr.ReadToEnd();
            resStream.Close();
             * 
             * 
             * 
             * 
             * Request.ServerVariables(“HTTP_REFERER“) )
分析：asp和php可以通过读取请求的HTTP_REFERER属性，来判断该请求是否来自本网站，从而来限制采集器，同样也限制了搜索引擎爬虫，严重影响搜索引擎对网站部分防盗链内容的收录。
适用网站：不太考虑搜索引擎收录的网站
采集器会怎么做：伪装HTTP_REFERER嘛
             * 
             * 

*/

        }
        /*
                  /// <summary>
                  /// 返回指定Url的IE窗口下的 IHTMLDocument2 对象。
                  /// </summary>
                  /// <returns>IHTMLDocument2</returns>
                  public static mshtml.IHTMLDocument2 GetIHTMLDocument2ByUrl(string url)
                  {
                      SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
                      foreach (SHDocVw.InternetExplorer ie in shellWindows)
                      {
                          string filename = System.IO.Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
                          if (filename.Equals("iexplore") && ie.LocationURL == url)
                          {
                              return ie.Document as mshtml.IHTMLDocument2;

                          }
                          else
                          {
                              return null;
                          }
                      }


                  }

        */
        private void button53_Click(object sender, EventArgs e)
        {
            SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
            foreach (SHDocVw.InternetExplorer ie in shellWindows)
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
                //if (filename.Equals("iexplore") && ie.LocationURL == url)
                if (filename.Equals("iexplore"))
                {
                    object url = "http://www.kicksonnike.com/air-max-air-max-90_c140";
                    object oEmpty = "";
                    ie.Navigate2(ref url, ref oEmpty, ref oEmpty, ref oEmpty, ref oEmpty);

                    mshtml.IHTMLDocument2 htmlDoc = ie.Document as mshtml.IHTMLDocument2;

                    //string ls_html = "" + htmlDoc.forms.;
                    string ls_html = htmlDoc.body.outerHTML.Substring(0, htmlDoc.body.outerHTML.Length);

                }

            }

        }

        private void button54_Click(object sender, EventArgs e)
        {
            try
            {

                string sql = "CREATE TABLE [" + textBox12.Text.Trim() + "]( " +
                  " [ID] [bigint] IDENTITY(1,1) NOT NULL," +
                  " [MAIN_CLASS] [nvarchar](255)  NULL," +
                  " [SUB_CLASS] [nvarchar](255)  NULL," +
                  " [URL] [nvarchar](255)  NULL," +
                  " [SUB_URL] [nvarchar](255)  NULL," +
                  " [HTML] [text]  NULL," +
                  " [STATUS] [nvarchar](1)  NULL," +
                  " CONSTRAINT [PK_" + textBox12.Text.Trim() + "] PRIMARY KEY CLUSTERED " +
                  " (" +
                  " 	[ID] ASC" +
                  " )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]" +
                  " ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

                if (checkBox1.Checked)
                {
                    sql = "drop table " + textBox12.Text.Trim() + "; " + sql;
                }

                DB.ExecuteSQL(sql);
            }
            catch
            {
                MessageBox.Show("DataBase is Exist!!");
                return;
            }

            string pig = "";

            // pig = getUrlSource(textBox4.Text.Trim().ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312
            // pig = pig.Replace("\"", "").Replace("'", "");

            /**/
            SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
            foreach (SHDocVw.InternetExplorer ie in shellWindows)
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
                if (filename.Equals("iexplore"))
                {
                    object url = textBox4.Text.Trim().ToString();
                    object oEmpty = "";
                    ie.Navigate2(ref url, ref oEmpty, ref oEmpty, ref oEmpty, ref oEmpty);
                    mshtml.IHTMLDocument2 htmlDoc = ie.Document as mshtml.IHTMLDocument2;

                    System.Threading.Thread.Sleep(5000);

                    try
                    {
                        pig = htmlDoc.body.outerHTML.Substring(0, htmlDoc.body.outerHTML.Length);
                    }
                    catch
                    {
                        pig = "";
                    }

                    pig = pig.Replace("\"", "").Replace("'", "");
                }



                if (pig != "")
                {


                    string ls_shtml = pig.Replace("\r\n", "").ToString();//替换换行符
                    ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace(textBox7.Text.Trim().Replace("\"", "").Replace("'", ""), "|");//将分隔码 替换成“|”
                    string[] dog_small = ls_shtml.Split('|');//子串

                    int pd = 0;

                    foreach (string bb in dog_small)
                    {
                        if (pd > 0)
                        {

                            string ls_Value_link = "";
                            string ls_Value_text = "";


                            //-----------------------------------------------------------------------------------------------------
                            try
                            {
                                ls_Value_text = OperateStr(bb, textBox11.Text.Trim().Replace("\"", "").Replace("'", ""), textBox10.Text.Trim().Replace("\"", "").Replace("'", ""), "", 1, 0, 0, "0");
                            }
                            catch
                            {
                                ls_Value_text = "";
                            }


                            try
                            {
                                ls_Value_link = OperateStr(bb, textBox9.Text.Trim().Replace("\"", "").Replace("'", ""), textBox8.Text.Trim().Replace("\"", "").Replace("'", ""), "", 1, 0, 0, "0");
                            }
                            catch
                            {
                                ls_Value_link = "";
                            }

                            string ls_up = "insert into " + textBox12.Text.Trim() + " (main_class,url,status)" +
                                   " values ('" + ls_Value_text + "','" + ls_Value_link + "'  ,1 )";
                            DB.ExecuteSQL(ls_up);

                        }

                        pd += 1;

                    }
                }











            }


        }

        private void button55_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(zzzzzzzzzzzzzzzzeeeeeeeeeeeeeennnnnnnnnnnnn);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;


            thread.Start();
        }

        public void zzzzzzzzzzzzzzzzeeeeeeeeeeeeeennnnnnnnnnnnn()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;



            string ls_sql = "SELECT ID,URL FROM rhe_B1 WHERE STATUS =1  order by ID";//IS NULL
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                //L_M2.Text = group.Table.Rows.Count.ToString();


                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = "";

                    //pig = getUrlSource(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312


                    /**/
                    SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
                    foreach (SHDocVw.InternetExplorer ie in shellWindows)
                    {
                        string filename = System.IO.Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
                        if (filename.Equals("iexplore"))
                        {
                            object url = group.Table.Rows[i]["URL"].ToString();
                            object oEmpty = "";
                            ie.Navigate2(ref url, ref oEmpty, ref oEmpty, ref oEmpty, ref oEmpty);
                            mshtml.IHTMLDocument2 htmlDoc = ie.Document as mshtml.IHTMLDocument2;

                            System.Threading.Thread.Sleep(10000);

                            try
                            {
                                pig = htmlDoc.body.outerHTML.Substring(0, htmlDoc.body.outerHTML.Length);
                            }
                            catch
                            {
                                pig = "";
                            }

                            pig = pig.Replace("\"", "").Replace("'", "");
                        }




                        //pig = HttpUtility.HtmlEncode(pig);

                        //L_M1.Text = i.ToString();

                        if (pig != "")
                        {
                            string ls_up = "UPDATE rhe_B1 SET HTML='" + pig + "' ,STATUS=2 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                            DB.ExecuteSQL(ls_up);

                            pig = "";
                        }



                    }


                    //抓google
                    //System.Threading.Thread.Sleep(2000);
                    //UPDATE CMS_B2 SET kw=substring(name2,1,CHARINDEX(' ',name2))


                }
            }

            MessageBox.Show("ok!");


        }

        private void button56_Click(object sender, EventArgs e)
        {
            try
            {

                string sql = "CREATE TABLE [" + textBox18.Text.Trim() + "]( " +
                  " [ID] [bigint] IDENTITY(1,1) NOT NULL," +
                  " [PID] [bigint] NULL," +
                  " [NB] [bigint] NULL," +
                  " [MAIN_CLASS] [nvarchar](255)  NULL," +
                  " [SUB_CLASS] [nvarchar](255)  NULL," +
                  " [URL] [nvarchar](255)  NULL," +
                  " [SUB_URL] [nvarchar](255)  NULL," +
                  " [HTML] [text]  NULL," +
                  " [STATUS] [nvarchar](1)  NULL," +
                  " CONSTRAINT [PK_" + textBox18.Text.Trim() + "] PRIMARY KEY CLUSTERED " +
                  " (" +
                  " 	[ID] ASC" +
                  " )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]" +
                  " ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

                if (checkBox2.Checked)
                {
                    sql = "drop table " + textBox18.Text.Trim() + "; " + sql;
                }

                DB.ExecuteSQL(sql);
            }
            catch
            {
                MessageBox.Show("DataBase is Exist!!");
                return;
            }





            DataGroup group = new DataGroup();

            DataGroup group_html = new DataGroup();
            group_html = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group_html = null;

            string ls_sql = "SELECT ID,HTML,MAIN_CLASS,URL FROM rhe_B1 WHERE STATUS =2 order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["HTML"].ToString().Replace("\r\n", "").ToString();//替换换行符
                    ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace(textBox15.Text.Trim().Replace("\"", "").Replace("'", ""), "|");//将分隔码 替换成“|”
                    string[] dog_small = ls_shtml.Split('|');//子串


                    if (dog_small.Length == 1)
                    {
                        string ls_up = "INSERT INTO " + textBox18.Text.Trim() + " (PID, MAIN_CLASS,SUB_CLASS,SUB_URL,STATUS) " +
                                " values ('" + group_html.Table.Rows[i]["ID"].ToString() + "','" + group_html.Table.Rows[i]["MAIN_CLASS"].ToString() + "','','" + group_html.Table.Rows[i]["URL"].ToString() + "',1)";
                        DB.ExecuteSQL(ls_up);

                    }
                    else
                    {
                        int pd = 0;

                        foreach (string bb in dog_small)
                        {
                            if (pd > 0)
                            {

                                string ls_SubValue_link = "";
                                string ls_SubValue_text = "";

                                //-----------------------------------------------------------------------------------------------------

                                try
                                {
                                    ls_SubValue_link = OperateStr(bb, textBox14.Text.Trim().Replace("\"", "").Replace("'", ""), textBox13.Text.Trim().Replace("\"", "").Replace("'", ""), "", 1, 0, 0, "0");
                                }
                                catch
                                {
                                    ls_SubValue_link = "";
                                }


                                try
                                {
                                    ls_SubValue_text = OperateStr(bb, textBox17.Text.Trim().Replace("\"", "").Replace("'", ""), textBox16.Text.Trim().Replace("\"", "").Replace("'", ""), "", 1, 0, 0, "0");
                                }
                                catch
                                {
                                    ls_SubValue_text = "";
                                }



                                string ls_up = "INSERT INTO " + textBox18.Text.Trim() + " (PID, MAIN_CLASS,SUB_CLASS,SUB_URL,STATUS) " +
                                    " values ('" + group_html.Table.Rows[i]["ID"].ToString() + "','" + group_html.Table.Rows[i]["MAIN_CLASS"].ToString() + "', '" + ls_SubValue_text + "','" + ls_SubValue_link + "',1)";
                                DB.ExecuteSQL(ls_up);


                                ls_SubValue_link = "";
                                ls_SubValue_text = "";


                            }
                            group = null;
                            //-----------------------------------------------------------------------------------------------------

                            pd += 1;

                        }

                    }
                }

            }

            MessageBox.Show("OK!!");

        }

        private void button57_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(zzzzzzzzzzzzzzzzeeeeeeeeeeeeeennnnnnnnnnnnn2);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;


            thread.Start();
        }


        public void zzzzzzzzzzzzzzzzeeeeeeeeeeeeeennnnnnnnnnnnn2()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;



            string ls_sql = "SELECT ID,sub_url as URL FROM rhe_B2 WHERE STATUS =1  order by ID";//IS NULL
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                //L_M2.Text = group.Table.Rows.Count.ToString();


                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = "";

                    //pig = getUrlSource(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312


                    /**/
                    SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
                    foreach (SHDocVw.InternetExplorer ie in shellWindows)
                    {
                        string filename = System.IO.Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
                        if (filename.Equals("iexplore"))
                        {
                            object url = group.Table.Rows[i]["URL"].ToString();
                            object oEmpty = "";
                            ie.Navigate2(ref url, ref oEmpty, ref oEmpty, ref oEmpty, ref oEmpty);
                            mshtml.IHTMLDocument2 htmlDoc = ie.Document as mshtml.IHTMLDocument2;

                            System.Threading.Thread.Sleep(10000);

                            try
                            {
                                pig = htmlDoc.body.outerHTML.Substring(0, htmlDoc.body.outerHTML.Length);
                            }
                            catch
                            {
                                pig = "";
                            }

                            pig = pig.Replace("\"", "").Replace("'", "");
                        }




                        //pig = HttpUtility.HtmlEncode(pig);

                        //L_M1.Text = i.ToString();

                        if (pig != "")
                        {
                            string ls_up = "UPDATE rhe_B2 SET HTML='" + pig + "' ,STATUS=2 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                            DB.ExecuteSQL(ls_up);

                            pig = "";
                        }



                    }


                    //抓google
                    //System.Threading.Thread.Sleep(2000);
                    //UPDATE CMS_B2 SET kw=substring(name2,1,CHARINDEX(' ',name2))


                }
            }

            MessageBox.Show("ok!");


        }

        private void button58_Click(object sender, EventArgs e)
        {


            DataGroup group_html = new DataGroup();
            group_html = null;

            string ls_sql = "SELECT ID,HTML FROM shoestrader_b2 WHERE STATUS =2 order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["HTML"].ToString().Replace("\r\n", "").ToString();//替换换行符
                    ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”


                    string ls_Value = "";
                    //-----------------------------------------------------------------------------------------------------

                    try
                    {

                        //ls_Value = OperateStr_Adv(ls_shtml, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, group.Table.Rows[j]["CYCLE"].ToString(), group.Table.Rows[j]["STR_AREA"].ToString(), group.Table.Rows[j]["STR_AFT"].ToString(), group.Table.Rows[j]["STR_BEF"].ToString());
                        ls_Value = OperateStr(ls_shtml, textBox20.Text.Trim().Replace("\"", "").Replace("'", ""), textBox19.Text.Trim().Replace("\"", "").Replace("'", ""), "", 1, 0, 0, "0");

                    }
                    catch
                    {
                        ls_Value = "";
                    }

                    string ls_up = "UPDATE shoestrader_b2 SET nb='" + ls_Value + "' ,STATUS=4 WHERE ID='" + group_html.Table.Rows[i]["ID"].ToString() + "'";

                    //string ls_up = "insert into pick_p2 (flag,pid,url_pic) values ('B','" + group_html.Table.Rows[i]["ID"].ToString() + "','" + ls_Value + "')";

                    DB.ExecuteSQL(ls_up);


                    //-----------------------------------------------------------------------------------------------------


                }

            }

            MessageBox.Show("OK");


        }

        private void button59_Click(object sender, EventArgs e)
        {
            //?page=2&sort=20a


            try
            {

                string sql = "CREATE TABLE [" + textBox21.Text.Trim() + "]( " +
                  " [ID] [bigint] IDENTITY(1,1) NOT NULL," +
                  " [PID] [bigint] NULL," +
                  " [NB] [bigint] NULL," +
                  " [MAIN_CLASS] [nvarchar](255)  NULL," +
                  " [Big_CLASS] [nvarchar](255)  NULL," +
                  " [SUB_CLASS] [nvarchar](255)  NULL," +
                  " [URL] [nvarchar](255)  NULL," +
                  " [SUB_URL] [nvarchar](255)  NULL," +
                  " [HTML] [text]  NULL," +
                  " [STATUS] [nvarchar](1)  NULL," +
                  " CONSTRAINT [PK_" + textBox21.Text.Trim() + "] PRIMARY KEY CLUSTERED " +
                  " (" +
                  " 	[ID] ASC" +
                  " )WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]" +
                  " ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";

                if (checkBox3.Checked)
                {
                    sql = "drop table " + textBox21.Text.Trim() + "; " + sql;
                }

                DB.ExecuteSQL(sql);
            }
            catch
            {
                MessageBox.Show("DataBase is Exist!!");
                return;
            }



            DataGroup group_html = new DataGroup();
            group_html = null;

            string ls_sql = "SELECT ID,1 as BIG_CLASS,MAIN_CLASS,SUB_CLASS,url as SUB_URL,NB,HTML FROM usa_nike_b2 WHERE STATUS is null order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    int LS_NB = int.Parse(group_html.Table.Rows[i]["NB"].ToString().Trim());


                    if (LS_NB <= 28)
                    {

                        string ls_up = "INSERT INTO " + textBox21.Text.Trim() + " (PID,BIG_CLASS, MAIN_CLASS,SUB_CLASS,SUB_URL,HTML,STATUS) " +
                                        " values ('" + group_html.Table.Rows[i]["ID"].ToString() + "','" + group_html.Table.Rows[i]["BIG_CLASS"].ToString() + "','" + group_html.Table.Rows[i]["MAIN_CLASS"].ToString() + "','" + group_html.Table.Rows[i]["SUB_CLASS"].ToString() + "','" + group_html.Table.Rows[i]["SUB_URL"].ToString() + "','" + group_html.Table.Rows[i]["HTML"].ToString() + "',9)";
                        DB.ExecuteSQL(ls_up);

                    }
                    else
                    {
                        //?page=2&sort=20a


                        int xman = LS_NB / 28 + 1;


                        for (int x = 1; x <= xman; x++)
                        {

                            if (x == 1)
                            {

                                string ls_up = "INSERT INTO " + textBox21.Text.Trim() + " (PID, big_class,MAIN_CLASS,SUB_CLASS,SUB_URL,HTML,STATUS) " +
                                             " values ('" + group_html.Table.Rows[i]["ID"].ToString() + "','" + group_html.Table.Rows[i]["BIG_CLASS"].ToString() + "','" + group_html.Table.Rows[i]["MAIN_CLASS"].ToString() + "','" + group_html.Table.Rows[i]["SUB_CLASS"].ToString() + "','" + group_html.Table.Rows[i]["SUB_URL"].ToString() + "','" + group_html.Table.Rows[i]["HTML"].ToString() + "',9)";
                                DB.ExecuteSQL(ls_up);

                            }
                            else
                            {
                                string T_url = "?page=" + x.ToString() + "&sort=20a";

                                //string T_url = "?p=" + x.ToString();

                                string ls_up = "INSERT INTO " + textBox21.Text.Trim() + " (PID, big_class,MAIN_CLASS,SUB_CLASS,SUB_URL,HTML,STATUS) " +
                                             " values ('" + group_html.Table.Rows[i]["ID"].ToString() + "','" + group_html.Table.Rows[i]["BIG_CLASS"].ToString() + "','" + group_html.Table.Rows[i]["MAIN_CLASS"].ToString() + "','" + group_html.Table.Rows[i]["SUB_CLASS"].ToString() + "','" + group_html.Table.Rows[i]["SUB_URL"].ToString() + T_url + "','',0)";
                                DB.ExecuteSQL(ls_up);


                            }

                        }

                    }





                }

            }

            MessageBox.Show("OK");


        }

        private void button60_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(zzzzzzzzzzzzzzzzeeeeeeeeeeeeeennnnnnnnnnnnn3);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;


            thread.Start();
        }

        public void zzzzzzzzzzzzzzzzeeeeeeeeeeeeeennnnnnnnnnnnn3()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;



            string ls_sql = "SELECT ID,sub_url as URL FROM shoestrader_b3 WHERE STATUS =0  order by ID";//IS NULL
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                //L_M2.Text = group.Table.Rows.Count.ToString();


                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = "";

                    //pig = getUrlSource(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312


                    /**/
                    SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
                    foreach (SHDocVw.InternetExplorer ie in shellWindows)
                    {
                        string filename = System.IO.Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
                        if (filename.Equals("iexplore"))
                        {
                            object url = group.Table.Rows[i]["URL"].ToString();
                            object oEmpty = "";
                            ie.Navigate2(ref url, ref oEmpty, ref oEmpty, ref oEmpty, ref oEmpty);
                            mshtml.IHTMLDocument2 htmlDoc = ie.Document as mshtml.IHTMLDocument2;

                            System.Threading.Thread.Sleep(15000);

                            try
                            {
                                pig = htmlDoc.body.outerHTML.Substring(0, htmlDoc.body.outerHTML.Length);
                            }
                            catch
                            {
                                pig = "";
                            }

                            pig = pig.Replace("\"", "").Replace("'", "");
                        }




                        //pig = HttpUtility.HtmlEncode(pig);

                        //L_M1.Text = i.ToString();

                        if (pig != "")
                        {
                            string ls_up = "UPDATE shoestrader_b3 SET HTML='" + pig + "' ,STATUS=2 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                            DB.ExecuteSQL(ls_up);

                            pig = "";
                        }



                    }


                    //抓google
                    //System.Threading.Thread.Sleep(2000);
                    //UPDATE CMS_B2 SET kw=substring(name2,1,CHARINDEX(' ',name2))


                }
            }

            MessageBox.Show("ok!");


        }

        private void button61_Click(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();

            DataGroup group_html = new DataGroup();
            group_html = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group_html = null;

            string ls_sql = "SELECT top 100 ID,HTML,big_class,main_class,sub_class FROM shoestrader_b3  where status=2 order by ID";//WHERE STATUS =1
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["HTML"].ToString().Replace("\r\n", "").ToString();//替换换行符
                    ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("<DIV class=productlistingdesc><A href", "|");//将分隔码 替换成“|”

                    string[] dog_small = ls_shtml.Split('|');//子串


                    int xxx = dog_small.Length - 1;

                    string ls_up2 = "update shoestrader_b3 set nb='" + xxx + "' , status=6 where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up2);



                    int pd = 0;

                    foreach (string bb in dog_small)
                    {
                        if (pd > 0)
                        {

                            string ls_FILED = "";
                            string ls_Value = "";

                            //-----------------------------------------------------------------------------------------------------
                            group = null;
                            string ls_rule = "SELECT RID,FILED,OID,CYCLE,STRING_S,STRING_E,STRING_NEW,INCLUDE_S,INCLUDE_E FROM URL_RULE WHERE siteid=100 and STATUS=1";
                            group = DB.GetDataGroup(ls_rule);
                            if (group.Table.Rows.Count > 0)
                            {
                                for (int j = 0; j < group.Table.Rows.Count; j++)
                                {
                                    try
                                    {
                                        ls_Value += ",'" + OperateStr(bb, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, "0") + "'";
                                    }
                                    catch
                                    {
                                        ls_Value = "," + " ";
                                    }

                                    ls_FILED += "," + group.Table.Rows[j]["FILED"].ToString();


                                }

                                string ls_up = "INSERT INTO shoestrader_b4 (PID " + ls_FILED + ",STATUS,main_class,sub_class,big_class) values ('" + group_html.Table.Rows[i]["ID"].ToString() + "' " + ls_Value + ",1,'" + group_html.Table.Rows[i]["main_class"].ToString() + "','" + group_html.Table.Rows[i]["sub_class"].ToString() + "','" + group_html.Table.Rows[i]["big_class"].ToString() + "')";
                                DB.ExecuteSQL(ls_up);

                                ls_Value = "";
                                ls_FILED = "";


                            }
                            group = null;
                            //-----------------------------------------------------------------------------------------------------
                        }
                        pd += 1;


                    }





                }

                MessageBox.Show("OK!!");
            }

        }

        private void button62_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(zzzzzzzzzzzzzzzzeeeeeeeeeeeeeennnnnnnnnnnnn4);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;


            thread.Start();
        }

        public void zzzzzzzzzzzzzzzzeeeeeeeeeeeeeennnnnnnnnnnnn4()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;



            string ls_sql = "SELECT ID,url_product as URL FROM shoestrader_b4 WHERE STATUS =2  order by ID";//IS NULL
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                //L_M2.Text = group.Table.Rows.Count.ToString();


                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = "";

                    //pig = getUrlSource(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312


                    /**/
                    SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindowsClass();
                    foreach (SHDocVw.InternetExplorer ie in shellWindows)
                    {
                        string filename = System.IO.Path.GetFileNameWithoutExtension(ie.FullName).ToLower();
                        if (filename.Equals("iexplore"))
                        {
                            object url = group.Table.Rows[i]["URL"].ToString();
                            object oEmpty = "";
                            ie.Navigate2(ref url, ref oEmpty, ref oEmpty, ref oEmpty, ref oEmpty);
                            mshtml.IHTMLDocument2 htmlDoc = ie.Document as mshtml.IHTMLDocument2;

                            System.Threading.Thread.Sleep(6000);

                            try
                            {
                                pig = htmlDoc.body.outerHTML.Substring(0, htmlDoc.body.outerHTML.Length);
                            }
                            catch
                            {
                                pig = "";
                            }


                            /*All Categories

                             * 
                             *  int intLength = str.Length;//【HTML总长度】
            int startIndex = str.IndexOf(startstr);//【开始字符串】在【HTML】位置


            if (startIndex == -1)
            {
                return "";
            }

            //int endIndex = str.IndexOf(endstr);
            int x = startstr.Length;//【开始字符串】的长度
            string cat = str.Substring(0, startIndex + x);//从【开头】到【开始字符串】

            cat = str.Replace(cat, "");//【目标字符串】后到【尾巴】
            int end_len = cat.IndexOf(endstr);//【目标字符串】的长度

                             * 
                             * 
                             * 
                            
                            */
                            string startstr = "<DIV id=topTags>";
                            int intLength = pig.Length;//【HTML总长度】
                            int startIndex = pig.IndexOf(startstr);//【开始字符串】在【HTML】位置
                            if (startIndex != -1)
                            {
                                int x = startstr.Length;//【开始字符串】的长度
                                string cat = pig.Substring(0, startIndex + x);//从【开头】到【开始字符串】
                                cat = pig.Replace(cat, "");//【目标字符串】后到【尾巴】
                                pig = cat.Replace("\"", "").Replace("'", "");
                            }
                            else
                            {
                                pig = pig.Replace("\"", "").Replace("'", "");
                            }


                        }




                        //pig = HttpUtility.HtmlEncode(pig);

                        //L_M1.Text = i.ToString();

                        if (pig != "")
                        {
                            string ls_up = "UPDATE shoestrader_b4 SET HTML='" + pig + "' ,STATUS=3 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                            DB.ExecuteSQL(ls_up);

                            pig = "";
                        }



                    }


                    //抓google
                    //System.Threading.Thread.Sleep(2000);
                    //UPDATE CMS_B2 SET kw=substring(name2,1,CHARINDEX(' ',name2))

                }
            }

            MessageBox.Show("ok!");


        }

        private void button63_Click(object sender, EventArgs e)
        {


            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;



            string ls_sql = "SELECT top 100 ID,html FROM shoestrader_b4 WHERE STATUS =3  order by ID";//IS NULL
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = group.Table.Rows[i]["html"].ToString();


                    string startstr = "<DIV id=topTags>";
                    int intLength = pig.Length;//【HTML总长度】
                    int startIndex = pig.IndexOf(startstr);//【开始字符串】在【HTML】位置
                    if (startIndex != -1)
                    {
                        int x = startstr.Length;//【开始字符串】的长度
                        string cat = pig.Substring(0, startIndex + x);//从【开头】到【开始字符串】
                        cat = pig.Replace(cat, "");//【目标字符串】后到【尾巴】

                        //pig = cat.Replace("\"", "").Replace("'", "");

                        string ls_up = "UPDATE shoestrader_b4 SET HTML='" + cat + "' ,STATUS=4 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);
                    }




                }
            }

            MessageBox.Show("ok!");


        }

        private void button160_Click(object sender, EventArgs e)
        {
            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT ID, description as remark FROM plan425_b2 order by ID";// product_name,description
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {

                    string ls_shtml = group_html.Table.Rows[i]["remark"].ToString().Replace("\n", "");//.Replace("\t", "<br/>").Replace("<br/><br/>", "<br/>")

                    int start_X = ls_shtml.IndexOf("<hr>");
                    int start_Y = ls_shtml.LastIndexOf("Fearture:");

                    if (start_X > 0)
                    {
                        string ls_cut = ls_shtml.Substring(start_X, start_Y - start_X  );// 加上<hr>的长度

                        ls_shtml = ls_shtml.Replace(ls_cut, "") + "<br/>" + ls_cut;

                        string ls_up2 = "update plan425_b2 set description='" + ls_shtml + "'  where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                        DB.ExecuteSQL(ls_up2);

                    }
                }

            }


            MessageBox.Show("OK!!");
        }
        private void button64_Click(object sender, EventArgs e)
        {
            //l_html.Text = NoHTML(l_html.Text);

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT ID, description as remark FROM plan425_b2 where status=2 order by ID";//WHERE STATUS =1   where status=3 product_name,description
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {

                   string ls_shtml = group_html.Table.Rows[i]["remark"].ToString().Replace("\n", "");

                    ls_shtml = group_html.Table.Rows[i]["remark"].ToString().Replace("\t", "<br/>");

                   // string ls_shtml = ReMoveElement(group_html.Table.Rows[i]["remark"].ToString(), "img");
                   //NoHTML 
                    //开头  -> <strong>Material</strong>

                    int start = ls_shtml.IndexOf("<br/>Paiement"); ////int start = ls_shtml.IndexOf("<br/>This<br/>");   //<br/>Each
                   //int start = ls_shtml.LastIndexOf(" ");

                   if (start > 0)
                   {
                       ls_shtml = ls_shtml.Substring(0, start -0 );

                       //ls_shtml = ls_shtml.Substring(start+0, ls_shtml.Length-start-1);

                       string ls_up2 = "update plan425_b2 set description='" + ls_shtml + "'  where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up2);
                   
                   }
                }

            }


            MessageBox.Show("OK!!");

        }

        private void button65_Click(object sender, EventArgs e)
        {

            DataGroup group_main_class = new DataGroup();
            string ls_group_main_class = "select id,a,b,spinner from a_gp4 where flag=1 and status=1  ";
            group_main_class = DB.GetDataGroup(ls_group_main_class);
            if (group_main_class.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_main_class.Table.Rows.Count; x++)
                {
                    DataGroup group_kw = new DataGroup();
                    group_kw = null;
                    string ls_group_kw = "select id from a_gp4 where id='" + group_main_class.Table.Rows[x]["id"].ToString().Trim() + "'   order by id ";
                    group_kw = DB.GetDataGroup(ls_group_kw);

                    if (group_kw.Table.Rows.Count > 0)
                    {
                        for (int k = 0; k < group_kw.Table.Rows.Count; k++)
                        {

                            //int pig = k + 1;

                            //if (pig > 1)
                            //{
                            string ls_up = "UPDATE a_gp4 SET tag='" + x + "'  WHERE id='" + group_kw.Table.Rows[k]["id"].ToString() + "' ";
                            DB.ExecuteSQL(ls_up);

                            //}

                        }
                    }

                }

            }


            MessageBox.Show("ok!");
        }

        private void button66_Click(object sender, EventArgs e)
        {
            DataGroup group_files = new DataGroup();

            string ls_group_files = "select  y.id as ID  from  rhe_pic y where   y.flag='B' ";
            group_files = DB.GetDataGroup(ls_group_files);
            if (group_files.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_files.Table.Rows.Count; x++)
                {
                    try
                    {
                        File.Copy("e:\\rhe\\" + group_files.Table.Rows[x]["id"].ToString().Trim() + ".jpg", "e:\\rheOK\\B\\xx-" + group_files.Table.Rows[x]["id"].ToString().Trim() + ".jpg", true);
                    }
                    catch
                    { }

                }
            }


            /*
             DataGroup group_files = new DataGroup();

             string ls_group_files = "select id,url_ename from jade_pic where flag='B' ";
             group_files = DB.GetDataGroup(ls_group_files);
             if (group_files.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_files.Table.Rows.Count; x++)
                {

                    FileCoppy2("e:\\jf\\" + group_files.Table.Rows[x]["id"].ToString().Trim() + ".jpg", "e:\\JFOK\\", group_files.Table.Rows[x]["url_ename"].ToString().Trim());



                }
            }

            */

            MessageBox.Show("OK!!");
        }

        private void button67_Click(object sender, EventArgs e)
        {

            DataGroup group_main_class = new DataGroup();
            string ls_group_main_class = "select id,product_name  as ppp from plan425_b2 where status=2 ";
            group_main_class = DB.GetDataGroup(ls_group_main_class);
            if (group_main_class.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_main_class.Table.Rows.Count; x++)
                {

                    int start = group_main_class.Table.Rows[x]["ppp"].ToString().LastIndexOf(" ");
                    //int start = group_main_class.Table.Rows[x]["ppp"].ToString().IndexOf("&");

                    if (start > 0)
                    {

                        //string dog = group_main_class.Table.Rows[x]["ppp"].ToString().Substring(start + 1, group_main_class.Table.Rows[x]["main_class"].ToString().Length - start - 1);
                        string dog = group_main_class.Table.Rows[x]["ppp"].ToString().Substring(0, start);


                        string ls_up = "UPDATE plan425_b2 SET product_name='" + dog + "' WHERE id='" + group_main_class.Table.Rows[x]["id"].ToString() + "' ";
                        DB.ExecuteSQL(ls_up);
                    }

                }

            }


            MessageBox.Show("ok!");
        }

        private void button68_Click(object sender, EventArgs e)
        {

            DataGroup group = new DataGroup();

            string ls_group = "select id from rhe_pic WHERE flag='S' order by id";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {

                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    try
                    {

                        File.Copy("e:\\rheok\\s-operation\\x-" + group.Table.Rows[x]["id"].ToString().Trim() + ".jpg", "e:\\rheok\\s-operation-name\\x-" + group.Table.Rows[x]["id"].ToString().Trim() + ".jpg", true);
                    }
                    catch
                    { }
                }

            }
            MessageBox.Show("OK!!");

        }

        private void button69_Click(object sender, EventArgs e)
        {
            string LS_html = l_html.Text;

            //<a href="http://en.wikipedia.org/wiki/MoinMoin" title="MoinMoin">MoinMoin</a><span 
            //title="">LV Men&#39;s Shoes</a>


            l_html.Text = OperateStr_Adv_html(LS_html.Replace("'", "\"")
                //, "\">", "</a>", ""
                  , "\">", "</a>", ""
                                        , 1
                                        , 0, 0
                                        , "1"
                                        , ""
                                        , "", "");

        }

        private void button70_Click(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();

            string ls_group = "select id,main_class,sub_class,url,html,max from NFL_B1 where status=3";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {

                    for (int ppp = 1; ppp <= int.Parse(group.Table.Rows[x]["max"].ToString().Trim()); ppp++)
                    {
                        if (ppp == 1)
                        {
                            string ls_up = "insert into NFL_B2(pid,main_class,sub_class,url,html,status) values ('" + group.Table.Rows[x]["ID"].ToString() + "','" + group.Table.Rows[x]["main_class"].ToString() + "','" + group.Table.Rows[x]["sub_class"].ToString() + "','" + group.Table.Rows[x]["url"].ToString() + "?page=" + ppp + "','" + group.Table.Rows[x]["html"].ToString() + "',1)";
                            DB.ExecuteSQL(ls_up);
                        }
                        else
                        {

                            string ls_up = "insert into NFL_B2(pid,main_class,sub_class,url,status) values ('" + group.Table.Rows[x]["ID"].ToString() + "','" + group.Table.Rows[x]["main_class"].ToString() + "','" + group.Table.Rows[x]["sub_class"].ToString() + "','" + group.Table.Rows[x]["url"].ToString() + "?page=" + ppp + "',0)";
                            DB.ExecuteSQL(ls_up);
                        }



                    }

                }

            }
            MessageBox.Show("OK!!");
        }

        private void button71_Click(object sender, EventArgs e)
        {
            /*update a_gp4 set status=0 where flag=1 and status=1  and id 
not in
(
SELECT MIN(id) AS id FROM a_gp4 where flag=1 and status=1 GROUP BY a,b,spinner 
)*/
            DataGroup group = new DataGroup();
            DataGroup group2 = new DataGroup();

            string ls_group = "select id,a from a_gp4 where flag=1 ";//SELECT gid      ,w1      ,w2      ,w3      ,w4      ,w5      ,w6      ,w7      ,w8      ,w9      ,w10      ,w11      ,w12      ,w13      ,w14      ,w15      ,w16      ,w17      ,w18      ,w19      ,w20      ,w21      ,w22      ,w23      ,w24      ,w25      ,w26      ,w27      ,w28      ,w29      ,w30 FROM A_GP3";
            //string ls_group = "select id,Description,Packing_Method from JewelOra_b3 order by id";
            //string ls_group = "select id,url,max,main_class,sub_class from JewelOra_b3 where status=4";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {

                    group2 = null;
                    string ls_pp = "select distinct a,idx from a_gp4 where status=1 and gpid in (select gpid from [A_GP4] where status=1 and flag=1 and a='" + group.Table.Rows[x]["a"].ToString().Trim() + "')order by idx,a";
                    group2 = DB.GetDataGroup(ls_pp);
                    if (group2.Table.Rows.Count > 1)
                    {
                        string ls_spinner = "";

                        for (int i = 0; i < group2.Table.Rows.Count; i++)
                        {
                            if (i + 1 != group2.Table.Rows.Count)
                            {
                                ls_spinner += group2.Table.Rows[i]["a"].ToString().Trim() + "|";
                            }
                            else
                            {
                                ls_spinner += group2.Table.Rows[i]["a"].ToString().Trim();
                            }
                        }

                        ls_spinner = "{" + ls_spinner + "}";

                        string ls_exe = "update a_gp4 set spinner='" + ls_spinner + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "' ";
                        DB.ExecuteSQL(ls_exe);
                    }

                    /*
                    group2 = null;
                    string ls_temp = "select * from a_gp4 where idx>1 and  a='" + group.Table.Rows[x]["a"].ToString().Trim() + "'";
                    group2 = DB.GetDataGroup(ls_temp);
                    if (group2.Table.Rows.Count > 1)
                    {
                        string exesql = "update a_gp4 set flag=0 where id='" + group.Table.Rows[x]["ID"].ToString().Trim() + "'";
                        DB.ExecuteSQL(exesql);
                    }
                    */

                    /*
                    string do_sql = "";

                    if (group.Table.Rows[x]["w1"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w1"].ToString().Trim().Replace("'", "") + "',1,1,1);";
                    }
                    if (group.Table.Rows[x]["w2"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w2"].ToString().Trim().Replace("'", "") + "',2,1,1);";
                    }
                    if (group.Table.Rows[x]["w3"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w3"].ToString().Trim().Replace("'", "") + "',3,1,1);";
                    }
                    if (group.Table.Rows[x]["w4"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w4"].ToString().Trim().Replace("'", "") + "',4,1,1);";
                    }
                    if (group.Table.Rows[x]["w5"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w5"].ToString().Trim().Replace("'", "") + "',5,1,1);";
                    }
                    if (group.Table.Rows[x]["w6"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w6"].ToString().Trim().Replace("'", "") + "',6,1,1);";
                    }
                    if (group.Table.Rows[x]["w7"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w7"].ToString().Trim().Replace("'", "") + "',7,1,1);";
                    }
                    if (group.Table.Rows[x]["w8"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w8"].ToString().Trim().Replace("'", "") + "',8,1,1);";
                    }
                    if (group.Table.Rows[x]["w9"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w9"].ToString().Trim().Replace("'", "") + "',9,1,1);";
                    }
                    if (group.Table.Rows[x]["w10"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w10"].ToString().Trim().Replace("'", "") + "',10,1,1);";
                    }
                    if (group.Table.Rows[x]["w11"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w10"].ToString().Trim().Replace("'", "") + "',11,1,1);";
                    }
                    if (group.Table.Rows[x]["w12"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w12"].ToString().Trim().Replace("'", "") + "',12,1,1);";
                    }
                    if (group.Table.Rows[x]["w13"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w13"].ToString().Trim().Replace("'", "") + "',13,1,1);";
                    }
                    if (group.Table.Rows[x]["w14"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w14"].ToString().Trim().Replace("'", "") + "',14,1,1);";
                    }
                    if (group.Table.Rows[x]["w15"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w15"].ToString().Trim().Replace("'", "") + "',15,1,1);";
                    }
                    if (group.Table.Rows[x]["w16"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w16"].ToString().Trim().Replace("'", "") + "',16,1,1);";
                    }
                    if (group.Table.Rows[x]["w17"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w17"].ToString().Trim().Replace("'", "") + "',17,1,1);";
                    }
                    if (group.Table.Rows[x]["w18"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w18"].ToString().Trim().Replace("'", "") + "',18,1,1);";
                    }
                    if (group.Table.Rows[x]["w19"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w19"].ToString().Trim().Replace("'", "") + "',19,1,1);";
                    }
                    if (group.Table.Rows[x]["w20"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w20"].ToString().Trim().Replace("'", "") + "',20,1,1);";
                    }
                    if (group.Table.Rows[x]["w21"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w21"].ToString().Trim().Replace("'", "") + "',21,1,1);";
                    }
                    if (group.Table.Rows[x]["w22"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w22"].ToString().Trim().Replace("'", "") + "',22,1,1);";
                    }
                    if (group.Table.Rows[x]["w23"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w23"].ToString().Trim().Replace("'", "") + "',23,1,1);";
                    }
                    if (group.Table.Rows[x]["w24"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w24"].ToString().Trim().Replace("'", "") + "',24,1,1);";
                    }
                    if (group.Table.Rows[x]["w25"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w25"].ToString().Trim().Replace("'", "") + "',25,1,1);";
                    }
                    if (group.Table.Rows[x]["w26"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w26"].ToString().Trim().Replace("'", "") + "',26,1,1);";
                    }
                    if (group.Table.Rows[x]["w27"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w27"].ToString().Trim().Replace("'", "") + "',27,1,1);";
                    }
                    if (group.Table.Rows[x]["w28"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w28"].ToString().Trim().Replace("'", "") + "',28,1,1);";
                    }
                    if (group.Table.Rows[x]["w29"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w29"].ToString().Trim().Replace("'", "") + "',29,1,1);";
                    }
                    if (group.Table.Rows[x]["w30"].ToString().Trim().Replace("'", "").Length > 0)
                    {
                        do_sql += "insert into a_gp4 (gpid,a,idx,flag,status) values ('" + group.Table.Rows[x]["gid"].ToString().Trim().Replace("'", "") + "','" + group.Table.Rows[x]["w30"].ToString().Trim().Replace("'", "") + "',30,1,1);";
                    }

                    DB.ExecuteSQL(do_sql);

                    */

                    /*页码
                    int ls_max = int.Parse(group.Table.Rows[x]["max"].ToString().Trim());
                    for (int m = 1; m <= ls_max; m++)
                    {

                        string ls_up = "insert into UKGOLF_B2 (pid,main_class,sub_class,url,status) values ('" 
                            + group.Table.Rows[x]["id"].ToString().Trim() + "','"
                            + group.Table.Rows[x]["main_class"].ToString().Trim() + "','"
                            + group.Table.Rows[x]["sub_class"].ToString().Trim().Replace("'","") + "','"
                            //+ group.Table.Rows[x]["url"].ToString().Trim() + "?page="+m+"&sort=2d',1)";
                            + group.Table.Rows[x]["url"].ToString().Trim() + "" + m + "',1)";
                        DB.ExecuteSQL(ls_up);

                    }
                    */


                    //string dog = NoHTML(group.Table.Rows[x]["remark2"].ToString().Trim());

                    //string dog = group.Table.Rows[x]["remark"].ToString().Trim();

                    //string product_name = group.Table.Rows[x]["remark_del"].ToString().Trim();

                    //dog = dog.Replace(product_name, "").Trim();
                    //dog = dog.Replace("</strong>", "").Trim();

                    //dog = dog + ". ";

                    //string dog = dog = "|real-nike-shoes-" + group.Table.Rows[x]["id"].ToString().Trim() + ".jpg";

                    //if (x > 0)
                    //{
                    //    dog = "|real-nike-shoes-" + group.Table.Rows[x]["pid"].ToString().Trim() + ".jpg";
                    //}

                    //string ls_up = "UPDATE usa_nike_b3 SET pic_group2=pic_group2+'" + dog + "' WHERE id='" + group.Table.Rows[x]["pid"].ToString() + "' ";
                    //DB.ExecuteSQL(ls_up);

                    //string dog = group.Table.Rows[x]["pic_group2"].ToString().Trim();

                    //dog = dog.Substring(1, dog.Length - 1).Trim();

                    //string ls_up = "UPDATE usa_nike_b3 SET bigimghtml='" + x + "' WHERE id='" + group.Table.Rows[x]["id"].ToString() + "' ";
                    //DB.ExecuteSQL(ls_up);



                    /*

                        if (dog.Length > 5)
                        {
                            if (dog.Substring(0, 1) == ",")
                            {
                                dog = dog.Substring(1, dog.Length - 1).Trim();

                            }
                        }
                        */

                    /*

                    string dog = group.Table.Rows[x]["product_name"].ToString().Trim();
                    string m = group.Table.Rows[x]["main_class"].ToString().Trim();
                    string s = group.Table.Rows[x]["sub_class"].ToString().Trim();

                    if (m.Length > 5)
                    {
                        dog = dog.Replace(m, "").Trim();
                    }
                    if (s.Length > 5)
                    {
                        dog = dog.Replace(s, "").Trim();
                    }

                    dog = dog.Replace("  ", " ").Trim();

                    string fuck = " MD";
                    if (dog.LastIndexOf(fuck) > -1)
                    {
                        int intLength = dog.Length;

                        int startIndex = dog.LastIndexOf(fuck);

                        dog = dog.Substring(0, intLength - startIndex+1);
                    }
                    */


                    /*拼 
                    string hhh="<a href=\"http://www.cheap-nike-shoes.org/\">Cheap Nike Shoes</a>";
                    string bbb = "<br><a href=\"http://www.cheap-nike-shoes.org/\">Air Max 90</a>";
                    string ccc = " <a href=\"http://www.cheap-nike-shoes.org/\">Air Max 95</a>";


                    string ls_up = "UPDATE JewelOra_b3 SET desc2='" + hhh + "<strong>" + group.Table.Rows[x]["brand"].ToString() + "</strong><br>" + TrueLan_BR(group.Table.Rows[x]["remark2"].ToString()) + bbb + ccc + "' WHERE id='" + group.Table.Rows[x]["id"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up);
                     */


                    /*
                    string dog = "<img src=\"http://images.discount-nfl-jerseys.org/Discount-NFL-JERSEYS-" + group.Table.Rows[x]["id"].ToString().Trim() + ".jpg\" alt=\"Discount NFL JERSEYS -" + group.Table.Rows[x]["picname"].ToString().Trim() + "\" title=\"Discount NFL JERSEYS -" + group.Table.Rows[x]["picname"].ToString().Trim() + "\" /><br/>";

                    string ls_up = "UPDATE NFL_B3 SET bigimghtml='" + dog + "' WHERE id='" + group.Table.Rows[x]["id"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up);
                    */
                }

            }
            MessageBox.Show("OK!!");



        }

        private void button72_Click(object sender, EventArgs e)
        {

            DataGroup group = new DataGroup();
            DataGroup group_if = new DataGroup();

            string ls_group = "select distinct size as attrib from plan425_b2 where len(size) >0 ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {

                    string attrib = group.Table.Rows[x]["attrib"].ToString().Trim();

                    if (attrib.IndexOf("|") > -1)
                    {

                        string[] dog_small = attrib.Split('|');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = bb.Trim();

                            group_if = null;
                            string ls_double = "select id from plan425_ATTRIB where name='" + OK_bb + "' and status=1   ";
                            group_if = DB.GetDataGroup(ls_double);
                            if (group_if.Table.Rows.Count == 0 && OK_bb.Length >0 )
                            {
                                string ls_up = "insert into plan425_ATTRIB (name,status) values ('" + OK_bb + "',1)";
                                DB.ExecuteSQL(ls_up);
                            }
                        }
                    }
                    else
                    {
                        group_if = null;
                        string ls_double = "select id from plan425_ATTRIB where name='" + attrib + "' and status=1   ";
                        group_if = DB.GetDataGroup(ls_double);
                        if (group_if.Table.Rows.Count == 0  && attrib.Length > 0)
                        {
                            string ls_up = "insert into plan425_ATTRIB (name,status) values ('" + attrib + "',1)";
                            DB.ExecuteSQL(ls_up);
                        }
                    }
                }

            }
            MessageBox.Show("OK!!");
        }

        private void button73_Click(object sender, EventArgs e)
        {
            /*
select 'insert into  `jv1_products_options_values` (products_options_values_id,language_id,products_options_values_name,products_options_values_sort_order) values ('+cast(id as nvarchar(10))+',1,"'+name+'",0);'
from plan425_ATTRIB
             */

            /*
             select
'insert into  `jv1_products_attributes` (products_attributes_id ,	products_id ,	options_id ,	options_values_id,price_prefix ,product_attribute_is_free,products_attributes_weight_prefix,attributes_image ,attributes_qty_prices,attributes_qty_prices_onetime) 
values ('+cast(id as nvarchar(10))+','+cast(proid as nvarchar(10))+',1,'+cast(attribid as nvarchar(10))+''
+');'
from plan425_ATTRIB_PRO
             
             */

            /*  );   替换    ,'+',1,'+','','','');          */

            /*update `jv1_products_description`
set products_description=replace(products_description,'Replica ','')*/

            DataGroup group = new DataGroup();
            DataGroup group_if = new DataGroup();

            string ls_group = "select id,SIZE as attrib,xID AS nb from plan425_b2 where len(size) >0 ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {

                    string attrib = group.Table.Rows[x]["attrib"].ToString().Trim();

                    if (attrib.IndexOf("|") > -1)
                    {

                        string[] dog_small = attrib.Split('|');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = bb.Trim();

                            group_if = null;
                            string ls_double = "select id from plan425_ATTRIB where name='" + OK_bb + "' and status=1   ";
                            group_if = DB.GetDataGroup(ls_double);
                            if (group_if.Table.Rows.Count == 1)
                            {
                                string ls_up = "insert into plan425_ATTRIB_PRO (PROID,ATTRIBID,status) values ('" + group.Table.Rows[x]["nb"].ToString().Trim() + "','" + group_if.Table.Rows[0]["id"].ToString().Trim() + "',1)";
                                DB.ExecuteSQL(ls_up);
                            }
                            group_if = null;
                        }
                    }
                    else
                    {
                        group_if = null;
                        string ls_double = "select id from plan425_ATTRIB where name='" + attrib + "' and status=1   ";
                        group_if = DB.GetDataGroup(ls_double);
                        if (group_if.Table.Rows.Count == 1)
                        {
                            string ls_up = "insert into plan425_ATTRIB_PRO (PROID,ATTRIBID,status) values ('" + group.Table.Rows[x]["nb"].ToString().Trim() + "','" + group_if.Table.Rows[0]["id"].ToString().Trim() + "',1)";
                            DB.ExecuteSQL(ls_up);

                        }
                        group_if = null;
                    }
                }

            }
            MessageBox.Show("OK!!");


        }

        public static string TrueLan(string TextString)
        {

            //第一步格式化（纯格式）
            string ls_text = TextString;

            ls_text = ls_text.Replace("\r\n", "");

            ls_text = ls_text.Replace(",,", ",");
            ls_text = ls_text.Replace("..", ".");
            ls_text = ls_text.Replace("!!", "!");
            ls_text = ls_text.Replace("??", "?");
            ls_text = ls_text.Replace(";;", ";");

            ls_text = ls_text.Replace(",", " , ");
            ls_text = ls_text.Replace(".", " . ");
            ls_text = ls_text.Replace("!", " ! ");
            ls_text = ls_text.Replace("?", " ? ");
            ls_text = ls_text.Replace(";", " ; ");

            ls_text = ls_text.Replace("  ", " ");
            ls_text = ls_text.Trim();

            //第二步格式化（每3插入<br>，大于180插入<br>，换行用<p></p>包裹）
            //对象:  『.』『!』『?』『;』
            //㊣

            int ls_len = ls_text.Length;

            //得到数量
            string ls_temp = ls_text.Replace(".", "㊣").Replace("!", "㊣").Replace("?", "㊣").Replace(";", "㊣");
            string ls_spl = ls_temp.Replace("㊣", "");
            int int_spl = ls_spl.Length;
            int CCC = ls_len - int_spl;

            //如果是短文本
            if (CCC == 1)
            {
                return TextString;
            }

            //将ls_temp通过㊣分割
            string[] dog_small = ls_temp.Split('㊣');


            //定义NewString
            string NewString = "";
            //定义字符位
            int HereText = 0;
            //定义小组容器标志
            int Flag = 0;

            int num = 0;

            foreach (string bb in dog_small)
            {

                //TextInfo tInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

                //tInfo.ToTitleCase(bb)


                string str1 = "";
                string str2 = "";
                string bbb = "";


                if (bb.Trim().Length > 1)
                {
                    str1 = bb.Trim().Substring(0, 1);
                    str2 = bb.Trim().Substring(1, bb.Trim().Length - 1);
                    bbb = str1.ToUpper() + str2;
                }
                else
                {
                    bbb = bb.Trim();
                }





                num += 1;

                if (num < CCC)
                {



                    HereText = HereText + bb.Length + 1;

                    if (bb.Length >= 200)//判断本文长度
                    {
                        if (Flag == 0)//判断前文是否Over
                        {
                            //ToTitleCase

                            NewString += "<p>  " + bbb + ls_text.Substring(HereText - 1, 1) + "</p>";
                            Flag = 0;
                        }
                        else
                        {
                            NewString += "</p><p>  " + bbb + ls_text.Substring(HereText - 1, 1) + "</p>";
                            Flag = 0;
                        }

                    }
                    else//长度少于180,以及累计少于180
                    {
                        if (Flag == 0)//判断前文是否Over
                        {
                            NewString += "<p>  " + bbb + ls_text.Substring(HereText - 1, 1) + "";
                            Flag += 1;
                        }
                        else
                        {

                            if (Flag < 2)
                            {
                                NewString += "" + bbb + ls_text.Substring(HereText - 1, 1) + "";
                                Flag += 1;
                            }
                            else
                            {

                                NewString += "" + bbb + ls_text.Substring(HereText - 1, 1) + "</p>";
                                Flag = 0;

                            }

                        }


                    }
                }

                if (num == CCC)
                {


                    if (bb.Length >= 200)//判断本文长度
                    {
                        if (Flag == 0)//判断前文是否Over
                        {
                            NewString += "<p>  " + bbb + " ." + "</p>";
                            Flag = 0;
                        }
                        else
                        {
                            NewString += "</p><p>  " + bbb + " ." + "</p>";
                            Flag = 0;
                        }

                    }
                    else
                    {

                        NewString += "" + bbb + " ." + "</p>";

                    }
                }

            }




            return NewString;

        }

        public static string TrueLan_BR(string TextString)
        {

            //第一步格式化（纯格式）
            string ls_text = TextString;

            ls_text = ls_text.Replace("\r\n", "");

            ls_text = ls_text.Replace(",,", ",");
            ls_text = ls_text.Replace("..", ".");
            ls_text = ls_text.Replace("!!", "!");
            ls_text = ls_text.Replace("??", "?");
            ls_text = ls_text.Replace(";;", ";");

            ls_text = ls_text.Replace(",", " , ");
            ls_text = ls_text.Replace(".", " . ");
            ls_text = ls_text.Replace("!", " ! ");
            ls_text = ls_text.Replace("?", " ? ");
            ls_text = ls_text.Replace(";", " ; ");

            ls_text = ls_text.Replace("  ", " ");
            ls_text = ls_text.Trim();

            //第二步格式化（每3插入<br>，大于180插入<br>，换行用<p></p>包裹）
            //对象:  『.』『!』『?』『;』
            //㊣

            int ls_len = ls_text.Length;

            //得到数量
            string ls_temp = ls_text.Replace(".", "㊣").Replace("!", "㊣").Replace("?", "㊣").Replace(";", "㊣");
            string ls_spl = ls_temp.Replace("㊣", "");
            int int_spl = ls_spl.Length;
            int CCC = ls_len - int_spl;

            if (CCC == 1)
            {
                return TextString;
            }

            //将ls_temp通过㊣分割
            string[] dog_small = ls_temp.Split('㊣');


            //定义NewString
            string NewString = "";
            //定义字符位
            int HereText = 0;
            //定义小组容器标志
            int Flag = 0;

            int num = 0;

            foreach (string bb in dog_small)
            {
                num += 1;

                if (num < CCC)
                {

                    HereText = HereText + bb.Length + 1;

                    if (bb.Length >= 200)//判断本文长度
                    {
                        if (Flag == 0)//判断前文是否Over
                        {
                            NewString += "" + bb + ls_text.Substring(HereText - 1, 1) + "<br>";
                            Flag = 0;
                        }
                        else
                        {
                            NewString += "<br>" + bb + ls_text.Substring(HereText - 1, 1) + "<br>";
                            Flag = 0;
                        }

                    }
                    else//长度少于180,以及累计少于180
                    {
                        if (Flag == 0)//判断前文是否Over
                        {
                            NewString += "" + bb + ls_text.Substring(HereText - 1, 1) + "";
                            Flag += 1;
                        }
                        else
                        {

                            if (Flag < 2)
                            {
                                NewString += "" + bb + ls_text.Substring(HereText - 1, 1) + "";
                                Flag += 1;
                            }
                            else
                            {

                                NewString += "" + bb + ls_text.Substring(HereText - 1, 1) + "<br>";
                                Flag = 0;

                            }

                        }


                    }
                }

                if (num == CCC)
                {

                    if (bb.Length >= 200)//判断本文长度
                    {
                        if (Flag == 0)//判断前文是否Over
                        {
                            NewString += "" + bb + " ." + "";
                            Flag = 0;
                        }
                        else
                        {
                            NewString += "<br>" + bb + " ." + "";
                            Flag = 0;
                        }

                    }
                    else
                    {

                        NewString += "" + bb + " ." + "";

                    }
                }

            }




            return NewString;

        }

        private void button74_Click_1(object sender, EventArgs e)
        {
            try
            {
                textBox22.Text = "";
                button75.Enabled = false;

                string ls_kws = "http://ezinearticles.com/search/?q=" + t_kws.Text.Trim().Replace(" ", "+");
                string pig = getUrlSource(ls_kws, "utf-8").Replace("'", "\"");//utf-8  gb2312
                //of <span class="number">486</span>

                string ls_shtml = pig.Replace("\r\n", "").ToString();//替换换行符
                ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”
                string ls_Value = OperateStr_Adv(ls_shtml, "of <span class=number>", "</span>", "", 1, 0, 0, "", "", "", "");
                ls_Value = NoHTML(ls_Value).Trim();

                textBox22.Text = ls_Value.Trim();

                if (int.Parse(textBox22.Text.Replace(",", "")) > 0)
                {
                    button75.Enabled = true;

                    MessageBox.Show(ls_Value);
                }
                else
                {
                    MessageBox.Show("Null");
                }
            }
            catch
            {
                MessageBox.Show("Error");
            }


        }

        private void button75_Click(object sender, EventArgs e)
        {
            int xpagecount = int.Parse(textBox22.Text.Replace(",", "")) / 25 + 1;
            for (int m = 0; m < xpagecount; m++)
            {
                //http://ezinearticles.com/search/?q=jade+jewelry&index=475

                if (m == 0)
                {
                    string ls_kws = "http://ezinearticles.com/search/?q=" + t_kws.Text.Trim().Replace(" ", "+");
                    string pig = getUrlSource_proxy(ls_kws, "utf-8").Replace("'", "\"");//utf-8  gb2312

                    string ls_up = "insert into BLOG_B1 (KW,PAGE_URL,HTML,STATUS,FLAG) values ('"
                        + t_kws.Text.Trim() + "','"
                        + ls_kws + "','"
                        + pig + "',0,0)";
                    DB.ExecuteSQL(ls_up);
                }
                else
                {
                    string ls_kws = "http://ezinearticles.com/search/?q=" + t_kws.Text.Trim().Replace(" ", "+") + "&index=" + (m * 25).ToString();
                    string pig = getUrlSource(ls_kws, "utf-8").Replace("'", "\"");//utf-8  gb2312

                    string ls_up = "insert into BLOG_B1 (KW,PAGE_URL,HTML,STATUS,FLAG) values ('"
                        + t_kws.Text.Trim() + "','"
                        + ls_kws + "','"
                        + pig + "',0,0)";
                    DB.ExecuteSQL(ls_up);
                }

            }

            MessageBox.Show("OK.");

        }

        private void button77_Click(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;

            string ls_sql = "select id,page_url as url from BLOG_B1 where status=0 and kw='" + t_kws.Text.Trim() + "' and id not in (select id from BLOG_B1 where status=0 and kw='" + t_kws.Text.Trim() + "' and html like '%of <span class=\"number\">" + textBox22.Text.Trim() + "</span>%')";
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                //L_M2.Text = group.Table.Rows.Count.ToString();


                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = "";

                    pig = getUrlSource(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312


                    if (pig != "")
                    {
                        string ls_up = "UPDATE BLOG_B1 SET HTML='" + pig + "' ,STATUS=0 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);

                        pig = "";
                    }

                }


                MessageBox.Show("do ok!");
            }
            else
            {

                MessageBox.Show("ok!");
            }
        }

        private void button76_Click(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();
            DataGroup group_html = new DataGroup();
            group_html = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group_html = null;

            string ls_sql = "SELECT ID,HTML FROM BLOG_B1 WHERE STATUS =0 order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["HTML"].ToString().Replace("\r\n", "").ToString();//替换换行符
                    ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("<div class=result-title>", "|");//将分隔码 替换成“|”

                    //去掉后面的代码  
                    //int temp_right = ls_shtml.IndexOf("<!-- bof: whats_new -->");
                    //ls_shtml = ls_shtml.Substring(0, temp_right);


                    string[] dog_small = ls_shtml.Split('|');//子串

                    int pd = 0;

                    foreach (string bb in dog_small)
                    {
                        if (pd > 0)
                        {
                            string ls_ttt = "";
                            string ls_FILED = "";
                            string ls_Value = "";

                            //-----------------------------------------------------------------------------------------------------
                            group = null;
                            string ls_rule = "SELECT RID,FILED,OID,CYCLE,STRING_S,STRING_E,STRING_NEW,INCLUDE_S,INCLUDE_E FROM URL_RULE WHERE siteid=200 and STATUS=1";
                            group = DB.GetDataGroup(ls_rule);
                            if (group.Table.Rows.Count > 0)
                            {
                                for (int j = 0; j < group.Table.Rows.Count; j++)
                                {
                                    try
                                    {
                                        ls_ttt = OperateStr(bb, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, "0");
                                        ls_Value += ",'" + ls_ttt + "'";
                                    }
                                    catch
                                    {
                                        ls_ttt = "";
                                        ls_Value = "," + " ";
                                    }

                                    ls_FILED += "," + group.Table.Rows[j]["FILED"].ToString();


                                }


                                DataGroup temp = new DataGroup();
                                temp = null;
                                string ls_temp = "select id from BLOG_B2 where ARTICLES_URL='" + ls_ttt + "'";
                                temp = DB.GetDataGroup(ls_temp);
                                if (temp.RecCount == 0)
                                {

                                    string ls_up = "INSERT INTO BLOG_B2 (PID " + ls_FILED + ",STATUS) values ('" + group_html.Table.Rows[i]["ID"].ToString() + "' " + ls_Value + ",2)";
                                    DB.ExecuteSQL(ls_up);

                                }
                                else
                                {
                                    string ls_up = "INSERT INTO BLOG_B2 (PID " + ls_FILED + ",STATUS) values ('" + group_html.Table.Rows[i]["ID"].ToString() + "' " + ls_Value + ",0)";
                                    DB.ExecuteSQL(ls_up);
                                }


                                ls_Value = "";
                                ls_FILED = "";
                                ls_ttt = "";


                            }
                            group = null;
                            //-----------------------------------------------------------------------------------------------------
                        }
                        pd += 1;
                    }


                }


            }

            MessageBox.Show("OK!!");
        }

        private void button78_Click(object sender, EventArgs e)
        {
            string xxxx = textBox23.Text.Trim();

            DataGroup group = new DataGroup();
            group = null;
            string ls_sql = "SELECT ID,ARTICLES_URL AS URL FROM BLOG_B2 WHERE STATUS='2' ";//and substring(cast( id as nvarchar(10)),len(id),1)='" + xxxx + "'
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = "";
                    pig = getUrlSource_proxy("http://ezinearticles.com/" + group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312

                    if (pig != "")
                    {
                        string ls_up = "UPDATE BLOG_B2 SET HTML='" + pig + "' ,STATUS='1' WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);

                        pig = "";
                    }
                }
            }
            group = null;
            MessageBox.Show("ok!");
        }

        private void button79_Click(object sender, EventArgs e)
        {
            try
            {
                DataGroup group = new DataGroup();

                DataGroup group_html = new DataGroup();
                group_html = null;
                DataEntity de = new DataEntity();
                de.RemoveAll();
                group_html = null;

                string ls_sql = "SELECT  ID,HTML FROM BLOG_B2 WHERE STATUS ='1'  order by ID";
                group_html = DB.GetDataGroup(ls_sql);
                if (group_html.Table.Rows.Count > 0)
                {

                    for (int i = 0; i < group_html.Table.Rows.Count; i++)
                    {
                        string ls_shtml = group_html.Table.Rows[i]["HTML"].ToString().Replace("\r\n", "").ToString();//替换换行符
                        ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                        ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”


                        //string ls_FILED = "";
                        string ls_Value = "";
                        //-----------------------------------------------------------------------------------------------------
                        group = null;
                        string ls_rule = "SELECT RID,FILED,OID,CYCLE,STRING_S,STRING_E,STRING_NEW,INCLUDE_S,INCLUDE_E,STR_AREA,STR_AFT,STR_BEF FROM URL_RULE WHERE siteid=201 and STATUS=1";
                        group = DB.GetDataGroup(ls_rule);
                        if (group.Table.Rows.Count > 0)
                        {
                            for (int j = 0; j < group.Table.Rows.Count; j++)
                            {
                                try
                                {
                                    ls_Value = OperateStr_Adv(ls_shtml, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, group.Table.Rows[j]["CYCLE"].ToString(), group.Table.Rows[j]["STR_AREA"].ToString(), group.Table.Rows[j]["STR_AFT"].ToString(), group.Table.Rows[j]["STR_BEF"].ToString());
                                    ls_Value = NoHTML(ls_Value).Trim();
                                }
                                catch
                                {
                                    ls_Value = "";
                                }

                                string ls_up = "UPDATE BLOG_B2 SET " + group.Table.Rows[j]["FILED"].ToString() + "='" + ls_Value + "' ,STATUS=8 WHERE ID='" + group_html.Table.Rows[i]["ID"].ToString() + "'";

                                DB.ExecuteSQL(ls_up);
                            }


                        }
                        ls_shtml = null;
                        group = null;
                        //-----------------------------------------------------------------------------------------------------


                    }

                }

            }
            catch
            { }

            MessageBox.Show("OK");
        }

        public string TrueLan_Format(string TextString)
        {
            DataGroup group = new DataGroup();
            group = null;


            //第一步格式化（纯格式）
            string ls_text = TextString;

            ls_text = ls_text.Replace("\r\n", "");

            ls_text = ls_text.Replace(",,", ",");
            ls_text = ls_text.Replace("..", ".");
            ls_text = ls_text.Replace("!!", "!");
            ls_text = ls_text.Replace("??", "?");
            ls_text = ls_text.Replace(";;", ";");
            ls_text = ls_text.Replace("  ", " ");
            ls_text = ls_text.Trim();

            /*
            ls_text = ls_text.Replace(",", " , ");
            ls_text = ls_text.Replace(".", " . ");
            ls_text = ls_text.Replace("!", " ! ");
            ls_text = ls_text.Replace("?", " ? ");
            ls_text = ls_text.Replace(";", " ; ");
            ls_text = ls_text.Replace("  ", " ");
            */



            int ls_len = ls_text.Length;

            string ls_temp = ls_text.Replace(".", "㊣").Replace("!", "㊣").Replace("?", "㊣").Replace(";", "㊣").Replace(",", "㊣").Replace(" ", "㊣").Replace(" ", "㊣");
            //ls_temp = ls_temp.Replace("㊣㊣", "㊣");

            //将ls_temp通过㊣分割
            string[] dog_small = ls_temp.Split('㊣');

            //定义NewString
            string NewString = "";
            //定义字符位
            int HereText = 0;

            int num = 0;

            foreach (string bb in dog_small)
            {
                num += 1;

                HereText = HereText + bb.Length + 1;

                string ls_sql = "select b from a_twins where a='" + bb + "'";

                group = DB.GetDataGroup(ls_sql);
                if (group.Table.Rows.Count > 0)
                {
                    NewString += "{" + bb + "|" + group.Table.Rows[0]["b"].ToString() + "}" + ls_text.Substring(HereText - 1, 1);
                }
                else
                {
                    if (num == dog_small.Length)
                    {
                        NewString += bb;
                    }
                    else
                    {
                        NewString += bb + ls_text.Substring(HereText - 1, 1);
                    }

                }

            }

            ls_text = NewString.Replace(",", ", ");
            ls_text = ls_text.Replace(".", ". ");
            ls_text = ls_text.Replace("!", "! ");
            ls_text = ls_text.Replace("?", "? ");
            ls_text = ls_text.Replace(";", "; ");
            ls_text = ls_text.Replace("  ", " ");
            NewString = ls_text.Trim();

            return NewString;

        }

        public string TrueLan_Format_Adv(string TextString)
        {

            DataGroup group = new DataGroup();
            group = null;


            //第一步格式化（纯格式）
            string ls_text = TextString;

            ls_text = ls_text.Replace("\r\n", "");

            ls_text = ls_text.Replace(",,", ",");
            ls_text = ls_text.Replace("..", ".");
            ls_text = ls_text.Replace("!!", "!");
            ls_text = ls_text.Replace("??", "?");
            ls_text = ls_text.Replace(";;", ";");
            ls_text = ls_text.Replace("  ", " ");
            ls_text = ls_text.Trim();

            /*
            ls_text = ls_text.Replace(",", " , ");
            ls_text = ls_text.Replace(".", " . ");
            ls_text = ls_text.Replace("!", " ! ");
            ls_text = ls_text.Replace("?", " ? ");
            ls_text = ls_text.Replace(";", " ; ");
            ls_text = ls_text.Replace("  ", " ");
            */



            int ls_len = ls_text.Length;

            string ls_temp = ls_text.Replace(".", "㊣").Replace("!", "㊣").Replace("?", "㊣").Replace(";", "㊣").Replace(",", "㊣").Replace(" ", "㊣").Replace(" ", "㊣");
            //ls_temp = ls_temp.Replace("㊣㊣", "㊣");

            //将ls_temp通过㊣分割
            string[] dog_small = ls_temp.Split('㊣');

            //定义NewString
            string NewString = "";
            //定义字符位
            int HereText = 0;

            int num = 0;

            foreach (string bb in dog_small)
            {
                num += 1;

                HereText = HereText + bb.Length + 1;

                string ls_sql = "select b from a_twins where a='" + bb + "'";

                group = DB.GetDataGroup(ls_sql);
                if (group.Table.Rows.Count > 0)
                {
                    NewString += "{" + bb + "|" + group.Table.Rows[0]["b"].ToString() + "}" + ls_text.Substring(HereText - 1, 1);
                }
                else
                {
                    if (num == dog_small.Length)
                    {
                        NewString += bb;
                    }
                    else
                    {
                        NewString += bb + ls_text.Substring(HereText - 1, 1);
                    }

                }

            }

            ls_text = NewString.Replace(",", ", ");
            ls_text = ls_text.Replace(".", ". ");
            ls_text = ls_text.Replace("!", "! ");
            ls_text = ls_text.Replace("?", "? ");
            ls_text = ls_text.Replace(";", "; ");
            ls_text = ls_text.Replace("  ", " ");
            NewString = ls_text.Trim();

            return NewString;

        }

        private void button81_Click(object sender, EventArgs e)
        {
            /*
              
             select 'a = reg'+tag+'.Replace(a,"【'+tag+'】");' as Da1,
'static Regex reg'+tag+' = new Regex(" '+a+' ", RegexOptions.Compiled | RegexOptions.IgnoreCase);' as Da2,
'a = reg'+tag+'.Replace(a,"'+spinner+'");' as Db1,
'static Regex reg'+tag+' = new Regex("【'+tag+'】", RegexOptions.Compiled | RegexOptions.IgnoreCase);' as Db2
from a_gp4 where flag=1 and status=1 

             */

            //TrueLan_Format("Three Ways to Step Out of Your Fashion Comfort Zone - Go Crazy With Bone Carved Jewelry , how are you? did you know? Forget the Diamond - How About Bone Carved Jewelry? ");
            //Three {Ways|means} to {Step|footfall} Out of Your Fashion {Comfort|abundance} {Zone|area} - Go Crazy With {Bone|cartilage} Carved Jewelry , howwareeyouudidiyouo{know|apperceive}o{Forget|overlook}gthetDiamondo-dHowHAbouto{Bone|cartilage}oCarvedvJewelrylr

            DataGroup group = new DataGroup();
            DataGroup group2 = new DataGroup();

            string ls_group = "select ID,TITLE1,TITLE2,TEXT1,TEXT2,TEXT3,TEXT4 from BLOG_B2 WHERE STATUS=8 ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    string ls_text = group.Table.Rows[x]["TEXT1"].ToString().Trim();

                    ls_text = ls_text.Replace(",,", ",");
                    ls_text = ls_text.Replace("..", ".");
                    ls_text = ls_text.Replace("!!", "!");
                    ls_text = ls_text.Replace("??", "?");
                    ls_text = ls_text.Replace(";;", ";");

                    ls_text = ls_text.Replace("{", "(");
                    ls_text = ls_text.Replace("}", ")");

                    ls_text = ls_text.Replace(",", " , ");
                    ls_text = ls_text.Replace(".", " . ");
                    ls_text = ls_text.Replace("!", " ! ");
                    ls_text = ls_text.Replace("?", " ? ");
                    ls_text = ls_text.Replace(";", " ; ");
                    ls_text = ls_text.Replace("  ", " ");

                    ls_text = " " + ls_text.Trim().Replace("'", "㊣") + " ";

                    ls_text = format_clear_flag(ls_text);

                    //spinner_txt2tag_v1 xxx = new spinner_txt2tag_v1();
                    //ls_text = xxx.spinner_txt2tag(ls_text);

                    //spinner_tag2spin_v1 yyy = new spinner_tag2spin_v1();
                    //string ls_text4 = yyy.spinner_tag2spin(ls_text);

                    //string  ls_text5 = spinner2fulltext(' ' + ls_text4);


                    DB.ExecuteSQL("Update BLOG_B2 set TEXT2='" + ls_text.Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'");

                    //DB.ExecuteSQL("Update BLOG_B2 set TEXT5='" + ls_text5.Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'");



                }
            }

            MessageBox.Show("OK");

        }






        private string format_clear_flag(string key)
        {
            string x = key;

            //published by***     ok
            if (x.ToLower().IndexOf("published by") > -1)
            {
                int ix = x.ToLower().IndexOf("published by");
                x = x.Substring(0, ix);
            }

            //Source:***          ok
            if (x.ToLower().IndexOf("source:") > -1)
            {
                int ix = x.ToLower().IndexOf("source:");
                x = x.Substring(0, ix);
            }

            //Author:***          ok
            if (x.ToLower().IndexOf("author:") > -1)
            {
                int ix = x.ToLower().IndexOf("author:");
                x = x.Substring(0, ix);
            }

            //Copyright***        ok
            if (x.ToLower().IndexOf("copyright") > -1)
            {
                int ix = x.ToLower().IndexOf("copyright");
                x = x.Substring(0, ix);
            }

            //My website is***    
            if (x.ToLower().IndexOf("my website is") > -1)
            {
                int ix = x.ToLower().IndexOf("my website is");
                x = x.Substring(0, ix);
            }

            //on our web site***
            if (x.ToLower().IndexOf("on our web site") > -1)
            {
                int ix = x.ToLower().IndexOf("on our web site");
                x = x.Substring(0, ix);
            }

            //on our web site***
            if (x.ToLower().IndexOf("on our web site") > -1)
            {
                int ix = x.ToLower().IndexOf("on our web site");
                x = x.Substring(0, ix);
            }

            //our web site***
            if (x.ToLower().IndexOf("our web site") > -1)
            {
                int ix = x.ToLower().IndexOf("our web site");
                x = x.Substring(0, ix);
            }

            //by e-mailing us***
            if (x.ToLower().IndexOf("by e-mailing us") > -1)
            {
                int ix = x.ToLower().IndexOf("by e-mailing us");
                x = x.Substring(0, ix);
            }
            //to email me***
            if (x.ToLower().IndexOf("to email me") > -1)
            {
                int ix = x.ToLower().IndexOf("to email me");
                x = x.Substring(0, ix);
            }
            //Email me***
            if (x.ToLower().IndexOf("email me") > -1)
            {
                int ix = x.ToLower().IndexOf("email me");
                x = x.Substring(0, ix);
            }
            //I am located in***
            if (x.ToLower().IndexOf("i am located in") > -1)
            {
                int ix = x.ToLower().IndexOf("i am located in");
                x = x.Substring(0, ix);
            }
            //you may contact at***
            if (x.ToLower().IndexOf("you may contact at") > -1)
            {
                int ix = x.ToLower().IndexOf("you may contact at");
                x = x.Substring(0, ix);
            }
            //My website***
            if (x.ToLower().IndexOf("my website") > -1)
            {
                int ix = x.ToLower().IndexOf("my website");
                x = x.Substring(0, ix);
            }
            //by emailing***
            if (x.ToLower().IndexOf("by emailing") > -1)
            {
                int ix = x.ToLower().IndexOf("by emailing");
                x = x.Substring(0, ix);
            }
            //my email address***
            if (x.ToLower().IndexOf("my email address") > -1)
            {
                int ix = x.ToLower().IndexOf("my email address");
                x = x.Substring(0, ix);
            }
            //welcome to call me***
            if (x.ToLower().IndexOf("welcome to call me") > -1)
            {
                int ix = x.ToLower().IndexOf("welcome to call me");
                x = x.Substring(0, ix);
            }
            //Email me at***
            if (x.ToLower().IndexOf("email me at") > -1)
            {
                int ix = x.ToLower().IndexOf("email me at");
                x = x.Substring(0, ix);
            }
            //whom you may contact at***
            if (x.ToLower().IndexOf("whom you may contact at") > -1)
            {
                int ix = x.ToLower().IndexOf("whom you may contact at");
                x = x.Substring(0, ix);
            }
            //For more information***
            if (x.ToLower().IndexOf("for more information") > -1)
            {
                int ix = x.ToLower().IndexOf("for more information");
                x = x.Substring(0, ix);
            }

            //More information 
            if (x.ToLower().IndexOf("more information") > -1)
            {
                int ix = x.ToLower().IndexOf("more information");
                x = x.Substring(0, ix);
            }

            //For more info***
            if (x.ToLower().IndexOf("for more info") > -1)
            {
                int ix = x.ToLower().IndexOf("for more info");
                x = x.Substring(0, ix);
            }
            //Post Office Box***
            if (x.ToLower().IndexOf("post office box") > -1)
            {
                int ix = x.ToLower().IndexOf("post office box");
                x = x.Substring(0, ix);
            }
            //I am located in***
            if (x.ToLower().IndexOf("i am located in") > -1)
            {
                int ix = x.ToLower().IndexOf("i am located in");
                x = x.Substring(0, ix);
            }
            //site on ***
            if (x.ToLower().IndexOf("site on ") > -1)
            {
                int ix = x.ToLower().IndexOf("site on ");
                x = x.Substring(0, ix);
            }
            //please visit***
            if (x.ToLower().IndexOf("please visit") > -1)
            {
                int ix = x.ToLower().IndexOf("please visit");
                x = x.Substring(0, ix);
            }

            //Website:***         ok
            if (x.ToLower().IndexOf("website:") > -1)
            {
                int ix = x.ToLower().IndexOf("website:");
                x = x.Substring(0, ix);
            }
            //sites:***
            if (x.ToLower().IndexOf("sites:") > -1)
            {
                int ix = x.ToLower().IndexOf("sites:");
                x = x.Substring(0, ix);
            }

            //visit http***
            if (x.ToLower().IndexOf("visit http") > -1)
            {
                int ix = x.ToLower().IndexOf("visit http");
                x = x.Substring(0, ix);
            }

            //≤≥
            x = x.Replace("[", "【").Replace("]", "】");
            //[***] 
            x = Regex.Replace(x, @"【.*?】", "", RegexOptions.IgnoreCase);

            //http://***  <-
            x = Regex.Replace(x, @" http://.*?[^ ]", "", RegexOptions.IgnoreCase);
            x = Regex.Replace(x, @" www.*?[^ ]", "", RegexOptions.IgnoreCase);




            //http://***.html
            x = Regex.Replace(x, @"http://.*?[^ ].html", "", RegexOptions.IgnoreCase);
            //http://***.htm
            x = Regex.Replace(x, @"http://.*?[^ ].htm", "", RegexOptions.IgnoreCase);
            //http://***.jsp
            x = Regex.Replace(x, @"http://.*?[^ ].jsp", "", RegexOptions.IgnoreCase);
            //http://***.net
            x = Regex.Replace(x, @"http://.*[^ ].net", "", RegexOptions.IgnoreCase);
            //http://***.org
            x = Regex.Replace(x, @"http://.*?[^ ].org", "", RegexOptions.IgnoreCase);
            //http://***.com/
            x = Regex.Replace(x, @"http://.*?[^ ].com/", "", RegexOptions.IgnoreCase);
            //http://www.***.NET.au
            x = Regex.Replace(x, @"www.*?[^ ].au", "", RegexOptions.IgnoreCase);
            //visit: ***.com
            x = Regex.Replace(x, @"visit: .*?[^ ].com", "", RegexOptions.IgnoreCase);
            //www.***.com
            x = Regex.Replace(x, @"www.*?[^ ].com", "", RegexOptions.IgnoreCase);
            //www.***.org
            x = Regex.Replace(x, @"www.*?[^ ].org", "", RegexOptions.IgnoreCase);
            //www.***.net
            x = Regex.Replace(x, @"www.*?[^ ].net", "", RegexOptions.IgnoreCase);

            //http://***.aspx
            x = Regex.Replace(x, @"http://.*?[^ ].aspx", "", RegexOptions.IgnoreCase);


            //@
            x = Regex.Replace(x, @"([^ ]*)@([^ ]*)", "", RegexOptions.IgnoreCase);


            //***.net <-
            x = Regex.Replace(x, @"([^ ]*).net ", "", RegexOptions.IgnoreCase);
            //***.com <-
            x = Regex.Replace(x, @"([^ ]*).com ", "", RegexOptions.IgnoreCase);
            //***.org <-
            x = Regex.Replace(x, @"([^ ]*).org ", "", RegexOptions.IgnoreCase);

            //abc.123/***.html
            x = Regex.Replace(x, @"([^ ]*).html ", "", RegexOptions.IgnoreCase);
            //abc.123/***.htm
            x = Regex.Replace(x, @"([^ ]*).htm ", "", RegexOptions.IgnoreCase);
            //abc.123/***.aspx
            x = Regex.Replace(x, @"([^ ]*).aspx ", "", RegexOptions.IgnoreCase);
            //abc.123/***.jsp
            x = Regex.Replace(x, @"([^ ]*).jsp ", "", RegexOptions.IgnoreCase);
            //abc.-123/***.php
            x = Regex.Replace(x, @"([^ ]*).php ", "", RegexOptions.IgnoreCase);





            x = Regex.Replace(x, @" http://.*?[^ ]", "", RegexOptions.IgnoreCase);


            return x;

        }

        private void button82_Click(object sender, EventArgs e)
        {


            // x = Regex.Replace(x, @"{.*?}", spinner_txt2spin(Regex.Match(x,@"{.*?}")), RegexOptions.IgnoreCase);


            string str = "pig: {one day|at some point|in the future|someday|sooner or later} hello {steven|tom}  ";

            int CCC = str.Length - str.Replace("{", "").Length;

            string cat = "";
            int KEY_INDEX = str.IndexOf("{");
            string Xman = str.Substring(0, KEY_INDEX);
            int startIndex = 0;
            int endIndex = 0;

            for (int i = 0; i < CCC; i++)
            {
                cat = str.Substring(0, KEY_INDEX);
                cat = str.Replace(cat, "");//【新世纪】

                startIndex = cat.IndexOf("{");

                Xman += cat.Substring(0, startIndex);

                string mouse = cat.Substring(0, startIndex + 1);//从【开头】到【开始字符串】
                mouse = cat.Replace(mouse, "");
                endIndex = mouse.IndexOf("}");

                string pig = cat.Substring(startIndex + 1, endIndex).Trim();
                string[] dog_small = pig.Split('|');

                Random ra = new Random();
                int num = dog_small.Length;
                int value = ra.Next(num);

                Xman += dog_small[value].ToString();

                KEY_INDEX += startIndex + 1 + endIndex + 1;
            }

            MessageBox.Show(Xman);



        }


        public string spinner2fulltext(string str)
        {

            int CCC = str.Length - str.Replace("{", "").Length;

            string cat = "";
            int KEY_INDEX = str.IndexOf("{");

            if (KEY_INDEX == -1)
            {
                return "";
            }

            string Xman = str.Substring(0, KEY_INDEX);
            int startIndex = 0;
            int endIndex = 0;

            for (int i = 0; i < CCC; i++)
            {
                cat = str.Substring(0, KEY_INDEX);
                cat = str.Replace(cat, "");//【新世纪】

                startIndex = cat.IndexOf("{");

                Xman += cat.Substring(0, startIndex);

                string mouse = cat.Substring(0, startIndex + 1);//从【开头】到【开始字符串】
                mouse = cat.Replace(mouse, "");
                endIndex = mouse.IndexOf("}");

                string pig = cat.Substring(startIndex + 1, endIndex);
                string[] dog_small = pig.Split('|');

                Random ra = new Random();
                int num = dog_small.Length;
                int value = ra.Next(num);

                Xman += dog_small[value].ToString();

                KEY_INDEX += startIndex + 1 + endIndex + 1;
            }


            Xman += str.Substring(KEY_INDEX, str.Length - KEY_INDEX - 1);

            return Xman;

        }

        private void l_html_TextChanged(object sender, EventArgs e)
        {

        }

        private void button83_Click(object sender, EventArgs e)
        {
            button83.Enabled = false;

            Thread thread = new Thread(spinner_a);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;


            thread.Start();

            button83.Enabled = true;
        }


        public void spinner_a()
        {
            /*
              
                 select 'a = reg'+tag+'.Replace(a,"【'+tag+'】");' as Da1,
    'static Regex reg'+tag+' = new Regex(" '+a+' ", RegexOptions.Compiled | RegexOptions.IgnoreCase);' as Da2,
    'a = reg'+tag+'.Replace(a,"'+spinner+'");' as Db1,
    'static Regex reg'+tag+' = new Regex("【'+tag+'】", RegexOptions.Compiled | RegexOptions.IgnoreCase);' as Db2
    from a_gp4 where flag=1 and status=1 

                 */

            //TrueLan_Format("Three Ways to Step Out of Your Fashion Comfort Zone - Go Crazy With Bone Carved Jewelry , how are you? did you know? Forget the Diamond - How About Bone Carved Jewelry? ");
            //Three {Ways|means} to {Step|footfall} Out of Your Fashion {Comfort|abundance} {Zone|area} - Go Crazy With {Bone|cartilage} Carved Jewelry , howwareeyouudidiyouo{know|apperceive}o{Forget|overlook}gthetDiamondo-dHowHAbouto{Bone|cartilage}oCarvedvJewelrylr

            DataGroup group = new DataGroup();

            string ls_group = "select ID,TITLE1,TITLE2,TEXT1,TEXT2,TEXT3,TEXT4 from BLOG_B2 WHERE STATUS='8' ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    string ls_text = group.Table.Rows[x]["TEXT1"].ToString().Trim();

                    ls_text = ls_text.Replace(",,", ",");
                    ls_text = ls_text.Replace("..", ".");
                    ls_text = ls_text.Replace("!!", "!");
                    ls_text = ls_text.Replace("??", "?");
                    ls_text = ls_text.Replace(";;", ";");

                    ls_text = ls_text.Replace("{", "(");
                    ls_text = ls_text.Replace("}", ")");

                    ls_text = ls_text.Replace(",", " , ");
                    ls_text = ls_text.Replace(".", " . ");
                    ls_text = ls_text.Replace("!", " ! ");
                    ls_text = ls_text.Replace("?", " ? ");
                    ls_text = ls_text.Replace(";", " ; ");
                    ls_text = ls_text.Replace("  ", " ");

                    ls_text = " " + ls_text.Trim().Replace("'", "㊣") + " ";

                    ls_text = format_clear_flag(ls_text);


                    DB.ExecuteSQL("Update BLOG_B2 set status='A' , TEXT2='" + ls_text + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'");


                }
            }



            MessageBox.Show("OK");
        }

        private void button84_Click(object sender, EventArgs e)
        {

            button84.Enabled = false;

            Thread thread = new Thread(spinner_b);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;

            thread.Start();

            button84.Enabled = true;
        }

        public void spinner_b()
        {
            DataGroup group = new DataGroup();

            string ls_group = "select ID,TITLE1,TITLE2,TEXT1,TEXT2,TEXT3,TEXT4 from BLOG_B2 WHERE STATUS='A' ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    string ls_text = group.Table.Rows[x]["TEXT2"].ToString().Trim();

                    //spinner_txt2tag_v1 xxx = new spinner_txt2tag_v1();
                    //ls_text = xxx.spinner_txt2tag(ls_text);

                    //spinner_tag2spin_v1 yyy = new spinner_tag2spin_v1();
                    //ls_text = yyy.spinner_tag2spin(ls_text);

                    DB.ExecuteSQL("Update BLOG_B2 set status='B' , TEXT3='" + ls_text + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'");

                    //spinner_txt2tag_v1 xxx = new spinner_txt2tag_v1();
                    //ls_text = xxx.spinner_txt2tag(ls_text);

                    //spinner_tag2spin_v1 yyy = new spinner_tag2spin_v1();
                    //string ls_text4 = yyy.spinner_tag2spin(ls_text);

                    //string  ls_text5 = spinner2fulltext(' ' + ls_text4);

                    //DB.ExecuteSQL("Update BLOG_B2 set TEXT5='" + ls_text5.Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'");
                }
            }


            MessageBox.Show("OK");
        }

        private void button85_Click(object sender, EventArgs e)
        {

            button85.Enabled = false;

            Thread thread = new Thread(spinner_c);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;

            thread.Start();

            button85.Enabled = true;
        }

        public void spinner_c()
        {
            DataGroup group = new DataGroup();

            string ls_group = "select ID,TITLE1,TITLE2,TEXT1,TEXT2,TEXT3,TEXT4 from BLOG_B2 WHERE STATUS='B' ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    string ls_text = group.Table.Rows[x]["TEXT3"].ToString().Trim();

                    try
                    {
                        string ls_text1 = spinner2fulltext("   " + ls_text + " ");
                        string ls_text2 = spinner2fulltext("   " + ls_text + " ");

                        DB.ExecuteSQL("Update BLOG_B2 set status='C' , TEXT4='" + ls_text1.Replace("  ", "").Replace("㊣", "''").Trim() + "', TEXT5='" + ls_text2.Replace("  ", "").Replace("㊣", "''").Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'");
                    }
                    catch
                    {
                        DB.ExecuteSQL("Update BLOG_B2 set status='8'  where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'");
                    }

                }
            }

            MessageBox.Show("OK");
        }


        public void spinner_title()
        {
            DataGroup group = new DataGroup();

            string ls_group = "select ID,TITLE1,TITLE2,TITLE3 from BLOG_B2 WHERE STATUS='D' ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    string ls_text = group.Table.Rows[x]["TITLE1"].ToString().Trim();

                    ls_text = ls_text.Replace(",,", ",");
                    ls_text = ls_text.Replace("..", ".");
                    ls_text = ls_text.Replace("!!", "!");
                    ls_text = ls_text.Replace("??", "?");
                    ls_text = ls_text.Replace(";;", ";");

                    ls_text = ls_text.Replace("{", "(");
                    ls_text = ls_text.Replace("}", ")");

                    ls_text = ls_text.Replace(",", " , ");
                    ls_text = ls_text.Replace(".", " . ");
                    ls_text = ls_text.Replace("!", " ! ");
                    ls_text = ls_text.Replace("?", " ? ");
                    ls_text = ls_text.Replace(";", " ; ");
                    ls_text = ls_text.Replace("  ", " ");

                    ls_text = " " + ls_text.Trim().Replace("'", "㊣") + " ";

                    ls_text = format_clear_flag(ls_text);

                    //spinner_txt2tag_v1 xxx = new spinner_txt2tag_v1();
                    //ls_text = xxx.spinner_txt2tag(ls_text);

                    //spinner_tag2spin_v1 yyy = new spinner_tag2spin_v1();
                    //ls_text = yyy.spinner_tag2spin(ls_text);

                    string ls_text1 = spinner2fulltext("   " + ls_text + " ");
                    string ls_text2 = spinner2fulltext("   " + ls_text + " ");

                    if (ls_text1 == "")
                    {
                        ls_text1 = ls_text;
                    }
                    if (ls_text2 == "")
                    {
                        ls_text2 = ls_text;
                    }

                    DB.ExecuteSQL("Update BLOG_B2 set status='D' , TITLE2='" + ls_text1.Replace("  ", "").Replace("㊣", "''").Trim() + "', TITLE3='" + ls_text2.Replace("  ", "").Replace("㊣", "''").Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'");

                }
            }


            MessageBox.Show("OK");
        }

        private void button86_Click(object sender, EventArgs e)
        {

            button86.Enabled = false;

            Thread thread = new Thread(spinner_title);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;

            thread.Start();

            button86.Enabled = true;
        }


        private void button87_Click(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();

            string ls_group = "select ID,TEXT3,TEXT4,TEXT5 from BLOG_B2 WHERE STATUS in ('D','E') and (pid <186 or pid >264)  ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    /*
                    string ls_text4 = group.Table.Rows[x]["TEXT4"].ToString().Trim().Replace("㊣", "''").Replace("'", "''").Replace("  ", " ").Replace("''''", "''");
                    string ls_text5 = group.Table.Rows[x]["TEXT5"].ToString().Trim().Replace("㊣", "''").Replace("'", "''").Replace("  ", " ").Replace("''''", "''");

                    ls_text4 = TrueLan(ls_text4);
                    ls_text5 = TrueLan(ls_text5);

                    DB.ExecuteSQL("Update BLOG_B2 set status='E' , TEXT6='" + ls_text4 + "', TEXT7='" + ls_text5 + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'");
                    */

                    string ls_text3 = group.Table.Rows[x]["TEXT3"].ToString().Trim().Replace("㊣", "''").Replace("'", "''").Replace("  ", " ").Replace("''''", "''");

                    ls_text3 = TrueLan(ls_text3);

                    DB.ExecuteSQL("Update BLOG_B2 set status='E' , TEXT6='" + ls_text3 + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'");

                }
            }


            MessageBox.Show("OK");




        }

        private void button80_Click(object sender, EventArgs e)
        {
            //of that for on a an as with

            TextInfo tInfo = Thread.CurrentThread.CurrentCulture.TextInfo;



            DataGroup group = new DataGroup();

            string ls_group = "select ID,TITLE2,TITLE3 from BLOG_B2 WHERE STATUS in ('D','E') ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    string ls_text4 = group.Table.Rows[x]["TITLE2"].ToString().Trim().Replace("㊣", "''").Replace("'", "''").Replace("  ", " ").Replace("''''", "''");
                    string ls_text5 = group.Table.Rows[x]["TITLE3"].ToString().Trim().Replace("㊣", "''").Replace("'", "''").Replace("  ", " ").Replace("''''", "''");

                    ls_text4 = ls_text4.ToLower();
                    ls_text5 = ls_text5.ToLower();


                    ls_text4 = tInfo.ToTitleCase(ls_text4);
                    ls_text5 = tInfo.ToTitleCase(ls_text4);

                    ls_text4 = ls_text4.Replace(" Of ", " of ").Replace(" That ", " that ").Replace(" For ", " for ").Replace(" On ", " on ").Replace(" A ", " a ").Replace(" An ", " an ").Replace(" As ", " as ").Replace(" With ", " with ").Replace(" In ", " in ").Replace(" By ", " by ").Replace(" At ", " at ").Replace(" From ", " from ").Replace(" To ", " to ");
                    ls_text5 = ls_text5.Replace(" Of ", " of ").Replace(" That ", " that ").Replace(" For ", " for ").Replace(" On ", " on ").Replace(" A ", " a ").Replace(" An ", " an ").Replace(" As ", " as ").Replace(" With ", " with ").Replace(" In ", " in ").Replace(" By ", " by ").Replace(" At ", " at ").Replace(" From ", " from ").Replace(" To ", " to ");

                    DB.ExecuteSQL("Update BLOG_B2 set status='E' , TITLE2='" + ls_text4 + "', TITLE3='" + ls_text5 + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'");

                }
            }


            MessageBox.Show("OK");
        }

        private void button88_Click(object sender, EventArgs e)
        {

        }

        private string returnSpinnedPart(string part)
        {
            Random random = new Random();

            string[] values = part.Split('|');

            if (values.Length > 0)
                return values[random.Next(0, values.Length)];

            return "";
        }

        private string returnSpinnedContent(string input)
        {
            string spinned = input;
            try
            {
                Regex regexObj = new Regex(@"\{.*?\}", RegexOptions.Singleline);
                MatchCollection m = regexObj.Matches(input);
                foreach (Match match in m)
                {
                    string part = returnSpinnedPart(match.Value.Replace("{", "").Replace("}", ""));
                    spinned = spinned.Replace(match.Value, part);
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("RegEx prob");
            }
            return spinned;
        }

        
        private void button90_Click(object sender, EventArgs e)
        {

            string ls_add = "<a href={xx}</a>";

            DataGroup group = new DataGroup();

            string ls_group = "select id,title3 as h1,text6 as t1 from BLOG_B2 where status='e' and (pid <186 or pid >264) and len(title3) >2";// "select id,base,s,t,a,b from xrumer_symon";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {


                    string filename = group.Table.Rows[x]["h1"].ToString().Replace("?", "").Replace(":", "").Replace("/", "-").Trim();
                    string filetext = group.Table.Rows[x]["t1"].ToString().Trim() + " " + ls_add;

                    FileStream fs = new FileStream(@"d:\doc-jade3\" + filename + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("utf-8"));
                    sw.Flush();
                    sw.BaseStream.Seek(0, SeekOrigin.Begin);
                    sw.WriteLine(filetext);
                    sw.Flush();
                    sw.Close();


                    /*
                    string ls_a = group.Table.Rows[x]["a"].ToString().Trim().Replace("'", "''");
                    string ls_b = group.Table.Rows[x]["b"].ToString().Trim().Replace("'", "''");

                    string ls_t = group.Table.Rows[x]["t"].ToString().Trim().Replace("'", "''");


                    if (ls_a.Length > 0)
                    {
                        string bb = "";

                        if (ls_t.Length > 0)
                        {
                            bb = ls_t;
                        }

                        if (ls_b.Length > 0)
                        {
                            bb = ls_b + ',' + bb;
                        }

                        string ls_up = "insert into xrumer_symon2 (s,t) values ('" + ls_a + "','" + bb + "')";
                        DB.ExecuteSQL(ls_up.Replace(",,", ","));
                    }


                    //if (ls_b.Length > 0)
                    //{
                    //    string bb = "";

                    //    if (ls_t.Length > 0)
                    //    {
                    //        bb = ls_t;
                    //    }

                    //    if (ls_a.Length > 0)
                    //    {
                    //        bb = ls_a + ',' + bb;
                    //    }

                    //    string ls_up = "insert into xrumer_symon2 (s,t) values ('" + ls_b + "','" + bb + "')";
                    //    DB.ExecuteSQL(ls_up.Replace(",,", ","));
                    //}
                    */





                }


            }

            MessageBox.Show("OK!!");
        }

        private void button91_Click(object sender, EventArgs e)
        {

            //Thread thread = new Thread(GET_GOOGLE_PING);

            //Thread.Sleep(2000);//等待200毫秒

            //thread.IsBackground = true;

            //thread.Start();


            //GET_GOOGLE_PING();



            OFDScript.Title = "打开(Open)";
            OFDScript.FileName = "";
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.Filter = "All files (*.*)|*.*|SiteMap文件(*.xml)|*.xml|文本文件(*.txt)|*.txt";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性


            OFDScript.InitialDirectory = "e:\\downloads\\";
            OFDScript.RestoreDirectory = true;


            try
            {
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    StreamReader sr = new StreamReader(OFDScript.FileName, System.Text.Encoding.Default);
                    string ls_googleping = sr.ReadToEnd();


                    string ls_name = OFDScript.FileName.Replace(".txt", "").Replace(".xml", "") + "-KO.txt";

                    //提取好的
                    string ls_ok = NoGoogle(ls_googleping);
                    using (FileStream fs = File.Open(@ls_name, FileMode.Append))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_ok);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                    }

                    ls_googleping = null;
                    ls_ok = null;
                    sr.Close();
                    

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            MessageBox.Show("GOOGLE OK!!");
        }

        public static string NoGoogle(string Htmlstring)
        {
            Htmlstring = Regex.Replace(Htmlstring, @"<weblog.*?url=.", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @". rssUrl=.*?/>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<.*?>", "", RegexOptions.IgnoreCase);
            return Htmlstring;
        }

        private void button92_Click(object sender, EventArgs e)
        {
            OFDScript.Title = "打开(Open)";
            OFDScript.FileName = "";
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.Filter = "文本文件(*.txt)|*.txt";//All files (*.*)|*.*|SiteMap文件(*.xml)|*.xml|
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性

            OFDScript.InitialDirectory = "e:\\downloads\\";
            OFDScript.RestoreDirectory = true;

            if (OFDScript.ShowDialog() == DialogResult.OK)
            {
                //获取去掉重复的url
                //FileStream fs = new FileStream(OFDScript.FileName, FileMode.Open, FileAccess.Read);
                //BinaryReader br = new BinaryReader(fs);
                //br.BaseStream.Seek(0, SeekOrigin.Begin);
                //string[] bytes = br.ReadString((int)br.BaseStream.Length);

                ////数组转换成string
                //string[] outltempArray = Array.ConvertAll(bytes, (Converter<char, string>)Convert.ToString);

                StreamReader fs = new StreamReader(OFDScript.FileName, System.Text.Encoding.Default);
                string ls_googleping = fs.ReadToEnd().Replace("\r\n", "|").Replace("\n", "|");

                string[] outltempArray = ls_googleping.Split('|');

                //正则表达抓取
                string[] ltempArray = this.regexinput.Lines;
                //string[] outltempArray = this.xmloutput.Lines;
                //this.regexoutput.Text = "";
                int ppp = ltempArray.GetUpperBound(0);
                for (int counter = 0; counter <= ppp; counter++)
                {
                    int xxx = outltempArray.GetUpperBound(0);
                    for (int counter2 = 0; counter2 <= xxx; counter2++)
                    {
                        Regex r = new Regex(ltempArray[counter]);
                        if (r.IsMatch(outltempArray[counter2]))
                        {
                            //this.regexoutput.Text = this.regexoutput.Text + outltempArray[counter2] + "\r\n";

                            string ls_name = OFDScript.FileName.Replace(".txt", "").Replace(".xml", "") + "-MPR.txt";

                            using (FileStream fs2 = File.Open(@ls_name, FileMode.Append))
                            {
                                byte[] b = System.Text.Encoding.Default.GetBytes(outltempArray[counter2] + "\r\n");
                                fs2.Write(b, 0, b.Length);
                                fs2.Close();
                            }


                        }
                    }
                }





            }

            MessageBox.Show("OK");
        }

        private void button92_Click_1(object sender, EventArgs e)
        {
            StreamReader fs = new StreamReader(@"e:\\downloads\\0RegCode.txt", System.Text.Encoding.Default);
            regexinput.Text = fs.ReadToEnd();

        }

        private void button93_Click(object sender, EventArgs e)
        {
            OFDScript.Title = "打开(Open)";
            OFDScript.FileName = "";
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.Filter = "文本文件(*.txt)|*.txt";//All files (*.*)|*.*|SiteMap文件(*.xml)|*.xml|
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性

            OFDScript.InitialDirectory = "e:\\downloads\\";
            OFDScript.RestoreDirectory = true;

            if (OFDScript.ShowDialog() == DialogResult.OK)
            {

                StreamReader fs = new StreamReader(OFDScript.FileName, System.Text.Encoding.Default);
                string ls_googleping = fs.ReadToEnd().Replace("\r\n", "|").Replace("\n", "|");

                string[] outltempArray = ls_googleping.Split('|');


                int xxx = outltempArray.GetUpperBound(0);
                for (int counter2 = 0; counter2 <= xxx; counter2++)
                {

                    string ls_name = OFDScript.FileName.Replace(".txt", "").Replace(".xml", "") + "-MPRF.txt";

                    if (outltempArray[counter2].IndexOf("profile") > 1 || outltempArray[counter2].IndexOf("member") > 1 || outltempArray[counter2].IndexOf("user") > 1 || outltempArray[counter2].IndexOf("yabb") > 1)
                    {
                        FileStream fs2 = File.Open(@ls_name, FileMode.Append);
                        byte[] b = System.Text.Encoding.Default.GetBytes(outltempArray[counter2] + "\r\n");
                        fs2.Write(b, 0, b.Length);
                        fs2.Close();
                    }
                }

            }

            MessageBox.Show("OK");
        }

        private void button94_Click(object sender, EventArgs e)
        {
            OFDScript.Title = "打开(Open)";
            OFDScript.FileName = "";
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.Filter = "Profile文本(*.txt)|*.txt";//All files (*.*)|*.*|SiteMap文件(*.xml)|*.xml|
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性

            OFDScript.InitialDirectory = "e:\\downloads\\";
            OFDScript.RestoreDirectory = true;

            if (OFDScript.ShowDialog() == DialogResult.OK)
            {

                StreamReader fs = new StreamReader(OFDScript.FileName, System.Text.Encoding.Default);
                string ls_googleping = fs.ReadToEnd().Replace("\r\n", "|").Replace("\n", "|");

                string[] outltempArray = ls_googleping.Split('|');

                string ls_name = OFDScript.FileName.Replace(".txt", "").Replace(".xml", "") + "-Purl.txt";

                int xxx = outltempArray.GetUpperBound(0);
                for (int counter2 = 0; counter2 <= xxx; counter2++)
                {



                }

            }

            MessageBox.Show("OK");
        }

        private void button95_Click(object sender, EventArgs e)
        {
            //Microshaoft.Utils.HttpWebClient xxx = new Microshaoft.Utils.HttpWebClient();
            //xxx.DownloadFile("http://blogsearch.google.com/changes.xml", "c:\\11.xml", 5);

            Thread thread = new Thread(dfiles);
            Thread.Sleep(2000);//等待200毫秒
            thread.IsBackground = true;
            thread.Start();
        }

        public void dfiles()
        {
            string lsResult;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://blogsearch.google.com/changes.xml");
            HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(rep.GetResponseStream(), Encoding.GetEncoding("utf-8"));

            lsResult = sr.ReadToEnd();

            using (FileStream fs = File.Open(@"c:\\changes-" + DateTime.Now.ToString().Replace(":", "").Replace(" ", "-") + ".xml", FileMode.Append))
            {
                byte[] b = System.Text.Encoding.Default.GetBytes(lsResult);
                fs.Write(b, 0, b.Length);

                fs.Close();
            }



        }

        private void button96_Click(object sender, EventArgs e)
        {
            //启动定时器
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(online);

            aTimer.Interval = 1000;//这里设置时间为1秒
            aTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            aTimer.Enabled = true;

        }


        private void online(object source, System.Timers.ElapsedEventArgs e)
        {
            System.Timers.Timer t = null;
            if (source is System.Timers.Timer)
            {
                t = source as System.Timers.Timer;//获取定时器时间
            }
            t.Stop();//停止定时器
            {
                //定时下载
                dfiles();
                //MessageBox.Show(DateTime.Now.ToString());

            }
            t.Interval = 1800000;//重新修改定时器时间
            t.Start();//启动定时器
        }

        private void button97_Click(object sender, EventArgs e)
        {

        }


        [System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);
        public void Dispose()
        {
            GC.Collect(); GC.SuppressFinalize(this);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        private void button98_Click(object sender, EventArgs e)
        {
            //desc_cn  colour  weight  material  规格  pattern   objects  originalarea
            //desc_cn2
            //select 'update `jv1_products_description` set products_description="'+replace(desc_en2,'"','''')+'"  where products_name="'+main_class+'-'+replace(sub_class,'/',' ')+' '+ename+'";' from jade_b3 where mid is not null

            //select 'update `jv1_products_description` set `bigimghtml`="'+replace(bigimghtml,'"','''')+'"  where products_name="'+pname+'";' as ok from JewelOra_b3 where status=4


            /*         SELECT [id]
                      ,[pack]
                      ,[Main material]
                      ,[Auxiliary Material]
                      ,[gg]
                      ,[KK]
                      ,[model]
                      ,[color2]
                  FROM [Jade].[dbo].[Xuping_engs]

                [unionattrib]
            */

            DataGroup group_field = new DataGroup();
            group_field = null;

            DataGroup group = new DataGroup();
            group = null;


            string ls_sql = "select id ,[Main Material] as aaa ,[Auxiliary Material] as bbb,gg,KK,Weight,pack,color2,model from Xuping_engs   order by id";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {
                    string ls_shtml = "";

                    //if (group_field.Table.Rows[i]["pname"].ToString().Trim().Length > 1)
                    //{
                    //    ls_shtml += "<p>" + group_field.Table.Rows[i]["pname"].ToString().Trim() + "</p>";
                    //}
                    //else
                    //{
                    //    //ls_shtml += "<p>" + group_field.Table.Rows[i]["ename"].ToString().Trim() + "</p>";
                    //}
                    if (group_field.Table.Rows[i]["aaa"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p><b>Main Material:</b>" + group_field.Table.Rows[i]["aaa"].ToString().Trim() + "</p>";
                    }


                    if (group_field.Table.Rows[i]["bbb"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p><b>Auxiliary Material:</b>" + group_field.Table.Rows[i]["bbb"].ToString().Trim() + "</p>";
                    }


                    if (group_field.Table.Rows[i]["gg"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p><b>Specifications:</b>" + group_field.Table.Rows[i]["gg"].ToString().Trim() + "</p>";
                    }


                    if (group_field.Table.Rows[i]["color2"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p><b>Color:</b>" + group_field.Table.Rows[i]["color2"].ToString().Trim() + "</p>";
                    }

                    if (group_field.Table.Rows[i]["Weight"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p><b>Weight:</b>" + group_field.Table.Rows[i]["Weight"].ToString().Trim() + "g</p>";
                    }

                    string a = "<div style=\"float:left;width:420px;margin-top:5px;\">";

                    string ls_up2 = "update Xuping_engs set unionattrib ='" + a + ls_shtml.Replace('\'', '‘') + "</div>'  where id='" + group_field.Table.Rows[i]["id"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up2);

                }
            }

            MessageBox.Show("OK!");

        }



        private void button89_Click(object sender, EventArgs e)
        {
            //desc_cn  colour  weight  material  规格  pattern   objects  originalarea
            //desc_cn2
            //select 'update `jv1_products_description` set products_description="'+replace(desc_en2,'"','''')+'"  where products_name="'+main_class+'-'+replace(sub_class,'/',' ')+' '+ename+'";' from jade_b3 where mid is not null

            //select 'update `jv1_products_description` set `bigimghtml`="'+replace(bigimghtml,'"','''')+'"  where products_name="'+pname+'";' as ok from JewelOra_b3 where status=4



            DataGroup group_field = new DataGroup();
            group_field = null;

            DataGroup group_if = new DataGroup();
            group_if = null;

            string ls_sql = "select id ,[Main Material] as aaa ,[Auxiliary Material] as bbb,gg,KK,Weight,pack,color2,model,fullname,[desc] from Xuping_engs   order by id";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {

                    string ls_id = group_field.Table.Rows[i]["id"].ToString();
                    string ls_fullname = group_field.Table.Rows[i]["fullname"].ToString();

                    string ls_shtml = "";
                    string dog = "";

                    string ls_group_if = "select id from Xuping_pic where pid='" + ls_id + "' order by id ";
                    group_if = null;
                    group_if = DB.GetDataGroup(ls_group_if);
                    if (group_if.Table.Rows.Count > 0)
                    {
                        for (int y = 0; y < group_if.Table.Rows.Count; y++)
                        {
                            dog += "<img src=\"http://images.wholesalejewelryshop.org/" + ls_fullname + "_" + (y + 1) + ".jpg\" alt=\"" + ls_fullname + "_" + (y + 1) + "\" title=\"" + ls_fullname + "_" + (y + 1) + "\" /><br/>";
                        }

                    }

                    

                    if (group_field.Table.Rows[i]["desc"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<div class=\"sidebox-header-left\"><h3 class=\"leftBoxHeading\" >Product Description:</h3></div><div class=\"sideBoxContent\">"
                            + group_field.Table.Rows[i]["desc"].ToString().Trim() + "</div><br>";
                    }

                    if (dog.Length > 1)
                    {
                        ls_shtml += "<div class=\"sidebox-header-left\"><h3 class=\"leftBoxHeading\">Product Images:</h3></div><div class=\"sideBoxContent\"   align=\"center\">"
                            + dog + "</div>";
                    }

                    string ls_up = "update Xuping_engs set bigimghtml ='" + ls_shtml.Replace('\'', '‘') + "'  where id='" + group_field.Table.Rows[i]["id"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up);


                    //if (group_field.Table.Rows[i]["Packing_Method"].ToString().Trim().Length > 1)
                    //{
                    //    ls_shtml2 += "<div class=\"sidebox-header-left\"><h3 class=\"leftBoxHeading\" >Packing Method:</h3></div><div class=\"sideBoxContent\">"
                    //        + group_field.Table.Rows[i]["Packing_Method"].ToString().Trim() + "</div>";
                    //}

                }


                MessageBox.Show("OK!");


            }
        }


        private void button99_Click(object sender, EventArgs e)
        {

            DataGroup group = new DataGroup();
            DataGroup group_if = new DataGroup();

            string ls_group = "select distinct color as attrib from Xuping_engs where len(color) >0 ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {

                    string attrib = group.Table.Rows[x]["attrib"].ToString().Replace("、", "/").Replace("，", "/").Trim();

                    if (attrib.IndexOf("/") > -1)
                    {

                        string[] dog_small = attrib.Split('/');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = bb.Trim();

                            group_if = null;
                            string ls_double = "select id from plan425_ATTRIB where name='" + OK_bb + "' and status=1   ";
                            group_if = DB.GetDataGroup(ls_double);
                            if (group_if.Table.Rows.Count == 0)
                            {
                                string ls_up = "insert into plan425_ATTRIB (name,status) values ('" + OK_bb + "',1)";
                                DB.ExecuteSQL(ls_up);
                            }
                        }
                    }
                    else
                    {
                        group_if = null;
                        string ls_double = "select id from plan425_ATTRIB where name='" + attrib + "' and status=1   ";
                        group_if = DB.GetDataGroup(ls_double);
                        if (group_if.Table.Rows.Count == 0)
                        {
                            string ls_up = "insert into plan425_ATTRIB (name,status) values ('" + attrib + "',1)";
                            DB.ExecuteSQL(ls_up);
                        }
                    }
                }

            }
            MessageBox.Show("OK!!");
        }

        private void button101_Click(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();
            DataGroup group_if = new DataGroup();

            string ls_group = "select id, color as attrib from Xuping_engs where len(color) >0 ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {

                    string attrib = group.Table.Rows[x]["attrib"].ToString().Replace("、", "/").Replace("，", "/").Trim();

                    if (attrib.IndexOf("/") > -1)
                    {

                        string[] dog_small = attrib.Split('/');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = bb.Trim();

                            group_if = null;
                            string ls_double = "select ename from plan425_ATTRIB where name='" + OK_bb + "'   ";
                            group_if = DB.GetDataGroup(ls_double);
                            if (group_if.Table.Rows.Count == 1)
                            {
                                string ls_up = "update Xuping_engs set color2 = color2 + ' / " + group_if.Table.Rows[0]["ename"].ToString().Trim() + "'  where id='" + group.Table.Rows[x]["id"].ToString() + "'  ";
                                DB.ExecuteSQL(ls_up);
                            }
                            group_if = null;
                        }
                    }
                   
                }

            }
            MessageBox.Show("OK!!");
        }

        private void button102_Click(object sender, EventArgs e)
        {

            DataGroup group = new DataGroup();

            string ls_group = "select id, color2 as attrib from Xuping_engs where len(color2) >0 ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {

                    string attrib = group.Table.Rows[x]["attrib"].ToString().Trim();

                    attrib = attrib.Substring(2,attrib.Length-2);

                    string ls_up = "update Xuping_engs set color2 =  ' " + attrib + "'  where id='" + group.Table.Rows[x]["id"].ToString() + "'  ";
                                DB.ExecuteSQL(ls_up);
                           

                }

            }
            MessageBox.Show("OK!");

        }

        private void button103_Click(object sender, EventArgs e)
        {
            DataGroup group_main_class = new DataGroup();
            string ls_group_main_class = "select distinct fullname  from xuping_engs ";
            group_main_class = DB.GetDataGroup(ls_group_main_class);
            if (group_main_class.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_main_class.Table.Rows.Count; x++)
                {
                    DataGroup group_kw = new DataGroup();
                    group_kw = null;
                    string ls_group_kw = "select id from xuping_engs where fullname='" + group_main_class.Table.Rows[x]["fullname"].ToString().Replace("'","").Trim() + "'  order by main_class,sub_class,ID ";
                    //string ls_group_kw = "select id from xuping_engs where main_class='" + group_main_class.Table.Rows[x]["main_class"].ToString().Trim() + "' and replace(sub_class,'''','-')='" + group_main_class.Table.Rows[x]["sub_class"].ToString().Trim().Replace('\'', '-') + "' order by main_class,sub_class,ID ";
                    group_kw = DB.GetDataGroup(ls_group_kw);

                    if (group_kw.Table.Rows.Count > 0)
                    {
                        for (int k = 0; k < group_kw.Table.Rows.Count; k++)
                        {

                            int pig = k;//+ 1

                            string ls_up = "UPDATE xuping_engs SET bightml='" + pig + "'  WHERE id='" + group_kw.Table.Rows[k]["id"].ToString() + "' ";
                            DB.ExecuteSQL(ls_up);

                        }
                    }

                }

            }


            MessageBox.Show("ok!");
        }

        private void button104_Click(object sender, EventArgs e)
        {
            string ls_up1 = @"  

update plan425_b2 set price=89 where price <90 and status in(2,5);

--update plan425_b2 set 
--main_class=(select v_categories_name_1 from plan425_b2_copy where v_products_model=plan425_b2.product_code),
--sub_class=(select v_categories_name_2 from plan425_b2_copy where v_products_model=plan425_b2.product_code),
--price=(select v_products_price from plan425_b2_copy where v_products_model=plan425_b2.product_code)

update plan425_b2 set sub_class=replace(sub_class,'''','');

--update plan425_b2 set pic_s='' where id in
--( select pid from  plan425_PIC where flag='S' and status=0)

update plan425_b2 set pic_s='' where id not in (select pid from plan425_pic where flag='S' and status=1) and status=2;
update plan425_b2 set pic_group='' where id not in (select pid from plan425_pic where flag='B' and status=1) and status=2;

--update plan425_b2 set status=0 where pic_s ='' and pic_group='';

update plan425_b2 set pic_name=lower(replace(replace(replace(replace(pname_ok,' ','-'),'.','-'),'#',''),'*','-'))+'.jpg'

                ";

            DB.ExecuteSQL(ls_up1); 
            
            
            
            DataGroup group = new DataGroup();
            DataGroup group_if = new DataGroup();
            DataGroup group_if2 = new DataGroup();
            DataGroup group_null = new DataGroup();
            DataGroup group_bad = new DataGroup();
            string ls_group = "select id,pname_ok as fullname from plan425_b2 where status =2  order by id ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {

                /*   */
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    string ls_id = group.Table.Rows[x]["id"].ToString();
                    string ls_fullname = group.Table.Rows[x]["fullname"].ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-").Replace(".", "-").Replace("#", "").Replace("*", "-").Trim().ToLower();

                    /*   */
                     
                    //详细的大图
                    string ls_group2 = "select id from plan425_pic where pid='" + ls_id + "'     and flag='B' and status =1 order by id ";
                   group_if = null;
                    group_if2 = null;
                    group_if = DB.GetDataGroup(ls_group2);
                    if (group_if.Table.Rows.Count > 0)
                    {
                        for (int y = 0; y < group_if.Table.Rows.Count; y++)
                        {

                            try
                            {
                                File.Copy("E:\\Plan15\\plan425-0415\\" + group_if.Table.Rows[y]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan425-0415-Big\\" + ls_fullname + "_" + (y + 1) + ".jpg", true);

                                string ls_up = "UPDATE plan425_pic SET oldname='" + ls_fullname + "_" + (y + 1) + ".jpg'  WHERE id='" + group_if.Table.Rows[y]["id"].ToString().Trim() + "' ";
                                DB.ExecuteSQL(ls_up);

                            }
                            catch { }
                        }
                    }
               

                    //主图-小图
                    string ls_group3 = "select id from plan425_pic where pid='" + ls_id + "'    and flag='S' and status =1  order by id ";
                    group_if2 = null;
                    group_if2 = DB.GetDataGroup(ls_group3);
                    if (group_if2.Table.Rows.Count > 0)
                    {
                        for (int y = 0; y < group_if2.Table.Rows.Count; y++)
                        {
                            try
                            {
                                File.Copy("E:\\Plan15\\plan425-0415\\" + group_if2.Table.Rows[y]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan425-0415-Small\\" + ls_fullname + ".jpg", true);

                                //File.Copy("E:\\Plan15\\plan425-0415\\" + group_if2.Table.Rows[y]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan425-0415-Big\\" + ls_fullname + ".jpg", true);

                                string ls_up = "UPDATE plan425_pic SET oldname='" + ls_fullname + ".jpg'  WHERE id='" + group_if2.Table.Rows[y]["id"].ToString().Trim() + "' ";
                                DB.ExecuteSQL(ls_up);

                            }
                            catch { }
                        }
                    }

                }


                //如果没有明细图，只有一个大图的情况

                //将主图第一个放到详细图里
                string ls_bignull = "select  (select id from plan425_pic where pid=plan425_b2.id and flag='S') as id, pname_ok as fullname from plan425_b2  where pic_group='' and status =2 and len(pic_s)>5  order by id ";
                //详细图有水印，用主图

                group_null = DB.GetDataGroup(ls_bignull);
                if (group_null.Table.Rows.Count > 0)
                {
                    for (int g = 0; g < group_null.Table.Rows.Count; g++)
                    {

                        try
                        {
                            File.Copy("E:\\Plan15\\plan425-0415\\" + group_null.Table.Rows[g]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan425-0415-Big\\" + group_null.Table.Rows[g]["fullname"].ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-").Replace(".", "-").Replace("#", "").Replace("*", "-").Trim().ToLower().Trim() + ".jpg", true);
                        }
                        catch
                        { }

                        //string ls_up = "UPDATE plan425_pic SET oldname='" + group_null.Table.Rows[g]["fullname"].ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-").Replace(".", "-").Trim().ToLower().Trim() + ".jpg'  WHERE id='" + group_null.Table.Rows[g]["id"].ToString().Trim() + "' ";
                        //DB.ExecuteSQL(ls_up);
                    }   
                }




                //特殊情况，没有主图，抓详细图第一个作为主图
                string ls_bad = "select  (select min(id) from plan425_pic where pid=plan425_b2.id and flag='B' and filesize>200) as id, pname_ok as fullname,pic_name from plan425_b2 where  status =2 and ( pic_s is null or pic_s ='') order by id ";
                group_bad = DB.GetDataGroup(ls_bad);
                if (group_bad.Table.Rows.Count > 0)
                {
                    for (int g = 0; g < group_bad.Table.Rows.Count; g++)
                    {
                        if (group_bad.Table.Rows[g]["id"].ToString().Trim().Length > 0)
                        {
                            File.Copy("E:\\Plan15\\plan425-0415\\" + group_bad.Table.Rows[g]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan425-0415-Small\\" + group_bad.Table.Rows[g]["fullname"].ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-").Replace(".", "-").Replace("#", "").Replace("*", "-").Trim().ToLower().Trim() + ".jpg", true);
                        }
                    }
                }



            }

            MessageBox.Show("ok!");

        }

        private void button105_Click(object sender, EventArgs e)
        {

            //round(convert(float,price)*0.4/6.5*2 ,2) 价格四舍五入

            DataGroup group = new DataGroup();
            DataGroup group_if = new DataGroup();
            string ls_group = "select id,fullname from Xuping_engs order by id ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    string ls_id = group.Table.Rows[x]["id"].ToString();
                    string ls_fullname = group.Table.Rows[x]["fullname"].ToString();

                    string ls_group2 = "select min(id) as id from Xuping_pic where pid='" + ls_id + "' order by id ";
                    group_if = null;
                    group_if = DB.GetDataGroup(ls_group2);
                    if (group_if.Table.Rows.Count > 0)
                    {
                        for (int y = 0; y < group_if.Table.Rows.Count; y++)
                        {

                            File.Copy("e:\\xuping\\" + group_if.Table.Rows[y]["id"].ToString().Trim() + ".jpg", "e:\\xuping-name-main\\" + ls_fullname +  ".jpg", true);

                        }

                    }
                }
            }
            MessageBox.Show("ok!");
        }

        private void button100_Click(object sender, EventArgs e)
        {
            string pp = l_html.Text.Trim();

            string xxx = GetTranslation(pp, "zh-CN", "en");

            MessageBox.Show(xxx);
        }

       

        public string GetTranslation(string tobeTranslate, string languge, string desLangauge)
        {
            string text = tobeTranslate;
            Google.API.Translate.TranslateClient ppp = new Google.API.Translate.TranslateClient("www");
            string xxx = ppp.Translate(text,languge, desLangauge);
            return xxx;
        }



      
        private void perm(string[] list, int i, int n)
        {
            int j;

            if (i == n)
            {
                string ls_OK = "";
                //listprint(list);//打印其中一种排列组合
                int x = list.Length;
                for (int z = 0; z < x; z++)
                {
                    ls_OK += list[z].ToString() + " ";
                }

                ls_OK = ls_OK.Trim();

                string ls_lsok = " insert into A_temp(pname) values('" + ls_OK + "');";
                DB.ExecuteSQL(ls_lsok);

                ls_OK = "";
                ls_lsok = "";

            }
            else
            {
                for (j = i; j <= n; j++)
                {
                    SWAP(ref list[i], ref list[j]);
                    perm(list, i + 1, n);
                    SWAP(ref list[i], ref list[j]);//数组一定要复原！！！！！
                }
            }
        }
        private void SWAP(ref string a, ref string b)
        {
            string c = a;
            a = b;
            b = c;
        }




        private void button156_Click(object sender, EventArgs e)
        {
         
        string ls_cc = @"  
        UPDATE plan425_b2 SET price_mark= pname_OK
        ";
        DB.ExecuteSQL(ls_cc);

        DataGroup group_main_class = new DataGroup();
        string ls_group_main_class = "select  pname_OK as remark from plan425_b2 where status !=0 group by pname_OK having count(*) >1";
        group_main_class = DB.GetDataGroup(ls_group_main_class);
        if (group_main_class.Table.Rows.Count > 0)
        {
            for (int x = 0; x < group_main_class.Table.Rows.Count; x++)
            {
                DataGroup group_price_mark = new DataGroup();
                group_price_mark = null;
                string ls_group_price_mark = "select id from plan425_b2 where status !=0 and pname_OK='" + group_main_class.Table.Rows[x]["remark"].ToString().Trim() + "'   order by id ";
                group_price_mark = DB.GetDataGroup(ls_group_price_mark);


                string ls_lsok = " delete A_temp;";
                DB.ExecuteSQL(ls_lsok);

                string lw = group_main_class.Table.Rows[x]["remark"].ToString().Trim();
                if (lw.IndexOf(" ") > -1)
                {
                    string[] dog_small = lw.Split(' ');
                    if (dog_small.Length > 5)
                    {
                        perm(dog_small, dog_small.Length - 3, dog_small.Length - 1);
                    }
                    else
                    {
                        perm(dog_small, 2, dog_small.Length - 1);
                    }
                }
                string lr = "Cheap|Designer|Fashion|Cheapest|Outlet|Design|Luxury|Buy|Sale|Deluxe|Discount|Top|Wholesale|New|Grade|Good";
                string[] dog_lr = lr.Split('|');
                foreach (string bb in dog_lr)
                {
                    string lw2 = group_main_class.Table.Rows[x]["remark"].ToString().Trim()+" "+bb;
                    string[] dog_small2 = lw2.Split(' ');
                    if (dog_small2.Length > 6)
                    {
                        perm(dog_small2, dog_small2.Length - 3, dog_small2.Length - 1);
                    }
                    else
                    {
                        perm(dog_small2, 3, dog_small2.Length - 1);
                    }
                }


                DataGroup group_temp = new DataGroup();
                group_temp = null;
                string ls_group_temp = "select pname from a_temp";
                group_temp = DB.GetDataGroup(ls_group_temp);

                if (group_temp.Table.Rows.Count < group_price_mark.Table.Rows.Count)
                {
                    MessageBox.Show("替换的全排列数量不够，需修改手动循环深度！");
                    return;
                }



                if (group_price_mark.Table.Rows.Count > 0)
                {
                    for (int k = 0; k < group_price_mark.Table.Rows.Count; k++)
                    {

                        string ls_up = "UPDATE plan425_b2 SET pname_OK='" + group_temp.Table.Rows[k]["pname"].ToString() + "'  WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; 
                        DB.ExecuteSQL(ls_up);

                      /*
                     if (k == 0) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Cheap'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                     if (k == 1) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Value'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                     if (k == 2) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' AAA'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                     if (k == 3) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Cheapest'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                     if (k == 4) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' designer'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                     if (k == 5) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Deluxe'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                     if (k == 6) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Fashion'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }


                     //string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Cheap '+'" + (k - 6) + "'    WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up);
                      */

                    }
                }

            }

        }
       

            MessageBox.Show("ok!");


        }



        private void button106_Click(object sender, EventArgs e)
        {
            string ls_cc = @"  
            UPDATE plan425_b2 SET price_mark= pname_OK
            ";
            DB.ExecuteSQL(ls_cc);

            DataGroup group_main_class = new DataGroup();
            string ls_group_main_class = "select  pname_OK as remark from plan425_b2 where status !=0 group by pname_OK having count(*) >1";
            //string ls_group_main_class = "select id,a,b,spinner from a_gp4 where flag=1 and status=1  ";
            group_main_class = DB.GetDataGroup(ls_group_main_class);
            if (group_main_class.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_main_class.Table.Rows.Count; x++)
                {
                    DataGroup group_price_mark = new DataGroup();
                    group_price_mark = null;
                    string ls_group_price_mark = "select id from plan425_b2 where pname_OK='" + group_main_class.Table.Rows[x]["remark"].ToString().Trim() + "'   order by id ";
                    group_price_mark = DB.GetDataGroup(ls_group_price_mark);

                    if (group_price_mark.Table.Rows.Count > 0)
                    {
                        for (int k = 0; k < group_price_mark.Table.Rows.Count; k++)
                        {
                            if (k == 0) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Cheap'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 1) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Value'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 2) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' AAA'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 3) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Cheapest'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 4) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' designer'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 5) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Deluxe'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 6) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Fashion'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                          
                           
                            if (k == 7) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Advanced'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 8) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Aggressive'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 9) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Alert'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 10) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Ambitious'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 11) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Amiable'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 12) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Aspiring'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 13) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Audacious'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 14) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Beautiful'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 15) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Buy'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 16) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Candid'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 17) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' canvas'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 18) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Capable'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 19) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Charitable'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 20) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Charm'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 21) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Able'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 22) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' classic'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 23) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Competent'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 24) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Confident'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 25) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Conscientious'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 26) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Constructive'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 27) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Contemplative'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 28) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Cooperative'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 29) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Cosmetic'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 30) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Cost'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 31) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Creative'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 32) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Dashing'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 33) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Adaptable'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 34) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Dependable'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 35) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Design'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 36) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Active'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 37) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Diplomatic'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 38) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Discount'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 39) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Dutiful'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 40) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Dynamic'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 41) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Earnest'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 42) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Efficient'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 43) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Energetic'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 44) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Enthusiastic'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 45) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Expressive'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 46) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Adroit'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 47) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Favorites'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 48) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Friendly'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 49) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Good'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 50) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Grade'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 51) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Hearty'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 52) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' High'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 53) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' hobo'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 54) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Humorous'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 55) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Impartial'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 56) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Independent'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 57) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Ingenious'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 58) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Initiative'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 59) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Intellective'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 60) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Inventive'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 61) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' jean'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 62) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Just'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 63) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' knockoff'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 64) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Learned'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 65) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Logical'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 66) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Love'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 67) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Luxury'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 68) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Modest'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 69) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' New'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 70) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Online'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 71) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Original'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 72) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Painstaking'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 73) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' patent'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 74) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Persevering'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 75) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Precise'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 76) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Purposeful'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 77) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Qualified'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 78) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Rational'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 79) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Reliable'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 80) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Responsible'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 81) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' sale'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 82) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Sincere'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 83) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Smart'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 84) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Specials'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 85) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Spirited'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 86) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Sporting'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 87) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Steady'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 88) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Strong'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 89) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Temperate'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 90) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Tireless'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 91) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Top'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 92) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' travel'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }

                            if (k == 93) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' china'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 94) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' whosale'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 95) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Unique'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 96) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Aesthetic'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 97) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Arts'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            if (k == 98) { string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Skill'   WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up); }
                            /* */

                            if (k > 98) //98 6
                            {

                                string ls_up = "UPDATE plan425_b2 SET pname_OK=price_mark+' Cheap '+'"+(k - 6)+"'    WHERE id='" + group_price_mark.Table.Rows[k]["id"].ToString() + "' "; DB.ExecuteSQL(ls_up);

                            }



                        }
                    }

                }

            }


            MessageBox.Show("ok!");


        }

        private void button107_Click(object sender, EventArgs e)
        {
            /*
             
             select *,'Cheap '+pname_ok+' Online' as title,pname_ok+', '+pname_ok+' For Sale' as kw,'Offer Cheap '+pname_ok+' at lowest price.Best quality  guranteed.' as de from plan425_b2
 
            
select product_code,pic_name,pname_OK,description,price-14,
dbo.fn_getdate(null,null),
dbo.fn_getdate(null,null),
dbo.RandData(3,20),
main_class,sub_class,1,'Cheap '+pname_ok+' Online' as title,pname_ok+', '+pname_ok+' For Sale' as kw,'Offer Cheap '+pname_ok+' at lowest price.Best quality  guranteed.' as de ,url_product
from plan425_b2

             
             
             
             */


            //  '/^[0-9a-zA-Z|-]+$/'


        }

        private void button108_Click(object sender, EventArgs e)
        {
            //desc_cn  colour  weight  material  规格  pattern   objects  originalarea
            //desc_cn2
            //select 'update `jv1_products_description` set products_description="'+replace(desc_en2,'"','''')+'"  where products_name="'+main_class+'-'+replace(sub_class,'/',' ')+' '+ename+'";' from jade_b3 where mid is not null

            //select 'update `jv1_products_description` set `bigimghtml`="'+replace(bigimghtml,'"','''')+'"  where products_name="'+pname+'";' as ok from JewelOra_b3 where status=4



            DataGroup group_field = new DataGroup();
            group_field = null;

            DataGroup group_if = new DataGroup();
            group_if = null;

            string ls_sql = "select id ,pname_ok as fullname from plan425_b2 where status in (2,3) order by id";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {

                    string ls_id = group_field.Table.Rows[i]["id"].ToString();
                    string ls_fullname = group_field.Table.Rows[i]["fullname"].ToString();

                    string ls_shtml = "";
                    string dog = "";




                    string ls_group_if = "select id,oldname as picname from plan425_pic where pid='" + ls_id + "' /**/ and flag='B' and oldname !='' order by id ";
                    group_if = null;
                    group_if = DB.GetDataGroup(ls_group_if);
                    if (group_if.Table.Rows.Count > 0)
                    {
                        for (int y = 0; y < group_if.Table.Rows.Count; y++)
                        {
                            if (y < 6)
                            {
                                dog += "<img src=\"http://images./" + group_if.Table.Rows[y]["picname"].ToString() + "\" alt=\"" + ls_fullname + " " + (y + 1) + "\" title=\"" + ls_fullname + " " + (y + 1) + "\" /><br/>";
                            }
                            else
                            {
                                if (y < 12)
                                {
                                    dog += "<img src=\"http://images.x.com/" + group_if.Table.Rows[y]["picname"].ToString() + "\"  /><br/>";
                                }
                            }
                        }
                    }




                    if (dog.Length > 1)
                    {
                        ls_shtml += "<div class=\"sidebox-header-left\"><h3 class=\"leftBoxHeading\">Product Images:</h3></div><div class=\"sideBoxContent\"   align=\"center\">"
                            + dog + "</div>";
                    }

                    string ls_up = "update plan425_b2 set bigimghtml ='" + ls_shtml.Replace('\'', '‘') + "'  where id='" + group_field.Table.Rows[i]["id"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up);
                }
            }


           
            /* */
            DataGroup group_null = new DataGroup();
            group_null = null;
            //如果没有明细图，只有一个大图的情况
            //将主图第一个放到详细图里
            string ls_bignull = "select id,pname_ok as pname,pic_name from plan425_b2 where status in (2,3)  and (pic_group='' or pic_group is null )  and pic_s is not null    order by id ";
            group_null = DB.GetDataGroup(ls_bignull);
            if (group_null.Table.Rows.Count > 0)
            {
                for (int g = 0; g < group_null.Table.Rows.Count; g++)
                {
                    string dog = "<img src=\"http://images.x.com/" + group_null.Table.Rows[g]["pic_name"].ToString() + "\" alt=\"" + group_null.Table.Rows[g]["pname"].ToString() + "\" title=\"" + group_null.Table.Rows[g]["pname"].ToString() + "\" /><br/>";
                    
                    //File.Copy("E:\\Plan15\\plan2-0330\\" + group_null.Table.Rows[g]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan2-0330-Big\\" + group_null.Table.Rows[g]["fullname"].ToString().Trim() + ".jpg", true);
                    string ls_shtml = "<div class=\"sidebox-header-left\"><h3 class=\"leftBoxHeading\">Product Images:</h3></div><div class=\"sideBoxContent\"   align=\"center\">"
                           + dog + "</div>";

                    string ls_up = "update plan425_b2 set bigimghtml ='" + ls_shtml.Replace('\'', '‘') + "'  where id='" + group_null.Table.Rows[g]["id"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up);

                }
            }


            

            MessageBox.Show("html  is  OK!");





        }

        private void button109_Click(object sender, EventArgs e)
        {
                DataGroup group_html = new DataGroup();
                group_html = null;

                string ls_sql = "SELECT  id,main_class,url,url_group,logo_group,sub_class_group FROM plan425_b0 WHERE STATUS =2  order by ID";
                group_html = DB.GetDataGroup(ls_sql);
                if (group_html.Table.Rows.Count > 0)
                {

                    for (int i = 0; i < group_html.Table.Rows.Count; i++)
                    {
                        string ls_url_group = group_html.Table.Rows[i]["url_group"].ToString().Trim();
                        string ls_logo_group = group_html.Table.Rows[i]["logo_group"].ToString().Trim();
                        string ls_sub_class_group = group_html.Table.Rows[i]["sub_class_group"].ToString().Trim();

                        if (ls_url_group.Length > 10)
                        {

                            string[] dog_small_1 = ls_url_group.Split('|');//子串
                            string[] dog_small_2 = ls_logo_group.Split('|');//子串
                            string[] dog_small_3 = ls_sub_class_group.Split('|');//子串

                            int xxx = 0;

                            foreach (string bb in dog_small_1)
                            {
                                
                                    string ls_1 = "insert into plan425_b1 (pid,url) values ( '" + group_html.Table.Rows[i]["id"].ToString().Trim() + "','"+bb+"' ); ";
                                    DB.ExecuteSQL(ls_1);


                                    string ls_2 = "update plan425_b1 set logo= '" + dog_small_2[xxx] + "' where pid = '" + group_html.Table.Rows[i]["id"].ToString().Trim() + "' and url='" + bb + "' ";
                                    DB.ExecuteSQL(ls_2);


                                    string ls_3 = "update plan425_b1 set sub_class= '" + dog_small_3[xxx] + "' where pid = '" + group_html.Table.Rows[i]["id"].ToString().Trim() + "' and url='" + bb + "' ";
                                    DB.ExecuteSQL(ls_3);


                                    xxx += 1;
                            }
                        }
                        else
                        {
                            string ls_up = "insert into plan425_b1 (pid,url) values ('" + group_html.Table.Rows[i]["ID"].ToString() + "','" + group_html.Table.Rows[i]["url"].ToString() + "')";
                            DB.ExecuteSQL(ls_up);
                        }

                       


                    }

                }

            MessageBox.Show("OK");
        }

        private void button110_Click(object sender, EventArgs e)
        {

                DataGroup group_html = new DataGroup();
                group_html = null;

                string ls_sql = "SELECT  ID,html as HTML FROM plan425_b1 order by ID";
                group_html = DB.GetDataGroup(ls_sql);
                if (group_html.Table.Rows.Count > 0)
                {

                    for (int i = 0; i < group_html.Table.Rows.Count; i++)
                    {
                        string ls_shtml = group_html.Table.Rows[i]["HTML"].ToString().Replace("\r\n", "").ToString();//替换换行符
                        //ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                        //ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”

                        //ls_Value = OperateStr_Adv(ls_shtml, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, group.Table.Rows[j]["CYCLE"].ToString(), group.Table.Rows[j]["STR_AREA"].ToString(), group.Table.Rows[j]["STR_AFT"].ToString(), group.Table.Rows[j]["STR_BEF"].ToString());
                        string LS_html = OperateStr_Adv_html(ls_shtml, "&nbsp;1/<b>", "</b>", "", 1, 0, 0, "0", "", "", "");

                        string ls_2 = "update plan425_b1 set page= '" + LS_html + "' where id = '" + group_html.Table.Rows[i]["id"].ToString().Trim() + "' ";
                        DB.ExecuteSQL(ls_2);


                    }
                }
                MessageBox.Show("OK");




        }

        private void button111_Click(object sender, EventArgs e)
        {
            DataGroup group_html = new DataGroup();
            group_html = null;
            string ls_sql = "select id,main_class,sub_class,url,html,status,logo,page from plan425_b1 WHERE STATUS =1  order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_main_class = group_html.Table.Rows[i]["main_class"].ToString().Trim();
                    string ls_sub_class = group_html.Table.Rows[i]["sub_class"].ToString().Trim();
                    string ls_url = group_html.Table.Rows[i]["url"].ToString().Trim();
                    string ls_html = group_html.Table.Rows[i]["html"].ToString().Trim();
                    string ls_logo = group_html.Table.Rows[i]["logo"].ToString().Trim();

                    try
                    {
                        int ls_page = int.Parse(group_html.Table.Rows[i]["page"].ToString().Trim());


                        for (int kkk = 1; kkk <= ls_page; kkk++)
                        {
                            if (kkk == 1)
                            {
                                string ls_up = "insert into plan425_b15 (main_class,sub_class,url,html,status,logo) " +
                                    " values ('" + ls_main_class + "','" + ls_sub_class + "','" + ls_url + "','" + ls_html + "','1','" + ls_logo + "')";
                                DB.ExecuteSQL(ls_up);
                            }
                            else
                            {
                                string ls_up = "insert into plan425_b15 (main_class,sub_class,url,html,status,logo) " +
                                   " values ('" + ls_main_class + "','" + ls_sub_class + "','" + ls_url.Replace(".html", "-" + kkk + ".html") + "',null,'0','" + ls_logo + "')";
                                DB.ExecuteSQL(ls_up);
                            }

                        }
                    }
                    catch
                    { }


                }

            }

            MessageBox.Show("OK");
        }

        private void button112_Click(object sender, EventArgs e)
        {
          /*  DataGroup group_field = new DataGroup();
            group_field = null;

            string ls_sql = "select id,pic_s from usa_nike_b3";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {
                    //FileCoppy(group_field.Table.Rows[i]["filename"].ToString().Trim(), "e:\\A1\\", group_field.Table.Rows[i]["id"].ToString().Trim() + ".JPG");

                    try
                    {
                        File.Copy("E:\\PHP-1.5.6\\htdocs\\150L302\\images\\" + group_field.Table.Rows[i]["pic_s"].ToString().Trim() ,
                                  "E:\\PHP-1.5.6\\htdocs\\150L302\\images\\123\\" + group_field.Table.Rows[i]["pic_s"].ToString().Trim() , true);
                    }
                    catch
                    { }

                }
            }


            MessageBox.Show("OK!");*/
        }

        private void button113_Click(object sender, EventArgs e)
        {
            /*
             update `jv1_categories` set `sort_order`='100';
             
             update  `jv1_categories_description` set `categories_name`=concat('2010 ',`categories_name`) where `categories_name` like '%Jordan%';
             
             update `jv1_categories` set `sort_order`='101' where 
                `categories_id` in 
                (
                select `categories_id` from `jv1_categories_description` where `categories_name` like '%Jordan%'
                );
            
             */

            //1.生成xid
            //2.以xid为id导出csv


            DataGroup group_xid = new DataGroup();
            string ls_group_xid = "select pname1,id from plan425_b2 where status in (2,3,5) order by main_class asc,id desc";
            group_xid = DB.GetDataGroup(ls_group_xid);
            if (group_xid.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_xid.Table.Rows.Count; x++)
                {
                    int xx = x + 1;

                    string ls_up = "UPDATE plan425_b2 SET xid='" + xx + "'  WHERE id='" + group_xid.Table.Rows[x]["id"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up);
                }

                string ls_exe = "UPDATE plan425_b2 SET product_code='Bags-'+cast(xid as nvarchar(10)) ";  //Jerseys
                DB.ExecuteSQL(ls_exe);

            }

            DataGroup group_super = new DataGroup();
//pname_OK as v_products_name_1,                    description+'<br/>'+desc2 as v_products_description_1,
            /*   string ls_super = @"
   select product_code as v_products_model,            pic_name as v_products_image,
   pname_OK as v_products_name_1,                     desc2 as v_products_description_1,
   '' as v_products_url_1, '' as v_specials_price, '' as v_specials_group_a_price,'' as  v_specials_group_b_price,'' as v_specials_group_c_price,'' as v_specials_group_d_price, '' as v_specials_last_modified,'' as v_specials_expires_date,
   price - 11 as v_products_price,
   '' as v_products_group_a_price,'' as v_products_group_b_price,'' as v_products_group_c_price,'' as v_products_group_d_price,
   '' as v_products_weight,
   dbo.fn_getdate('2011-05-22','2011-05-30') as v_last_modified,
   dbo.fn_getdate('2011-05-22','2011-05-30') as v_date_added,
   dbo.RandData(3000,3000) as v_products_quantity,
   '' as v_manufacturers_name,
   main_class as v_categories_name_1,sub_class as v_categories_name_2,
   '' as v_categories_name_3,'' as v_categories_name_4,'' as v_categories_name_5,'' as v_categories_name_6,'' as v_categories_name_7,'' as v_tax_class_title,
   1 as v_status,
   '' as v_EASYPOPULATE_CONFIG_CUSTOM_FIELDS,'' as v_metatags_products_name_status,
   1 as v_metatags_title_status,'' as v_metatags_model_status,'' as v_metatags_price_status,'' as v_metatags_title_tagline_status,
   ''+pname_ok+' Online'  v_metatags_title_1,
   ''+pname_ok+', '+pname_ok+' Manufacturer '+kw as v_metatags_keywords_1,
   ''+pname_ok+' Best quality '+main_class as v_metatags_description_1
   from plan425_b2 where status in (2,3) order by id desc
                   ";
   */
            /*  
string ls_super = @"
select product_code as v_products_model,            pic_name as v_products_image, pname_OK as v_products_name_1,                      desc2  as v_products_description_1,
'' as v_products_url_1, '' as v_specials_price, '' as v_specials_group_a_price,'' as  v_specials_group_b_price,'' as v_specials_group_c_price,'' as v_specials_group_d_price, '' as v_specials_last_modified,'' as v_specials_expires_date,
price as v_products_price,
'' as v_products_group_a_price,'' as v_products_group_b_price,'' as v_products_group_c_price,'' as v_products_group_d_price,
'' as v_products_weight,
dbo.fn_getdate('2012-02-15','2012-03-10') as v_last_modified,
dbo.fn_getdate('2012-02-15','2012-03-10') as v_date_added,
dbo.RandData(14,72) as v_products_quantity,
'' as v_manufacturers_name,
main_class as v_categories_name_1,sub_class as v_categories_name_2,
'' as v_categories_name_3,'' as v_categories_name_4,'' as v_categories_name_5,'' as v_categories_name_6,'' as v_categories_name_7,'' as v_tax_class_title,
1 as v_status,
'' as v_EASYPOPULATE_CONFIG_CUSTOM_FIELDS,'' as v_metatags_products_name_status,
1 as v_metatags_title_status,'' as v_metatags_model_status,'' as v_metatags_price_status,'' as v_metatags_title_tagline_status,
''+pname_ok+'' as v_metatags_title_1,
''+pname_ok+', Buy '+pname_ok as  v_metatags_keywords_1,
''+pname_ok+' Free Shipping! '+replace(main_class,' Catalogue','') +' Manufacturer Best Price! '+pname_ok+' Five-Star Quality!'  as v_metatags_description_1
from plan425_b2 where status in (2,3,5) order by main_class asc,id desc  
";
*/
            string ls_super = @"
select product_code as v_products_model,            pic_name as v_products_image, pname_OK as v_products_name_1,                      desc2  as v_products_description_1,
'' as v_products_url_1, '' as v_specials_price, '' as v_specials_group_a_price,'' as  v_specials_group_b_price,'' as v_specials_group_c_price,'' as v_specials_group_d_price, '' as v_specials_last_modified,'' as v_specials_expires_date,
price as v_products_price,
'' as v_products_group_a_price,'' as v_products_group_b_price,'' as v_products_group_c_price,'' as v_products_group_d_price,
'' as v_products_weight,
dbo.fn_getdate('2012-04-7','2012-04-13') as v_last_modified,
dbo.fn_getdate('2012-04-7','2012-04-13') as v_date_added,
dbo.RandData(14,72) as v_products_quantity,
'' as v_manufacturers_name,
main_class as v_categories_name_1,sub_class as v_categories_name_2,
'' as v_categories_name_3,'' as v_categories_name_4,'' as v_categories_name_5,'' as v_categories_name_6,'' as v_categories_name_7,'' as v_tax_class_title,
1 as v_status,
'' as v_EASYPOPULATE_CONFIG_CUSTOM_FIELDS,'' as v_metatags_products_name_status,
1 as v_metatags_title_status,'' as v_metatags_model_status,'' as v_metatags_price_status,'' as v_metatags_title_tagline_status,
''+pname_ok+'' as v_metatags_title_1,
''+pname_ok+', Buy '+pname_ok as  v_metatags_keywords_1,
'' as v_metatags_description_1
from plan425_b2 where status in (2,3,5) order by main_class asc,id desc  
";
            string ls_name = "E:\\PHP-1.5.6\\vhosts\\www.x.com\\Pic\\plan-pd.csv";

                using (FileStream fs = File.Open(@ls_name, FileMode.Create))
                {
                    byte[] b = System.Text.Encoding.Default.GetBytes("");
                    fs.Write(b, 0, b.Length);
                    fs.Close();
                }


            group_super = DB.GetDataGroup(ls_super);
            if (group_super.Table.Rows.Count > 0)
            {
                //v_products_model,v_products_image,v_products_name_1,v_products_description_1,v_products_url_1,v_specials_price,v_specials_group_a_price,v_specials_group_b_price,v_specials_group_c_price,v_specials_group_d_price,v_specials_last_modified,v_specials_expires_date,v_products_price,v_products_group_a_price,v_products_group_b_price,v_products_group_c_price,v_products_group_d_price,v_products_weight,v_last_modified,v_date_added,v_products_quantity,v_manufacturers_name,v_categories_name_1,v_categories_name_2,v_categories_name_3,v_categories_name_4,v_categories_name_5,v_categories_name_6,v_categories_name_7,v_tax_class_title,v_status,v_EASYPOPULATE_CONFIG_CUSTOM_FIELDS,v_metatags_products_name_status,v_metatags_title_status,v_metatags_model_status,v_metatags_price_status,v_metatags_title_tagline_status,v_metatags_title_1,v_metatags_keywords_1,v_metatags_description_1
                

                string ls_ddd = "v_products_model,v_products_image,v_products_name_1,v_products_description_1,v_products_url_1,v_specials_price,v_specials_group_a_price,v_specials_group_b_price,v_specials_group_c_price,v_specials_group_d_price,v_specials_last_modified,v_specials_expires_date,v_products_price,v_products_group_a_price,v_products_group_b_price,v_products_group_c_price,v_products_group_d_price,v_products_weight,v_last_modified,v_date_added,v_products_quantity,v_manufacturers_name,v_categories_name_1,v_categories_name_2,v_categories_name_3,v_categories_name_4,v_categories_name_5,v_categories_name_6,v_categories_name_7,v_tax_class_title,v_status,v_EASYPOPULATE_CONFIG_CUSTOM_FIELDS,v_metatags_products_name_status,v_metatags_title_status,v_metatags_model_status,v_metatags_price_status,v_metatags_title_tagline_status,v_metatags_title_1,v_metatags_keywords_1,v_metatags_description_1\n";

                for (int x = 0; x < group_super.Table.Rows.Count; x++)
                {

                    ls_ddd += group_super.Table.Rows[x]["v_products_model"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_products_image"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_products_name_1"].ToString() + ",";

                    if (group_super.Table.Rows[x]["v_products_description_1"].ToString().Length > 10)
                    {
                        ls_ddd += "\"" + group_super.Table.Rows[x]["v_products_description_1"].ToString().Replace("\"","\"\"") + "\",";
                    }
                    else
                    {
                        ls_ddd += group_super.Table.Rows[x]["v_products_name_1"].ToString() + ",";
                    }
                    ls_ddd += group_super.Table.Rows[x]["v_products_url_1"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_specials_price"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_specials_group_a_price"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_specials_group_b_price"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_specials_group_c_price"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_specials_group_d_price"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_specials_last_modified"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_specials_expires_date"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_products_price"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_products_group_a_price"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_products_group_b_price"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_products_group_c_price"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_products_group_d_price"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_products_weight"].ToString() + ",";
                    
                    //随机日期
                    ls_ddd += group_super.Table.Rows[x]["v_last_modified"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_date_added"].ToString() + ",";


                    //递加日期，每天自动发布10个产品

                    //int yyyyyy = x / 200 + 1;

                    //ls_ddd += DateTime.Now.AddDays(yyyyyy).ToString("yyyy-MM-dd") + ",";
                    //ls_ddd += DateTime.Now.AddDays(yyyyyy).ToString("yyyy-MM-dd") + ",";

                    
                    ls_ddd += group_super.Table.Rows[x]["v_products_quantity"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_manufacturers_name"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_categories_name_1"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_categories_name_2"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_categories_name_3"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_categories_name_4"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_categories_name_5"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_categories_name_6"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_categories_name_7"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_tax_class_title"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_status"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_EASYPOPULATE_CONFIG_CUSTOM_FIELDS"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_metatags_products_name_status"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_metatags_title_status"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_metatags_model_status"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_metatags_price_status"].ToString() + ",";
                    ls_ddd += group_super.Table.Rows[x]["v_metatags_title_tagline_status"].ToString() + ",";
                    ls_ddd += "\"" + group_super.Table.Rows[x]["v_metatags_title_1"].ToString() + "\",";
                    ls_ddd += "\"" + group_super.Table.Rows[x]["v_metatags_keywords_1"].ToString() + "\",";
                    ls_ddd += "\"" + group_super.Table.Rows[x]["v_metatags_description_1"].ToString().Trim() + "\"\n";




                    using (FileStream fs = File.Open(@ls_name, FileMode.Append))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_ddd);
                        fs.Write(b, 0, b.Length);

                        fs.Close();

                        ls_ddd = "";
                    }


                }
            }


            MessageBox.Show("恭喜！导出了要仔细目测，注意关键字，网址，产品描述！    这里要修改手动用SQL调整目录！甚至去掉一些不好的分类！切记！");


        }

        private void button114_Click(object sender, EventArgs e)
        {
            //E:\WebHosting\PHP\vhosts\images.vmuch.com

            //File.Copy(OrignFile, NewPath + NewFile, true);



            DeleteFolderxx("E:\\PHP-1.5.6\\vhosts\\images.vmuch.com\\"); 
            DirectoryInfo d = Directory.CreateDirectory("E:\\PHP-1.5.6\\vhosts\\images.vmuch.com\\"); 




            MessageBox.Show("Clear!");


        }


        public void DeleteFolderxx(string dir)
        {
            if (Directory.Exists(dir)) //如果存在这个文件夹删除之
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                        File.Delete(d); //直接删除其中的文件
                }
                Directory.Delete(dir); //删除已空文件夹
            }
        }

        public static void CopyDirxx(string srcPath, string aimPath)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加之
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                // 判断目标目录是否存在如果不存在则新建之
                if (!Directory.Exists(aimPath)) Directory.CreateDirectory(aimPath);
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                // 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                // string[] fileList = Directory.GetFiles(srcPath);
                string[] fileList = Directory.GetFileSystemEntries(srcPath);
                // 遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                    //if (Directory.Exists(file))
                        //CopyDir(file, aimPath + Path.GetFileName(file));
                    // 否则直接Copy文件
                    //else
                        File.Copy(file, aimPath + Path.GetFileName(file), true);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        } 


        private void button115_Click(object sender, EventArgs e)
        {
            //E:\Plan15\plan425-0415-Big
            

            string xxx="E:\\Plan15\\"+textBox24.Text+"-Big\\";

            CopyDirxx(xxx, "E:\\PHP-1.5.6\\vhosts\\images.vmuch.com\\");

            MessageBox.Show("Copy Ok!");
        }

        private void button116_Click(object sender, EventArgs e)
        {
            //clear
            string ls_name = "E:\\PHP-1.5.6\\vhosts\\www.x.com\\plan-bigimg.sql";
            using (FileStream fs = File.Open(@ls_name, FileMode.Create))
            {
                byte[] b = System.Text.Encoding.Default.GetBytes("");
                fs.Write(b, 0, b.Length);
                fs.Close();
            }



            DataGroup group_super = new DataGroup();
            
            string ls_super = " select 'update `jv3_products_description` set `bigimghtml`=\"'+replace(bigimghtml,'\"','''')+'\"  where products_name=\"'+pname_OK+'\";' as ok from plan425_b2 where status in (2,3,5) ";
            
            group_super = DB.GetDataGroup(ls_super);
            if (group_super.Table.Rows.Count > 0)
            {
                //v_products_model,v_products_image,v_products_name_1,v_products_description_1,v_products_url_1,v_specials_price,v_specials_group_a_price,v_specials_group_b_price,v_specials_group_c_price,v_specials_group_d_price,v_specials_last_modified,v_specials_expires_date,v_products_price,v_products_group_a_price,v_products_group_b_price,v_products_group_c_price,v_products_group_d_price,v_products_weight,v_last_modified,v_date_added,v_products_quantity,v_manufacturers_name,v_categories_name_1,v_categories_name_2,v_categories_name_3,v_categories_name_4,v_categories_name_5,v_categories_name_6,v_categories_name_7,v_tax_class_title,v_status,v_EASYPOPULATE_CONFIG_CUSTOM_FIELDS,v_metatags_products_name_status,v_metatags_title_status,v_metatags_model_status,v_metatags_price_status,v_metatags_title_tagline_status,v_metatags_title_1,v_metatags_keywords_1,v_metatags_description_1
                

                string ls_ddd = "";

                for (int x = 0; x < group_super.Table.Rows.Count; x++)
                {

                    ls_ddd += group_super.Table.Rows[x]["ok"].ToString() + "\n";

                    using (FileStream fs = File.Open(@ls_name, FileMode.Append))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_ddd);
                        fs.Write(b, 0, b.Length);

                        fs.Close();

                        ls_ddd = "";
                    }


                }
            }



            MessageBox.Show("生产大图显示SQL 成功了！ 成功啦！");
        }

        private void button117_Click(object sender, EventArgs e)
        {

            string xxx1 = "E:\\Plan15\\" + textBox24.Text + "-Big\\";
            string xxx2 = "E:\\Plan15\\" + textBox24.Text + "-Small\\";
            string xxx3 = "E:\\Plan15\\" + textBox24.Text + "-Big-500\\";
            string xxx4 = "E:\\Plan15\\" + textBox24.Text + "-Small-250\\";


            DeleteFolderxx(xxx1);
            DirectoryInfo d1 = Directory.CreateDirectory(xxx1);

            DeleteFolderxx(xxx2);
            DirectoryInfo d2 = Directory.CreateDirectory(xxx2);

            //DeleteFolderxx(xxx3);
            //DirectoryInfo d3 = Directory.CreateDirectory(xxx3);

            //DeleteFolderxx(xxx4);
            //DirectoryInfo d4 = Directory.CreateDirectory(xxx4);


            MessageBox.Show("数据全部删除了，快点新生成！！！！   快～～～～～～～～～～～ ");

        }

        private void button118_Click(object sender, EventArgs e)
        {
            /*
             批量尺寸
             insert into  `jv1_products_attributes` (products_id ,	options_id ,	options_values_id,price_prefix ,product_attribute_is_free,products_attributes_weight_prefix,attributes_image ,attributes_qty_prices,attributes_qty_prices_onetime)  
select a.products_id,1,8,'+',1,'+','','','' from `jv1_products` a,`jv1_categories` b,`jv1_products_to_categories` c
where a.products_id=c.products_id and b.categories_id=c.categories_id 
and b.categories_id=17*/
            DataGroup group_super = new DataGroup();

            string ls_super = " select 'insert into  `jv1_products_options_values` (products_options_values_id,language_id,products_options_values_name,products_options_values_sort_order) values ('+cast(id as nvarchar(10))+',1,\"'+name+'\",0);' as ok from plan425_ATTRIB ";

            group_super = DB.GetDataGroup(ls_super);
            if (group_super.Table.Rows.Count > 0)
            {
                //v_products_model,v_products_image,v_products_name_1,v_products_description_1,v_products_url_1,v_specials_price,v_specials_group_a_price,v_specials_group_b_price,v_specials_group_c_price,v_specials_group_d_price,v_specials_last_modified,v_specials_expires_date,v_products_price,v_products_group_a_price,v_products_group_b_price,v_products_group_c_price,v_products_group_d_price,v_products_weight,v_last_modified,v_date_added,v_products_quantity,v_manufacturers_name,v_categories_name_1,v_categories_name_2,v_categories_name_3,v_categories_name_4,v_categories_name_5,v_categories_name_6,v_categories_name_7,v_tax_class_title,v_status,v_EASYPOPULATE_CONFIG_CUSTOM_FIELDS,v_metatags_products_name_status,v_metatags_title_status,v_metatags_model_status,v_metatags_price_status,v_metatags_title_tagline_status,v_metatags_title_1,v_metatags_keywords_1,v_metatags_description_1
                string ls_name = "E:\\PHP-1.5.6\\vhosts\\www.x.com\\plan-Attrib.sql";

                string ls_ddd = "";

                for (int x = 0; x < group_super.Table.Rows.Count; x++)
                {

                    ls_ddd += group_super.Table.Rows[x]["ok"].ToString() + "\n";

                    using (FileStream fs = File.Open(@ls_name, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_ddd);
                        fs.Write(b, 0, b.Length);

                        fs.Close();

                        ls_ddd = "";
                    }


                }
            }








            
            ls_super = " select 'insert into  `jv1_products_attributes` (products_attributes_id ,	products_id ,	options_id ,	options_values_id,price_prefix ,product_attribute_is_free,products_attributes_weight_prefix,attributes_image ,attributes_qty_prices,attributes_qty_prices_onetime)  values ('+cast(id as nvarchar(10))+','+cast(proid as nvarchar(10))+',1,'+cast(attribid as nvarchar(10))+''  +');' as ok from plan425_ATTRIB_PRO ";

            group_super = DB.GetDataGroup(ls_super);
            if (group_super.Table.Rows.Count > 0)
            {
                //v_products_model,v_products_image,v_products_name_1,v_products_description_1,v_products_url_1,v_specials_price,v_specials_group_a_price,v_specials_group_b_price,v_specials_group_c_price,v_specials_group_d_price,v_specials_last_modified,v_specials_expires_date,v_products_price,v_products_group_a_price,v_products_group_b_price,v_products_group_c_price,v_products_group_d_price,v_products_weight,v_last_modified,v_date_added,v_products_quantity,v_manufacturers_name,v_categories_name_1,v_categories_name_2,v_categories_name_3,v_categories_name_4,v_categories_name_5,v_categories_name_6,v_categories_name_7,v_tax_class_title,v_status,v_EASYPOPULATE_CONFIG_CUSTOM_FIELDS,v_metatags_products_name_status,v_metatags_title_status,v_metatags_model_status,v_metatags_price_status,v_metatags_title_tagline_status,v_metatags_title_1,v_metatags_keywords_1,v_metatags_description_1
                string ls_name = "E:\\PHP-1.5.6\\vhosts\\www.x.com\\plan425--Attrib2--" + DateTime.Now.ToString().Replace(":", "").Replace(" ", "-") + ".sql";

                string ls_ddd = "";

                for (int x = 0; x < group_super.Table.Rows.Count; x++)
                {

                    ls_ddd += group_super.Table.Rows[x]["ok"].ToString().Replace(");", ",'+',1,'+','','','');") + "\n";

                    using (FileStream fs = File.Open(@ls_name, FileMode.Append))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_ddd);
                        fs.Write(b, 0, b.Length);

                        fs.Close();

                        ls_ddd = "";
                    }


                }
            }




            MessageBox.Show("生成属性SQL 成功了！ 恭喜！恭喜！");
        }

        private void button119_Click(object sender, EventArgs e)
        {
            string ls_ttt = @"
drop table plan425_ATTRIB;
CREATE TABLE [dbo].[plan425_ATTRIB](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[NAME] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[STATUS] [nvarchar](1) COLLATE Chinese_PRC_CI_AS NULL,
	[ENAME] [nvarchar](100) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [PK_plan425_ATTRIB] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];

drop table [plan425_ATTRIB_PRO]
CREATE TABLE [dbo].[plan425_ATTRIB_PRO](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ProID] [bigint] NOT NULL,
	[AttribID] [bigint] NOT NULL,
	[STATUS] [nvarchar](1) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [PK_plan425_ATTRIB_PRO] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
";

            DB.ExecuteSQL(ls_ttt);

            MessageBox.Show("建立属性表 成功了！");



        }

        private void button120_Click(object sender, EventArgs e)
        {
            string ls_up1 = @"  
update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ') where status in (2,5);

update plan425_b2 set pname_OK=rtrim(ltrim(pname_OK)) where status in (2,5);



";

            string ls_up2 = @"  

update plan425_b2 set pname1=pname_OK where status in (2,5);


";

            string ls_up3 = @"  



";


            string ls_up4 = @"  



";


            string ls_up5 = @"  

";


DB.ExecuteSQL(ls_up1);



            DataGroup group = new DataGroup();
            string ls_group_main_class = "select  id,pname_ok as remark from plan425_b2 where status in (2,5) ";
            group = DB.GetDataGroup(ls_group_main_class);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {

                    string attrib = group.Table.Rows[x]["remark"].ToString().Trim();

                    if (attrib.IndexOf(" ") > -1)
                    {
                        string ls_do = "";

                        string[] dog_small = attrib.Split(' ');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = FormatString(bb.Trim());

                           

                            if (OK_bb.Length > 2)
                            {
                                string pp = " " + OK_bb + " ";
                                if (ls_do.IndexOf(pp) > -1)
                                {

                                }
                                else
                                {
                                    ls_do += " " + OK_bb + " ";
                                }
                            }
                            
                        }
                       string ls_up = "update plan425_b2 set pname_OK='" + ls_do.Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'";
                        DB.ExecuteSQL(ls_up);
                    }
                }
            }


DB.ExecuteSQL(ls_up2);
DB.ExecuteSQL(ls_up3);
DB.ExecuteSQL(ls_up4);
DB.ExecuteSQL(ls_up5);


MessageBox.Show("恭喜！名称处理完成，目测一下，十万火急很重要！");




        }



        private string FormatString(string v)
        {
            //v = v.ToLower();

            if (string.IsNullOrEmpty(v)) return v;
            return v.Substring(0, 1).ToUpper() + (v.Length > 1 ? v.Substring(1).ToLower() : "");
        }

        private void button122_Click(object sender, EventArgs e)
        {

            DataGroup group = new DataGroup();
            string ls_group_main_class = "select  id,product_name , pname_OK,main_class,sub_class from plan425_B2 ";
            group = DB.GetDataGroup(ls_group_main_class);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {

                    string bbbb = group.Table.Rows[x]["product_name"].ToString().Replace("\n", " ").Trim();
                    string cccc = group.Table.Rows[x]["pname_OK"].ToString().Replace("\n", " ").Trim();
                    string dddd = group.Table.Rows[x]["main_class"].ToString().Replace("\n", " ").Trim();
                    string eeee = group.Table.Rows[x]["sub_class"].ToString().Replace("\n", " ").Trim();

                    string ls_do = "";
                    string ls_do2 = "";
                    string ls_do3 = "";
                    string ls_do4 = "";

                    if (bbbb.IndexOf(" ") > -1)
                    {
                        string[] dog_small = bbbb.Split(' ');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = FormatString(bb.Trim());

                            if (OK_bb.Length > 1)
                            {
                                if (OK_bb.Length == 2)
                                {
                                    OK_bb = OK_bb.ToUpper();
                                }
                                string pp = " " + OK_bb + " ";
                                if (ls_do4.IndexOf(pp) > -1)
                                {

                                }
                                else
                                {
                                    ls_do4 += " " + OK_bb + " ";
                                }
                            }
                        }

                    }

                    if (cccc.IndexOf(" ") > -1)
                    {
                        string[] dog_small = cccc.Split(' ');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = FormatString(bb.Trim());

                            if (OK_bb.Length > 1)
                            {
                                if (OK_bb.Length == 2)
                                {
                                    OK_bb = OK_bb.ToUpper();
                                }

                                //try
                                //{
                                //    int.Parse(OK_bb);
                                //    OK_bb = "";
                                //}
                                //catch { }

                                string pp = " " + OK_bb + " ";
                                if (ls_do.IndexOf(pp) > -1)
                                {

                                }
                                else
                                {
                                    ls_do += " " + OK_bb + " ";
                                }
                            }
                        }

                    }

                    if (dddd.IndexOf(" ") > -1)
                    {
                        string[] dog_small2 = dddd.Split(' ');//子串
                        foreach (string bb in dog_small2)
                        {
                            string OK_bb = FormatString(bb.Trim());

                            if (OK_bb.Length > 1)
                            {
                                if (OK_bb.Length == 2)
                                {
                                    OK_bb = OK_bb.ToUpper();
                                }

                                //string pp = " " + OK_bb + " ";
                                //if (ls_do2.IndexOf(pp) > -1)
                                //{

                                //}
                                //else
                                //{
                                    ls_do2 += " " + OK_bb + " ";
                                //}
                            }
                        }
                    }
                    else
                    {
                        ls_do2 = FormatString(dddd);
                    
                    }


                    if (eeee.IndexOf(" ") > -1)
                    {
                        string[] dog_small3 = eeee.Split(' ');//子串
                        foreach (string bb in dog_small3)
                        {
                            string OK_bb = FormatString(bb.Trim());

                            if (OK_bb.Length > 1)
                            {
                                if (OK_bb.Length == 2)
                                {
                                    OK_bb = OK_bb.ToUpper();
                                }

                                //string pp = " " + OK_bb + " ";
                                //if (ls_do3.IndexOf(pp) > -1)
                                //{

                                //}
                                //else
                                //{
                                    ls_do3 += " " + OK_bb + " ";
                                //}
                            }
                        }
                    }
                    else
                    {
                        ls_do3 = FormatString(eeee);
                    
                    }

                    string ls_up = "update plan425_B2 set product_name='" + ls_do4.Replace("  ", " ").Trim() + "',pname_OK='" + ls_do.Replace("  ", " ").Trim() + "',main_class='" + ls_do2.Replace("  ", " ").Trim() + "',sub_class='" + ls_do3.Replace("  ", " ").Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'";
                DB.ExecuteSQL(ls_up);

                    
                }
            }



            MessageBox.Show("ok!");
        }

        private void button123_Click(object sender, EventArgs e)
        {

            DataGroup group_class = new DataGroup();
            string ls_group_main_class = "select distinct kw2 from sitemap";
            group_class = DB.GetDataGroup(ls_group_main_class);
            if (group_class.Table.Rows.Count > 0)
            {

                string dt = string.Format("{0:R}", DateTime.Now);


                string dog = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
                dog += "<rss version=\"2.0\">\n";
                dog += "    <channel>\n";
                dog += "        <title>wholesale nfl jerseys</title>\n";
                dog += "        <copyright>www.wholesalenfljerseysshop.com</copyright>\n";
                dog += "        <lastBuildDate>" + dt + "</lastBuildDate>\n";
                dog += "        <docs>http://www.wholesalenfljerseysshop.com/rss/wholesale-nfl-jerseysshop.xml</docs>\n";


                string monkey = string.Empty;


                for (int i = 0; i < group_class.Table.Rows.Count; i++)
                {

                    string ls_do = group_class.Table.Rows[i]["kw2"].ToString().Trim();


                    //----------------------------------------------------------------------------------------------------------------------------
                    DataGroup group = new DataGroup();
                    string ls_sql = "select id,url,product_name,'Cheap '+ kw +', 2011 New Arrival'+ kw +' On Sale'  as kw from sitemap where kw2='" + ls_do + "'";
                    group = DB.GetDataGroup(ls_sql);
                    if (group.Table.Rows.Count > 0)
                    {


                        dog += "        <item>\n";
                        dog += "            <title>" + HttpUtility.HtmlEncode(ls_do) + "</title>\n";
                        dog += "            <description>" + HttpUtility.HtmlEncode(ls_do) + "</description>\n";
                        dog += "            <link>http://www.wholesalenfljerseysshop.com/rss/" + HttpUtility.HtmlEncode(ls_do.Replace(" ", "-")) + ".xml</link>\n";
                        dog += "            <guid>http://www.wholesalenfljerseysshop.com/rss/" + HttpUtility.HtmlEncode(ls_do.Replace(" ", "-")) + ".xml</guid>\n";
                        dog += "            <pubDate>" + dt + "</pubDate>\n";
                        dog += "        </item>\n\n";

                        monkey += "http://www.wholesalenfljerseysshop.com/rss/" + HttpUtility.HtmlEncode(ls_do.Replace(" ", "-")) + ".xml\n";


                        string pig = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
                        pig += "<rss version=\"2.0\">\n";
                        pig += "    <channel>\n";
                        pig += "        <title>" + HttpUtility.HtmlEncode(ls_do) + "</title>\n";
                        pig += "        <copyright>www.wholesalenfljerseysshop.com</copyright>\n";
                        pig += "        <lastBuildDate>" + dt + "</lastBuildDate>\n";
                        pig += "        <docs>http://www.wholesalenfljerseysshop.com/rss/" + HttpUtility.HtmlEncode(ls_do.Replace(" ", "-")) + ".xml</docs>\n";


                        for (int x = 0; x < group.Table.Rows.Count; x++)
                        {
                            //pig = HttpUtility.HtmlEncode(pig);

                            string url = HttpUtility.HtmlEncode(group.Table.Rows[x]["url"].ToString().Trim());
                            string product_name = HttpUtility.HtmlEncode(group.Table.Rows[x]["product_name"].ToString().Trim());
                            string kw = HttpUtility.HtmlEncode(group.Table.Rows[x]["kw"].ToString().Trim());

                            pig += "        <item>\n";
                            pig += "            <title>" + product_name + "</title>\n";
                            pig += "            <description>" + kw + "</description>\n";
                            pig += "            <link>" + url + "</link>\n";
                            pig += "            <guid>" + url + "</guid>\n";
                            pig += "            <pubDate>" + dt + "</pubDate>\n";
                            pig += "        </item>\n\n";

                        }

                        pig += "    </channel>\n</rss>";

                        string ls_name = "c:\\Rss\\" + HttpUtility.HtmlEncode(ls_do.Replace(" ","-")) + ".xml";

                        using (FileStream fs = File.Open(@ls_name, FileMode.Append))
                        {
                            byte[] b = System.Text.Encoding.Default.GetBytes(pig);
                            fs.Write(b, 0, b.Length);

                            fs.Close();

                            pig = "";
                        }
                    }
                    //----------------------------------------------------------------------------------------------------------------------------


                }

                    dog += "    </channel>\n</rss>";

                    string ls_dog = "c:\\Rss\\wholesale-nfl-jerseysshop.xml";

                    using (FileStream fs = File.Open(@ls_dog, FileMode.Append))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(dog);
                        fs.Write(b, 0, b.Length);

                        fs.Close();

                        dog = "";
                    }


                    string ls_monkey = "c:\\Rss\\rss.txt";
                    using (FileStream fs = File.Open(ls_monkey, FileMode.Append))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(monkey);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        monkey = "";
                    }

                

            }

            MessageBox.Show("ok!");
        }


        private void button125_Click(object sender, EventArgs e)
        {
            DataGroup group_html = new DataGroup();
            group_html = null;

            string ls_sql = "SELECT  id,main_class,url,sub_class,nb FROM plan425_b0 WHERE nb >0  order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_main_class = group_html.Table.Rows[i]["main_class"].ToString().Trim();
                    string ls_sub_class = group_html.Table.Rows[i]["sub_class"].ToString().Trim();
                    string ls_url = group_html.Table.Rows[i]["url"].ToString().Trim();
                    string ls_nb = group_html.Table.Rows[i]["nb"].ToString().Trim();

                    for (int ppp = 1; ppp <= int.Parse(ls_nb); ppp++)
                    {

                        if (ppp == 1 )
                        {
                            string ls_1 = "insert into plan425_b1 (main_class,sub_class,url,status) values ( '" + ls_main_class + "','" + ls_sub_class + "','" + ls_url + "',1); ";
                            DB.ExecuteSQL(ls_1);
                        }
                        else
                        {
                            string ls_1 = "insert into plan425_b1 (main_class,sub_class,url,status) values ( '" + ls_main_class + "','" + ls_sub_class + "','" + ls_url.Replace(".html", "") + "-" + ppp + ".html"  +"',1); ";
                            DB.ExecuteSQL(ls_1);
                        }

                    }

                }

            }

            MessageBox.Show("OK");
        }

        private void button126_Click(object sender, EventArgs e)
        {

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT ID,description as remark FROM plan425_b2   order by ID";//WHERE STATUS =1   where status=3 
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {

                    string ls_shtml = group_html.Table.Rows[i]["remark"].ToString().Replace("\n", "");


                    string ppp = OperateStr_Adv_html(ls_shtml.Replace("'", "\"")
                                       , "Materials:", "<br>", ""
                                       , 1
                                       , 0, 0
                                       , "0"
                                       , ""
                                       , "", "");


                        string ls_up2 = "update plan425_b2 set desc2='" + ppp + "' , status=2 where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                        DB.ExecuteSQL(ls_up2);

                   
                }

            }


            MessageBox.Show("OK!!");



           
        }

        private void button127_Click(object sender, EventArgs e)
        {
            /**/
            //先生成二级目录
            DataGroup group_dir = new DataGroup();
            string ls_group_dir = "select distinct sub_class as dir from plan425_b2 where status !=0  ";
            group_dir = DB.GetDataGroup(ls_group_dir);
            if (group_dir.Table.Rows.Count > 0)
            {

                for (int kkk = 0; kkk < group_dir.Table.Rows.Count; kkk++)
                {
                    string xxx1 = "E:\\Plan15\\plan425-0415-Sub\\" + group_dir.Table.Rows[kkk]["dir"].ToString().Trim() + "";
                    DirectoryInfo d1 = Directory.CreateDirectory(xxx1);
                }

            }


            


            DataGroup group = new DataGroup();
            DataGroup group_if = new DataGroup();
            DataGroup group_if2 = new DataGroup();
            DataGroup group_null = new DataGroup();
            DataGroup group_bad = new DataGroup();
            string ls_group = "select id,pname_ok as fullname from plan425_b2 where status !=0 order by id ";
            group = DB.GetDataGroup(ls_group);
            if (group.Table.Rows.Count > 0)
            {

                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    string ls_id = group.Table.Rows[x]["id"].ToString();
                    string ls_fullname = group.Table.Rows[x]["fullname"].ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-").Replace(".", "-").Replace("#", "").Replace("*", "-").Trim().ToLower();

                    //详细的大图
                    string ls_group2 = "select id, (select sub_class from plan425_b2 where id=plan425_pic.pid)  as sub_class from plan425_pic where pid='" + ls_id + "'     and flag='B' and status !=0 order by id ";
                    group_if = null;
                    group_if2 = null;
                    group_if = DB.GetDataGroup(ls_group2);
                    if (group_if.Table.Rows.Count > 0)
                    {
                        for (int y = 0; y < group_if.Table.Rows.Count; y++)
                        {

                            try
                            {
                                File.Copy("E:\\Plan15\\plan425-0415-Big-500\\" + ls_fullname + "_" + (y + 1) + ".jpg", "E:\\Plan15\\plan425-0415-Sub\\" + group_if.Table.Rows[y]["sub_class"].ToString().Trim() + "\\" + ls_fullname + "_" + (y + 1) + ".jpg", true);

                                string ls_up = "UPDATE plan425_pic SET oldname='" + group_if.Table.Rows[y]["sub_class"].ToString().Trim()+"/"+ls_fullname + "_" + (y + 1) + ".jpg'  WHERE id='" + group_if.Table.Rows[y]["id"].ToString().Trim() + "' ";
                                DB.ExecuteSQL(ls_up);

                            }
                            catch { }
                        }
                    }


                    //主图-小图
                    string ls_group3 = "select id,(select sub_class from plan425_b2 where id=plan425_pic.pid)  as sub_class from plan425_pic where pid='" + ls_id + "'    and flag='S' and status !=0  order by id ";
                    group_if2 = null;
                    group_if2 = DB.GetDataGroup(ls_group3);
                    if (group_if2.Table.Rows.Count > 0)
                    {
                        for (int y = 0; y < group_if2.Table.Rows.Count; y++)
                        {
                            try
                            {

                                File.Copy("E:\\Plan15\\plan425-0415-Big-500\\" + ls_fullname + ".jpg", "E:\\Plan15\\plan425-0415-Sub\\" + group_if2.Table.Rows[y]["sub_class"].ToString().Trim() + "\\" + ls_fullname + ".jpg", true);

                                string ls_up = "UPDATE plan425_pic SET oldname='" + group_if.Table.Rows[y]["sub_class"].ToString().Trim()+"/"+ls_fullname + ".jpg'  WHERE id='" + group_if2.Table.Rows[y]["id"].ToString().Trim() + "' ";
                                DB.ExecuteSQL(ls_up);


                            }
                            catch { }
                        }
                    }

                }


                //如果没有明细图，只有一个大图的情况

                //将主图第一个放到详细图里
                string ls_bignull = "select  (select id from plan425_pic where pid=plan425_b2.id and flag='S') as id, pname_ok as fullname,sub_class from plan425_b2  where pic_group='' and len(pic_s)>5  order by id ";
                //详细图有水印，用主图

                group_null = DB.GetDataGroup(ls_bignull);
                if (group_null.Table.Rows.Count > 0)
                {
                    for (int g = 0; g < group_null.Table.Rows.Count; g++)
                    {

                        try
                        {
                            File.Copy("E:\\Plan15\\plan425-0415-Big-500\\" + group_null.Table.Rows[g]["fullname"].ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-").Replace(".", "-").Replace("#", "").Replace("*", "-").Trim().ToLower().Trim() + ".jpg", "E:\\Plan15\\plan425-0415-Sub\\" + group_null.Table.Rows[g]["sub_class"].ToString().Trim() + "\\" + group_null.Table.Rows[g]["fullname"].ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-").Replace(".", "-").Replace("#", "").Replace("*", "-").Trim().ToLower().Trim() + ".jpg", true);
                        }
                        catch
                        { }
                    }
                }




            }

            MessageBox.Show("ok!");
        }

        private void button128_Click(object sender, EventArgs e)
        {

            DataGroup group_field = new DataGroup();
            group_field = null;

            DataGroup group = new DataGroup();
            group = null;


            string ls_sql = "select id,Product_Type,Price_Terms,Payment_Terms,Package,Minimum_Order,Delivery_Time,Pattern,Thickness,Width,Features from plan425_b2 where status=2 order by id";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {

                    string ls_shtml = "";
    
                    if (group_field.Table.Rows[i]["Product_Type"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Product Type:<b>" + group_field.Table.Rows[i]["Product_Type"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Pattern"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Pattern:<b>" + group_field.Table.Rows[i]["Pattern"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Thickness"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Thickness:<b>" + group_field.Table.Rows[i]["Thickness"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Width"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Width:<b>" + group_field.Table.Rows[i]["Width"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Minimum_Order"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Minimum Order:<b>" + group_field.Table.Rows[i]["Minimum_Order"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Delivery_Time"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Delivery Detail:<b>" + group_field.Table.Rows[i]["Delivery_Time"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Price_Terms"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Price Terms:<b>" + group_field.Table.Rows[i]["Price_Terms"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Payment_Terms"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Payment Terms:<b>" + group_field.Table.Rows[i]["Payment_Terms"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Package"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Packaging:<b>" + group_field.Table.Rows[i]["Package"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Features"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Features:<b>" + group_field.Table.Rows[i]["Features"].ToString().Trim() + "</b></p>";
                    }


                    string a = "";

                    string ls_up2 = "update plan425_b2 set description ='" +a+ ls_shtml.Replace('\'', '‘') + " '  where id='" + group_field.Table.Rows[i]["id"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up2);

                 




                }
            }

            MessageBox.Show("OK!");
        }

        private void button129_Click(object sender, EventArgs e)
        {
            //------------------------------------生成目录表和目录描述表--------------------------------------------------------------
            string ls_ttt = @"
drop table plan425_categories;
CREATE TABLE [dbo].[plan425_categories](
	[categories_id] [bigint] IDENTITY(1,1) NOT NULL,
	[parent_id] [bigint] NULL,
	[categories_image] [nvarchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[sort_order] [bigint] NULL,
	[categories_status] [nvarchar](1) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [PK_plan425_categories] PRIMARY KEY CLUSTERED 
(
	[categories_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];

drop table [plan425_categories_description]
CREATE TABLE [dbo].[plan425_categories_description](
	[categories_id] [bigint] IDENTITY(1,1) NOT NULL,
	[language_id] [bigint] NULL,
	[categories_name] [nvarchar](200) COLLATE Chinese_PRC_CI_AS NULL,
	[categories_description] [nvarchar](500) COLLATE Chinese_PRC_CI_AS NULL,
	[url] [nvarchar](200) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [PK_plan425_categories_description] PRIMARY KEY CLUSTERED 
(
	[categories_id] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];

update plan425_b2 set sub_class='' where sub_class is null;
update plan425_b2 set cid='',curl='',cid_sub='',curl_sub='';

update plan425_b2 set purl=lower(replace(replace(replace(pname_ok,'.',''),' ','-'),'--','-'))+'-p-'+cast(xid as nvarchar(10))+'.html';

";
            try
            {
                DB.ExecuteSQL(ls_ttt);
            }
            catch
            { }
            //--------------------------------生成2张表的数据------------------------------------------------------------------
            DataGroup group_if = new DataGroup();
            DataGroup group_field = new DataGroup();
            DataGroup group_sub = new DataGroup();
            DataGroup group_url = new DataGroup();
            DataGroup group_cid = new DataGroup();
            string ls_sql = "select distinct main_class,sub_class from plan425_b2 where status in (2,3,5)";
            group_field = null;
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {
                    string ls_if = "select b.categories_name from plan425_categories a,plan425_categories_description b where a.categories_id=b.categories_id and a.parent_id=0 and b.categories_name='" + group_field.Table.Rows[i]["main_class"].ToString().Trim() + "'";
                    group_if = null;
                    group_if = DB.GetDataGroup(ls_if);
                    if (group_if.Table.Rows.Count == 0)
                    {
                        string ls_aaa = "insert into plan425_categories (parent_id,categories_image,sort_order,categories_status)  values (0,'',0,1) ";
                        DB.ExecuteSQL(ls_aaa);
                        string ls_bbb = "insert into plan425_categories_description (language_id,categories_name,categories_description)  values (1,'" + group_field.Table.Rows[i]["main_class"].ToString().Trim() + "' ,'') ";
                        DB.ExecuteSQL(ls_bbb);
                    }

                    if (group_field.Table.Rows[i]["sub_class"].ToString().Trim().Length > 0)
                    {
                        string ls_sub = "select a.categories_id from plan425_categories_description a,plan425_categories b where a.categories_id=b.categories_id and b.parent_id=0 and  a.categories_name='" + group_field.Table.Rows[i]["main_class"].ToString().Trim() + "'";
                        group_sub = null;
                        group_sub = DB.GetDataGroup(ls_sub);
                        if (group_sub.Table.Rows.Count > 0)
                        {
                            string ls_ccc = "insert into plan425_categories (parent_id,categories_image,sort_order,categories_status)  values ('" + group_sub.Table.Rows[0]["categories_id"].ToString().Trim() + "' ,'',0,1) ";
                            DB.ExecuteSQL(ls_ccc);
                            string ls_ddd = "insert into plan425_categories_description (language_id,categories_name,categories_description)  values (1,'" + group_field.Table.Rows[i]["sub_class"].ToString().Trim() + "' ,'') ";
                            DB.ExecuteSQL(ls_ddd);
                        }
                    }
                }
            }
            //------------------------------------为产品描述表生成url--------------------------------------------------------------
            string ls_url = @"
select categories_id,parent_id,
(select categories_name from plan425_categories_description where plan425_categories_description.categories_id=plan425_categories.categories_id)
+'-c-'+cast(categories_id as nvarchar(20)) as categories_name
from plan425_categories where parent_id=0
union
select categories_id,parent_id,
(select categories_name from plan425_categories_description 
where plan425_categories_description.categories_id=plan425_categories.parent_id) 
+'-'+
(select categories_name from plan425_categories_description where plan425_categories_description.categories_id=plan425_categories.categories_id)
+'-c-'+cast(parent_id as nvarchar(20))+'_'+cast(categories_id as nvarchar(20))  as categories_name
from plan425_categories where parent_id!=0";
            group_url = null;
            group_url = DB.GetDataGroup(ls_url);
            if (group_url.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_url.Table.Rows.Count; i++)
                {
                    string ls_UP = "update plan425_categories_description set url='" + group_url.Table.Rows[i]["categories_name"].ToString().Trim().Replace("\"", "").Replace("/", "").Replace(".", "").Replace(" ", "-").Replace("--", "-").ToLower() +".html"+ "' where categories_id='" + group_url.Table.Rows[i]["categories_id"].ToString().Trim() + "'";
                    DB.ExecuteSQL(ls_UP);
                }
            }
            //--------------------------------为每个产品添加目录的url------------------------------------------------------------------
            string ls_cid = @"
select categories_id,categories_name,sub_categories_name,parent_id,url from
(
select a.categories_id,b.categories_name,'' as sub_categories_name,a.parent_id,b.url 
from plan425_categories a,plan425_categories_description b
where a.categories_id=b.categories_id and parent_id=0
union
select a.categories_id, 
(select x.categories_name from plan425_categories y,plan425_categories_description x
where x.categories_id=y.categories_id and y.categories_id=a.parent_id) as categories_name,
b.categories_name as sub_categories_name,a.parent_id,b.url 
from plan425_categories a,plan425_categories_description b
where a.categories_id=b.categories_id and parent_id!=0
) xxx order by xxx.categories_id";
            group_cid = null;
            group_cid = DB.GetDataGroup(ls_cid);
            if (group_cid.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_cid.Table.Rows.Count; i++)
                {
                    if (group_cid.Table.Rows[i]["parent_id"].ToString().Trim() == "0")
                    {
                        string ls_upc1 = "update plan425_b2 set cid='" + group_cid.Table.Rows[i]["categories_id"].ToString().Trim() + "' , " +
                            " curl='" + group_cid.Table.Rows[i]["url"].ToString().Trim() + "' " +
                            " where main_class='" + group_cid.Table.Rows[i]["categories_name"].ToString().Trim() + "' ";
                        DB.ExecuteSQL(ls_upc1);
                    }
                    else
                    {
                        string ls_upc3 = "update plan425_b2 set cid_sub='" + group_cid.Table.Rows[i]["categories_id"].ToString().Trim() + "' , " +
                                " curl_sub='" + group_cid.Table.Rows[i]["url"].ToString().Trim() + "' " +
                                " where main_class='" + group_cid.Table.Rows[i]["categories_name"].ToString().Trim() + "' " +
                                " and sub_class='" + group_cid.Table.Rows[i]["sub_categories_name"].ToString().Trim() + "' ";
                        DB.ExecuteSQL(ls_upc3);
                    }
                }
            }

            /*
            //clear
            string ls_name = "E:\\PHP-1.5.6\\vhosts\\www.x.com\\plan-categories.sql";
            using (FileStream fs = File.Open(@ls_name, FileMode.Create))
            {
                byte[] b = System.Text.Encoding.Default.GetBytes("");
                fs.Write(b, 0, b.Length);
                fs.Close();
            }

            //------------------------------为2站表的数据生成脚本--------------------------------------------------------------------
            DataGroup group_super = new DataGroup();
            string ls_super = " select 'insert into `jv3_categories` (`categories_id`,`parent_id`,`sort_order`,`categories_status`) values ('+ cast(categories_id as nvarchar(10))+','+cast(parent_id as nvarchar(10))+','+cast(sort_order as nvarchar(10))+','+categories_status+' );' as ok from plan425_categories  "+
                "union select 'insert into `jv3_categories_description` (`categories_id`,`language_id`,`categories_name`) values ('+ cast(categories_id as nvarchar(10))+','+cast(language_id as nvarchar(10))+','''+categories_name+''' );' as ok from plan425_categories_description ";
            group_super = DB.GetDataGroup(ls_super);
            if (group_super.Table.Rows.Count > 0)
            {
                
                string ls_ddd = "";
                for (int x = 0; x < group_super.Table.Rows.Count; x++)
                {
                    ls_ddd += group_super.Table.Rows[x]["ok"].ToString() + "\n";
                    using (FileStream fs = File.Open(@ls_name, FileMode.Append))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_ddd);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_ddd = "";
                    }
                }
            }
            */
            //------------------------------------恭喜发财！--------------------------------------------------------------
            MessageBox.Show("OK!");
        }

        private void button130_Click(object sender, EventArgs e)
        {
            DataGroup group_list = new DataGroup();
            string ls_kw_list = "select id,kw from plan425_kw where id <75 order by id";
            group_list = null;
            group_list = DB.GetDataGroup(ls_kw_list);
            ArrayList list = new ArrayList();
            if (group_list.Table.Rows.Count > 0)
            {
                for (int y = 0; y < group_list.Table.Rows.Count; y++)
                {
                    list.Add(group_list.Table.Rows[y]["kw"].ToString());
                }
            }
            Random ra = new Random();
            int num = group_list.Table.Rows.Count;
            int value = ra.Next(num);

            DataGroup group_product_images = new DataGroup();
            string ls_group_product_images = "select id from plan425_b2 ";
            group_product_images = DB.GetDataGroup(ls_group_product_images);
            if (group_product_images.Table.Rows.Count > 0)
            {
                for (int y = 0; y < group_product_images.Table.Rows.Count; y++)
                {
                    value = ra.Next(num);
                    string dog = list[value].ToString(); //.Replace('\'', '-').Replace(' ', '-');

                    string ls_up = "UPDATE plan425_b2 SET KW='" + dog + "' WHERE ID='" + group_product_images.Table.Rows[y]["ID"].ToString() + "'  ";
                    DB.ExecuteSQL(ls_up);
                }
            }
            MessageBox.Show("OK!");
        }

        private void button131_Click(object sender, EventArgs e)
        {
            //代码暂时取消

            DataGroup group_list = new DataGroup();
            string ls_kw_list = "select id,kw from plan425_kw where id <75 order by id";
            group_list = null;
            group_list = DB.GetDataGroup(ls_kw_list);

            string ls_cc = @"  
            --update plan425_b2 set pname2=pname1;
            ";
            DB.ExecuteSQL(ls_cc);


            DataGroup group_main_class = new DataGroup();
            string ls_group_main_class = "select  pname2 as remark from plan425_b2 where status !=0 group by pname2 having count(*) >1";
            //string ls_group_main_class = "select id,a,b,spinner from a_gp4 where flag=1 and status=1  ";
            group_main_class = DB.GetDataGroup(ls_group_main_class);
            if (group_main_class.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_main_class.Table.Rows.Count; x++)
                {
                    DataGroup group_kw = new DataGroup();
                    group_kw = null;
                    string ls_group_kw = "select id from plan425_b2 where pname2='" + group_main_class.Table.Rows[x]["remark"].ToString().Trim() + "'   order by id ";
                    group_kw = DB.GetDataGroup(ls_group_kw);

                    if (group_kw.Table.Rows.Count > 0)
                    {
                        for (int k = 0; k < group_kw.Table.Rows.Count; k++)
                        {
                            string ls_up = "UPDATE plan425_b2 SET pname2=pname1+' '+'" + group_list.Table.Rows[k]["kw"].ToString().Trim() + "'   WHERE id='" + group_kw.Table.Rows[k]["id"].ToString() + "' "; 
                            DB.ExecuteSQL(ls_up);
                        }
                    }

                }

            }
                

            MessageBox.Show("ok!");
        }

        private void button132_Click(object sender, EventArgs e)
        {


            string ls_xx = "UPDATE plan425_b2 SET desc2='<b>'+pname_OK +'</b><br/>'+description; UPDATE plan425_b2 SET desc2=replace(desc2,'<br/><br/>','<br/>');";
            DB.ExecuteSQL(ls_xx);
            MessageBox.Show("OK!x");
            return;

            /*
            1.主KW1个
            2.本产品关键字一个
            3.产品描述
            4.5.目录级关键字1个（如果目录有2个级别，目录就有2个）,
            6.主KW1个,
            其中第二个主关键字，融合到通用描述里面去
             */

            //-------------------主关键字和网址--------------------------------------------
            string ls_main_kw = " Hawaii";
            string ls_sub_kw = " Outlet Hawaii";
            string ls_main_url = "http://www.x.com";
            //--------------------随机关键字-----------------------------------------------
            //DataGroup group_list = new DataGroup();            
            //string ls_kw_list = "select id,kw from plan425_kw order by id";            group_list = null;            group_list = DB.GetDataGroup(ls_kw_list);            ArrayList list = new ArrayList();            if (group_list.Table.Rows.Count > 0)            {                for (int y = 0; y < group_list.Table.Rows.Count; y++)                {                    list.Add(group_list.Table.Rows[y]["kw"].ToString());                }            }            Random ra = new Random();            int num = group_list.Table.Rows.Count;            int value = ra.Next(num);
            //-----------------------------------------------------------------------------

            DataGroup group_add_link = new DataGroup();
            string ls_group_add_link = "select id,main_class,sub_class,xid,purl,curl,curl_sub,product_name,pname1,pname_ok,description,brand from plan425_b2 where status in (2,3,5)";
            group_add_link = DB.GetDataGroup(ls_group_add_link);
            if (group_add_link.Table.Rows.Count > 0)
            {
                for (int y = 0; y < group_add_link.Table.Rows.Count; y++)
                {
                    string dog = "";

                    string good1 = "<a href=\"" + ls_main_url + "\">" + group_add_link.Table.Rows[y]["brand"].ToString() + ls_main_kw + "</a>";
                    //string good1 = "<a href=\"" + ls_main_url + "\">"  + ls_main_kw + "</a>";
                    string good2 = "<a href=\"" + group_add_link.Table.Rows[y]["purl"].ToString() + "\">" + group_add_link.Table.Rows[y]["pname_ok"].ToString() + "</a>";
                    string good3 = group_add_link.Table.Rows[y]["description"].ToString();


                    string good_dir = "";

                    //value = ra.Next(num);
                    //dog = list[value].ToString();
                    //string good4 = "<a href=\"" + group_add_link.Table.Rows[y]["curl"].ToString() + "\">" + dog + "</a>";

                    string good4 = "<a href=\"" + group_add_link.Table.Rows[y]["curl"].ToString() + "\">" + group_add_link.Table.Rows[y]["brand"].ToString()+ls_sub_kw + "</a>";
                    //string good4 = "<a href=\"" + group_add_link.Table.Rows[y]["curl"].ToString() + "\">" + ls_sub_kw + "</a>";

                    good_dir = good4;

                    //string good5 ="";
                    //if (group_add_link.Table.Rows[y]["curl_sub"].ToString().Length > 5)
                    //{
                    //    //value = ra.Next(num);
                    //    //dog = list[value].ToString();
                    //    //good5 = "<a href=\"" + group_add_link.Table.Rows[y]["curl_sub"].ToString() + "\">" + dog + "</a>";
                    //    good5 = "<a href=\"" + group_add_link.Table.Rows[y]["curl_sub"].ToString() + "\">" + ls_sub_kw + "</a>";

                    //    good_dir = good5;
                    //}
                    //else
                    //{
                    //    good_dir = good4;
                    //}
                    
                    //string good6 = "<a href=\"" + ls_main_url + "\">" + ls_sub_kw + "</a>";

                    
                    //--------------------取同一个大类的商品---------------------------------
                    DataGroup group_list = new DataGroup();
                    string ls_kw_list = "select purl,pname1,pname_ok from plan425_b2 where id='" + group_add_link.Table.Rows[y]["xid"].ToString() + "' and status in (2,3,5) ";
                    group_list = null; group_list = DB.GetDataGroup(ls_kw_list);
                    //-----------------------------------------------------------------------------
                    string good_class = "<a href=\"" + group_list.Table.Rows[0]["purl"].ToString() + "\">" + group_list.Table.Rows[0]["pname_ok"].ToString() + "</a>";

                                                                        
                    //string sum_good = good1 + " >> " + good2 + "<br/>" + good3 + "<br/>You may also like " + " " + good_dir + " >> " + good_class + "<br/>";
                                                                         //------添加原名称
                    string sum_good = good1 + " >> " + good2 + "<br/>" + group_add_link.Table.Rows[y]["product_name"].ToString() + "<br/>" + good3 + "<br/>You may also like " + " " + good_dir + " >> " + good_class + "<br/>";

                    
                    

                    //class=navigationlink style=\"TEXT-DECORATION: none;\" 

                    string ls_up = "UPDATE plan425_b2 SET desc2='" + sum_good + "' WHERE ID='" + group_add_link.Table.Rows[y]["ID"].ToString() + "'  ";
                    DB.ExecuteSQL(ls_up);
                }
            }
            MessageBox.Show("OK!");
        }

        private void button133_Click(object sender, EventArgs e)
        {
            DataGroup group_class = new DataGroup();
            string ls_class="select distinct main_class from plan425_b2 where status in (2,3,5)";
            group_class=null;
            group_class= DB.GetDataGroup(ls_class);
            if (group_class.Table.Rows.Count > 0)
            {
                for (int ppp = 0; ppp < group_class.Table.Rows.Count; ppp++)
                {


                    DataGroup group_list = new DataGroup();
                    string ls_kw_list = "select id from plan425_b2 where status in (2,3,5) and main_class='" + group_class.Table.Rows[ppp]["main_class"].ToString() + "' ORDER BY NEWID() ";
                    group_list = null;
                    group_list = DB.GetDataGroup(ls_kw_list);
 

                    DataGroup group_product_images = new DataGroup();
                    string ls_group_product_images = "select id from plan425_b2 where status in (2,3,5) and main_class='" + group_class.Table.Rows[ppp]["main_class"].ToString() + "'";
                    group_product_images = DB.GetDataGroup(ls_group_product_images);
                    if (group_product_images.Table.Rows.Count > 0)
                    {
                        for (int y = 0; y < group_product_images.Table.Rows.Count; y++)
                        {
                            string ls_up = "UPDATE plan425_b2 SET xid='" + group_list.Table.Rows[y]["ID"].ToString() + "' WHERE ID='" + group_product_images.Table.Rows[y]["ID"].ToString() + "'  ";
                            DB.ExecuteSQL(ls_up);
                        }
                    }


                }

            }
            MessageBox.Show("OK!");
        }

        private void button134_Click(object sender, EventArgs e)
        {

            DataGroup group_field = new DataGroup();
            group_field = null;

            DataGroup group = new DataGroup();
            group = null;


            string ls_sql = "select id, material,color, size from plan425_b2 where status=2 order by id";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {

                    string ls_shtml = "";


                    //if (group_field.Table.Rows[i]["mid"].ToString().Trim().Length > 1)
                    //{
                    //    ls_shtml += "<p>Ref:<b>" + group_field.Table.Rows[i]["mid"].ToString().Trim() + "</b></p>";
                    //}

                    if (group_field.Table.Rows[i]["material"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<br/>Material:<b>" + group_field.Table.Rows[i]["material"].ToString().Trim() + "</b>";
                    }

                    if (group_field.Table.Rows[i]["color"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<br/>Color:<b>" + group_field.Table.Rows[i]["color"].ToString().Trim() + "</b>";
                    }

                    if (group_field.Table.Rows[i]["size"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<br/>Size:<b>" + group_field.Table.Rows[i]["size"].ToString().Trim() + "</b>";
                    }

                    //if (group_field.Table.Rows[i]["design"].ToString().Trim().Length > 1)
                    //{
                    //    ls_shtml += "<p>Design:<b>" + group_field.Table.Rows[i]["design"].ToString().Trim() + "</b></p>";
                    //}


                    //if (group_field.Table.Rows[i]["feature"].ToString().Trim().Length > 1)
                    //{
                    //    ls_shtml += "<p>Feature: " + group_field.Table.Rows[i]["feature"].ToString().Trim() + "</p>";
                    //}

                   
                    //if (group_field.Table.Rows[i]["package"].ToString().Trim().Length > 1)
                    //{
                    //    ls_shtml += "<p>Package: " + group_field.Table.Rows[i]["package"].ToString().Trim() + "</p>";
                    //}

                    string a = "";

                    string ls_up2 = "update plan425_b2 set description ='" + a + ls_shtml.Replace('\'', '‘') + "<br/>' +description  where id='" + group_field.Table.Rows[i]["id"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up2);






                }
            }

            MessageBox.Show("OK!");
        }

        private void button135_Click(object sender, EventArgs e)
        {

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT ID,description as remark FROM plan425_b2 where status=2 order by ID";//WHERE STATUS =1   where status=3 
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["remark"].ToString().Replace("\n", "").Replace("\t","").Replace("\r\n","");

                    string ls_up2 = "update plan425_b2 set description='" + IsGanBr(ls_shtml).Trim() + "'  where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up2);
                }

            }


            MessageBox.Show("OK!!");
        }

        private void button136_Click(object sender, EventArgs e)
        {
            DataGroup group_field = new DataGroup();
            group_field = null;

            DataGroup group = new DataGroup();
            group = null;

            string ls_sql = "select id,product_name,Brand,Ref,Series,Type,Color,Material,Season,Size from plan425_b2 where status in (2,5) order by id";
            group_field = DB.GetDataGroup(ls_sql);
            if (group_field.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_field.Table.Rows.Count; i++)
                {

                    string ls_shtml = "";

                   
                    
                    if (group_field.Table.Rows[i]["Color"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Color: <b>" + group_field.Table.Rows[i]["Color"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Size"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Size: <b>" + group_field.Table.Rows[i]["Size"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Material"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Material: <b>" + group_field.Table.Rows[i]["Material"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Season"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Season: <b>" + group_field.Table.Rows[i]["Season"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Type"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Type: <b>" + group_field.Table.Rows[i]["Type"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Ref"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Ref: <b>" + group_field.Table.Rows[i]["Ref"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Series"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Series: <b>" + group_field.Table.Rows[i]["Series"].ToString().Trim() + "</b></p>";
                    }
                    if (group_field.Table.Rows[i]["Brand"].ToString().Trim().Length > 1)
                    {
                        ls_shtml += "<p>Brand: <b>" + group_field.Table.Rows[i]["Brand"].ToString().Trim() + "</b></p>";
                    }

                    string a = "";// "<br/><b>" + group_field.Table.Rows[i]["product_name"].ToString().Trim() + "</b><br/>";

                    string ls_up2 = "update plan425_b2 set description ='" + a + ls_shtml.Replace('\'', '‘') + " '  where id='" + group_field.Table.Rows[i]["id"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up2);

                }
            }

            MessageBox.Show("OK!");
        }

        private void button137_Click(object sender, EventArgs e)
        {
            //DataGroup group = new DataGroup();

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group_html = null;

            string ls_sql = "SELECT ID,pic_group as pic_group FROM  plan425_b2 where pic_group !=''  order by ID";//WHERE STATUS =2
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["pic_group"].ToString();
                    string[] dog_small = ls_shtml.Split('|');

                    

                    int yyy = dog_small.Length;
                    string xxx = yyy.ToString();

                    //foreach (string bb in dog_small)
                    //{
                    //    string xx = bb.Trim();

                    //    group = null;
                    //    string ls_double = "select id from plan425_PIC where url_pic='" + xx + "' and status=1   ";
                    //    group = DB.GetDataGroup(ls_double);
                    //    if (group.Table.Rows.Count == 0)
                    //    {
                    //        string ls_up = "insert into plan425_PIC (pid,url_pic,flag,status) values ('" + group_html.Table.Rows[i]["ID"].ToString().Trim() + "','" + xx + "','B',0)";//http://www.xxx.com/
                    //        DB.ExecuteSQL(ls_up);
                    //    }

                    //}

                    string ls_up2 = "update plan425_b2 set pname1='"+xxx+"'  where id='" + group_html.Table.Rows[i]["id"].ToString().Trim() + "' ";
                    DB.ExecuteSQL(ls_up2);

                }
            }

            MessageBox.Show("OK!");

        }

        private void button138_Click(object sender, EventArgs e)
        {
            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group_html = null;

            string ls_sql = "SELECT ID,pic_group as pic_group,pname1,pic_s FROM  plan425_b2 where pic_group !=''  order by ID";//WHERE STATUS =2
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_pic_s = group_html.Table.Rows[i]["pic_s"].ToString().Trim().Replace(".jpg", "");
                    int ls_count = int.Parse(group_html.Table.Rows[i]["pname1"].ToString().Trim());

                    string xxx = "";

                    for (int y = 1; y <= ls_count; y++)
                    {

                        
                        
                        if (y==1)
                        {
                            xxx += ls_pic_s + "_0" + y.ToString() + ".jpg";
                        }
                        else if (y > 1 && y < 10)
                        {
                            xxx += "|" + ls_pic_s + "_0" + y.ToString() + ".jpg";
                        }
                        else
                        {
                            xxx += "|" + ls_pic_s + "_" +  y.ToString() + ".jpg";
                        }

                    }
                         string ls_up2 = "update plan425_b2 set pic_group='" + xxx + "'  where id='" + group_html.Table.Rows[i]["id"].ToString().Trim() + "' ";
                        DB.ExecuteSQL(ls_up2);
                    

                }
            }

            MessageBox.Show("OK!");
        }

        private void button139_Click(object sender, EventArgs e)
        {

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT ID,html FROM plan425_b2 order by ID";//WHERE STATUS =1   where status=3 
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {

                    string ls_shtml = group_html.Table.Rows[i]["html"].ToString().Replace("\n", "");
                    //if (ls_shtml.IndexOf("add-to-basket.gif") > -1)
                    //if (ls_shtml.IndexOf("button_in_cart.gif") > -1)
                    //if (ls_shtml.IndexOf("car.jpg") > -1)
                    if (ls_shtml.IndexOf("add_to_cart.gif") > -1)
                    {
                        string ls_up2 = "update plan425_b2 set status=1 where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                        DB.ExecuteSQL(ls_up2);

                    }
                    else
                    {
                        string ls_up2 = "update plan425_b2 set status=0 where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                        DB.ExecuteSQL(ls_up2);
                    }

                }

            }


            MessageBox.Show("OK!!");
        }

        private void button140_Click(object sender, EventArgs e)
        {
            string LS_html = l_html.Text;
            l_html.Text = NoHTML(LS_html).Trim();

        }

        private void button142_Click(object sender, EventArgs e)
        {
            /*
             * 一.sql主key替换
             * 二.2个配置文件替换
             * 三.Meta文件替换
             * 四.top head文件替换
             */

            
            //------------------------------------------------------------------------------------------------
            DataGroup group_config = new DataGroup();
            string ls_config = "SELECT ga,ga_flag,id,code,dbname,dbuser,dbpassword,url,admin_dir,kw_main,TITLE,SITE_TAGLINE,CUSTOM_KEYWORDS,HOME_PAGE_META_DESCRIPTION,HOME_PAGE_META_KEYWORDS,HOME_PAGE_TITLE FROM PlanConfig Where Status=2";
            group_config = DB.GetDataGroup(ls_config);
            if (group_config.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_config.Table.Rows.Count; i++)
                {
                    //----------------------------------------取配置--------------------------------------------------
                    string lc_code = group_config.Table.Rows[i]["code"].ToString().Trim();
                    string lc_ga = group_config.Table.Rows[i]["ga"].ToString().Trim();
                    string lc_ga_flag = group_config.Table.Rows[i]["ga_flag"].ToString().Trim();
                    string lc_dbname = group_config.Table.Rows[i]["dbname"].ToString().Trim();
                    string lc_dbuser = group_config.Table.Rows[i]["dbuser"].ToString().Trim();
                    string lc_dbpassword = group_config.Table.Rows[i]["dbpassword"].ToString().Trim();
                    string lc_url = group_config.Table.Rows[i]["url"].ToString().Trim();
                    string lc_url_shot = group_config.Table.Rows[i]["url"].ToString().Replace("www.","").Trim();
                    string lc_admin_dir = group_config.Table.Rows[i]["admin_dir"].ToString().Trim();
                    string lc_kw_main = group_config.Table.Rows[i]["kw_main"].ToString().Trim();
                    string lc_TITLE = group_config.Table.Rows[i]["TITLE"].ToString().Trim();
                    string lc_SITE_TAGLINE = group_config.Table.Rows[i]["SITE_TAGLINE"].ToString().Trim();
                    string lc_CUSTOM_KEYWORDS = group_config.Table.Rows[i]["CUSTOM_KEYWORDS"].ToString().Trim();
                    string lc_HOME_PAGE_META_DESCRIPTION = group_config.Table.Rows[i]["HOME_PAGE_META_DESCRIPTION"].ToString().Trim();
                    string lc_HOME_PAGE_META_KEYWORDS = group_config.Table.Rows[i]["HOME_PAGE_META_KEYWORDS"].ToString().Trim();
                    string lc_HOME_PAGE_TITLE = group_config.Table.Rows[i]["HOME_PAGE_TITLE"].ToString().Trim();
                    //------------------------------------------------------------------------------------------------
                    //--------------------------------------sql主key替换----------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //string ls_sqlfile_Base = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\150l302B.sql";
                    string ls_sqlfile_Base = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\150LB.sql";
                    string ls_sqlfile_New = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\150lc.sql";
                    StreamReader fs_sqlfile = new StreamReader(ls_sqlfile_Base, System.Text.Encoding.UTF8);
                    string ls_sqlfile_Text = fs_sqlfile.ReadToEnd();
                    fs_sqlfile.Close();
                    ls_sqlfile_Text = ls_sqlfile_Text.Replace("'nike'", "'" + lc_kw_main + "'").Replace("x.com", lc_url_shot);
                    //订单编码规则                             AUTO_INCREMENT=16302001
                    ls_sqlfile_Text = ls_sqlfile_Text.Replace("AUTO_INCREMENT=16302001", "AUTO_INCREMENT=16" + lc_code + "001");
                    using (FileStream fs = File.Open(ls_sqlfile_New, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_sqlfile_Text);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_sqlfile_Text = "";
                    }
                    //------------------------------------------END---------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //---------------------------------------2个配置文件替换------------------------------------------
                    string ls_configure_Base = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\includes\\configure.php";
                    string ls_configure_New = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\includes\\configure.php";
                    StreamReader fs_configure = new StreamReader(ls_configure_Base, System.Text.Encoding.UTF8);
                    string ls_configure = fs_configure.ReadToEnd();
                    fs_configure.Close();
                    //【标准站】替换为【新站】
                    ls_configure = ls_configure.Replace("x.com", lc_url_shot);
                    //【数据库名称】替换
                    ls_configure = ls_configure.Replace("define('DB_DATABASE', '150L302');", "define('DB_DATABASE', '" + lc_dbname + "');");

                    ////-----------------------------------------★★★★★★★★★★★★★★-------------------------------------
                    ////【数据库用户名】
                    //ls_configure = ls_configure.Replace("define('DB_SERVER_USERNAME', 'root');", "define('DB_SERVER_USERNAME', '" + lc_dbuser + "');");
                    ////【旧密码】替换为【新密码】
                    //ls_configure = ls_configure.Replace("define('DB_SERVER_PASSWORD', 'admin');", "define('DB_SERVER_PASSWORD', '" + lc_dbpassword + "');");
                    ////【旧路径】替换为【新路径】
                    //ls_configure = ls_configure.Replace("define('DIR_FS_CATALOG', 'E:/PHP-1.5.6/vhosts/", "define('DIR_FS_CATALOG', '/home/wwwroot/");
                    //ls_configure = ls_configure.Replace("define('DIR_FS_SQL_CACHE', 'E:/PHP-1.5.6/vhosts/", "define('DIR_FS_SQL_CACHE', '/home/wwwroot/");
                    ////-----------------------------------------★★★★★★★★★★★★★★-------------------------------------

                    //去掉文件只读
                    System.IO.File.SetAttributes(ls_configure_New, System.IO.FileAttributes.Normal);
                    /*去除文件夹的只读属性：  System.IO.DirectoryInfo DirInfo = new DirectoryInfo(“filepath”);
                　　　　　　　DirInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
                        去除文件的只读属性：　System.IO.File.SetAttributes("filepath", System.IO.FileAttributes.Normal);*/

                    using (FileStream fs = File.Open(ls_configure_New, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_configure);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_configure = "";
                    }
                    //------------------------------------------------------------------------------------------------
                    string ls_configure_admin_Base = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\xing\\includes\\configure.php";
                    string ls_configure_admin_New = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\xing\\includes\\configure.php";
                    StreamReader fs_configure_admin = new StreamReader(ls_configure_admin_Base, System.Text.Encoding.UTF8);
                    string ls_configure_admin = fs_configure_admin.ReadToEnd();
                    fs_configure_admin.Close();
                    //【标准站】替换为【新站】
                    ls_configure_admin = ls_configure_admin.Replace("x.com", lc_url_shot);
                    //【数据库名称】替换
                    ls_configure_admin = ls_configure_admin.Replace("define('DB_DATABASE', '150L302');", "define('DB_DATABASE', '" + lc_dbname + "');");
                    ////-----------------------------------------★★★★★★★★★★★★★★-------------------------------------
                    ////【数据库用户名】
                    //ls_configure_admin = ls_configure_admin.Replace("define('DB_SERVER_USERNAME', 'root');", "define('DB_SERVER_USERNAME', '" + lc_dbuser + "');");
                    ////【旧密码】替换为【新密码】
                    //ls_configure_admin = ls_configure_admin.Replace("define('DB_SERVER_PASSWORD', 'admin');", "define('DB_SERVER_PASSWORD', '" + lc_dbpassword + "');");
                    ////【旧路径】替换为【新路径】
                    //ls_configure_admin = ls_configure_admin.Replace("define('DIR_FS_ADMIN', 'E:/WebHosting/PHP-1.5.6/vhosts/", "define('DIR_FS_ADMIN', '/home/wwwroot/");
                    //ls_configure_admin = ls_configure_admin.Replace("define('DIR_FS_CATALOG',  'E:/PHP-1.5.6/vhosts/", "define('DIR_FS_CATALOG', '/home/wwwroot/");
                    //ls_configure_admin = ls_configure_admin.Replace("define('DIR_FS_SQL_CACHE', 'E:/PHP-1.5.6/vhosts/", "define('DIR_FS_SQL_CACHE', '/home/wwwroot/");
                    ////-----------------------------------------★★★★★★★★★★★★★★-------------------------------------

                    using (FileStream fs = File.Open(ls_configure_admin_New, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_configure_admin);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_configure_admin = "";
                    }
                    //------------------------------------------END---------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //-----------------------------------------meta文件替换-------------------------------------------
                    string ls_meta_tags_Base = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\includes\\languages\\english\\meta_tags.php";
                    string ls_meta_tags_New = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\includes\\languages\\english\\meta_tags.php";
                    StreamReader fs_meta_tags = new StreamReader(ls_meta_tags_Base, System.Text.Encoding.UTF8);
                    string ls_meta_tags = fs_meta_tags.ReadToEnd();
                    fs_meta_tags.Close();
                    ls_meta_tags = ls_meta_tags.Replace("define('TITLE', 'nike');", "define('TITLE', '" + lc_TITLE + "');");
                    ls_meta_tags = ls_meta_tags.Replace("define('SITE_TAGLINE', 'nike');", "define('SITE_TAGLINE', '" + lc_SITE_TAGLINE + "');");
                    ls_meta_tags = ls_meta_tags.Replace("define('CUSTOM_KEYWORDS', 'designer bags, nike');", "define('CUSTOM_KEYWORDS', '" + lc_CUSTOM_KEYWORDS + "');");
                    ls_meta_tags = ls_meta_tags.Replace("define('HOME_PAGE_META_DESCRIPTION', '');", "define('HOME_PAGE_META_DESCRIPTION', '" + lc_HOME_PAGE_META_DESCRIPTION + "');");
                    ls_meta_tags = ls_meta_tags.Replace("define('HOME_PAGE_META_KEYWORDS', 'nike');", "define('HOME_PAGE_META_KEYWORDS', '" + lc_HOME_PAGE_META_KEYWORDS + "');");
                    ls_meta_tags = ls_meta_tags.Replace("define('HOME_PAGE_TITLE', 'nike');", "define('HOME_PAGE_TITLE', '" + lc_HOME_PAGE_TITLE + "');");
                    using (FileStream fs = File.Open(ls_meta_tags_New, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_meta_tags);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_meta_tags = "";
                    }
                    //------------------------------------------END---------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //----------------------------------------Google分析代码------------------------------------------lc_ga_flag
                    //string ls_foot_Base_A = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\includes\\templates\\bloom_black\\common\\tpl_footer.php";
                    //string ls_foot_New_A = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\includes\\templates\\bloom_black\\common\\tpl_footer.php";
                    //StreamReader fs_foot_A = new StreamReader(ls_foot_Base_A, System.Text.Encoding.UTF8);
                    //string ls_foot_A = fs_foot_A.ReadToEnd();
                    //fs_foot_A.Close();
                    ////--是否添加GA
                    //if (lc_ga_flag == "0")
                    //{
                    //    int start = ls_foot_A.LastIndexOf("<script");
                    //    if (start > 0)
                    //    {
                    //        ls_foot_A = ls_foot_A.Substring(0, start - 0);
                    //    }
                    //}
                    //else
                    //{
                    //    ls_foot_A = ls_foot_A.Replace("UA-25806422-1", lc_ga);
                    //}
                    //using (FileStream fs = File.Open(ls_foot_New_A, FileMode.Create))
                    //{
                    //    byte[] b = System.Text.Encoding.Default.GetBytes(ls_foot_A);
                    //    fs.Write(b, 0, b.Length);
                    //    fs.Close();
                    //    ls_foot_A = "";
                    //}
                    //------------------------------------------------------------------------------------------------
                    //string ls_foot_Base_B = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\includes\\templates\\bloom_blue\\common\\tpl_footer.php";
                    //string ls_foot_New_B = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\includes\\templates\\bloom_blue\\common\\tpl_footer.php";
                    //StreamReader fs_foot_B = new StreamReader(ls_foot_Base_B, System.Text.Encoding.UTF8);
                    //string ls_foot_B = fs_foot_B.ReadToEnd();
                    //fs_foot_B.Close();
                    ////--是否添加GA
                    //if (lc_ga_flag == "0")
                    //{
                    //    int start = ls_foot_B.LastIndexOf("<script");
                    //    if (start > 0)
                    //    {
                    //        ls_foot_B = ls_foot_B.Substring(0, start - 0);
                    //    }
                    //}
                    //else
                    //{
                    //    ls_foot_B = ls_foot_B.Replace("UA-25806422-1", lc_ga);
                    //}
                    //using (FileStream fs = File.Open(ls_foot_New_B, FileMode.Create))
                    //{
                    //    byte[] b = System.Text.Encoding.Default.GetBytes(ls_foot_B);
                    //    fs.Write(b, 0, b.Length);
                    //    fs.Close();
                    //    ls_foot_B = "";
                    //}
                    //------------------------------------------------------------------------------------------------
                    //string ls_foot_Base_C = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\includes\\templates\\bloom_green\\common\\tpl_footer.php";
                    //string ls_foot_New_C = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\includes\\templates\\bloom_green\\common\\tpl_footer.php";
                    //StreamReader fs_foot_C = new StreamReader(ls_foot_Base_C, System.Text.Encoding.UTF8);
                    //string ls_foot_C = fs_foot_C.ReadToEnd();
                    //fs_foot_C.Close();
                    ////--是否添加GA
                    //if (lc_ga_flag == "0")
                    //{
                    //    int start = ls_foot_C.LastIndexOf("<script");
                    //    if (start > 0)
                    //    {
                    //        ls_foot_C = ls_foot_C.Substring(0, start - 0);
                    //    }
                    //}
                    //else
                    //{
                    //    ls_foot_C = ls_foot_C.Replace("UA-25806422-1", lc_ga);
                    //}
                    //using (FileStream fs = File.Open(ls_foot_New_C, FileMode.Create))
                    //{
                    //    byte[] b = System.Text.Encoding.Default.GetBytes(ls_foot_C);
                    //    fs.Write(b, 0, b.Length);
                    //    fs.Close();
                    //    ls_foot_C = "";
                    //}
                    //------------------------------------------------------------------------------------------------
                    //string ls_foot_Base_D = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\includes\\templates\\bloom_orange\\common\\tpl_footer.php";
                    //string ls_foot_New_D = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\includes\\templates\\bloom_orange\\common\\tpl_footer.php";
                    //StreamReader fs_foot_D = new StreamReader(ls_foot_Base_D, System.Text.Encoding.UTF8);
                    //string ls_foot_D = fs_foot_D.ReadToEnd();
                    //fs_foot_D.Close();
                    ////--是否添加GA
                    //if (lc_ga_flag == "0")
                    //{
                    //    int start = ls_foot_D.LastIndexOf("<script");
                    //    if (start > 0)
                    //    {
                    //        ls_foot_D = ls_foot_D.Substring(0, start - 0);
                    //    }
                    //}
                    //else
                    //{
                    //    ls_foot_D = ls_foot_D.Replace("UA-25806422-1", lc_ga);
                    //}
                    //using (FileStream fs = File.Open(ls_foot_New_D, FileMode.Create))
                    //{
                    //    byte[] b = System.Text.Encoding.Default.GetBytes(ls_foot_D);
                    //    fs.Write(b, 0, b.Length);
                    //    fs.Close();
                    //    ls_foot_D = "";
                    //}
                    //----------------------------------------标志ID1--------------------------------------------------
                    string ls_foot_Base_E = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\includes\\modules\\payment\\YourSpay.php";
                    string ls_foot_New_E = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\includes\\modules\\payment\\YourSpay.php";
                    StreamReader fs_foot_E = new StreamReader(ls_foot_Base_E, System.Text.Encoding.UTF8);
                    string ls_foot_E = fs_foot_E.ReadToEnd();
                    fs_foot_E.Close();
                    ls_foot_E = ls_foot_E.Replace("checkout_payresult.php?page=302", "checkout_payresult.php?page=" + lc_code);
                    using (FileStream fs = File.Open(ls_foot_New_E, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_foot_E);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_foot_E = "";
                    }
                    //----------------------------------------标志ID3--------------------------------------------------
                    string ls_foot_Base_F = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\includes\\modules\\payment\\MYourSpay.php";
                    string ls_foot_New_F = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\includes\\modules\\payment\\MYourSpay.php";
                    StreamReader fs_foot_F = new StreamReader(ls_foot_Base_F, System.Text.Encoding.UTF8);
                    string ls_foot_F = fs_foot_F.ReadToEnd();
                    fs_foot_F.Close();
                    ls_foot_F = ls_foot_F.Replace(" 302", "checkout_payresult.php?page=" + lc_code);
                    using (FileStream fs = File.Open(ls_foot_New_F, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_foot_F);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_foot_F = "";
                    }
                    //--------------------------------------------End-------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //----------------------------------------其他文件替换--------------------------------------------
                    DataGroup group_list = new DataGroup();
                    string ls_list = "select id,local from PlanConfig_D where status=1";
                    group_list = DB.GetDataGroup(ls_list);
                    if (group_list.Table.Rows.Count > 0)
                    {
                        for (int j = 0; j < group_list.Table.Rows.Count; j++)
                        {
                            //----------------------------------------取配置--------------------------------------------------
                            string lc_local_fulldir = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + group_list.Table.Rows[j]["local"].ToString().Trim();
                            StreamReader fs_local_fulldir = new StreamReader(@lc_local_fulldir, System.Text.Encoding.UTF8);
                            string ls_local_fulldir = fs_local_fulldir.ReadToEnd();
                            fs_local_fulldir.Close();

                            ls_local_fulldir = ls_local_fulldir.Replace("nike",lc_kw_main);
                            ls_local_fulldir = ls_local_fulldir.Replace("x.com", lc_url_shot);
                            //ls_local_fulldir = ls_local_fulldir.Replace("UA-25806422-1", lc_ga);

                            using (FileStream fs = File.Open(lc_local_fulldir, FileMode.Create))
                            {
                                byte[] b = System.Text.Encoding.Default.GetBytes(ls_local_fulldir);
                                fs.Write(b, 0, b.Length);
                                fs.Close();
                                ls_local_fulldir = "";
                            }
                        }
                    }
                    //------------------------------------------End---------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                }
            }

            MessageBox.Show("OK!!");


        }

        /// <summary>
        /// 拷贝目录内容
        /// </summary>
        /// <param name="source">源目录</param>
        /// <param name="destination">目的目录</param>
        /// <param name="copySubDirs">是否拷贝子目录</param>
        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination, bool copySubDirs)
        {
            if (!destination.Exists)
            {
                destination.Create(); //目标目录若不存在就创建
            }
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(destination.FullName, file.Name), true); //复制目录中所有文件
            }
            if (copySubDirs)
            {
                DirectoryInfo[] dirs = source.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    string destinationDir = Path.Combine(destination.FullName, dir.Name);
                    CopyDirectory(dir, new DirectoryInfo(destinationDir), copySubDirs); //复制子目录
                }
            }
        }

        private void button143_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------------------------------------------------------------------------------
            //------------------------------------------------------------------------------------------------------------------------------------
            string ls_dir = t_dir.Text.Trim();
            if (ls_dir.Length < 12)
            {
                MessageBox.Show("新站域名的要正确！！！");
                return;
            }
            DirectoryInfo source = new DirectoryInfo("E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\");
            DirectoryInfo destination = new DirectoryInfo("E:\\PHP-1.5.6\\vhosts\\" + ls_dir + "\\");
            if (!destination.Exists)
            {
                
            }
            //else
            //{
            //    MessageBox.Show("域名已经有了，清醒点啊！！！");
            //    return;
            //}

            DataGroup group_config = new DataGroup();
            string ls_config = "SELECT id,code,dbname,dbuser,dbpassword,url,admin_dir,kw_main,TITLE,SITE_TAGLINE,CUSTOM_KEYWORDS,HOME_PAGE_META_DESCRIPTION,HOME_PAGE_META_KEYWORDS,HOME_PAGE_TITLE FROM PlanConfig " +
                " Where Status=1 and url='" + ls_dir + "'";

            group_config = DB.GetDataGroup(ls_config);
            if (group_config.Table.Rows.Count > 0)
            {
                
                DirectoryInfo dirimg = new DirectoryInfo("E:\\PHP-1.5.6\\vhosts\\" + ls_dir.Replace("www.","images.") + "\\");
                if (!dirimg.Exists)
                {
                    Directory.CreateDirectory("E:\\PHP-1.5.6\\vhosts\\" + ls_dir.Replace("www.", "images.") + "\\");
                }

                CopyDirectory(source, destination, true);

                        //------------------------------------------------------------------------------------------------------------------------------------
                        //------------------------------------------------------------------------------------------------------------------------------------
                        //------------------------------------------------------------------------------------------------------------------------------------
                        string ls_lan_text = "\r\n127.0.0.1       " + ls_dir + "\r\n" + "127.0.0.1       " + ls_dir.Replace("www", "images") + "\r\n";
                        string ls_lan = "C:\\WINDOWS\\system32\\drivers\\etc\\hosts";
                
                        StreamReader fs_is_ect = new StreamReader(ls_lan, System.Text.Encoding.UTF8);
                        string ls_is_ect = fs_is_ect.ReadToEnd();
                        fs_is_ect.Close();
                        if (ls_is_ect.IndexOf(ls_dir) == -1)
                        {
                            using (FileStream fs = File.Open(ls_lan, FileMode.Append))
                            {
                                byte[] b = System.Text.Encoding.Default.GetBytes(ls_lan_text);
                                fs.Write(b, 0, b.Length);
                                fs.Close();
                            }
                        }
                        //------------------------------------------------------------------------------------------------------------------------------------
                        //------------------------------------------------------------------------------------------------------------------------------------
                        string ls_php_text = "<VirtualHost *>\r\n" +
                             "    <Directory \"../vhosts/www." + ls_dir.Replace("www.", "") + "\">\r\n" +
                             "        Options -Indexes FollowSymLinks\r\n" +
                             "    </Directory>\r\n" +
                             "    ServerAdmin admin@www." + ls_dir.Replace("www.", "") + "\r\n" +
                             "    DocumentRoot \"../vhosts/www." + ls_dir.Replace("www.", "") + "\"\r\n" +
                             "    ServerName www." + ls_dir.Replace("www.", "") + ":80\r\n" +
                             "    ServerAlias *.www." + ls_dir.Replace("www.", "") + "\r\n" +
                             "    ErrorLog logs/www." + ls_dir.Replace("www.", "") + "-error_log\r\n" +
                             "    php_admin_value open_basedir \"E:\\PHP-1.5.6\\vhosts\\www." + ls_dir.Replace("www.", "") + ";C:\\WINDOWS\\Temp;\"\r\n" +
                             "</VirtualHost>\r\n\r\n" +
                             "<VirtualHost *>\r\n" +
                             "    <Directory \"../vhosts/images." + ls_dir.Replace("www.", "") + "\">\r\n" +
                             "        Options -Indexes FollowSymLinks\r\n" +
                             "    </Directory>\r\n" +
                             "    ServerAdmin admin@images." + ls_dir.Replace("www.", "") + "\r\n" +
                             "    DocumentRoot \"../vhosts/images." + ls_dir.Replace("www.", "") + "\"\r\n" +
                             "    ServerName images." + ls_dir.Replace("www.", "") + ":80\r\n" +
                             "    ServerAlias *.images." + ls_dir.Replace("www.", "") + "\r\n" +
                             "    ErrorLog logs/images." + ls_dir.Replace("www.", "") + "-error_log\r\n" +
                             "    php_admin_value open_basedir \"E:\\PHP-1.5.6\\vhosts\\images." + ls_dir.Replace("www.", "") + ";C:\\WINDOWS\\Temp;\"\r\n" +
                             "</VirtualHost>\r\n\r\n";
                        //------------------------------------------------------------------------------------------------------------------------------------
                        string ls_php = "E:\\PHP-1.5.6\\Apache-20\\conf\\extra\\httpd-vhosts.conf";
                        StreamReader fs_is_php = new StreamReader(ls_php, System.Text.Encoding.UTF8);
                        string ls_is_php = fs_is_php.ReadToEnd();
                        fs_is_php.Close();
                        if (ls_is_php.IndexOf(ls_dir) == -1)
                        {
                            using (FileStream fs = File.Open(ls_php, FileMode.Append))
                            {
                                byte[] b = System.Text.Encoding.Default.GetBytes(ls_php_text);
                                fs.Write(b, 0, b.Length);
                                fs.Close();
                            }
                        }
                        //------------------------------------------------------------------------------------------------------------------------------------
                        RestartService("Apache_pn", 3000);
                        //------------------------------------------------------------------------------------------------------------------------------------
                        //------------------------------------------------------------------------------------------------------------------------------------

                        string ls_db = "";
                        string ls_url = "";

                //-----------------------------------------------------------------------------------------------------------------------------------
                for (int i = 0; i < group_config.Table.Rows.Count; i++)
                {
                    //----------------------------------------取配置--------------------------------------------------
                    string lc_code = group_config.Table.Rows[i]["code"].ToString().Trim();
                    string lc_dbname = group_config.Table.Rows[i]["dbname"].ToString().Trim();
                    ls_db = lc_dbname;
                    string lc_dbuser = group_config.Table.Rows[i]["dbuser"].ToString().Trim();
                    string lc_dbpassword = group_config.Table.Rows[i]["dbpassword"].ToString().Trim();
                    string lc_url = group_config.Table.Rows[i]["url"].ToString().Trim();
                    ls_url = lc_url;
                    string lc_url_shot = group_config.Table.Rows[i]["url"].ToString().Replace("www.", "").Trim();
                    string lc_admin_dir = group_config.Table.Rows[i]["admin_dir"].ToString().Trim();
                    string lc_kw_main = group_config.Table.Rows[i]["kw_main"].ToString().Trim();
                    string lc_TITLE = group_config.Table.Rows[i]["TITLE"].ToString().Trim();
                    string lc_SITE_TAGLINE = group_config.Table.Rows[i]["SITE_TAGLINE"].ToString().Trim();
                    string lc_CUSTOM_KEYWORDS = group_config.Table.Rows[i]["CUSTOM_KEYWORDS"].ToString().Trim();
                    string lc_HOME_PAGE_META_DESCRIPTION = group_config.Table.Rows[i]["HOME_PAGE_META_DESCRIPTION"].ToString().Trim();
                    string lc_HOME_PAGE_META_KEYWORDS = group_config.Table.Rows[i]["HOME_PAGE_META_KEYWORDS"].ToString().Trim();
                    string lc_HOME_PAGE_TITLE = group_config.Table.Rows[i]["HOME_PAGE_TITLE"].ToString().Trim();
                    //------------------------------------------------------------------------------------------------
                    //--------------------------------------sql主key替换----------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    string ls_sqlfile_Base = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\150L302B.sql";
                    string ls_sqlfile_New = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\150LB.sql";
                    StreamReader fs_sqlfile = new StreamReader(ls_sqlfile_Base, System.Text.Encoding.UTF8);
                    string ls_sqlfile_Text = fs_sqlfile.ReadToEnd();
                    fs_sqlfile.Close();
                    ls_sqlfile_Text = ls_sqlfile_Text.Replace("'nike'", "'" + lc_kw_main + "'").Replace("x.com", lc_url_shot);
                    using (FileStream fs = File.Open(ls_sqlfile_New, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_sqlfile_Text);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_sqlfile_Text = "";
                    }
                    //------------------------------------------END---------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //---------------------------------------2个配置文件替换------------------------------------------
                    string ls_configure_Base = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\includes\\configure.php";
                    string ls_configure_New = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\includes\\configure.php";
                    StreamReader fs_configure = new StreamReader(ls_configure_Base, System.Text.Encoding.UTF8);
                    string ls_configure = fs_configure.ReadToEnd();
                    fs_configure.Close();
                    //【标准站】替换为【新站】
                    ls_configure = ls_configure.Replace("x.com", lc_url_shot);
                    //【数据库名称】替换
                    ls_configure = ls_configure.Replace("define('DB_DATABASE', '150L302');", "define('DB_DATABASE', '" + lc_dbname + "');");
                    //【数据库用户名】
                    //ls_configure = ls_configure.Replace("define('DB_SERVER_USERNAME', 'root');", "define('DB_SERVER_USERNAME', '" + lc_dbuser + "');");
                    //【旧密码】替换为【新密码】
                    //ls_configure = ls_configure.Replace("define('DB_SERVER_PASSWORD', 'admin');", "define('DB_SERVER_PASSWORD', '" + lc_dbpassword + "');");
                    //【旧路径】替换为【新路径】
                    //ls_configure = ls_configure.Replace("define('DIR_FS_CATALOG', 'E:/PHP-1.5.6/vhosts//", "define('DIR_FS_CATALOG', '/home/wwwroot/");
                    //ls_configure = ls_configure.Replace("define('DIR_FS_SQL_CACHE', 'E:/PHP-1.5.6/vhosts/", "define('DIR_FS_SQL_CACHE', '/home/wwwroot/");

                    System.IO.File.SetAttributes(ls_configure_New, System.IO.FileAttributes.Normal);

                    using (FileStream fs = File.Open(ls_configure_New, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_configure);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_configure = "";
                    }
                    //------------------------------------------------------------------------------------------------
                    string ls_configure_admin_Base = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\xing\\includes\\configure.php";
                    string ls_configure_admin_New = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\xing\\includes\\configure.php";
                    StreamReader fs_configure_admin = new StreamReader(ls_configure_admin_Base, System.Text.Encoding.UTF8);
                    string ls_configure_admin = fs_configure_admin.ReadToEnd();
                    fs_configure_admin.Close();
                    //【标准站】替换为【新站】
                    ls_configure_admin = ls_configure_admin.Replace("x.com", lc_url_shot);
                    //【数据库名称】替换
                    ls_configure_admin = ls_configure_admin.Replace("define('DB_DATABASE', '150L302');", "define('DB_DATABASE', '" + lc_dbname + "');");
                    //【数据库用户名】
                    //ls_configure_admin = ls_configure_admin.Replace("define('DB_SERVER_USERNAME', 'root');", "define('DB_SERVER_USERNAME', '" + lc_dbuser + "');");
                    //【旧密码】替换为【新密码】
                    //ls_configure_admin = ls_configure_admin.Replace("define('DB_SERVER_PASSWORD', 'admin');", "define('DB_SERVER_PASSWORD', '" + lc_dbpassword + "');");
                    //【旧路径】替换为【新路径】
                    //ls_configure_admin = ls_configure_admin.Replace("define('DIR_FS_CATALOG', 'E:/PHP-1.5.6/vhosts//", "define('DIR_FS_CATALOG', '/home/wwwroot/");
                    //ls_configure_admin = ls_configure_admin.Replace("define('DIR_FS_SQL_CACHE', 'E:/PHP-1.5.6/vhosts/", "define('DIR_FS_SQL_CACHE', '/home/wwwroot/");
                    using (FileStream fs = File.Open(ls_configure_admin_New, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_configure_admin);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_configure_admin = "";
                    }
                    //------------------------------------------END---------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                    //------------------------------------------------------------------------------------------------
                }


                //mysql -u root -padmin
                //create database 139L;
                //use 139L
                
                string ls_bat_dir = "C:\\Documents and Settings\\Administrator\\桌面\\a.bat";
                string ls_bat_Text = "path E:\\PHP-1.5.6\\MySQL-5.0.90\\bin\r\nmysql -u root -padmin <b.sql";
                using (FileStream fs = File.Open(ls_bat_dir, FileMode.Create))
                {
                    byte[] b = System.Text.Encoding.Default.GetBytes(ls_bat_Text);
                    fs.Write(b, 0, b.Length);
                    fs.Close();
                    ls_bat_Text = "";
                }


                string ls_mysql_dir = "C:\\Documents and Settings\\Administrator\\桌面\\b.sql";
                //string ls_mysql_Text = "drop database " + ls_db + ";\r\ncreate database " + ls_db + ";\r\nuse " + ls_db + "\r\nsource E:/PHP-1.5.6/vhosts/" + ls_url + "/150LB.sql;";
                string ls_mysql_Text = "create database " + ls_db + ";\r\nuse " + ls_db + "\r\nsource E:/PHP-1.5.6/vhosts/" + ls_url + "/150LB.sql;";
                using (FileStream fs = File.Open(ls_mysql_dir, FileMode.Create))
                {
                    byte[] b = System.Text.Encoding.Default.GetBytes(ls_mysql_Text);
                    fs.Write(b, 0, b.Length);
                    fs.Close();
                    ls_mysql_Text = "";
                }


                MessageBox.Show("恭喜恭喜，成功复制！ \r\n 亲 下一步手动执行下bat文件哦");
            }
           //------------------------------------------------------------------------------------------------------------------------------------
           //------------------------------------------------------------------------------------------------------------------------------------
           //------------------------------------------------------------------------------------------------------------------------------------
        }

        public static void RestartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // ...
            }
        }


        public string FiltPhone(string phoneold)
        {
            string phonenew = "";

            Regex Expression = new Regex("13[0-9]{9}|15[0-9]{9}");
            Match match = Expression.Match(phoneold, 0, phoneold.Length);
            if (match.Success)
                return phonenew = phoneold.Substring(match.Index, 11);
            else
                return "";

        }

        public string IsGucci(string phoneold)
        {
            string phonenew = "";
            Regex Expression = new Regex("28[0-9]{4}|27[0-9]{4}|26[0-9]{4}|25[0-9]{4}|24[0-9]{4}|23[0-9]{4}|22[0-9]{4}");
            Match match = Expression.Match(phoneold, 0, phoneold.Length);
            if (match.Success)
                return phonenew = phoneold.Substring(match.Index, 6);
            else
                return "";
        }

        private void button141_Click(object sender, EventArgs e)
        {

            //----------------预处理名称-----------------
            string ls_init1 = @"

update plan425_b2 set main_class=rtrim(ltrim(replace(main_class,'''','')))
update plan425_b2 set sub_class=rtrim(ltrim(replace(sub_class,'''','')))
            ";
            DB.ExecuteSQL(ls_init1);

            DataGroup group = new DataGroup();
            string ls_group_main_class = "select  id,/**/product_name as pname_OK,main_class,sub_class from plan425_B2 where brand='x'  ";
            group = DB.GetDataGroup(ls_group_main_class);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    //string aaaa = group.Table.Rows[x]["main_class"].ToString().Replace("/", " ").Replace("  ", " ").Trim();
                    //string bbbb = group.Table.Rows[x]["sub_class"].ToString().Replace("/", " ").Replace("  ", " ").Trim();
                    string cccc = group.Table.Rows[x]["pname_OK"].ToString().Replace("\n", " ").Trim();
                    string dddd = group.Table.Rows[x]["main_class"].ToString().Replace("\n", " ").Trim();
                    string eeee = group.Table.Rows[x]["sub_class"].ToString().Replace("\n", " ").Trim();

                    string ls_do = "";
                    string ls_do2 = "";
                    string ls_do3 = "";

                    if (cccc.IndexOf(" ") > -1)
                    {
                        string[] dog_small = cccc.Split(' ');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = FormatString(bb.Trim());

                            if (OK_bb.Length > 1)
                            {
                                if (OK_bb.Length == 2)
                                {
                                    OK_bb = OK_bb.ToUpper();
                                }

                                string pp = " " + OK_bb + " ";
                                if (ls_do.IndexOf(pp) > -1)
                                {

                                }
                                else
                                {
                                    ls_do += " " + OK_bb + " ";
                                }
                            }

                        }

                    }

                    if (dddd.IndexOf(" ") > -1)
                    {
                        string[] dog_small2 = dddd.Split(' ');//子串
                        foreach (string bb in dog_small2)
                        {
                            string OK_bb = FormatString(bb.Trim());

                            if (OK_bb.Length > 1)
                            {
                                if (OK_bb.Length == 2)
                                {
                                    OK_bb = OK_bb.ToUpper();
                                }

                                //string pp = " " + OK_bb + " ";
                                //if (ls_do2.IndexOf(pp) > -1)
                                //{

                                //}
                                //else
                                //{
                                ls_do2 += " " + OK_bb + " ";
                                //}
                            }
                        }
                    }
                    else
                    {
                        ls_do2 = FormatString(dddd);

                    }

                    if (eeee.IndexOf(" ") > -1)
                    {
                        string[] dog_small3 = eeee.Split(' ');//子串
                        foreach (string bb in dog_small3)
                        {
                            string OK_bb = FormatString(bb.Trim());

                            if (OK_bb.Length > 1)
                            {
                                if (OK_bb.Length == 2)
                                {
                                    OK_bb = OK_bb.ToUpper();
                                }

                                //string pp = " " + OK_bb + " ";
                                //if (ls_do3.IndexOf(pp) > -1)
                                //{

                                //}
                                //else
                                //{
                                ls_do3 += " " + OK_bb + " ";
                                //}
                            }
                        }
                    }
                    else
                    {
                        ls_do3 = FormatString(eeee);

                    }
                    string ls_up = "update plan425_B2 set product_name='" + ls_do.Replace("  ", " ").Trim() + "',main_class='" + ls_do2.Replace("  ", " ").Trim() + "',sub_class='" + ls_do3.Replace("  ", " ").Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'";
                    DB.ExecuteSQL(ls_up);

                }
            }






            //-----------------LV编码生成---------------
            DataGroup group_lv = new DataGroup();
            group_lv = null;
            string ls_sql_lv = "SELECT id,product_name,description FROM plan425_b2 where brand='x' and status=2 ";
            group_lv = DB.GetDataGroup(ls_sql_lv);
            if (group_lv.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_lv.Table.Rows.Count; i++)
                {
                    string ls_ppp = IsLV(group_lv.Table.Rows[i]["product_name"].ToString());

                    string ls_up_lv = "update plan425_b2 set code='" + ls_ppp + "'  where id='" + group_lv.Table.Rows[i]["ID"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up_lv);
                }
            }


            string ls_up2 = "update plan425_b2 set status=0 where brand='x' and code not in  (select code from plan42510 where flag='OK') ";
            DB.ExecuteSQL(ls_up2);


            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            DataGroup group_check_cc = new DataGroup();
            group_check_cc = null;
            DataGroup group_cc_good = new DataGroup();
            group_cc_good = null;
            string ls_cc_good = "select ' union select id from plan425_b2 where product_name like ''%'+code+'%''' as code from PlanCC10 where flag='OK' ";
            string ls_full_sql_cc = "";
            string ls_gid_cc = "0";
            group_cc_good = DB.GetDataGroup(ls_cc_good);
            if (group_cc_good.Table.Rows.Count > 0)
            {
                //---------------------------------------------------------------------------
                for (int i = 0; i < group_cc_good.Table.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        ls_full_sql_cc = group_cc_good.Table.Rows[i]["code"].ToString().Replace("union ", "");
                    }
                    else
                    {
                        ls_full_sql_cc += group_cc_good.Table.Rows[i]["code"].ToString();
                    }
                }
                group_check_cc = null;
                group_check_cc = DB.GetDataGroup(ls_full_sql_cc);
                if (group_check_cc.Table.Rows.Count > 0)
                {
                    for (int j = 0; j < group_check_cc.Table.Rows.Count; j++)
                    {
                        ls_gid_cc += "," + group_check_cc.Table.Rows[j]["ID"].ToString();
                    }
                }
                group_check_cc = null;
                //---------------------------------------------------------------------------
            }
            string ls_up2_cc = "update plan425_b2 set status=0 where id not in (" + ls_gid_cc + ")  /**/ and ( main_class like '%chanel%'  or sub_class like '%chanel%' );update plan425_b2 set status=2 where  main_class like '%chanel%' and sub_class like '%2012%'; ";
            DB.ExecuteSQL(ls_up2_cc);
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------


            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            DataGroup group_check_Pd = new DataGroup();
            group_check_Pd = null;

            DataGroup group_Pd_good = new DataGroup();
            group_Pd_good = null;
            string ls_Pd_good = "select ' union select id from plan425_b2 where product_name like ''%'+code+'%''' as code from PlanPd10 where flag='OK' ";

            string ls_full_sql_Pd = "";
            string ls_gid_Pd = "0";

            group_Pd_good = DB.GetDataGroup(ls_Pd_good);
            if (group_Pd_good.Table.Rows.Count > 0)
            {
                //---------------------------------------------------------------------------
                for (int i = 0; i < group_Pd_good.Table.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        ls_full_sql_Pd = group_Pd_good.Table.Rows[i]["code"].ToString().Replace("union ", "");
                    }
                    else
                    {
                        ls_full_sql_Pd += group_Pd_good.Table.Rows[i]["code"].ToString();
                    }
                }
                group_check_Pd = null;
                group_check_Pd = DB.GetDataGroup(ls_full_sql_Pd);
                if (group_check_Pd.Table.Rows.Count > 0)
                {
                    for (int j = 0; j < group_check_Pd.Table.Rows.Count; j++)
                    {
                        ls_gid_Pd += "," + group_check_Pd.Table.Rows[j]["ID"].ToString();
                    }
                }
                group_check_Pd = null;
                //---------------------------------------------------------------------------
            }
            string ls_up2_Pd = "update plan425_b2 set status=0 where id not in (" + ls_gid_Pd + ")  /**/ and ( main_class like '%Prada%'  or sub_class like '%Prada%' );";
            DB.ExecuteSQL(ls_up2_Pd);
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------







            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            DataGroup group_check_Fi = new DataGroup();
            group_check_Fi = null;

            DataGroup group_Fi_good = new DataGroup();
            group_Fi_good = null;
            string ls_Fi_good = "select ' union select id from plan425_b2 where product_name like ''%'+code+'%''' as code from PlanFi10 where flag='OK' ";

            string ls_full_sql_Fi = "";
            string ls_gid_Fi = "0";

            group_Fi_good = DB.GetDataGroup(ls_Fi_good);
            if (group_Fi_good.Table.Rows.Count > 0)
            {
                //---------------------------------------------------------------------------
                for (int i = 0; i < group_Fi_good.Table.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        ls_full_sql_Fi = group_Fi_good.Table.Rows[i]["code"].ToString().Replace("union ", "");
                    }
                    else
                    {
                        ls_full_sql_Fi += group_Fi_good.Table.Rows[i]["code"].ToString();
                    }
                }
                group_check_Fi = null;
                group_check_Fi = DB.GetDataGroup(ls_full_sql_Fi);
                if (group_check_Fi.Table.Rows.Count > 0)
                {
                    for (int j = 0; j < group_check_Fi.Table.Rows.Count; j++)
                    {
                        ls_gid_Fi += "," + group_check_Fi.Table.Rows[j]["ID"].ToString();
                    }
                }
                group_check_Fi = null;
                //---------------------------------------------------------------------------
            }
            string ls_up2_Fi = "update plan425_b2 set status=0 where id not in (" + ls_gid_Fi + ")  /**/ and ( main_class like '%Fendi%'  or sub_class like '%Fendi%' );";
            DB.ExecuteSQL(ls_up2_Fi);
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------






            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            DataGroup group_check_Mi = new DataGroup();
            group_check_Mi = null;

            DataGroup group_Mi_good = new DataGroup();
            group_Mi_good = null;
            string ls_Mi_good = "select ' union select id from plan425_b2 where product_name like ''%'+code+'%''' as code from PlanMi10 where flag='OK' ";

            string ls_full_sql_Mi = "";
            string ls_gid_Mi = "0";

            group_Mi_good = DB.GetDataGroup(ls_Mi_good);
            if (group_Mi_good.Table.Rows.Count > 0)
            {
                //---------------------------------------------------------------------------
                for (int i = 0; i < group_Mi_good.Table.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        ls_full_sql_Mi = group_Mi_good.Table.Rows[i]["code"].ToString().Replace("union ", "");
                    }
                    else
                    {
                        ls_full_sql_Mi += group_Mi_good.Table.Rows[i]["code"].ToString();
                    }
                }
                group_check_Mi = null;
                group_check_Mi = DB.GetDataGroup(ls_full_sql_Mi);
                if (group_check_Mi.Table.Rows.Count > 0)
                {
                    for (int j = 0; j < group_check_Mi.Table.Rows.Count; j++)
                    {
                        ls_gid_Mi += "," + group_check_Mi.Table.Rows[j]["ID"].ToString();
                    }
                }
                group_check_Mi = null;
                //---------------------------------------------------------------------------
            }
            string ls_up2_Mi = "update plan425_b2 set status=0 where id not in (" + ls_gid_Mi + ")  /**/ and ( main_class like '%Miu%'  or sub_class like '%Miu%' );";
            DB.ExecuteSQL(ls_up2_Mi);
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------





            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            DataGroup group_check_CHOLE = new DataGroup();
            group_check_CHOLE = null;

            DataGroup group_CHOLE_good = new DataGroup();
            group_CHOLE_good = null;
            string ls_CHOLE_good = "select ' union select id from plan425_b2 where product_name like ''%'+code+'%''' as code from PlanCHOLE10 where flag='OK' ";

            string ls_full_sql_CHOLE = "";
            string ls_gid_CHOLE = "0";

            group_CHOLE_good = DB.GetDataGroup(ls_CHOLE_good);
            if (group_CHOLE_good.Table.Rows.Count > 0)
            {
                //---------------------------------------------------------------------------
                for (int i = 0; i < group_CHOLE_good.Table.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        ls_full_sql_CHOLE = group_CHOLE_good.Table.Rows[i]["code"].ToString().Replace("union ", "");
                    }
                    else
                    {
                        ls_full_sql_CHOLE += group_CHOLE_good.Table.Rows[i]["code"].ToString();
                    }
                }
                group_check_CHOLE = null;
                group_check_CHOLE = DB.GetDataGroup(ls_full_sql_CHOLE);
                if (group_check_CHOLE.Table.Rows.Count > 0)
                {
                    for (int j = 0; j < group_check_CHOLE.Table.Rows.Count; j++)
                    {
                        ls_gid_CHOLE += "," + group_check_CHOLE.Table.Rows[j]["ID"].ToString();
                    }
                }
                group_check_CHOLE = null;
                //---------------------------------------------------------------------------
            }
            string ls_up2_CHOLE = "update plan425_b2 set status=0 where id not in (" + ls_gid_CHOLE + ")  /**/ and ( main_class like '%CHOLE%'  or sub_class like '%CHOLE%' );";
            DB.ExecuteSQL(ls_up2_CHOLE);
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------



            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            DataGroup group_Gu_good = new DataGroup();
            string ls_Gu_good = "select id,product_name  from plan425_b2 /**/ where main_class like '%Gucci%'  or sub_class like '%Gucci%' ";
            group_Gu_good = null;
            group_Gu_good = DB.GetDataGroup(ls_Gu_good);
            if (group_Gu_good.Table.Rows.Count > 0)
            {
                //---------------------------------------------------------------------------
                for (int i = 0; i < group_Gu_good.Table.Rows.Count; i++)
                {
                    string ls_is_gucci = IsGucci(group_Gu_good.Table.Rows[i]["product_name"].ToString());

                    //string ls_is_gucci_code = ls_is_gucci;

                    if (ls_is_gucci == "")
                    {
                        string ls_is_gucci_sql = "update plan425_b2 set status=0 where id ='" + group_Gu_good.Table.Rows[i]["id"].ToString() + "' ";
                        DB.ExecuteSQL(ls_is_gucci_sql);
                    }
                    else
                    {
                        string ls_is_gucci_sql = "update plan425_b2 set code='" + ls_is_gucci + "' where id ='" + group_Gu_good.Table.Rows[i]["id"].ToString() + "' ";
                        DB.ExecuteSQL(ls_is_gucci_sql);
                    }
                }
            }
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------
            //---------------------------------------------------------------------------




            //---------------------------------------------------------------------------
            DataGroup group_html = new DataGroup();
            group_html = null;
            DataGroup group_check2 = new DataGroup();
            group_check2 = null;
            string ls_sql = "select id,brand,words from PlanConfig_Black where status=1";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_brand = group_html.Table.Rows[i]["brand"].ToString().Trim();
                    string ls_words = group_html.Table.Rows[i]["words"].ToString().Trim();

                    string ls_check = "select id from plan425_b2 where (main_class like '%" + ls_brand + "%' or sub_class like '%" + ls_brand + "%') and product_name like '%" + ls_words + "%' ";
                    group_check2 = DB.GetDataGroup(ls_check);
                    if (group_check2.Table.Rows.Count > 0)
                    {
                        for (int j = 0; j < group_check2.Table.Rows.Count; j++)
                        {
                            string ls_up33 = "update plan425_b2 set status=0 where id='" + group_check2.Table.Rows[j]["ID"].ToString() + "' ";
                            DB.ExecuteSQL(ls_up33);
                        }
                    }
                }
            }
            /**/


            MessageBox.Show("过滤缺货OK，最好目测下！");
        }

        private void button144_Click(object sender, EventArgs e)
        {


            MessageBox.Show("小SQLOK！");


        }

        private void button144_Click_1(object sender, EventArgs e)
        {
            string ls_up1 = @"  
update plan425_b2 set pname1=product_name;
update plan425_b2 set pname_OK=pname1

update plan425_b2 set description=replace(description,' (1cm≈0.394 inch)','')
update plan425_b2 set description=replace(description,'(1cm≈0.394 inch)','')
update plan425_b2 set description=replace(description,'?','')

--update plan425_b2 set main_class=(select main_class from plan425_b1 where id=plan425_b2.pid)
--update plan425_b2 set sub_class=(select sub_class from plan425_b1 where id=plan425_b2.pid)

update plan425_b2 set main_class=replace(main_class,'-',' ')
update plan425_b2 set main_class=replace(main_class,'-',' ')

update plan425_b2 set main_class=replace(main_class,'ˉ','')
update plan425_b2 set sub_class=replace(sub_class,'ˉ','')
update plan425_b2 set pname_OK=replace(pname_OK,'ˉ','')

update plan425_b2 set main_class=replace(main_class,'“','')
update plan425_b2 set main_class=replace(main_class,'”','')
update plan425_b2 set sub_class=replace(sub_class,'“','')
update plan425_b2 set sub_class=replace(sub_class,'”','')

update plan425_b2 set main_class=replace(main_class,'_','')
update plan425_b2 set sub_class=replace(sub_class,'_','')

update plan425_b2 set main_class=replace(main_class,'“','')

update plan425_b2 set main_class=replace(main_class,'''','')
update plan425_b2 set sub_class=replace(sub_class,'''','')

update plan425_b2 set sub_class=replace(sub_class,'’','')

update plan425_b2 set main_class='' where main_class is null;
update plan425_b2 set sub_class='' where sub_class is null;

--update plan425_b2 set pname_OK=main_class + ' '+sub_class+' '+pname_OK where len(pname_OK) <20

--select * from plan425_b2 where len(pname_OK) <20

update plan425_b2 set pname_OK=replace(pname_OK,'<br>','')
update plan425_b2 set pname_OK=replace(pname_OK,'<br/>','')
update plan425_b2 set pname_OK=replace(pname_OK,'''','')

update plan425_b2 set pname_OK=replace(pname_OK,'designer','')

update plan425_b2 set pname_OK=replace(pname_OK,'Wholesale','')
update plan425_b2 set pname_OK=replace(pname_OK,'replica','')
update plan425_b2 set pname_OK=replace(pname_OK,'relica','')
update plan425_b2 set pname_OK=replace(pname_OK,'cheapest','')
update plan425_b2 set pname_OK=replace(pname_OK,'cheap','')

update plan425_b2 set pname_OK=replace(pname_OK,char(13),' ')

update plan425_b2 set pname_OK=replace(pname_OK,'–',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'[','')
update plan425_b2 set pname_OK=replace(pname_OK,']','')
update plan425_b2 set pname_OK=replace(pname_OK,'=','')
update plan425_b2 set pname_OK=replace(pname_OK,'$','')

update plan425_b2 set pname_OK=replace(pname_OK,',',' ')

update plan425_b2 set pname_OK=replace(pname_OK,'“','')
update plan425_b2 set pname_OK=replace(pname_OK,'”','')
update plan425_b2 set description=replace(description,'”','')

update plan425_b2 set pname_OK=replace(pname_OK,'&#039;','')
update plan425_b2 set pname_OK=replace(pname_OK,'&amp;','')
update plan425_b2 set pname_OK=replace(pname_OK,'&quot;','')
update plan425_b2 set pname_OK=replace(pname_OK,'/',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'\','')
update plan425_b2 set pname_OK=replace(pname_OK,'+',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'_','')

update plan425_b2 set pname_OK=replace(pname_OK,'’',' ')
update plan425_b2 set pname_OK=replace(pname_OK,',',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'?','')
update plan425_b2 set pname_OK=replace(pname_OK,'/',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'-',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'è',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'(',' ')
update plan425_b2 set pname_OK=replace(pname_OK,')',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'’ ',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'&',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'.',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'#','')
update plan425_b2 set pname_OK=replace(pname_OK,'*','')
update plan425_b2 set pname_OK=replace(pname_OK,';','')

update plan425_b2 set pname_OK=replace(pname_OK,'nbsp','')
update plan425_b2 set pname_OK=replace(pname_OK,'Y S L','YSL')

update plan425_b2 set price=replace(price,'.00','')
update plan425_b2 set price=replace(price,'$','')

update plan425_b2 set price=replace(price,',','')
update plan425_b2 set price=floor(price)

--update plan425_b2 set pname1=replace(pname1,'cm','CM ')
--delete plan425_b2 where pname_OK is null or pname_OK =''

--update plan425_b2 set bigimghtml=replace(bigimghtml,'é','e')
update plan425_b2 set pname_OK=replace(pname_OK,'é','e')

update plan425_b2 set description=replace(description,' X ',' * ')
update plan425_b2 set description=replace(description,'×',' * ')




--delete from plan425_b2 where 
--(select count(*) from plan425_pic where /* status !=0  and*/ plan425_b2.id=plan425_pic.pid ) =0

update plan425_b2 set status=0 where status=2 and id not in 
(select min(id) from plan425_b2 group by url_product)

update  plan425_b2  set status=0 where  status=2 and
(select count(*) from plan425_pic where  status !=0  and  plan425_b2.id=plan425_pic.pid ) =0


update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')

update plan425_b2 set pname_OK=rtrim(ltrim(pname_OK))



";

            string ls_up2 = @"  

update plan425_b2 set pname1=pname_OK

--update plan425_b2 set pname_OK=replace(pname_OK,'CM','CM ')
update plan425_b2 set pname_OK=replace(pname_OK,'100%','yyKKb')
update plan425_b2 set pname_OK=replace(pname_OK,'100*100cm','yyKKc')
update plan425_b2 set pname_OK=replace(pname_OK,'200*65cm','yyKKd')
update plan425_b2 set pname_OK=replace(pname_OK,'190*46cm','yyKKe')
update plan425_b2 set pname_OK=replace(pname_OK,'190*22cm','yyKKf')
update plan425_b2 set pname_OK=replace(pname_OK,'165*33cm','yyKKg')
update plan425_b2 set pname_OK=replace(pname_OK,'170*65cm','yyKKh')
update plan425_b2 set pname_OK=replace(pname_OK,'160*50cm','yyKKj')
update plan425_b2 set pname_OK=replace(pname_OK,'85*85cm','yyKKk')
update plan425_b2 set pname_OK=replace(pname_OK,'180*65cm','yyKKl')
update plan425_b2 set pname_OK=replace(pname_OK,'180*25cm','yyKKm')


update plan425_b2 set pname_OK=replace(pname_OK,'3.1','xxKKL')
update plan425_b2 set pname_OK=replace(pname_OK,'140CM','xxKKM')
update plan425_b2 set pname_OK=replace(pname_OK,'100CM','xxKKN')
update plan425_b2 set pname_OK=replace(pname_OK,'33CM','xxKKO')
update plan425_b2 set pname_OK=replace(pname_OK,'65CM','xxKKP')
update plan425_b2 set pname_OK=replace(pname_OK,'46CM','xxKKQ')
update plan425_b2 set pname_OK=replace(pname_OK,'33CM','xxKKR')
update plan425_b2 set pname_OK=replace(pname_OK,'85CM','xxKKT')

update plan425_b2 set pname_OK=replace(pname_OK,'2.55','KKKKK')
update plan425_b2 set pname_OK=replace(pname_OK,'60CM','KKKKL')
update plan425_b2 set pname_OK=replace(pname_OK,'55CM','KKKKM')
update plan425_b2 set pname_OK=replace(pname_OK,'50CM','KKKKN')
update plan425_b2 set pname_OK=replace(pname_OK,'45CM','KKKKO')
update plan425_b2 set pname_OK=replace(pname_OK,'40CM','KKKKP')
update plan425_b2 set pname_OK=replace(pname_OK,'35CM','KKKKQ')
update plan425_b2 set pname_OK=replace(pname_OK,'25CM','KKKKR')
update plan425_b2 set pname_OK=replace(pname_OK,'20CM','KKKKS')
update plan425_b2 set pname_OK=replace(pname_OK,'32CM','KKKKT')
update plan425_b2 set pname_OK=replace(pname_OK,'22CM','KKKKU')
update plan425_b2 set pname_OK=replace(pname_OK,'30CM','KKKLC')

update plan425_b2 set pname_OK=replace(pname_OK,'34CM','KKKKV')
update plan425_b2 set pname_OK=replace(pname_OK,'42CM','KKKKW')


update plan425_b2 set pname_OK=replace(pname_OK,' 925 ','yyKKa')



update plan425_b2 set pname_OK=replace(pname_OK,'4KEY','KKKKX')
update plan425_b2 set pname_OK=replace(pname_OK,'6KEY','KKKKY')
update plan425_b2 set pname_OK=replace(pname_OK,'4 KEY','KKKLA')
update plan425_b2 set pname_OK=replace(pname_OK,'6 KEY','KKKLB')

update plan425_b2 set pname_OK=replace(pname_OK,' 45 ',' xxKKA ')
update plan425_b2 set pname_OK=replace(pname_OK,' 35 ',' xxKKB ')
update plan425_b2 set pname_OK=replace(pname_OK,' 25 ',' xxKKC ')
update plan425_b2 set pname_OK=replace(pname_OK,'2010','KKKKD')
update plan425_b2 set pname_OK=replace(pname_OK,'2011','KKKKE')
update plan425_b2 set pname_OK=replace(pname_OK,' 40 ',' xxKKF ')
update plan425_b2 set pname_OK=replace(pname_OK,' 30 ',' xxKKG ')
update plan425_b2 set pname_OK=replace(pname_OK,' 50 ',' xxKKH ')
update plan425_b2 set pname_OK=replace(pname_OK,' 55 ',' xxKKI ')
update plan425_b2 set pname_OK=replace(pname_OK,' 60 ',' xxKKJ ')
update plan425_b2 set pname_OK=replace(pname_OK,' 65 ',' xxKKU ')



update plan425_b2 set pname_OK=replace(pname_OK,' xxKKA ',' 45 ')
update plan425_b2 set pname_OK=replace(pname_OK,' xxKKB ',' 35 ')
update plan425_b2 set pname_OK=replace(pname_OK,' xxKKC ',' 25 ')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKD','2010')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKE','2011')
update plan425_b2 set pname_OK=replace(pname_OK,' xxKKF ',' 40 ')
update plan425_b2 set pname_OK=replace(pname_OK,' xxKKG ',' 30 ')
update plan425_b2 set pname_OK=replace(pname_OK,' xxKKH ',' 50 ')
update plan425_b2 set pname_OK=replace(pname_OK,' xxKKI ',' 55 ')
update plan425_b2 set pname_OK=replace(pname_OK,' xxKKJ ',' 60 ')
update plan425_b2 set pname_OK=replace(pname_OK,' xxKKU ',' 65 ')

update plan425_b2 set pname_OK=replace(pname_OK,'KKKKK','2.55')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKL','60CM')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKM','55CM')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKN','50CM')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKO','45CM')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKP','40CM')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKQ','35CM')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKR','25CM')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKS','20CM')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKT','32CM')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKU','22CM')

update plan425_b2 set pname_OK=replace(pname_OK,'KKKKV','34CM')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKW','42CM')

update plan425_b2 set pname_OK=replace(pname_OK,'KKKKX','4KEY')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKKY','6KEY')

update plan425_b2 set pname_OK=replace(pname_OK,'KKKLA','4 KEY')
update plan425_b2 set pname_OK=replace(pname_OK,'KKKLB','6 KEY')

update plan425_b2 set pname_OK=replace(pname_OK,'KKKLC','30CM')

update plan425_b2 set pname_OK=replace(pname_OK,'xxKKL','3.1')
update plan425_b2 set pname_OK=replace(pname_OK,'xxKKM','140CM')
update plan425_b2 set pname_OK=replace(pname_OK,'xxKKN','100CM')
update plan425_b2 set pname_OK=replace(pname_OK,'xxKKO','33CM')
update plan425_b2 set pname_OK=replace(pname_OK,'xxKKP','65CM')
update plan425_b2 set pname_OK=replace(pname_OK,'xxKKQ','46CM')
update plan425_b2 set pname_OK=replace(pname_OK,'xxKKR','33CM')
update plan425_b2 set pname_OK=replace(pname_OK,'xxKKT','85CM')

update plan425_b2 set pname_OK=replace(pname_OK,'yyKKa',' 925 ')

update plan425_b2 set pname_OK=replace(pname_OK,'yyKKb','100%')
update plan425_b2 set pname_OK=replace(pname_OK,'yyKKc','100*100cm')
update plan425_b2 set pname_OK=replace(pname_OK,'yyKKd','200*65cm')
update plan425_b2 set pname_OK=replace(pname_OK,'yyKKe','190*46cm')
update plan425_b2 set pname_OK=replace(pname_OK,'yyKKf','190*22cm')
update plan425_b2 set pname_OK=replace(pname_OK,'yyKKg','165*33cm')
update plan425_b2 set pname_OK=replace(pname_OK,'yyKKh','170*65cm')
update plan425_b2 set pname_OK=replace(pname_OK,'yyKKj','160*50cm')
update plan425_b2 set pname_OK=replace(pname_OK,'yyKKk','85*85cm')
update plan425_b2 set pname_OK=replace(pname_OK,'yyKKl','180*65cm')
update plan425_b2 set pname_OK=replace(pname_OK,'yyKKm','180*25cm')

update plan425_b2 set status=0 where pname_OK=''

update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')
update plan425_b2 set pname_OK=replace(pname_OK,'  ',' ')

update plan425_b2 set pname_OK=rtrim(ltrim(pname_OK))

";


            DB.ExecuteSQL(ls_up1);



            DataGroup group = new DataGroup();
            string ls_group_main_class = "select  id,pname_ok as remark from plan425_b2 ";
            group = DB.GetDataGroup(ls_group_main_class);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {

                    string attrib = group.Table.Rows[x]["remark"].ToString().Trim();

                    if (attrib.IndexOf(" ") > -1)
                    {
                        string ls_do = "";

                        string[] dog_small = attrib.Split(' ');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = FormatString(bb.Trim());



                            if (OK_bb.Length > 1)//长度限制
                            {
                                string pp = " " + OK_bb + " ";
                                if (ls_do.IndexOf(pp) > -1)
                                {

                                }
                                else
                                {
                                    ls_do += " " + OK_bb + " ";
                                }
                            }

                        }
                        string ls_up = "update plan425_b2 set pname_OK='" + ls_do.Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'";
                        DB.ExecuteSQL(ls_up);
                    }
                }
            }


            DB.ExecuteSQL(ls_up2);


            MessageBox.Show("恭喜！名称处理完成，目测一下，十万火急很重要！");



        }

        private void button145_Click(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;
            string ls_sql = "SELECT ID, URL_PIC FROM plan394_pic WHERE STATUS =0  order by ID";//
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = "";
                    pig = getUrlSource_proxy(group.Table.Rows[i]["URL_PIC"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312
                    //pig = getUrlSource_Adv(group.Table.Rows[i]["URL_PIC"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312


                    /* string fileName = Request.QueryString["file"];
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(Server.MapPath(fileName));

                    Response.Clear();
                    if ( fileName.EndsWith(".exe") ) {
                        Response.ContentType = "application/exe";
                    }
                    else {
                        Response.ContentType = "application/octet-stream";
                    }
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
                    Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    Response.WriteFile(fileInfo.FullName);
                    Response.Flush();
                    */
                    if (pig != "")
                    {
                        //下载文件(new Uri(group.Table.Rows[i]["URL_PIC"].ToString()), "e:/plan15/plan425-0415/", group.Table.Rows[i]["ID"].ToString());
                        string ls_lan = "E:\\Plan15\\plan394-0415\\" + group.Table.Rows[i]["ID"].ToString() + ".jpg";
                        using (FileStream fs = File.Open(ls_lan, FileMode.Create))
                        {
                            byte[] b = System.Text.Encoding.Default.GetBytes(pig);
                            fs.Write(b, 0, b.Length);
                            fs.Close();
                        }
                        string ls_up = "UPDATE plan394_pic SET status=1  WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);
                    }
                    
                    pig = "";
                }
            }
            MessageBox.Show("ok!");
        }

        Utility.PasswordGenerator aa = new Utility.PasswordGenerator();

        private void pwd_Click(object sender, EventArgs e)
        {

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT ID FROM planconfig where status=2 order by ID";//WHERE STATUS =1   where status=3 
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_up2 = "update planconfig set dbpassword='" + aa.Generate() + "'  where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up2);

                }

            }

            MessageBox.Show("OK!!");


        }

        private void button146_Click(object sender, EventArgs e)
        {
            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT id,dbname,dbuser,dbpassword FROM planconfig where status=2 order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    l_html.Text += "create database " + group_html.Table.Rows[i]["dbname"].ToString() + ";\r\n";

                }

                l_html.Text += "\r\n";
                l_html.Text += "\r\n";

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    l_html.Text += "CREATE USER " + group_html.Table.Rows[i]["dbuser"].ToString() + "@localhost IDENTIFIED BY '" + group_html.Table.Rows[i]["dbpassword"].ToString() + "';\r\n";
                    l_html.Text += "GRANT SELECT,INSERT,UPDATE,DELETE ON " + group_html.Table.Rows[i]["dbname"].ToString() + ".* TO " + group_html.Table.Rows[i]["dbuser"].ToString() + "@localhost;\r\n";
                    l_html.Text += "\r\n";
                }

            }

            MessageBox.Show("OK!!");
        }

        private void button147_Click(object sender, EventArgs e)
        {


            l_html.Text = "mysql -u root -h   localhost -p test \r\n\r\n";


            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT id,url,dbname,dbuser,dbpassword FROM planconfig where status=2 order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    l_html.Text += "use " + group_html.Table.Rows[i]["dbname"].ToString() + ";\r\n";
                    l_html.Text += "source /home/wwwroot/" + group_html.Table.Rows[i]["url"].ToString() + "/150LB.sql;\r\n";

                }

                l_html.Text += "\r\n";
                l_html.Text += "\r\n";



            }

            MessageBox.Show("OK!!");
        }

        private void button148_Click(object sender, EventArgs e)
        {

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT id,url  FROM planconfig where status=2 order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    //------------------------------------------------------------------------------------------------------------------------------------
                    string ls_php_text = "log_format  " + group_html.Table.Rows[i]["url"].ToString() + "  '$server_name - $remote_addr - $remote_user [$time_local] $request '" +
                    "\r\n" +
                    "'$status $body_bytes_sent $http_referer '" +
                    "\r\n" +
                    "'$http_user_agent $http_x_forwarded_for';" +
                    "\r\n" +
                    "server\r\n" +
                    "{\r\n" +
                    "listen       80;\r\n" +
                    "server_name " + group_html.Table.Rows[i]["url"].ToString() + " " + group_html.Table.Rows[i]["url"].ToString().Replace("www.", "") + ";\r\n" +
                    "index index.html index.htm index.php default.html default.htm default.php;\r\n" +
                    "root  /home/wwwroot/" + group_html.Table.Rows[i]["url"].ToString() + ";\r\n" +
                    "\r\n" +
                    "error_page 403 = /e404.html;\r\n" +
                    "location  /e404.html {\r\n" +
                    "root /home/wwwroot;\r\n" +
                    "allow all;\r\n" +
                    "}\r\n" +
                    "if ($http_accept_language ~* ^zh) {\r\n" +
                    "return   502;\r\n" +
                    "}\r\n" +
                    "if ($http_accept_language ~* ^fr) {\r\n" +
                    "return   502;\r\n" +
                    "}\r\n" +
                    "\r\n" +
                    "include zen.conf;\r\n" +
                    "location ~ .*\\.(php|php5)?$\r\n" +
                    "{\r\n" +
                    "fastcgi_pass  unix:/tmp/php-cgi.sock;\r\n" +
                    "fastcgi_index index.php;\r\n" +
                    "include fcgi.conf;\r\n" +
                    "}\r\n" +
                    "\r\n" +
                    "location ~ .*\\.(gif|jpg|jpeg|png|bmp|swf)$\r\n" +
                    "{\r\n" +
                    "expires      30d;\r\n" +
                    "}\r\n" +
                    "\r\n" +
                    "location ~ .*\\.(js|css)?$\r\n" +
                    "{\r\n" +
                    "expires      12h;\r\n" +
                    "}\r\n" +
                    "\r\n" +
                    "access_log  /home/wwwlogs/" + group_html.Table.Rows[i]["url"].ToString() + ".log  " + group_html.Table.Rows[i]["url"].ToString() + ";" +
                    "}\r\n";
                    //------------------------------------------------------------------------------------------------------------------------------------
                    string ls_php = "c:\\conf\\" + group_html.Table.Rows[i]["url"].ToString() + ".conf";

                    using (FileStream fs = File.Open(ls_php, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_php_text);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                    }
                    //------------------------------------------------------------------------------------------------------------------------------------

                    //------------------------------------------------------------------------------------------------------------------------------------
                    string ls_php_text2 = "log_format  " + group_html.Table.Rows[i]["url"].ToString().Replace("www.", "images.") + "  '$server_name - $remote_addr - $remote_user [$time_local] $request '" +
                    "\r\n" +
                    "'$status $body_bytes_sent $http_referer '" +
                    "\r\n" +
                    "'$http_user_agent $http_x_forwarded_for';" +
                    "\r\n" +
                    "server\r\n" +
                    "{\r\n" +
                    "listen       80;\r\n" +
                    "server_name " + group_html.Table.Rows[i]["url"].ToString().Replace("www.", "images.") + ";\r\n" +
                    "index index.html index.htm index.php default.html default.htm default.php;\r\n" +
                    "root  /home/wwwroot/" + group_html.Table.Rows[i]["url"].ToString().Replace("www.", "images.") + ";\r\n" +
                    "\r\n" +
                    "error_page 403 = /e404.html;\r\n" +
                    "location  /e404.html {\r\n" +
                    "root /home/wwwroot;\r\n" +
                    "allow all;\r\n" +
                    "}\r\n" +
                    "if ($http_accept_language ~* ^zh) {\r\n" +
                    "return   502;\r\n" +
                    "}\r\n" +
                    "if ($http_accept_language ~* ^fr) {\r\n" +
                    "return   502;\r\n" +
                    "}\r\n" +
                    "\r\n" +
                    "include zen.conf;\r\n" +
                    "location ~ .*\\.(php|php5)?$\r\n" +
                    "{\r\n" +
                    "fastcgi_pass  unix:/tmp/php-cgi.sock;\r\n" +
                    "fastcgi_index index.php;\r\n" +
                    "include fcgi.conf;\r\n" +
                    "}\r\n" +
                    "\r\n" +
                    "location ~ .*\\.(gif|jpg|jpeg|png|bmp|swf)$\r\n" +
                    "{\r\n" +
                    "expires      30d;\r\n" +
                    "}\r\n" +
                    "\r\n" +
                    "location ~ .*\\.(js|css)?$\r\n" +
                    "{\r\n" +
                    "expires      12h;\r\n" +
                    "}\r\n" +
                    "\r\n" +
                    "access_log  /home/wwwlogs/" + group_html.Table.Rows[i]["url"].ToString().Replace("www.", "images.") + ".log  " + group_html.Table.Rows[i]["url"].ToString().Replace("www.", "images.") + ";" +
                    "}\r\n";
                    //------------------------------------------------------------------------------------------------------------------------------------
                    string ls_php2 = "c:\\conf\\" + group_html.Table.Rows[i]["url"].ToString().Replace("www.", "images.") + ".conf";

                    using (FileStream fs = File.Open(ls_php2, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_php_text2);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                    }
                    //------------------------------------------------------------------------------------------------------------------------------------

                }

            }
            MessageBox.Show("OK!!");

            
        }

        private void button149_Click(object sender, EventArgs e)
        {

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT id,product_name,description FROM plan_lib where brand='Gucci' and code is null ";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_ppp = IsGucci(group_html.Table.Rows[i]["product_name"].ToString());

                    string ls_up2 = "update plan_lib set code='" + ls_ppp + "'  where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up2);
                }

            }

            MessageBox.Show("OK!!");
        }


        public string IsLV(string phoneold)
        {
            string phonenew = "";
            //Regex Expression = new Regex("28[0-9]{4}|27[0-9]{4}|26[0-9]{4}|25[0-9]{4}|24[0-9]{4}|23[0-9]{4}|22[0-9]{4}");
            //NMLKJIHGFEA       0-9a-zA-Z
            Regex Expression = new Regex("M[0-9]{5}|N[0-9]{5}|m[0-9]{5}|n[0-9]{5}|M[0-9]{4}[a-zA-Z]{1}|N[0-9]{4}[a-zA-Z]{1}|m[0-9]{4}[a-zA-Z]{1}|n[0-9]{4}[a-zA-Z]{1}");
            Match match = Expression.Match(phoneold, 0, phoneold.Length);
            if (match.Success)
                return phonenew = phoneold.Substring(match.Index, 6).ToUpper();
            else
                return "";
        }


       public static string IsGanBr(string Htmlstring)
        {
            
            //删除脚本  
            //Htmlstring = Regex.Replace(Htmlstring, @"- A", "<br/>-A", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- B", "<br/>-B", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- C", "<br/>-C", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- D", "<br/>-D", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- E", "<br/>-E", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- F", "<br/>-F", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- G", "<br/>-G", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- H", "<br/>-H", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- I", "<br/>-I", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- J", "<br/>-J", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- K", "<br/>-K", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- L", "<br/>-L", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- M", "<br/>-M", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- N", "<br/>-N", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- O", "<br/>-O", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- P", "<br/>-P", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- Q", "<br/>-Q", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- R", "<br/>-R", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- S", "<br/>-S", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- T", "<br/>-T", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- U", "<br/>-U", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- V", "<br/>-V", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- W", "<br/>-W", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- X", "<br/>-X", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- Y", "<br/>-Y", RegexOptions.None);
            //Htmlstring = Regex.Replace(Htmlstring, @"- Z", "<br/>-Z", RegexOptions.None);
          

            //Htmlstring = Regex.Replace(Htmlstring, @"<!--[^@]*-->", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"\<FORM[^@]*\</FORM\>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"\<img[^\>]*\>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"\<table[^\>]*\>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"\<tbody[^\>]*\>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"\<tr[^\>]*\>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"\<td[^\>]*\>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"\</tr\>", "", RegexOptions.IgnoreCase);
            //Htmlstring = Regex.Replace(Htmlstring, @"\</td\>", "", RegexOptions.IgnoreCase);

            return Htmlstring;

       }

        private void button150_Click(object sender, EventArgs e)
        {
            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;
            string ls_sql = "SELECT id,product_name,description FROM plan_lib where brand='x' and code is null ";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_ppp = IsLV(group_html.Table.Rows[i]["product_name"].ToString());

                    string ls_up2 = "update plan_lib set code='" + ls_ppp + "'  where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up2);
                }

            }

            MessageBox.Show("OK!!");
        }

        private void button151_Click(object sender, EventArgs e)
        {
/*
update plan_Lib set pic_group_count=
(select count(*) from PLAN302_pic xx where filesize > 1000 and status=1
and plan_Lib.bid=xx.pid
) where from_table='PLAN302';
*/
            string ls_up = "";

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT distinct from_table as xx FROM plan_lib where pic_group_count is null";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {

                    ls_up += "update plan_Lib set pic_group_count=  "+
                            " (select count(*) from " + group_html.Table.Rows[i]["xx"].ToString() + "_pic xx where filesize > 1000 and flag='B' and status=1 " +
                            " and plan_Lib.bid=xx.pid "+
                            " ) where from_table='" + group_html.Table.Rows[i]["xx"].ToString() + "'; ";
                                                            

                    
                }

                DB.ExecuteSQL(ls_up);

                ls_up = "";

            }



            string ls_init = @"


";

            DB.ExecuteSQL(ls_init);

            MessageBox.Show("OK!!");


        }

        private void button152_Click(object sender, EventArgs e)
        {
            string ls_init2 = @"
            update plan42510 set add_flag=null;
            update plan42510 set add_flag=1 where code in
            (select code from plan425_b2 where status in(2,5) );

            UPDATE plan_Lib SET AREA_FLAG =null;
            UPDATE plan_Lib SET AREA_FLAG=1 where code in
            (select code from plan42510 where flag='OK' and add_flag is null)
            and brand='x';
            ";
            DB.ExecuteSQL(ls_init2);
            
            
            
            DataGroup group_html = new DataGroup();
            group_html = null;

            DataGroup group_do = new DataGroup();
            group_do = null;

            string ls_sql = "SELECT code,avg(price)as pp FROM plan_Lib where AREA_FLAG=1 group by code";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_do = "SELECT from_table,bid,main_class,sub_class,product_name,pic_s,pic_group,description,brand,code FROM plan_Lib where area_flag=1 and code='" + group_html.Table.Rows[i]["code"].ToString() + "' " +
                                    "order by code,d_color desc,d_size desc,pic_group_count desc,price desc";
                    string ls_price = group_html.Table.Rows[i]["pp"].ToString();
                    group_do = DB.GetDataGroup(ls_do);
                    string ls_add = "insert into plan425_b2 (from_table,bid,main_class,sub_class,product_name,pic_s,pic_group,description,brand,code,pid,FROM_FLAG,status,price)"+
                        "values ('" + group_do.Table.Rows[0]["from_table"].ToString() + "','" + group_do.Table.Rows[0]["bid"].ToString() + "',"+
                                "'" + group_do.Table.Rows[0]["main_class"].ToString() + "','" + group_do.Table.Rows[0]["sub_class"].ToString() + "',"+
                                "'" + group_do.Table.Rows[0]["product_name"].ToString() + "','" + group_do.Table.Rows[0]["pic_s"].ToString() + "',"+
                                "'" + group_do.Table.Rows[0]["pic_group"].ToString() + "','" + group_do.Table.Rows[0]["description"].ToString() + "'," +
                                "'" + group_do.Table.Rows[0]["brand"].ToString() + "','" + group_do.Table.Rows[0]["code"].ToString() + "'," +
                                "'0','1','5','" + ls_price + "')";
                    DB.ExecuteSQL(ls_add);

                }

            }

            MessageBox.Show("OK!!");

        }

        private void button153_Click(object sender, EventArgs e)
        {
            string ls_up1 = @"  
update plan425_b2 set price=89 where price <90 and status in(2,5);
update plan425_b2 set pic_name=lower(replace(replace(replace(replace(pname_ok,' ','-'),'.','-'),'#',''),'*','-'))+'.jpg'
                ";

            DB.ExecuteSQL(ls_up1);
            
            
            
            DataGroup group_from = new DataGroup();

            DataGroup group = new DataGroup();
            DataGroup group_if = new DataGroup();
            DataGroup group_if2 = new DataGroup();
            DataGroup group_null = new DataGroup();
            DataGroup group_bad = new DataGroup();

            string ls_from = "select FROM_TABLE,count(*) xx from plan425_b2 where status=5 group by FROM_TABLE  ";
            group_from = DB.GetDataGroup(ls_from);
            if (group_from.Table.Rows.Count > 0)
            {
                for (int pp = 0; pp < group_from.Table.Rows.Count; pp++)
                {

                    string ls_group = "select bid as id,pname_ok as fullname from plan425_b2 where status =5 and  FROM_TABLE='" + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "' order by id ";
                    group = DB.GetDataGroup(ls_group);
                    if (group.Table.Rows.Count > 0)
                    {

                        /*   */
                        for (int x = 0; x < group.Table.Rows.Count; x++)
                        {
                            string ls_id = group.Table.Rows[x]["id"].ToString();
                            string ls_fullname = group.Table.Rows[x]["fullname"].ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-").Replace(".", "-").Replace("#", "").Replace("*", "-").Trim().ToLower();

                            /*   */

                            //详细的大图
                            string ls_group2 = "select id from " + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "_pic where pid='" + ls_id + "'     and flag='B' and status =1 order by id ";
                            group_if = null;
                            group_if2 = null;
                            group_if = DB.GetDataGroup(ls_group2);
                            if (group_if.Table.Rows.Count > 0)
                            {
                                for (int y = 0; y < group_if.Table.Rows.Count; y++)
                                {

                                    try
                                    {
                                        File.Copy("E:\\Plan15\\" + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "-0415\\" + group_if.Table.Rows[y]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan425-0415-Big\\" + ls_fullname + "_" + (y + 1) + ".jpg", true);

                                        string ls_up = "UPDATE " + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "_pic SET oldname='" + ls_fullname + "_" + (y + 1) + ".jpg'  WHERE id='" + group_if.Table.Rows[y]["id"].ToString().Trim() + "' ";
                                        DB.ExecuteSQL(ls_up);

                                    }
                                    catch { }
                                }
                            }


                            //主图-小图
                            string ls_group3 = "select id from " + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "_pic where pid='" + ls_id + "'    and flag='S' and status =1  order by id ";
                            group_if2 = null;
                            group_if2 = DB.GetDataGroup(ls_group3);
                            if (group_if2.Table.Rows.Count > 0)
                            {
                                for (int y = 0; y < group_if2.Table.Rows.Count; y++)
                                {
                                    try
                                    {
                                        File.Copy("E:\\Plan15\\" + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "-0415\\" + group_if2.Table.Rows[y]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan425-0415-Small\\" + ls_fullname + ".jpg", true);

                                        //File.Copy("E:\\Plan15\\plan425-0415\\" + group_if2.Table.Rows[y]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan425-0415-Big\\" + ls_fullname + ".jpg", true);

                                        string ls_up = "UPDATE " + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "_pic SET oldname='" + ls_fullname + ".jpg'  WHERE id='" + group_if2.Table.Rows[y]["id"].ToString().Trim() + "' ";
                                        DB.ExecuteSQL(ls_up);

                                    }
                                    catch { }
                                }
                            }

                        }


                        //如果没有明细图，只有一个大图的情况

                        //将主图第一个放到详细图里
                        string ls_bignull = "select  (select id from " + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "_pic where pid=plan425_b2.bid and flag='S') as id, pname_ok as fullname from plan425_b2  where pic_group='' and status =5 and len(pic_s)>5  and FROM_TABLE='" + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "' order by id ";
                        //详细图有水印，用主图

                        group_null = DB.GetDataGroup(ls_bignull);
                        if (group_null.Table.Rows.Count > 0)
                        {
                            for (int g = 0; g < group_null.Table.Rows.Count; g++)
                            {

                                try
                                {
                                    File.Copy("E:\\Plan15\\" + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "-0415\\" + group_null.Table.Rows[g]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan425-0415-Big\\" + group_null.Table.Rows[g]["fullname"].ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-").Replace(".", "-").Replace("#", "").Replace("*", "-").Trim().ToLower().Trim() + ".jpg", true);
                                }
                                catch
                                { }

                                //string ls_up = "UPDATE plan425_pic SET oldname='" + group_null.Table.Rows[g]["fullname"].ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-").Replace(".", "-").Trim().ToLower().Trim() + ".jpg'  WHERE id='" + group_null.Table.Rows[g]["id"].ToString().Trim() + "' ";
                                //DB.ExecuteSQL(ls_up);
                            }
                        }




                        //特殊情况，没有主图，抓详细图第一个作为主图
                        string ls_bad = "select  (select min(id) from " + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "_pic where pid=plan425_b2.bid and flag='B' and status =1 ) as id, pname_ok as fullname,pic_name from plan425_b2 where  status =5 and ( pic_s is null or pic_s ='') and FROM_TABLE='" + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "'  order by id ";
                        group_bad = DB.GetDataGroup(ls_bad);
                        if (group_bad.Table.Rows.Count > 0)
                        {
                            for (int g = 0; g < group_bad.Table.Rows.Count; g++)
                            {
                                if (group_bad.Table.Rows[g]["id"].ToString().Trim().Length > 0)
                                {
                                    File.Copy("E:\\Plan15\\" + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "-0415\\" + group_bad.Table.Rows[g]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan425-0415-Small\\" + group_bad.Table.Rows[g]["fullname"].ToString().Replace(" ", "-").Replace("/", "-").Replace(":", "-").Replace(".", "-").Replace("#", "").Replace("*", "-").Trim().ToLower().Trim() + ".jpg", true);
                                }
                            }
                        }



                    }


                //
                }

            }

            MessageBox.Show("ok!");
        }

        private void button154_Click(object sender, EventArgs e)
        {
            DataGroup group_from = new DataGroup();


            DataGroup group_field = new DataGroup();
            group_field = null;
            DataGroup group_if = new DataGroup();
            group_if = null;


            string ls_from = "select FROM_TABLE,count(*) xx from plan425_b2 where status=5 group by FROM_TABLE ";
            group_from = DB.GetDataGroup(ls_from);
            if (group_from.Table.Rows.Count > 0)
            {
                for (int pp = 0; pp < group_from.Table.Rows.Count; pp++)
                {

                    string ls_sql = "select bid as id,pname_ok as fullname from plan425_b2 where status =5 and FROM_TABLE='" + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "' order by id";
                    group_field = DB.GetDataGroup(ls_sql);
                    if (group_field.Table.Rows.Count > 0)
                    {

                        for (int i = 0; i < group_field.Table.Rows.Count; i++)
                        {

                            string ls_id = group_field.Table.Rows[i]["id"].ToString();
                            string ls_fullname = group_field.Table.Rows[i]["fullname"].ToString();

                            string ls_shtml = "";
                            string dog = "";


                            string ls_group_if = "select id,oldname as picname from " + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "_pic where pid='" + ls_id + "' /**/ and flag='B' and oldname !='' order by id ";
                            group_if = null;
                            group_if = DB.GetDataGroup(ls_group_if);
                            if (group_if.Table.Rows.Count > 0)
                            {
                                for (int y = 0; y < group_if.Table.Rows.Count; y++)
                                {
                                    if (y < 6)
                                    {
                                        dog += "<img src=\"http://images.x.com/" + group_if.Table.Rows[y]["picname"].ToString() + "\" alt=\"" + ls_fullname + " " + (y + 1) + "\" title=\"" + ls_fullname + " " + (y + 1) + "\" /><br/>";
                                    }
                                    else
                                    {
                                        if (y < 12)
                                        {
                                            dog += "<img src=\"http://images.x.com/" + group_if.Table.Rows[y]["picname"].ToString() + "\"  /><br/>";
                                        }
                                    }
                                }
                            }




                            if (dog.Length > 1)
                            {
                                ls_shtml += "<div class=\"sidebox-header-left\"><h3 class=\"leftBoxHeading\">Product Images:</h3></div><div class=\"sideBoxContent\"   align=\"center\">"
                                    + dog + "</div>";
                            }

                            string ls_up = "update plan425_b2 set bigimghtml ='" + ls_shtml.Replace('\'', '‘') + "'  where bid='" + group_field.Table.Rows[i]["id"].ToString().Trim() + "' and FROM_TABLE='" + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "'   ";
                            DB.ExecuteSQL(ls_up);
                        }
                    }


                    /* */
                    DataGroup group_null = new DataGroup();
                    group_null = null;
                    //如果没有明细图，只有一个大图的情况
                    //将主图第一个放到详细图里
                    string ls_bignull = "select bid as id,pname_ok as pname,pic_name from plan425_b2 where status =5  and (pic_group='' or pic_group is null )  and pic_s is not null and FROM_TABLE='" + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "'   order by id ";
                    group_null = DB.GetDataGroup(ls_bignull);
                    if (group_null.Table.Rows.Count > 0)
                    {
                        for (int g = 0; g < group_null.Table.Rows.Count; g++)
                        {
                            string dog = "<img src=\"http://images.x.com/" + group_null.Table.Rows[g]["pic_name"].ToString() + "\" alt=\"" + group_null.Table.Rows[g]["pname"].ToString() + "\" title=\"" + group_null.Table.Rows[g]["pname"].ToString() + "\" /><br/>";

                            //File.Copy("E:\\Plan15\\plan2-0330\\" + group_null.Table.Rows[g]["id"].ToString().Trim() + ".jpg", "E:\\Plan15\\plan2-0330-Big\\" + group_null.Table.Rows[g]["fullname"].ToString().Trim() + ".jpg", true);
                            string ls_shtml = "<div class=\"sidebox-header-left\"><h3 class=\"leftBoxHeading\">Product Images:</h3></div><div class=\"sideBoxContent\"   align=\"center\">"
                                   + dog + "</div>";

                            string ls_up = "update plan425_b2 set bigimghtml ='" + ls_shtml.Replace('\'', '‘') + "'  where bid='" + group_null.Table.Rows[g]["id"].ToString().Trim() + "'  and FROM_TABLE='" + group_from.Table.Rows[pp]["FROM_TABLE"].ToString() + "' ";
                            DB.ExecuteSQL(ls_up);

                        }
                    }

                }

            }


            MessageBox.Show("html  is  OK!");
        }

        private void button155_Click(object sender, EventArgs e)
        {

           string ls_init_a = @"
            ALTER TABLE plan425_b2 ADD FROM_TABLE [varchar](20) COLLATE Chinese_PRC_CI_AS NULL;
            ALTER TABLE plan425_b2 ADD FROM_FLAG [varchar](1) COLLATE Chinese_PRC_CI_AS NULL;
            ALTER TABLE plan425_b2 ADD CODE [varchar](20) COLLATE Chinese_PRC_CI_AS NULL;
            ALTER TABLE plan425_b2 ADD BRAND [varchar](20) COLLATE Chinese_PRC_CI_AS NULL;
            ALTER TABLE plan425_b2 ADD BID BIGINT;";
           //DB.ExecuteSQL(ls_init_a);

           string ls_init_b = @"
        

            ";
            DB.ExecuteSQL(ls_init_b);


            //这种情况没有考虑

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;
            string ls_sql = "SELECT id,product_name,description FROM plan425_b2 where brand='x'  ";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_ppp = IsLV(group_html.Table.Rows[i]["product_name"].ToString());

                    string ls_up2 = "update plan425_b2 set code='" + ls_ppp + "'  where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up2);
                }

            }



            



              MessageBox.Show("OK!!");


        }

        private void button157_Click(object sender, EventArgs e)
        {
            /*

INSERT INTO plan426_b2
(PID,main_class,sub_class,URL_PRODUCT,STATUS
,product_name,price,PIC_S,Pic_group,Description
,Brand
,Ref
,Series
,Type
,Color
,Material
,Season
,Size
,FROM_TABLE,bid)
select PID,main_class,sub_class,URL_PRODUCT,5
,product_name,price,PIC_S,Pic_group,a_tag
,a_Brand
,a_Ref
,a_Series
,a_Type
,a_Color
,a_Material
,a_Season
,a_Size ,'planlv',id from planlv_b2
      
            insert into plan425_b2 (from_table,bid,main_class,sub_class,product_name,pic_s,pic_group,description,brand,code,pid,FROM_FLAG,status,price)
            select FROM_TABLE,bid,main_class,sub_class,product_name,pic_s,pic_group,description,brand,code,0,1,5,price
            from plan_Lib where FROM_TABLE='PLAN335' and brand='HERMES'

            update plan425_b2 set status=0 where brand='PRADA' and status=2

            insert into plan_Lib (from_table,Bid,main_class,sub_class,product_name,price,pic_s,pic_group,description,brand,code,status)
            select 'plan425',id,main_class,sub_class,product_name,price,pic_s,pic_group,description,brand,code,1
            from plan425_b2 where status=2 and len(description)>50
             */
        }

        private void button158_Click(object sender, EventArgs e)
        {
            DataGroup group_html = new DataGroup();
            group_html = null;
            string ls_sql = "SELECT id,url FROM planconfig where status=4";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_id = group_html.Table.Rows[i]["id"].ToString();
                    string ls_ppp = group_html.Table.Rows[i]["url"].ToString();

                    string ls_sqlfile_Base = "D:\\WebHosting\\php\\vhosts\\0\\" + ls_ppp + "\\includes\\configure.php";
                    StreamReader fs_sqlfile = new StreamReader(ls_sqlfile_Base, System.Text.Encoding.UTF8);
                    string ls_sqlfile_Text = fs_sqlfile.ReadToEnd();
                    fs_sqlfile.Close();


                    string ls_DBNAME = OperateStr_Adv(ls_sqlfile_Text, "define('DB_DATABASE', '", "'", "", 1, 0, 0, "0", "", "", "");
                    string ls_DBUSER = OperateStr_Adv(ls_sqlfile_Text, "define('DB_SERVER_USERNAME', '", "'", "", 1, 0, 0, "0", "", "", "");
                    string ls_DBPASSWORD = OperateStr_Adv(ls_sqlfile_Text, "define('DB_SERVER_PASSWORD', '", "'", "", 1, 0, 0, "0", "", "", "");




                    string ls_sqlfile_Base2 = "D:\\WebHosting\\php\\vhosts\\0\\" + ls_ppp + "\\includes\\languages\\english\\meta_tags.php";
                    StreamReader fs_sqlfile2 = new StreamReader(ls_sqlfile_Base2, System.Text.Encoding.UTF8);
                    string ls_sqlfile_Text2 = fs_sqlfile2.ReadToEnd();
                    fs_sqlfile2.Close();

                    string ls_TITLE = OperateStr_Adv(ls_sqlfile_Text2, "define('TITLE', '", "'", "", 1, 0, 0, "0", "", "", "");
                    string ls_SITE_TAGLINE = OperateStr_Adv(ls_sqlfile_Text2, "define('SITE_TAGLINE', '", "'", "", 1, 0, 0, "0", "", "", "");
                    string ls_CUSTOM_KEYWORDS = OperateStr_Adv(ls_sqlfile_Text2, "define('CUSTOM_KEYWORDS', '", "'", "", 1, 0, 0, "0", "", "", "");
                    string ls_HOME_PAGE_META_DESCRIPTION = OperateStr_Adv(ls_sqlfile_Text2, "define('HOME_PAGE_META_DESCRIPTION', '", "'", "", 1, 0, 0, "0", "", "", "");
                    string ls_HOME_PAGE_META_KEYWORDS = OperateStr_Adv(ls_sqlfile_Text2, "define('HOME_PAGE_META_KEYWORDS', '", "'", "", 1, 0, 0, "0", "", "", "");
                    string ls_HOME_PAGE_TITLE = OperateStr_Adv(ls_sqlfile_Text2, "define('HOME_PAGE_TITLE', '", "'", "", 1, 0, 0, "0", "", "", "");


                    string ls_do = "update planconfig set    TITLE='" + ls_TITLE + "',SITE_TAGLINE='" + ls_SITE_TAGLINE + "',CUSTOM_KEYWORDS='" + ls_CUSTOM_KEYWORDS + "',HOME_PAGE_META_DESCRIPTION='" + ls_HOME_PAGE_META_DESCRIPTION + "',HOME_PAGE_META_KEYWORDS='" + ls_HOME_PAGE_META_KEYWORDS + "',HOME_PAGE_TITLE='" + ls_HOME_PAGE_TITLE + "' ,                     dbname='" + ls_DBNAME + "',dbuser='" + ls_DBUSER + "',dbpassword='" + ls_DBPASSWORD + "' where status=4 and id='" + ls_id + "'";
                    DB.ExecuteSQL(ls_do);
                    
                }

            }

            MessageBox.Show("OK!");
        }

        private void button159_Click(object sender, EventArgs e)
        {

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            group_html = null;

            string ls_sql = "SELECT ID,description as remark FROM plan425_b2 order by ID";//WHERE STATUS =1   where status=3 
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {

                    string ls_shtml = group_html.Table.Rows[i]["remark"].ToString().Replace("\n", "").Replace("<br/>-", "∷");
                    string[] dog_small = ls_shtml.Split('∷');//子串

                    int dog_count = dog_small.Length;

                    if (dog_count <= 2)
                    {
                        string ls_up2 = "update plan425_b2 set description='" + group_html.Table.Rows[i]["remark"].ToString() + "'  where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                        DB.ExecuteSQL(ls_up2);
                    }
                    else
                    {

                        string ls_temp = "";
                        string ls_xx = "";

                        int pd = 0;
                        foreach (string bb in dog_small)
                        {
                            ls_temp = bb;
                            if (pd > 0 && bb.Trim().Length >5)
                            {
                                if (bb.IndexOf("--") > -1)
                                {
                                    ls_temp = "<br/>" + pd.ToString() + ". " + bb.Trim() + ";";
                                }
                                else
                                {
                                    ls_temp = "<br/>" + pd.ToString() + ". " + bb.Trim();
                                }
                            }

                            ls_xx += ls_temp;
                            pd += 1;
                        }

                        string ls_up2 = "update plan425_b2 set description='" + ls_xx + "'  where id='" + group_html.Table.Rows[i]["ID"].ToString() + "' ";
                        DB.ExecuteSQL(ls_up2);
                    }
                }

            }


            MessageBox.Show("OK!!");

        }

        private void button161_Click(object sender, EventArgs e)
        {
            DataGroup group = new DataGroup();
            string ls_group_main_class = "select  id,description from plan425_B2 where status=2 ";
            group = DB.GetDataGroup(ls_group_main_class);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {
                    string bbbb = group.Table.Rows[x]["description"].ToString().Replace("\n", " ").Replace("<br>", "<br/>").Replace("<br/>", "∷").Trim();
                    string ls_do = "";

                    if (bbbb.IndexOf("∷") > -1)
                    {
                        string[] dog_small = bbbb.Split('∷');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = bb.Trim();// FormatString(bb.Trim());

                            if (OK_bb.Length > 1)
                            {

                                //if (OK_bb.IndexOf("replica") != -1 && OK_bb.IndexOf("Size") == -1)//
                                //if (OK_bb.IndexOf("--") == -1 )
                                if (OK_bb.ToLower().IndexOf("strong>") != -1)//
                                {
                                    //|| OK_bb.IndexOf("<strong>Color</strong>") == -1 || OK_bb.IndexOf("<strong>COLOR</strong>") == -1
                                    ls_do += "";
                                }
                                else
                                {
                                    ls_do += "<br/>" + OK_bb.Trim();
                                }


                            }
                        }

                    }


                    string ls_up = "update plan425_B2 set description='" + ls_do.Replace("∷", "<br/>").Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'";
                    DB.ExecuteSQL(ls_up);
                }
            }



            MessageBox.Show("ok!");
        }

        //NMLKJIHGFEA       0-9a-zA-Z
        //Regex Expression = new Regex("M[0-9]{5}|N[0-9]{5}|m[0-9]{5}|n[0-9]{5}|M[0-9]{4}[a-zA-Z]{1}|N[0-9]{4}[a-zA-Z]{1}|m[0-9]{4}[a-zA-Z]{1}|n[0-9]{4}[a-zA-Z]{1}");


        public string IsGucci_bad(string phoneold)
        {
            string phonenew = "";
            Regex Expression = new Regex("[0-9]{6} [0-9a-zA-Z]{5} [0-9]{4}");
            Match match = Expression.Match(phoneold, 0, phoneold.Length);
            if (match.Success)
                return phonenew = phoneold.Substring(match.Index, 17);
            else
                return "";
        }

        private void button162_Click(object sender, EventArgs e)
        {
            //针对CP取编码和清除编码，操作前去掉年份
            /*
            DataGroup group = new DataGroup();
            string ls_group_main_class = "select  id,pname_OK as description from plan425_B2 where brand in ('Chanel','Prada') and status=2 ";
            group = DB.GetDataGroup(ls_group_main_class);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {                                                                                                                 //根据空格分隔
                    string bbbb = group.Table.Rows[x]["description"].ToString().Replace("\n", " ").Replace("<br>", "<br/>").Replace(" ", "∷").Trim();
                    string ls_do = "";

                    if (bbbb.IndexOf("∷") > -1)
                    {
                        string[] dog_small = bbbb.Split('∷');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = bb.Trim();// FormatString(bb.Trim());

                            if (OK_bb.Length > 1)
                            {


                                if (OK_bb.ToLower().IndexOf("0") != -1 || OK_bb.ToLower().IndexOf("1") != -1 || OK_bb.ToLower().IndexOf("2") != -1 || OK_bb.ToLower().IndexOf("3") != -1 || OK_bb.ToLower().IndexOf("4") != -1 || OK_bb.ToLower().IndexOf("5") != -1 || OK_bb.ToLower().IndexOf("6") != -1 || OK_bb.ToLower().IndexOf("7") != -1 || OK_bb.ToLower().IndexOf("8") != -1 || OK_bb.ToLower().IndexOf("9") != -1)//
                                {
                                    ls_do += "";

                                    string ls_xx = "update plan425_B2 set code='" + OK_bb.Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'";
                                    DB.ExecuteSQL(ls_xx);
                                }
                                else
                                {
                                    //还原空格
                                    ls_do += " " + OK_bb.Trim();
                                }

                            }
                        }

                    }
                    string ls_up = "update plan425_B2 set pname3='" + ls_do.Replace("∷", " ").Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'";
                    DB.ExecuteSQL(ls_up);
                }
            }
            */

            //去掉gucci三段编码
            /**/
            DataGroup group_Gu_good = new DataGroup();
            string ls_Gu_good = "select id,product_name  from plan425_b2 where main_class like '%Gucci%'  or sub_class like '%Gucci%' ";
            group_Gu_good = null;
            group_Gu_good = DB.GetDataGroup(ls_Gu_good);
            if (group_Gu_good.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_Gu_good.Table.Rows.Count; i++)
                {
                    string ls_is_gucci = IsGucci_bad(group_Gu_good.Table.Rows[i]["product_name"].ToString());

                    //string ls_is_gucci_code = ls_is_gucci;

                    if (ls_is_gucci == "")
                    {
                        //string ls_is_gucci_sql = "update plan425_b2 set status=0 where id ='" + group_Gu_good.Table.Rows[i]["id"].ToString() + "' ";
                        //DB.ExecuteSQL(ls_is_gucci_sql);
                    }
                    else
                    {
                        string ls_is_gucci_sql = "update plan425_b2 set desc2='" + ls_is_gucci + "' where id ='" + group_Gu_good.Table.Rows[i]["id"].ToString() + "' ";
                        DB.ExecuteSQL(ls_is_gucci_sql);
                    }
                }
            } 
           
            


            //去掉含数字的词
            /*
            DataGroup group = new DataGroup();
            string ls_group_main_class = "select  id,pname_OK as description from plan425_B2 where brand='Gucci' and status=2 ";
            group = DB.GetDataGroup(ls_group_main_class);
            if (group.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group.Table.Rows.Count; x++)
                {                                                                                                                 //根据空格分隔
                    string bbbb = group.Table.Rows[x]["description"].ToString().Replace("\n", " ").Replace("<br>", "<br/>").Replace(" ", "∷").Trim();
                    string ls_do = "";

                    if (bbbb.IndexOf("∷") > -1)
                    {
                        string[] dog_small = bbbb.Split('∷');//子串
                        foreach (string bb in dog_small)
                        {
                            string OK_bb = bb.Trim();// FormatString(bb.Trim());

                            if (OK_bb.Length > 1)
                            {

                                //if (OK_bb.IndexOf("replica") != -1 && OK_bb.IndexOf("Size") == -1)//
                                //if (OK_bb.IndexOf("--") == -1 )
                                if (OK_bb.ToLower().IndexOf("0") != -1 || OK_bb.ToLower().IndexOf("1") != -1 || OK_bb.ToLower().IndexOf("2") != -1 || OK_bb.ToLower().IndexOf("3") != -1 || OK_bb.ToLower().IndexOf("4") != -1 || OK_bb.ToLower().IndexOf("5") != -1 || OK_bb.ToLower().IndexOf("6") != -1 || OK_bb.ToLower().IndexOf("7") != -1 || OK_bb.ToLower().IndexOf("8") != -1 || OK_bb.ToLower().IndexOf("9") != -1)//
                                {
                                    //|| OK_bb.IndexOf("<strong>Color</strong>") == -1 || OK_bb.IndexOf("<strong>COLOR</strong>") == -1
                                    ls_do += "";
                                }
                                else
                                {
                                           //还原空格
                                    ls_do += " " + OK_bb.Trim();
                                }


                            }
                        }

                    }


                    string ls_up = "update plan425_B2 set pname3='" + ls_do.Replace("∷", " ").Trim() + "' where id='" + group.Table.Rows[x]["id"].ToString().Trim() + "'";
                    DB.ExecuteSQL(ls_up);
                }
            }
            */

            MessageBox.Show("OK!!");
        }

        private void button163_Click(object sender, EventArgs e)
        {
            l_html.Text = IsGucci_bad(l_html.Text);
        }

        private void button164_Click(object sender, EventArgs e)
        {
            return;
            
            //先清空
            string ls_mysql_dirx = "C:\\Documents and Settings\\Administrator\\桌面\\b.sql";
            string ls_mysql_Textx = "";
            using (FileStream fs = File.Open(ls_mysql_dirx, FileMode.Create))
            {
                byte[] b = System.Text.Encoding.Default.GetBytes(ls_mysql_Textx);
                fs.Write(b, 0, b.Length);
                fs.Close();
                ls_mysql_Textx = "";
            }

            DataGroup group_config = new DataGroup();
            string ls_config = "SELECT ga,ga_flag,id,code,dbname,dbuser,dbpassword,url,admin_dir,kw_main,TITLE,SITE_TAGLINE,CUSTOM_KEYWORDS,HOME_PAGE_META_DESCRIPTION,HOME_PAGE_META_KEYWORDS,HOME_PAGE_TITLE FROM PlanConfig Where Status=1";
            group_config = DB.GetDataGroup(ls_config);
            if (group_config.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_config.Table.Rows.Count; i++)
                {
                    string ls_dir = group_config.Table.Rows[i]["url"].ToString().Trim();

                    DirectoryInfo source = new DirectoryInfo("E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\");
                    DirectoryInfo destination = new DirectoryInfo("E:\\PHP-1.5.6\\vhosts\\" + ls_dir + "\\");
                    
                    DirectoryInfo dirimg = new DirectoryInfo("E:\\PHP-1.5.6\\vhosts\\" + ls_dir.Replace("www.", "images.") + "\\");
                    if (!dirimg.Exists)
                    {
                        Directory.CreateDirectory("E:\\PHP-1.5.6\\vhosts\\" + ls_dir.Replace("www.", "images.") + "\\");
                    }

                    CopyDirectory(source, destination, true);

                    //------------------------------------------------------------------------------------------------------------------------------------
                    //------------------------------------------------------------------------------------------------------------------------------------
                    //------------------------------------------------------------------------------------------------------------------------------------
                    string ls_lan_text = "\r\n127.0.0.1       " + ls_dir + "\r\n" + "127.0.0.1       " + ls_dir.Replace("www", "images") + "\r\n";
                    string ls_lan = "C:\\WINDOWS\\system32\\drivers\\etc\\hosts";

                    StreamReader fs_is_ect = new StreamReader(ls_lan, System.Text.Encoding.UTF8);
                    string ls_is_ect = fs_is_ect.ReadToEnd();
                    fs_is_ect.Close();
                    if (ls_is_ect.IndexOf(ls_dir) == -1)
                    {
                        using (FileStream fs = File.Open(ls_lan, FileMode.Append))
                        {
                            byte[] b = System.Text.Encoding.Default.GetBytes(ls_lan_text);
                            fs.Write(b, 0, b.Length);
                            fs.Close();
                        }
                    }
                    //------------------------------------------------------------------------------------------------------------------------------------
                    //------------------------------------------------------------------------------------------------------------------------------------
                    string ls_php_text = "<VirtualHost *>\r\n" +
                         "    <Directory \"../vhosts/www." + ls_dir.Replace("www.", "") + "\">\r\n" +
                         "        Options -Indexes FollowSymLinks\r\n" +
                         "    </Directory>\r\n" +
                         "    ServerAdmin admin@www." + ls_dir.Replace("www.", "") + "\r\n" +
                         "    DocumentRoot \"../vhosts/www." + ls_dir.Replace("www.", "") + "\"\r\n" +
                         "    ServerName www." + ls_dir.Replace("www.", "") + ":80\r\n" +
                         "    ServerAlias *.www." + ls_dir.Replace("www.", "") + "\r\n" +
                         "    ErrorLog logs/www." + ls_dir.Replace("www.", "") + "-error_log\r\n" +
                         "    php_admin_value open_basedir \"E:\\PHP-1.5.6\\vhosts\\www." + ls_dir.Replace("www.", "") + ";C:\\WINDOWS\\Temp;\"\r\n" +
                         "</VirtualHost>\r\n\r\n" +
                         "<VirtualHost *>\r\n" +
                         "    <Directory \"../vhosts/images." + ls_dir.Replace("www.", "") + "\">\r\n" +
                         "        Options -Indexes FollowSymLinks\r\n" +
                         "    </Directory>\r\n" +
                         "    ServerAdmin admin@images." + ls_dir.Replace("www.", "") + "\r\n" +
                         "    DocumentRoot \"../vhosts/images." + ls_dir.Replace("www.", "") + "\"\r\n" +
                         "    ServerName images." + ls_dir.Replace("www.", "") + ":80\r\n" +
                         "    ServerAlias *.images." + ls_dir.Replace("www.", "") + "\r\n" +
                         "    ErrorLog logs/images." + ls_dir.Replace("www.", "") + "-error_log\r\n" +
                         "    php_admin_value open_basedir \"E:\\PHP-1.5.6\\vhosts\\images." + ls_dir.Replace("www.", "") + ";C:\\WINDOWS\\Temp;\"\r\n" +
                         "</VirtualHost>\r\n\r\n";
                    //------------------------------------------------------------------------------------------------------------------------------------
                    string ls_php = "E:\\PHP-1.5.6\\Apache-20\\conf\\extra\\httpd-vhosts.conf";
                    StreamReader fs_is_php = new StreamReader(ls_php, System.Text.Encoding.UTF8);
                    string ls_is_php = fs_is_php.ReadToEnd();
                    fs_is_php.Close();
                    if (ls_is_php.IndexOf(ls_dir) == -1)
                    {
                        using (FileStream fs = File.Open(ls_php, FileMode.Append))
                        {
                            byte[] b = System.Text.Encoding.Default.GetBytes(ls_php_text);
                            fs.Write(b, 0, b.Length);
                            fs.Close();
                        }
                    }
                    //------------------------------------------------------------------------------------------------------------------------------------
                    //RestartService("Apache_pn", 3000);
                    //------------------------------------------------------------------------------------------------------------------------------------
                    //------------------------------------------------------------------------------------------------------------------------------------

                    string ls_db = "";
                    string ls_url = "";

                    //-----------------------------------------------------------------------------------------------------------------------------------
                    //for (int i = 0; i < group_config.Table.Rows.Count; i++)
                    //{
                        //----------------------------------------取配置--------------------------------------------------
                        string lc_code = group_config.Table.Rows[i]["code"].ToString().Trim();
                        string lc_dbname = group_config.Table.Rows[i]["dbname"].ToString().Trim();
                        ls_db = lc_dbname;
                        string lc_dbuser = group_config.Table.Rows[i]["dbuser"].ToString().Trim();
                        string lc_dbpassword = group_config.Table.Rows[i]["dbpassword"].ToString().Trim();
                        string lc_url = group_config.Table.Rows[i]["url"].ToString().Trim();
                        ls_url = lc_url;
                        string lc_url_shot = group_config.Table.Rows[i]["url"].ToString().Replace("www.", "").Trim();
                        string lc_admin_dir = group_config.Table.Rows[i]["admin_dir"].ToString().Trim();
                        string lc_kw_main = group_config.Table.Rows[i]["kw_main"].ToString().Trim();
                        string lc_TITLE = group_config.Table.Rows[i]["TITLE"].ToString().Trim();
                        string lc_SITE_TAGLINE = group_config.Table.Rows[i]["SITE_TAGLINE"].ToString().Trim();
                        string lc_CUSTOM_KEYWORDS = group_config.Table.Rows[i]["CUSTOM_KEYWORDS"].ToString().Trim();
                        string lc_HOME_PAGE_META_DESCRIPTION = group_config.Table.Rows[i]["HOME_PAGE_META_DESCRIPTION"].ToString().Trim();
                        string lc_HOME_PAGE_META_KEYWORDS = group_config.Table.Rows[i]["HOME_PAGE_META_KEYWORDS"].ToString().Trim();
                        string lc_HOME_PAGE_TITLE = group_config.Table.Rows[i]["HOME_PAGE_TITLE"].ToString().Trim();
                        //------------------------------------------------------------------------------------------------
                        //--------------------------------------sql主key替换----------------------------------------------
                        //------------------------------------------------------------------------------------------------
                        string ls_sqlfile_Base = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\150L302B.sql";
                        string ls_sqlfile_New = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\150LB.sql";
                        StreamReader fs_sqlfile = new StreamReader(ls_sqlfile_Base, System.Text.Encoding.UTF8);
                        string ls_sqlfile_Text = fs_sqlfile.ReadToEnd();
                        fs_sqlfile.Close();
                        ls_sqlfile_Text = ls_sqlfile_Text.Replace("'nike'", "'" + lc_kw_main + "'").Replace("x.com", lc_url_shot);
                        using (FileStream fs = File.Open(ls_sqlfile_New, FileMode.Create))
                        {
                            byte[] b = System.Text.Encoding.Default.GetBytes(ls_sqlfile_Text);
                            fs.Write(b, 0, b.Length);
                            fs.Close();
                            ls_sqlfile_Text = "";
                        }
                        //------------------------------------------END---------------------------------------------------
                        //------------------------------------------------------------------------------------------------
                        //---------------------------------------2个配置文件替换------------------------------------------
                        string ls_configure_Base = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\includes\\configure.php";
                        string ls_configure_New = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\includes\\configure.php";
                        StreamReader fs_configure = new StreamReader(ls_configure_Base, System.Text.Encoding.UTF8);
                        string ls_configure = fs_configure.ReadToEnd();
                        fs_configure.Close();
                        //【标准站】替换为【新站】
                        ls_configure = ls_configure.Replace("x.com", lc_url_shot);
                        //【数据库名称】替换
                        ls_configure = ls_configure.Replace("define('DB_DATABASE', '150L302');", "define('DB_DATABASE', '" + lc_dbname + "');");
                        //【数据库用户名】
                        //ls_configure = ls_configure.Replace("define('DB_SERVER_USERNAME', 'root');", "define('DB_SERVER_USERNAME', '" + lc_dbuser + "');");
                        //【旧密码】替换为【新密码】
                        //ls_configure = ls_configure.Replace("define('DB_SERVER_PASSWORD', 'admin');", "define('DB_SERVER_PASSWORD', '" + lc_dbpassword + "');");
                        //【旧路径】替换为【新路径】
                        //ls_configure = ls_configure.Replace("define('DIR_FS_CATALOG', 'E:/PHP-1.5.6/vhosts//", "define('DIR_FS_CATALOG', '/home/wwwroot/");
                        //ls_configure = ls_configure.Replace("define('DIR_FS_SQL_CACHE', 'E:/PHP-1.5.6/vhosts/", "define('DIR_FS_SQL_CACHE', '/home/wwwroot/");

                        System.IO.File.SetAttributes(ls_configure_New, System.IO.FileAttributes.Normal);

                        using (FileStream fs = File.Open(ls_configure_New, FileMode.Create))
                        {
                            byte[] b = System.Text.Encoding.Default.GetBytes(ls_configure);
                            fs.Write(b, 0, b.Length);
                            fs.Close();
                            ls_configure = "";
                        }
                        //------------------------------------------------------------------------------------------------
                        string ls_configure_admin_Base = "E:\\PHP-1.5.6\\vhosts\\新标www.x.com\\xing\\includes\\configure.php";
                        string ls_configure_admin_New = "E:\\PHP-1.5.6\\vhosts\\" + lc_url + "\\xing\\includes\\configure.php";
                        StreamReader fs_configure_admin = new StreamReader(ls_configure_admin_Base, System.Text.Encoding.UTF8);
                        string ls_configure_admin = fs_configure_admin.ReadToEnd();
                        fs_configure_admin.Close();
                        //【标准站】替换为【新站】
                        ls_configure_admin = ls_configure_admin.Replace("x.com", lc_url_shot);
                        //【数据库名称】替换
                        ls_configure_admin = ls_configure_admin.Replace("define('DB_DATABASE', '150L302');", "define('DB_DATABASE', '" + lc_dbname + "');");
                        //【数据库用户名】
                        //ls_configure_admin = ls_configure_admin.Replace("define('DB_SERVER_USERNAME', 'root');", "define('DB_SERVER_USERNAME', '" + lc_dbuser + "');");
                        //【旧密码】替换为【新密码】
                        //ls_configure_admin = ls_configure_admin.Replace("define('DB_SERVER_PASSWORD', 'admin');", "define('DB_SERVER_PASSWORD', '" + lc_dbpassword + "');");
                        //【旧路径】替换为【新路径】
                        //ls_configure_admin = ls_configure_admin.Replace("define('DIR_FS_CATALOG', 'E:/PHP-1.5.6/vhosts//", "define('DIR_FS_CATALOG', '/home/wwwroot/");
                        //ls_configure_admin = ls_configure_admin.Replace("define('DIR_FS_SQL_CACHE', 'E:/PHP-1.5.6/vhosts/", "define('DIR_FS_SQL_CACHE', '/home/wwwroot/");
                        using (FileStream fs = File.Open(ls_configure_admin_New, FileMode.Create))
                        {
                            byte[] b = System.Text.Encoding.Default.GetBytes(ls_configure_admin);
                            fs.Write(b, 0, b.Length);
                            fs.Close();
                            ls_configure_admin = "";
                        }
                        //------------------------------------------END---------------------------------------------------
                        //------------------------------------------------------------------------------------------------
                        //------------------------------------------------------------------------------------------------
                    //}


                    //mysql -u root -padmin
                    //create database 139L;
                    //use 139L

                    string ls_bat_dir = "C:\\Documents and Settings\\Administrator\\桌面\\a.bat";
                    string ls_bat_Text = "path E:\\PHP-1.5.6\\MySQL-5.0.90\\bin\r\nmysql -u root -padmin <b.sql";
                    using (FileStream fs = File.Open(ls_bat_dir, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_bat_Text);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_bat_Text = "";
                    }


                    string ls_mysql_dir = "C:\\Documents and Settings\\Administrator\\桌面\\b.sql";
                    string ls_mysql_Text = "\r\ndrop database " + ls_db + ";\r\ncreate database " + ls_db + ";\r\nuse " + ls_db + "\r\nsource E:/PHP-1.5.6/vhosts/" + ls_url + "/150LB.sql;\r\n";
                    //string ls_mysql_Text = "\r\ncreate database " + ls_db + ";\r\nuse " + ls_db + "\r\nsource E:/PHP-1.5.6/vhosts/" + ls_url + "/150LB.sql;\r\n";
                    using (FileStream fs = File.Open(ls_mysql_dir, FileMode.Append))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_mysql_Text);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_mysql_Text = "";
                    }



                }
            }

            //------------------------------------------------------------------------------------------------------------------------------------
            //RestartService("Apache_pn", 3000);
            //------------------------------------------------------------------------------------------------------------------------------------
            MessageBox.Show("恭喜恭喜，【批量】复制成功！ \r\n 亲 下一步手动执行下bat文件哦");
        }

        private void button165_Click(object sender, EventArgs e)
        {
            //1.生成xid
            DataGroup group_xid = new DataGroup();
            string ls_group_xid = "select pname1,id from plan425_b2 where status in (2,3,5) order by main_class asc,id desc";
            group_xid = DB.GetDataGroup(ls_group_xid);
            if (group_xid.Table.Rows.Count > 0)
            {
                for (int x = 0; x < group_xid.Table.Rows.Count; x++)
                {
                    int xx = x + 1;

                    string ls_up = "UPDATE plan425_b2 SET xid='" + xx + "'  WHERE id='" + group_xid.Table.Rows[x]["id"].ToString() + "' ";
                    DB.ExecuteSQL(ls_up);
                }
                string ls_exe = "UPDATE plan425_b2 SET product_code='x-'+cast(xid as nvarchar(10)) ";  //Jerseys
                DB.ExecuteSQL(ls_exe);
            }


            //clear
            string ls_name = "E:\\PHP-1.5.6\\vhosts\\www.x.com\\plan-product.sql";
            using (FileStream fs = File.Open(@ls_name, FileMode.Create))
            {
                byte[] b = System.Text.Encoding.Default.GetBytes("");
                fs.Write(b, 0, b.Length);
                fs.Close();
            }
            //jv3_products
            //products_id, products_type, products_quantity, products_model, products_image, products_price, products_virtual, products_date_added, products_last_modified, 
            //products_date_available, products_weight, products_status, products_tax_class_id, manufacturers_id, products_ordered, products_quantity_order_min, 
            //products_quantity_order_units, products_priced_by_attribute, product_is_free, product_is_call, products_quantity_mixed, product_is_always_free_shipping, 
            //products_qty_box_status, products_quantity_order_max, products_sort_order, products_discount_type, products_discount_type_from, products_price_sorter, master_categories_id, 
            //products_mixed_discount_quantity, metatags_title_status, metatags_products_name_status, metatags_model_status, metatags_price_status, metatags_title_tagline_status
            //---------------
            //  产品Xid        1              产品数量          产品编码         图片名称         价格              0                 日期1                   日期2        
            //'1970-01-01 08:00:00'       0                     1                 0                    0                 0                    1                    
            //1                               0                          0                0                   0                         0                            
            //1                          0                            0                       0                     0                           价格                大分类id          
            //1                            1                            0                           0                   0                          0                      

            //jv3_products_description
            //products_id, language_id, products_name, products_description,  bigimghtml
            //    产品Xid      1          产品名称           产品描述          图片描述                 

            //jv3_products_to_categories    
            //products_id 	categories_id
            //    产品Xid      目录ID

            DataGroup group_super = new DataGroup();
            string ls_super = @" 
select ok from (
select 'insert into `jv3_products` (`products_id`, `products_type`, `products_quantity`, `products_model`, `products_image`, `products_price`, `products_virtual`, `products_date_added`, `products_last_modified`, `products_date_available`, `products_weight`, `products_status`, `products_tax_class_id`, `manufacturers_id`, `products_ordered`, `products_quantity_order_min`, `products_quantity_order_units`, `products_priced_by_attribute`, `product_is_free`, `product_is_call`, `products_quantity_mixed`, `product_is_always_free_shipping`, `products_qty_box_status`, `products_quantity_order_max`, `products_sort_order`, `products_discount_type`, `products_discount_type_from`, `products_price_sorter`, `master_categories_id`, `products_mixed_discount_quantity`, `metatags_title_status`, `metatags_products_name_status`, `metatags_model_status`, `metatags_price_status`, `metatags_title_tagline_status`) '+
'values ('+ cast(xid as nvarchar(10))+',1 ,'+cast(dbo.RandData(14,72) as nvarchar(10))+','''+product_code+''','''+pic_name+''','+price+',0,'''+
cast(dbo.fn_getdate('2012-04-7','2012-04-13') as nvarchar(10))+''','''+cast(dbo.fn_getdate('2012-04-7','2012-04-13') as nvarchar(10)) +''',''1970-01-01 08:00:00'''+
',0,1,0,0,0,1,1,0,0,0,0,0,1,0,0,0,0,'+price+','+(case cid_sub when '' then cid else cid_sub end)+',1,1,0,0,0,0'+
');' as ok , xid,'1' as f  from plan425_b2 where status in (2,3,5) 
union
select 'insert into `jv3_products_description` (`products_id`, `language_id`, `products_name`, `products_description`, `bigimghtml`) '+
'values ('+ cast(xid as nvarchar(10))+',1,'''+pname_OK+''','''+desc2+''','''+bigimghtml+''''+
');' as ok , xid,'2' as f  from plan425_b2 where status in (2,3,5)
union
select 'insert into `jv3_products_to_categories` (`products_id`, `categories_id`) '+
'values ('+ cast(xid as nvarchar(10))+','+(case cid_sub when '' then cid else cid_sub end)+
');' as ok , xid,'3' as f  from plan425_b2 where status in (2,3,5)
) xx order by f,xid
";

            group_super = DB.GetDataGroup(ls_super);
            if (group_super.Table.Rows.Count > 0)
            {
                string ls_ddd = "";
                for (int x = 0; x < group_super.Table.Rows.Count; x++)
                {
                    ls_ddd += group_super.Table.Rows[x]["ok"].ToString() + "\n";
                    using (FileStream fs = File.Open(@ls_name, FileMode.Append))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_ddd);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_ddd = "";
                    }
                }
            }
            //------------------------------------恭喜发财！--------------------------------------------------------------


            MessageBox.Show("恭喜！导出了要仔细目测，注意关键字，网址，产品描述！    这里要修改手动用SQL调整目录！甚至去掉一些不好的分类！切记！");
        }

        

       





    }
}
