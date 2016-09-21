namespace wms
{
    partial class Invoice
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
            this.txButton7 = new TX.Framework.WindowUI.Controls.TXButton();
            this.txButton1 = new TX.Framework.WindowUI.Controls.TXButton();
            this.txButton2 = new TX.Framework.WindowUI.Controls.TXButton();
            this.txButton3 = new TX.Framework.WindowUI.Controls.TXButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(51, 137);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(802, 299);
            this.dataGridView1.TabIndex = 12;
            // 
            // txButton7
            // 
            this.txButton7.Image = null;
            this.txButton7.Location = new System.Drawing.Point(51, 63);
            this.txButton7.Name = "txButton7";
            this.txButton7.Size = new System.Drawing.Size(142, 37);
            this.txButton7.TabIndex = 17;
            this.txButton7.Text = "1.Import All Site Order";
            this.txButton7.UseVisualStyleBackColor = true;
            this.txButton7.Click += new System.EventHandler(this.txButton7_Click);
            // 
            // txButton1
            // 
            this.txButton1.Image = null;
            this.txButton1.Location = new System.Drawing.Point(214, 63);
            this.txButton1.Name = "txButton1";
            this.txButton1.Size = new System.Drawing.Size(142, 37);
            this.txButton1.TabIndex = 18;
            this.txButton1.Text = "2. Invoice";
            this.txButton1.UseVisualStyleBackColor = true;
            this.txButton1.Click += new System.EventHandler(this.txButton1_Click);
            // 
            // txButton2
            // 
            this.txButton2.Image = null;
            this.txButton2.Location = new System.Drawing.Point(378, 63);
            this.txButton2.Name = "txButton2";
            this.txButton2.Size = new System.Drawing.Size(142, 37);
            this.txButton2.TabIndex = 19;
            this.txButton2.Text = "3. Get Tracking Number";
            this.txButton2.UseVisualStyleBackColor = true;
            this.txButton2.Click += new System.EventHandler(this.txButton2_Click);
            // 
            // txButton3
            // 
            this.txButton3.Image = null;
            this.txButton3.Location = new System.Drawing.Point(214, 99);
            this.txButton3.Name = "txButton3";
            this.txButton3.Size = new System.Drawing.Size(142, 37);
            this.txButton3.TabIndex = 20;
            this.txButton3.Text = "OverStock Invoice";
            this.txButton3.UseVisualStyleBackColor = true;
            this.txButton3.Click += new System.EventHandler(this.txButton3_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(378, 111);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(475, 20);
            this.textBox1.TabIndex = 21;
            // 
            // Invoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1162, 769);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.txButton3);
            this.Controls.Add(this.txButton2);
            this.Controls.Add(this.txButton1);
            this.Controls.Add(this.txButton7);
            this.Controls.Add(this.dataGridView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "Invoice";
            this.Text = "Invoice";
            this.Load += new System.EventHandler(this.Invoice_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private TX.Framework.WindowUI.Controls.TXButton txButton7;
        private TX.Framework.WindowUI.Controls.TXButton txButton1;
        private TX.Framework.WindowUI.Controls.TXButton txButton2;
        private TX.Framework.WindowUI.Controls.TXButton txButton3;
        private System.Windows.Forms.TextBox textBox1;
    }
}