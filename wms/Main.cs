using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TX.Framework.WindowUI.Forms;

namespace wms
{
    public partial class Main : BaseForm
    {
        public Main()
        {
            InitializeComponent();
        }

        private void invoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void fBAShippingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void fBASalesListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void txMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void invoiceToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            wms.Invoice_new invoice_N= new wms.Invoice_new();
            invoice_N.MdiParent = this;
            invoice_N.Show();
        }

        private void productBarCodeScanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wms.barcode pbc = new wms.barcode();
            pbc.MdiParent = this;
            pbc.Show();
        }

        private void fBAShippingListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wms.fba_query fba = new wms.fba_query();
            fba.MdiParent = this;
            fba.Show();
        }

        private void fBASalesListToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            wms.fba_shipping_sales fba_shipping_s = new wms.fba_shipping_sales();
            fba_shipping_s.MdiParent = this;
            fba_shipping_s.Show();
        }

        private void exportAddressDHLUspsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tracking_dhl dhl = new wms.tracking_dhl();
            dhl.MdiParent = this;
            dhl.Show();
        }

        private void exportAddressUPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tracking_ups ups = new tracking_ups();
            ups.MdiParent = this;
            ups.Show();
        }


    }
}
