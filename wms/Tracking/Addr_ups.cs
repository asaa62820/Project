using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using TX.Framework.WindowUI.Forms;

namespace wms
{
    public partial class tracking_ups : BaseForm
    {
        public tracking_ups()
        {
            InitializeComponent();
        }

        //dbaccess.DbHelperACE DB = new dbaccess.DbHelperACE; 

        //dbaccess DB = new dbaccess();

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";

            string ls_textBox1 = textBox1.Text;
            string[] sArray = Regex.Split(ls_textBox1, "\r\n", RegexOptions.IgnoreCase);
            foreach (string i in sArray)
            {
                string ls_isin = "select [Order#] from [Address List] where [Order#]='" + i.ToString().Trim() + "'";

                object obj3 = dbaccess.DbHelperACE.GetSingle(ls_isin);

                if (Object.Equals(obj3, null))
                {
                    textBox2.Text += "   " + i.ToString().Trim() + "\r\n";
                }
                else
                {
                    if (obj3.ToString() == i.ToString().Trim())
                    {
                        textBox2.Text += i.ToString().Trim() + "\r\n";
                    }
                    else
                    {
                        textBox2.Text += "   " + i.ToString().Trim() + "\r\n";
                    }
                }
            }



           

            //ds=dbaccess.DbHelperACE.Query(ls_un_number);
            //int ls_mid = 0;
            //ls_mid = dbaccess.DbHelperACE.GetMaxID(" [Address List].[Order#]", "[Address List]");

            set_label();



            string ls_s = textBox2.Text.Trim().Replace("\r\n\r\n", "\r\n").Replace("\r\n \r\n", "\r\n").Replace("\r\n  \r\n", "\r\n").Replace("   \r\n", "\r\n").Replace("\r\n   ", "\r\n");
            string[] sArray2 = Regex.Split(ls_s, "\r\n", RegexOptions.IgnoreCase);
            //label3.Text = sArray.Length.ToString();


            int ls_count = 0;
            foreach (string i in sArray2)
            {
                if (i.Trim().Length > 0)
                {
                    ls_count++;
                }
            }
            label3.Text = ls_count.ToString();




        }

        private void Form1_Load(object sender, EventArgs e)
        {
            set_label();
        }

