namespace wms
{
    partial class fba_shipping_sales
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
            this.label6 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.item = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.item_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fbasku = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QTY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fba_sale_qty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.asin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fbaprice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fulfillment_center_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shippingtrackernumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShippingDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.remark = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txGroupBox1 = new TX.Framework.WindowUI.Controls.TXGroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.txGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(59, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 20);
            this.label6.TabIndex = 67;
            this.label6.Text = "Item:";
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(110, 28);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(136, 30);
            this.textBox3.TabIndex = 66;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(291, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 20);
            this.label1.TabIndex = 52;
            this.label1.Text = "SKU:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(523, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(114, 20);
            this.label7.TabIndex = 54;
            this.label7.Text = "Product Name:";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(655, 30);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(136, 30);
            this.textBox2.TabIndex = 53;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(343, 30);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(136, 30);
            this.textBox1.TabIndex = 51;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button4.Location = new System.Drawing.Point(979, 28);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(99, 34);
            this.button4.TabIndex = 52;
            this.button4.Text = "Clear";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button2.Location = new System.Drawing.Point(847, 28);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(106, 34);
            this.button2.TabIndex = 51;
            this.button2.Text = "Search";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.item,
            this.item_name,
            this.fbasku,
            this.QTY,
            this.fba_sale_qty,
            this.asin,
            this.fbaprice,
            this.fulfillment_center_id,
            this.shippingtrackernumber,
            this.ShippingDate,
            this.status,
            this.remark});
            this.dataGridView1.Location = new System.Drawing.Point(12, 100);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1367, 730);
            this.dataGridView1.TabIndex = 50;
            this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
            this.dataGridView1.Click += new System.EventHandler(this.dataGridView1_Click);
            // 
            // item
            // 
            this.item.HeaderText = "Item";
            this.item.Name = "item";
            // 
            // item_name
            // 
            this.item_name.HeaderText = "Item Name";
            this.item_name.Name = "item_name";
            this.item_name.Width = 200;
            // 
            // fbasku
            // 
            this.fbasku.HeaderText = "SKU";
            this.fbasku.Name = "fbasku";
            // 
            // QTY
            // 
            this.QTY.HeaderText = "Qty";
            this.QTY.Name = "QTY";
            this.QTY.Width = 80;
            // 
            // fba_sale_qty
            // 
            this.fba_sale_qty.HeaderText = "Fba Sales Qty";
            this.fba_sale_qty.Name = "fba_sale_qty";
            this.fba_sale_qty.Width = 90;
            // 
            // asin
            // 
            this.asin.HeaderText = "Asin";
            this.asin.Name = "asin";
            this.asin.Width = 90;
            // 
            // fbaprice
            // 
            this.fbaprice.HeaderText = "Price";
            this.fbaprice.Name = "fbaprice";
            // 
            // fulfillment_center_id
            // 
            this.fulfillment_center_id.HeaderText = "fulfillment center id";
            this.fulfillment_center_id.Name = "fulfillment_center_id";
            // 
            // shippingtrackernumber
            // 
            this.shippingtrackernumber.HeaderText = "Tracker Number";
            this.shippingtrackernumber.Name = "shippingtrackernumber";
            // 
            // ShippingDate
            // 
            this.ShippingDate.HeaderText = "Shipping Date";
            this.ShippingDate.Name = "ShippingDate";
            // 
            // status
            // 
            this.status.HeaderText = "Status";
            this.status.Name = "status";
            // 
            // remark
            // 
            this.remark.HeaderText = "Remark";
            this.remark.Name = "remark";
            // 
            // txGroupBox1
            // 
            this.txGroupBox1.BackColor = System.Drawing.Color.Transparent;
            this.txGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(168)))), ((int)(((byte)(192)))));
            this.txGroupBox1.CaptionColor = System.Drawing.Color.Black;
            this.txGroupBox1.CaptionFont = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold);
            this.txGroupBox1.Controls.Add(this.label6);
            this.txGroupBox1.Controls.Add(this.button4);
            this.txGroupBox1.Controls.Add(this.textBox1);
            this.txGroupBox1.Controls.Add(this.button2);
            this.txGroupBox1.Controls.Add(this.label1);
            this.txGroupBox1.Controls.Add(this.textBox3);
            this.txGroupBox1.Controls.Add(this.textBox2);
            this.txGroupBox1.Controls.Add(this.label7);
            this.txGroupBox1.Location = new System.Drawing.Point(12, 27);
            this.txGroupBox1.Name = "txGroupBox1";
            this.txGroupBox1.Size = new System.Drawing.Size(1367, 67);
            this.txGroupBox1.TabIndex = 54;
            this.txGroupBox1.TabStop = false;
            this.txGroupBox1.Text = "FBA Shipping and Sales Report";
            this.txGroupBox1.TextMargin = 6;
            // 
            // fba_shipping_sales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1391, 836);
            this.Controls.Add(this.txGroupBox1);
            this.Controls.Add(this.dataGridView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "fba_shipping_sales";
            this.Text = "fba_shipping_sales";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.txGroupBox1.ResumeLayout(false);
            this.txGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn item;
        private System.Windows.Forms.DataGridViewTextBoxColumn item_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn fbasku;
        private System.Windows.Forms.DataGridViewTextBoxColumn QTY;
        private System.Windows.Forms.DataGridViewTextBoxColumn fba_sale_qty;
        private System.Windows.Forms.DataGridViewTextBoxColumn asin;
        private System.Windows.Forms.DataGridViewTextBoxColumn fbaprice;
        private System.Windows.Forms.DataGridViewTextBoxColumn fulfillment_center_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn shippingtrackernumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShippingDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
        private System.Windows.Forms.DataGridViewTextBoxColumn remark;
        private TX.Framework.WindowUI.Controls.TXGroupBox txGroupBox1;
    }
}