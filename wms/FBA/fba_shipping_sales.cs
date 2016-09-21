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

namespace wms
{
    public partial class fba_shipping_sales : BaseForm
    {
        public fba_shipping_sales()
        {
            InitializeComponent();
        }

        DBUtil DB = new DBUtil();

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            OleDbConnection myConn = new OleDbConnection(connstr);
            myConn.Open();

            string sqlstr = "";
            if (textBox3.Text.Length > 0)
            {
                sqlstr += " and [item] like '" + textBox3.Text.Trim() + "%'  ";
            }
            if (textBox1.Text.Length > 0)
            {
                sqlstr += " and fbasku like '" + textBox1.Text.Trim() + "%'  ";
            }
            if (textBox2.Text.Length > 0)
            {
                sqlstr += " and  [item name] like '%" + textBox2.Text.Trim() + "%'  ";
            }

            string ls_sql = @"select item,[item name] as item_name,fbasku,qty,
(select sum(cast([quantity] as float)) as QTY
from t_amazon_fba_sale_list where [fulfillment-channel] = 'Amazon'  and item = t_amazon_home_fba_sku_list.item
and[order-status] in ('Shipped','Pending')   and[item-status] in ('Unshipped','Shipped')   
group by item,unit_price,[sku]) fba_sale_qty,
asin,fbaprice,[fulfillment-center-id] as fulfillment_center_id,shippingtrackernumber,shippingdate,status,remark from t_amazon_home_fba_sku_list where fbasku is not null and status is not null " + sqlstr + @"
order by status desc, item";

            OleDbDataAdapter adapter = new OleDbDataAdapter(ls_sql, myConn);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Columns[0].DataPropertyName = "item";
            dataGridView1.Columns[1].DataPropertyName = "item_name";
            dataGridView1.Columns[2].DataPropertyName = "fbasku";
            dataGridView1.Columns[3].DataPropertyName = "qty";
            dataGridView1.Columns[4].DataPropertyName = "fba_sale_qty";
            dataGridView1.Columns[5].DataPropertyName = "asin";
            dataGridView1.Columns[6].DataPropertyName = "fbaprice";
            dataGridView1.Columns[7].DataPropertyName = "fulfillment_center_id";
            dataGridView1.Columns[8].DataPropertyName = "shippingtrackernumber";
            dataGridView1.Columns[9].DataPropertyName = "shippingdate";
            dataGridView1.Columns[10].DataPropertyName = "status";
            dataGridView1.Columns[11].DataPropertyName = "remark";
            myConn.Close();


            int x = dataGridView1.RowCount - 1;
            for (int i = 0; i < x; i++)
            {
                string ls_qty = dataGridView1.Rows[i].Cells[3].Value.ToString();
                string ls_qty2 = dataGridView1.Rows[i].Cells[4].Value.ToString();

                if (ls_qty2.Length == 0)
                {
                    dataGridView1[4, i].Style.BackColor = Color.LightBlue;
                }
                else
                {
                    if (float.Parse(ls_qty2) / float.Parse(ls_qty) < 0.5)
                    {
                        dataGridView1[4, i].Style.BackColor = Color.Pink;
                    }
                }
            }


        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

           



        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {


        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }
    }
}
