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

namespace wms
{
    public partial class MySql : Form
    {
        public MySql()
        {
            InitializeComponent();
        }
        DBUtil DB = new DBUtil();

        private void button2_Click(object sender, EventArgs e)
        {
            string ls_t1 = textBox1.Text;

            DB.ExecuteSQL("truncate table t_sql;");
            string[] dog_small = ls_t1.Replace(",", "|").Split('|');
            int cc = 0;
            foreach (string bb in dog_small)
            {
                string ls_txt_bb = bb.Trim();
                if (ls_txt_bb.Length > 0)
                {
                    string ls_ins = "insert into t_sql (fields) values ('" + ls_txt_bb + "');";
                    DB.ExecuteSQL(ls_ins);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ls_zz1zz = "convert(varchar(8), getdate(), 112)";
            string ls_zz2zz = ",0)";

            string ls_t2 = textBox2.Text;
            string ls_t3 = textBox3.Text.Replace(ls_zz1zz, "zz1zz").Replace(ls_zz2zz, "zz2zz");


            string ls_t5 = textBox5.Text.Replace(ls_zz1zz, "zz1zz").Replace(ls_zz2zz, "zz2zz");



            DB.ExecuteSQL("truncate table t_sql_d;");

            string[] dog_small_f = ls_t2.Replace(",", "|").Split('|');
            string[] dog_small_s = ls_t3.Replace(",", "|").Split('|');
            string[] dog_small_s2 = ls_t5.Replace(",", "|").Split('|');

            int k = dog_small_s2.Length;
            int j = dog_small_f.Length;

            for (int i = 0; i < j; i++)
            {
                if (k == j)
                {

                    string ls_ins = "insert into t_sql_d (fields,sql,sql2) values ('" + dog_small_f[i].ToString().Replace("\r", "").Replace("\n", "").Trim() + "','" + dog_small_s[i].ToString().Replace("\r", "").Replace("\n", "").Replace("'", "''").Replace("zz1zz", ls_zz1zz).Trim().Replace("zz2zz", ls_zz2zz).Trim() + "','" + dog_small_s2[i].ToString().Replace("\r", "").Replace("\n", "").Replace("'", "''").Replace("zz1zz", ls_zz1zz).Trim().Replace("zz2zz", ls_zz2zz).Trim() + "');";
                    DB.ExecuteSQL(ls_ins);

                }
                else
                {
                    string ls_ins = "insert into t_sql_d (fields,sql) values ('" + dog_small_f[i].ToString().Replace("\r", "").Replace("\n", "").Trim() + "','" + dog_small_s[i].ToString().Replace("\r", "").Replace("\n", "").Replace("'", "''").Replace("zz1zz", ls_zz1zz).Trim().Replace("zz2zz", ls_zz2zz).Trim() + "');";
                    DB.ExecuteSQL(ls_ins);
                }

            }



            DB.ExecuteSQL("update [t_sql_d] set sid=(select id from [t_sql] where fields=[t_sql_d].fields);update[t_sql] set eb6a = (select sql from[t_sql_d] where fields =[t_sql].fields),eb6b = (select sql2 from[t_sql_d] where fields =[t_sql].fields); ");

            MessageBox.Show("ok!");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string ls_get = "select fields,sql from [t_sql_d] order by sid";

            string ls_a = "";
            string ls_b = "";

            DataGroup group_amz2 = new DataGroup();
            group_amz2 = null;

            int xxx = 0;

            group_amz2 = DB.GetDataGroup(ls_get);
            if (group_amz2.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_amz2.Table.Rows.Count; i++)
                {
                    string ls_fields = group_amz2.Table.Rows[i]["fields"].ToString();
                    string ls_sql = group_amz2.Table.Rows[i]["sql"].ToString();

                    if ((xxx + 1) % 5 == 0)
                    {
                        ls_a += ls_fields+", \r\n";
                        ls_b += ls_sql + ", \r\n";

                        xxx++;
                    }
                    else
                    {
                        if (ls_sql.IndexOf("select") != -1)
                        {
                            ls_a += "\r\n" + ls_fields + ", \r\n";
                            ls_b += "\r\n" + ls_sql + ", \r\n";
                            xxx = 0;
                        }
                        else
                        {
                            ls_a += ls_fields + ", ";
                            ls_b += ls_sql + ", ";
                            xxx++;
                        }
                    }
                }
            }

            textBox4.Text = ls_a.Replace("\r\n\r\n", "\r\n") + "\r\n\r\n\r\n\r\n" + ls_b.Replace("\r\n\r\n", "\r\n");
        }

        private void button4_Click(object sender, EventArgs e)
        {


            string ls_get = "select fields,eb6a,eb6b from [t_sql] where eb6a is not null order by id ";

            string ls_a = "";
            string ls_b = "";
            string ls_b2 = "";

            DataGroup group_amz2 = new DataGroup();
            group_amz2 = null;

            int xxx = 0;

            group_amz2 = DB.GetDataGroup(ls_get);
            if (group_amz2.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group_amz2.Table.Rows.Count; i++)
                {
                    string ls_fields = group_amz2.Table.Rows[i]["fields"].ToString();
                    string ls_sql = group_amz2.Table.Rows[i]["eb6a"].ToString();
                    string ls_sql2 = group_amz2.Table.Rows[i]["eb6b"].ToString();

                    if ((xxx + 1) % 5 == 0)
                    {
                        ls_a += ls_fields + ", \r\n";
                        ls_b += ls_sql + ", \r\n";
                        ls_b2 += ls_sql2 + ", \r\n";

                        xxx++;
                    }
                    else
                    {
                        if (ls_sql.IndexOf("select") != -1)
                        {
                            ls_a += "\r\n" + ls_fields + ", \r\n";
                            ls_b += "\r\n" + ls_sql + ", \r\n";
                            ls_b2 += "\r\n" + ls_sql2 + ", \r\n";
                            xxx = 0;
                        }
                        else
                        {
                            ls_a += ls_fields + ", ";
                            ls_b += ls_sql + ", ";
                            ls_b2 += ls_sql2 + ", ";
                            xxx++;
                        }
                    }
                }
            }

            textBox4.Text = ls_a.Replace("\r\n\r\n", "\r\n") + "\r\n\r\n\r\n\r\n" + ls_b.Replace("\r\n\r\n", "\r\n") + "\r\n\r\n\r\n\r\n" + ls_b2.Replace("\r\n\r\n", "\r\n");




        }
    }
}
