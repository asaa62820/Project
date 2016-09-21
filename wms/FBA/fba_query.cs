using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using TX.Framework.WindowUI.Forms;

namespace wms
{
    public partial class fba_query : BaseForm
    {
        public fba_query()
        {
            InitializeComponent();
        }


        private void fba_query_Load(object sender, EventArgs e)
        {
            // Y:\New Amazon Home\FBA Sales\

            string ls_field_Separator = "	";
            string ls_txt_left = "";
            string ls_txt_add = "";
            string ls_local = "Y:\\New Amazon Home\\FBA Sales\\";
            string ls_local2 = "Y:\\New Amazon Home\\FBA Sales\\History\\";
            DirectoryInfo d_dir = new DirectoryInfo(ls_local2);
            if (!d_dir.Exists)
            {
                d_dir.Create();
            }


            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();


            DirectoryInfo ls_log_files_dir = new DirectoryInfo(ls_local);
            FileInfo[] files = ls_log_files_dir.GetFiles();
            foreach (FileInfo file in files)
            {

                StreamReader fs_is_ect = new StreamReader(file.FullName, System.Text.Encoding.UTF8);
                string ls_is_ect = fs_is_ect.ReadToEnd();
                fs_is_ect.Close();
                string[] dog_small = ls_is_ect.Replace("\n", "|").Split('|');
                int cc = 0;
                foreach (string bb in dog_small)
                {
                    string ls_txt_bb = bb.Trim();
                    if (ls_txt_bb.Length > 10)
                    {

                        if (cc == 0)
                        {
                            ls_txt_left = "insert into t_amazon_fba_sale_list ([" + ls_txt_bb.Replace("|", "").Replace(ls_field_Separator, "|") + "])";
                        }
                        else
                        {
                            string[] dog_this_csv = ls_txt_bb.Replace(ls_field_Separator, "|").Split('|');
                            if (dog_this_csv[5] == "Amazon")
                            {
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
                        }
                        cc++;
                    }
                }

                if (ls_txt_add.Length>1)
                { 
                    OleDbCommand cmd = new OleDbCommand(ls_txt_add, myConn);
                    cmd.ExecuteNonQuery();
                }


                ls_txt_add = "";
                File.Move(ls_local+file.Name, ls_local2 + file.Name);
            }

            //ls_txt_add="delete t_amazon_fba_sale_list where id not in (select max(id) from t_amazon_fba_sale_list group by [amazon-order-id],[sku],[purchase-date],[order-status],[item-status]);";
            ls_txt_add = "delete t_amazon_fba_sale_list where id not in (select max(id) from t_amazon_fba_sale_list group by [amazon-order-id],[sku],[purchase-date]);";
            OleDbCommand cmd2 = new OleDbCommand(ls_txt_add, myConn);
            cmd2.ExecuteNonQuery();


            ls_txt_add="update t_amazon_fba_sale_list set item=(select item from t_amazon_home_fba_sku_list where fbasku=t_amazon_fba_sale_list.sku) where item is null;";
            cmd2 = new OleDbCommand(ls_txt_add, myConn);
            cmd2.ExecuteNonQuery();

            ls_txt_add="update t_amazon_fba_sale_list set unit_price=(select FbaPrice from t_amazon_home_fba_sku_list where fbasku=t_amazon_fba_sale_list.sku) where unit_price is null;";
            cmd2 = new OleDbCommand(ls_txt_add, myConn);
            cmd2.ExecuteNonQuery();

            myConn.Close();

            int year = DateTime.Now.Year;  //当前年
            int month = DateTime.Now.Month;//当前月
            int day = DateTime.Now.Day;    //当天
            int day2 = DateTime.Now.AddDays(-7).Day;

            dateTimePicker1.Value = new DateTime(year, month, day2);
            dateTimePicker2.Value = new DateTime(year, month, day);

            //dtp_Begin.Value = new DateTime(year, month, 1);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();

            string sqlstr = "";
            if (textBox1.Text.Length > 0)
            {
                sqlstr += " and [product-name] like '%" + textBox1.Text.Trim() + "%'  ";
            }

            if (textBox2.Text.Length > 0)
            {
                sqlstr += " and [sku] like '" + textBox2.Text.Trim() + "%'  ";
            }

            if (textBox3.Text.Length > 0)
            {
                sqlstr += " and [item] like '" + textBox3.Text.Trim() + "%'  ";
            }


            string ls_s = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string ls_e = dateTimePicker2.Value.ToString("yyyy-MM-dd");


            sqlstr += " and cast( [purchase-date] as date) >= cast('"+ ls_s + "' as date) ";
            sqlstr += " and cast( [purchase-date] as date) <= cast('" + ls_e + "' as date) ";

            string ls_c1 = "";
            string ls_c2 = "";
            string ls_c3 = "";
            string ls_c4 = "";
            string ls_c5 = "";
            string ls_c6 = "";
            if (checkBox1.Checked == true)
            {
                ls_c1 = "Shipped";
            }

            if (checkBox2.Checked == true)
            {
                ls_c2 = "Pending";
            }
            if (checkBox3.Checked == true)
            {
                ls_c3 = "Cancelled";
            }

            sqlstr += " and [order-status] in ('" + ls_c1 + "','" + ls_c2 + "','" + ls_c3 + "')  ";



            if (checkBox4.Checked == true)
            {
                ls_c4 = "Cancelled";
            }

            if (checkBox5.Checked == true)
            {
                ls_c5 = "Unshipped";
            }
            if (checkBox6.Checked == true)
            {
                ls_c6 = "Shipped";
            }

            sqlstr += " and [item-status] in ('" + ls_c4 + "','" + ls_c5 + "','" + ls_c6 + "')  ";


            string ls_sql = "select item,unit_price,[product-name] as product_name,[sku],[order-status] as order_status,[item-status] as item_status,sum(cast([quantity] as float)) as QTY,round(sum(cast([item-price] as float )),2) as Unit_Pirce,round(sum(cast([item-price] as float )),2) as Amount,count([amazon-order-id]) as Total_Orders from t_amazon_fba_sale_list " +
                "where [fulfillment-channel]='Amazon'  "+ sqlstr+
                "group by item,unit_price,[product-name],[sku],[order-status],[item-status]";

            OleDbDataAdapter adapter = new OleDbDataAdapter(ls_sql, myConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].DataPropertyName = "item"; 
            dataGridView1.Columns[1].DataPropertyName = "product_name";
            dataGridView1.Columns[2].DataPropertyName = "sku";
            dataGridView1.Columns[3].DataPropertyName = "order_status";
            dataGridView1.Columns[4].DataPropertyName = "item_status";
            dataGridView1.Columns[5].DataPropertyName = "QTY";
            dataGridView1.Columns[6].DataPropertyName = "unit_price";
            dataGridView1.Columns[7].DataPropertyName = "Amount";
            dataGridView1.Columns[8].DataPropertyName = "Total_Orders";

            myConn.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
