namespace wms
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txMenuStrip1 = new TX.Framework.WindowUI.Controls.TXMenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.invoiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.productBarCodeScanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printBarCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.fBAShippingListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fBASalesListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAddressDHLUspsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAddressUPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txMenuStrip1
            // 
            this.txMenuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(247)))), ((int)(((byte)(252)))));
            this.txMenuStrip1.BeginBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(247)))), ((int)(((byte)(252)))));
            this.txMenuStrip1.EndBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(247)))), ((int)(((byte)(252)))));
            this.txMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem4,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
            this.txMenuStrip1.Location = new System.Drawing.Point(3, 27);
            this.txMenuStrip1.Name = "txMenuStrip1";
            this.txMenuStrip1.Size = new System.Drawing.Size(1301, 24);
            this.txMenuStrip1.TabIndex = 14;
            this.txMenuStrip1.Text = "txMenuStrip1";
            this.txMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.txMenuStrip1_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.invoiceToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(49, 20);
            this.toolStripMenuItem1.Text = "Order";
            // 
            // invoiceToolStripMenuItem
            // 
            this.invoiceToolStripMenuItem.Name = "invoiceToolStripMenuItem";
            this.invoiceToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.invoiceToolStripMenuItem.Text = "Invoice";
            this.invoiceToolStripMenuItem.Click += new System.EventHandler(this.invoiceToolStripMenuItem_Click_1);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.productBarCodeScanToolStripMenuItem,
            this.printBarCodeToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(64, 20);
            this.toolStripMenuItem2.Text = "BarCode";
            // 
            // productBarCodeScanToolStripMenuItem
            // 
            this.productBarCodeScanToolStripMenuItem.Name = "productBarCodeScanToolStripMenuItem";
            this.productBarCodeScanToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.productBarCodeScanToolStripMenuItem.Text = "Product Bar Code Scan";
            this.productBarCodeScanToolStripMenuItem.Click += new System.EventHandler(this.productBarCodeScanToolStripMenuItem_Click);
            // 
            // printBarCodeToolStripMenuItem
            // 
            this.printBarCodeToolStripMenuItem.Name = "printBarCodeToolStripMenuItem";
            this.printBarCodeToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.printBarCodeToolStripMenuItem.Text = "Print Bar Code";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fBAShippingListToolStripMenuItem,
            this.fBASalesListToolStripMenuItem});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(40, 20);
            this.toolStripMenuItem3.Text = "FBA";
            // 
            // fBAShippingListToolStripMenuItem
            // 
            this.fBAShippingListToolStripMenuItem.Name = "fBAShippingListToolStripMenuItem";
            this.fBAShippingListToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fBAShippingListToolStripMenuItem.Text = "FBA Shipping Query";
            this.fBAShippingListToolStripMenuItem.Click += new System.EventHandler(this.fBAShippingListToolStripMenuItem_Click);
            // 
            // fBASalesListToolStripMenuItem
            // 
            this.fBASalesListToolStripMenuItem.Name = "fBASalesListToolStripMenuItem";
            this.fBASalesListToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fBASalesListToolStripMenuItem.Text = "FBA Sales List";
            this.fBASalesListToolStripMenuItem.Click += new System.EventHandler(this.fBASalesListToolStripMenuItem_Click_1);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportAddressDHLUspsToolStripMenuItem,
            this.exportAddressUPSToolStripMenuItem});
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(65, 20);
            this.toolStripMenuItem4.Text = "Tracking";
            // 
            // exportAddressDHLUspsToolStripMenuItem
            // 
            this.exportAddressDHLUspsToolStripMenuItem.Name = "exportAddressDHLUspsToolStripMenuItem";
            this.exportAddressDHLUspsToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.exportAddressDHLUspsToolStripMenuItem.Text = "Export Address to DHL Usps";
            this.exportAddressDHLUspsToolStripMenuItem.Click += new System.EventHandler(this.exportAddressDHLUspsToolStripMenuItem_Click);
            // 
            // exportAddressUPSToolStripMenuItem
            // 
            this.exportAddressUPSToolStripMenuItem.Name = "exportAddressUPSToolStripMenuItem";
            this.exportAddressUPSToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.exportAddressUPSToolStripMenuItem.Text = "Export Address to UPS";
            this.exportAddressUPSToolStripMenuItem.Click += new System.EventHandler(this.exportAddressUPSToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Pink;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1307, 752);
            this.Controls.Add(this.txMenuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IsMdiContainer = true;
            this.Location = new System.Drawing.Point(0, 0);
            this.MainMenuStrip = this.txMenuStrip1;
            this.Name = "Main";
            this.Text = "Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Main_Load);
            this.txMenuStrip1.ResumeLayout(false);
            this.txMenuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TX.Framework.WindowUI.Controls.TXMenuStrip txMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem invoiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem productBarCodeScanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printBarCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fBAShippingListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fBASalesListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem exportAddressDHLUspsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAddressUPSToolStripMenuItem;
    }
}