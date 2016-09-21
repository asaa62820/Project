namespace wms
{
    partial class Invoice_new
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
            this.txButton3 = new TX.Framework.WindowUI.Controls.TXButton();
            this.txButton2 = new TX.Framework.WindowUI.Controls.TXButton();
            this.txButton1 = new TX.Framework.WindowUI.Controls.TXButton();
            this.txButton7 = new TX.Framework.WindowUI.Controls.TXButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // txButton3
            // 
            this.txButton3.Image = null;
            this.txButton3.Location = new System.Drawing.Point(334, 118);
            this.txButton3.Name = "txButton3";
            this.txButton3.Size = new System.Drawing.Size(178, 37);
            this.txButton3.TabIndex = 26;
            this.txButton3.Text = "OverStock Invoice";
            this.txButton3.UseVisualStyleBackColor = true;
            this.txButton3.Click += new System.EventHandler(this.txButton3_Click);
            // 
            // txButton2
            // 
            this.txButton2.Image = null;
            this.txButton2.Location = new System.Drawing.Point(536, 78);
            this.txButton2.Name = "txButton2";
            this.txButton2.Size = new System.Drawing.Size(209, 37);
            this.txButton2.TabIndex = 25;
            this.txButton2.Text = "3. Get Tracking Number";
            this.txButton2.UseVisualStyleBackColor = true;
            this.txButton2.Click += new System.EventHandler(this.txButton2_Click);
            // 
            // txButton1
            // 
            this.txButton1.Image = null;
            this.txButton1.Location = new System.Drawing.Point(334, 78);
            this.txButton1.Name = "txButton1";
            this.txButton1.Size = new System.Drawing.Size(178, 37);
            this.txButton1.TabIndex = 24;
            this.txButton1.Text = "2. Invoice";
            this.txButton1.UseVisualStyleBackColor = true;
            this.txButton1.Click += new System.EventHandler(this.txButton1_Click);
            // 
            // txButton7
            // 
            this.txButton7.Image = null;
            this.txButton7.Location = new System.Drawing.Point(124, 78);
            this.txButton7.Name = "txButton7";
            this.txButton7.Size = new System.Drawing.Size(193, 37);
            this.txButton7.TabIndex = 23;
            this.txButton7.Text = "1.Import All Site Order";
            this.txButton7.UseVisualStyleBackColor = true;
            this.txButton7.Click += new System.EventHandler(this.txButton7_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(124, 171);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(929, 299);
            this.dataGridView1.TabIndex = 22;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(124, 779);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(142, 36);
            this.button1.TabIndex = 28;
            this.button1.Text = "1.Import All Site Order";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(125, 478);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 24);
            this.label1.TabIndex = 29;
            this.label1.Text = ".";
            // 
            // Invoice_new
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1249, 768);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txButton3);
            this.Controls.Add(this.txButton2);
            this.Controls.Add(this.txButton1);
            this.Controls.Add(this.txButton7);
            this.Controls.Add(this.dataGridView1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "Invoice_new";
            this.Text = "Invoice_new";
            this.Load += new System.EventHandler(this.Invoice_new_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TX.Framework.WindowUI.Controls.TXButton txButton3;
        private TX.Framework.WindowUI.Controls.TXButton txButton2;
        private TX.Framework.WindowUI.Controls.TXButton txButton1;
        private TX.Framework.WindowUI.Controls.TXButton txButton7;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
    }
}