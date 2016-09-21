using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TMS.Framework.Business.DataAccess;
using TMS.Framework.Publics.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace UniversalAnalyse
{
    public partial class 校验URL : Form
    {

        DBUtil DB = new DBUtil();

        MyEXCEL EE = new MyEXCEL();

        DataSet excelds = new DataSet();
        DataTable dtTableExtendedPropert;
        DataTable dtViewExtendedPropert;

        //获取所有表的字段信息
        DataTable FiledDt;



        public 校验URL()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //checkedListBox1.Items.Clear();

            dataGridView1.AutoGenerateColumns = false;

            DataGroup group = new DataGroup();
            group = null;

            //string ls_sql = "SELECT TOP 10 ID,URL,REGURL,'' as ShowNum,0 AS FLAG_Error,OPERATION,STATUS FROM CC03 WHERE STATUS IS NULL  AND TYPE='wpmu-pr2'  AND REGURL IS NOT NULL and class ='注册成功！'";

            string ls_sql = "SELECT TOP 10 ID,URL,REGURL,'' as ShowNum,0 AS FLAG_Error,OPERATION,STATUS FROM CC03 WHERE  TYPE='vbullets-pr3'  AND REGURL IS NOT NULL  and  (operation='0'  or operation='Y' ) ";

            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                dataGridView1.DataSource = group.Table;

                dataGridView1.Columns[0].DataPropertyName = "ID";
                dataGridView1.Columns[0].Width = 60;

                dataGridView1.Columns[1].DataPropertyName = "URL";
                dataGridView1.Columns[1].Width = 300;

                dataGridView1.Columns[2].DataPropertyName = "REGURL";
                dataGridView1.Columns[2].Width = 300;


                dataGridView1.Columns[3].DataPropertyName = "ShowNum";
                dataGridView1.Columns[3].Width = 60;


                dataGridView1.Columns[4].DataPropertyName = "FLAG_Error";
                dataGridView1.Columns[4].Width = 80;

            }


            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                dataGridView1.Rows[i].Cells["ShowNum"].Value = i + 1;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string ID = "";
            string x = "0";

            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                ID = dataGridView1.Rows[i].Cells["ID"].Value.ToString();
                x = dataGridView1.Rows[i].Cells["FLAG_Error"].Value.ToString();

                x = "2";

                string ls_sql = "update CC03 set operation='" + x + "',status=10,cdate=getdate()  where id='" + ID + "'";
                DB.ExecuteSQL(ls_sql);
            }

            MessageBox.Show("恭喜！请继续...");


        }

        private void button3_Click(object sender, EventArgs e)
        {

            Thread thread = new Thread(xxxxxxxxxxxxxx);
            thread.IsBackground = true;
            thread.Start();
        }


        public void xxxxxxxxxxxxxx()
        {
            //Process 用来实现访问外部程序,必须设置相应的FileName和Arguments属性
            Process myProcess = new Process();
            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {
                string myUrl = dataGridView1.Rows[i].Cells["REGURL"].Value.ToString().Trim();

                myProcess.StartInfo.FileName = "C:\\Program Files\\Mozilla Firefox\\firefox.exe";
                myProcess.StartInfo.Arguments = myUrl;
                myProcess.Start();

                System.Threading.Thread.Sleep(600);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /*[InternetShortcut]
            URL=http://www.kuqin.com/ruhe/20070813/257.html
            */


            DataGroup group = new DataGroup();
            group = null;
            string ls_sql = "SELECT URL,REGURL FROM CC03 WHERE (operation ='Y' or operation='0') AND TYPE='wpmu-pr2'  AND REGURL IS NOT NULL and class !='注册成功！'";
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                string md = "";

                for (int i = 0; i < group.Table.Rows.Count; i++)
                {


                    //1-10
                    if (i == 0 || i % 10 == 0)
                    {
                        md = "c:\\BookMarks\\wpmu-pr2人工注册\\";

                        md += (i + 1).ToString() + "-" + (i + 10).ToString();
                    }

                    if (!Directory.Exists(@md))//若文件夹不存在则新建文件夹  
                    {
                        Directory.CreateDirectory(@md); //新建文件夹  
                    }


                    string strPath = md + "\\" + (i + 1).ToString() + ".URL";
                    using (StreamWriter sw = new StreamWriter(strPath, false, Encoding.Unicode))
                    {
                        sw.Write("[InternetShortcut]\r\nURL=" + group.Table.Rows[i]["REGURL"].ToString());
                    }




                }
            }


            MessageBox.Show("恭喜！生成成功！");

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(pppppppppp);
            thread.IsBackground = true;
            thread.Start();
        }


        public void pppppppppp()
        {
            //Process 用来实现访问外部程序,必须设置相应的FileName和Arguments属性
            Process myProcess = new Process();

            string ls_url = l_html.Text.Replace("\r\n", "|");
            string[] dog_small = ls_url.Split('|');
            foreach (string bb in dog_small)
            {
                string xx = bb.Trim();

                if (xx.Length > 5)
                {

                    //C:\Program Files (x86)\Mozilla Firefox\firefox.exe
                    myProcess.StartInfo.FileName = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe";
                    myProcess.StartInfo.Arguments = xx;
                    myProcess.Start();

                    System.Threading.Thread.Sleep(1000);
                }

            }


        }



        #region 固定模版裁剪并缩放
        /// <summary>
        /// 上传图片(以Post方式获取源文件)
        /// 按模版比例最大范围的裁剪图片并缩放至模版尺寸
        /// </summary>
        /// <param name="postedFile">原图HttpPostedFile对象</param>
        /// <param name="templateWidth">模版宽(单位:px)</param>
        /// <param name="templateHeight">模版高(单位:px)</param>
        /// <param name="fileSaveUrl">保存路径</param>
        public static void UploadImage(string postedFile, int templateWidth, int templateHeight, string fileSaveUrl)
        {
            //从文件获取原始图片，并使用流中嵌入的颜色管理信息
            System.Drawing.Image initImage = System.Drawing.Image.FromFile(postedFile, true);//(postedFile.InputStream, true);

            //原图宽高均小于模版，不作处理，直接保存
            if (initImage.Width <= templateWidth && initImage.Height <= templateHeight)
            {
                initImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            else
            {
                //模版的宽高比例
                double templateRate = double.Parse(templateWidth.ToString()) / templateHeight;
                //原图片的宽高比例
                double initRate = double.Parse(initImage.Width.ToString()) / initImage.Height;

                //原图与模版比例相等，直接缩放
                if (templateRate == initRate)
                {
                    //按模版大小生成最终图片
                    System.Drawing.Image templateImage = new System.Drawing.Bitmap(templateWidth, templateHeight);
                    System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                    templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    templateG.Clear(Color.White);
                    templateG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, templateWidth, templateHeight), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);
                    templateImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                //原图与模版比例不等，裁剪后缩放
                else
                {
                    //裁剪对象
                    System.Drawing.Image pickedImage = null;
                    System.Drawing.Graphics pickedG = null;

                    //定位
                    Rectangle fromR = new Rectangle(0, 0, 0, 0);//原图裁剪定位
                    Rectangle toR = new Rectangle(0, 0, 0, 0);//目标定位

                    //宽为标准进行裁剪
                    if (templateRate > initRate)
                    {
                        //裁剪对象实例化
                        pickedImage = new System.Drawing.Bitmap(initImage.Width, int.Parse(Math.Floor(initImage.Width / templateRate).ToString()));
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                        //裁剪源定位
                        fromR.X = 0;
                        fromR.Y = int.Parse(Math.Floor((initImage.Height - initImage.Width / templateRate) / 2).ToString());
                        fromR.Width = initImage.Width;
                        fromR.Height = int.Parse(Math.Floor(initImage.Width / templateRate).ToString());

                        //裁剪目标定位
                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = initImage.Width;
                        toR.Height = int.Parse(Math.Floor(initImage.Width / templateRate).ToString());
                    }
                    //高为标准进行裁剪
                    else
                    {
                        pickedImage = new System.Drawing.Bitmap(int.Parse(Math.Floor(initImage.Height * templateRate).ToString()), initImage.Height);
                        pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                        fromR.X = int.Parse(Math.Floor((initImage.Width - initImage.Height * templateRate) / 2).ToString());
                        fromR.Y = 0;
                        fromR.Width = int.Parse(Math.Floor(initImage.Height * templateRate).ToString());
                        fromR.Height = initImage.Height;

                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = int.Parse(Math.Floor(initImage.Height * templateRate).ToString());
                        toR.Height = initImage.Height;
                    }

                    //设置质量
                    pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    //裁剪
                    pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);

                    //按模版大小生成最终图片
                    System.Drawing.Image templateImage = new System.Drawing.Bitmap(templateWidth, templateHeight);
                    System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                    templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    templateG.Clear(Color.White);
                    templateG.DrawImage(pickedImage, new System.Drawing.Rectangle(0, 0, templateWidth, templateHeight), new System.Drawing.Rectangle(0, 0, pickedImage.Width, pickedImage.Height), System.Drawing.GraphicsUnit.Pixel);
                    templateImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);

                    //释放资源
                    templateG.Dispose();
                    templateImage.Dispose();

                    pickedG.Dispose();
                    pickedImage.Dispose();
                }
            }

            //释放资源
            initImage.Dispose();
        }
        #endregion


        private void button7_Click(object sender, EventArgs e)
        {
            System.Drawing.Image initImage = System.Drawing.Image.FromFile("d:\\test\\13790.jpg", true);//(postedFile.InputStream, true);

            Bitmap bmpOut = new Bitmap(350, 310, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmpOut);
            g.DrawImage(initImage, new Rectangle(0, 0, 350, 310), new Rectangle(125, 120, 350, 310), GraphicsUnit.Pixel);
            bmpOut.Save("d:\\test\\2-2.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            System.Drawing.Image initImage = System.Drawing.Image.FromFile("d:\\test\\13787.jpg", true);//(postedFile.InputStream, true);

            Bitmap bmpOut = new Bitmap(350, 310, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmpOut);
            g.DrawImage(initImage, new Rectangle(0, 0, 350, 310), new Rectangle(125, 60, 350, 310), GraphicsUnit.Pixel);
            bmpOut.Save("d:\\test\\6-6.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private void button8_Click(object sender, EventArgs e)
        {


            System.Drawing.Image initImage = System.Drawing.Image.FromFile("d:\\test\\1.jpg", true);//(postedFile.InputStream, true);
            pictureBox1.Image = initImage;




            label3.Text = "第1张,共5张";


        }

        #region 固定模版裁剪并缩放
        /// <summary>
        /// 上传图片(以Post方式获取源文件)
        /// 按模版比例最大范围的裁剪图片并缩放至模版尺寸
        /// </summary>
        /// <param name="postedFile">原图HttpPostedFile对象</param>
        /// <param name="templateWidth">模版宽(单位:px)</param>
        /// <param name="templateHeight">模版高(单位:px)</param>
        /// <param name="fileSaveUrl">保存路径</param>
        public static void UploadImage2(string postedFile, int templateWidth, int templateHeight, string fileSaveUrl, int StartX, int StartY)
        {
            //从文件获取原始图片，并使用流中嵌入的颜色管理信息
            System.Drawing.Image initImage = System.Drawing.Image.FromFile(postedFile, true);//(postedFile.InputStream, true);


            //按模版大小生成最终图片
            System.Drawing.Image templateImage = new System.Drawing.Bitmap(templateWidth, templateHeight);
            System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
            templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            templateG.Clear(Color.White);
            templateG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, templateWidth, templateHeight), new System.Drawing.Rectangle(StartX, StartY, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);
            templateImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);



            //释放资源
            initImage.Dispose();
        }
        #endregion



        private Point m_ptStart = new Point(0, 0);
        private Point m_ptEnd = new Point(0, 0);
        // true: MouseUp or false: MouseMove 
        private bool m_bMouseDown = false;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (!m_bMouseDown)
            {
                m_ptStart = new Point(e.X, e.Y);
                m_ptEnd = new Point(e.X, e.Y);
            }
            m_bMouseDown = !m_bMouseDown;


        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (m_ptStart.X >= 0 && m_ptEnd.X >= 0
                 && m_ptStart.Y >= 0 && m_ptEnd.Y >= 0
                 && m_ptStart.X <= 254 && m_ptEnd.X <= 254
                 && m_ptStart.Y <= 163 && m_ptEnd.Y <= 163)
            {
                m_ptEnd = new Point(e.X, e.Y);
                m_bMouseDown = !m_bMouseDown;
                // this.pictureBox1.Refresh();
            }
            else
            {
                m_ptEnd = m_ptStart;
                m_bMouseDown = !m_bMouseDown;
                // this.pictureBox1.Refresh();
            }


        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (m_ptStart.Equals(m_ptEnd)) return;

            //e.Graphics.DrawLine(System.Drawing.Pens.Red, m_ptStart, m_ptEnd);

            //画矩形加上以下六行 
            //if (m_ptEnd.X - m_ptStart.X < 0 || m_ptEnd.Y - m_ptStart.Y < 0)
            // {
            //     return;
            //}

            e.Graphics.DrawRectangle(System.Drawing.Pens.Blue, m_ptStart.X, m_ptStart.Y, m_ptEnd.X - m_ptStart.X, m_ptEnd.Y - m_ptStart.Y);


        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            m_ptEnd = new Point(e.X, e.Y);
            this.pictureBox1.Refresh();

        }

        private void button9_Click(object sender, EventArgs e)
        {
            l_html.Text = "";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            OFDScript.Title = "数据库概要设计文档模板";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            try
            {

                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {
                    //MyEXCEL.数据库概要设计文档(dtTableExtendedPropert, OFDScript.FileName, 概要设计字段DT());//FiledDt);

                    MyEXCEL.f_barcode(OFDScript.FileName);
                    
                    

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            OFDScript.Title = "数据库概要设计文档模板";
            OFDScript.FileName = "";
            //为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
            OFDScript.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OFDScript.Filter = "EXCEL文件(*.xls)|*.xls";
            OFDScript.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            OFDScript.CheckFileExists = true;  //验证路径有效性
            OFDScript.CheckPathExists = true; //验证文件有效性
            //try
            //{

                //生成功能外部设计文档
                if (OFDScript.ShowDialog() == DialogResult.OK)
                {

                    Excel.Application app = new Excel.ApplicationClass();
                    Excel.Workbook workBook = app.Workbooks.Open(OFDScript.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    Excel.Worksheet ws_bc1 = (Excel.Worksheet)workBook.Sheets[1];
                    app.Visible = true;
                    ws_bc1.Select(Type.Missing);


                    //复制模板区域
                    int rowIndex = 0;
                    while (rowIndex < 17)
                    {
                        指定位置插入行(ws_bc1.get_Range("A18", Type.Missing));
                        rowIndex++;
                    }
                    Excel.Range range1 = ws_bc1.get_Range("A1:M17", Type.Missing);
                    Excel.Range range = ws_bc1.get_Range("A18:M34", Type.Missing);
                    复制单元格(range1, range);


                    string ls_sql = "select id,barcode,style,color,size,qty,ln from t_url_barcode";
                    DataGroup group_INV3 = new DataGroup();
                    group_INV3 = null;
                    group_INV3=DB.GetDataGroup(ls_sql);
                    int bc_qty = 0;

                    int i_base_id = 18;
                    if (group_INV3.Table.Rows.Count > 0)
                    {
                        for (int INV3 = 0; INV3 < group_INV3.Table.Rows.Count; INV3++)
                        {
                        string ls_size = group_INV3.Table.Rows[INV3]["size"].ToString();

                        string ls_code = "*" + group_INV3.Table.Rows[INV3]["barcode"].ToString()+"*";
                        string ls_code_num = group_INV3.Table.Rows[INV3]["barcode"].ToString()+"\n "+ ls_size;
                        string ls_lab = group_INV3.Table.Rows[INV3]["style"].ToString().Replace("THIGH", "").Trim() + " " + group_INV3.Table.Rows[INV3]["color"].ToString();
                        ls_size = "";
                        string ls_ln = group_INV3.Table.Rows[INV3]["ln"].ToString();
                        ls_lab = ls_lab.Replace("  "," ").Trim();
                        string ls_qty = group_INV3.Table.Rows[INV3]["qty"].ToString();
                            int i_qty = int.Parse(ls_qty);
                            for (int i_xx = 0; i_xx < i_qty; i_xx++)
                            {
                                bc_qty++;

                                int x_mod = bc_qty % 32;
                                switch (x_mod)
                                {
                                    /*ABAB*/
                                    case 1:
                                        ws_bc1.get_Range("A" + (i_base_id + 0), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("B" + (i_base_id + 0), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("A" + (i_base_id + 1), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("B" + (i_base_id + 1), Type.Missing).Value2 = ls_size;

                                        ws_bc1.get_Range("B" + (i_base_id + 0), "B" + (i_base_id + 1)).Merge(false);
                                    //worksheet.Rows[1].Cells[7].Style.Font.Size = 20;
                                    //worksheet.get_Range("A7", "A7").Style.Font.Size = 20;
                                    //xlWorkSheet.get_Range("b2", "e3").Merge(false);

                                    ws_bc1.get_Range("A" + (i_base_id + 8), Type.Missing).Value2 = ls_ln;
                                        break;
                                    case 2:
                                        ws_bc1.get_Range("A" + (i_base_id + 2), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("B" + (i_base_id + 2), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("A" + (i_base_id + 3), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("B" + (i_base_id + 3), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("B" + (i_base_id + 2), "B" + (i_base_id + 3)).Merge(false);
                                    break;
                                    case 3:
                                        ws_bc1.get_Range("A" + (i_base_id + 4), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("B" + (i_base_id + 4), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("A" + (i_base_id + 5), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("B" + (i_base_id + 5), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("B" + (i_base_id + 4), "B" + (i_base_id + 5)).Merge(false);
                                    break;
                                    case 4:
                                        ws_bc1.get_Range("A" + (i_base_id + 6), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("B" + (i_base_id + 6), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("A" + (i_base_id + 7), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("B" + (i_base_id + 7), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("B" + (i_base_id + 6), "B" + (i_base_id + 7)).Merge(false);
                                    break;
                                    case 5:
                                        ws_bc1.get_Range("A" + (i_base_id + 9), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("B" + (i_base_id + 9), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("A" + (i_base_id + 10), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("B" + (i_base_id + 10), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("B" + (i_base_id + 9), "B" + (i_base_id + 10)).Merge(false);
                                    break;
                                    case 6:
                                        ws_bc1.get_Range("A" + (i_base_id + 11), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("B" + (i_base_id + 11), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("A" + (i_base_id + 12), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("B" + (i_base_id + 12), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("B" + (i_base_id + 11), "B" + (i_base_id + 12)).Merge(false);
                                    break;
                                    case 7:
                                        ws_bc1.get_Range("A" + (i_base_id + 13), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("B" + (i_base_id + 13), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("A" + (i_base_id + 14), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("B" + (i_base_id + 14), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("B" + (i_base_id + 13), "B" + (i_base_id + 14)).Merge(false);
                                    break;
                                    case 8:
                                        ws_bc1.get_Range("A" + (i_base_id + 15), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("B" + (i_base_id + 15), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("A" + (i_base_id + 16), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("B" + (i_base_id + 16), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("B" + (i_base_id + 15), "B" + (i_base_id + 16)).Merge(false);
                                    break;

                                    /*EFEF*/
                                    case 9:
                                        ws_bc1.get_Range("E" + (i_base_id + 0), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("F" + (i_base_id + 0), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("E" + (i_base_id + 1), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("F" + (i_base_id + 1), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("F" + (i_base_id + 0), "F" + (i_base_id + 1)).Merge(false);
                                    break;
                                    case 10:
                                        ws_bc1.get_Range("E" + (i_base_id + 2), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("F" + (i_base_id + 2), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("E" + (i_base_id + 3), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("F" + (i_base_id + 3), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("F" + (i_base_id + 2), "F" + (i_base_id + 3)).Merge(false);
                                    break;
                                    case 11:
                                        ws_bc1.get_Range("E" + (i_base_id + 4), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("F" + (i_base_id + 4), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("E" + (i_base_id + 5), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("F" + (i_base_id + 5), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("F" + (i_base_id + 4), "F" + (i_base_id + 5)).Merge(false);
                                    break;
                                    case 12:
                                        ws_bc1.get_Range("E" + (i_base_id + 6), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("F" + (i_base_id + 6), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("E" + (i_base_id + 7), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("F" + (i_base_id + 7), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("F" + (i_base_id + 6), "F" + (i_base_id + 7)).Merge(false);
                                    break;
                                    case 13:
                                        ws_bc1.get_Range("E" + (i_base_id + 9), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("F" + (i_base_id + 9), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("E" + (i_base_id + 10), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("F" + (i_base_id + 10), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("F" + (i_base_id + 9), "F" + (i_base_id + 10)).Merge(false);
                                    break;
                                    case 14:
                                        ws_bc1.get_Range("E" + (i_base_id + 11), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("F" + (i_base_id + 11), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("E" + (i_base_id + 12), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("F" + (i_base_id + 12), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("F" + (i_base_id + 11), "F" + (i_base_id + 12)).Merge(false);
                                    break;
                                    case 15:
                                        ws_bc1.get_Range("E" + (i_base_id + 13), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("F" + (i_base_id + 13), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("E" + (i_base_id + 14), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("F" + (i_base_id + 14), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("F" + (i_base_id + 13), "F" + (i_base_id + 14)).Merge(false);
                                    break;
                                    case 16:
                                        ws_bc1.get_Range("E" + (i_base_id + 15), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("F" + (i_base_id + 15), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("E" + (i_base_id + 16), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("F" + (i_base_id + 16), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("F" + (i_base_id + 15), "F" + (i_base_id + 16)).Merge(false);
                                    break;
                                    /*HIHI*/
                                    case 17:
                                        ws_bc1.get_Range("H" + (i_base_id + 0), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("I" + (i_base_id + 0), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("H" + (i_base_id + 1), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("I" + (i_base_id + 1), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("I" + (i_base_id + 0), "I" + (i_base_id + 1)).Merge(false);
                                    break;
                                    case 18:
                                        ws_bc1.get_Range("H" + (i_base_id + 2), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("I" + (i_base_id + 2), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("H" + (i_base_id + 3), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("I" + (i_base_id + 3), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("I" + (i_base_id + 2), "I" + (i_base_id + 3)).Merge(false);
                                    break;
                                    case 19:
                                        ws_bc1.get_Range("H" + (i_base_id + 4), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("I" + (i_base_id + 4), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("H" + (i_base_id + 5), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("I" + (i_base_id + 5), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("I" + (i_base_id + 4), "I" + (i_base_id + 5)).Merge(false);
                                    break;
                                    case 20:
                                        ws_bc1.get_Range("H" + (i_base_id + 6), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("I" + (i_base_id + 6), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("H" + (i_base_id + 7), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("I" + (i_base_id + 7), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("I" + (i_base_id + 6), "I" + (i_base_id + 7)).Merge(false);
                                    break;
                                    case 21:
                                        ws_bc1.get_Range("H" + (i_base_id + 9), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("I" + (i_base_id + 9), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("H" + (i_base_id + 10), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("I" + (i_base_id + 10), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("I" + (i_base_id + 9), "I" + (i_base_id + 10)).Merge(false);
                                    break;
                                    case 22:
                                        ws_bc1.get_Range("H" + (i_base_id + 11), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("I" + (i_base_id + 11), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("H" + (i_base_id + 12), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("I" + (i_base_id + 12), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("I" + (i_base_id + 11), "I" + (i_base_id + 12)).Merge(false);
                                    break;
                                    case 23:
                                        ws_bc1.get_Range("H" + (i_base_id + 13), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("I" + (i_base_id + 13), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("H" + (i_base_id + 14), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("I" + (i_base_id + 14), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("I" + (i_base_id + 13), "I" + (i_base_id + 14)).Merge(false);
                                    break;
                                    case 24:
                                        ws_bc1.get_Range("H" + (i_base_id + 15), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("I" + (i_base_id + 15), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("H" + (i_base_id + 16), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("I" + (i_base_id + 16), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("I" + (i_base_id + 15), "I" + (i_base_id + 16)).Merge(false);
                                    break;
                                    /*LMLM*/
                                    case 25:
                                        ws_bc1.get_Range("L" + (i_base_id + 0), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("M" + (i_base_id + 0), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("L" + (i_base_id + 1), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("M" + (i_base_id + 1), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("M" + (i_base_id + 0), "M" + (i_base_id + 1)).Merge(false);
                                    break;
                                    case 26:
                                        ws_bc1.get_Range("L" + (i_base_id + 2), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("M" + (i_base_id + 2), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("L" + (i_base_id + 3), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("M" + (i_base_id + 3), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("M" + (i_base_id + 2), "M" + (i_base_id + 3)).Merge(false);
                                    break;
                                    case 27:
                                        ws_bc1.get_Range("L" + (i_base_id + 4), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("M" + (i_base_id + 4), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("L" + (i_base_id + 5), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("M" + (i_base_id + 5), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("M" + (i_base_id + 4), "M" + (i_base_id + 5)).Merge(false);
                                    break;
                                    case 28:
                                        ws_bc1.get_Range("L" + (i_base_id + 6), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("M" + (i_base_id + 6), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("L" + (i_base_id + 7), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("M" + (i_base_id + 7), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("M" + (i_base_id + 6), "M" + (i_base_id + 7)).Merge(false);
                                    break;
                                    case 29:
                                        ws_bc1.get_Range("L" + (i_base_id + 9), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("M" + (i_base_id + 9), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("L" + (i_base_id + 10), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("M" + (i_base_id + 10), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("M" + (i_base_id + 9), "M" + (i_base_id + 10)).Merge(false);
                                    break;
                                    case 30:
                                        ws_bc1.get_Range("L" + (i_base_id + 11), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("M" + (i_base_id + 11), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("L" + (i_base_id + 12), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("M" + (i_base_id + 12), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("M" + (i_base_id + 11), "M" + (i_base_id + 12)).Merge(false);
                                    break;
                                    case 31:
                                        ws_bc1.get_Range("L" + (i_base_id + 13), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("M" + (i_base_id + 13), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("L" + (i_base_id + 14), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("M" + (i_base_id + 14), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("M" + (i_base_id + 13), "M" + (i_base_id + 14)).Merge(false);
                                    break;
                                    case 0:
                                        ws_bc1.get_Range("L" + (i_base_id + 15), Type.Missing).Value2 = ls_code;
                                        ws_bc1.get_Range("M" + (i_base_id + 15), Type.Missing).Value2 = ls_code_num;
                                        ws_bc1.get_Range("L" + (i_base_id + 16), Type.Missing).Value2 = ls_lab;
                                        ws_bc1.get_Range("M" + (i_base_id + 16), Type.Missing).Value2 = ls_size;
                                        ws_bc1.get_Range("M" + (i_base_id + 15), "M" + (i_base_id + 16)).Merge(false);

                                        //复制模板区域
                                        rowIndex = 0;
                                        while (rowIndex < 17)
                                        {
                                            指定位置插入行(ws_bc1.get_Range("A18", Type.Missing));
                                            rowIndex++;
                                        }

                                        Excel.Range range2 = ws_bc1.get_Range("A1:M17", Type.Missing);
                                        Excel.Range range3 = ws_bc1.get_Range("A18:M34", Type.Missing);
                                        复制单元格(range2, range3);

                                        break;


                                }
                            }
                        }




                    }
                }
                    

                    


                
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message.ToString());
            //}
        }


        public static void 指定位置插入行(Excel.Range range)
        {
            range.Select();
            range.EntireRow.Insert(Type.Missing, Type.Missing);
        }
        private static void 复制单元格(Excel.Range range, Excel.Range range1)
        {
            range.Copy(Type.Missing);
            range1.PasteSpecial(Excel.XlPasteType.xlPasteAll, Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, System.Type.Missing, System.Type.Missing);
        }




        private DataTable 概要设计字段DT()
        {
            System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
            string connstr = configurationAppSettings.GetValue("universal_analyse_connstr", typeof(string)).ToString();

            OleDbConnection myConn = new OleDbConnection(connstr);

            OleDbCommand cmd = new OleDbCommand(MyEXCEL.获取所有FK(), myConn);
            OleDbDataAdapter oda = new OleDbDataAdapter(cmd);
            DataTable FKDt = new DataTable();
            oda.Fill(FKDt);

            cmd = new OleDbCommand(MyEXCEL.获取所有索引(), myConn);
            oda = new OleDbDataAdapter(cmd);
            DataTable indexDt = new DataTable();
            oda.Fill(indexDt);

            DataTable TepDt = FiledDt.Copy();
            TepDt.Columns.Add("fk_name");//fk名称
            TepDt.Columns.Add("fk_r_name");//关联表名
            TepDt.Columns.Add("fk_r_c_name");//关联字段

            string tabname = string.Empty;
            string colname = string.Empty;

            DataRow[] tepIndexDr = null;
            DataRow[] tepFkDr = null;

            for (int i = 0; i < TepDt.Rows.Count; i++)
            {
                colname = TepDt.Rows[i][3].ToString().Trim();
                if (TepDt.Rows[i][0].ToString().Trim() != "")
                {
                    tabname = TepDt.Rows[i][0].ToString();
                    tepIndexDr = indexDt.Select("i_t_name='" + tabname + "'");
                    tepFkDr = FKDt.Select("fk_t_name='" + tabname + "'");
                }
                if (tepIndexDr != null)
                {
                    if (tepIndexDr.Length > 0)
                    {
                        for (int indexi = 0; indexi < tepIndexDr.Length; indexi++)
                        {
                            if (tepIndexDr[indexi]["i_c_name"].ToString().Trim() == colname)
                            {
                                TepDt.Rows[i]["IndexName"] = tepIndexDr[indexi][0].ToString();
                            }
                        }
                    }
                }

                if (tepFkDr != null)
                {
                    if (tepFkDr.Length > 0)
                    {
                        for (int indexf = 0; indexf < tepFkDr.Length; indexf++)
                        {
                            if (tepFkDr[indexf]["fk_t_c_name"].ToString().Trim() == colname)
                            {
                                TepDt.Rows[i]["fk_name"] = tepFkDr[indexf][0].ToString();
                                TepDt.Rows[i]["fk_r_name"] = tepFkDr[indexf][2].ToString();
                                TepDt.Rows[i]["fk_r_c_name"] = tepFkDr[indexf][4].ToString();
                            }
                        }
                    }
                }
            }

            return TepDt;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            /*
CREATE TABLE [dbo].T_URL_BARCODE(
[ID] [bigint] IDENTITY(1,1) NOT NULL,
[BARCODE] [nvarchar](50) NULL,
[STYLE] [nvarchar](50) NULL,
[COLOR] [nvarchar](20) NULL,
[QTY] [BIGINT] NULL,
[PSKU] [nvarchar](50) NULL,
[LN] [nvarchar](50) NULL,
[SIZE] [nvarchar](10) NULL,
[STATUS] [nvarchar](1) NULL,
CONSTRAINT [PK_T_URL_BARCODE] PRIMARY KEY CLUSTERED 
([ID] ASC)) ON [PRIMARY]
*/
            string LS_FULL_INV = @"
select ln,[style no] as style, color,[P-SKU] AS PSKU, sum([q1]) as q1,sum([q2]) as q2,sum([q3]) as q3 ,sum([q4]) as q4,sum([q5]) as q5,sum([q55]) as q5d5,
sum([q6]) as q6,sum([q65]) as q6d5,sum([q7]) as q7,sum([q75]) as q7d5,sum([q8]) as q8 ,sum([q85]) as q8d5,sum([q9]) as q9,
sum([q10]) as q10,sum([q11]) as q11 ,sum([q12]) as q12 ,sum([q13]) as q13
from [dbo].[T_URL_INV3] where ln like '%o%' and ln not like '%oe%' AND [P-SKU] !='X'
group by ln,[style no],color,[P-SKU] order by ln,[style no],color,[P-SKU]
";
            string ls_ddd = "";
            int i = 0;
            int j = 0;

            DataGroup group_INV3 = new DataGroup();
            group_INV3 = null;
            group_INV3 = DB.GetDataGroup(LS_FULL_INV);
            if (group_INV3.Table.Rows.Count > 0)
            {
                for (int INV3 = 0; INV3 < group_INV3.Table.Rows.Count; INV3++)
                {

                    for (int XXX = 4; XXX < 21; XXX++)
                    {

                        string ls_p_sku_size = group_INV3.Table.Rows[INV3][XXX].ToString();

                        i++;

                        if (ls_p_sku_size != "0")
                        {
                            j++;
                            string ls_style_no = group_INV3.Table.Rows[INV3]["style"].ToString();
                            string ls_color = group_INV3.Table.Rows[INV3]["color"].ToString();
                            string ls_p_sku_size_title = group_INV3.Table.Columns[XXX].ColumnName.Replace("q", "").Replace("d", ".");
                            string ls_p_sku = group_INV3.Table.Rows[INV3]["psku"].ToString() + ls_p_sku_size_title;
                            string ls_ln = group_INV3.Table.Rows[INV3]["ln"].ToString();

                            DB.ExecuteSQL(@"insert into T_URL_BARCODE (style,color,qty,psku,ln,size,status) values(
'" + ls_style_no + "','" + ls_color + "','" + ls_p_sku_size + "','" + ls_p_sku 
+ "','" + ls_ln + "','" + ls_p_sku_size_title + "','0');");

                        }


                    }
                }

                DB.ExecuteSQL("update t_url_barcode set barcode=right('00000' + cast(id as nvarchar(10)),5);");

            }


            MessageBox.Show("ok");
        }

       
    }
}