        private void set_label()
        {
            string ls_un_number = "SELECT Count([Order#]) FROM [Address List] WHERE [Address List].printed=0; ";
            string ls_all_number = "SELECT Count([Order#]) FROM [Address List]; ";
            object obj1 = dbaccess.DbHelperACE.GetSingle(ls_un_number);
            object obj2 = dbaccess.DbHelperACE.GetSingle(ls_all_number);
            label1.Text = "Unprinted: " + obj1.ToString() + " / " + obj2.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            string ls_name = "c:\\ups\\Labels.csv";
            using (FileStream fs = File.Open(@ls_name, FileMode.Create))
            {
                byte[] b = System.Text.Encoding.Default.GetBytes("");
                fs.Write(b, 0, b.Length);
                fs.Close();
            }

            //"615203","002-0066182-4139417",,"Catherine Fernando","2976 SANTOS LN APT 301",,,"WALNUT CREEK","CA","94597-7500","United States",

            string ls_textBox2 = textBox2.Text;
            string[] sArray = Regex.Split(ls_textBox2, "\r\n", RegexOptions.IgnoreCase);
            foreach (string i in sArray)
            {
                if (i.ToString().Length > 0)
                {
                    string ls_get_sql = "SELECT [Order#], [Today's Invoice_order-id], [Global Shipping Reference ID], [recipient-name], [ship-address-1], [ship-address-2], [ship-address-3], [ship-city], [ship-state], [ship-zip], [ship-country], phone " +
                    " FROM [Address List] WHERE ([Address List].addDate)=Date() and  [Order#]='" + i.ToString().Trim() + "'";

                    DataSet ds = new DataSet();
                    ds = dbaccess.DbHelperACE.Query(ls_get_sql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        string ls_1 = ds.Tables[0].Rows[0]["Order#"].ToString();
                        string ls_2 = ds.Tables[0].Rows[0]["Today's Invoice_order-id"].ToString();
                        string ls_3 = ds.Tables[0].Rows[0]["Global Shipping Reference ID"].ToString();
                        string ls_4 = ds.Tables[0].Rows[0]["recipient-name"].ToString();
                        string ls_5 = ds.Tables[0].Rows[0]["ship-address-1"].ToString();
                        string ls_6 = ds.Tables[0].Rows[0]["ship-address-2"].ToString();
                        string ls_7 = ds.Tables[0].Rows[0]["ship-address-3"].ToString();
                        string ls_8 = ds.Tables[0].Rows[0]["ship-city"].ToString();
                        string ls_9 = ds.Tables[0].Rows[0]["ship-state"].ToString();
                        string ls_10 = ds.Tables[0].Rows[0]["ship-zip"].ToString();
                        string ls_11 = ds.Tables[0].Rows[0]["ship-country"].ToString();
                        string ls_12 = ds.Tables[0].Rows[0]["phone"].ToString();


                        string ls_ddd = "\"" + ls_1 + "\",";
                        ls_ddd += "\"" + ls_2 + "\",";
                        ls_ddd += "\"" + ls_3 + "\",";
                        ls_ddd += "\"" + ls_4 + "\",";
                        ls_ddd += "\"" + ls_5 + "\",";
                        ls_ddd += "\"" + ls_6 + "\",";
                        ls_ddd += "\"" + ls_7 + "\",";
                        ls_ddd += "\"" + ls_8 + "\",";
                        ls_ddd += "\"" + ls_9 + "\",";
                        ls_ddd += "\"" + ls_10 + "\",";
                        ls_ddd += "\"" + ls_11 + "\",";
                        ls_ddd += "\"" + ls_12 + "\"\r\n";

                        using (FileStream fs = File.Open(@ls_name, FileMode.Append))
                        {
                            byte[] b = System.Text.Encoding.Default.GetBytes(ls_ddd);
                            fs.Write(b, 0, b.Length);
                            fs.Close();
                            ls_ddd = "";
                        }

                        string ls_up_flag = "update [Address List] set Printed = -1 where [Order#]='" + ls_1 + "'";
                        dbaccess.DbHelperACE.ExecuteSql(ls_up_flag);

                    }

                    

                }
            }


            MessageBox.Show("ok");

            //string ls_set_print = "UPDATE [Print Label] INNER JOIN [Address List] ON [Print Label].[Order #]=[Address List].[Order#] SET [Address List].Printed = -1; ";

            




            set_label();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string ls_name = "y:\\Labels.txt";
            using (FileStream fs = File.Open(@ls_name, FileMode.Create))
            {
                byte[] b = System.Text.Encoding.Default.GetBytes("");
                fs.Write(b, 0, b.Length);
                fs.Close();
            }



            string ls_textBox2 = textBox2.Text;
            string[] sArray = Regex.Split(ls_textBox2, "\r\n", RegexOptions.IgnoreCase);
            foreach (string i in sArray)
            {
                if (i.ToString().Length > 0)
                {
                    string ls_get_sql = "SELECT [Order#], [Today's Invoice_order-id], [Global Shipping Reference ID], [recipient-name], [ship-address-1], [ship-address-2], [ship-address-3], [ship-city], [ship-state], [ship-zip], [ship-country], phone " +
                    " FROM [Address List] WHERE ([Address List].addDate)=Date() and  [Order#]='" + i.ToString().Trim() + "'";

                    DataSet ds = new DataSet();
                    ds = dbaccess.DbHelperACE.Query(ls_get_sql);

                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        string ls_1 = ds.Tables[0].Rows[0]["Order#"].ToString();
                        string ls_2 = ds.Tables[0].Rows[0]["Today's Invoice_order-id"].ToString();
                        string ls_3 = ds.Tables[0].Rows[0]["Global Shipping Reference ID"].ToString();
                        string ls_4 = ds.Tables[0].Rows[0]["recipient-name"].ToString();
                        string ls_5 = ds.Tables[0].Rows[0]["ship-address-1"].ToString();
                        string ls_6 = ds.Tables[0].Rows[0]["ship-address-2"].ToString();
                        string ls_7 = ds.Tables[0].Rows[0]["ship-address-3"].ToString();
                        string ls_8 = ds.Tables[0].Rows[0]["ship-city"].ToString();
                        string ls_9 = ds.Tables[0].Rows[0]["ship-state"].ToString();
                        string ls_10 = ds.Tables[0].Rows[0]["ship-zip"].ToString();
                        string ls_11 = ds.Tables[0].Rows[0]["ship-country"].ToString();
                        string ls_12 = ds.Tables[0].Rows[0]["phone"].ToString();


                        string ls_ddd = "\"" + ls_1 + "\"	";
                        ls_ddd += "\"" + ls_2 + "\"	";
                        ls_ddd += "\"" + ls_3 + "\"	";
                        ls_ddd += "\"" + ls_4 + "\"	";
                        ls_ddd += "\"" + ls_5 + "\"	";
                        ls_ddd += "\"" + ls_6 + "\"	";
                        ls_ddd += "\"" + ls_7 + "\"	";
                        ls_ddd += "\"" + ls_8 + "\"	";
                        ls_ddd += "\"" + ls_9 + "\"	";
                        ls_ddd += "\"" + ls_10 + "\"	";
                        ls_ddd += "\"" + ls_11 + "\"	";
                        ls_ddd += "\"" + ls_12 + "\"\r\n";

                        using (FileStream fs = File.Open(@ls_name, FileMode.Append))
                        {
                            byte[] b = System.Text.Encoding.Default.GetBytes(ls_ddd.Replace("\"\"", ""));
                            fs.Write(b, 0, b.Length);
                            fs.Close();
                            ls_ddd = "";
                        }

                        string ls_up_flag = "update [Address List] set Printed = -1 where [Order#]='" + ls_1 + "'";
                        dbaccess.DbHelperACE.ExecuteSql(ls_up_flag);

                    }


                }
            }


            MessageBox.Show("ok");

            string ls_set_print = "UPDATE [Print Label] INNER JOIN [Address List] ON [Print Label].[Order #]=[Address List].[Order#] SET [Address List].Printed = -1; ";




            set_label();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string ls_s = textBox1.Text.Trim().Replace("\r\n\r\n", "\r\n").Replace("\r\n \r\n", "\r\n").Replace("\r\n  \r\n", "\r\n");
            string[] sArray = Regex.Split(ls_s, "\r\n", RegexOptions.IgnoreCase);
            //label2.Text = sArray.Length.ToString();

            int ls_count = 0;
            foreach (string i in sArray)
            {
                if (i.Trim().Length>0)
                { 
                ls_count++;
                }
            }
            label2.Text = ls_count.ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            set_label(); 
            textBox1.Text = "";
            textBox2.Text = "";
            label2.Text = "0";
            label3.Text = "0";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            all_list fm_all = new all_list();
            fm_all.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            all_list fm_all = new all_list();
            fm_all.Show();
        }


    }
}
