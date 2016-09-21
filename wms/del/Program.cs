using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UniversalAnalyse
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Application.Run(new Form1());

            //Application.Run(new Photo());

            //Application.Run(new wmb());

            Application.Run(new wms.Main());

            //Application.Run(new wms.Invoice_new());
        }
    }
}