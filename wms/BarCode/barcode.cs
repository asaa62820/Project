using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using TMS.Framework.Business.DataAccess;
using TMS.Framework.Publics.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using TX.Framework.WindowUI.Forms;

namespace wms
{
    public partial class barcode : BaseForm
    {
        //DBUtil DB = new DBUtil();

        //private DateTime _dt = DateTime.Now; 

        public barcode()
        {
            InitializeComponent();
        }
        private void barcode_Load(object sender, EventArgs e)
        {
            //BarCode.Start();
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();
            string ls_up = "update [t_barcode_Listing_table] set [units on hand]=(select [units on hand] from [t_lisa_Listing_Table] where item=[t_barcode_Listing_table].item);";
            OleDbCommand cmd = new OleDbCommand(ls_up, myConn);
            cmd.ExecuteNonQuery();




            string ls_m_sql = "select 'BarCode Count:'+cast((select count(item) from [t_barcode_Listing_table] where len(barcode)>7) as nvarchar(20))+'   '+'Instock Item:'+cast((select count(item) from [t_barcode_Listing_table] where [Units On Hand] >0) as nvarchar(20))+'   '+'All Item:'+cast((select count(item) from [t_barcode_Listing_table]) as nvarchar(20))";
            OleDbDataAdapter adapter2 = new OleDbDataAdapter(ls_m_sql, myConn);
            DataSet ds2 = new DataSet();
            adapter2.Fill(ds2);
            this.Text = " " + ds2.Tables[0].Rows[0][0].ToString();


            myConn.Close();
        }

        
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }









        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            textBox6.Text = "";
            textBox5.Text = "";
            textBox4.Text = "";
            textBox3.Text = "";
            textBox2.Text = "";
            textBox8.Text = "";
            textBox9.Text = "";
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            dataGridView1.AutoGenerateColumns = false;

            string ls_search_code = textBox1.Text.Trim();
            string ls_search_code2 = textBox6.Text.Trim();
            string ls_bc = textBox8.Text.Trim();
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();

            string sqlstr = "select item as item,[item name] as item_name,barcode,weight,[Units On Hand] as Qty from [t_barcode_Listing_table] where 1=1";

            if (ls_search_code.Length > 0)
            {
                sqlstr += " and item like '" + ls_search_code + "%'  ";
            }

            if (ls_search_code2.Length > 0)
            {
                sqlstr += " and [item name] like '%" + ls_search_code2 + "%'  ";
            }


            if (ls_bc.Length > 7)
            {
                sqlstr += " and [barcode] = '" + ls_bc + "'   ";
            }

