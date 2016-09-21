using ExcelLibrary.SpreadSheet;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using TX.Framework.WindowUI.Forms;
using System.Diagnostics;

namespace wms
{
    public partial class Invoice_new : BaseForm
    {
        public Invoice_new()
        {
            InitializeComponent();
        }

        DataTable dt_xls;

        private void txButton7_Click(object sender, EventArgs e)
        {
            #region clear
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();

            string ls_up=@"
truncate table [dbo].[t_imp_1];
truncate table [dbo].[t_imp_2];
truncate table [dbo].[t_imp_3];
truncate table [dbo].[t_imp_4];
truncate table [dbo].[t_imp_5];
truncate table [dbo].[t_imp_6];
truncate table [dbo].[t_imp_7];
truncate table [dbo].[t_imp_8];
truncate table [dbo].[t_imp_9];
truncate table [dbo].[t_imp_10];
truncate table [dbo].[t_imp_11];
truncate table [dbo].[t_imp_12];
truncate table [dbo].[t_imp_13];";

            OleDbCommand cmd = new OleDbCommand(ls_up, myConn);
            cmd.ExecuteNonQuery();

            string sqlstr = "select fromid, site_code from t_base_from where status='1';";
            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataTable dtViewCol = new DataTable();
            adapter.Fill(dtViewCol);
            for (int index = 0; index < dtViewCol.Rows.Count; index++)
            {
                string ls_fromid = dtViewCol.Rows[index]["fromid"].ToString();
                string ls_site_code = dtViewCol.Rows[index]["site_code"].ToString();
                string ls_dir = "";
                /*Monday
                Tuesday
                Wednesday
                Thursday
                Friday
                Saturday
                Sunday*/
                if (DateTime.Now.AddDays(1).DayOfWeek.ToString() != "Saturday" && DateTime.Now.AddDays(1).DayOfWeek.ToString() != "Sunday")
                {
                    ls_dir = "z:\\Invoice Printing\\" + DateTime.Now.AddDays(1).ToString("yyyy-MM") + "\\" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "\\" + ls_site_code + "\\";
                    DirectoryInfo d_dir = new DirectoryInfo(ls_dir);
                    if (!d_dir.Exists)
                    {
                        d_dir.Create();
                    }
                }

                if (DateTime.Now.AddDays(2).DayOfWeek.ToString() != "Saturday" && DateTime.Now.AddDays(2).DayOfWeek.ToString() != "Sunday")
                {

                    ls_dir = "z:\\Invoice Printing\\" + DateTime.Now.AddDays(2).ToString("yyyy-MM") + "\\" + DateTime.Now.AddDays(2).ToString("yyyy-MM-dd") + "\\" + ls_site_code + "\\";
                    DirectoryInfo d_dir = new DirectoryInfo(ls_dir);
                    if (!d_dir.Exists)
                    {
                        d_dir.Create();
                    }

                }


                if (DateTime.Now.AddDays(3).DayOfWeek.ToString() != "Saturday" && DateTime.Now.AddDays(3).DayOfWeek.ToString() != "Sunday")
                {
                    ls_dir = "z:\\Invoice Printing\\" + DateTime.Now.AddDays(3).ToString("yyyy-MM") + "\\" + DateTime.Now.AddDays(3).ToString("yyyy-MM-dd") + "\\" + ls_site_code + "\\";
                    DirectoryInfo d_dir = new DirectoryInfo(ls_dir);
                    if (!d_dir.Exists)
                    {
                        d_dir.Create();
                    }

                }

            }
            dtViewCol = null;
            #endregion


            string ls_load_file2 = "select fileid,table_name,file_type,file_dir,(select site_code from t_base_from where fromid=[t_base_file].fromid) as file_dir2,file_name,field_count,include_head,field_Separator,imp_check_file_time from [t_base_file] where file_type in ('txt','csv','xls') and status='1'  ";
            adapter = new OleDbDataAdapter(ls_load_file2, myConn);
            DataTable dt_imp = new DataTable();

            adapter.Fill(dt_imp);

            for (int i = 0; i < dt_imp.Rows.Count; i++)
            {

                string ls_file_type = dt_imp.Rows[i]["file_type"].ToString();
                string ls_file_FullName = "z:\\Invoice Printing\\" + DateTime.Now.ToString("yyyy-MM") + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" + dt_imp.Rows[i]["file_dir2"].ToString() + "\\" + dt_imp.Rows[i]["file_name"].ToString();

                string ls_old_FullName = dt_imp.Rows[i]["file_dir"].ToString() + "\\" + dt_imp.Rows[i]["file_name"].ToString();

                System.IO.FileInfo file_old = new System.IO.FileInfo(ls_old_FullName);
                string ls_create_time_old = file_old.LastWriteTime.ToString("yyyyMMdd");

                System.IO.FileInfo file = new System.IO.FileInfo(ls_file_FullName);



                if (file_old.Exists && !file.Exists && ls_create_time_old == DateTime.Now.ToString("yyyyMMdd"))
                {
                    file_old.CopyTo(ls_file_FullName);
                }

                System.IO.FileInfo file_ok = new System.IO.FileInfo(ls_file_FullName);



                //string ls_create_time = file.LastWriteTime.ToString("yyyyMMdd");
                string ls_create_time_ok = file_ok.LastWriteTime.ToString("yyyyMMdd");
                if (DateTime.Now.ToString("yyyyMMdd") == ls_create_time_ok)
                {
                    string ls_field_Separator = dt_imp.Rows[i]["field_Separator"].ToString();
                    string ls_table_name = dt_imp.Rows[i]["table_name"].ToString();
                    switch (ls_file_type)
                    {
                        case "txt":
                            #region txt
                            string ls_txt_left = "";
                            string ls_txt_add = "";
                            StreamReader fs_is_ect = new StreamReader(ls_file_FullName, System.Text.Encoding.UTF8);
                            string ls_txt_is_ect = fs_is_ect.ReadToEnd();
                            fs_is_ect.Close();
                            string[] dog_small = ls_txt_is_ect.Replace("\r\n", "|").Split('|');
                            int cc = 0;
                            foreach (string bb in dog_small)
                            {

                                string ls_txt_bb = bb.Trim();
                                if (ls_txt_bb.Length > 10)
                                {


                                    if (cc == 0)
                                    {
                                        ls_txt_left = "insert into " + ls_table_name + "([" + ls_txt_bb.Replace("|", "").Replace(ls_field_Separator, "|") + "])";
                                    }
                                    else
                                    {
                                        string[] dog_this_csv = ls_txt_bb.Replace(ls_field_Separator, "|").Split('|');
                                        string[] dog_this_csv_left = ls_txt_left.Split('|');
                                        int ls_txt_dog = dog_this_csv.Length;
                                        string ls_txt_left_1 = "";
                                        for (int y = 0; y < ls_txt_dog; y++)
                                        {
                                            string ls_txt_cat = dog_this_csv_left[y] + "],[";
                                            ls_txt_left_1 = ls_txt_left_1 + ls_txt_cat;
                                        }
                                        ls_txt_left_1 = ls_txt_left_1 + "])";
                                        ls_txt_left_1 = ls_txt_left_1.Replace(",[])", ")").Replace("])])", "])");
                                        string ls_txt_this_csv = " values ('" + ls_txt_bb.Replace("'", "''").Replace(ls_field_Separator, "','") + "');";
                                        ls_txt_add += ls_txt_left_1 + ls_txt_this_csv;
                                    }
                                    cc++;
                                }
                            }

                            cmd = new OleDbCommand(ls_txt_add, myConn);
                            cmd.ExecuteNonQuery();


                            ls_txt_add = "";
                            cc = 0;
                            break;
                        #endregion
                        case "csv":
                            #region csv
                            string ls_csv_left = "";
                            string ls_csv_add = "";
                            StreamReader fs_is_csv = new StreamReader(ls_file_FullName, System.Text.Encoding.UTF8);
                            string ls_csv_is_ect = fs_is_csv.ReadToEnd();
                            fs_is_csv.Close();
                            string[] dog_csv = ls_csv_is_ect.Replace("\r\n", "|").Replace("\n", "|").Split('|');
                            int dd = 0;
                            foreach (string bb in dog_csv)
                            {

                                string ls_csv_bb = bb.Trim();
                                if (ls_csv_bb.Length > 10)
                                {
                                    if (dd == 0)
                                    {
                                        ls_csv_left = "insert into " + ls_table_name + "([" + ls_csv_bb.Replace("|", "").Replace(ls_field_Separator, "|") + "])";
                                    }
                                    else
                                    {
                                        string ls_zzz1zzz = "\"You may return most new and unopened items within 30 days of delivery for a full refund. To initiate a return, visit overstock.com/myaccount or call 1-800-843-2446. For an international return, email international@overstock.com or call 00-1-919-576-9926 for instructions.\"";
                                        string ls_csv_11 = "";
                                        if (ls_table_name == "t_imp_8" || ls_table_name == "t_imp_10" || ls_table_name == "t_imp_11")
                                        {
                                            ls_csv_bb = ls_csv_bb.Replace(ls_zzz1zzz, "ls_zzz1zzz");

                                            ls_csv_11 = ls_csv_bb.Replace(ls_field_Separator, "|");
                                        }
                                        else
                                        {
                                            ls_csv_11 = ls_csv_bb.Replace("\"" + ls_field_Separator + "\"", "|");
                                        }
                                        string[] dog_this_csv = ls_csv_11.Split('|');


                                        string[] dog_this_csv_left = ls_csv_left.Split('|');
                                        int ls_csv_dog = dog_this_csv.Length;
                                        string ls_csv_left_1 = "";
                                        for (int y = 0; y < ls_csv_dog; y++)
                                        {
                                            string ls_csv_cat = dog_this_csv_left[y] + "],[";
                                            ls_csv_left_1 = ls_csv_left_1 + ls_csv_cat;
                                        }
                                        ls_csv_left_1 = ls_csv_left_1 + "])";
                                        ls_csv_left_1 = ls_csv_left_1.Replace(",[])", ")").Replace("])])", "])");
                                        string ls_csv_this_csv = "";

                                        if (ls_table_name == "t_imp_8" || ls_table_name == "t_imp_10" || ls_table_name == "t_imp_11")
                                        {
                                            ls_csv_this_csv = " values ('" + ls_csv_bb.Replace("'", "''").Replace(ls_field_Separator, "','").Replace("\"", "").Replace("ls_zzz1zzz", ls_zzz1zzz) + "');";
                                        }
                                        else
                                        {
                                            ls_csv_this_csv = " values ('" + ls_csv_bb.Replace("'", "''").Replace("\"" + ls_field_Separator + "\"", "','").Replace("\"", "") + "');";
                                        }
                                        ls_csv_add += ls_csv_left_1 + ls_csv_this_csv;
                                        ls_csv_this_csv = "";
                                    }
                                    dd++;
                                }
                            }

                            cmd = new OleDbCommand(ls_csv_add, myConn);
                            cmd.ExecuteNonQuery();


                            ls_csv_add = "";
                            dd = 0;
                            break;
                        #endregion
                        case "xls":
                            #region xls only new egg
                            if (ls_table_name == "t_imp_13")
                            {
                                string ls_xls_left = "";
                                string ls_paras = "";

                                //ExcelOptions.ExcelOptions xx = new ExcelOptions.ExcelOptions();
                                //dt_xls = xx.GetExcelData(ls_file_FullName);

                                dt_xls = NPOIOprateExcel.ExcelUtility.ExcelToDataTable(ls_file_FullName, false);

                                for (int z = 0; z < dt_xls.Rows.Count; z++)
                                {
                                    for (int zc = 0; zc < dt_xls.Columns.Count; zc++)
                                    {
                                        if (z == 0)
                                        {
                                            ls_xls_left += "[" + dt_xls.Rows[z][zc] + "],";
                                        }
                                        else
                                        {
                                            if (dt_xls.Rows[z][0].ToString() == "Unshipped")
                                            {
                                                ls_paras += "'" + dt_xls.Rows[z][zc].ToString().Replace("'", "''") + "',";
                                            }
                                        }
                                    }

                                    //if (range_is_get.Text.ToString() == "Order Status" || range_is_get.Text.ToString() == "Unshipped")

                                    if (z > 0 && dt_xls.Rows[z][0].ToString() == "Unshipped")
                                    {
                                        string ls_insert = "insert into " + ls_table_name + " (" + ls_xls_left.Substring(0, ls_xls_left.Length - 1) + ") values (" + ls_paras.Substring(0, ls_paras.Length - 1) + ")";

                                        cmd = new OleDbCommand(ls_insert, myConn);
                                        cmd.ExecuteNonQuery();


                                    }
                                    ls_paras = "";
                                }

                            }
                            #endregion
                            else
                            {
                                #region xls full
                                Workbook book = null;
                                try
                                {
                                    book = Workbook.Load(ls_file_FullName);
                                }
                                catch (DirectoryNotFoundException ex)
                                {
                                    MessageBox.Show("File directory not found!");
                                    return;
                                }
                                catch (FileNotFoundException ex)
                                {
                                    MessageBox.Show("Excel file not found!");
                                    return;
                                }

                                Worksheet sheet = book.Worksheets[0];
                                string ls_left = "";

                                for (int rowIndex = sheet.Cells.FirstRowIndex; rowIndex <= sheet.Cells.LastRowIndex; rowIndex++)
                                {
                                    Row row = sheet.Cells.GetRow(rowIndex);
                                    string paras = "";

                                    var numberOfCols = sheet.Cells.LastColIndex;
                                    if (numberOfCols == 11)
                                    {
                                        numberOfCols = 12;
                                    }

                                    for (int colIndex = sheet.Cells.FirstColIndex; colIndex < numberOfCols; colIndex++)
                                    {
                                        if (rowIndex == 0)
                                        {
                                            ls_left += "[" + row.GetCell(colIndex).Value.ToString() + "],";
                                        }
                                        else
                                        {
                                            switch (colIndex)
                                            {
                                                case 1:
                                                    paras += "'" + row.GetCell(colIndex).DateTimeValue.ToString("MM/dd/yyyy") + "',";
                                                    break;
                                                case 2:
                                                    var cell = row.GetCell(colIndex);
                                                    var st = cell.StringValue;
                                                    var dl = Convert.ToDouble(st);
                                                    var date = DateTime.FromOADate(dl).ToString("MM/dd/yyyy");
                                                    paras += "'" + date + "',";
                                                    break;
                                                case 3:
                                                    paras += row.GetCell(colIndex).Value.ToString() + ",";
                                                    break;
                                                default:
                                                    paras += "'" + row.GetCell(colIndex).Value.ToString() + "',";
                                                    break;
                                            }
                                        }
                                    }

                                    if (rowIndex > 0)
                                    {
                                        string ls_insert = "insert into " + ls_table_name + " (" + ls_left.Substring(0, ls_left.Length - 1) + ") values (" + paras.Substring(0, paras.Length - 1) + ")";

                                        cmd = new OleDbCommand(ls_insert, myConn);
                                        cmd.ExecuteNonQuery();

                                    }
                                }
                            }
                            #endregion
                            break;
                    }
                }
            }

            dt_imp = null;




            #region set data full

            string ls_gc="select [access_dir],[access_table],[sql_table],[insert_f],[select_f] from [t_base_access] where status=1;";
            adapter = new OleDbDataAdapter(ls_gc, myConn);
            DataTable dt_gc = new DataTable();
            adapter.Fill(dt_gc);
            for (int i = 0; i < dt_gc.Rows.Count; i++)
            {
                    string ls_access_dir = dt_gc.Rows[i]["access_dir"].ToString();
                    string ls_access_table = dt_gc.Rows[i]["access_table"].ToString();
                    string ls_select_f = dt_gc.Rows[i]["select_f"].ToString();
                    string ls_sql_table = dt_gc.Rows[i]["sql_table"].ToString();


                cmd = new OleDbCommand("truncate table " + ls_sql_table + ";", myConn);
                cmd.ExecuteNonQuery();


                    //change cpu x86

                    OleDbConnection objConn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ls_access_dir + ";");
                    OleDbCommand MDBCommand = new OleDbCommand("Select " + ls_select_f + " FROM" + ls_access_table, objConn);
                    OleDbDataReader rdr2;
                    objConn.Open();
                    rdr2 = MDBCommand.ExecuteReader();
                    SqlBulkCopy sbc = new SqlBulkCopy(ConfigurationManager.AppSettings["dbConnectionString"]);
                    sbc.DestinationTableName = ls_sql_table;
                try
                {
                    sbc.WriteToServer(rdr2);
                }
                catch
                {
                }
                    sbc.Close();
                    rdr2.Close();
                    objConn.Close();
               
            }
            dt_gc = null;


