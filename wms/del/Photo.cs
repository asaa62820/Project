using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;
using System.Collections;
//using C1.C1Excel;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

using System.Net;
using TMS.Framework.Business.DataAccess;
using TMS.Framework.Publics.Data;

using System.Web;
using System.Threading;


using System.Diagnostics;

using System.Collections.Specialized;


using System.Globalization;
using System.Security.Cryptography;
using System.Timers;

namespace UniversalAnalyse
{
    public partial class Photo : Form
    {
        DBUtil DB = new DBUtil();

        public Photo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(xxxxxxxxxxxxxx);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;


            thread.Start();
        }


        public void xxxxxxxxxxxxxx()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;

            string ls_sql = "SELECT ID,/*url_product as */ URL FROM Photo_hongzilan_B1 /**/ WHERE STATUS =1  order by ID";//IS NULL
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = "";

                    pig = getUrlSource(group.Table.Rows[i]["URL"].ToString(), "gb2312").Replace("'", "\"");//utf-8  gb2312

                    if (pig != "")
                    {
                        string ls_up = "UPDATE Photo_hongzilan_B1 SET HTML='" + pig + "' ,STATUS=2 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);

                        pig = "";
                    }

                }
            }

            MessageBox.Show("ok!");


        }

        public string getUrlSource(string strUrl, string strEncoding)
        {
            string lsResult;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strUrl);
                HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(rep.GetResponseStream(), Encoding.GetEncoding(strEncoding));

                lsResult = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                lsResult = "";
                Console.WriteLine(ex.Message);
            }
            return lsResult;
        }

        private void button31_Click(object sender, EventArgs e)
        {

            DataGroup group = new DataGroup();

            DataGroup group_html = new DataGroup();
            group_html = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group_html = null;

            string ls_sql = "SELECT ID,HTML FROM Photo_hongzilan_B1 WHERE status=2 order by ID";
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["HTML"].ToString().Replace("\r\n", "").ToString();//替换换行符
                    ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”替换
                    ls_shtml = ls_shtml.Replace("<div><a href=", "|");//将分隔码 替换成“|”


                    string[] dog_small = ls_shtml.Split('|');//子串

                    int pd = 0;

                    foreach (string bb in dog_small)
                    {
                        if (pd > 0)
                        {

                            string ls_FILED = "";
                            string ls_Value = "";

                            //-----------------------------------------------------------------------------------------------------
                            group = null;
                            string ls_rule = "SELECT RID,FILED,OID,CYCLE,STRING_S,STRING_E,STRING_NEW,INCLUDE_S,INCLUDE_E FROM URL_RULE WHERE siteid=301 and STATUS=1";
                            group = DB.GetDataGroup(ls_rule);
                            if (group.Table.Rows.Count > 0)
                            {
                                for (int j = 0; j < group.Table.Rows.Count; j++)
                                {
                                    try
                                    {
                                        ls_Value += ",'" + OperateStr(bb, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, "0") + "'";
                                    }
                                    catch
                                    {
                                        ls_Value = "," + " ";
                                    }

                                    ls_FILED += "," + group.Table.Rows[j]["FILED"].ToString();


                                }

                                string ls_up = "INSERT INTO Photo_hongzilan_B2 (PID " + ls_FILED + ",STATUS) values ('" + group_html.Table.Rows[i]["ID"].ToString() + "' " + ls_Value + ",1)";
                                DB.ExecuteSQL(ls_up);

                                ls_Value = "";
                                ls_FILED = "";


                            }
                            group = null;
                            //-----------------------------------------------------------------------------------------------------
                        }
                        pd += 1;
                    }





                }

                MessageBox.Show("OK!!");
            }
        }


        /// <summary>
        /// 根据各个操作类型处理字符串 //1://截取  2://删除段  3://删除字符串  4://字符串前缀  5://段落前缀  6://字符串后缀  7://段落后缀  8://替换字符串  9://替换段落
        /// </summary>
        /// <param name="str">要处理的字符串</param>
        /// <param name="startstr">开始字符串</param>
        /// <param name="endstr">结束字符串</param>
        /// <param name="Newstr">替换/前缀/后缀的字符串</param>
        /// <param name="operate">操作类型</param>
        /// <param name="inHead">包含头1，不包含头0</param>
        /// <param name="inTail">包含尾1，不包含尾0</param>
        /// <returns>返回处理过后的字符串</returns>
        private string OperateStr(string str, string startstr, string endstr, string Newstr, int operate, int inHead, int inTail, string CYCLE)
        {
            int intLength = str.Length;//【HTML总长度】
            int startIndex = str.IndexOf(startstr);//【开始字符串】在【HTML】位置


            if (startIndex == -1)
            {
                return "";
            }

            //int endIndex = str.IndexOf(endstr);
            int x = startstr.Length;//【开始字符串】的长度
            string cat = str.Substring(0, startIndex + x);//从【开头】到【开始字符串】

            cat = str.Replace(cat, "");//【目标字符串】后到【尾巴】
            int end_len = cat.IndexOf(endstr);//【目标字符串】的长度

            int endIndex = cat.IndexOf(endstr);


            int KEY_INDEX = startIndex + startstr.Length + endIndex + endstr.Length;



            int CCC = 0;
            if (CYCLE == "1")
            {
                //Regex r = new Regex(startstr); // 定义一个Regex对象实例
                //Match m = r.Match(str); // 在字符串中匹配
                //if (m.Success)
                //{
                //    CCC = m.Length;
                //}

                CCC = (str.Length - str.Replace(startstr, String.Empty).Length) / startstr.Length;
            }




            switch (operate)
            {
                case 1://截取
                    //if (endIndex <= startIndex || startIndex == -1)
                    //    return "";

                    if (inHead == 1 && inTail == 1)//表示包含头尾,保证能找到尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex + endstr.Length);

                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        //return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length);

                        if (CCC == 0)
                        {
                            return str.Substring(startIndex + startstr.Length, end_len);
                        }
                        else
                        {
                            string wolf = "";
                            wolf = "" + str.Substring(startIndex + startstr.Length, end_len);

                            for (int i = 1; i < CCC; i++)
                            {
                                //------------重新定位【开始长度位】--------------
                                //--------------【KEY_INDEX】---------------------
                                //------------先替换掉处理的【HTML】--------------

                                //int temp_index = startIndex + startstr.Length + end_len + endstr.Length+337 ;

                                cat = str.Substring(0, KEY_INDEX);
                                cat = str.Replace(cat, "");//【新世纪】

                                startIndex = cat.IndexOf(startstr);

                                string mouse = cat.Substring(0, startIndex + startstr.Length);//从【开头】到【开始字符串】
                                mouse = cat.Replace(mouse, "");
                                endIndex = mouse.IndexOf(endstr);

                                wolf += "|" + cat.Substring(startIndex + startstr.Length, endIndex);

                                KEY_INDEX += startIndex + startstr.Length + endIndex + endstr.Length;
                            }

                            return wolf;
                        }

                    }
                    break;
                case 2://删除段
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), "");
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), "");
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), "");
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), "");
                    }
                    break;
                case 3://删除字符串
                    return str.Replace(startstr, "");
                    break;
                case 4://字符串前缀
                    return str.Replace(startstr, Newstr + startstr);
                    break;
                case 5://段落前缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;
                    if (inHead == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex));

                    if (inHead == 0)
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr + str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length));
                    break;
                case 6://字符串后缀
                    return str.Replace(startstr, startstr + Newstr);
                    break;
                case 7://段落后缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inTail == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), str.Substring(startIndex, endIndex - startIndex + endstr.Length) + Newstr);

                    if (inTail == 0)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex) + Newstr);
                    break;
                case 8://替换字符串
                    return str.Replace(startstr, Newstr);
                    break;
                case 9://替换段落
                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), Newstr);
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), Newstr);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr);
                    }

                    break;
            }
            return str;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(xxxxxxxxxxxxxx222);

            Thread.Sleep(2000);//等待200毫秒

            thread.IsBackground = true;


            thread.Start();
        }
        public void xxxxxxxxxxxxxx222()
        {
            DataGroup group = new DataGroup();
            group = null;
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group = null;

            string ls_sql = "SELECT ID,sub_photo_url as URL FROM Photo_hongzilan_B2 WHERE status=1  order by ID";//IS NULL
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    string pig = "";

                    pig = getUrlSource(group.Table.Rows[i]["URL"].ToString(), "utf-8").Replace("'", "\"");//utf-8  gb2312

                    if (pig != "")
                    {
                        string ls_up = "UPDATE Photo_hongzilan_B2  SET HTML='" + pig + "' ,STATUS=2 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                        DB.ExecuteSQL(ls_up);

                        pig = "";
                    }
                    else
                    {
                        MessageBox.Show("stop");
                    }

                }
            }

            MessageBox.Show("ok!");

        }

        private void button32_Click(object sender, EventArgs e)
        {
            try
            {
                DataGroup group = new DataGroup();

                DataGroup group_html = new DataGroup();
                group_html = null;
                DataEntity de = new DataEntity();
                de.RemoveAll();
                group_html = null;

                string ls_sql = "SELECT  ID,HTML FROM Photo_hongzilan_B2 WHERE STATUS =2  order by ID";
                group_html = DB.GetDataGroup(ls_sql);
                if (group_html.Table.Rows.Count > 0)
                {

                    for (int i = 0; i < group_html.Table.Rows.Count; i++)
                    {
                        string ls_shtml = group_html.Table.Rows[i]["HTML"].ToString().Replace("\r\n", "").ToString();//替换换行符
                        ls_shtml = ls_shtml.Replace("|", "");//原先的“|”替换
                        ls_shtml = ls_shtml.Replace("\"", "");//原先的“|”


                        //string ls_FILED = "";
                        string ls_Value = "";
                        //-----------------------------------------------------------------------------------------------------
                        group = null;
                        string ls_rule = "SELECT RID,FILED,OID,CYCLE,STRING_S,STRING_E,STRING_NEW,INCLUDE_S,INCLUDE_E,STR_AREA,STR_AFT,STR_BEF FROM URL_RULE WHERE siteid=302 and STATUS=1";
                        group = DB.GetDataGroup(ls_rule);
                        if (group.Table.Rows.Count > 0)
                        {
                            for (int j = 0; j < group.Table.Rows.Count; j++)
                            {
                                try
                                {

                                    ls_Value = OperateStr_Adv(ls_shtml, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, group.Table.Rows[j]["CYCLE"].ToString(), group.Table.Rows[j]["STR_AREA"].ToString(), group.Table.Rows[j]["STR_AFT"].ToString(), group.Table.Rows[j]["STR_BEF"].ToString());

                                    ls_Value = NoHTML(ls_Value).Trim();

                                    ls_Value = ls_Value.Replace("s128/", "");

                                    //ls_Value = OperateStr(ls_shtml, group.Table.Rows[j]["STRING_S"].ToString(), group.Table.Rows[j]["STRING_E"].ToString(), "", 1, 0, 0, group.Table.Rows[j]["CYCLE"].ToString());

                                }
                                catch
                                {
                                    ls_Value = "";
                                }

                                string ls_up = "UPDATE Photo_hongzilan_B2 SET " + group.Table.Rows[j]["FILED"].ToString() + "='" + ls_Value + "' ,STATUS=4 WHERE ID='" + group_html.Table.Rows[i]["ID"].ToString() + "'";

                                //string ls_up = "insert into pick_p2 (flag,pid,url_pic) values ('B','" + group_html.Table.Rows[i]["ID"].ToString() + "','" + ls_Value + "')";

                                DB.ExecuteSQL(ls_up);
                            }


                        }
                        ls_shtml = null;
                        group = null;
                        //-----------------------------------------------------------------------------------------------------


                    }

                }

            }
            catch
            { }

            MessageBox.Show("OK");
        }



        ///   <summary>
        ///   去除HTML标记
        ///   </summary>
        ///   <param   name="NoHTML">包括HTML的源码   </param>
        ///   <returns>已经去除后的文字</returns>  
        public static string NoHTML(string Htmlstring)
        {
            /*
            //删除脚本  
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML  
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
             * 
             * 
                html=Regex.Replace(html,@"\<a[^\>]*\>","");
                html=Regex.Replace(html,@"\</a\>","");
            */

            Htmlstring = Regex.Replace(Htmlstring, @"\<a[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</a\>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<FONT[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</FONT\>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<P[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</P\>", "", RegexOptions.IgnoreCase);


            Htmlstring = Regex.Replace(Htmlstring, @"\<h2[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</h2\>", "", RegexOptions.IgnoreCase);


            Htmlstring = Regex.Replace(Htmlstring, @"\<DIV[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</DIV\>", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"\<SPAN[^\>]*\>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"\</SPAN\>", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("? ", "");
            Htmlstring.Replace("?", "");

            Htmlstring.Replace("&nbsp;", "");



            return Htmlstring;
        }

        private string OperateStr_Adv(string str, string startstr, string endstr, string Newstr, int operate, int inHead, int inTail, string CYCLE, string STR_AREA, string STR_AFT, string STR_BEF)
        {
            //先截断字符----------------------------------------------------------------
            if (STR_AREA == "1")
            {
                str = str.Replace("\t", "");

                int temp_left = str.IndexOf(STR_AFT);

                if (temp_left == -1)
                {
                    return "";
                }
                temp_left += STR_AFT.Length;
                str = str.Replace(str.Substring(0, temp_left), "");

                int temp_right = str.IndexOf(STR_BEF);
                str = str.Substring(0, temp_right).Replace("\t", "");//////////////.Replace("</option>", "").Replace("<option ", "") + "value=";

            }
            //--------------------------------------------------------------------------


            int intLength = str.Length;//【HTML总长度】
            int startIndex = str.IndexOf(startstr);//【开始字符串】在【HTML】位置


            if (startIndex == -1)
            {
                return "";
            }

            //int endIndex = str.IndexOf(endstr);
            int x = startstr.Length;//【开始字符串】的长度
            string cat = str.Substring(0, startIndex + x);//从【开头】到【开始字符串】

            cat = str.Replace(cat, "");//【目标字符串】后到【尾巴】
            int end_len = cat.IndexOf(endstr);//【目标字符串】的长度

            int endIndex = cat.IndexOf(endstr);


            int KEY_INDEX = startIndex + startstr.Length + endIndex + endstr.Length;



            int CCC = 0;
            if (CYCLE == "1")
            {
                //Regex r = new Regex(startstr); // 定义一个Regex对象实例
                //Match m = r.Match(str); // 在字符串中匹配
                //if (m.Success)
                //{
                //    CCC = m.Length;
                //}

                CCC = (str.Length - str.Replace(startstr, String.Empty).Length) / startstr.Length;
            }




            switch (operate)
            {
                case 1://截取
                    //if (endIndex <= startIndex || startIndex == -1)
                    //    return "";

                    if (inHead == 1 && inTail == 1)//表示包含头尾,保证能找到尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex + endstr.Length);

                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Substring(startIndex, endIndex - startIndex);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        //return str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length);

                        if (CCC == 0)
                        {
                            return str.Substring(startIndex + startstr.Length, end_len);
                        }
                        else
                        {
                            string wolf = "";
                            wolf = "" + str.Substring(startIndex + startstr.Length, end_len).Trim();

                            for (int i = 1; i < CCC; i++)
                            {
                                //------------重新定位【开始长度位】--------------
                                //--------------【KEY_INDEX】---------------------
                                //------------先替换掉处理的【HTML】--------------

                                //int temp_index = startIndex + startstr.Length + end_len + endstr.Length+337 ;

                                cat = str.Substring(0, KEY_INDEX);
                                cat = str.Replace(cat, "");//【新世纪】

                                startIndex = cat.IndexOf(startstr);

                                string mouse = cat.Substring(0, startIndex + startstr.Length);//从【开头】到【开始字符串】
                                mouse = cat.Replace(mouse, "");
                                endIndex = mouse.IndexOf(endstr);

                                wolf += "|" + cat.Substring(startIndex + startstr.Length, endIndex).Trim();

                                KEY_INDEX += startIndex + startstr.Length + endIndex + endstr.Length;
                            }

                            return wolf;
                        }

                    }
                    break;
                case 2://删除段
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), "");
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), "");
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), "");
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), "");
                    }
                    break;
                case 3://删除字符串
                    return str.Replace(startstr, "");
                    break;
                case 4://字符串前缀
                    return str.Replace(startstr, Newstr + startstr);
                    break;
                case 5://段落前缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;
                    if (inHead == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex));

                    if (inHead == 0)
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr + str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length));
                    break;
                case 6://字符串后缀
                    return str.Replace(startstr, startstr + Newstr);
                    break;
                case 7://段落后缀
                    if (endIndex <= startIndex || startIndex == -1)
                        return str;

                    if (inTail == 1)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), str.Substring(startIndex, endIndex - startIndex + endstr.Length) + Newstr);

                    if (inTail == 0)
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr + str.Substring(startIndex, endIndex - startIndex) + Newstr);
                    break;
                case 8://替换字符串
                    return str.Replace(startstr, Newstr);
                    break;
                case 9://替换段落
                    if (inHead == 1 && inTail == 1)//表示包含头尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex + endstr.Length), Newstr);
                    }

                    if (inHead == 1 && inTail == 0)//表示包含头，不包含尾
                    {
                        return str.Replace(str.Substring(startIndex, endIndex - startIndex), Newstr);
                    }

                    if (inHead == 0 && inTail == 1)//表示不包含头，包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length + endstr.Length), Newstr);
                    }

                    if (inHead == 0 && inTail == 0)//表示不包含头，也不包含尾
                    {
                        return str.Replace(str.Substring(startIndex + startstr.Length, endIndex - startIndex - startstr.Length), Newstr);
                    }

                    break;
            }
            return str;
        }

        private void button33_Click(object sender, EventArgs e)
        {
            /*
             insert into Asics_pic (pid,url_pic,flag,status)
             select id,'http://www.asicsshoesmart.com/'+pic_s,'S',1 from Asics_b3
             */
            DataGroup group = new DataGroup();

            DataGroup group_html = new DataGroup();
            DataEntity de = new DataEntity();
            de.RemoveAll();
            group_html = null;

            string ls_sql = "SELECT ID,pic_group as pic_group FROM  Photo_hongzilan_B2 where  status=4  order by ID";//WHERE STATUS =2
            group_html = DB.GetDataGroup(ls_sql);
            if (group_html.Table.Rows.Count > 0)
            {

                for (int i = 0; i < group_html.Table.Rows.Count; i++)
                {
                    string ls_shtml = group_html.Table.Rows[i]["pic_group"].ToString();
                    string[] dog_small = ls_shtml.Split('|');

                    foreach (string bb in dog_small)
                    {
                        string xx = bb.Trim();
                        //string ls_up = "insert into ZY_PIC (pid,url_pic,flag,status) values ('" + group_html.Table.Rows[i]["ID"].ToString() + "','http://www.xxx.com/" + xx + "','B',1)";

                        group = null;
                        string ls_double = "select id from Photo_hongzilan_PIC where url_pic='" + xx + "' and status=1   ";
                        group = DB.GetDataGroup(ls_double);
                        if (group.Table.Rows.Count == 0)
                        {
                            string ls_up = "insert into Photo_hongzilan_PIC (pid,url_pic,flag,status) values ('" + group_html.Table.Rows[i]["ID"].ToString().Trim() + "','" + xx + "','B',2)";//http://www.xxx.com/
                            DB.ExecuteSQL(ls_up);
                        }

                    }



                }
            }

            MessageBox.Show("OK!");

        }

        private void button34_Click(object sender, EventArgs e)
        {

            /*
             update chun_pic set url_pic ='http://www.chuncuinet.com'+url_pic where status=1
            update chun_pic set status=0 where url_pic like '%../../%'
            update chun_pic set status=2 where url_pic like '%http%'
            
             insert into chun_pic (pid,url_pic,flag,status)
             select id,'http://www.chuncuinet.com/'+pic_s,'S',1 from chun_c2
             
             */


            DataGroup group = new DataGroup();
            group = null;
            //		AbsoluteUri	"http://www.otbags.com/"	string

            string ls_sql = "SELECT ID, URL_PIC,(select sub_photo_name from Photo_hongzilan_B2 where id=Photo_hongzilan_PIC.pid) as picdir  FROM Photo_hongzilan_PIC WHERE STATUS =2  order by ID";//and flag='B'  and url_pic!='http://www.otbags.com/' and id >0 
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {
                    //string pig = getUrlSource(group.Table.Rows[i]["URL_PIC"].ToString(), "utf-8");
                    //string ls_up = "UPDATE JADE_PIC SET PD='" + pig + "' ,STATUS=2 WHERE ID='" + group.Table.Rows[i]["ID"].ToString() + "'";
                    //DB.ExecuteSQL(ls_up);
                    //pig = "";

                    //string xx = "";

                    //xx=group.Table.Rows[i]["URL_PIC"].ToString();

                    //xx=

                    //.Replace("http://www.jades.cn/huayu/", "d:/2010/").Replace("/","\\")

                    try
                    {

                        下载文件(new Uri(group.Table.Rows[i]["URL_PIC"].ToString()), "e:/hongzilan/" + group.Table.Rows[i]["picdir"].ToString().Replace("/", "-").Replace(":", "") + "/", group.Table.Rows[i]["ID"].ToString());
                    }
                    catch
                    {

                    }

                }
            }

            MessageBox.Show("OK!");
        }


        #region 功能代码
        /// <summary>
        /// 下载文件到指定目录，并返回下载后存放的文件路径
        /// </summary>
        /// <param name="Uri">网址</param>
        /// <param name="存放目录">存放目录，如果该目录中已存在与待下载文件同名的文件，那么将自动重命名</param>
        /// <returns>下载文件存放的文件路径</returns>
        public string 下载文件(Uri Uri, string 存放目录, string 文件名)
        {

            var q = WebRequest.Create(Uri).GetResponse();
            var s = q.GetResponseStream();
            var b = new BinaryReader(s);

            if (q.ContentLength == -1)
            {
                return "";
            }



            var file = "";

            try
            {
                file = 生成下载文件存放路径(存放目录, Uri, q.ContentType, 文件名);
            }
            catch
            {
                b.Close();
                s.Close();
                return "";
            }

            FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
            fs.Write(b.ReadBytes((int)q.ContentLength), 0, (int)q.ContentLength);
            fs.Close();
            b.Close();
            s.Close();
            return file;

        }

        string 生成下载文件存放路径(string 存放目录, Uri Uri, string ContentType, string 文件名)
        {
            try
            {
                var ex = "jpg";//获取对应扩展名(ContentType);
                string up = null;
                string upne = null;
                if (Uri.LocalPath == "/")
                {
                    //处理Url是域名的情况
                    up = upne = Uri.Host;
                }
                else
                {
                    if (Uri.LocalPath.EndsWith("/"))
                    {
                        //处理Url是目录的情况
                        up = Uri.LocalPath.Substring(0, Uri.LocalPath.Length - 1);
                        upne = Path.GetFileName(up);
                    }
                    else
                    {
                        //处理常规Url
                        up = Uri.LocalPath;
                        upne = Path.GetFileNameWithoutExtension(up);
                    }
                }

                //var name = string.IsNullOrEmpty(ex) ? Path.GetFileName(up) : upne + "." + ex;
                var name = 文件名 + "." + ex;

                var fn = Path.Combine(存放目录, name);
                var x = 1;
                while (File.Exists(fn))
                {
                    fn = Path.Combine(存放目录, Path.GetFileNameWithoutExtension(name) + "(" + x++ + ")" + Path.GetExtension(name));
                }
                return fn;

            }
            catch
            {
                return null;
            }
        }

       


        #endregion





        private void button3_Click(object sender, EventArgs e)
        {
            DirectoryInfo theDir = new DirectoryInfo("E:\\hongzilan\\");


            DataGroup group = new DataGroup();
            group = null;
            string ls_sql = " select sub_photo_name from Photo_hongzilan_B2 where status=4 ";
            group = DB.GetDataGroup(ls_sql);
            if (group.Table.Rows.Count > 0)
            {
                for (int i = 0; i < group.Table.Rows.Count; i++)
                {

                    theDir.CreateSubdirectory(group.Table.Rows[i]["sub_photo_name"].ToString().Replace("/", "-").Replace(":", ""));

                }

            }

            MessageBox.Show("OK!");



        }




    }
}
