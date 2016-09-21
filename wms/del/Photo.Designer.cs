namespace UniversalAnalyse
{
    partial class Photo
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
            this.button1 = new System.Windows.Forms.Button();
            this.button31 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button32 = new System.Windows.Forms.Button();
            this.button33 = new System.Windows.Forms.Button();
            this.button34 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(28, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(247, 44);
            this.button1.TabIndex = 0;
            this.button1.Text = "抓相册主界面";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button31
            // 
            this.button31.ForeColor = System.Drawing.Color.Fuchsia;
            this.button31.Location = new System.Drawing.Point(28, 62);
            this.button31.Name = "button31";
            this.button31.Size = new System.Drawing.Size(247, 44);
            this.button31.TabIndex = 30;
            this.button31.Text = "分拆出各个子相册Split:do Block";
            this.button31.UseVisualStyleBackColor = true;
            this.button31.Click += new System.EventHandler(this.button31_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(28, 112);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(247, 44);
            this.button2.TabIndex = 31;
            this.button2.Text = "抓子相册html";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button32
            // 
            this.button32.ForeColor = System.Drawing.Color.Green;
            this.button32.Location = new System.Drawing.Point(28, 162);
            this.button32.Name = "button32";
            this.button32.Size = new System.Drawing.Size(247, 44);
            this.button32.TabIndex = 32;
            this.button32.Text = "抓子相册主题分拆出图片名 Loop Get Sigle";
            this.button32.UseVisualStyleBackColor = true;
            this.button32.Click += new System.EventHandler(this.button32_Click);
            // 
            // button33
            // 
            this.button33.ForeColor = System.Drawing.Color.Fuchsia;
            this.button33.Location = new System.Drawing.Point(28, 212);
            this.button33.Name = "button33";
            this.button33.Size = new System.Drawing.Size(247, 44);
            this.button33.TabIndex = 33;
            this.button33.Text = "拆分出图片名Un Split";
            this.button33.UseVisualStyleBackColor = true;
            this.button33.Click += new System.EventHandler(this.button33_Click);
            // 
            // button34
            // 
            this.button34.ForeColor = System.Drawing.Color.Blue;
            this.button34.Location = new System.Drawing.Point(28, 309);
            this.button34.Name = "button34";
            this.button34.Size = new System.Drawing.Size(247, 44);
            this.button34.TabIndex = 34;
            this.button34.Text = "Dfile";
            this.button34.UseVisualStyleBackColor = true;
            this.button34.Click += new System.EventHandler(this.button34_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(28, 262);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(247, 41);
            this.button3.TabIndex = 35;
            this.button3.Text = "生成目录";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Photo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 443);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button34);
            this.Controls.Add(this.button33);
            this.Controls.Add(this.button32);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button31);
            this.Controls.Add(this.button1);
            this.Name = "Photo";
            this.Text = "Photo";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button31;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button32;
        private System.Windows.Forms.Button button33;
        private System.Windows.Forms.Button button34;
        private System.Windows.Forms.Button button3;
    }
}