            #endregion


            #region all import data insert into order table
           string ls_big_sql= @"
/*amazon home*/
INSERT INTO [t_biz_order]
([FROMID], [Order_DAY], [batch], [order_id], [payments_date], 
[shipping_class], [shipping_name], 
[total_itemprice], 
[total_shipping], 
[total_tax], 
[total_discount], 
[total_giftwrap], [total_Gift_Certificates], 
[total_sum], 
[Notes], [ebay_userid], [buy_name], [email], [phone], 
[name], [ship_to_company], [addr1], [addr2], [city], 
[state], [zip], [country], [Global_Shipping_Reference_ID], [sku], 
[sys_sku], [item], [product_name], [qty], [item_price], 
[item_shipping_price], [item_shipping_tax], [item_shipping_discount], [item_us_tax], [item_discount], 
[item_giftwrap], [shipping_details], [status], [remark])
select 
'1', convert(varchar(8), getdate(), 112), '', a.[order-id], b.[payments-date], 
a.[ship-service-level], a.[ship-service-level], 
(select cast(a.[quantity-purchased] as float) * sum(cast([item-price] as float)) from [t_imp_1] where [order-id]=a.[order-id]) as total_itemprice, 
(select cast(a.[quantity-purchased] as float) * (sum(cast([shipping-price] as float)) +sum(cast([shipping-tax] as float)))  from [t_imp_1] where [order-id]=a.[order-id]) as total_shipping, 
(select cast(a.[quantity-purchased] as float) * sum(cast([item-tax] as float)) from [t_imp_1] where [order-id]=a.[order-id]) as total_tax, 
(select cast(a.[quantity-purchased] as float) * sum(cast([item-promotion-discount] as float))+sum(cast([ship-promotion-discount] as float)) from [t_imp_1] where [order-id]=a.[order-id]) as total_discount, 
'0.0', '0.0', 
(select cast(a.[quantity-purchased] as float) * (sum(cast([item-price] as float)) + sum(cast([shipping-price] as float)) +sum(cast([shipping-tax] as float)) +sum(cast([item-tax] as float))-(sum(cast([item-promotion-discount] as float))+sum(cast([ship-promotion-discount] as float)) ))  from [t_imp_1] where [order-id]=a.[order-id]) as total_sum, 
'', '', a.[buyer-name], '', a.[ship-phone-number], 
b.[recipient-name], '', b.[ship-address-1], b.[ship-address-2], b.[ship-city], 
b.[ship-state], b.[ship-postal-code], b.[ship-country], '', a.[sku], 
'', '', a.[product-name], a.[quantity-purchased], a.[item-price], 
a.[shipping-price] as [item_shipping_price], a.[shipping-tax] as [item_shipping_tax], a.[ship-promotion-discount], a.[item-tax], a.[item-promotion-discount], 
'0.0', '', '1', ''
from t_imp_1 a ,t_imp_2 b where a.[order-id]=b.[order-id] and a.[order-item-id]=b.[order-item-id]
and a.[order-id] not in (select order_id from t_biz_order);


/*amazon 2*/
INSERT INTO [t_biz_order]
([FROMID], [Order_DAY], [batch], [order_id], [payments_date], 
[shipping_class], [shipping_name], 
[total_itemprice], 
[total_shipping], 
[total_tax], 
[total_discount], 
[total_giftwrap], [total_Gift_Certificates], 
[total_sum], 
[Notes], [ebay_userid], [buy_name], [email], [phone], 
[name], [ship_to_company], [addr1], [addr2], [city], 
[state], [zip], [country], [Global_Shipping_Reference_ID], [sku], 
[sys_sku], [item], [product_name], [qty], [item_price], 
[item_shipping_price], [item_shipping_tax], [item_shipping_discount], [item_us_tax], [item_discount], 
[item_giftwrap], [shipping_details], [status], [remark])
select 
'2', convert(varchar(8), getdate(), 112), '', a.[order-id], b.[payments-date], 
a.[ship-service-level], a.[ship-service-level], 
(select cast(a.[quantity-purchased] as float) * sum(cast([item-price] as float)) from [t_imp_3] where [order-id]=a.[order-id]) as total_itemprice, 
(select cast(a.[quantity-purchased] as float) * (sum(cast([shipping-price] as float)) +sum(cast([shipping-tax] as float)))  from [t_imp_3] where [order-id]=a.[order-id]) as total_shipping, 
(select cast(a.[quantity-purchased] as float) * sum(cast([item-tax] as float)) from [t_imp_3] where [order-id]=a.[order-id]) as total_tax, 
(select cast(a.[quantity-purchased] as float) * sum(cast([item-promotion-discount] as float))+sum(cast([ship-promotion-discount] as float)) from [t_imp_3] where [order-id]=a.[order-id]) as total_discount, 
'0.0', '0.0', 
(select cast(a.[quantity-purchased] as float) * (sum(cast([item-price] as float)) + sum(cast([shipping-price] as float)) +sum(cast([shipping-tax] as float)) +sum(cast([item-tax] as float))-(sum(cast([item-promotion-discount] as float))+sum(cast([ship-promotion-discount] as float)) ))  from [t_imp_3] where [order-id]=a.[order-id]) as total_sum, 
'', '', a.[buyer-name], '', a.[ship-phone-number], 
b.[recipient-name], '', b.[ship-address-1], b.[ship-address-2], b.[ship-city], 
b.[ship-state], b.[ship-postal-code], b.[ship-country], '', a.[sku], 
'', '', a.[product-name], a.[quantity-purchased], a.[item-price], 
a.[shipping-price] as [item_shipping_price], a.[shipping-tax] as [item_shipping_tax], a.[ship-promotion-discount], a.[item-tax], a.[item-promotion-discount], 
'0.0', '', '1', ''
from t_imp_3 a ,t_imp_4 b where a.[order-id]=b.[order-id] and a.[order-item-id]=b.[order-item-id]
and a.[order-id] not in (select order_id from t_biz_order);



/*ebay bbn*/
INSERT INTO [t_biz_order]
([FROMID], [Order_DAY], [batch], [order_id], [payments_date], 
[shipping_class], [shipping_name], [total_itemprice], 
[total_shipping], 
[total_tax], 
[total_discount], [total_giftwrap], [total_Gift_Certificates], 
[total_sum], 
[Notes], 
[ebay_userid], 
[buy_name], 
[email], 
[phone], 
[name], 
[ship_to_company], 
[addr1], 
[addr2], 
[city], 
[state], 
[zip], 
[country], 
[Global_Shipping_Reference_ID], [sku], [sys_sku], [item], [product_name], 
[qty], [item_price], [item_shipping_price], [item_shipping_tax], [item_shipping_discount], 
[item_us_tax], [item_discount], [item_giftwrap], [shipping_details], [status], 
[remark])
select 
'3', convert(varchar(8), getdate(), 112) as [Order_DAY], '', [Sales Record Number], [Paid on Date], 
[shipping service], [shipping service], cast(Quantity as float) * cast(replace([sale price],'$','') as float) , 
replace([Shipping and Handling],'$',''), 
replace([US Tax],'$',''), 
'0.0', '0.0', '0.0', 
replace([Total Price],'$',''), 
[Notes to yourself], 
[User Id], 
'', 
[Buyer Email], 
[Buyer Phone Number], 
[Buyer Fullname], 
'', 
[Ship To Address 1], 
[Ship To Address 2], 
[Ship To City], 
[Ship To State], 
[Ship To Zip], 
[Ship To Country], 
[Global Shipping Reference ID], [Item Number], [Custom Label], [Custom Label], [Item Title], 
[Quantity], replace([Sale Price],'$',''), '0.0', '0.0', '0.0', 
'0.0', '0.0', '0.0', '', '1', 
'' 
FROM [t_imp_5] where [item number] !=''  and [Paid on Date]!='' and [Sales Record Number] not in (select order_id from t_biz_order)
union
select
'3', convert(varchar(8), getdate(), 112) as [Order_DAY], '', [Sales Record Number], (select x.[Paid on Date] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Paid on Date], 
[shipping service], [shipping service], cast(Quantity as float) * cast(replace([sale price],'$','') as float) , 
(select replace(x.[Shipping and Handling],'$','') from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Shipping and Handling], 
(select replace(x.[US Tax],'$','') from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [US Tax], 
'0.0', '0.0', '0.0', 
(select x.[Total Price] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Total Price], 
(select x.[Notes to yourself] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Notes to yourself], 
(select x.[User Id] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [User Id], 
'', 
(select x.[Buyer Email] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Buyer Email], 
(select x.[Buyer Phone Number] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Buyer Phone Number], 
(select x.[Buyer Fullname] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Buyer Fullname], 
'', 
(select x.[Ship To Address 1] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 1], 
(select x.[Ship To Address 2] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 2], 
(select x.[Ship To City] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To City], 
(select x.[Ship To State] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To State], 
(select x.[Ship To Zip] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To Zip], 
(select x.[Ship To Country] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To Country], 
[Global Shipping Reference ID], [Item Number], [Custom Label], [Custom Label], [Item Title], 
[Quantity], replace([Sale Price],'$',''), '0.0', '0.0', '0.0', 
'0.0', '0.0', '0.0', '', '1', 
''
FROM [t_imp_5]  where [item number] !='' and [Paid on Date]='' and [Sales Record Number] not in (select order_id from t_biz_order);



/*ebay lisa*/
INSERT INTO [t_biz_order]
([FROMID], [Order_DAY], [batch], [order_id], [payments_date], 
[shipping_class], [shipping_name], [total_itemprice], 
[total_shipping], 
[total_tax], 
[total_discount], [total_giftwrap], [total_Gift_Certificates], 
[total_sum], 
[Notes], 
[ebay_userid], 
[buy_name], 
[email], 
[phone], 
[name], 
[ship_to_company], 
[addr1], 
[addr2], 
[city], 
[state], 
[zip], 
[country], 
[Global_Shipping_Reference_ID], [sku], [sys_sku], [item], [product_name], 
[qty], [item_price], [item_shipping_price], [item_shipping_tax], [item_shipping_discount], 
[item_us_tax], [item_discount], [item_giftwrap], [shipping_details], [status], 
[remark])
select 
'4', convert(varchar(8), getdate(), 112) as [Order_DAY], '', [Sales Record Number], [Paid on Date], 
[shipping service], [shipping service], cast(Quantity as float) * cast(replace([sale price],'$','') as float) , 
replace([Shipping and Handling],'$',''), 
replace([US Tax],'$',''), 
'0.0', '0.0', '0.0', 
replace([Total Price],'$',''), 
[Notes to yourself], 
[User Id], 
'', 
[Buyer Email], 
[Buyer Phone Number], 
[Buyer Fullname], 
'', 
[Ship To Address 1], 
[Ship To Address 2], 
[Ship To City], 
[Ship To State], 
[Ship To Zip], 
[Ship To Country], 
[Global Shipping Reference ID], [Item Number], [Custom Label], [Custom Label], [Item Title], 
[Quantity], replace([Sale Price],'$',''), '0.0', '0.0', '0.0', 
'0.0', '0.0', '0.0', '', '1', 
'' 
FROM [t_imp_6] where [item number] !=''  and [Paid on Date]!='' and [Sales Record Number] not in (select order_id from t_biz_order)
union
select
'4', convert(varchar(8), getdate(), 112) as [Order_DAY], '', [Sales Record Number], (select x.[Paid on Date] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Paid on Date], 
[shipping service], [shipping service], cast(Quantity as float) * cast(replace([sale price],'$','') as float) , 
(select replace(x.[Shipping and Handling],'$','') from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Shipping and Handling], 
(select replace(x.[US Tax],'$','') from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [US Tax], 
'0.0', '0.0', '0.0', 
(select x.[Total Price] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Total Price], 
(select x.[Notes to yourself] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Notes to yourself], 
(select x.[User Id] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [User Id], 
'', 
(select x.[Buyer Email] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Buyer Email], 
(select x.[Buyer Phone Number] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Buyer Phone Number], 
(select x.[Buyer Fullname] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Buyer Fullname], 
'', 
(select x.[Ship To Address 1] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 1], 
(select x.[Ship To Address 2] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 2], 
(select x.[Ship To City] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To City], 
(select x.[Ship To State] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To State], 
(select x.[Ship To Zip] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To Zip], 
(select x.[Ship To Country] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To Country], 
[Global Shipping Reference ID], [Item Number], [Custom Label], [Custom Label], [Item Title], 
[Quantity], replace([Sale Price],'$',''), '0.0', '0.0', '0.0', 
'0.0', '0.0', '0.0', '', '1', 
''
FROM [t_imp_6]  where [item number] !='' and [Paid on Date]='' and [Sales Record Number] not in (select order_id from t_biz_order);


/*ebay wmb*/
INSERT INTO [t_biz_order]
([FROMID], [Order_DAY], [batch], [order_id], [payments_date], 
[shipping_class], [shipping_name], [total_itemprice], 
[total_shipping], 
[total_tax], 
[total_discount], [total_giftwrap], [total_Gift_Certificates], 
[total_sum], 
[Notes], 
[ebay_userid], 
[buy_name], 
[email], 
[phone], 
[name], 
[ship_to_company], 
[addr1], 
[addr2], 
[city], 
[state], 
[zip], 
[country], 
[Global_Shipping_Reference_ID], [sku], [sys_sku], [item], [product_name], 
[qty], [item_price], [item_shipping_price], [item_shipping_tax], [item_shipping_discount], 
[item_us_tax], [item_discount], [item_giftwrap], [shipping_details], [status], 
[remark])
select 
'5', convert(varchar(8), getdate(), 112) as [Order_DAY], '', [Sales Record Number], [Paid on Date], 
[shipping service], [shipping service], cast(Quantity as float) * cast(replace([sale price],'$','') as float) , 
replace([Shipping and Handling],'$',''), 
replace([US Tax],'$',''), 
'0.0', '0.0', '0.0', 
replace([Total Price],'$',''), 
[Notes to yourself], 
[User Id], 
'', 
[Buyer Email], 
[Buyer Phone Number], 
[Buyer Fullname], 
'', 
[Ship To Address 1], 
[Ship To Address 2], 
[Ship To City], 
[Ship To State], 
[Ship To Zip], 
[Ship To Country], 
[Global Shipping Reference ID], [Item Number], [Custom Label], [Custom Label], [Item Title], 
[Quantity], replace([Sale Price],'$',''), '0.0', '0.0', '0.0', 
'0.0', '0.0', '0.0', '', '1', 
'' 
FROM [t_imp_7] where [item number] !=''  and [Paid on Date]!='' and [Sales Record Number] not in (select order_id from t_biz_order)
union
select
'5', convert(varchar(8), getdate(), 112) as [Order_DAY], '', [Sales Record Number], (select x.[Paid on Date] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Paid on Date], 
[shipping service], [shipping service], cast(Quantity as float) * cast(replace([sale price],'$','') as float) , 
(select replace(x.[Shipping and Handling],'$','') from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Shipping and Handling], 
(select replace(x.[US Tax],'$','') from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [US Tax], 
'0.0', '0.0', '0.0', 
(select x.[Total Price] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Total Price], 
(select x.[Notes to yourself] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Notes to yourself], 
(select x.[User Id] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [User Id], 
'', 
(select x.[Buyer Email] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Buyer Email], 
(select x.[Buyer Phone Number] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Buyer Phone Number], 
(select x.[Buyer Fullname] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Buyer Fullname], 
'', 
(select x.[Ship To Address 1] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 1], 
(select x.[Ship To Address 2] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 2], 
(select x.[Ship To City] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To City], 
(select x.[Ship To State] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To State], 
(select x.[Ship To Zip] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To Zip], 
(select x.[Ship To Country] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To Country], 
[Global Shipping Reference ID], [Item Number], [Custom Label], [Custom Label], [Item Title], 
[Quantity], replace([Sale Price],'$',''), '0.0', '0.0', '0.0', 
'0.0', '0.0', '0.0', '', '1', 
''
FROM [t_imp_7]  where [item number] !='' and [Paid on Date]='' and [Sales Record Number] not in (select order_id from t_biz_order);


/*WeMakeBeauty.com*/
INSERT INTO [t_biz_order]
([FROMID], [Order_DAY], [batch], [order_id], [payments_date], 
[shipping_class], 
[shipping_name], 
[total_itemprice], 
[total_shipping], [total_tax], 
[total_discount], 
[total_giftwrap], 
[total_Gift_Certificates], [total_sum], [Notes], [ebay_userid], [buy_name], 
[email], [phone], [name], [ship_to_company], [addr1], 
[addr2], [city], [state], [zip], [country], 
[sku], [sys_sku], [item], [product_name], [qty], 
[item_price], [item_shipping_price], [item_shipping_tax], [item_shipping_discount], [item_us_tax], 
[item_discount], [item_giftwrap], [status], [remark])
select 
'6' as [FROMID], convert(varchar(8), getdate(), 112) as [Order_DAY], '' as [batch], a.[orderid] as [order_id], a.[orderdate] as [payments_date], 
a.[shippingmethodid] as [shipping_class], 
(select top 1 [ShipService] from t_vol_shippingid where [shippingmethodid]=a.[shippingmethodid]) as [shipping_name], 
(select sum(cast(productprice as float)) from t_imp_9 where cast(productprice as float) > 0 and orderid=a.orderid) as [total_itemprice], 
a.[totalshippingcost] as [total_shipping], cast(salestax1 as float)+cast(salestax2 as float)+cast(salestax3 as float) as [total_tax], 
(select isnull(sum(cast(productprice as float)),0) from t_imp_9 where cast(productprice as float) < 0 and orderid=a.orderid) as [total_discount], 
(select isnull(sum(cast([giftwrap] as float)),0) from t_imp_9 where  orderid=a.orderid) as [total_giftwrap], 
(select isnull(sum(cast(productprice as float)),0) from t_imp_9 where orderid=a.orderid)+a.[totalshippingcost]+cast(salestax1 as float)+cast(salestax2 as float)+cast(salestax3 as float)+(select isnull(sum(cast([giftwrap] as float)),0) from t_imp_9 where  orderid=a.orderid)
 as [total_Gift_Certificates_sub], 
total_payment_received as [total_sum], a.[ordernotes]+' '+a.[order_comments] as [Notes], '' as [ebay_userid], billingfirstname+' '+billinglastname as [buy_name], 
'' as [email], a.[shipphonenumber] as [phone], a.[shipfirstname]+' '+a.[shiplastname] as [name], a.[shipcompanyname] as [ship_to_company], a.[shipaddress1] as [addr1], 
a.[shipaddress2] as [addr2], a.[shipcity] as [city], a.[shipstate] as [state], a.[shippostalcode] as [zip], a.[shipcountry] as [country], 
b.[productcode] as [sku], b.[productcode] as [sys_sku], case when substring(b.[productcode],1,3)='11-' then  Substring(b.[productcode],4,len(b.[productcode])-3) ELSE b.[productcode] END as [item], b.[productname] as [product_name], b.[quantity] as [qty], 
b.[productprice] as [item_price], a.[totalshippingcost] as [item_shipping_price], '0.0' as [item_shipping_tax], '0.0' as [item_shipping_discount], cast(salestax1 as float)+cast(salestax2 as float)+cast(salestax3 as float) as [item_us_tax], 
'0.0' as [item_discount], b.[giftwrap] as [item_giftwrap], '1' as [status], '' as [remark]
from t_imp_8 a,
(select orderid,productcode,productname,productprice,giftwrap,sum(cast(quantity as float)) as quantity from t_imp_9
group by orderid,productcode,productname,productprice,giftwrap) as b
where a.[orderid]=b.[orderid] and a.[orderid] not in (select order_id from t_biz_order);
/*
select 
'6' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch],a.[orderid],a.[orderdate],
b.[productcode],b.[productcode],b.[productcode],b.[productname],b.[quantity],
b.[productprice],a.[totalshippingcost],cast(salestax1 as float)+cast(salestax2 as float)+cast(salestax3 as float) as [us-tax],a.[shippingmethodid],'',
'',a.[shipphonenumber],a.[shipfirstname]+' '+a.[shiplastname],a.[shipcompanyname],a.[shipaddress1],
a.[shipaddress2],a.[shipcity],a.[shipstate],a.[shippostalcode],a.[shipcountry],
'',total_payment_received,'',a.[ordernotes],'',b.[item_giftwrap],
'','1','',billingfirstname+' '+billinglastname
from t_imp_8 a,t_imp_9 b where a.[orderid]=b.[orderid];
*/


/*Over Stock*/
INSERT INTO [t_biz_order]
(
[FROMID], [Order_DAY], [batch], [order_id], [payments_date], 
[shipping_class], [shipping_name], 
[total_itemprice], 
[total_shipping], 
[total_tax], [total_discount], [total_giftwrap], [total_Gift_Certificates], [total_sum], 
[Notes], [ebay_userid], [buy_name], [email], [phone], 
[name], [ship_to_company], [addr1], [addr2], [city], 
[state], [zip], [country], [sku], [sys_sku], 
[item], 
[product_name], 
[qty], [item_price], [item_shipping_price], [item_shipping_tax], [item_shipping_discount], 
[item_us_tax], [item_discount], [item_giftwrap], [shipping_details], [status], 
[Retailer_Order_Number],
[Return_Contact_Name],
[Return_Address_1],
[Return_Address_2],
[Return_City],
[Return_State_Or_Province],
[Return_Postal_Code],
[Return_Country_Code],
[Return_Phone],
[Return_Alternate_Phone],
[remark],[retailer_first_cost],[SOFS_Order_Line_Number]
)
select 
'7' as [FROMID], convert(varchar(8), getdate(), 112) as [Order_DAY], '' as [batch], [SOFS Order Number], [Order Date], 
[Shipping Service Level Small Parcel],[Shipping Service Level Small Parcel],
case when [item Price]='Replacement' then '0.0' Else (select sum(cast(a.Quantity as float) * cast(a.[item Price] as float)) from [t_imp_10] as a where a.[SOFS Order Number]=[t_imp_10].[SOFS Order Number]) End, 
(select sum(cast(a.[Retailer Additional Shipping Cost] as float)) from [t_imp_10] as a where a.[SOFS Order Number]=[t_imp_10].[SOFS Order Number]), 
'0.0', '0.0', '0.0', '0.0', 
case when [item Price]='Replacement' then '0.0' Else (select sum(cast(a.Quantity as float) * cast(a.[item Price] as float)) + sum(cast(a.[Retailer Additional Shipping Cost] as float)) from [t_imp_10] as a where a.[SOFS Order Number]=[t_imp_10].[SOFS Order Number]) End, 
'', '', '', '', [Ship Phone], 
[Ship Contact Name],'',[Ship Address 1],RTRIM(isnull([Ship Address 2],'')+' '+isnull([Ship Address 3],'')),[Ship City],
[Ship State Or Province],[Ship Postal Code],[Ship Country Code],[SOFS SKU],[Supplier SKU],
[Supplier SKU],
[Item Name],
[Quantity],[Item Price],[Retailer Additional Shipping Cost],'0.0','0.0',
'0.0','0.0','0.0','','1',
[Retailer Order Number],
[Return Contact Name],
[Return Address 1],
[Return Address 2]+' '
[Return Address 3],
[Return City],
[Return State Or Province],
[Return Postal Code],
[Return Country Code],
[Return Phone],
[Return Alternate Phone],
'',[retailer first cost],[SOFS Order Line Number]
FROM [t_imp_10] where [SOFS Order Number] not in (select order_id from t_biz_order);
/*
INSERT INTO [t_biz_order]
(
[FROMID], [Order_DAY], [batch], [order_id], [payments_date], 
[shipping_class], [shipping_name], 
[total_itemprice], 
[total_shipping], 
[total_tax], [total_discount], [total_giftwrap], [total_Gift_Certificates], [total_sum], 
[Notes], [ebay_userid], [buy_name], [email], [phone], 
[name], [ship_to_company], [addr1], [addr2], [city], 
[state], [zip], [country], [sku], [sys_sku], 
[item], 
[product_name], 
[qty], [item_price], [item_shipping_price], [item_shipping_tax], [item_shipping_discount], 
[item_us_tax], [item_discount], [item_giftwrap], [shipping_details], [status], 
[remark]
)
select 
'7' as [FROMID], convert(varchar(8), getdate(), 112) as [Order_DAY], '' as [batch], [Order #], [Overstock Order Date], 
[Ship Method], [Ship Method], 
case when [Unit Price]='Replacement' then '0.0' Else (select sum(cast(a.Quantity as float) * cast(a.[Unit Price] as float)) from [t_imp_10] as a where a.[Order #]=[t_imp_10].[Order #]) End, 
case when [Unit Price]='Replacement' then '0.0' Else (select sum(cast(a.[Shipping Cost] as float)) from [t_imp_10] as a where a.[Order #]=[t_imp_10].[Order #]) End, 
'0.0', '0.0', '0.0', '0.0', 
case when [Unit Price]='Replacement' then '0.0' Else (select sum(cast(a.Quantity as float) * cast(a.[Unit Price] as float)) + sum(cast(a.[Shipping Cost] as float)) from [t_imp_10] as a where a.[Order #]=[t_imp_10].[Order #]) End, 
'', '', '', '', '', 
'', '', '', '', '', 
'', '', '', [Overstock SKU], [Partner SKU], 
[Partner SKU], 
(select [Product Name] from [t_ov_Active_Listing_Table] where [Partner SKU]=[t_imp_10].[Partner SKU]), 
[Quantity], [Unit Price], [Shipping Cost], '0.0', '0.0', 
'0.0', '0.0', '0.0', [Shipping Details], '1', 
''
FROM [t_imp_10];
*/


/*buy.com*/
INSERT INTO [t_biz_order]
([FROMID], [Order_DAY], [batch], [order_id], [payments_date], 
[shipping_class], [shipping_name], 
[total_itemprice], 
[total_shipping], 
[total_tax], 
[total_discount],[total_giftwrap], [total_Gift_Certificates], 
[total_sum], 
[Notes], [ebay_userid], 
[buy_name], [email], [phone], [name], [ship_to_company], 
[addr1], [addr2], [city], [state], [zip], 
[country], [sku], [sys_sku], [item], [product_name], 
[qty], [item_price], [item_shipping_price], [item_shipping_tax], [item_shipping_discount], 
[item_us_tax], [item_discount], [item_giftwrap], [status], [remark])
select 
'8' as [FROMID], convert(varchar(8), getdate(), 112) as [Order_DAY], '' as [batch], [OrderId], [OrderDate], 
[ShippingMethodId], 
(select top 1 [ShipService] from t_vol_shippingid where [ShippingMethodId]=[t_imp_11].[ShippingMethodId]) as [shipping_name], 
(select sum((cast(a.[Qty Ordered] as float)-cast(a.[Qty Shipped] as float)-cast(a.[Qty Cancelled] as float)) * cast(a.[Price] as float)) from [t_imp_11] as a where a.[SellerShopperNumber]=[t_imp_11].[SellerShopperNumber]), 
(select sum(cast(a.[ShippingCost] as float)) from [t_imp_11] as a where a.[SellerShopperNumber]=[t_imp_11].[SellerShopperNumber]), 
(select sum(cast(a.[TaxCost] as float)) from [t_imp_11] as a where a.[SellerShopperNumber]=[t_imp_11].[SellerShopperNumber]),
'0.0','0.0', '0.0', 
(select sum((cast(a.[Qty Ordered] as float)-cast(a.[Qty Shipped] as float)-cast(a.[Qty Cancelled] as float)) * cast(a.[Price] as float)) +sum(cast(a.[ShippingCost] as float))+sum(cast(a.[TaxCost] as float)) from [t_imp_11] as a where a.[SellerShopperNumber]=[t_imp_11].[SellerShopperNumber]), 
'', '', 
billtofirstname+' '+billtolastname, '', [billtophone], [ShipToName], [ShipToCompany], 
[ShipToStreet1], [ShipToStreet2], [ShipToCity], [ShipToState], [ShipToZipCode], 
'', [Sku], [ReferenceId], case when substring([ReferenceId],1,3)='11-' then  Substring([ReferenceId],4,len([ReferenceId])-3) ELSE [ReferenceId] END, [ItemName], 
cast([Qty Ordered] as float)-cast([Qty Shipped] as float)-cast([Qty Cancelled] as float) as qty, [Price], [ShippingCost], '0.0', '0.0', 
[TaxCost], '0.0', '0.0', '1', ''
FROM [t_imp_11] where [OrderId] not in (select order_id from t_biz_order);



/*new egg*/
INSERT INTO [t_biz_order]
([FROMID], [Order_DAY], [batch], [order_id], [payments_date], 
[shipping_class], [shipping_name], 
[total_itemprice], 
[total_shipping], [total_tax], [total_discount], [total_giftwrap], [total_Gift_Certificates], 
[total_sum], [Notes], [ebay_userid], [buy_name], [email], 
[phone], [name], [ship_to_company], [addr1], [addr2], 
[city], [state], [zip], [country], [sku], 
[sys_sku], [item], 
[product_name], 
[qty], [item_price], [item_shipping_price], [item_shipping_tax], [item_shipping_discount], 
[item_us_tax], [item_discount], [item_giftwrap], [shipping_details], [status], 
[remark])
select 
'9' as [FROMID], convert(varchar(8), getdate(), 112) as [Order_DAY], '' as [batch], [Order Number], [Order Date & Time], 
[Order Shipping Method], [Order Shipping Method], 
(select sum(cast(a.[Item Quantity Ordered] as float) * cast(a.[Item Unit Price]  as float) ) from [t_imp_13] as a where a.[Order Number]=[t_imp_13].[Order Number] ) as [total_itemprice], 
[Order Shipping Total], '0.0', '0.0', '0.0', '0.0', 
[Order Total], '', '', [Ship To Name], '', 
[Ship To Phone Number], [Ship To Name], [Ship To Company], [Ship To Address Line 1], [Ship To Address Line 2], 
[Ship To City], [Ship To State], [Ship To Zipcode], [Ship to Country], [Item Newegg #], 
[Item Seller Part #], case when substring([Item Seller Part #],1,2)='11' then  Substring([Item Seller Part #],3,len([Item Seller Part #])-2)ELSE [Item Seller Part #] END, 
(select [title] from [t_vol_Listing_Table] where '11'+[custom label]=t_imp_13.[Item Seller Part #]), 
[Item Quantity Ordered], [Item Unit Price], [Item Shipping Charge], '0.0', '0.0', 
'0.0', '0.0', '0.0', '', '1', 
''
FROM [t_imp_13] where  [Order Status]='Unshipped' and  [Order Number] not in (select order_id from t_biz_order);
";

            #endregion

            cmd = new OleDbCommand(ls_big_sql, myConn);
            cmd.ExecuteNonQuery();

            //update amz item data
            string ls_sku_item=@"
update [t_biz_order] set sys_sku =
case when substring(sku,1,1)='A' then  Substring(sku,2,len(sku)-1)
when substring(sku,1,1)='B' then  Substring(sku,2,len(sku)-1)
when substring(sku,1,1)='C' then  Substring(sku,2,len(sku)-1)
when substring(sku,1,1)='D' then  Substring(sku,2,len(sku)-1)
when substring(sku,1,1)='E' then  Substring(sku,2,len(sku)-1)
ELSE sku END where fromid='1' and [item]='';
update [t_biz_order] set item =(select item from [t_amazon_home_full_sku_list] where [SKU(meaningless)]=[t_biz_order].[sys_sku])
where fromid ='1' and [item]='';
update [t_biz_order] set sys_sku =
case when substring(sku,1,2)='10' then  Substring(sku,3,len(sku)-2)
when substring(sku,1,2)='20' then  Substring(sku,3,len(sku)-2)
when substring(sku,1,3)='30' then  Substring(sku,3,len(sku)-2)
when substring(sku,1,4)='40' then  Substring(sku,3,len(sku)-2)
when substring(sku,1,5)='50' then  Substring(sku,3,len(sku)-2)
ELSE sku END where fromid='2' and [item]='';
update [t_biz_order] set item =(select item from [t_amazon_2_full_sku_list] where [SKU(meaningless)]=[t_biz_order].[sys_sku])
where fromid ='2' and [item]='';
";

            cmd = new OleDbCommand(ls_sku_item, myConn);
            cmd.ExecuteNonQuery();


            string ls_from7_item=@"
update t_biz_order set [payments_date]=(select top 1 [payments_date]from t_biz_order a where a.order_id=t_biz_order.order_id and [payments_date]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [payments_date]='';
update t_biz_order set [shipping_class]=(select top 1 [shipping_class]from t_biz_order a where a.order_id=t_biz_order.order_id and [shipping_class]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [shipping_class]='';
update t_biz_order set [shipping_name]=(select top 1 [shipping_name]from t_biz_order a where a.order_id=t_biz_order.order_id and [shipping_name]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [shipping_name]='';
update t_biz_order set [phone]=(select top 1 [phone]from t_biz_order a where a.order_id=t_biz_order.order_id and [phone]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [phone]='';
update t_biz_order set [name]=(select top 1 [name]from t_biz_order a where a.order_id=t_biz_order.order_id and [name]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [name]='';
update t_biz_order set [ship_to_company]=(select top 1 [ship_to_company]from t_biz_order a where a.order_id=t_biz_order.order_id and [ship_to_company]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [ship_to_company]='';
update t_biz_order set [addr1]=(select top 1 [addr1]from t_biz_order a where a.order_id=t_biz_order.order_id and [addr1]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [addr1]='';
update t_biz_order set [addr2]=(select top 1 [addr2]from t_biz_order a where a.order_id=t_biz_order.order_id and [addr2]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [addr2]='';
update t_biz_order set [city]=(select top 1 [city]from t_biz_order a where a.order_id=t_biz_order.order_id and [city]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [city]='';
update t_biz_order set [state]=(select top 1 [state]from t_biz_order a where a.order_id=t_biz_order.order_id and [state]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [state]='';
update t_biz_order set [state]=(select top 1 [state]from t_biz_order a where a.order_id=t_biz_order.order_id and [state]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [state]='';
update t_biz_order set [zip]=(select top 1 [zip]from t_biz_order a where a.order_id=t_biz_order.order_id and [zip]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [zip]='';
update t_biz_order set [country]=(select top 1 [country]from t_biz_order a where a.order_id=t_biz_order.order_id and [country]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [country]='';
update t_biz_order set [Retailer_Order_Number]=(select top 1 [Retailer_Order_Number]from t_biz_order a where a.order_id=t_biz_order.order_id and [Retailer_Order_Number]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [Retailer_Order_Number]='';
update t_biz_order set [Return_Contact_Name]=(select top 1 [Return_Contact_Name]from t_biz_order a where a.order_id=t_biz_order.order_id and [Return_Contact_Name]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [Return_Contact_Name]='';
update t_biz_order set [Return_Address_1]=(select top 1 [Return_Address_1]from t_biz_order a where a.order_id=t_biz_order.order_id and [Return_Address_1]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [Return_Address_1]='';
update t_biz_order set [Return_Address_2]=(select top 1 [Return_Address_2]from t_biz_order a where a.order_id=t_biz_order.order_id and [Return_Address_2]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [Return_Address_2]='';
update t_biz_order set [Return_City]=(select top 1 [Return_City]from t_biz_order a where a.order_id=t_biz_order.order_id and [Return_City]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [Return_City]='';
update t_biz_order set [Return_State_Or_Province]=(select top 1 [Return_State_Or_Province]from t_biz_order a where a.order_id=t_biz_order.order_id and [Return_State_Or_Province]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [Return_State_Or_Province]='';
update t_biz_order set [Return_Postal_Code]=(select top 1 [Return_Postal_Code]from t_biz_order a where a.order_id=t_biz_order.order_id and [Return_Postal_Code]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [Return_Postal_Code]='';
update t_biz_order set [Return_Country_Code]=(select top 1 [Return_Country_Code]from t_biz_order a where a.order_id=t_biz_order.order_id and [Return_Country_Code]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [Return_Country_Code]='';
update t_biz_order set [Return_Phone]=(select top 1 [Return_Phone]from t_biz_order a where a.order_id=t_biz_order.order_id and [Return_Phone]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [Return_Phone]='';
update t_biz_order set [Return_Alternate_Phone]=(select top 1 [Return_Alternate_Phone]from t_biz_order a where a.order_id=t_biz_order.order_id and [Return_Alternate_Phone]!='')where fromid='7' and order_day=convert(varchar(8), getdate(), 112) and [Return_Alternate_Phone]='';
";

            cmd = new OleDbCommand(ls_from7_item, myConn);
            cmd.ExecuteNonQuery();

            myConn.Close();

            show_totoal();


        }

        private void insert_access_overstock()
        {
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();
            string ls_get_ov_order = @"select [Retailer_Order_Number],convert(char(10),[payments_date],101) pd1,convert(char(10),[payments_date],101) as pd2,qty,sku,item,
name + '        ' + addr1 + '        ' + isnull(addr2, '') + '        ' + city + '        ' + state + '-' + zip as address,
[retailer_first_cost] as cost,
[item_shipping_price],[item_price],[shipping_name],'New' as [status],[order_id],SOFS_Order_Line_Number
from t_biz_order where fromid = '7' and order_day = convert(varchar(8), getdate(), 112)
order by item,[shipping_name]";
            OleDbDataAdapter adapter = new OleDbDataAdapter(ls_get_ov_order, myConn);
            DataTable dt_access = new DataTable();
            adapter.Fill(dt_access);
            
            OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Z:\Invoicing Frontends\Over Stock Invoicing_FrontEnd_v1.accdb");
            conn.Open();

            string clearSql = "delete * from [Open Orders]";
            OleDbCommand cmd = new OleDbCommand(clearSql, conn);
            cmd.ExecuteNonQuery();

            for (int i = 0; i < dt_access.Rows.Count; i++)
            {
                    string ls_Retailer_Order_Number = dt_access.Rows[i]["Retailer_Order_Number"].ToString();
                    string ls_SOFS_Order_Line_Number = dt_access.Rows[i]["SOFS_Order_Line_Number"].ToString();
                    string ls_oid = dt_access.Rows[i]["Retailer_Order_Number"].ToString();
                    string ls_pd1 = dt_access.Rows[i]["pd1"].ToString();
                    string ls_pd2 = dt_access.Rows[i]["pd2"].ToString();
                    string ls_qty = dt_access.Rows[i]["qty"].ToString();
                    string ls_sku = dt_access.Rows[i]["sku"].ToString();
                    string ls_item = dt_access.Rows[i]["item"].ToString();
                    string ls_address = dt_access.Rows[i]["address"].ToString();
                    string ls_cost = dt_access.Rows[i]["cost"].ToString();
                    string ls_item_shipping_price = dt_access.Rows[i]["item_shipping_price"].ToString();
                    string ls_item_price = dt_access.Rows[i]["item_price"].ToString();
                    string ls_shipping_name = dt_access.Rows[i]["shipping_name"].ToString();
                    string ls_status = dt_access.Rows[i]["status"].ToString();
                    string ls_SOFS_ID = dt_access.Rows[i]["order_id"].ToString();

                    string ls_insert = "insert into[open orders] ([Order #],[Sent To Warehouse],[Overstock Order Date],[Quantity],[Overstock SKU],[Partner SKU],[Shipping Details],[Overstock Cost],[Shipping Cost],[Unit Price],[Ship Method],[Status],[SOFS_Order_Number],[SOFS_Order_Line_Number]) " +
                        "values('" + ls_oid + "','" + ls_pd1 + "','" + ls_pd2 + "'," + ls_qty + ",'" + ls_sku + "','"
                        + ls_item + "','" + ls_address + "'," + ls_cost + "," + ls_item_shipping_price + ",'" + ls_item_price + "','" + ls_shipping_name + "','" + ls_status + "','" + ls_SOFS_ID + "','" + ls_SOFS_Order_Line_Number + "')";
                    cmd = new OleDbCommand(ls_insert, conn);
                    cmd.ExecuteNonQuery();
                
            }


            dt_access = null;

            //clearSql = "delete * from [AUI]";
            //cmd = new OleDbCommand(clearSql, conn);
            //cmd.ExecuteNonQuery();

            string ls_un = @"select [Retailer_Order_Number],convert(char(10),[payments_date],101) pd1,convert(char(10),[payments_date],101) as pd2,qty,sku,item,
name + '        ' + addr1 + '        ' + isnull(addr2, '') + '        ' + city + '        ' + state + '-' + zip as address,
[retailer_first_cost] as cost,
[item_shipping_price],[item_price],[shipping_name],'New' as [status],[order_id],SOFS_Order_Line_Number
from t_biz_order where fromid = '7' and status=1 and order_day != convert(varchar(8), getdate(), 112)
order by item,[shipping_name]";
            adapter = new OleDbDataAdapter(ls_un, myConn);
            DataTable dt_aui = new DataTable();
            adapter.Fill(dt_aui);

            for (int i = 0; i < dt_aui.Rows.Count; i++)
            {
                string ls_Retailer_Order_Number = dt_aui.Rows[i]["Retailer_Order_Number"].ToString();
                string ls_SOFS_Order_Line_Number = dt_aui.Rows[i]["SOFS_Order_Line_Number"].ToString();
                string ls_oid = dt_aui.Rows[i]["Retailer_Order_Number"].ToString();
                string ls_pd1 = dt_aui.Rows[i]["pd1"].ToString();
                string ls_pd2 = dt_aui.Rows[i]["pd2"].ToString();
                string ls_qty = dt_aui.Rows[i]["qty"].ToString();
                string ls_sku = dt_aui.Rows[i]["sku"].ToString();
                string ls_item = dt_aui.Rows[i]["item"].ToString();
                string ls_address = dt_aui.Rows[i]["address"].ToString();
                string ls_cost = dt_aui.Rows[i]["cost"].ToString();
                string ls_item_shipping_price = dt_aui.Rows[i]["item_shipping_price"].ToString();
                string ls_item_price = dt_aui.Rows[i]["item_price"].ToString();
                string ls_shipping_name = dt_aui.Rows[i]["shipping_name"].ToString();
                string ls_status = dt_aui.Rows[i]["status"].ToString();
                string ls_SOFS_ID = dt_aui.Rows[i]["order_id"].ToString();

                string ls_insert = "insert into [open orders] ([Order #],[Sent To Warehouse],[Overstock Order Date],[Quantity],[Overstock SKU],[Partner SKU],[Shipping Details],[Overstock Cost],[Shipping Cost],[Unit Price],[Ship Method],[Status],[SOFS_Order_Number],[SOFS_Order_Line_Number]) " +
                    "values('" + ls_oid + "','" + ls_pd1 + "','" + ls_pd2 + "'," + ls_qty + ",'" + ls_sku + "','"
                    + ls_item + "','" + ls_address + "'," + ls_cost + "," + ls_item_shipping_price + ",'" + ls_item_price + "','" + ls_shipping_name + "','" + ls_status + "','" + ls_SOFS_ID + "','" + ls_SOFS_Order_Line_Number + "')";
                cmd = new OleDbCommand(ls_insert, conn);
                cmd.ExecuteNonQuery();

            }

            dt_aui = null;
            conn.Close();

            myConn.Close();
            this.Info("Invoice already generated!");

        }

        private void txButton1_Click(object sender, EventArgs e)
        {
            //insert_access_overstock();

            #region create invoice head
            string ls_dir = "z:\\Invoice Printing\\" + DateTime.Now.ToString("yyyy-MM") + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
            string ls_style_head = "";


            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();
            string sqlstr = "select fromid,(select site_code from t_base_from where fromid=[t_biz_order].FROMID) as site_code from [t_biz_order]  where status=1  and order_day=convert(varchar(8), getdate(), 112) group by fromid ;";
            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataTable dt_invoice_head = new DataTable();
            adapter.Fill(dt_invoice_head);
            for (int i_file = 0; i_file < dt_invoice_head.Rows.Count; i_file++)
            {
                string ls_fromid = dt_invoice_head.Rows[i_file]["fromid"].ToString();
                string ls_site_code = dt_invoice_head.Rows[i_file]["site_code"].ToString();

                switch (ls_fromid)
                {
                    case "1":
                        ls_style_head = "<STYLE TYPE='text/css'><!--BODY{font-family:arial;width: 720px;text-align:top;}table{font-size:16px;border-collapse:collapse;width: 720px;}.itemr{height:65px;vertical-align:top;}.bdout{border-top:1px solid black;border-bottom:1px solid black;border-left:1px solid black;border-right:1px solid black;}.bdside{border-left:1px solid black;border-right:1px solid black;}.bdleft{border-left:1px solid black;}.bdright{border-right:1px solid black;}.bdtop{border-top:1px solid black;}.bdbotoom{border-bottom:1px solid black;}--></style>";
                        break;
                    case "2":
                        ls_style_head = "<STYLE TYPE='text/css'><!--BODY{font-family:arial;width: 720px;text-align:top;}table{font-size:16px;border-collapse:collapse;width: 720px;}.itemr{height:65px;vertical-align:top;}.bdout{border-top:1px solid black;border-bottom:1px solid black;border-left:1px solid black;border-right:1px solid black;}.bdside{border-left:1px solid black;border-right:1px solid black;}.bdleft{border-left:1px solid black;}.bdright{border-right:1px solid black;}.bdtop{border-top:1px solid black;}.bdbotoom{border-bottom:1px solid black;}--></style>";
                        break;
                    case "3":
                        ls_style_head = "<STYLE TYPE='text/css'><!--BODY{font-family:arial;width: 730px;text-align:top;}table{font-size:16px;border-collapse:collapse;width: 700px;}.itemr{height:65px;vertical-align:top;}.bdout{border-top:1px solid black;border-bottom:1px solid black;border-left:1px solid black;border-right:1px solid black;}.bdside{border-left:1px solid black;border-right:1px solid black;}.bdleft{border-left:1px solid black;}.bdright{border-right:1px solid black;}.bdtop{border-top:1px solid black;}.bdbotoom{border-bottom:1px solid black;}--></style>";
                        break;
                    case "4":
                        ls_style_head = "<STYLE TYPE='text/css'><!--BODY{font-family:arial;width: 730px;text-align:top;}table{font-size:16px;border-collapse:collapse;width: 700px;}.itemr{height:65px;vertical-align:top;}.bdout{border-top:1px solid black;border-bottom:1px solid black;border-left:1px solid black;border-right:1px solid black;}.bdside{border-left:1px solid black;border-right:1px solid black;}.bdleft{border-left:1px solid black;}.bdright{border-right:1px solid black;}.bdtop{border-top:1px solid black;}.bdbotoom{border-bottom:1px solid black;}--></style>";
                        break;
                    case "5":
                        ls_style_head = "<STYLE TYPE='text/css'><!--BODY{font-family:arial;width: 730px;text-align:top;}table{font-size:16px;border-collapse:collapse;width: 700px;}.itemr{height:65px;vertical-align:top;}.bdout{border-top:1px solid black;border-bottom:1px solid black;border-left:1px solid black;border-right:1px solid black;}.bdside{border-left:1px solid black;border-right:1px solid black;}.bdleft{border-left:1px solid black;}.bdright{border-right:1px solid black;}.bdtop{border-top:1px solid black;}.bdbotoom{border-bottom:1px solid black;}--></style>";
                        break;
                    case "6":
                        ls_style_head = "<STYLE TYPE='text/css'><!--BODY{font-family:arial;width: 720px;text-align:top;}table{font-size:16px;border-collapse:collapse;width: 720px;}.itemr{height:65px;vertical-align:top;}.bdout{border-top:1px solid black;border-bottom:1px solid black;border-left:1px solid black;border-right:1px solid black;}.bdside{border-left:1px solid black;border-right:1px solid black;}.bdleft{border-left:1px solid black;}.bdright{border-right:1px solid black;}.bdtop{border-top:1px solid black;}.bdbotoom{border-bottom:1px solid black;}--></style>";
                        break;
                    case "7":
                        ls_style_head = "<style type='text/css'>table{border-collapse:collapse;-webkit-print-color-adjust: exact;}.tb_item table, .tb_item th, .tb_item td {border: 1px solid black;} .ft1{font-size:25px;font-family:Helvetica;color:#000000;font-weight:bold;}" +
                            ".ft2{font-size:16px;font-family:Helvetica;color:#000000;padding-left:4px;}.ft3{font-size:16px;font-family:Helvetica;color:#000000;font-weight: bold;} " +
                            ".ft4{font-size:17px;font-family:Helvetica;color:#000000;padding-left:15px;p{ font-size:14px;font-family:Helvetica;color:#000000;margin:0 auto;padding-left:4px}body {  MARGIN: 0px;PADDING: 0px;}</style>";
                        break;
                    case "8":
                        ls_style_head = "<STYLE TYPE='text/css'><!--BODY{font-family:arial;width: 720px;text-align:top;}table{font-size:16px;border-collapse:collapse;width: 720px;}.itemr{height:65px;vertical-align:top;}.bdout{border-top:1px solid black;border-bottom:1px solid black;border-left:1px solid black;border-right:1px solid black;}.bdside{border-left:1px solid black;border-right:1px solid black;}.bdleft{border-left:1px solid black;}.bdright{border-right:1px solid black;}.bdtop{border-top:1px solid black;}.bdbotoom{border-bottom:1px solid black;}--></style>";
                        break;
                    case "9":
                        ls_style_head = "<STYLE TYPE='text/css'><!--BODY{font-family:arial;width: 720px;text-align:top;}table{font-size:16px;border-collapse:collapse;width: 720px;}.itemr{height:65px;vertical-align:top;}.bdout{border-top:1px solid black;border-bottom:1px solid black;border-left:1px solid black;border-right:1px solid black;}.bdside{border-left:1px solid black;border-right:1px solid black;}.bdleft{border-left:1px solid black;}.bdright{border-right:1px solid black;}.bdtop{border-top:1px solid black;}.bdbotoom{border-bottom:1px solid black;}--></style>";
                        break;
                }
                DirectoryInfo d_dir = new DirectoryInfo(ls_dir);
                if (!d_dir.Exists)
                {
                    d_dir.Create();
                }

                string ls_file = ls_dir + "\\" + ls_site_code + ".html";
                using (FileStream fs = File.Open(ls_file, FileMode.Create))
                {
                    byte[] b = System.Text.Encoding.Default.GetBytes(ls_style_head);
                    fs.Write(b, 0, b.Length);
                    fs.Close();
                    ls_style_head = "";
                }
            }
            dt_invoice_head = null;

            #endregion


            sqlstr = @"
select fromid,[order_id],min(item) as ordersku
from [t_biz_order]  where status=1  and order_day=convert(varchar(8), getdate(), 112) and  invoice is null group by fromid,[order_id]
order by fromid,ordersku
";
            adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataTable dt_query_max = new DataTable();
            adapter.Fill(dt_query_max);
            for (int i_file = 0; i_file < dt_query_max.Rows.Count; i_file++)
            {

                string ls_create_invoice_id_main = dt_query_max.Rows[i_file]["order_id"].ToString();

                //get invoice
                string ls_invoce = "";

                sqlstr = "select max([INVOICE]) as INVOICE from [t_biz_order] where substring([INVOICE],1,6)=substring(convert(varchar(8), getdate(), 112),3,6) ";
                adapter = new OleDbDataAdapter(sqlstr, myConn);
                DataTable dt_max = new DataTable();
                adapter.Fill(dt_max);

                if (dt_max.Rows.Count > 0 && dt_max.Rows[0]["INVOICE"].ToString().Length > 6)
                {
                    ls_invoce = DateTime.Now.ToString("yyMMdd") + (int.Parse(dt_max.Rows[0]["INVOICE"].ToString().Substring(6, 4)) + 1).ToString("D4");
                }
                else
                {
                    ls_invoce = DateTime.Now.ToString("yyMMdd") + "0001";
                }
                dt_max = null;

                OleDbCommand cmd = new OleDbCommand("update t_biz_order set invoice='" + ls_invoce + "' where [order_id]='" + ls_create_invoice_id_main + "' and invoice is null", myConn);
                cmd.ExecuteNonQuery();

                dt_max = null;
            }


            sqlstr = @"
select fromid,(select site_code from t_base_from where fromid=[t_biz_order].FROMID) as site_code,[order_id],count(oid) as nb,
max(total_sum) as total_sum,
max(total_tax) as total_tax,
max(total_shipping) as total_shipping,
max(total_discount) as total_discount,
max(total_giftwrap) as total_giftwrap,
max(total_Gift_Certificates) as total_Gift_Certificates,
min(item) as ordersku,
min(INVOICE) as bar_code,
max(Notes) as Notes
from[t_biz_order]  where status=1  and order_day=convert(varchar(8), getdate(), 112)   group by fromid,[order_id]
order by fromid,bar_code
";

            //and fromid=7


            adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataTable dt_order = new DataTable();
            adapter.Fill(dt_order);
            for (int i_order = 0; i_order < dt_order.Rows.Count; i_order++)
            {
                #region get invoice main value
                string ls_dir_bc = ls_dir + "\\barcode\\";
                DirectoryInfo d_dir = new DirectoryInfo(ls_dir_bc);
                if (!d_dir.Exists)
                {
                    d_dir.Create();
                }



                string ls_fromid = dt_order.Rows[i_order]["fromid"].ToString();
                string ls_bc_flag = "Y";
                //if (ls_fromid == "3" || ls_fromid == "4" || ls_fromid == "5" || ls_fromid == "7")
                //{
                //    ls_bc_flag = "Y";
                //}
                string ls_site_code = dt_order.Rows[i_order]["site_code"].ToString();
                string ls_order_id_main = dt_order.Rows[i_order]["order_id"].ToString();

                string ls_bar_code = dt_order.Rows[i_order]["bar_code"].ToString();
                Bitmap oBmp = GetCode39(ls_bar_code, ls_bc_flag);
                oBmp.Save(ls_dir_bc + ls_bar_code + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                oBmp.Dispose();

                string ls_nb = dt_order.Rows[i_order]["nb"].ToString();
                string ls_main_notes = dt_order.Rows[i_order]["Notes"].ToString();
                string ls_main_total_sum = "$" + dt_order.Rows[i_order]["total_sum"].ToString();
                string ls_main_total_tax = "" + dt_order.Rows[i_order]["total_tax"].ToString();
                string ls_main_total_shipping = "" + dt_order.Rows[i_order]["total_shipping"].ToString();
                string ls_main_total_discount = "" + dt_order.Rows[i_order]["total_discount"].ToString();
                string ls_main_total_giftwrap = "" + dt_order.Rows[i_order]["total_giftwrap"].ToString();
                string ls_main_total_Gift_Certificates = "" + (float.Parse(dt_order.Rows[i_order]["total_sum"].ToString()) - float.Parse(dt_order.Rows[i_order]["total_Gift_Certificates"].ToString())).ToString();

                string ls_html = "";
                #endregion

                string ls_sql_item = "select oid,invoice,name,phone,buy_name,ebay_userid,Global_Shipping_Reference_ID,addr1,addr2,city,state,zip,country,[payments_date],[order_id],[shipping_class], shipping_name,[qty],[sku],[product_name],[item],[item_price],Notes,Retailer_Order_Number,Return_Contact_Name,Return_Address_1,Return_Address_2,Return_City,Return_State_Or_Province,Return_Postal_Code,Return_Country_Code,Return_Phone from [t_biz_order] where status=1 and order_day=convert(varchar(8), getdate(), 112)     " +
                    "and order_id='" + ls_order_id_main + "' order by item;";
                adapter = new OleDbDataAdapter(ls_sql_item, myConn);
                DataTable dt_item = new DataTable();
                adapter.Fill(dt_item);

                for (int i = 0; i < dt_item.Rows.Count; i++)
                {
                    #region get item value

                    //string ls_oid = dt_invoice.Rows[i]["oid"].ToString();
                    string ls_invoice = dt_item.Rows[i]["invoice"].ToString();
                    string ls_name = dt_item.Rows[i]["name"].ToString();
                    string ls_phone = dt_item.Rows[i]["phone"].ToString();
                    string ls_buy_name = dt_item.Rows[i]["buy_name"].ToString();
                    string ls_ebay_userid = dt_item.Rows[i]["ebay_userid"].ToString();
                    string ls_Global_Shipping_Reference_ID = dt_item.Rows[i]["Global_Shipping_Reference_ID"].ToString();
                    string ls_addr1 = dt_item.Rows[i]["addr1"].ToString();
                    string ls_addr2 = dt_item.Rows[i]["addr2"].ToString();
                    string ls_city = dt_item.Rows[i]["city"].ToString();
                    string ls_state = dt_item.Rows[i]["state"].ToString();
                    string ls_zip = dt_item.Rows[i]["zip"].ToString();
                    string ls_country = dt_item.Rows[i]["country"].ToString();
                    string ls_payments_date = dt_item.Rows[i]["payments_date"].ToString();
                    string ls_payments_date_shot = "";
                    if (ls_payments_date.Length > 0)
                    {
                        ls_payments_date_shot = DateTime.Parse(ls_payments_date).ToString("MM/dd/yyyy");
                    }
                    else
                    {
                        ls_payments_date_shot = "";
                    }
                    string ls_order_id = dt_item.Rows[i]["order_id"].ToString();
                    //string ls_shipping_class = dt_item.Rows[i]["shipping_class"].ToString();
                    string ls_shipping_name = dt_item.Rows[i]["shipping_name"].ToString();
                    string ls_qty = dt_item.Rows[i]["qty"].ToString();
                    //string ls_sku = dt_item.Rows[i]["sku"].ToString();
                    string ls_product_name = dt_item.Rows[i]["product_name"].ToString();
                    string ls_item = dt_item.Rows[i]["item"].ToString();
                    string ls_Notes = dt_item.Rows[i]["Notes"].ToString();
                    string ls_item_price = dt_item.Rows[i]["item_price"].ToString().Replace("$", "");

                    string ls_Retailer_Order_Number = dt_item.Rows[i]["Retailer_Order_Number"].ToString();
                    string ls_Return_Contact_Name = dt_item.Rows[i]["Return_Contact_Name"].ToString();
                    string ls_Return_Address_1 = dt_item.Rows[i]["Return_Address_1"].ToString();
                    string ls_Return_Address_2 = dt_item.Rows[i]["Return_Address_2"].ToString();
                    string ls_Return_City = dt_item.Rows[i]["Return_City"].ToString();
                    string ls_Return_State_Or_Province = dt_item.Rows[i]["Return_State_Or_Province"].ToString();
                    string ls_Return_Postal_Code = dt_item.Rows[i]["Return_Postal_Code"].ToString();
                    string ls_Return_Country_Code = dt_item.Rows[i]["Return_Country_Code"].ToString();
                    string ls_Return_Phone = dt_item.Rows[i]["Return_Phone"].ToString();
                    //string ls_Return_Alternate_Phone = dt_item.Rows[i]["Return_Alternate_Phone"].ToString();

                    string ls_amount = "";
                    if (ls_item_price == "Replacement")
                    {
                        ls_amount = "Replacement";
                    }
                    else
                    {
                        try
                        {
                            ls_amount = (Double.Parse(ls_item_price) * int.Parse(ls_qty)).ToString();
                        }
                        catch
                        {
                            ls_amount = "";
                        }
                    }
                    #endregion
                    switch (ls_fromid)
                    {
                        case "1":
                            #region create invoice ama_home body
                            if (i == 0)
                            {
                                ls_html = "<body><div align='center' style='width:720px'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\la_Secret.jpg' width='248' /><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></div><br><div align='right'></div>";

                                if (ls_shipping_name != "Standard")
                                {
                                    ls_html += "<div align='left'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/></div>";
                                }
                                ls_html += "<br><table><td align='left' width='150'>Amazon Order ID:</td><td align='left'><strong>" + ls_order_id + "</strong></td><td align ='right'>Daily Order No.:</td><td align='right' width='70'><strong>" + ls_invoice + "</strong></td></table><table class='bdout'><td>Ship to:</td><td> </td><td> </td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_name + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_buy_name + "</td></tr>";

                                if (ls_addr2.Length > 0)
                                {
                                    ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_name + "</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>Beauty Secret LA</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr></table><br>";
                                }
                                else
                                {
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_name + "</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>Beauty Secret LA</td></tr></table><br>";
                                }

                                ls_html += "<table class='bdout'><tr align ='center'><td width='30'class='bdout'>QTY</td><td class='bdout' width='130'>SKU</td><td class='bdout'>Product Detail</td><td class='bdout' width='100'>price</td><td width='100' class='bdout'>Subtotal</td></tr>";
                            }

                            ls_html += "<tr class='itemr'><td width='30' align='center'>" + ls_qty + "</td><td class='bdside' width='130'>" + ls_item + "</td><td width='400'>" + ls_product_name + "</td><td class='bdside' width='100' align='right'>" + ls_item_price + "</td><td width='100'align='right'>" + ls_amount + "</td></tr>";

                            if (ls_nb == (i + 1).ToString())
                                for (int inb = (i + 1); inb < 6; inb++)
                                {
                                    ls_html += "<tr class='itemr'><td width='30' align='center'></td><td class='bdside' width='130'></td><td width='400'></td><td class='bdside' width='100' align='right'></td><td width='100'align='right'></td></tr>";
                                }


                            break;
                        #endregion
                        case "2":
                            #region create invoice ama_2 body
                            if (i == 0)
                            {
                                ls_html = "<body><div align='center' style='width:720px'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\amazonLogo.jpg' width='248' /><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></div><br><div align='right'></div>";

                                if (ls_shipping_name != "Standard")
                                {
                                    ls_html += "<div align='left'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/></div>";
                                }
                                ls_html += "<br><table><td align='left' width='150'>Amazon Order ID:</td><td align='left'><strong>" + ls_order_id + "</strong></td><td align ='right'>Daily Order No.:</td><td align='right' width='70'><strong>" + ls_invoice + "</strong></td></table><table class='bdout'><td>Ship to:</td><td> </td><td> </td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_name + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_buy_name + "</td></tr>";

                                if (ls_addr2.Length > 0)
                                {
                                    ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_name + "</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>Luxury Shop 4 Less</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr></table><br>";
                                }
                                else
                                {
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_name + "</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>Luxury Shop 4 Less</td></tr></table><br>";
                                }

                                ls_html += "<table class='bdout'><tr align ='center'><td width='30'class='bdout'>QTY</td><td class='bdout' width='130'>SKU</td><td class='bdout'>Product Detail</td><td class='bdout' width='100'>price</td><td width='100' class='bdout'>Subtotal</td></tr>";
                            }

                            ls_html += "<tr class='itemr'><td width='30' align='center'>" + ls_qty + "</td><td class='bdside' width='130'>" + ls_item + "</td><td width='400'>" + ls_product_name + "</td><td class='bdside' width='100' align='right'>" + ls_item_price + "</td><td width='100'align='right'>" + ls_amount + "</td></tr>";

                            if (ls_nb == (i + 1).ToString())
                                for (int inb = (i + 1); inb < 6; inb++)
                                {
                                    ls_html += "<tr class='itemr'><td width='30' align='center'></td><td class='bdside' width='130'></td><td width='400'></td><td class='bdside' width='100' align='right'></td><td width='100'align='right'></td></tr>";
                                }

                            break;
                        #endregion
                        case "3":
                            #region create invoice ebay_bbn body
                            if (i == 0)
                            {
                                ls_html = "<body><table><tr><td align='right' width='350'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\Ebaylogo.jpg' width='290' height='58'/></td><td align='right'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\logoEbay_x45.gif' height ='45'></td></tr><tr><td valign='bottom' align='left'>";

                                if (ls_shipping_name.IndexOf("Standard") != -1 && ls_shipping_name.IndexOf("First Class") != -1)
                                {
                                    ls_html += "<img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
                                }
                                ls_html += "</td><td align='right'><table style='width:320px;'><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table></td></tr></table>";
                                ls_html += "<br><table><td align='left' width='200'>Sales Record Number:</td><td align='left'><strong>" + ls_order_id + "</strong></td></table><table class='bdout'><tr><td>Ship to:</td><td> </td><td> </td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_name + "<br/>" + ls_Global_Shipping_Reference_ID + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr><tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_ebay_userid + "</td></tr>";
                                if (ls_addr2.Length > 0)
                                {
                                    ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_name + "</td></tr><tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>BestBeautyNet</td></tr><tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1' colspan='2'><strong>" + ls_Notes + "</strong></td></tr></table><br>";
                                }
                                else
                                {
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_name + "</td></tr><tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>BestBeautyNet</td></tr><tr><td width='350'> </td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1'><strong>" + ls_Notes + "</strong></td><td class='bdtop'></td></tr></table><br>";
                                }
                                ls_html += "<table class='bdout'><tr align ='center'><td width='30'class='bdout'>QTY</td><td class='bdout' width='130'>SKU</td><td class='bdout'>Product Detail</td><td class='bdout' width='100'>price</td><td width='100' class='bdout'>Subtotal</td></tr>";
                            }

                            ls_html += "<tr class='itemr'><td width='30' align='center'>" + ls_qty + "</td><td class='bdside' width='130'>" + ls_item + "</td><td width='400'>" + ls_product_name + "</td><td class='bdside' width='100' align='right'>" + ls_item_price + "</td><td width='100'align='right'>" + ls_amount + "</td></tr>";//subtotal
                            if (ls_nb == (i + 1).ToString())
                                for (int inb = (i + 1); inb < 6; inb++)
                                {
                                    ls_html += "<tr class='itemr'><td width='30' align='center'></td><td class='bdside' width='130'></td><td width='400'></td><td class='bdside' width='100' align='right'></td><td width='100'align='right'></td></tr>";
                                }

                            break;
                        #endregion
                        case "4":
                            #region create invoice ebay_lisa body
                            if (i == 0)
                            {
                                ls_html = "<body><table><tr><td align='right' width='350'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\logo.jpg' width='248' height='70'/></td><td align='right'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\logoEbay_x45.gif' height ='45'></td></tr><tr><td valign='bottom' align='left'>";

                                if (ls_shipping_name.IndexOf("Standard") != -1 && ls_shipping_name.IndexOf("First Class") != -1)
                                {
                                    ls_html += "<img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
                                }
                                ls_html += "</td><td align='right'><table style='width:320px;'><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table></td></tr></table>";
                                ls_html += "<br><table><td align='left' width='200'>Sales Record Number:</td><td align='left'><strong>" + ls_order_id + "</strong></td></table><table class='bdout'><tr><td>Ship to:</td><td> </td><td> </td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_name + "<br/>" + ls_Global_Shipping_Reference_ID + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr><tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_ebay_userid + "</td></tr>";
                                if (ls_addr2.Length > 0)
                                {
                                    ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_name + "</td></tr><tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>Lisaperfumes</td></tr><tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1' colspan='2'><strong>" + ls_Notes + "</strong></td></tr></table><br>";
                                }
                                else
                                {
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_name + "</td></tr><tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>Lisaperfumes</td></tr><tr><td width='350'> </td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1'><strong>" + ls_Notes + "</strong></td><td class='bdtop'></td></tr></table><br>";
                                }
                                ls_html += "<table class='bdout'><tr align ='center'><td width='30'class='bdout'>QTY</td><td class='bdout' width='130'>SKU</td><td class='bdout'>Product Detail</td><td class='bdout' width='100'>price</td><td width='100' class='bdout'>Subtotal</td></tr>";
                            }

                            ls_html += "<tr class='itemr'><td width='30' align='center'>" + ls_qty + "</td><td class='bdside' width='130'>" + ls_item + "</td><td width='400'>" + ls_product_name + "</td><td class='bdside' width='100' align='right'>" + ls_item_price + "</td><td width='100'align='right'>" + ls_amount + "</td></tr>";//subtotal
                            if (ls_nb == (i + 1).ToString())
                                for (int inb = (i + 1); inb < 6; inb++)
                                {
                                    ls_html += "<tr class='itemr'><td width='30' align='center'></td><td class='bdside' width='130'></td><td width='400'></td><td class='bdside' width='100' align='right'></td><td width='100'align='right'></td></tr>";
                                }

                            break;
                        #endregion
                        case "5":
                            #region create invoice ebay_wmb body
                            if (i == 0)
                            {
                                ls_html = "<body><table><tr><td align='right' width='350'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/></td><td align='right'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\logoEbay_x45.gif' height ='45'></td></tr><tr><td valign='bottom' align='left'>";
                                if (ls_shipping_name.IndexOf("Standard") != -1 && ls_shipping_name.IndexOf("First Class") != -1)
                                {
                                    ls_html += "<img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
                                }
                                ls_html += "</td><td align='right'><table style='width:320px;'><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table></td></tr></table>";
                                ls_html += "<br><table><td align='left' width='200'>Sales Record Number:</td><td align='left'><strong>" + ls_order_id + "</strong></td></table><table class='bdout'><tr><td>Ship to:</td><td> </td><td> </td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_name + "<br/>" + ls_Global_Shipping_Reference_ID + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr><tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_ebay_userid + "</td></tr>";
                                if (ls_addr2.Length > 0)
                                {
                                    ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_name + "</td></tr><tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>WeMakeBeauty</td></tr><tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1' colspan='2'><strong>" + ls_Notes + "</strong></td></tr></table><br>";
                                }
                                else
                                {
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_name + "</td></tr><tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>WeMakeBeauty</td></tr><tr><td width='350'> </td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1'><strong>" + ls_Notes + "</strong></td><td class='bdtop'></td></tr></table><br>";
                                }
                                ls_html += "<table class='bdout'><tr align ='center'><td width='30'class='bdout'>QTY</td><td class='bdout' width='130'>SKU</td><td class='bdout'>Product Detail</td><td class='bdout' width='100'>price</td><td width='100' class='bdout'>Subtotal</td></tr>";
                            }

                            ls_html += "<tr class='itemr'><td width='30' align='center'>" + ls_qty + "</td><td class='bdside' width='130'>" + ls_item + "</td><td width='400'>" + ls_product_name + "</td><td class='bdside' width='100' align='right'>" + ls_item_price + "</td><td width='100'align='right'>" + ls_amount + "</td></tr>";//subtotal
                            if (ls_nb == (i + 1).ToString())
                                for (int inb = (i + 1); inb < 6; inb++)
                                {
                                    ls_html += "<tr class='itemr'><td width='30' align='center'></td><td class='bdside' width='130'></td><td width='400'></td><td class='bdside' width='100' align='right'></td><td width='100'align='right'></td></tr>";
                                }

                            break;
                        #endregion
                        case "6":
                            #region create invoice wmb.com body
                            if (i == 0)
                            {
                                ls_html = "<div align='center'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/></div><br><div align='right'>";

                                if (ls_shipping_name.IndexOf("Standard Ground (U.S. Domestic)") != -1 && ls_shipping_name.IndexOf("Free Standard Shipping (U.S. Domestic)") != -1)
                                {
                                    ls_html += "<img style='float:left' src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
                                }
                                ls_html += "<table style='width:320px;'><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table><p style='clear:both;'></p></div>";
                                ls_html += "<br><table><td align='left' width='150'>Order ID:</td><td align='left'><strong>" + ls_order_id + "</strong></td><td align ='right'>Daily Order No.:</td><td align='right' width='70'><strong>" + ls_invoice + "</strong></td></table><table class='bdout'><tr><td>Ship to:</td><td> </td><td> </td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_name + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_buy_name + "</td></tr>";
                                if (ls_addr2.Length > 0)
                                {
                                    ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_name + "</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>WeMakeBeauty</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr></table><br>";
                                }
                                else
                                {
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_name + "</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>WeMakeBeauty</td></tr></table><br>";
                                }

                                ls_html += "<table class='bdout'><tr align ='center'><td width='30'class='bdout'>QTY</td><td class='bdout' width='130'>SKU</td><td class='bdout'>Product Detail</td><td class='bdout' width='100'>price</td><td width='100' class='bdout'>Subtotal</td></tr>";
                            }

                            //do not show discount item.
                            if (ls_item_price.Substring(0, 1) != "-")
                            {
                                ls_html += "<tr class='itemr'><td width='30' align='center'>" + ls_qty + "</td><td class='bdside' width='130'>" + ls_item + "</td><td width='400'>" + ls_product_name + "</td><td class='bdside' width='100' align='right'>" + ls_item_price + "</td><td width='100'align='right'>" + ls_amount + "</td></tr>";
                            }

                            if (ls_nb == (i + 1).ToString())
                            {
                                for (int inb = (i + 1); inb < 6; inb++)
                                {
                                    ls_html += "<tr class='itemr'><td width='30' align='center'></td><td class='bdside' width='130'></td><td width='400'></td><td class='bdside' width='100' align='right'></td><td width='100'align='right'></td></tr>";
                                }

                            }

                            break;
                        #endregion
                        case "7":
                            #region create invoice over stock body

                            //"<tr><td colspan='3' ><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\logo.png'  width='150'/><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr>" +
                            if (i == 0)
                            {
                                ls_html = "<table cellpadding='0' cellspacing='0' width='903px'>" +
                                "<tr><td colspan='3' ><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\logo.png'  width='150'/><div align='right'>              </div></td></tr>" +
                                "<tr align='center'><td colspan='3' ><span class='ft1'>Customer Delivery Receipt</span></td></tr>" +
                                "<tr height='42'><td colspan='3' ></td></tr>" +
                                "<tr><td style='vertical-align:text-top;' width='349px'><span class='ft4'>Return Address:</span><br/><span class='ft4'>" +
                                ls_Return_Contact_Name + "</span><br/><span class='ft4'>" +
                                ls_Return_Address_1 + "</span><br/><span class='ft4'>" +
                                ls_Return_Address_2 + "</span><br/><span class='ft4'>" +
                                ls_Return_City + "," + ls_Return_State_Or_Province + "-" + ls_Return_Postal_Code + " " + ls_Return_Country_Code + "</span><br/><span class='ft4'>" +
                                ls_Return_Phone + "<br/>" +
                                "</span></td>" +
                                "<td style='vertical-align:text-top;' width='349px'><span class='ft4'>Shipped To:</span><br/><span class='ft4'>" +
                                ls_name + "</span><br/><span class='ft4'>" +
                                ls_addr1 + "</span><br/>";

                                if (ls_addr2.Length > 0)
                                {
                                    ls_html += "<span class='ft4'>" + ls_addr2 + "</span><br/>";
                                }

                                ls_html += "<span class='ft4'>" + ls_city + "," + ls_state + "-" + ls_zip + " " + ls_country + "</span><br/><span class='ft4'>" +
                                ls_phone + "<br/>" +
                                "</span></td>" +
                                "<td  align='right'>" +
                                "<table width='205px' class ='tb_item'><tr><td><span class='ft3'>Order Date:<br/>" + ls_payments_date_shot + "</span></td></tr>" +
                                "<tr><td><span class='ft3'>Order #:<br/>" + ls_Retailer_Order_Number + "</span></td></tr>" +
                                "<tr><td><span class='ft3'>Ship Method:<br/>" + ls_shipping_name + "</span></td></tr>" +
                                "</table>" +
                                "</td></tr>" +
                                "<tr height='22'><td colspan='3' ></td></tr>";

                                ls_html += @"
<tr align='center'><td colspan='3'>
<table width='903px' class ='tb_item'><tr align='left' height='22' bgcolor='#e6e6e6'>
<td width='104'><span class='ft2'>Qty Ordered</span></td>
<td width='104'><span class='ft2'>Qty Shipped</span></td>
<td width='131'><span class='ft2'>Item Number</span></td>
<td width='131'><span class='ft2'>Vendor SKU</span></td>
<td width='363'><span class='ft2'>Product Description</span></td>
<td width='70'><span class='ft2'>Price</span></td>
</tr>";
                            }

                            ls_html += "<tr height='35' align='left'><td ><p>" + ls_qty + "</p></td>";
                            ls_html += "<td ><p>" + ls_qty + "</p></td>";
                            ls_html += "<td ><p>" + ls_item + "</p></td>";
                            ls_html += "<td ><p>" + ls_item + "</p></td>";
                            ls_html += "<td ><p>" + ls_product_name + "</p></td>";
                            ls_html += "<td ><p>" + ls_amount + "</p></td>";
                            ls_html += "</tr>";
                            break;
                        #endregion
                        case "8":
                            #region create invoice buy body
                            if (i == 0)
                            {
                                ls_html = "<table><tr><td width='180'></td><td align = 'center'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/></td><td width='180' align = 'right' valign = 'top'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\Logo_Rekuton.jpg' width='180' height='40'/></td></tr></table><br><div align='right'><blockquote valign='bottom'>";

                                if (ls_shipping_name != "Standard")
                                {
                                    ls_html += "<img style='float:left' src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
                                }
                                ls_html += "<table style='width:680px;'><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table><p style='clear:both;'></p></blockquote></div>";
                                ls_html += "<br><table><td align='left' width='80'>Order ID:</td><td align='left'><strong>" + ls_order_id + "</strong></td><td align ='right'>Daily Order No.:</td><td align='left' width='70'><strong>" + ls_invoice + "</strong></td></table><table class='bdout'><td>Ship to:</td><td> </td><td> </td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_name + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_buy_name + "</td></tr>";
                                if (ls_addr2.Length > 0)
                                {
                                    ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_name + "</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>WeMakeBeauty</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr></table><br>";
                                }
                                else
                                {
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_name + "</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>WeMakeBeauty</td></tr></table><br>";
                                }
                                ls_html += "<table class='bdout'><tr align ='center'><td width='30'class='bdout'>QTY</td><td class='bdout' width='130'>SKU</td><td class='bdout'>Product Detail</td><td class='bdout' width='100'>price</td><td width='100' class='bdout'>Subtotal</td></tr>";
                            }

                            ls_html += "<tr class='itemr'><td width='30' align='center'>" + ls_qty + "</td><td class='bdside' width='130'>" + ls_item + "</td><td width='400'>" + ls_product_name + "</td><td class='bdside' width='100' align='right'>" + ls_item_price + "</td><td width='100'align='right'>" + ls_amount + "</td></tr>";

                            if (ls_nb == (i + 1).ToString())
                            {
                                for (int inb = (i + 1); inb < 6; inb++)
                                {
                                    ls_html += "<tr class='itemr'><td width='30' align='center'></td><td class='bdside' width='130'></td><td width='400'></td><td class='bdside' width='100' align='right'></td><td width='100'align='right'></td></tr>";
                                }

                            }

                            break;
                        #endregion
                        case "9":
                            #region create invoice NewEgg body
                            if (i == 0)
                            {
                                ls_html = "<div align='center'><img src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/><img style='float:right' src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\neLogo.png' height='70px'/></div>";
                                if (ls_shipping_name.IndexOf("Standard") == -1)
                                {
                                    ls_html += "<div><img style='float:left' src='file://Z:\\Invoice Printing\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/></div>";
                                }
                                ls_html += "<table><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table>";
                                ls_html += "<br><table><td align='left' width='150'>NewEgg Order ID:</td><td align='left'><strong>" + ls_order_id + "</strong></td><td align ='right'>Daily Order No.:</td><td align='right' width='70'><strong>" + ls_invoice + "</strong></td></table><table class='bdout'><td>Ship to:</td><td> </td><td> </td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_name + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr>";
                                ls_html += "<tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_name + "</td></tr>";
                                if (ls_addr2.Length > 0)
                                {
                                    ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_name + "</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>WeMakeBeauty</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr></table><br>";
                                }
                                else
                                {
                                    ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_name + "</td></tr>";
                                    ls_html += "<tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>WeMakeBeauty</td></tr></table><br>";
                                }
                                ls_html += "<table class='bdout'><tr align ='center'><td width='30'class='bdout'>QTY</td><td class='bdout' width='130'>SKU</td><td class='bdout'>Product Detail</td><td class='bdout' width='100'>price</td><td width='100' class='bdout'>Subtotal</td></tr>";
                            }

                            ls_html += "<tr class='itemr'><td width='30' align='center'>" + ls_qty + "</td><td class='bdside' width='130'>" + ls_item + "</td><td width='400'>" + ls_product_name + "</td><td class='bdside' width='100' align='right'>" + ls_item_price + "</td><td width='100'align='right'>" + ls_amount + "</td></tr>";

                            if (ls_nb == (i + 1).ToString())
                            {
                                for (int inb = (i + 1); inb < 6; inb++)
                                {
                                    ls_html += "<tr class='itemr'><td width='30' align='center'></td><td class='bdside' width='130'></td><td width='400'></td><td class='bdside' width='100' align='right'></td><td width='100'align='right'></td></tr>";
                                }

                            }

                            break;
                            #endregion
                    }
                }
                dt_item = null;


                #region bottom html
                switch (ls_fromid)
                {
                    case "1":
                        ls_html += "<tr><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_main_total_shipping + "</td></tr>";
                        ls_html += "<tr><td class='bdleft'> </td><td> </td><td> </td><td class='bdside'>Tax:</td><td class='bdside' align='right'>" + ls_main_total_tax + "</td></tr>";
                        ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_main_total_sum + "</td></tr></table>";
                        ls_html += "<div width = '700'>Thank you for buying at Beauty Secret LA @ Amazon Marketplace. To provide feeback for the seller please visit: www.amazon.com/feedback. To contact the seller, please visit Amazon.com and click on 'Your Account' at the top of any page. In Your Account, go to the 'Orders' section and click on the link 'Leave seller feeback'. Select the order or click on the 'view Order' button. Click on the 'seller profile' under the appropriate product. On the lower right side of the page under 'seller Help', click on 'contact this seller'.</div><div style='page-break-after: always' align = 'center'>&nbsp;</div></body></html>";
                        break;
                    case "2":
                        ls_html += "<tr><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_main_total_shipping + "</td></tr>";
                        ls_html += "<tr><td class='bdleft'> </td><td> </td><td> </td><td class='bdside'>Tax:</td><td class='bdside' align='right'>" + ls_main_total_tax + "</td></tr>";
                        ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_main_total_sum + "</td></tr></table>";
                        ls_html += "<div width = '700'>Thank you for buying at Luxury Shop 4 Less @ Amazon Marketplace. To provide feeback for the seller please visit: www.amazon.com/feedback. To contact the seller, please visit Amazon.com and click on 'Your Account' at the top of any page. In Your Account, go to the 'Orders' section and click on the link 'Leave seller feeback'. Select the order or click on the 'view Order' button. Click on the 'seller profile' under the appropriate product. On the lower right side of the page under 'seller Help', click on 'contact this seller'.</div><div style='page-break-after: always' align = 'center'>&nbsp;</div></body></html>";
                        break;
                    case "3":
                        ls_html += "<tr><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_main_total_shipping + "</td></tr>";
                        ls_html += "<tr><td></td><td></td><td></td><td class='bdout'>US Tax:</td><td class = 'bdout' align='right'>" + ls_main_total_tax + "</td></tr>";
                        ls_html += "<tr><td class='bdbottom'>&nbsp </td><td class='bdbottom'>&nbsp</td><td class='bdbottom'>&nbsp</td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_main_total_sum + "</td></tr></table><br>";
                        //ls_html += "<div>Thank you for buying at BestBeautyNet. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break: always'>&nbsp</div>";
                        ls_html += "<div>Thank you for buying at BestBeautyNet. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break-after: always'>&nbsp</div>";
                        break;
                    case "4":
                        ls_html += "<tr><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_main_total_shipping + "</td></tr>";
                        ls_html += "<tr><td></td><td></td><td></td><td class='bdout'>US Tax:</td><td class = 'bdout' align='right'>" + ls_main_total_tax + "</td></tr>";
                        ls_html += "<tr><td class='bdbottom'>&nbsp </td><td class='bdbottom'>&nbsp</td><td class='bdbottom'>&nbsp</td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_main_total_sum + "</td></tr></table><br>";
                        //ls_html += "<div>Thank you for buying at lisaperfumes. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break: always'>&nbsp</div>";
                        ls_html += "<div>Thank you for buying at lisaperfumes. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break-after: always'>&nbsp</div>";
                        break;
                    case "5":
                        ls_html += "<tr><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_main_total_shipping + "</td></tr>";
                        ls_html += "<tr><td></td><td></td><td></td><td class='bdout'>US Tax:</td><td class = 'bdout' align='right'>" + ls_main_total_tax + "</td></tr>";
                        ls_html += "<tr><td class='bdbottom'>&nbsp </td><td class='bdbottom'>&nbsp</td><td class='bdbottom'>&nbsp</td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_main_total_sum + "</td></tr></table><br>";
                        //ls_html += "<div>Thank you for buying at WeMakeBeauty. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break: always'>&nbsp</div>";
                        ls_html += "<div>Thank you for buying at WeMakeBeauty. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break-after: always'>&nbsp</div>";
                        break;
                    case "6":
                        ls_html += "<tr><td class='bdtop' colspan='3'>" + ls_main_notes + "</td><td class='bdout'>Discount:</td><td class='bdout' align='right'>" + ls_main_total_sum + "</td></tr>";
                        ls_html += "<tr><td class='bdleft' colspan='3'> </td><td class='bdout'>Tax:</td><td class='bdout' align='right'>" + ls_main_total_tax + "</td></tr>";
                        ls_html += "<tr><td class='bdleft' colspan='3'> </td><td class='bdout'>Gift Wrap:</td><td class='bdout' align='right'>" + ls_main_total_giftwrap + "</td></tr>";
                        ls_html += "<tr><td class='bdleft' colspan='3'> </td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_main_total_shipping + "</td></tr>";
                        if (ls_main_total_Gift_Certificates != "0")
                        {
                            ls_html += "<tr><td class='bdleft' colspan='3'> </td><td class='bdout'>Gift Certificates:</td><td class='bdout' align='right'>" + ls_main_total_Gift_Certificates + "</td></tr>";
                        }
                        ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_main_total_sum + "</td></tr></table>";
                        //ls_html += "<div width = '700'>Thank you very much for shopping at WeMakeBeauty.com.  For any inquiry or question, please contact our customer service at Service@wemakebeauty.com.</div><div style='page-break-after: always' align = 'center'>&nbsp;</div></body></html>";
                        ls_html += "<div width = '700'>Thank you very much for shopping at WeMakeBeauty.com.  For any inquiry or question, please contact our customer service at Service@wemakebeauty.com.</div><div style='page-break-after: always' align = 'center'>&nbsp;</div>";
                        break;
                    case "7":
                        ls_html += @"</table></td></tr><tr height='22'><td colspan='3' ></td></tr>
<table width='903px' class ='tb_item'><tr><td><span class='ft3'><b>Return Instructions:</b></span><br><span class='ft2'>
You may return most new and unopened items within 30 days of delivery for a full refund. To initiate a return, visit overstock.com/myaccount or call 1-
800-843-2446. For an international return, email international@overstock.com or call 00-1-919-576-9926 for instructions.</span>
</td></tr></table><div style='page-break-after: always' align = 'center'>&nbsp;</div>";
                        break;
                    case "8":
                        ls_html += "<tr><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_main_total_shipping + "</td></tr>";
                        //ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_order.ToString() + "</td></tr>";
                        ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_main_total_sum + "</td></tr></table>";
                        //ls_html += "<div width = '700'>Thank you very much for shopping at WeMakeBeauty @ Rakuten.com!</div><div style='page-break: always' align = 'center'>&nbsp;</div>";
                        ls_html += "<div width = '700'>Thank you very much for shopping at WeMakeBeauty @ Rakuten.com!</div><div style='page-break-after: always' align = 'center'>&nbsp;</div>";
                        break;
                    case "9":
                        ls_html += "<tr><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_main_total_shipping + "</td></tr>";
                        ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_main_total_sum + "</td></tr></table>";
                        ls_html += "<div width = '700'>Thank you for buying at WeMakeBeauty @ NewEgg Marketplace. Please contact us at Newegg@wemakebeauty.com if you have any question or concerns regarding your order.</div><div style='page-break-after: always' align = 'center'>&nbsp;</div></body></html>";
                        break;
                }

                string ls_file = ls_dir + "\\" + ls_site_code + ".html";
                using (FileStream fs = File.Open(ls_file, FileMode.Append))
                {
                    byte[] b = System.Text.Encoding.Default.GetBytes(ls_html.Replace("<br/><br/>", "<br/>"));
                    fs.Write(b, 0, b.Length);
                    fs.Close();
                    ls_html = "";
                }
                #endregion

            }
            dt_order = null;
            myConn.Close();
            insert_access_overstock();

        }

        private void txButton3_Click(object sender, EventArgs e)
        {

            string ls_ovf = "z:\\Invoice Printing\\" + DateTime.Now.ToString("yyyy-MM") + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\7_Over_Stock.html";
            //textBox1.Text = ls_ovf;
            //ProcessStartInfo startInfo = new ProcessStartInfo("IExplore.exe");
            //startInfo.Verb = "RunAs";
            //startInfo.Arguments = @ls_ovf;
            //Process.Start(startInfo);

            Process.Start("iexplore", "file://" + ls_ovf);
        }

        private void txButton2_Click(object sender, EventArgs e)
        {
            /*
             create view v_tracking as
select x.[from],x.[order#],x.[today's invoice_order-id],
(select a.[tracking_id] from [t_imp_tracking] a where a.[order id]=x.[today's invoice_order-id] ) as Tracking,
(select a.[type] from [t_imp_tracking] a where a.[order id]=x.[today's invoice_order-id] ) as type,
(select a.[service_class] from [t_imp_tracking] a where a.[order id]=x.[today's invoice_order-id] ) as service_class
from t_address_list x
             */


            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();

            string ls_gc = "select [access_dir],[access_table],[sql_table],[insert_f],[select_f] from [t_base_access] where status=2;";
            OleDbDataAdapter adapter = new OleDbDataAdapter(ls_gc, myConn);
            DataTable dt_gc = new DataTable();
            adapter.Fill(dt_gc);
            for (int i = 0; i < dt_gc.Rows.Count; i++)
            {
                string ls_access_dir = dt_gc.Rows[i]["access_dir"].ToString();
                string ls_access_table = dt_gc.Rows[i]["access_table"].ToString();
                string ls_select_f = dt_gc.Rows[i]["select_f"].ToString();
                string ls_sql_table = dt_gc.Rows[i]["sql_table"].ToString();


                OleDbCommand cmd2 = new OleDbCommand("truncate table " + ls_sql_table + ";", myConn);
                cmd2.ExecuteNonQuery();


                //change cpu x86

                OleDbConnection objConn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ls_access_dir + ";");
                OleDbCommand MDBCommand = new OleDbCommand("Select " + ls_select_f + " FROM" + ls_access_table, objConn);
                OleDbDataReader rdr2;
                objConn.Open();
                rdr2 = MDBCommand.ExecuteReader();
                SqlBulkCopy sbc = new SqlBulkCopy(ConfigurationManager.AppSettings["dbConnectionString"]);
                sbc.DestinationTableName = ls_sql_table;
                try
                {
                    sbc.WriteToServer(rdr2);
                }
                catch
                {
                }
                sbc.Close();
                rdr2.Close();
                objConn.Close();

            }
            dt_gc = null;






            string ls_up = @"
insert into [t_biz_tracking] ([shipping_day],[from],[order_id],[lable_id],[Tracking],[type],[service_class],status)
select convert(varchar(8), getdate(), 112),x.[from],x.[order#],x.[today's invoice_order-id],
(select a.[tracking_id] from [t_imp_tracking] a where a.[order id]=x.[today's invoice_order-id] ) as Tracking,
(select a.[type] from [t_imp_tracking] a where a.[order id]=x.[today's invoice_order-id] ) as type,
(select a.[service_class] from [t_imp_tracking] a where a.[order id]=x.[today's invoice_order-id] ) as service_class ,
1
from t_address_list x
";



            OleDbCommand cmd = new OleDbCommand(ls_up, myConn);
            cmd.ExecuteNonQuery();


            ls_up = @"
update t_biz_order set TRACKING_NUMBER='' where TRACKING_NUMBER is null;

";
            cmd = new OleDbCommand(ls_up, myConn);
            cmd.ExecuteNonQuery();

            /*
update t_biz_order set status = 2 where order_id in (select order_id from [t_biz_tracking])
update t_biz_order set status = 2 where Retailer_Order_Number in (select order_id from [t_biz_tracking])
update t_biz_order set status = 2 where order_id in (select 
case when substring(order_id,1,1)='9' then  Substring(order_id,2,len(order_id)-1)ELSE order_id END 
from [t_biz_tracking] where [from]='website')
*/

            //循环取Tracking Set Status=2, Tracking 可以累加|号隔开,  for Bar Code Scanner

            string ls_get_tracking = "select id,[shipping_day],[from],[order_id],[lable_id],[Tracking],[type],[service_class],status from t_biz_tracking where status=1  ";
            adapter = new OleDbDataAdapter(ls_get_tracking, myConn);
            DataTable dt_track = new DataTable();
            adapter.Fill(dt_track);
            for (int i = 0; i < dt_track.Rows.Count; i++)
            {
                string ls_id = dt_track.Rows[i]["id"].ToString();
                string ls_lable_id = dt_track.Rows[i]["lable_id"].ToString();
                string ls_from = dt_track.Rows[i]["from"].ToString();
                string ls_Tracking = dt_track.Rows[i]["Tracking"].ToString();

                if (ls_Tracking.Length > 1)
                {
                    if (ls_from == "OverStock" || ls_from == "7") 
                    {
                        ls_up = "update t_biz_order set status=3 ,tracking_number=tracking_number+'|'+'" + ls_Tracking + "' where status in ('1','3') and Retailer_Order_Number='" + ls_lable_id + "';";
                        cmd = new OleDbCommand(ls_up, myConn);
                        cmd.ExecuteNonQuery();
                    }
                    else if (ls_from == "Website" || ls_from == "6") 
                    {
                        //95690
                        if (ls_lable_id.Substring(0,3).ToLower() == "wmb")
                        {
                            ls_lable_id = ls_lable_id.Substring(3, ls_lable_id.Length - 3);
                        }

                        ls_up = "update t_biz_order set status=3 ,tracking_number=tracking_number+'|'+'" + ls_Tracking + "' where status in ('1','3') and order_id='" + ls_lable_id + "';";
                        cmd = new OleDbCommand(ls_up, myConn);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        ls_up = "update t_biz_order set status=3 ,tracking_number=tracking_number+'|'+'" + ls_Tracking + "' where status in ('1','3') and order_id='" + ls_lable_id + "';";
                        cmd = new OleDbCommand(ls_up, myConn);
                        cmd.ExecuteNonQuery();

                    }

                    ls_up = "update t_biz_tracking set status=3 where id='" + ls_id + "';";
                    cmd = new OleDbCommand(ls_up, myConn);
                    cmd.ExecuteNonQuery();

                }

            }

            ls_up = "update t_biz_tracking set status=2 where status='3' ;";
            cmd = new OleDbCommand(ls_up, myConn);
            cmd.ExecuteNonQuery();

            myConn.Close();
            this.Info("Tracking generated");

            /*
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();
            OleDbCommand cmd = new OleDbCommand("truncate table t_imp_tracker_number;", myConn);
            cmd.ExecuteNonQuery();
            



            string ls_field_Separator = "	";
            string ls_txt_left = "";
            string ls_txt_add = "";
            string ls_tr_dir = "Y:\\Endicia Export\\" + DateTime.Now.ToString("yyMMdd") + ".TXT";//yyMMdd.txt

            System.IO.FileInfo file_old = new System.IO.FileInfo(ls_tr_dir);

            if (file_old.Exists)
            {

                StreamReader fs_is_ect = new StreamReader(ls_tr_dir, System.Text.Encoding.UTF8);
                string ls_txt_is_ect = fs_is_ect.ReadToEnd();
                fs_is_ect.Close();
                string[] dog_small = ls_txt_is_ect.Replace("\r\n", "|").Split('|');
                int cc = 0;
                foreach (string bb in dog_small)
                {
                    string ls_txt_bb = bb.Trim();
                    if (ls_txt_bb.Length > 10)
                    {

                        if (cc == 0)
                        {
                            ls_txt_left = "insert into t_imp_tracker_number ([" + ls_txt_bb.Replace("|", "").Replace(ls_field_Separator, "|") + "])";
                        }
                        else
                        {
                            string[] dog_this_csv = ls_txt_bb.Replace(ls_field_Separator, "|").Split('|');
                            string[] dog_this_csv_left = ls_txt_left.Split('|');
                            int ls_txt_dog = dog_this_csv.Length;
                            string ls_txt_left_1 = "";
                            for (int y = 0; y < ls_txt_dog; y++)
                            {
                                string ls_txt_cat = dog_this_csv_left[y] + "],[";
                                ls_txt_left_1 = ls_txt_left_1 + ls_txt_cat;
                            }
                            ls_txt_left_1 = ls_txt_left_1 + "])";
                            ls_txt_left_1 = ls_txt_left_1.Replace(",[])", ")").Replace("])])", "])");
                            string ls_txt_this_csv = " values ('" + ls_txt_bb.Replace("'", "''").Replace(ls_field_Separator, "','") + "');";
                            ls_txt_add += ls_txt_left_1 + ls_txt_this_csv;
                        }
                        cc++;
                    }
                }

                cmd = new OleDbCommand(ls_txt_add, myConn);
                cmd.ExecuteNonQuery();
                


            }

            ls_txt_add = @"
insert into [t_biz_tracking_number] ([ship_DAY],[od],tracking_id)
select distinct convert(varchar(8), getdate(), 112) as [ship_DAY],od,tracking_id from
(
select type, [address] as od, [tracking_id] from t_imp_tracker_number where len([address])>1
union
select type,order_id as od,[tracking_id]
from t_imp_tracker_number where len(order_id)>1
union
select type,log_id as od, [Balance ($) ] as [tracking_id]
from t_imp_tracker_number where tracking_id is null
union
select cast(type as nvarchar(50)),[order id],cast(tracking_id as nvarchar(50)) as [tracking_id] from t_imp_tracking
) as x
";
            cmd = new OleDbCommand(ls_txt_add, myConn);
            cmd.ExecuteNonQuery();
            

            ls_txt_add = "update t_biz_order set status = 2 where order_id in (select od from[t_biz_tracking_number])";

            cmd = new OleDbCommand(ls_txt_add, myConn);
            cmd.ExecuteNonQuery();
            myConn.Close();
            

            */

        }



        private Bitmap GetCode39(string strSource, string add_txt)
        {
            int x = 5; //左邊界  
            int y = 0; //上邊界  
            int WidLength = 2; //粗BarCode長度  
            int NarrowLength = 1; //細BarCode長度  
            int BarCodeHeight = 24; //BarCode高度  
            int intSourceLength = strSource.Length;
            string strEncode = "010010100"; //編碼字串 初值為 起始符號 *  

            string AlphaBet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*"; //Code39的字母  

            string[] Code39 = //Code39的各字母對應碼  
       {  
     /**//* 0 */ "000110100",    
     /**//* 1 */ "100100001",    
     /**//* 2 */ "001100001",    
     /**//* 3 */ "101100000",  
     /**//* 4 */ "000110001",    
     /**//* 5 */ "100110000",    
     /**//* 6 */ "001110000",    
     /**//* 7 */ "000100101",  
     /**//* 8 */ "100100100",    
     /**//* 9 */ "001100100",    
     /**//* A */ "100001001",    
     /**//* B */ "001001001",  
     /**//* C */ "101001000",    
     /**//* D */ "000011001",    
     /**//* E */ "100011000",    
     /**//* F */ "001011000",  
     /**//* G */ "000001101",    
     /**//* H */ "100001100",    
     /**//* I */ "001001100",    
     /**//* J */ "000011100",  
     /**//* K */ "100000011",    
     /**//* L */ "001000011",    
     /**//* M */ "101000010",    
     /**//* N */ "000010011",  
     /**//* O */ "100010010",    
     /**//* P */ "001010010",    
     /**//* Q */ "000000111",    
     /**//* R */ "100000110",  
     /**//* S */ "001000110",    
     /**//* T */ "000010110",    
     /**//* U */ "110000001",    
     /**//* V */ "011000001",  
     /**//* W */ "111000000",    
     /**//* X */ "010010001",    
     /**//* Y */ "110010000",    
     /**//* Z */ "011010000",  
     /**//* - */ "010000101",    
     /**//* . */ "110000100",    
     /**//*' '*/ "011000100",  
     /**//* $ */ "010101000",  
     /**//* / */ "010100010",    
     /**//* + */ "010001010",    
     /**//* % */ "000101010",    
     /**//* * */ "010010100"
};
            strSource = strSource.ToUpper();
            //實作圖片  

            int txt_height = 15;
            if (add_txt == "Y")
            {
                txt_height = 20;
            }
            else
            {
                txt_height = 0;
            }

            Bitmap objBitmap = new Bitmap(
              ((WidLength * 3 + NarrowLength * 7) * (intSourceLength + 2)) + (x * 2),
              BarCodeHeight + (y * 2) + txt_height);

            Graphics objGraphics = Graphics.FromImage(objBitmap); //宣告GDI+繪圖介面  
                                                                  //填上底色  
            objGraphics.FillRectangle(Brushes.White, 0, 0, objBitmap.Width, objBitmap.Height);

            for (int i = 0; i < intSourceLength; i++)
            {
                //檢查是否有非法字元  
                if (AlphaBet.IndexOf(strSource[i]) == -1 || strSource[i] == '*')
                {
                    objGraphics.DrawString("Error",
                      SystemFonts.DefaultFont, Brushes.Red, x, y);
                    return objBitmap;
                }
                //查表編碼  
                strEncode = string.Format("{0}0{1}", strEncode,
                 Code39[AlphaBet.IndexOf(strSource[i])]);
            }

            strEncode = string.Format("{0}0010010100", strEncode); //補上結束符號 *  

            int intEncodeLength = strEncode.Length; //編碼後長度  
            int intBarWidth;

            for (int i = 0; i < intEncodeLength; i++) //依碼畫出Code39 BarCode  
            {
                intBarWidth = strEncode[i] == '1' ? WidLength : NarrowLength;
                objGraphics.FillRectangle(i % 2 == 0 ? Brushes.Black : Brushes.White,
                 x, y, intBarWidth, BarCodeHeight);
                x += intBarWidth;
            }

            if (add_txt == "Y")
            {

                string ls_txt = "Lable NO:";

                Font font = new Font("Tahoma", 10, FontStyle.Regular);
                Font font2 = new Font("Tahoma", 12, FontStyle.Bold);
                Brush drawBrush = new SolidBrush(Color.Black);
                //for (int i = 0; i < ls_txt.Length; i++)
                //{
                objGraphics.DrawString(ls_txt, font, drawBrush, 30, 28);
                objGraphics.DrawString(int.Parse(strSource.Substring(6, 4)).ToString(), font2, drawBrush, 114, 26);
                //objGraphics.DrawString(ls_txt.Substring(i, 1), font, drawBrush, 11 * (i + 1)-5, 28);//23
                //}
            }


            return objBitmap;
        }

        private void Invoice_new_Load(object sender, EventArgs e)
        {
            show_totoal();
        }

        private void show_totoal()
        {
            #region show site result
            //dataGridView1.AutoGenerateColumns = false;
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();
            string sqlstr = @"
select x.FROMID,(select site_name from [t_base_from] where fromid=x.fromid) as site_name,x.all_order_num,x.all_item_num 
,(select count(distinct a.order_id) as order_num from [t_biz_order] a where status=1 and Order_DAY=convert(varchar(8), getdate(), 112) and a.FROMID=x.FROMID) as today_order
,(select count(a.order_id) as order_num from [t_biz_order] a where status=1 and Order_DAY=convert(varchar(8), getdate(), 112) and a.FROMID=x.FROMID) as today_order_item
,(select count(distinct a.order_id) as order_num from [t_biz_order] a where status=1 and Order_DAY!=convert(varchar(8), getdate(), 112) and a.FROMID=x.FROMID) as before_order
,(select count(a.order_id) as order_num from [t_biz_order] a where status=1 and Order_DAY!=convert(varchar(8), getdate(), 112) and a.FROMID=x.FROMID) as before_order_item
from (
select FROMID,count(distinct order_id) as all_order_num ,count(order_id) as all_item_num  from [t_biz_order] where status=1
group by fromid
) x
";

            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            //dataGridView1.Columns[0].DataPropertyName = "Item";
            //dataGridView1.Columns[1].DataPropertyName = "Item_Name";
            //dataGridView1.Columns[2].DataPropertyName = "Barcode";
            //dataGridView1.Columns[3].DataPropertyName = "Weight";

            sqlstr=@"
select
'Total: ' +
cast(sum(all_order_num) as nvarchar(20)) + ' Order (' +
cast(sum(all_item_num) as nvarchar(20)) + ' item) Unshipping;     Today: ' +
cast(sum(today_order) as nvarchar(20)) + ' Order(' +
cast(sum(today_order_item) as nvarchar(20)) + ' item);     Before: ' +
cast(sum(before_order) as nvarchar(20)) + ' Order(' +
cast(sum(before_order_item) as nvarchar(20)) + ' item)' as total
from
(
select x.FROMID, (select site_name from[t_base_from] where fromid = x.fromid) as site_name,x.all_order_num,x.all_item_num
,(select count(distinct a.order_id) as order_num from[t_biz_order] a where status = 1 and Order_DAY = convert(varchar(8), getdate(), 112) and a.FROMID = x.FROMID) as today_order
,(select count(a.order_id) as order_num from[t_biz_order] a where status = 1 and Order_DAY = convert(varchar(8), getdate(), 112) and a.FROMID = x.FROMID) as today_order_item
,(select count(distinct a.order_id) as order_num from[t_biz_order] a where status = 1 and Order_DAY!= convert(varchar(8), getdate(), 112) and a.FROMID = x.FROMID) as before_order
,(select count(a.order_id) as order_num from[t_biz_order] a where status = 1 and Order_DAY!= convert(varchar(8), getdate(), 112) and a.FROMID = x.FROMID) as before_order_item
from(
select FROMID, count(distinct order_id) as all_order_num, count(order_id) as all_item_num  from[t_biz_order] where status = 1
group by fromid
) x
) y
";
            adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataTable dt_total = new DataTable();
            adapter.Fill(dt_total);
            label1.Text = dt_total.Rows[0]["total"].ToString();
            dt_total = null;


            myConn.Close();
            #endregion
        }


    }
    }
