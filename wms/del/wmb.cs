using ExcelLibrary.SpreadSheet;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using TMS.Framework.Business.DataAccess;
using TMS.Framework.Publics.Data;
using System.Drawing;
using TX.Framework.WindowUI.Forms;

/*
create login wmuser with password='Wms12345678!', default_database=wms;
use wms;
create user wmuser for login wmuser with default_schema=dbo
exec sp_addrolemember 'db_owner', 'wmuser'
*/
namespace UniversalAnalyse
{
    public partial class wmb : MainForm
    {
        DBUtil DB = new DBUtil();
        DataTable dt_xls;
        public wmb()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region clear
            DB.ExecuteSQL(@"
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
truncate table [dbo].[t_imp_13];");
            #endregion
            string ls_load_file = "select fileid,table_name,file_type,file_dir,file_name,field_count,include_head,field_Separator,imp_check_file_time from [t_base_file] where file_type in ('txt','csv','xls') and status='1'";
            DataGroup group_files = new DataGroup();
            group_files = null;
            group_files = DB.GetDataGroup(ls_load_file);
            if (group_files.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_files.Table.Rows.Count; i++)
                {
                    string ls_file_type = group_files.Table.Rows[i]["file_type"].ToString();
                    string ls_file_FullName = group_files.Table.Rows[i]["file_dir"].ToString() + group_files.Table.Rows[i]["file_name"].ToString();
                    System.IO.FileInfo file = new System.IO.FileInfo(ls_file_FullName);
                    string ls_create_time = file.LastWriteTime.ToString("yyyyMMdd");
                    if (DateTime.Now.ToString("yyyyMMdd") == ls_create_time)
                    {
                        string ls_field_Separator = group_files.Table.Rows[i]["field_Separator"].ToString();
                        string ls_table_name = group_files.Table.Rows[i]["table_name"].ToString();
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
                                DB.ExecuteSQL(ls_txt_add);
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
                                string[] dog_csv = ls_csv_is_ect.Replace("\r\n", "|").Split('|');
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
                                            string ls_csv_11 = "";
                                            if (ls_table_name == "t_imp_8" || ls_table_name == "t_imp_11")
                                            {
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

                                            if (ls_table_name == "t_imp_8" || ls_table_name == "t_imp_11")
                                            {
                                                ls_csv_this_csv = " values ('" + ls_csv_bb.Replace("'", "''").Replace(ls_field_Separator, "','").Replace("\"", "") + "');";
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
                                DB.ExecuteSQL(ls_csv_add);
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

                                    dt_xls = NPOIOprateExcel.ExcelUtility.ExcelToDataTable(ls_file_FullName,false);

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
                                                if (dt_xls.Rows[z][0].ToString()== "Unshipped")
                                                { 
                                                ls_paras += "'" + dt_xls.Rows[z][zc].ToString().Replace("'", "''") + "',";
                                                }
                                            }
                                        }

                                        //if (range_is_get.Text.ToString() == "Order Status" || range_is_get.Text.ToString() == "Unshipped")

                                        if (z > 0 && dt_xls.Rows[z][0].ToString() == "Unshipped")
                                        {
                                            string ls_insert = "insert into " + ls_table_name + " (" + ls_xls_left.Substring(0, ls_xls_left.Length - 1) + ") values (" + ls_paras.Substring(0, ls_paras.Length - 1) + ")";
                                            DB.ExecuteSQL(ls_insert);
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
                                            DB.ExecuteSQL(ls_insert);
                                        }
                                    }
                                }
                                    #endregion
                                break;
                        }
                    }
                }


            }

            #region all import data insert into order table
            DB.ExecuteSQL(@"
/*amazon home*/
INSERT INTO [t_base_order]
([FROMID],[Order_DAY],[batch],[order_id],[payments_date]
,[sku],[sys_sku],[item],[product_name],[qty]
,[item_price],[shipping_price],[us_tax],[shipping_class],[item_discount]
,[shipping_discount],[phone],[name],[ship_to_company],[addr1]
,[addr2],[city],[state],[zip],[country]
,[email],[total_price],[total_shipping],[Notes],[ebay_userid],[giftwrap]
,[shipping_details],[status],[remark],[buy_name])
select 
'1',convert(varchar(8), getdate(), 112),'',a.[order-id],b.[payments-date],
a.[sku],'','',a.[product-name],a.[quantity-purchased],
a.[item-price],a.[shipping-price],'',a.[ship-service-level],a.[item-promotion-discount],
a.[ship-promotion-discount],a.[ship-phone-number],b.[recipient-name],'',b.[ship-address-1],
b.[ship-address-2],b.[ship-city],b.[ship-state],b.[ship-postal-code],b.[ship-country],
'','','','','','',
'','1','',a.[buyer-name]
from t_imp_1 a ,t_imp_2 b where a.[order-id]=b.[order-id] and a.[order-item-id]=b.[order-item-id];

/*amazon 2*/
INSERT INTO [t_base_order]
([FROMID],[Order_DAY],[batch],[order_id],[payments_date]
,[sku],[sys_sku],[item],[product_name],[qty]
,[item_price],[shipping_price],[us_tax],[shipping_class],[item_discount]
,[shipping_discount],[phone],[name],[ship_to_company],[addr1]
,[addr2],[city],[state],[zip],[country]
,[email],[total_price],[total_shipping],[Notes],[ebay_userid],[giftwrap]
,[shipping_details],[status],[remark],[buy_name])
select 
'2',convert(varchar(8), getdate(), 112),'',a.[order-id],b.[payments-date],
a.[sku],'','',a.[product-name],a.[quantity-purchased],
a.[item-price],a.[shipping-price],'',a.[ship-service-level],a.[item-promotion-discount],
a.[ship-promotion-discount],a.[ship-phone-number],b.[recipient-name],'',b.[ship-address-1],
b.[ship-address-2],b.[ship-city],b.[ship-state],b.[ship-postal-code],b.[ship-country],
'','','','','','',
'','1','',a.[buyer-name]
from t_imp_3 a ,t_imp_4 b where a.[order-id]=b.[order-id] and a.[order-item-id]=b.[order-item-id];


/*ebay bbn*/
INSERT INTO [t_base_order]
([FROMID],[Order_DAY],[batch],[order_id],[payments_date]
,[sku],[sys_sku],[item],[product_name],[qty]
,[item_price],[shipping_price],[us_tax],[shipping_class],[item_discount]
,[shipping_discount],[phone],[name],[ship_to_company],[addr1]
,[addr2],[city],[state],[zip],[country]
,[email],[total_price],[total_shipping],[Notes],[ebay_userid],[giftwrap]
,[shipping_details],[status],[remark],Global_Shipping_Reference_ID)
select 
'3' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch], [Sales Record Number],[Paid on Date],
[Item Number],[Custom Label],[Custom Label],[Item Title],[Quantity],
[Sale Price],[Shipping and Handling],[US Tax],[shipping service] as [shipping_class],'' as [item_discount],
'' as [shipping_discount],[Buyer Phone Number],[Buyer Fullname],'' as [ship_to_company],[Ship To Address 1],
[Ship To Address 2],[Ship To City],[Ship To State],[Ship To Zip],[Ship To Country],
[Buyer Email],[Total Price],'' as [total_shipping],[Notes to yourself],[User Id],'' as [giftwrap],
'' as [shipping_details],'1' as [status],'' as [remark],[Global Shipping Reference ID]
FROM [t_imp_5] where [item number] !=''  and [Paid on Date]!=''
union
select
'3' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch], [Sales Record Number],(select x.[Paid on Date] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Paid on Date],
[Item Number],[Custom Label],[Custom Label],[Item Title],[Quantity],
[Sale Price],
(select x.[Shipping and Handling] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Shipping and Handling],
(select x.[US Tax] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [US Tax],[shipping service] as [shipping_class],'' as [item_discount],
'' as [shipping_discount],
(select x.[Buyer Phone Number] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Buyer Phone Number],
(select x.[Buyer Fullname] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Buyer Fullname],
'' as [ship_to_company],
(select x.[Ship To Address 1] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 1],
(select x.[Ship To Address 2] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 2],
(select x.[Ship To City] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To City],
(select x.[Ship To State] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To State],
(select x.[Ship To Zip] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To Zip],
(select x.[Ship To Country] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Ship To Country],
(select x.[Buyer Email] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Buyer Email],
(select x.[Total Price] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Total Price],
'' as [total_shipping],
(select x.[Notes to yourself] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [Notes to yourself],
(select x.[User Id] from [t_imp_5] x where x.[Sales Record Number]=[t_imp_5].[Sales Record Number] and x.[Item Number]='') as [User Id],
'' as [giftwrap],
'' as [shipping_details],'1' as [status],'' as [remark],[Global Shipping Reference ID]
FROM [t_imp_5]  where [item number] !='' and [Paid on Date]='';



/*ebay lisa*/
INSERT INTO [t_base_order]
([FROMID],[Order_DAY],[batch],[order_id],[payments_date]
,[sku],[sys_sku],[item],[product_name],[qty]
,[item_price],[shipping_price],[us_tax],[shipping_class],[item_discount]
,[shipping_discount],[phone],[name],[ship_to_company],[addr1]
,[addr2],[city],[state],[zip],[country]
,[email],[total_price],[total_shipping],[Notes],[ebay_userid],[giftwrap]
,[shipping_details],[status],[remark],Global_Shipping_Reference_ID)
select 
'4' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch], [Sales Record Number],[Paid on Date],
[Item Number],[Custom Label],[Custom Label],[Item Title],[Quantity],
[Sale Price],[Shipping and Handling],[US Tax],[shipping service] as [shipping_class],'' as [item_discount],
'' as [shipping_discount],[Buyer Phone Number],[Buyer Fullname],'' as [ship_to_company],[Ship To Address 1],
[Ship To Address 2],[Ship To City],[Ship To State],[Ship To Zip],[Ship To Country],
[Buyer Email],[Total Price],'' as [total_shipping],[Notes to yourself],[User Id],'' as [giftwrap],
'' as [shipping_details],'1' as [status],'' as [remark],[Global Shipping Reference ID]
FROM [t_imp_6] where [item number] !=''  and [Paid on Date]!=''
union
select
'4' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch], [Sales Record Number],(select x.[Paid on Date] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Paid on Date],
[Item Number],[Custom Label],[Custom Label],[Item Title],[Quantity],
[Sale Price],
(select x.[Shipping and Handling] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Shipping and Handling],
(select x.[US Tax] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [US Tax],[shipping service] as [shipping_class],'' as [item_discount],
'' as [shipping_discount],
(select x.[Buyer Phone Number] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Buyer Phone Number],
(select x.[Buyer Fullname] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Buyer Fullname],
'' as [ship_to_company],
(select x.[Ship To Address 1] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 1],
(select x.[Ship To Address 2] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 2],
(select x.[Ship To City] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To City],
(select x.[Ship To State] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To State],
(select x.[Ship To Zip] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To Zip],
(select x.[Ship To Country] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Ship To Country],
(select x.[Buyer Email] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Buyer Email],
(select x.[Total Price] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Total Price],
'' as [total_shipping],
(select x.[Notes to yourself] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [Notes to yourself],
(select x.[User Id] from [t_imp_6] x where x.[Sales Record Number]=[t_imp_6].[Sales Record Number] and x.[Item Number]='') as [User Id],
'' as [giftwrap],
'' as [shipping_details],'1' as [status],'' as [remark],[Global Shipping Reference ID]
FROM [t_imp_6]  where [item number] !='' and [Paid on Date]='';


/*ebay wmb*/
INSERT INTO [t_base_order]
([FROMID],[Order_DAY],[batch],[order_id],[payments_date]
,[sku],[sys_sku],[item],[product_name],[qty]
,[item_price],[shipping_price],[us_tax],[shipping_class],[item_discount]
,[shipping_discount],[phone],[name],[ship_to_company],[addr1]
,[addr2],[city],[state],[zip],[country]
,[email],[total_price],[total_shipping],[Notes],[ebay_userid],[giftwrap]
,[shipping_details],[status],[remark],Global_Shipping_Reference_ID)
select 
'5' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch], [Sales Record Number],[Paid on Date],
[Item Number],[Custom Label],[Custom Label],[Item Title],[Quantity],
[Sale Price],[Shipping and Handling],[US Tax],[shipping service] as [shipping_class],'' as [item_discount],
'' as [shipping_discount],[Buyer Phone Number],[Buyer Fullname],'' as [ship_to_company],[Ship To Address 1],
[Ship To Address 2],[Ship To City],[Ship To State],[Ship To Zip],[Ship To Country],
[Buyer Email],[Total Price],'' as [total_shipping],[Notes to yourself],[User Id],'' as [giftwrap],
'' as [shipping_details],'1' as [status],'' as [remark],[Global Shipping Reference ID]
FROM [t_imp_7] where [item number] !=''  and [Paid on Date]!=''
union
select
'5' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch], [Sales Record Number],(select x.[Paid on Date] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Paid on Date],
[Item Number],[Custom Label],[Custom Label],[Item Title],[Quantity],
[Sale Price],
(select x.[Shipping and Handling] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Shipping and Handling],
(select x.[US Tax] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [US Tax],[shipping service] as [shipping_class],'' as [item_discount],
'' as [shipping_discount],
(select x.[Buyer Phone Number] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Buyer Phone Number],
(select x.[Buyer Fullname] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Buyer Fullname],
'' as [ship_to_company],
(select x.[Ship To Address 1] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 1],
(select x.[Ship To Address 2] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To Address 2],
(select x.[Ship To City] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To City],
(select x.[Ship To State] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To State],
(select x.[Ship To Zip] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To Zip],
(select x.[Ship To Country] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Ship To Country],
(select x.[Buyer Email] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Buyer Email],
(select x.[Total Price] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Total Price],
'' as [total_shipping],
(select x.[Notes to yourself] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [Notes to yourself],
(select x.[User Id] from [t_imp_7] x where x.[Sales Record Number]=[t_imp_7].[Sales Record Number] and x.[Item Number]='') as [User Id],
'' as [giftwrap],
'' as [shipping_details],'1' as [status],'' as [remark],[Global Shipping Reference ID]
FROM [t_imp_7]  where [item number] !='' and [Paid on Date]='';



/*WeMakeBeauty.com*/
INSERT INTO [t_base_order]
([FROMID],[Order_DAY],[batch],[order_id],[payments_date]
,[sku],[sys_sku],[item],[product_name],[qty]
,[item_price],[shipping_price],[us_tax],[shipping_class],[item_discount]
,[shipping_discount],[phone],[name],[ship_to_company],[addr1]
,[addr2],[city],[state],[zip],[country]
,[email],[total_price],[total_shipping],[Notes],[ebay_userid],[giftwrap]
,[shipping_details],[status],[remark],buy_name)
select 
'6' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch],a.[orderid],a.[orderdate],
b.[productcode],b.[productcode],b.[productcode],b.[productname],b.[quantity],
b.[productprice],a.[totalshippingcost],cast(salestax1 as float)+cast(salestax2 as float)+cast(salestax3 as float) as [us-tax],a.[shippingmethodid],'',
'',a.[shipphonenumber],a.[shipfirstname]+' '+a.[shiplastname],a.[shipcompanyname],a.[shipaddress1],
a.[shipaddress2],a.[shipcity],a.[shipstate],a.[shippostalcode],a.[shipcountry],
'',total_payment_received,'',a.[ordernotes],'',b.[giftwrap],
'','1','',billingfirstname+' '+billinglastname
from t_imp_8 a,
(select orderid,productcode,productname,productprice,giftwrap,sum(cast(quantity as float)) as quantity from t_imp_9
group by orderid,productcode,productname,productprice,giftwrap) as b
where a.[orderid]=b.[orderid];
/*
select 
'6' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch],a.[orderid],a.[orderdate],
b.[productcode],b.[productcode],b.[productcode],b.[productname],b.[quantity],
b.[productprice],a.[totalshippingcost],cast(salestax1 as float)+cast(salestax2 as float)+cast(salestax3 as float) as [us-tax],a.[shippingmethodid],'',
'',a.[shipphonenumber],a.[shipfirstname]+' '+a.[shiplastname],a.[shipcompanyname],a.[shipaddress1],
a.[shipaddress2],a.[shipcity],a.[shipstate],a.[shippostalcode],a.[shipcountry],
'',total_payment_received,'',a.[ordernotes],'',b.[giftwrap],
'','1','',billingfirstname+' '+billinglastname
from t_imp_8 a,t_imp_9 b where a.[orderid]=b.[orderid];
*/


/*Over Stock*/
INSERT INTO [t_base_order]
([FROMID],[Order_DAY],[batch],[order_id],[payments_date]
,[sku],[sys_sku],[item],[product_name],[qty]
,[item_price],[shipping_price],[us_tax],[shipping_class],[item_discount]
,[shipping_discount],[phone],[name],[ship_to_company],[addr1]
,[addr2],[city],[state],[zip],[country]
,[email],[total_price],[total_shipping],[Notes],[ebay_userid],[giftwrap]
,[shipping_details],[status],[remark])
select 
'7' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch],[Order #],[Overstock Order Date],
[Overstock SKU],[Partner SKU],[Partner SKU],'',[Quantity],
[Unit Price],[Shipping Cost],'',[Ship Method],'',
'','','','','',
'','','','','',
'','','','','','',
[Shipping Details],'1',''
FROM [t_imp_10];


/*buy.com*/
INSERT INTO [t_base_order]
([FROMID],[Order_DAY],[batch],[order_id],[payments_date]
,[sku],[sys_sku],[item],[product_name],[qty]
,[item_price],[shipping_price],[us_tax],[shipping_class],[item_discount]
,[shipping_discount],[phone],[name],[ship_to_company],[addr1]
,[addr2],[city],[state],[zip],[country]
,[email],[total_price],[total_shipping],[Notes],[ebay_userid],[giftwrap]
,[shipping_details],[status],[remark],buy_name)
select 
'8' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch],[OrderId],[OrderDate]
,[Sku],[ReferenceId],'',[ItemName],CONVERT(bigint,[Qty Ordered])-CONVERT(bigint,[Qty Shipped])-CONVERT(bigint,[Qty Cancelled]) as qty
,[Price],[ShippingCost],[TaxCost],[ShippingMethodId],'',
'',[billtophone],[ShipToName],[ShipToCompany],[ShipToStreet1],
[ShipToStreet2],[ShipToCity],[ShipToState],[ShipToZipCode],'',
'','','','','','',
'','1','',billtofirstname+' '+billtolastname
FROM [t_imp_11];



