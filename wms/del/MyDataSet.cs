using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UniversalAnalyse
{

    [Serializable]
    public class DataMyColumn
    {
        public int nTextNum = 0; // Ϊ���Ű��õ�
        public string sText = "";
    }

    [Serializable]
    public class DataMyRow
    {
        public IList<string> strList = new List<string>();
    }

    [Serializable]
    public class DataMyTable
    {
        public string sTableName = "";
        public string sExplain = "";
        public IList<DataMyRow> rowList = new List<DataMyRow>();
    }


    [Serializable]
    public class MyDataSet
    {
        public IList<DataMyTable> tableList = new List<DataMyTable>();
        public IList<DataMyColumn> columnList = new List<DataMyColumn>();

        public MyDataSet()
        {
            try
            {
                DataMyColumn myDataColumn = new DataMyColumn();
                myDataColumn.nTextNum = 45;
                myDataColumn.sText = "ע��";
                columnList.Add(myDataColumn);
                //
                myDataColumn = new DataMyColumn();
                myDataColumn.nTextNum = 30;
                myDataColumn.sText = "�ֶ���";
                columnList.Add(myDataColumn);
                //
                myDataColumn = new DataMyColumn();
                myDataColumn.nTextNum = 20;
                myDataColumn.sText = "�ֶ�����";
                columnList.Add(myDataColumn);
                //
                myDataColumn = new DataMyColumn();
                myDataColumn.nTextNum = 10;
                myDataColumn.sText = "�Ƿ�Ϊ��";
                columnList.Add(myDataColumn);
                //
                myDataColumn = new DataMyColumn();
                myDataColumn.nTextNum = 20;
                myDataColumn.sText = "ȱʡֵ";
                columnList.Add(myDataColumn);
                //
                myDataColumn = new DataMyColumn();
                myDataColumn.nTextNum = 20;
                myDataColumn.sText = "���˵��";
                columnList.Add(myDataColumn);
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                MessageBox.Show(str);
            }
        }
    }
}
