namespace UniversalAnalyse
{
    partial class wmb
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.orderManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invoiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.barCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.productBarCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printBarCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fBAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fBAShippingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fBASalesListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(34, 216);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(469, 299);
            this.dataGridView1.TabIndex = 2;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(46, 114);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(123, 40);
            this.button6.TabIndex = 9;
            this.button6.Text = "1.Import All Site Order";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(193, 114);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(86, 40);
            this.button7.TabIndex = 10;
            this.button7.Text = "2. Invoice";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(308, 114);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(132, 40);
            this.button8.TabIndex = 11;
            this.button8.Text = "3. Get Tracking Number";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // orderManagerToolStripMenuItem
            // 
            this.orderManagerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.invoiceToolStripMenuItem});
            this.orderManagerToolStripMenuItem.Name = "orderManagerToolStripMenuItem";
            this.orderManagerToolStripMenuItem.Size = new System.Drawing.Size(99, 20);
            this.orderManagerToolStripMenuItem.Text = "Order Manager";
            // 
            // invoiceToolStripMenuItem
            // 
            this.invoiceToolStripMenuItem.Name = "invoiceToolStripMenuItem";
            this.invoiceToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.invoiceToolStripMenuItem.Text = "Invoice";
            this.invoiceToolStripMenuItem.Click += new System.EventHandler(this.invoiceToolStripMenuItem_Click);
            // 
            // barCodeToolStripMenuItem
            // 
            this.barCodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.productBarCodeToolStripMenuItem,
            this.printBarCodeToolStripMenuItem});
            this.barCodeToolStripMenuItem.Name = "barCodeToolStripMenuItem";
            this.barCodeToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.barCodeToolStripMenuItem.Text = "BarCode";
            // 
            // productBarCodeToolStripMenuItem
            // 
            this.productBarCodeToolStripMenuItem.Name = "productBarCodeToolStripMenuItem";
            this.productBarCodeToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.productBarCodeToolStripMenuItem.Text = "Product BarCode";
            this.productBarCodeToolStripMenuItem.Click += new System.EventHandler(this.productBarCodeToolStripMenuItem_Click);
            // 
            // printBarCodeToolStripMenuItem
            // 
            this.printBarCodeToolStripMenuItem.Name = "printBarCodeToolStripMenuItem";
            this.printBarCodeToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.printBarCodeToolStripMenuItem.Text = "Print BarCode";
            // 
            // fBAToolStripMenuItem
            // 
            this.fBAToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fBAShippingToolStripMenuItem,
            this.fBASalesListToolStripMenuItem});
            this.fBAToolStripMenuItem.Name = "fBAToolStripMenuItem";
            this.fBAToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.fBAToolStripMenuItem.Text = "FBA";
            // 
            // fBAShippingToolStripMenuItem
            // 
            this.fBAShippingToolStripMenuItem.Name = "fBAShippingToolStripMenuItem";
            this.fBAShippingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.fBAShippingToolStripMenuItem.Text = "FBA Shipping";
            this.fBAShippingToolStripMenuItem.Click += new System.EventHandler(this.fBAShippingToolStripMenuItem_Click);
            // 
            // fBASalesListToolStripMenuItem
            // 
            this.fBASalesListToolStripMenuItem.Name = "fBASalesListToolStripMenuItem";
            this.fBASalesListToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.fBASalesListToolStripMenuItem.Text = "FBA Sales List";
            this.fBASalesListToolStripMenuItem.Click += new System.EventHandler(this.fBASalesListToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.orderManagerToolStripMenuItem,
            this.barCodeToolStripMenuItem,
            this.fBAToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(3, 31);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1553, 24);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // wmb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1559, 630);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Location = new System.Drawing.Point(0, 0);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "wmb";
            this.Text = "wmb";
            this.Load += new System.EventHandler(this.wmb_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.ToolStripMenuItem orderManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invoiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem barCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem productBarCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printBarCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fBAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fBAShippingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fBASalesListToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}