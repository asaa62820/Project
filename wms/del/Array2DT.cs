using System; 
using System.Data; 
 
namespace ArrayToDataTable 
{ 
    class ArrayToDataTable 
    { 
        /**//// <summary> 
        /// 把一个一维数组转换为DataTable 
        /// </summary> 
        /// <param name="ColumnName">列名</param> 
        /// <param name="Array">一维数组</param> 
        /// <returns>返回DataTable</returns> 
        /// <remarks>随风工作室 http://www.jobroon.cn/ </remarks> 
        public static DataTable Convert(string ColumnName, string[] Array) 
        { 
            DataTable dt = new DataTable(); 
            dt.Columns.Add(ColumnName, typeof(string)); 
 
            for (int i = 0; i < Array.Length; i++) 
            { 
                DataRow dr = dt.NewRow(); 
                dr[ColumnName] = Array[i].ToString(); 
                dt.Rows.Add(dr); 
            } 
 
            return dt; 
        } 
 
 
        /**//// <summary> 
        /// 反一个M行N列的二维数组转换为DataTable 
        /// </summary> 
        /// <param name="ColumnNames">一维数组，代表列名，不能有重复值</param> 
        /// <param name="Arrays">M行N列的二维数组</param> 
        /// <returns>返回DataTable</returns> 
        /// <remarks>随风工作室 http://www.jobroon.cn/ </remarks> 
        public static DataTable Convert(string[] ColumnNames, string[,] Arrays) 
        { 
            DataTable dt = new DataTable(); 
 
            foreach (string ColumnName in ColumnNames) 
            { 
                dt.Columns.Add(ColumnName, typeof(string)); 
            } 
 
            for (int i1 = 0; i1 < Arrays.GetLength(0); i1++) 
            { 
                DataRow dr = dt.NewRow(); 
                for (int i = 0; i < ColumnNames.Length; i++) 
                { 
                    dr[i] = Arrays[i1, i].ToString(); 
                } 
                dt.Rows.Add(dr); 
            } 
 
            return dt; 
 
        } 
 
 
        /**//// <summary> 
        /// 反一个M行N列的二维数组转换为DataTable 
        /// </summary> 
        /// <param name="Arrays">M行N列的二维数组</param> 
        /// <returns>返回DataTable</returns> 
        /// <remarks>随风工作室 http://www.jobroon.cn/ </remarks> 
        public static DataTable Convert(Object[,] Arrays) 
        { 
            DataTable dt = new DataTable(); 
 
            int a = Arrays.GetLength(0); 
            for (int i = 0; i < Arrays.GetLength(1); i++) 
            { 
                dt.Columns.Add("col" + i.ToString(), typeof(string)); 
            } 
 
            for (int i1 = 0; i1 < Arrays.GetLength(0); i1++) 
            { 
                DataRow dr = dt.NewRow(); 
                for (int i = 0; i < Arrays.GetLength(1); i++) 
                { 
                    dr[i] = Arrays[i1, i].ToString(); 
                } 
                dt.Rows.Add(dr); 
            } 
 
            return dt; 
 
        } 
 
    } 
}