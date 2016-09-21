using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;


namespace UniversalAnalyse
{
    public partial class myproxy : Form
    {
        public myproxy()
        {
            InitializeComponent();
        }

        private void myproxy_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            stops = false;
            timer1.Start();
        }

        int count = 0;

        private IPEndPoint getIPEND(ref string f)
        {


            try
            {

                lock (this)
                {

                    if (count < this.listBox1.Items.Count)
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }


                    string ipandx = this.listBox1.Items[count].ToString();

                    f = ipandx;


                    return new IPEndPoint(IPAddress.Parse(getip(ipandx)), int.Parse(getprot(ipandx)));

                }
            }
            catch
            {

                if (this.listBox1.Items.Count > 0)
                {
                    this.listBox1.BeginInvoke(new System.EventHandler(UpdateUI2), f);
                }

                f = "";

                this.richTextBox1.BeginInvoke(new System.EventHandler(UpdateUI), Thread.CurrentThread.Name + "获取代理错误按默认实行\r\n");

                return new IPEndPoint(IPAddress.Parse("58.181.224.33"), 81);



            }


        }

        private string getip(string ipLIST)
        {
            int x = 0;

            for (int i = 0; i < ipLIST.Length; i++)
            {
                if (ipLIST[i] == ':' || ipLIST[i] == ' ')
                    x = i;

            }

            return ipLIST.Remove(x, ipLIST.Length - x);


        }

        private string getprot(string ipLIST)
        {
            int x = 0;

            for (int i = 0; i < ipLIST.Length; i++)
            {
                if (ipLIST[i] == ':' || ipLIST[i] == ' ')
                    x = i;
            }

            return ipLIST.Remove(0, x + 1);


        }

        private void UpdateUI2(object o, System.EventArgs e)
        {

            try
            {

                this.listBox1.Items.Remove(o.ToString());

                this.label4.Text = this.listBox1.Items.Count.ToString();

            }
            catch
            {

            }


        }

        private void UpdateUI(object o, System.EventArgs e)
        {
            this.richTextBox1.AppendText(o.ToString());

            if (o.ToString().IndexOf("成功") > 0)
            {
                conuok++;

                this.label5.Text = conuok.ToString() + "连接成功";
            }

        }


        private void UpdateUI3(object o, System.EventArgs e)
        {
            this.richTextBox2.AppendText(o.ToString());


        }

        private int conuok = 0;


        private void gii()
        {
            TcpClient a = new TcpClient();



            string http = "CONNECT " + this.textBox1.Text + " HTTP/1.1 \r\n" +
 "Accept: */*\r\n" +
 "Content-Type: text/html\r\n" +
 "Proxy-Connection: Keep-Alive\r\n" +
 "Content-length: 0\r\n\r\n\r\n";

            byte[] kao = Encoding.ASCII.GetBytes(http);



            string f = "";

            IPEndPoint newsx = getIPEND(ref f);



            try
            {

                a.Connect(newsx);


            }
            catch
            {
                if (f != "")
                {
                    if (this.listBox1.Items.Count > 0)
                    {

                        this.listBox1.BeginInvoke(new System.EventHandler(UpdateUI2), f);

                    }

                }

                this.richTextBox1.BeginInvoke(new System.EventHandler(UpdateUI), Thread.CurrentThread.Name + "代理无法连接正在重新获取代理\r\n");




                a.Close();
                GC.Collect();
                Thread.CurrentThread.Abort();




            }


            NetworkStream off = a.GetStream();


            try
            {


                off.Write(kao, 0, kao.Length);

                byte[] classx = new byte[1024];

                off.Read(classx, 0, classx.Length);

                string fc = Encoding.ASCII.GetString(classx);

                fc = fc.Substring(0, fc.IndexOf("\r\n"));

                if (fc.IndexOf("established") < 0)
                {

                    off.Close();
                    a.Close();
                    GC.Collect();

                    if (f != "")
                    {
                        if (this.listBox1.Items.Count > 0)
                        {

                            this.listBox1.BeginInvoke(new System.EventHandler(UpdateUI2), f);

                        }

                    }

                    this.richTextBox1.BeginInvoke(new System.EventHandler(UpdateUI), Thread.CurrentThread.Name + "连接代理错误\r\n");


                    Thread.CurrentThread.Abort();

                    return;

                }
                else
                {

                    this.richTextBox1.BeginInvoke(new System.EventHandler(UpdateUI), Thread.CurrentThread.Name + "连接代理成功\r\n");

                    this.richTextBox2.BeginInvoke(new System.EventHandler(UpdateUI3), f + "\r\n");


                }
            }
            catch
            {

            }





            while (true)
            {
                if (stops == true)
                {
                    off.Close();
                    a.Close();
                    GC.Collect();
                    Thread.CurrentThread.Abort();
                }


                Thread.Sleep(1500);

                string bii = "#1<<<<<IDC<<<<<<<<=RaeVRae<<<<<<<BHODoHODo<<<<<<M`UBM`<<<<<<<<<<<<<<<<<<<<<<trIO<mH?@iHOLqIO@mHL<<<<<<<<<<<<<<<<<<<<]tZC]tZC]tZ<<<<<<<<<<<<<<<<<atZC]tZC]tZC\\<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<>c]tZC]tZC]tZC\\<<<<<<<<<<<<<>c]tZC]tZC]tZC\\<<<dmJO\\tGo@nGo@n<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<!";

                //  string bii = "1 219.255.135.8 8080 HTTP 韩国 ProxyCN 04-23 22:21 0.987 whois ";

                kao = Encoding.ASCII.GetBytes(bii);

                try
                {

                    off.Write(kao, 0, kao.Length);
                }
                catch
                {

                }

            }


        }

        private bool stops = false;

        private void button2_Click(object sender, EventArgs e)
        {
            stops = true;
            timer1.Stop();
        }


        private int max = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            max++;
            if (max < int.Parse(this.textBox4.Text))
            {


                Thread a = new Thread(new ThreadStart(gii));
                a.Name = max.ToString();
                a.Start();

            }

        }

        RichTextBox fii = new RichTextBox();

        private void button5_Click(object sender, EventArgs e)
        {
            this.openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "打开代理文件";

            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;

            openFileDialog1.AddExtension = true;

            openFileDialog1.Multiselect = false;

            openFileDialog1.Filter = "*.txt|*.txt|*.*|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {


                FileInfo f = new FileInfo(openFileDialog1.FileName);

                StreamReader ce = f.OpenText();

                fii.Text = ce.ReadToEnd();

                ce.Close();


                for (int i = 0; i < fii.Lines.Length; i++)
                {
                    this.listBox1.Items.Add(fii.Lines[i]);


                }

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Title = "保存代理文件";

            saveFileDialog1.OverwritePrompt = true;

            saveFileDialog1.CreatePrompt = true;

            saveFileDialog1.AddExtension = true;

            saveFileDialog1.Filter = "*.txt|*.txt|*.*|*.*";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.fii.Text = "";


                for (int i = 0; i < this.listBox1.Items.Count; i++)
                {


                    fii.AppendText(this.listBox1.Items[i].ToString() + "\n");


                }

                StreamWriter nn = new StreamWriter(saveFileDialog1.FileName);

                nn.Write(fii.Text);

                nn.Close();


            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.textBox2.Text != null)
            {
                this.listBox1.Items.Add(this.textBox2.Text);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem != null)
            {
                this.listBox1.Items.Remove(this.listBox1.SelectedItem);


            }
        }




    }
}