            if (this.checkBox1.Checked == true)
            {
                sqlstr += " and [Units On Hand] >0   ";
            }
            
            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].DataPropertyName = "Item";
            dataGridView1.Columns[1].DataPropertyName = "Item_Name";
            dataGridView1.Columns[2].DataPropertyName = "Qty";
            dataGridView1.Columns[3].DataPropertyName = "Barcode";
            dataGridView1.Columns[4].DataPropertyName = "Weight";
           


            if (ds.Tables[0].Rows.Count > 0)
            {

                int i_row = 0;
                dataGridView1.Rows[i_row].Selected = true;

                textBox5.Text = dataGridView1.Rows[i_row].Cells[0].Value.ToString();
                textBox4.Text = dataGridView1.Rows[i_row].Cells[1].Value.ToString();

                if (dataGridView1.Rows[i_row].Cells[3].Value != null)
                {
                    textBox2.Text = dataGridView1.Rows[i_row].Cells[3].Value.ToString();
                }

                if (dataGridView1.Rows[i_row].Cells[4].Value != null)
                {
                    textBox3.Text = dataGridView1.Rows[i_row].Cells[4].Value.ToString();
                }

                textBox2.Focus();

                l_index.Text = "0";
            }

            string ls_m_sql = "select 'BarCode Count:'+cast((select count(item) from [t_barcode_Listing_table] where len(barcode)>7) as nvarchar(20))+'   '+'Instock Item:'+cast((select count(item) from [t_barcode_Listing_table] where [Units On Hand] >0) as nvarchar(20))+'   '+'All Item:'+cast((select count(item) from [t_barcode_Listing_table]) as nvarchar(20))";


            OleDbDataAdapter adapter2 = new OleDbDataAdapter(ls_m_sql, myConn);
            DataSet ds2 = new DataSet();
            adapter2.Fill(ds2);
            //label11.Text = ds2.Tables[0].Rows[0][0].ToString();
            
            this.Text=" "+ ds2.Tables[0].Rows[0][0].ToString();

            //if (ds.Tables[0].Rows[0][0].ToString() != "0")
            //{
            //    textBox8.Text = ls_barcode;
            //    l_message.Text = "Error:The bar code has been used!\r\n Please use search button check it! Thanks! ";
            //    return;
            //}



            label11.Text = "";
            myConn.Close();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.White;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.BackColor = Color.Yellow;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            textBox2.BackColor = Color.White;
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox2.BackColor = Color.Yellow;
           
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            textBox3.BackColor = Color.Yellow;
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            textBox3.BackColor = Color.White;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int i_row=dataGridView1.CurrentCell.RowIndex;
            dataGridView1.Rows[i_row].Selected = true;
            textBox5.Text = dataGridView1.Rows[i_row].Cells[0].Value.ToString();
            textBox4.Text = dataGridView1.Rows[i_row].Cells[1].Value.ToString();
            if (dataGridView1.Rows[i_row].Cells[3].Value != null)
            {
                textBox2.Text = dataGridView1.Rows[i_row].Cells[3].Value.ToString();
            }

            if (dataGridView1.Rows[i_row].Cells[4].Value != null)
            {
                textBox3.Text = dataGridView1.Rows[i_row].Cells[4].Value.ToString();
            }
            textBox2.Focus();

            l_index.Text = i_row.ToString();

            l_message.Text = "";

        }

        private void textBox6_Enter(object sender, EventArgs e)
        {
            textBox6.BackColor = Color.Yellow;
        }

        private void textBox6_Leave(object sender, EventArgs e)
        {
            textBox6.BackColor = Color.White;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string ls_sku = textBox5.Text;
            string ls_barcode = textBox2.Text;
            string ls_weight = textBox3.Text;

            //if (ls_sku.Length > 5)//&& ls_barcode.Length > 7 && ls_weight.Length > 0
            //{

                System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
                string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
                OleDbConnection myConn = new OleDbConnection(connstr);
                myConn.Open();


                string sqlstr = "select count(*) as nb from  [t_barcode_Listing_table] where barcode='" + ls_barcode + "'";
                OleDbDataAdapter adapter = new OleDbDataAdapter(sqlstr, myConn);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                //if (ds.Tables[0].Rows[0][0].ToString() != "0")
                //{
                //    textBox8.Text = ls_barcode;
                //    l_message.Text = "Error:The bar code has been used!\r\n Please use search button check it! Thanks! ";
                //    return;
                //}






                string ls_up = "update [t_barcode_Listing_table] set barcode='" + ls_barcode + "'  ,weight='" + ls_weight + "' where item='" + ls_sku + "' ";
                //DB.ExecuteSQL(ls_up);
                OleDbCommand cmd = new OleDbCommand(ls_up, myConn);
                // Clear table data
                cmd.ExecuteNonQuery();


                string ls_m_sql = "select 'BarCode Count:'+cast((select count(item) from [t_barcode_Listing_table] where len(barcode)>7) as nvarchar(20))+'   '+'Instock Item:'+cast((select count(item) from [t_barcode_Listing_table] where [Units On Hand] >0) as nvarchar(20))+'   '+'All Item:'+cast((select count(item) from [t_barcode_Listing_table]) as nvarchar(20))";
                OleDbDataAdapter adapter2 = new OleDbDataAdapter(ls_m_sql, myConn);
                DataSet ds2 = new DataSet();
                adapter2.Fill(ds2);
                this.Text = " " + ds2.Tables[0].Rows[0][0].ToString();



                myConn.Close();

                int i_index = int.Parse(l_index.Text);
                dataGridView1.Rows[i_index].Cells[3].Value = ls_barcode;
                dataGridView1.Rows[i_index].Cells[4].Value = ls_weight;

                l_message.Text = "" + ls_sku + "  ---- Updated Success! \r\n" + textBox4.Text + " \r\n Barcode:" + ls_barcode + " \r\n Weight(oz):" + ls_weight + "";

                textBox5.Text = "";
                textBox4.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox9.Text = "";

                

                string ls_t1 = textBox1.Text;
                if (ls_t1.Length > 2)
                {
                    textBox1.Text = ls_t1.Substring(0, 2);
                }
                textBox1.Focus();
                textBox1.SelectionStart = textBox1.Text.Length;

            //}
            //else
            //{
            //    //if (ls_weight.Length < 10)
            //    //{
            //    //    textBox2.Focus();
            //    //}

            //    if (ls_sku.Length<5)
            //    {
            //        textBox1.Focus();
            //    }

            //}
            //textBox1.Focus();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //DateTime tempDt = DateTime.Now;
            //TimeSpan ts = tempDt.Subtract(_dt); 
            //if (ts.Milliseconds > 20)
            //{ 
            //    textBox2.Text = "";
            //    _dt = tempDt;
            //}
            //SendKeys "{tab}" 

            if (e.KeyChar == (char)13)
            {
                SendKeys.Send("{Tab}"); 
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                SendKeys.Send("{Tab}");
                SendKeys.Send("{Enter}");
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                SendKeys.Send("{Tab}");
                SendKeys.Send("{Enter}");
            }
        }

        private void textBox8_Enter(object sender, EventArgs e)
        {
            textBox8.BackColor = Color.Yellow;
        }

        private void textBox8_Leave(object sender, EventArgs e)
        {
            textBox8.BackColor = Color.White;
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //textBox3.Text = (float.Parse(textBox9.Text) * 16).ToString();
                string ls_textbox9 = textBox9.Text;
                if (ls_textbox9.IndexOf(".") != -1)
                {
                    string ls_pound = ls_textbox9.Substring(0, ls_textbox9.IndexOf("."));
                    string ls_ox = ls_textbox9.Substring(ls_textbox9.IndexOf(".")+1, ls_textbox9.Length- ls_textbox9.IndexOf(".")-1);

                    try
                    {
                        textBox3.Text = ((float.Parse(ls_pound) * 16) + float.Parse(ls_ox)).ToString();
                    }
                    catch { }

                }
                else
                {
                    try
                    {
                        textBox3.Text = (float.Parse(ls_textbox9) * 16).ToString();
                    }
                    catch { }
                }

            }
            catch
            { }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {


        }
    }
}
