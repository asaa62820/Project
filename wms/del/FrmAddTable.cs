using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace UniversalAnalyse
{
    public partial class FrmAddTable : Form
    {
        public FrmAddTable()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.ColumnIndex == 2 && e.RowIndex>-1)
            //{
            //    //DataGridViewCell dgc = (DataGridViewCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            //    DataGridViewComboBoxCell  dgComCell = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            //    if (dgComCell.Value.ToString() == "nvarchar")
            //    {
            //        DataGridViewCell dgc = (DataGridViewCell)dataGridView1.Rows[e.RowIndex].Cells[3];
            //        dgc.Value = "30";
            //    }
            //}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //            //修改数据类型
            //            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            //            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();
            //            OleDbConnection myConn = new OleDbConnection(connstr);
            //            string sqlStr = @"SELECT table_name,column_name FROM INFORMATION_SCHEMA.COLUMNS
            //                                WHERE (DATA_TYPE='TEXT' OR DATA_TYPE='NTEXT')
            //                                and substring(table_name,1,2)='T_' order by table_name";
            //            OleDbDataAdapter adapter = new OleDbDataAdapter(sqlStr, myConn);
            //            DataTable dt = new DataTable();
            //            adapter.Fill(dt);
            //            sqlStr = string.Empty;

            //            for (int i = 0; i < dt.Rows.Count; i++)
            //            {
            //                sqlStr += "ALTER TABLE {0} alter column  {1} nvarchar(MAX)";
            //                sqlStr = string.Format(sqlStr, dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
            //            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.RowIndex > -1)
            {
                //DataGridViewCell dgc = (DataGridViewCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                DataGridViewComboBoxCell dgComCell = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (dgComCell.Value.ToString() == "nvarchar")
                {
                    DataGridViewCell dgc = (DataGridViewCell)dataGridView1.Rows[e.RowIndex].Cells[3];
                    dgc.Value = "30";
                }
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.RowIndex > -1)
            {
                //DataGridViewCell dgc = (DataGridViewCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                DataGridViewComboBoxCell dgComCell = (DataGridViewComboBoxCell)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (dgComCell.Value != null  && dgComCell.Value.ToString() == "nvarchar")
                {
                    DataGridViewCell dgc = (DataGridViewCell)dataGridView1.Rows[e.RowIndex].Cells[3];
                    dgc.Value = "30";
                }
            }
        }
    }
}