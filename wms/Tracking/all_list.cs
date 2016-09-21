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
    public partial class all_list : BaseForm
    {
        public all_list()
        {
            InitializeComponent();
        }

        private void all_list_Load(object sender, EventArgs e)
        {
            //dataGridView1.DataSource = null;

            //DataTable dt = (DataTable)dataGridView1.DataSource;
            //dt.Rows.Clear();
            //dataGridView1.DataSource = dt; 


            DataSet ds=new DataSet();
            ds = dbaccess.DbHelperACE.Query("select [printed],[from],[order#],[Today's Invoice_order-id],[payments-date],[buyer-name],[Global Shipping Reference ID],[phone],[recipient-name],"+
            " [ship-address-1],[ship-address-2],[ship-address-3],[ship-city],[ship-state],[ship-zip],[ship-country],[addDate] from [address list] ");
            dataGridView1.DataSource = ds.Tables[0];
            //dataGridView1.AutoGenerateColumns = false;
            //dataGridView1.DataMember = "[order#]";

            //DataTable
            //dataGridView1.DataSource = Ds.Tables["T_Class"];

            set_label();
            

        }

        private void button5_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds = dbaccess.DbHelperACE.Query("select [printed],[from],[order#],[Today's Invoice_order-id],[payments-date],[buyer-name],[Global Shipping Reference ID],[phone],[recipient-name]," +
            " [ship-address-1],[ship-address-2],[ship-address-3],[ship-city],[ship-state],[ship-zip],[ship-country],[addDate] from [address list] ");
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds = dbaccess.DbHelperACE.Query("select [printed],[from],[order#],[Today's Invoice_order-id],[payments-date],[buyer-name],[Global Shipping Reference ID],[phone],[recipient-name]," +
            " [ship-address-1],[ship-address-2],[ship-address-3],[ship-city],[ship-state],[ship-zip],[ship-country],[addDate] FROM [Address List] WHERE ((([Address List].[Printed])=0)); ");
            dataGridView1.DataSource = ds.Tables[0];
        }


        private void set_label()
        {
            string ls_un_number = "SELECT Count([Order#]) FROM [Address List] WHERE [Address List].printed=0; ";
            string ls_all_number = "SELECT Count([Order#]) FROM [Address List]; ";
            object obj1 = dbaccess.DbHelperACE.GetSingle(ls_un_number);
            object obj2 = dbaccess.DbHelperACE.GetSingle(ls_all_number);
            label1.Text = "Unprinted: " + obj1.ToString() + " / " + obj2.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string ls_search_code = textBox1.Text;

            DataSet ds = new DataSet();
            ds = dbaccess.DbHelperACE.Query("select [printed],[from],[order#],[Today's Invoice_order-id],[payments-date],[buyer-name],[Global Shipping Reference ID],[phone],[recipient-name]," +
            " [ship-address-1],[ship-address-2],[ship-address-3],[ship-city],[ship-state],[ship-zip],[ship-country],[addDate] from [address list]  where "+
            " [from] like '%" + ls_search_code + "%'  or [order#] like '%" + ls_search_code + "%'   " +
            " or [buyer-name] like '%" + ls_search_code + "%'  or [Global Shipping Reference ID] like '%" + ls_search_code + "%'   " +
            " or [phone] like '%" + ls_search_code + "%'  or [recipient-name] like '%" + ls_search_code + "%'   " +
            " or [ship-address-1] like '%" + ls_search_code + "%'  or [ship-city] like '%" + ls_search_code + "%'   " +
            " or [ship-state] like '%" + ls_search_code + "%'  or [ship-zip] like '%" + ls_search_code + "%'   " +
            " or [ship-country] like '%" + ls_search_code + "%'  or [addDate] like '%" + ls_search_code + "%'    or [Today's Invoice_order-id] like '%" + ls_search_code + "%'    " +
            "");
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";

            DataSet ds = new DataSet();
            ds = dbaccess.DbHelperACE.Query("select [printed],[from],[order#],[Today's Invoice_order-id],[payments-date],[buyer-name],[Global Shipping Reference ID],[phone],[recipient-name]," +
            " [ship-address-1],[ship-address-2],[ship-address-3],[ship-city],[ship-state],[ship-zip],[ship-country],[addDate] from [address list] ");
            dataGridView1.DataSource = ds.Tables[0];
            //dataGridView1.AutoGenerateColumns = false;
            //dataGridView1.DataMember = "[order#]";

            //DataTable
            //dataGridView1.DataSource = Ds.Tables["T_Class"];

            set_label();
        }

    }
}