/*new egg*/
INSERT INTO [t_base_order]
([FROMID],[Order_DAY],[batch],[order_id],[payments_date]
,[sku],[sys_sku],[item],[product_name],[qty]
,[item_price],[shipping_price],[us_tax],[shipping_class],[item_discount]
,[shipping_discount],[phone],[name],[ship_to_company],[addr1]
,[addr2],[city],[state],[zip],[country]
,[email],[total_price],[total_shipping],[Notes],[ebay_userid],[giftwrap]
,[shipping_details],[status],[remark])
select 
'9' as [FROMID],convert(varchar(8), getdate(), 112) as [Order_DAY],'' as [batch],[Order Number],[Order Date & Time]
,[Item Newegg #],[Item Seller Part #],'','',[Item Quantity Ordered],
[Item Unit Price],[Item Shipping Charge],'',[Order Shipping Method],'',
'',[Ship To Phone Number],[Ship To Name],[Ship To Company],[Ship To Address Line 1],
[Ship To Address Line 2],[Ship To City],[Ship To State],[Ship To Zipcode],[Ship to Country],
'',[Order Total],[Order Shipping Total],'','',''
,'','1',''
FROM [t_imp_13] where  [Order Status]='Unshipped';

");

            #endregion




            #region show site result
            //dataGridView1.AutoGenerateColumns = false;
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();
            string sqlstr = @"select x.FROMID,(select site_name from [t_base_from] where fromid=x.fromid) as site_name,x.order_num,x.item_num from (
select FROMID,count(distinct order_id) as order_num ,count(order_id) as item_num  from [t_base_order] where Order_DAY=convert(varchar(8), getdate(), 112)
group by fromid) x";
            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            //dataGridView1.Columns[0].DataPropertyName = "Item";
            //dataGridView1.Columns[1].DataPropertyName = "Item_Name";
            //dataGridView1.Columns[2].DataPropertyName = "Barcode";
            //dataGridView1.Columns[3].DataPropertyName = "Weight";
            myConn.Close();
            #endregion

            #region set data full
            DataGroup group_access = new DataGroup();
            group_access = null;
            group_access = DB.GetDataGroup("select [access_dir],[access_table],[sql_table],[insert_f],[select_f] from [t_base_access] where status=1;");
            if (group_access.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_access.Table.Rows.Count; i++)
                {
                    string ls_access_dir = group_access.Table.Rows[i]["access_dir"].ToString();
                    string ls_access_table = group_access.Table.Rows[i]["access_table"].ToString();
                    string ls_select_f = group_access.Table.Rows[i]["select_f"].ToString();
                    string ls_sql_table = group_access.Table.Rows[i]["sql_table"].ToString();
                    DB.ExecuteSQL("truncate table " + ls_sql_table + ";");
                    //change cpu x86
                    OleDbConnection objConn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ls_access_dir + ";");
                    OleDbCommand MDBCommand = new OleDbCommand("Select " + ls_select_f + " FROM" + ls_access_table, objConn);
                    OleDbDataReader rdr2;
                    objConn.Open();
                    rdr2 = MDBCommand.ExecuteReader();
                    SqlBulkCopy sbc = new SqlBulkCopy(ConfigurationManager.AppSettings["dbConnectionString"]);
                    sbc.DestinationTableName = ls_sql_table;
                    sbc.WriteToServer(rdr2);
                    sbc.Close();
                    rdr2.Close();
                    objConn.Close();
                }
            }
            group_access = null;

            //update amz item data
            DB.ExecuteSQL(@"
update [t_base_order] set sys_sku =
case when substring(sku,1,1)='A' then  Substring(sku,2,len(sku)-1)
when substring(sku,1,1)='B' then  Substring(sku,2,len(sku)-1)
when substring(sku,1,1)='C' then  Substring(sku,2,len(sku)-1)
when substring(sku,1,1)='D' then  Substring(sku,2,len(sku)-1)
when substring(sku,1,1)='E' then  Substring(sku,2,len(sku)-1)
ELSE sku END where fromid='1' and [item]='';
update [t_base_order] set item =(select item from [t_amazon_home_full_sku_list] where [SKU(meaningless)]=[t_base_order].[sys_sku])
where fromid ='1' and [item]='';
update [t_base_order] set sys_sku =
case when substring(sku,1,2)='10' then  Substring(sku,3,len(sku)-2)
when substring(sku,1,2)='20' then  Substring(sku,3,len(sku)-2)
when substring(sku,1,3)='30' then  Substring(sku,3,len(sku)-2)
when substring(sku,1,4)='40' then  Substring(sku,3,len(sku)-2)
when substring(sku,1,5)='50' then  Substring(sku,3,len(sku)-2)
ELSE sku END where fromid='2' and [item]='';
update [t_base_order] set item =(select item from [t_amazon_2_full_sku_list] where [SKU(meaningless)]=[t_base_order].[sys_sku])
where fromid ='2' and [item]='';
update [t_base_order] set item =
case when substring(sys_sku,1,2)='11' then  Substring(sys_sku,3,len(sys_sku)-2)
ELSE sys_sku END where fromid ='9' and [item]='';
update [t_base_order] set item =
case when substring(sys_sku,1,3)='11-' then  Substring(sys_sku,4,len(sys_sku)-3)
ELSE sys_sku END where fromid='8' and [item]='';
");

            //update over stock product name
            DB.ExecuteSQL("update t_base_order set product_name=(select [Product Name] from [t_ov_Active_Listing_Table] where [Partner SKU]=t_base_order.sys_sku) where product_name='' and fromid='7'");

            //update new egg stock product name
            DB.ExecuteSQL("update t_base_order set product_name=(select [title] from [t_vol_Listing_Table] where '11'+[custom label]=t_base_order.sys_sku) where (product_name is null or product_name='' ) and fromid='9'");

            //split over stock addr
            string ls_ov_sql = "select oid,shipping_details from [t_base_order] where  fromid='7' and name='' ";
            DataGroup group_ov_addr = new DataGroup();
            group_ov_addr = null;
            group_ov_addr = DB.GetDataGroup(ls_ov_sql);
            if (group_ov_addr.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_ov_addr.Table.Rows.Count; i++)
                {
                    string ls_oid = group_ov_addr.Table.Rows[i]["oid"].ToString();
                    string ls_addr = group_ov_addr.Table.Rows[i]["shipping_details"].ToString();
                    string[] dog_small = ls_addr.Replace("        ", "|").Split('|');
                    DB.ExecuteSQL("update [t_base_order] set name='" + dog_small[0].ToString() + "',addr1='" + dog_small[1].ToString() + "',addr2='" + dog_small[2].ToString() + "',city='" + dog_small[3].ToString() + "',zip='" + dog_small[4].ToString() + "' where  fromid='7' and name='' and oid='" + ls_oid + "' ");
                }
            }
            #endregion

        }

        

        private void button2_Click(object sender, EventArgs e)
        {

        }


        private void button3_Click(object sender, EventArgs e)
        {
            #region create invoice head
            string ls_dir = "d:\\" + DateTime.Now.ToString("yyyy-MM") + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
            string ls_style_head = "";
            DataGroup group_file = new DataGroup();
            group_file = null;
            group_file = DB.GetDataGroup("select fromid,(select site_code from t_base_from where fromid=[t_base_order].FROMID) as site_code from [t_base_order]  where status=1  and order_day=convert(varchar(8), getdate(), 112) group by fromid ;");
            if (group_file.Table.Rows.Count > 0)
            {
                for (int i_file = 0; i_file < group_file.Table.Rows.Count; i_file++)
                {
                    string ls_fromid = group_file.Table.Rows[i_file]["fromid"].ToString();
                    string ls_site_code = group_file.Table.Rows[i_file]["site_code"].ToString();


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
                            ls_style_head = "<style type='text/css'>table{border-collapse:collapse;-webkit-print-color-adjust: exact;}.tb_item table, .tb_item th, .tb_item td {border: 1px solid black;} .ft1{font-size:24px;font-family:Helvetica;color:#000000;font-weight:bold;}.ft2{font-size:14px;font-family:Helvetica;color:#000000;padding-left:4px;} p{ font-size:14px;font-family:Helvetica;color:#000000;margin:0 auto;padding-left:4px}body {  MARGIN: 0px;PADDING: 0px;}</style>";
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
                    string ls_file = ls_dir + "\\"+ ls_site_code + ".html";
                    using (FileStream fs = File.Open(ls_file, FileMode.Create))
                    {
                        byte[] b = System.Text.Encoding.Default.GetBytes(ls_style_head);
                        fs.Write(b, 0, b.Length);
                        fs.Close();
                        ls_style_head = "";
                    }
                }
            }
            group_file = null;
            #endregion

            DataGroup group_order = new DataGroup();
            group_order = null;
            group_order = DB.GetDataGroup(@"
select x.order_id,x.fromid,x.site_code,x.nb,x.sum_order,x.sum_tax,x.sum_shipping,x.sum_item_discount,x.sum_shipping_discount,x.bar_code,x.total_price,x.Notes,x.sum_discount_forwbm,x.sum_giftwrap,x.us_tax_wmbcom,x.sum_shipping_wmbcom from
(
select fromid,(select site_code from t_base_from where fromid=[t_base_order].FROMID) as site_code,[order_id],count(oid) as nb,
sum(cast(qty as float)*cast(replace(replace(item_price,'$',''),'Replacement','') as float)) as sum_order, 
sum(cast(isnull(CASE WHEN replace(us_tax,'$','')='' THEN '0' else replace(us_tax,'$','') END,'0') as float)) as sum_tax, 
sum(cast(isnull(CASE WHEN replace(shipping_price,'$','')='' THEN '0' else replace(shipping_price,'$','') END,'0') as float)) as sum_shipping, 
sum(cast(item_discount as float)) as sum_item_discount, 
sum(cast(shipping_discount as float)) as sum_shipping_discount,
min(item) as ordersku,min(oid) as bar_code,
max(total_price) as total_price,
max(Notes) as Notes,
isnull(sum(
case when
substring(replace(item_price,'$',''),1,1)='-'
then cast(replace(replace(item_price,'$',''),'Replacement','') as float) end
),0) as sum_discount_forwbm,
sum(cast(isnull(CASE WHEN replace(giftwrap,'$','')='' THEN '0' else replace(giftwrap,'$','') END,'0') as float)) as sum_giftwrap,
max(us_tax) as us_tax_wmbcom,
isnull(max(replace(shipping_price,'$','')),0) as sum_shipping_wmbcom
from[t_base_order]  where status=1  and order_day=convert(varchar(8), getdate(), 112) group by fromid,[order_id]
) x order by x.ordersku
");
            if (group_order.Table.Rows.Count > 0)
            {
                for (int i_order = 0; i_order < group_order.Table.Rows.Count; i_order++)
                {
                    #region get invoice main value
                    string ls_dir_bc = ls_dir + "\\barcode\\";
                    DirectoryInfo d_dir = new DirectoryInfo(ls_dir_bc);
                    if (!d_dir.Exists)
                    {
                        d_dir.Create();
                    }

                    string ls_bar_code = group_order.Table.Rows[i_order]["bar_code"].ToString();
                    //Bitmap oBmp = GetCode39(ls_bar_code);
                    //oBmp.Save(ls_dir_bc + ls_bar_code + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    //oBmp.Dispose();

                    float ls_order = float.Parse(group_order.Table.Rows[i_order]["sum_order"].ToString());
                    float ls_ship  = float.Parse(group_order.Table.Rows[i_order]["sum_shipping"].ToString()) - float.Parse(group_order.Table.Rows[i_order]["sum_shipping_discount"].ToString());
                    float ls_tax   = float.Parse(group_order.Table.Rows[i_order]["sum_tax"].ToString());
                    float ls_amz_total = ls_order + ls_ship + ls_tax;

                    string ls_ebay_wmbcom_total_price = group_order.Table.Rows[i_order]["total_price"].ToString();
                    string ls_site_code= group_order.Table.Rows[i_order]["site_code"].ToString();
                    string ls_fromid = group_order.Table.Rows[i_order]["fromid"].ToString();
                    string ls_order_id_main = group_order.Table.Rows[i_order]["order_id"].ToString();
                    string ls_nb = group_order.Table.Rows[i_order]["nb"].ToString();

                    string ls_main_notes = group_order.Table.Rows[i_order]["Notes"].ToString();
                    string ls_main_sum_discount_forwmb = group_order.Table.Rows[i_order]["sum_discount_forwbm"].ToString();
                    string ls_main_sum_giftwrap = group_order.Table.Rows[i_order]["sum_giftwrap"].ToString();
                    string ls_us_tax_wmbcom = group_order.Table.Rows[i_order]["us_tax_wmbcom"].ToString().Replace("$","");
                    if (ls_us_tax_wmbcom == "")
                    {
                        ls_us_tax_wmbcom = "0";
                    }

                    string ls_sum_shipping_wmbcom = group_order.Table.Rows[i_order]["sum_shipping_wmbcom"].ToString();
                    float ls_main_sum_total_forwmb=ls_order + float.Parse(ls_sum_shipping_wmbcom) + float.Parse(ls_us_tax_wmbcom) + float.Parse(ls_main_sum_giftwrap);

                    string ls_html = "";
                    #endregion
                    DataGroup group_invoice = new DataGroup();
                    group_invoice = null;
                    group_invoice = DB.GetDataGroup("select oid,name,buy_name,ebay_userid,Global_Shipping_Reference_ID,addr1,addr2,city,state,zip,country,[payments_date],[order_id],[shipping_class],(select top 1 [ShipService] from t_vol_shippingid where [shippingmethodid]=[t_base_order].[shipping_class]) as shipping_name,[qty],[sku],[product_name],[item],[item_price],Notes from [t_base_order] where status=1 and order_day=convert(varchar(8), getdate(), 112) "+
                        "and order_id='" + ls_order_id_main + "' order by item;");
                    if (group_invoice.Table.Rows.Count > 0)
                    {
                        for (int i = 0; i < group_invoice.Table.Rows.Count; i++)
                        {
                            #region get item value

                            string ls_oid = group_invoice.Table.Rows[i]["oid"].ToString();
                            string ls_name = group_invoice.Table.Rows[i]["name"].ToString();
                            string ls_buy_name= group_invoice.Table.Rows[i]["buy_name"].ToString();
                            string ls_ebay_userid = group_invoice.Table.Rows[i]["ebay_userid"].ToString();
                            string ls_Global_Shipping_Reference_ID = group_invoice.Table.Rows[i]["Global_Shipping_Reference_ID"].ToString();
                            string ls_addr1 = group_invoice.Table.Rows[i]["addr1"].ToString();
                            string ls_addr2 = group_invoice.Table.Rows[i]["addr2"].ToString();
                            string ls_city = group_invoice.Table.Rows[i]["city"].ToString();
                            string ls_state = group_invoice.Table.Rows[i]["state"].ToString();
                            string ls_zip = group_invoice.Table.Rows[i]["zip"].ToString();
                            string ls_country = group_invoice.Table.Rows[i]["country"].ToString();
                            string ls_payments_date = group_invoice.Table.Rows[i]["payments_date"].ToString();
                            string ls_payments_date_shot = DateTime.Parse(ls_payments_date).ToString("MM/dd/yyyy");
                            string ls_order_id = group_invoice.Table.Rows[i]["order_id"].ToString();
                            string ls_shipping_class = group_invoice.Table.Rows[i]["shipping_class"].ToString();
                            string ls_shipping_name = group_invoice.Table.Rows[i]["shipping_name"].ToString();
                            string ls_qty = group_invoice.Table.Rows[i]["qty"].ToString();
                            //string ls_sku = group_invoice.Table.Rows[i]["sku"].ToString();
                            string ls_product_name = group_invoice.Table.Rows[i]["product_name"].ToString();
                            string ls_item = group_invoice.Table.Rows[i]["item"].ToString();
                            string ls_Notes = group_invoice.Table.Rows[i]["Notes"].ToString();
                            string ls_item_price = group_invoice.Table.Rows[i]["item_price"].ToString().Replace("$", "");
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
                                        ls_html = "<body><div align='center' style='width:720px'><img src='file://d:\\Pictures\\" + ls_site_code + "\\la_Secret.jpg' width='248' /><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></div><br><div align='right'></div>";

                                        if (ls_shipping_class != "Standard")
                                        {
                                            ls_html += "<div align='left'><img src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/></div>";
                                        }
                                        ls_html += "<br><table><td align='left' width='150'>Amazon Order ID:</td><td align='left'><strong>" + ls_order_id + "</strong></td><td align ='right'>Daily Order No.:</td><td align='right' width='70'><strong>" + ls_oid + "</strong></td></table><table class='bdout'><td>Ship to:</td><td> </td><td> </td></tr>";
                                        ls_html += "<tr><td width='350'>" + ls_name + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr>";
                                        ls_html += "<tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_buy_name + "</td></tr>";

                                        if (ls_addr2.Length > 0)
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_class + "</td></tr>";
                                            ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>Beauty Secret LA</td></tr>";
                                            ls_html += "<tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr></table><br>";
                                        }
                                        else
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_class + "</td></tr>";
                                            ls_html += "<tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>Beauty Secret LA</td></tr></table><br>";
                                        }
                                        
                                        ls_html += "<table class='bdout'><tr align ='center'><td width='30'class='bdout'>QTY</td><td class='bdout' width='130'>SKU</td><td class='bdout'>Product Detail</td><td class='bdout' width='100'>price</td><td width='100' class='bdout'>Subtotal</td></tr>";
                                    }

                                    ls_html += "<tr class='itemr'><td width='30' align='center'>" + ls_qty + "</td><td class='bdside' width='130'>" + ls_item + "</td><td width='400'>" + ls_product_name + "</td><td class='bdside' width='100' align='right'>" + ls_item_price + "</td><td width='100'align='right'>" + ls_amount + "</td></tr>";

                                    if (ls_nb == (i+1).ToString() )
                                    for (int inb=(i+1);inb<6;inb++)
                                    { 
                                        ls_html += "<tr class='itemr'><td width='30' align='center'></td><td class='bdside' width='130'></td><td width='400'></td><td class='bdside' width='100' align='right'></td><td width='100'align='right'></td></tr>";
                                    }


                                    break;
                                #endregion
                                case "2":
                                    #region create invoice ama_2 body
                                    if (i == 0)
                                    {
                                        ls_html = "<body><div align='center' style='width:720px'><img src='file://d:\\Pictures\\" + ls_site_code + "\\amazonLogo.jpg' width='248' /><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></div><br><div align='right'></div>";

                                        if (ls_shipping_class != "Standard")
                                        {
                                            ls_html += "<div align='left'><img src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/></div>";
                                        }
                                        ls_html += "<br><table><td align='left' width='150'>Amazon Order ID:</td><td align='left'><strong>" + ls_order_id + "</strong></td><td align ='right'>Daily Order No.:</td><td align='right' width='70'><strong>" + ls_oid + "</strong></td></table><table class='bdout'><td>Ship to:</td><td> </td><td> </td></tr>";
                                        ls_html += "<tr><td width='350'>" + ls_name + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr>";
                                        ls_html += "<tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_buy_name + "</td></tr>";

                                        if (ls_addr2.Length > 0)
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_class + "</td></tr>";
                                            ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>Luxury Shop 4 Less</td></tr>";
                                            ls_html += "<tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr></table><br>";
                                        }
                                        else
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_class + "</td></tr>";
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
                                        ls_html = "<body><table><tr><td align='right' width='350'><img src='file://d:\\Pictures\\" + ls_site_code + "\\Ebaylogo.jpg' width='290' height='58'/></td><td align='right'><img src='file://d:\\Pictures\\" + ls_site_code + "\\logoEbay_x45.gif' height ='45'></td></tr><tr><td valign='bottom' align='left'>";

                                        if (ls_shipping_class.IndexOf("Standard") != -1 && ls_shipping_class.IndexOf("First Class") != -1)
                                        {
                                            ls_html += "<img src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
                                        }
                                        ls_html += "</td><td align='right'><table style='width:320px;'><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table></td></tr></table>";
                                        ls_html += "<br><table><td align='left' width='200'>Sales Record Number:</td><td align='left'><strong>" + ls_order_id + "</strong></td></table><table class='bdout'><tr><td>Ship to:</td><td> </td><td> </td></tr>";
                                        ls_html += "<tr><td width='350'>" + ls_name + "<br/>" + ls_Global_Shipping_Reference_ID + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr><tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_ebay_userid + "</td></tr>";
                                        if (ls_addr2.Length > 0)
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_class + "</td></tr><tr><td width='350'>" + ls_city + ", " + ls_state +" "+ ls_zip + "</td><td width='130'>Seller:</td><td>BestBeautyNet</td></tr><tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1' colspan='2'><strong>" + ls_Notes + "</strong></td></tr></table><br>";
                                        }
                                        else
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_class + "</td></tr><tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>BestBeautyNet</td></tr><tr><td width='350'> </td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1'><strong>" + ls_Notes + "</strong></td><td class='bdtop'></td></tr></table><br>";
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
                                        ls_html = "<body><table><tr><td align='right' width='350'><img src='file://d:\\Pictures\\" + ls_site_code + "\\logo.jpg' width='248' height='70'/></td><td align='right'><img src='file://d:\\Pictures\\" + ls_site_code + "\\logoEbay_x45.gif' height ='45'></td></tr><tr><td valign='bottom' align='left'>";

                                        if (ls_shipping_class.IndexOf("Standard") != -1 && ls_shipping_class.IndexOf("First Class") != -1)
                                        {
                                            ls_html += "<img src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
                                        }
                                        ls_html += "</td><td align='right'><table style='width:320px;'><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table></td></tr></table>";
                                        ls_html += "<br><table><td align='left' width='200'>Sales Record Number:</td><td align='left'><strong>" + ls_order_id + "</strong></td></table><table class='bdout'><tr><td>Ship to:</td><td> </td><td> </td></tr>";
                                        ls_html += "<tr><td width='350'>" + ls_name + "<br/>" + ls_Global_Shipping_Reference_ID + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr><tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_ebay_userid + "</td></tr>";
                                        if (ls_addr2.Length > 0)
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_class + "</td></tr><tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>Lisaperfumes</td></tr><tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1' colspan='2'><strong>" + ls_Notes + "</strong></td></tr></table><br>";
                                        }
                                        else
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_class + "</td></tr><tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>Lisaperfumes</td></tr><tr><td width='350'> </td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1'><strong>" + ls_Notes + "</strong></td><td class='bdtop'></td></tr></table><br>";
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
                                        ls_html = "<body><table><tr><td align='right' width='350'><img src='file://d:\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/></td><td align='right'><img src='file://d:\\Pictures\\" + ls_site_code + "\\logoEbay_x45.gif' height ='45'></td></tr><tr><td valign='bottom' align='left'>";
                                        if (ls_shipping_class.IndexOf("Standard") != -1 && ls_shipping_class.IndexOf("First Class") != -1)
                                        {
                                            ls_html += "<img src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
                                        }
                                        ls_html += "</td><td align='right'><table style='width:320px;'><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table></td></tr></table>";
                                        ls_html += "<br><table><td align='left' width='200'>Sales Record Number:</td><td align='left'><strong>" + ls_order_id + "</strong></td></table><table class='bdout'><tr><td>Ship to:</td><td> </td><td> </td></tr>";
                                        ls_html += "<tr><td width='350'>" + ls_name + "<br/>" + ls_Global_Shipping_Reference_ID + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr><tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_ebay_userid + "</td></tr>";
                                        if (ls_addr2.Length > 0)
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_class + "</td></tr><tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>WeMakeBeauty</td></tr><tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1' colspan='2'><strong>" + ls_Notes + "</strong></td></tr></table><br>";
                                        }
                                        else
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td width ='260'>" + ls_shipping_class + "</td></tr><tr><td width='350'>" + ls_country + "</td><td width='130'>Seller:</td><td>WeMakeBeauty</td></tr><tr><td width='350'> </td><td> </td><td> </td></tr><tr><td class='bdtop'>Request Note:</td><td class='bdtop' width='400' rowspan='1'><strong>" + ls_Notes + "</strong></td><td class='bdtop'></td></tr></table><br>";
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
                                        ls_html = "<div align='center'><img src='file://d:\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/></div><br><div align='right'>";

                                        if (ls_shipping_name.IndexOf("Standard Ground (U.S. Domestic)") != -1 && ls_shipping_name.IndexOf("Free Standard Shipping (U.S. Domestic)") != -1)
                                        {
                                            ls_html += "<img style='float:left' src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
                                        }
                                        ls_html += "<table style='width:320px;'><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table><p style='clear:both;'></p></div>";
                                        ls_html += "<br><table><td align='left' width='150'>Order ID:</td><td align='left'><strong>" + ls_order_id + "</strong></td><td align ='right'>Daily Order No.:</td><td align='right' width='70'><strong>" + ls_oid + "</strong></td></table><table class='bdout'><tr><td>Ship to:</td><td> </td><td> </td></tr>";
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
                                    if (i == 0)
                                    {
                                        ls_html = "<table cellpadding='0' cellspacing='0' width='919px'>" +
                                        "<tr><td colspan='3' ><img src='file://d:\\Pictures\\" + ls_site_code + "\\logo.png'  width='300'/><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr>" +
                                        "<tr><td colspan='3' height='42'></td></tr><tr><td colspan='3' align='right' height='17'>" +
                                        "<table width='826'><tr align='left'><td height='22' width='365'><span class='ft1'>Ship To:</span></td><td width='365'><span class='ft1'>Send To:</span></td></tr>" +
                                        "<tr height='66' align='left' style='vertical-align:text-top;'>";

                                        ls_html += "<td><p>" + ls_name + "</p></td>";
                                        ls_html += "<td><p>" + ls_addr1 + "<br/>" + ls_addr2 + "<br/>" + ls_city + ls_zip + "</p></td>";

                                        ls_html += @"</tr></table></td></tr><tr><td colspan='3' height='22'></td></tr>
<tr height='41'><td colspan='3' align='right'><table width='826' class='tb_item'>
<tr align='left' bgcolor='#e6e6e6'><td height='22' widsth='25%'>
<span class='ft2'>Order Date</span></td><td width='25%'>
<span class='ft2'>Order Number</span></td><td width='25%'>
<span class='ft2'>Ship Via</span></td><td width='25%'>
<span class='ft2'>Ship Method</span></td></tr>
<tr height='22' align='left'>";

                                        ls_html += "<td><p>" + ls_payments_date_shot + "</p></td>";
                                        ls_html += "<td><p>" + ls_order_id + "</p></td>";
                                        ls_html += "<td><p>BEST</p></td>";
                                        ls_html += "<td><p>" + ls_shipping_class + "</p></td>";

                                        ls_html += @"</tr></table></td></tr><tr><td colspan='3' height='22'></td></tr><tr><td colspan='3' align='right' >
<table width='826' class ='tb_item'><tr align='left' bgcolor='#e6e6e6'><td height='22' width='126'>
<span class='ft2'>Quantity Ordered</span></td><td width='148'><span class='ft2'>Item Number</span></td>
<td width='185'><span class='ft2'>Description</span></td><td width='126'><span class='ft2'>Quantity Shipped</span></td><td width='163'>
<span class='ft2'>Vendor Sku</span></td><td width='78'><span class='ft2'>Price</span></td></tr>";
                                    }

                                    ls_html += "<tr height='55' align='left'><td ><p>" + ls_qty + "</p></td>";
                                    ls_html += "<td ><p>" + ls_item + "</p></td>";
                                    ls_html += "<td ><p>" + ls_product_name + "</p></td>";
                                    ls_html += "<td ><p>" + ls_qty + "</p></td>";
                                    ls_html += "<td ><p>" + ls_item + "</p></td>";
                                    ls_html += "<td ><p>" + ls_amount + "</p></td></tr>";
                                    break;
                                #endregion
                                case "8":
                                    #region create invoice buy body
                                    if (i == 0)
                                    {
                                        ls_html = "<table><tr><td width='180'></td><td align = 'center'><img src='file://d:\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/></td><td width='180' align = 'right' valign = 'top'><img src='file://d:\\Pictures\\" + ls_site_code + "\\Logo_Rekuton.jpg' width='180' height='40'/></td></tr></table><br><div align='right'><blockquote valign='bottom'>";

                                        if (ls_shipping_name != "Standard")
                                        {
                                            ls_html += "<img style='float:left' src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
                                        }
                                        ls_html += "<table style='width:680px;'><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table><p style='clear:both;'></p></blockquote></div>";
                                        ls_html += "<br><table><td align='left' width='80'>Order ID:</td><td align='left'><strong>" + ls_order_id + "</strong></td><td align ='right'>Daily Order No.:</td><td align='left' width='70'><strong>" + ls_oid + "</strong></td></table><table class='bdout'><td>Ship to:</td><td> </td><td> </td></tr>";
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
                                        ls_html = "<div align='center'><img src='file://d:\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/><img style='float:right' src='file://d:\\Pictures\\" + ls_site_code + "\\neLogo.png' height='70px'/></div>";
                                        if (ls_shipping_class.IndexOf("Standard") == -1)
                                        {
                                            ls_html += "<div><img style='float:left' src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/></div>";
                                        }
                                        ls_html += "<table><tr><td style='height:30px;'><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr></table>";
                                        ls_html += "<br><table><td align='left' width='150'>NewEgg Order ID:</td><td align='left'><strong>" + ls_order_id + "</strong></td><td align ='right'>Daily Order No.:</td><td align='right' width='70'><strong>" + ls_oid + "</strong></td></table><table class='bdout'><td>Ship to:</td><td> </td><td> </td></tr>";
                                        ls_html += "<tr><td width='350'>" + ls_name + "</td><td width='130'>Payment Date:</td><td>" + ls_payments_date + "</td></tr>";
                                        ls_html += "<tr><td width='350'>" + ls_addr1 + "</td><td width='130'>Buyer Name:</td><td>" + ls_name + "</td></tr>";
                                        if (ls_addr2.Length > 0)
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_addr2 + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_class + "</td></tr>";
                                            ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Seller:</td><td>WeMakeBeauty</td></tr>";
                                            ls_html += "<tr><td width='350'>" + ls_country + "</td><td> </td><td> </td></tr></table><br>";
                                        }
                                        else
                                        {
                                            ls_html += "<tr><td width='350'>" + ls_city + ", " + ls_state + " " + ls_zip + "</td><td width='130'>Shipping Method:</td><td>" + ls_shipping_class + "</td></tr>";
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
                    }


                    switch (ls_fromid)
                    {
                        case "1":
                            ls_html += "<tr><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_ship.ToString() + "</td></tr>";
                            ls_html += "<tr><td class='bdleft'> </td><td> </td><td> </td><td class='bdside'>Tax:</td><td class='bdside' align='right'>" + ls_tax.ToString() + "</td></tr>";
                            ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_amz_total.ToString() + "</td></tr></table>";
                            ls_html += "<div width = '700'>Thank you for buying at Beauty Secret LA @ Amazon Marketplace. To provide feeback for the seller please visit: www.amazon.com/feedback. To contact the seller, please visit Amazon.com and click on 'Your Account' at the top of any page. In Your Account, go to the 'Orders' section and click on the link 'Leave seller feeback'. Select the order or click on the 'view Order' button. Click on the 'seller profile' under the appropriate product. On the lower right side of the page under 'seller Help', click on 'contact this seller'.</div><div style='page-break-after: always' align = 'center'>&nbsp;</div></body></html>";
                            break;
                        case "2":
                            ls_html += "<tr><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_ship.ToString() + "</td></tr>";
                            ls_html += "<tr><td class='bdleft'> </td><td> </td><td> </td><td class='bdside'>Tax:</td><td class='bdside' align='right'>" + ls_tax.ToString() + "</td></tr>";
                            ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_amz_total.ToString() + "</td></tr></table>";
                            ls_html += "<div width = '700'>Thank you for buying at Luxury Shop 4 Less @ Amazon Marketplace. To provide feeback for the seller please visit: www.amazon.com/feedback. To contact the seller, please visit Amazon.com and click on 'Your Account' at the top of any page. In Your Account, go to the 'Orders' section and click on the link 'Leave seller feeback'. Select the order or click on the 'view Order' button. Click on the 'seller profile' under the appropriate product. On the lower right side of the page under 'seller Help', click on 'contact this seller'.</div><div style='page-break-after: always' align = 'center'>&nbsp;</div></body></html>";
                            break;
                        case "3":
                            ls_html += "<tr><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_ship.ToString() + "</td></tr>";
                            ls_html += "<tr><td></td><td></td><td></td><td class='bdout'>US Tax:</td><td class = 'bdout' align='right'>" +ls_tax.ToString()+ "</td></tr>";
                            ls_html += "<tr><td class='bdbottom'>&nbsp </td><td class='bdbottom'>&nbsp</td><td class='bdbottom'>&nbsp</td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_ebay_wmbcom_total_price + "</td></tr></table><br>";
                            //ls_html += "<div>Thank you for buying at BestBeautyNet. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break: always'>&nbsp</div>";
                            ls_html += "<div>Thank you for buying at BestBeautyNet. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break-after: always'>&nbsp</div>";
                            break;
                        case "4":
                            ls_html += "<tr><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_ship.ToString() + "</td></tr>";
                            ls_html += "<tr><td></td><td></td><td></td><td class='bdout'>US Tax:</td><td class = 'bdout' align='right'>" + ls_tax.ToString() + "</td></tr>";
                            ls_html += "<tr><td class='bdbottom'>&nbsp </td><td class='bdbottom'>&nbsp</td><td class='bdbottom'>&nbsp</td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_ebay_wmbcom_total_price + "</td></tr></table><br>";
                            //ls_html += "<div>Thank you for buying at lisaperfumes. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break: always'>&nbsp</div>";
                            ls_html += "<div>Thank you for buying at lisaperfumes. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break-after: always'>&nbsp</div>";
                            break;
                        case "5":
                            ls_html += "<tr><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdtop'>&nbsp</td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_ship.ToString() + "</td></tr>";
                            ls_html += "<tr><td></td><td></td><td></td><td class='bdout'>US Tax:</td><td class = 'bdout' align='right'>" + ls_tax.ToString() + "</td></tr>";
                            ls_html += "<tr><td class='bdbottom'>&nbsp </td><td class='bdbottom'>&nbsp</td><td class='bdbottom'>&nbsp</td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_ebay_wmbcom_total_price + "</td></tr></table><br>";
                            //ls_html += "<div>Thank you for buying at WeMakeBeauty. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break: always'>&nbsp</div>";
                            ls_html += "<div>Thank you for buying at WeMakeBeauty. To provide feeback for the seller please visit: http://my.ebay.com. Login and click on Purchase History Tab on the left menu.  You will be able to see all your purchases.  Simply click on the Leave Feedback Action  to leave us feedback.  To contact the seller,  use the Contact Seller Instead.  Have  a wonderful day.</div><div style='page-break-after: always'>&nbsp</div>";
                            break;
                        case "6":
                            ls_html += "<tr><td class='bdtop' colspan='3'>" + ls_main_notes + "</td><td class='bdout'>Discount:</td><td class='bdout' align='right'>" + ls_main_sum_discount_forwmb + "</td></tr>";
                            ls_html += "<tr><td class='bdleft' colspan='3'> </td><td class='bdout'>Tax:</td><td class='bdout' align='right'>" + ls_us_tax_wmbcom.ToString() + "</td></tr>";
                            ls_html += "<tr><td class='bdleft' colspan='3'> </td><td class='bdout'>Gift Wrap:</td><td class='bdout' align='right'>" + ls_main_sum_giftwrap + "</td></tr>";
                            ls_html += "<tr><td class='bdleft' colspan='3'> </td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_sum_shipping_wmbcom.ToString() + "</td></tr>";
                            if (ls_ebay_wmbcom_total_price != ls_main_sum_total_forwmb.ToString())
                            {
                                float cj = float.Parse(ls_ebay_wmbcom_total_price) - ls_main_sum_total_forwmb;
                                ls_html += "<tr><td class='bdleft' colspan='3'> </td><td class='bdout'>Gift Certificates:</td><td class='bdout' align='right'>" + cj.ToString() + "</td></tr>";
                            }
                            ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_ebay_wmbcom_total_price + "</td></tr></table>";
                            //ls_html += "<div width = '700'>Thank you very much for shopping at WeMakeBeauty.com.  For any inquiry or question, please contact our customer service at Service@wemakebeauty.com.</div><div style='page-break-after: always' align = 'center'>&nbsp;</div></body></html>";
                            ls_html += "<div width = '700'>Thank you very much for shopping at WeMakeBeauty.com.  For any inquiry or question, please contact our customer service at Service@wemakebeauty.com.</div><div style='page-break-after: always' align = 'center'>&nbsp;</div>";
                            break;
                        case "7":
                            ls_html += @"</table></td></tr><tr><td colspan='3' height='35'></td></tr><tr><td colspan='3' align='right'><img alt='' src='file://d:\\Pictures\\" + ls_site_code + @"\\os.jpg' width='826'/></td></tr></table><div style='page-break-after: always' align = 'center'>&nbsp;</div>";
                            break;
                        case "8":
                            ls_html += "<tr><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_ship.ToString() + "</td></tr>";
                            //ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_order.ToString() + "</td></tr>";
                            ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" +(ls_ship+ ls_order).ToString() + "</td></tr></table>";
                            //ls_html += "<div width = '700'>Thank you very much for shopping at WeMakeBeauty @ Rakuten.com!</div><div style='page-break: always' align = 'center'>&nbsp;</div>";
                            ls_html += "<div width = '700'>Thank you very much for shopping at WeMakeBeauty @ Rakuten.com!</div><div style='page-break-after: always' align = 'center'>&nbsp;</div>";
                            break;
                        case "9":
                            ls_html += "<tr><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdtop'> </td><td class='bdout'>Shipping:</td><td class='bdout' align='right'>" + ls_ship.ToString() + "</td></tr>";
                            ls_html += "<tr><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdbottom'> </td><td class='bdout'>Total:</td><td class='bdout' align='right' >" + ls_ebay_wmbcom_total_price + "</td></tr></table>";
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

                   



                }
            }



            MessageBox.Show("good");
        }



        private Bitmap GetCode39(string strSource,string add_txt)
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
                txt_height = 15;
            }
            else
            {
                txt_height = 0;
            }

            Bitmap objBitmap = new Bitmap(
              ((WidLength * 3 + NarrowLength * 7) * (intSourceLength + 2)) + (x * 2),
              BarCodeHeight + (y * 2)+ txt_height);

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
                Font font = new Font("Tahoma", 10, FontStyle.Bold);
                Brush drawBrush = new SolidBrush(Color.Black);
                for (int i = 0; i < intSourceLength; i++)
                {
                    objGraphics.DrawString(strSource.Substring(i, 1), font, drawBrush, 14 * (i + 1), 23);
                }
            }
            

                return objBitmap;
        }

        private void wmb_Load(object sender, EventArgs e)
        {
            #region show site result
            //dataGridView1.AutoGenerateColumns = false;
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();
            string sqlstr = @"select x.FROMID,(select site_name from [t_base_from] where fromid=x.fromid) as site_name,x.order_num,x.item_num from (
select FROMID,count(distinct order_id) as order_num ,count(order_id) as item_num  from [t_biz_order] where Order_DAY=convert(varchar(8), getdate(), 112)
group by fromid) x";
            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            //dataGridView1.Columns[0].DataPropertyName = "Item";
            //dataGridView1.Columns[1].DataPropertyName = "Item_Name";
            //dataGridView1.Columns[2].DataPropertyName = "Barcode";
            //dataGridView1.Columns[3].DataPropertyName = "Weight";
            myConn.Close();
            #endregion
        }

        private void button4_Click(object sender, EventArgs e)
        {
            

            wms.MySql mw = new wms.MySql();
            mw.Show();

        }

        private void button5_Click(object sender, EventArgs e)
        {

            DataGroup group_amz2 = new DataGroup();
            group_amz2 = null;
            string ls_amz2_sql = @"
SELECT y.[fromid],y.[Order_DAY],y.[order-id],y.[payments-date],min(y.[item]) as item,y.[shipping_class],
sum(cast(y.[item_price] as float)) as [total_itemprice],
sum(cast(y.[item_tax] as float)) as [total_tax], 
sum(cast(y.[item_discount] as float))+sum(cast(y.[item_promotion_discount] as float)) as [total_discount],
sum(cast(y.[shipping_price] as float)) as [total_shipping],
sum(cast(y.[item_price] as float))+sum(cast(y.[item_tax] as float)) -( sum(cast(y.[item_discount] as float))+sum(cast(y.[item_promotion_discount] as float)))+sum(cast(y.[shipping_price] as float)) as [total_sum],
y.[buyer-name],y.[ship-phone-number],y.[recipient-name],y.[ship-address-1],y.[ship-address-2],y.[ship-city],y.[ship-state],y.[ship-postal-code],y.[ship-country]
from
(
select x.fromid,x.Order_DAY,x.[order-id],x.[payments-date],x.sys_sku,(select item from [t_amazon_2_full_sku_list] where [SKU(meaningless)]=x.sys_sku) as item,
x.shipping_class,x.item_price,x.item_discount,x.item_tax,x.shipping_price,x.item_promotion_discount,
x.[buyer-name],x.[ship-phone-number],x.[recipient-name],x.[ship-address-1],
x.[ship-address-2],x.[ship-city],x.[ship-state],x.[ship-postal-code],x.[ship-country]
from
(
select 
'2' as fromid,convert(varchar(8), getdate(), 112) as Order_DAY,a.[order-id],b.[payments-date],
case when substring(a.sku,1,2)='10' then  Substring(a.sku,3,len(a.sku)-2)
when substring(a.sku,1,2)='20' then  Substring(a.sku,3,len(a.sku)-2)
when substring(a.sku,1,3)='30' then  Substring(a.sku,3,len(a.sku)-2)
when substring(a.sku,1,4)='40' then  Substring(a.sku,3,len(a.sku)-2)
when substring(a.sku,1,5)='50' then  Substring(a.sku,3,len(a.sku)-2)
ELSE a.sku END
as sys_sku,
a.[ship-service-level] as shipping_class,
a.[item-price] as item_price,
a.[item-promotion-discount] as item_discount,
a.[item-tax] as item_tax,
a.[shipping-price] as shipping_price,
a.[ship-promotion-discount] as item_promotion_discount,
a.[buyer-name],a.[ship-phone-number],b.[recipient-name],b.[ship-address-1],
b.[ship-address-2],b.[ship-city],b.[ship-state],b.[ship-postal-code],b.[ship-country]
from t_imp_3 a ,t_imp_4 b where a.[order-id]=b.[order-id] and a.[order-item-id]=b.[order-item-id]
group by a.[order-id] ,b.[payments-date],a.sku,a.[ship-service-level],a.[item-price],a.[item-promotion-discount],a.[item-tax],a.[shipping-price],a.[ship-promotion-discount],
a.[buyer-name],a.[ship-phone-number],b.[recipient-name],b.[ship-address-1],
b.[ship-address-2],b.[ship-city],b.[ship-state],b.[ship-postal-code],b.[ship-country]
) as x
) y group by  y.[fromid],y.[Order_DAY],y.[order-id],y.[payments-date],y.[shipping_class],y.[buyer-name],y.[ship-phone-number],y.[recipient-name],y.[ship-address-1],y.[ship-address-2],y.[ship-city],y.[ship-state],y.[ship-postal-code],y.[ship-country]
order by item
";
            group_amz2 = DB.GetDataGroup(ls_amz2_sql);
            if (group_amz2.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_amz2.Table.Rows.Count; i++)
                {
                    string ls_fromid = group_amz2.Table.Rows[i]["fromid"].ToString();
                    string ls_Order_DAY = group_amz2.Table.Rows[i]["Order_DAY"].ToString();
                    string ls_order_id = group_amz2.Table.Rows[i]["order-id"].ToString();
                    string ls_payments_date = group_amz2.Table.Rows[i]["payments-date"].ToString();
                    string ls_shipping_class = group_amz2.Table.Rows[i]["shipping_class"].ToString();
                    string ls_total_itemprice = group_amz2.Table.Rows[i]["total_itemprice"].ToString();
                    string ls_total_tax = group_amz2.Table.Rows[i]["total_tax"].ToString();
                    string ls_total_discount = group_amz2.Table.Rows[i]["total_discount"].ToString();
                    string ls_total_shipping = group_amz2.Table.Rows[i]["total_shipping"].ToString();
                    string ls_total_sum = group_amz2.Table.Rows[i]["total_sum"].ToString();
                    string ls_buyer_name = group_amz2.Table.Rows[i]["buyer-name"].ToString().Replace("'","''");
                    string ls_phone = group_amz2.Table.Rows[i]["ship-phone-number"].ToString().Replace("'", "''");
                    string ls_name = group_amz2.Table.Rows[i]["recipient-name"].ToString().Replace("'", "''");
                    string ls_add1 = group_amz2.Table.Rows[i]["ship-address-1"].ToString().Replace("'", "''");
                    string ls_add2 = group_amz2.Table.Rows[i]["ship-address-2"].ToString().Replace("'", "''");
                    string ls_city = group_amz2.Table.Rows[i]["ship-city"].ToString().Replace("'", "''");
                    string ls_state = group_amz2.Table.Rows[i]["ship-state"].ToString().Replace("'", "''");
                    string ls_zip = group_amz2.Table.Rows[i]["ship-postal-code"].ToString().Replace("'", "''");
                    string ls_country = group_amz2.Table.Rows[i]["ship-country"].ToString().Replace("'", "''");
                    string ls_invoce = "";

                    //get invoice
                    DataGroup group_invoice = new DataGroup();
                    group_invoice = null;
                    group_invoice = DB.GetDataGroup("select max([INVOICE]) as INVOICE from [t_order_main] where substring([INVOICE],1,6)=substring(convert(varchar(8), getdate(), 112),3,6) ");
                    if (group_invoice.Table.Rows.Count > 0 && group_invoice.Table.Rows[0]["INVOICE"].ToString().Length>6)
                    {
                        ls_invoce = DateTime.Now.ToString("yyMMdd") + (int.Parse(group_invoice.Table.Rows[0]["INVOICE"].ToString().Substring(6, 4)) + 1).ToString("D4");
                    }
                    else
                    {
                        ls_invoce = DateTime.Now.ToString("yyMMdd") + "0001";
                    }
                    group_invoice = null;

                    //insert into order main
                    DataGroup group_is_order = new DataGroup();
                    group_is_order = null;
                    group_is_order = DB.GetDataGroup("select [order_id] from [t_order_main] where [order_id]='" + ls_order_id + "'");
                    if (group_is_order.Table.Rows.Count == 0)
                    {

                        string ls_ins_amz2_sql = @"INSERT INTO [t_order_main]
([INVOICE],[FROMID],[Order_DAY],[batch],[order_id]
,[payments_date],[shipping_class],[shipping_name],[total_itemprice],[total_shipping]
,[total_tax],[total_discount],[total_giftwrap],[total_Gift_Certificates],[total_sum]
,[Notes],[ebay_userid],[buy_name],[email],[phone]
,[name],[ship_to_company],[addr1],[addr2],[city]
,[state],[zip],[country],[Global_Shipping_Reference_ID],[TRACKING_NUMBER]
,[shipping_details],[status],[remark]) VALUES ('" + ls_invoce + "','" + ls_fromid + "','" + ls_Order_DAY + "','','" + ls_order_id + "','"
    + ls_payments_date + "','" + ls_shipping_class + "','" + ls_shipping_class + "','" + ls_total_itemprice + "','" + ls_total_shipping + "','"
    + ls_total_tax + "','" + ls_total_discount + "','0.00','0.00','" + ls_total_sum
    + "','','','" + ls_buyer_name + "','','" + ls_phone
    + "','" + ls_name + "','','" + ls_add1 + "','" + ls_add2 + "','" + ls_city + "','"
    + ls_state + "','" + ls_zip + "','" + ls_country + "','','','','1','') ";
                        DB.ExecuteSQL(ls_ins_amz2_sql);

                    }
                    else
                    {
                        DB.ExecuteSQL("update [t_order_main] set remark='Double' where [order_id]='" + ls_order_id + "'");
                    }

                    //insert into order detail
                    string ls_insert_detail = @"
select x.fromid,x.Order_DAY,x.[order-id],x.sys_sku,(select item from [t_amazon_2_full_sku_list] where [SKU(meaningless)]=x.sys_sku) as item,
x.[product-name],x.[quantity-purchased],
x.[item-price],x.[shipping-price],x.[item-promotion-discount],
x.[ship-promotion-discount]
from
(
select 
'2' as fromid,convert(varchar(8), getdate(), 112) as Order_DAY,a.[order-id],
case when substring(a.sku,1,2)='10' then  Substring(a.sku,3,len(a.sku)-2)
when substring(a.sku,1,2)='20' then  Substring(a.sku,3,len(a.sku)-2)
when substring(a.sku,1,3)='30' then  Substring(a.sku,3,len(a.sku)-2)
when substring(a.sku,1,4)='40' then  Substring(a.sku,3,len(a.sku)-2)
when substring(a.sku,1,5)='50' then  Substring(a.sku,3,len(a.sku)-2)
ELSE a.sku END
as sys_sku,a.[product-name],a.[quantity-purchased],
a.[item-price],a.[shipping-price],a.[ship-service-level],a.[item-promotion-discount],
a.[ship-promotion-discount]
from t_imp_3 a ,t_imp_4 b where a.[order-id]=b.[order-id] and a.[order-item-id]=b.[order-item-id]
) as x where x.[order-id]='"+ ls_order_id + "'";



                }
            }
            group_amz2 = null;

            MessageBox.Show("amz2 ok");

        }

        private void button6_Click(object sender, EventArgs e)
        {
            #region clear
            DB.ExecuteSQL(@"
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
truncate table [dbo].[t_imp_13];");
            #endregion
            string ls_load_file = "select fileid,table_name,file_type,file_dir,file_name,field_count,include_head,field_Separator,imp_check_file_time from [t_base_file] where file_type in ('txt','csv','xls') and status='1'";
            DataGroup group_files = new DataGroup();
            group_files = null;
            group_files = DB.GetDataGroup(ls_load_file);
            if (group_files.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_files.Table.Rows.Count; i++)
                {
                    string ls_file_type = group_files.Table.Rows[i]["file_type"].ToString();
                    string ls_file_FullName = group_files.Table.Rows[i]["file_dir"].ToString() + group_files.Table.Rows[i]["file_name"].ToString();
                    System.IO.FileInfo file = new System.IO.FileInfo(ls_file_FullName);
                    string ls_create_time = file.LastWriteTime.ToString("yyyyMMdd");
                    if (DateTime.Now.ToString("yyyyMMdd") == ls_create_time)
                    {
                        string ls_field_Separator = group_files.Table.Rows[i]["field_Separator"].ToString();
                        string ls_table_name = group_files.Table.Rows[i]["table_name"].ToString();
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
                                DB.ExecuteSQL(ls_txt_add);
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
                                string[] dog_csv = ls_csv_is_ect.Replace("\r\n", "|").Split('|');
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
                                            string ls_csv_11 = "";
                                            if (ls_table_name == "t_imp_8" || ls_table_name == "t_imp_11")
                                            {
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

                                            if (ls_table_name == "t_imp_8" || ls_table_name == "t_imp_11")
                                            {
                                                ls_csv_this_csv = " values ('" + ls_csv_bb.Replace("'", "''").Replace(ls_field_Separator, "','").Replace("\"", "") + "');";
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
                                DB.ExecuteSQL(ls_csv_add);
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
                                            DB.ExecuteSQL(ls_insert);
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
                                            DB.ExecuteSQL(ls_insert);
                                        }
                                    }
                                }
                                #endregion
                                break;
                        }
                    }
                }


            }


            #region set data full
            DataGroup group_access = new DataGroup();
            group_access = null;
            group_access = DB.GetDataGroup("select [access_dir],[access_table],[sql_table],[insert_f],[select_f] from [t_base_access] where status=1;");
            if (group_access.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_access.Table.Rows.Count; i++)
                {
                    string ls_access_dir = group_access.Table.Rows[i]["access_dir"].ToString();
                    string ls_access_table = group_access.Table.Rows[i]["access_table"].ToString();
                    string ls_select_f = group_access.Table.Rows[i]["select_f"].ToString();
                    string ls_sql_table = group_access.Table.Rows[i]["sql_table"].ToString();
                    DB.ExecuteSQL("truncate table " + ls_sql_table + ";");
                    //change cpu x86
                    OleDbConnection objConn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ls_access_dir + ";");
                    OleDbCommand MDBCommand = new OleDbCommand("Select " + ls_select_f + " FROM" + ls_access_table, objConn);
                    OleDbDataReader rdr2;
                    objConn.Open();
                    rdr2 = MDBCommand.ExecuteReader();
                    SqlBulkCopy sbc = new SqlBulkCopy(ConfigurationManager.AppSettings["dbConnectionString"]);
                    sbc.DestinationTableName = ls_sql_table;
                    sbc.WriteToServer(rdr2);
                    sbc.Close();
                    rdr2.Close();
                    objConn.Close();
                }
            }
            group_access = null;


            //split over stock addr
            string ls_ov_sql = "select oid,shipping_details from [t_biz_order] where  fromid='7' and name='' ";
            DataGroup group_ov_addr = new DataGroup();
            group_ov_addr = null;
            group_ov_addr = DB.GetDataGroup(ls_ov_sql);
            if (group_ov_addr.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_ov_addr.Table.Rows.Count; i++)
                {
                    string ls_oid = group_ov_addr.Table.Rows[i]["oid"].ToString();
                    string ls_addr = group_ov_addr.Table.Rows[i]["shipping_details"].ToString();
                    string[] dog_small = ls_addr.Replace("        ", "|").Split('|');
                    DB.ExecuteSQL("update [t_biz_order] set name='" + dog_small[0].ToString() + "',addr1='" + dog_small[1].ToString() + "',addr2='" + dog_small[2].ToString() + "',city='" + dog_small[3].ToString() + "',zip='" + dog_small[4].ToString() + "' where  fromid='7' and name='' and oid='" + ls_oid + "' ");
                }
            }
            #endregion


            #region all import data insert into order table
            DB.ExecuteSQL(@"
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
from t_imp_1 a ,t_imp_2 b where a.[order-id]=b.[order-id] and a.[order-item-id]=b.[order-item-id];


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
from t_imp_3 a ,t_imp_4 b where a.[order-id]=b.[order-id] and a.[order-item-id]=b.[order-item-id];



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
FROM [t_imp_5] where [item number] !=''  and [Paid on Date]!=''
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
FROM [t_imp_5]  where [item number] !='' and [Paid on Date]='';



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
FROM [t_imp_6] where [item number] !=''  and [Paid on Date]!=''
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
FROM [t_imp_6]  where [item number] !='' and [Paid on Date]='';


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
FROM [t_imp_7] where [item number] !=''  and [Paid on Date]!=''
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
FROM [t_imp_7]  where [item number] !='' and [Paid on Date]='';


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
total_payment_received as [total_sum], a.[ordernotes] as [Notes], '' as [ebay_userid], billingfirstname+' '+billinglastname as [buy_name], 
'' as [email], a.[shipphonenumber] as [phone], a.[shipfirstname]+' '+a.[shiplastname] as [name], a.[shipcompanyname] as [ship_to_company], a.[shipaddress1] as [addr1], 
a.[shipaddress2] as [addr2], a.[shipcity] as [city], a.[shipstate] as [state], a.[shippostalcode] as [zip], a.[shipcountry] as [country], 
b.[productcode] as [sku], b.[productcode] as [sys_sku], case when substring(b.[productcode],1,3)='11-' then  Substring(b.[productcode],4,len(b.[productcode])-3) ELSE b.[productcode] END as [item], b.[productname] as [product_name], b.[quantity] as [qty], 
b.[productprice] as [item_price], a.[totalshippingcost] as [item_shipping_price], '0.0' as [item_shipping_tax], '0.0' as [item_shipping_discount], cast(salestax1 as float)+cast(salestax2 as float)+cast(salestax3 as float) as [item_us_tax], 
'0.0' as [item_discount], b.[giftwrap] as [item_giftwrap], '1' as [status], '' as [remark]
from t_imp_8 a,
(select orderid,productcode,productname,productprice,giftwrap,sum(cast(quantity as float)) as quantity from t_imp_9
group by orderid,productcode,productname,productprice,giftwrap) as b
where a.[orderid]=b.[orderid];
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
FROM [t_imp_11];



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
FROM [t_imp_13] where  [Order Status]='Unshipped';

");

            #endregion

            //update amz item data
            DB.ExecuteSQL(@"
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
");

            #region show site result
            //dataGridView1.AutoGenerateColumns = false;
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();
            string sqlstr = @"select x.FROMID,(select site_name from [t_base_from] where fromid=x.fromid) as site_name,x.order_num,x.item_num from (
select FROMID,count(distinct order_id) as order_num ,count(order_id) as item_num  from [t_biz_order] where Order_DAY=convert(varchar(8), getdate(), 112)
group by fromid) x";
            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            //dataGridView1.Columns[0].DataPropertyName = "Item";
            //dataGridView1.Columns[1].DataPropertyName = "Item_Name";
            //dataGridView1.Columns[2].DataPropertyName = "Barcode";
            //dataGridView1.Columns[3].DataPropertyName = "Weight";
            myConn.Close();
            #endregion



        }

        private void button7_Click(object sender, EventArgs e)
        {
            #region create invoice head
            string ls_dir = "d:\\" + DateTime.Now.ToString("yyyy-MM") + "\\" + DateTime.Now.ToString("yyyy-MM-dd");
            string ls_style_head = "";
            DataGroup group_file = new DataGroup();
            group_file = null;
            group_file = DB.GetDataGroup("select fromid,(select site_code from t_base_from where fromid=[t_biz_order].FROMID) as site_code from [t_biz_order]  where status=1  and order_day=convert(varchar(8), getdate(), 112) group by fromid ;");
            if (group_file.Table.Rows.Count > 0)
            {
                for (int i_file = 0; i_file < group_file.Table.Rows.Count; i_file++)
                {
                    string ls_fromid = group_file.Table.Rows[i_file]["fromid"].ToString();
                    string ls_site_code = group_file.Table.Rows[i_file]["site_code"].ToString();

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
                            ls_style_head = "<style type='text/css'>table{border-collapse:collapse;-webkit-print-color-adjust: exact;}.tb_item table, .tb_item th, .tb_item td {border: 1px solid black;}"+
                                ".ft1{font-size:24px;font-family:Helvetica;color:#000000;font-weight:bold;}"+
                                ".ft2{font-size:16px;font-family:Helvetica;color:#000000;padding-left:4px;}"+
                                "p{ font-size:16px;font-family:Helvetica;color:#000000;margin:0 auto;padding-left:4px}body {  MARGIN: 0px;PADDING: 0px;}</style>";
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
            }
            group_file = null;
            #endregion


            DataGroup group_create_invoice = new DataGroup();
            group_create_invoice = null;
            group_create_invoice = DB.GetDataGroup(@"
select fromid,[order_id],min(item) as ordersku
from [t_biz_order]  where status=1  and order_day=convert(varchar(8), getdate(), 112) group by fromid,[order_id]
order by fromid,ordersku
");
            if (group_create_invoice.Table.Rows.Count > 0)
            {
                for (int i_create_invoice = 0; i_create_invoice < group_create_invoice.Table.Rows.Count; i_create_invoice++)
                {
                    string ls_create_invoice_id_main = group_create_invoice.Table.Rows[i_create_invoice]["order_id"].ToString();

                    //get invoice
                    string ls_invoce = "";
                    DataGroup group_iv = new DataGroup();
                    group_iv = null;
                    group_iv = DB.GetDataGroup("select max([INVOICE]) as INVOICE from [t_biz_order] where substring([INVOICE],1,6)=substring(convert(varchar(8), getdate(), 112),3,6) ");
                    if (group_iv.Table.Rows.Count > 0 && group_iv.Table.Rows[0]["INVOICE"].ToString().Length > 6)
                    {
                        ls_invoce = DateTime.Now.ToString("yyMMdd") + (int.Parse(group_iv.Table.Rows[0]["INVOICE"].ToString().Substring(6, 4)) + 1).ToString("D4");
                    }
                    else
                    {
                        ls_invoce = DateTime.Now.ToString("yyMMdd") + "0001";
                    }
                    group_iv = null;
                    DB.ExecuteSQL("update t_biz_order set invoice='" + ls_invoce + "' where [order_id]='" + ls_create_invoice_id_main + "' and invoice is null");
                }
            }


            DataGroup group_order = new DataGroup();
            group_order = null;
            group_order = DB.GetDataGroup(@"
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
from[t_biz_order]  where status=1  and order_day=convert(varchar(8), getdate(), 112) group by fromid,[order_id]
order by fromid,bar_code
");
            if (group_order.Table.Rows.Count > 0)
            {
                for (int i_order = 0; i_order < group_order.Table.Rows.Count; i_order++)
                {
                    #region get invoice main value
                    string ls_dir_bc = ls_dir + "\\barcode\\";
                    DirectoryInfo d_dir = new DirectoryInfo(ls_dir_bc);
                    if (!d_dir.Exists)
                    {
                        d_dir.Create();
                    }



                    string ls_fromid = group_order.Table.Rows[i_order]["fromid"].ToString();
                    string ls_bc_flag = "N";
                    if (ls_fromid == "3" || ls_fromid == "4" || ls_fromid == "5" || ls_fromid == "7")
                    {
                        ls_bc_flag = "Y";
                    }
                    string ls_site_code = group_order.Table.Rows[i_order]["site_code"].ToString();
                    string ls_order_id_main = group_order.Table.Rows[i_order]["order_id"].ToString();

                    string ls_bar_code = group_order.Table.Rows[i_order]["bar_code"].ToString();
                    Bitmap oBmp = GetCode39(ls_bar_code, ls_bc_flag);
                    oBmp.Save(ls_dir_bc + ls_bar_code + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    oBmp.Dispose();

                    string ls_nb = group_order.Table.Rows[i_order]["nb"].ToString();
                    string ls_main_notes = group_order.Table.Rows[i_order]["Notes"].ToString();
                    string ls_main_total_sum = "$"+group_order.Table.Rows[i_order]["total_sum"].ToString();
                    string ls_main_total_tax = "" + group_order.Table.Rows[i_order]["total_tax"].ToString();
                    string ls_main_total_shipping = "" + group_order.Table.Rows[i_order]["total_shipping"].ToString();
                    string ls_main_total_discount = "" + group_order.Table.Rows[i_order]["total_discount"].ToString();
                    string ls_main_total_giftwrap = "" + group_order.Table.Rows[i_order]["total_giftwrap"].ToString();
                    string ls_main_total_Gift_Certificates = "" + (float.Parse(group_order.Table.Rows[i_order]["total_sum"].ToString()) - float.Parse(group_order.Table.Rows[i_order]["total_Gift_Certificates"].ToString())).ToString();

                    string ls_html = "";
                    #endregion
                    DataGroup group_invoice = new DataGroup();
                    group_invoice = null;
                    group_invoice = DB.GetDataGroup("select oid,invoice,name,buy_name,ebay_userid,Global_Shipping_Reference_ID,addr1,addr2,city,state,zip,country,[payments_date],[order_id],[shipping_class], shipping_name,[qty],[sku],[product_name],[item],[item_price],Notes from [t_biz_order] where status=1 and order_day=convert(varchar(8), getdate(), 112) " +
                        "and order_id='" + ls_order_id_main + "' order by item;");
                    if (group_invoice.Table.Rows.Count > 0)
                    {
                        for (int i = 0; i < group_invoice.Table.Rows.Count; i++)
                        {
                            #region get item value

                            //string ls_oid = group_invoice.Table.Rows[i]["oid"].ToString();
                            string ls_invoice = group_invoice.Table.Rows[i]["invoice"].ToString();
                            string ls_name = group_invoice.Table.Rows[i]["name"].ToString();
                            string ls_buy_name = group_invoice.Table.Rows[i]["buy_name"].ToString();
                            string ls_ebay_userid = group_invoice.Table.Rows[i]["ebay_userid"].ToString();
                            string ls_Global_Shipping_Reference_ID = group_invoice.Table.Rows[i]["Global_Shipping_Reference_ID"].ToString();
                            string ls_addr1 = group_invoice.Table.Rows[i]["addr1"].ToString();
                            string ls_addr2 = group_invoice.Table.Rows[i]["addr2"].ToString();
                            string ls_city = group_invoice.Table.Rows[i]["city"].ToString();
                            string ls_state = group_invoice.Table.Rows[i]["state"].ToString();
                            string ls_zip = group_invoice.Table.Rows[i]["zip"].ToString();
                            string ls_country = group_invoice.Table.Rows[i]["country"].ToString();
                            string ls_payments_date = group_invoice.Table.Rows[i]["payments_date"].ToString();
                            string ls_payments_date_shot = DateTime.Parse(ls_payments_date).ToString("MM/dd/yyyy");
                            string ls_order_id = group_invoice.Table.Rows[i]["order_id"].ToString();
                            //string ls_shipping_class = group_invoice.Table.Rows[i]["shipping_class"].ToString();
                            string ls_shipping_name = group_invoice.Table.Rows[i]["shipping_name"].ToString();
                            string ls_qty = group_invoice.Table.Rows[i]["qty"].ToString();
                            //string ls_sku = group_invoice.Table.Rows[i]["sku"].ToString();
                            string ls_product_name = group_invoice.Table.Rows[i]["product_name"].ToString();
                            string ls_item = group_invoice.Table.Rows[i]["item"].ToString();
                            string ls_Notes = group_invoice.Table.Rows[i]["Notes"].ToString();
                            string ls_item_price = group_invoice.Table.Rows[i]["item_price"].ToString().Replace("$", "");
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
                                        ls_html = "<body><div align='center' style='width:720px'><img src='file://d:\\Pictures\\" + ls_site_code + "\\la_Secret.jpg' width='248' /><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></div><br><div align='right'></div>";

                                        if (ls_shipping_name != "Standard")
                                        {
                                            ls_html += "<div align='left'><img src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/></div>";
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
                                        ls_html = "<body><div align='center' style='width:720px'><img src='file://d:\\Pictures\\" + ls_site_code + "\\amazonLogo.jpg' width='248' /><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></div><br><div align='right'></div>";

                                        if (ls_shipping_name != "Standard")
                                        {
                                            ls_html += "<div align='left'><img src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/></div>";
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
                                        ls_html = "<body><table><tr><td align='right' width='350'><img src='file://d:\\Pictures\\" + ls_site_code + "\\Ebaylogo.jpg' width='290' height='58'/></td><td align='right'><img src='file://d:\\Pictures\\" + ls_site_code + "\\logoEbay_x45.gif' height ='45'></td></tr><tr><td valign='bottom' align='left'>";

                                        if (ls_shipping_name.IndexOf("Standard") != -1 && ls_shipping_name.IndexOf("First Class") != -1)
                                        {
                                            ls_html += "<img src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
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
                                        ls_html = "<body><table><tr><td align='right' width='350'><img src='file://d:\\Pictures\\" + ls_site_code + "\\logo.jpg' width='248' height='70'/></td><td align='right'><img src='file://d:\\Pictures\\" + ls_site_code + "\\logoEbay_x45.gif' height ='45'></td></tr><tr><td valign='bottom' align='left'>";

                                        if (ls_shipping_name.IndexOf("Standard") != -1 && ls_shipping_name.IndexOf("First Class") != -1)
                                        {
                                            ls_html += "<img src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
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
                                        ls_html = "<body><table><tr><td align='right' width='350'><img src='file://d:\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/></td><td align='right'><img src='file://d:\\Pictures\\" + ls_site_code + "\\logoEbay_x45.gif' height ='45'></td></tr><tr><td valign='bottom' align='left'>";
                                        if (ls_shipping_name.IndexOf("Standard") != -1 && ls_shipping_name.IndexOf("First Class") != -1)
                                        {
                                            ls_html += "<img src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
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
                                        ls_html = "<div align='center'><img src='file://d:\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/></div><br><div align='right'>";

                                        if (ls_shipping_name.IndexOf("Standard Ground (U.S. Domestic)") != -1 && ls_shipping_name.IndexOf("Free Standard Shipping (U.S. Domestic)") != -1)
                                        {
                                            ls_html += "<img style='float:left' src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
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
                                    if (i == 0)
                                    {
                                        ls_html = "<table cellpadding='0' cellspacing='0' width='919px'>" +
                                        "<tr><td colspan='3' ><img src='file://d:\\Pictures\\" + ls_site_code + "\\logo.png'  width='300'/><div align='right'><img src='file://" + ls_dir_bc + ls_bar_code + ".jpg' /></div></td></tr>" +
                                        "<tr><td colspan='3' height='42'></td></tr><tr><td colspan='3' align='right' height='17'>" +
                                        "<table width='826'><tr align='left'><td height='22' width='365'><span class='ft1'>Ship To:</span></td><td width='365'><span class='ft1'>Send To:</span></td></tr>" +
                                        "<tr height='66' align='left' style='vertical-align:text-top;'>";

                                        ls_html += "<td><p>" + ls_name + "</p></td>";
                                        ls_html += "<td><p>" + ls_addr1 + "<br/>" + ls_addr2 + "<br/>" + ls_city + ls_zip + "</p></td>";

                                        ls_html += @"</tr></table></td></tr><tr><td colspan='3' height='22'></td></tr>
<tr height='41'><td colspan='3' align='right'><table width='826' class='tb_item'>
<tr align='left' bgcolor='#e6e6e6'><td height='22' widsth='25%'>
<span class='ft2'>Order Date</span></td><td width='25%'>
<span class='ft2'>Order Number</span></td><td width='25%'>
<span class='ft2'>Ship Via</span></td><td width='25%'>
<span class='ft2'>Ship Method</span></td></tr>
<tr height='22' align='left'>";

                                        ls_html += "<td><p>" + ls_payments_date_shot + "</p></td>";
                                        ls_html += "<td><p>" + ls_order_id + "</p></td>";
                                        ls_html += "<td><p>BEST</p></td>";
                                        ls_html += "<td><p>" + ls_shipping_name + "</p></td>";

                                        ls_html += @"</tr></table></td></tr><tr><td colspan='3' height='22'></td></tr><tr><td colspan='3' align='right' >
<table width='826' class ='tb_item'><tr align='left' bgcolor='#e6e6e6'><td height='22' width='126'>
<span class='ft2'>Quantity Ordered</span></td><td width='148'><span class='ft2'>Item Number</span></td>
<td width='185'><span class='ft2'>Description</span></td><td width='126'><span class='ft2'>Quantity Shipped</span></td><td width='163'>
<span class='ft2'>Vendor Sku</span></td><td width='78'><span class='ft2'>Price</span></td></tr>";
                                    }

                                    ls_html += "<tr height='55' align='left'><td ><p>" + ls_qty + "</p></td>";
                                    ls_html += "<td ><p>" + ls_item + "</p></td>";
                                    ls_html += "<td ><p>" + ls_product_name + "</p></td>";
                                    ls_html += "<td ><p>" + ls_qty + "</p></td>";
                                    ls_html += "<td ><p>" + ls_item + "</p></td>";
                                    ls_html += "<td ><p>" + ls_amount + "</p></td></tr>";
                                    break;
                                #endregion
                                case "8":
                                    #region create invoice buy body
                                    if (i == 0)
                                    {
                                        ls_html = "<table><tr><td width='180'></td><td align = 'center'><img src='file://d:\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/></td><td width='180' align = 'right' valign = 'top'><img src='file://d:\\Pictures\\" + ls_site_code + "\\Logo_Rekuton.jpg' width='180' height='40'/></td></tr></table><br><div align='right'><blockquote valign='bottom'>";

                                        if (ls_shipping_name != "Standard")
                                        {
                                            ls_html += "<img style='float:left' src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/>";
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
                                        ls_html = "<div align='center'><img src='file://d:\\Pictures\\" + ls_site_code + "\\toplogo.jpg' width='248' height='70'/><img style='float:right' src='file://d:\\Pictures\\" + ls_site_code + "\\neLogo.png' height='70px'/></div>";
                                        if (ls_shipping_name.IndexOf("Standard") == -1)
                                        {
                                            ls_html += "<div><img style='float:left' src='file://d:\\Pictures\\" + ls_site_code + "\\fast shipping.png' height = '50'/></div>";
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
                    }


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
                            ls_html += @"</table></td></tr><tr><td colspan='3' height='35'></td></tr><tr><td colspan='3' align='right'><img alt='' src='file://d:\\Pictures\\" + ls_site_code + @"\\os.jpg' width='826'/></td></tr></table><div style='page-break-after: always' align = 'center'>&nbsp;</div>";
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





                }
            }



            MessageBox.Show("good");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string ls_field_Separator = "	";
            string ls_txt_left = "";
            string ls_txt_add = "";
            string ls_tr_dir = "Y:\\Endicia Export\\"+DateTime.Now.ToString("yyMMdd")+".TXT";//yyMMdd.txt
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
            DB.ExecuteSQL(ls_txt_add);
            ls_txt_add = "";




            MessageBox.Show("ok");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            
        }

        private void invoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fBAShippingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wms.fba_query fba = new wms.fba_query();
            fba.Show();
        }

        private void fBASalesListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wms.fba_shipping_sales fba_shipping_s = new wms.fba_shipping_sales();
            fba_shipping_s.Show();
        }

        private void productBarCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
