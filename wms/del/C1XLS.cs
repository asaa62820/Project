using System;
using System.Collections.Generic;
using System.Text;
//using C1.C1Excel;

namespace UniversalAnalyse
{
    class C1XLS
    {
    }

    //public class MyXLCol
    //{
    //    public string sHyperlink;
    //    public object value;
    //    public int Width;
    //    public XLStyle xlStyle;
    //    public XLStyle xlStyleColumn;
    //}

    //public class MyXLRow
    //{
    //    public IList<MyXLCol> cells = new List<MyXLCol>();
    //}

    //public class MyXLSheet
    //{
    //    public IList<MyXLRow> rows = new List<MyXLRow>();
    //    public IList<XLCellRange> xlCellRangeList = new List<XLCellRange>();

    //    public void SetRowCol(int nRow, int nCol)
    //    {
    //        rows.Clear();
    //        for (int i = 0; i < nRow; i++)
    //        {
    //            MyXLRow myXLRow = new MyXLRow();

    //            for (int j = 0; j < nCol; j++)
    //            {
    //                MyXLCol myXLCell = new MyXLCol();
    //                myXLRow.cells.Add(myXLCell);
    //            }

    //            rows.Add(myXLRow);
    //        }
    //    }

    //    public void 复制合并单元格(XLCellRangeCollection xlCellRangeCollection)
    //    {
    //        xlCellRangeList.Clear();
    //        for (int i = 0; i < xlCellRangeCollection.Count; i++)
    //        {
    //            XLCellRange xlCellRange = xlCellRangeCollection[i];
    //            XLCellRange xlCellRangeNew = new XLCellRange(xlCellRange.RowFrom, xlCellRange.RowTo, xlCellRange.ColumnFrom, xlCellRange.ColumnTo);

    //            xlCellRangeList.Add(xlCellRangeNew);
    //        }
    //    }

    //    public void 粘贴合并单元格(XLCellRangeCollection xlCellRangeCollection)
    //    {
    //        xlCellRangeCollection.Clear();
    //        for (int i = 0; i < xlCellRangeList.Count; i++)
    //        {
    //            XLCellRange xlCellRange = xlCellRangeList[i];
    //            XLCellRange xlCellRangeNew = new XLCellRange(xlCellRange.RowFrom, xlCellRange.RowTo, xlCellRange.ColumnFrom, xlCellRange.ColumnTo);

    //            xlCellRangeCollection.Add(xlCellRangeNew);
    //        }
    //    }

    //    public void 变换合并单元格(int nRow)
    //    {
    //        int nCount = xlCellRangeList.Count;

    //        for (int i = 0; i < nCount; i++)
    //        {
    //            XLCellRange xlCellRange = xlCellRangeList[i];

    //            int nRowFrom = xlCellRange.RowFrom;
    //            int nRowTo = xlCellRange.RowTo;
    //            int nColumnFrom = xlCellRange.ColumnFrom;
    //            int nColumnTo = xlCellRange.ColumnTo;

    //            XLCellRange xlCellRangeNew = null;
    //            //
    //            if (nRowFrom >= nRow)
    //                nRowFrom++;

    //            if (nRowTo >= nRow)
    //                nRowTo++;

    //            xlCellRangeNew = new XLCellRange(nRowFrom, nRowTo, nColumnFrom, nColumnTo);
    //            xlCellRangeList[i] = xlCellRangeNew;

    //            if (nRowFrom == nRowTo && nRowFrom == nRow + 1)
    //            {
    //                xlCellRangeNew = new XLCellRange(nRowFrom - 1, nRowTo - 1, nColumnFrom, nColumnTo);
    //                xlCellRangeList.Add(xlCellRangeNew);
    //            }
    //        }

    //    }

    //    public void SetMyXLCell(MyXLCol myXLCell, MyXLCol myXLNewCell, C1XLBook c1XLBook)
    //    {
    //        myXLNewCell.value = myXLCell.value;
    //        myXLNewCell.sHyperlink = myXLCell.sHyperlink;
    //        myXLNewCell.xlStyle = new XLStyle(c1XLBook);

    //        myXLNewCell.xlStyle.AlignHorz = myXLCell.xlStyle.AlignHorz;
    //        myXLNewCell.xlStyle.AlignVert = myXLCell.xlStyle.AlignVert;
    //        myXLNewCell.xlStyle.BackColor = myXLCell.xlStyle.BackColor;
    //        myXLNewCell.xlStyle.BackPattern = myXLCell.xlStyle.BackPattern;
    //        myXLNewCell.xlStyle.BorderBottom = myXLCell.xlStyle.BorderBottom;
    //        myXLNewCell.xlStyle.BorderColorBottom = myXLCell.xlStyle.BorderColorBottom;
    //        myXLNewCell.xlStyle.BorderColorLeft = myXLCell.xlStyle.BorderColorLeft;
    //        myXLNewCell.xlStyle.BorderColorRight = myXLCell.xlStyle.BorderColorRight;
    //        myXLNewCell.xlStyle.BorderColorTop = myXLCell.xlStyle.BorderColorTop;
    //        myXLNewCell.xlStyle.BorderLeft = myXLCell.xlStyle.BorderLeft;
    //        myXLNewCell.xlStyle.BorderRight = myXLCell.xlStyle.BorderRight;
    //        myXLNewCell.xlStyle.BorderTop = myXLCell.xlStyle.BorderTop;
    //        myXLNewCell.xlStyle.Font = myXLCell.xlStyle.Font;
    //        myXLNewCell.xlStyle.Diagonal = myXLCell.xlStyle.Diagonal;
    //        myXLNewCell.xlStyle.DiagonalColor = myXLCell.xlStyle.DiagonalColor;
    //        myXLNewCell.xlStyle.DiagonalStyle = myXLCell.xlStyle.DiagonalStyle;
    //        myXLNewCell.xlStyle.ForeColor = myXLCell.xlStyle.ForeColor;
    //        myXLNewCell.xlStyle.Format = myXLCell.xlStyle.Format;
    //        myXLNewCell.xlStyle.Locked = myXLCell.xlStyle.Locked;
    //        myXLNewCell.xlStyle.PatternColor = myXLCell.xlStyle.PatternColor;
    //        myXLNewCell.xlStyle.Rotation = myXLCell.xlStyle.Rotation;
    //        myXLNewCell.xlStyle.WordWrap = myXLCell.xlStyle.WordWrap;
    //        // 对列的属性设置
    //        myXLNewCell.Width = myXLCell.Width;
    //        myXLNewCell.xlStyleColumn = new XLStyle(c1XLBook);

    //        myXLNewCell.xlStyleColumn.AlignHorz = myXLCell.xlStyleColumn.AlignHorz;
    //        myXLNewCell.xlStyleColumn.AlignVert = myXLCell.xlStyleColumn.AlignVert;
    //        myXLNewCell.xlStyleColumn.BackColor = myXLCell.xlStyleColumn.BackColor;
    //        myXLNewCell.xlStyleColumn.BackPattern = myXLCell.xlStyleColumn.BackPattern;
    //        myXLNewCell.xlStyleColumn.BorderBottom = myXLCell.xlStyleColumn.BorderBottom;
    //        myXLNewCell.xlStyleColumn.BorderColorBottom = myXLCell.xlStyleColumn.BorderColorBottom;
    //        myXLNewCell.xlStyleColumn.BorderColorLeft = myXLCell.xlStyleColumn.BorderColorLeft;
    //        myXLNewCell.xlStyleColumn.BorderColorRight = myXLCell.xlStyleColumn.BorderColorRight;
    //        myXLNewCell.xlStyleColumn.BorderColorTop = myXLCell.xlStyleColumn.BorderColorTop;
    //        myXLNewCell.xlStyleColumn.BorderLeft = myXLCell.xlStyleColumn.BorderLeft;
    //        myXLNewCell.xlStyleColumn.BorderRight = myXLCell.xlStyleColumn.BorderRight;
    //        myXLNewCell.xlStyleColumn.BorderTop = myXLCell.xlStyleColumn.BorderTop;
    //        myXLNewCell.xlStyleColumn.Font = myXLCell.xlStyleColumn.Font;
    //        myXLNewCell.xlStyleColumn.Diagonal = myXLCell.xlStyleColumn.Diagonal;
    //        myXLNewCell.xlStyleColumn.DiagonalColor = myXLCell.xlStyleColumn.DiagonalColor;
    //        myXLNewCell.xlStyleColumn.DiagonalStyle = myXLCell.xlStyleColumn.DiagonalStyle;
    //        myXLNewCell.xlStyleColumn.ForeColor = myXLCell.xlStyleColumn.ForeColor;
    //        myXLNewCell.xlStyleColumn.Format = myXLCell.xlStyleColumn.Format;
    //        myXLNewCell.xlStyleColumn.Locked = myXLCell.xlStyleColumn.Locked;
    //        myXLNewCell.xlStyleColumn.PatternColor = myXLCell.xlStyleColumn.PatternColor;
    //        myXLNewCell.xlStyleColumn.Rotation = myXLCell.xlStyleColumn.Rotation;
    //        myXLNewCell.xlStyleColumn.WordWrap = myXLCell.xlStyleColumn.WordWrap;

    //    }

    //    public void SetMyXLSheetCell(int nRow, int nCol, C1XLBook c1XLBook, XLSheet xlSheet)
    //    {
    //        rows[nRow].cells[nCol].value = xlSheet[nRow, nCol].Value;
    //        rows[nRow].cells[nCol].sHyperlink = xlSheet[nRow, nCol].Hyperlink;
    //        rows[nRow].cells[nCol].xlStyle = new XLStyle(c1XLBook);

    //        rows[nRow].cells[nCol].xlStyle.AlignHorz = xlSheet[nRow, nCol].Style.AlignHorz;
    //        rows[nRow].cells[nCol].xlStyle.AlignVert = xlSheet[nRow, nCol].Style.AlignVert;
    //        rows[nRow].cells[nCol].xlStyle.BackColor = xlSheet[nRow, nCol].Style.BackColor;
    //        rows[nRow].cells[nCol].xlStyle.BackPattern = xlSheet[nRow, nCol].Style.BackPattern;
    //        rows[nRow].cells[nCol].xlStyle.BorderBottom = xlSheet[nRow, nCol].Style.BorderBottom;
    //        rows[nRow].cells[nCol].xlStyle.BorderColorBottom = xlSheet[nRow, nCol].Style.BorderColorBottom;
    //        rows[nRow].cells[nCol].xlStyle.BorderColorLeft = xlSheet[nRow, nCol].Style.BorderColorLeft;
    //        rows[nRow].cells[nCol].xlStyle.BorderColorRight = xlSheet[nRow, nCol].Style.BorderColorRight;
    //        rows[nRow].cells[nCol].xlStyle.BorderColorTop = xlSheet[nRow, nCol].Style.BorderColorTop;
    //        rows[nRow].cells[nCol].xlStyle.BorderLeft = xlSheet[nRow, nCol].Style.BorderLeft;
    //        rows[nRow].cells[nCol].xlStyle.BorderRight = xlSheet[nRow, nCol].Style.BorderRight;
    //        rows[nRow].cells[nCol].xlStyle.BorderTop = xlSheet[nRow, nCol].Style.BorderTop;
    //        rows[nRow].cells[nCol].xlStyle.Font = xlSheet[nRow, nCol].Style.Font;
    //        rows[nRow].cells[nCol].xlStyle.Diagonal = xlSheet[nRow, nCol].Style.Diagonal;
    //        rows[nRow].cells[nCol].xlStyle.DiagonalColor = xlSheet[nRow, nCol].Style.DiagonalColor;
    //        rows[nRow].cells[nCol].xlStyle.DiagonalStyle = xlSheet[nRow, nCol].Style.DiagonalStyle;
    //        rows[nRow].cells[nCol].xlStyle.ForeColor = xlSheet[nRow, nCol].Style.ForeColor;
    //        rows[nRow].cells[nCol].xlStyle.Format = xlSheet[nRow, nCol].Style.Format;
    //        rows[nRow].cells[nCol].xlStyle.Locked = xlSheet[nRow, nCol].Style.Locked;
    //        rows[nRow].cells[nCol].xlStyle.PatternColor = xlSheet[nRow, nCol].Style.PatternColor;
    //        rows[nRow].cells[nCol].xlStyle.Rotation = xlSheet[nRow, nCol].Style.Rotation;
    //        rows[nRow].cells[nCol].xlStyle.WordWrap = xlSheet[nRow, nCol].Style.WordWrap;
    //        // 对列的属性设置
    //        rows[nRow].cells[nCol].Width = xlSheet.Columns[nCol].Width;
    //        rows[nRow].cells[nCol].xlStyleColumn = new XLStyle(c1XLBook);

    //        rows[nRow].cells[nCol].xlStyleColumn.AlignHorz = xlSheet.Columns[nCol].Style.AlignHorz;
    //        rows[nRow].cells[nCol].xlStyleColumn.AlignVert = xlSheet.Columns[nCol].Style.AlignVert;
    //        rows[nRow].cells[nCol].xlStyleColumn.BackColor = xlSheet.Columns[nCol].Style.BackColor;
    //        rows[nRow].cells[nCol].xlStyleColumn.BackPattern = xlSheet.Columns[nCol].Style.BackPattern;
    //        rows[nRow].cells[nCol].xlStyleColumn.BorderBottom = xlSheet.Columns[nCol].Style.BorderBottom;
    //        rows[nRow].cells[nCol].xlStyleColumn.BorderColorBottom = xlSheet.Columns[nCol].Style.BorderColorBottom;
    //        rows[nRow].cells[nCol].xlStyleColumn.BorderColorLeft = xlSheet.Columns[nCol].Style.BorderColorLeft;
    //        rows[nRow].cells[nCol].xlStyleColumn.BorderColorRight = xlSheet.Columns[nCol].Style.BorderColorRight;
    //        rows[nRow].cells[nCol].xlStyleColumn.BorderColorTop = xlSheet.Columns[nCol].Style.BorderColorTop;
    //        rows[nRow].cells[nCol].xlStyleColumn.BorderLeft = xlSheet.Columns[nCol].Style.BorderLeft;
    //        rows[nRow].cells[nCol].xlStyleColumn.BorderRight = xlSheet.Columns[nCol].Style.BorderRight;
    //        rows[nRow].cells[nCol].xlStyleColumn.BorderTop = xlSheet.Columns[nCol].Style.BorderTop;
    //        rows[nRow].cells[nCol].xlStyleColumn.Font = xlSheet.Columns[nCol].Style.Font;
    //        rows[nRow].cells[nCol].xlStyleColumn.Diagonal = xlSheet.Columns[nCol].Style.Diagonal;
    //        rows[nRow].cells[nCol].xlStyleColumn.DiagonalColor = xlSheet.Columns[nCol].Style.DiagonalColor;
    //        rows[nRow].cells[nCol].xlStyleColumn.DiagonalStyle = xlSheet.Columns[nCol].Style.DiagonalStyle;
    //        rows[nRow].cells[nCol].xlStyleColumn.ForeColor = xlSheet.Columns[nCol].Style.ForeColor;
    //        rows[nRow].cells[nCol].xlStyleColumn.Format = xlSheet.Columns[nCol].Style.Format;
    //        rows[nRow].cells[nCol].xlStyleColumn.Locked = xlSheet.Columns[nCol].Style.Locked;
    //        rows[nRow].cells[nCol].xlStyleColumn.PatternColor = xlSheet.Columns[nCol].Style.PatternColor;
    //        rows[nRow].cells[nCol].xlStyleColumn.Rotation = xlSheet.Columns[nCol].Style.Rotation;
    //        rows[nRow].cells[nCol].xlStyleColumn.WordWrap = xlSheet.Columns[nCol].Style.WordWrap;

    //    }

    //    public void SetXLSheetCell(int nRow, int nCol, C1XLBook c1XLBook, XLSheet xlSheet)
    //    {
    //        XLStyle xlStyle = new XLStyle(c1XLBook);
    //        xlSheet[nRow, nCol].Value = rows[nRow].cells[nCol].value;

    //        xlStyle.AlignHorz = rows[nRow].cells[nCol].xlStyle.AlignHorz;
    //        xlStyle.AlignVert = rows[nRow].cells[nCol].xlStyle.AlignVert;
    //        xlStyle.BackColor = rows[nRow].cells[nCol].xlStyle.BackColor;
    //        xlStyle.BackPattern = rows[nRow].cells[nCol].xlStyle.BackPattern;
    //        xlStyle.BorderBottom = rows[nRow].cells[nCol].xlStyle.BorderBottom;
    //        xlStyle.BorderColorBottom = rows[nRow].cells[nCol].xlStyle.BorderColorBottom;
    //        xlStyle.BorderColorLeft = rows[nRow].cells[nCol].xlStyle.BorderColorLeft;
    //        xlStyle.BorderColorRight = rows[nRow].cells[nCol].xlStyle.BorderColorRight;
    //        xlStyle.BorderColorTop = rows[nRow].cells[nCol].xlStyle.BorderColorTop;
    //        xlStyle.BorderLeft = rows[nRow].cells[nCol].xlStyle.BorderLeft;
    //        xlStyle.BorderRight = rows[nRow].cells[nCol].xlStyle.BorderRight;
    //        xlStyle.BorderTop = rows[nRow].cells[nCol].xlStyle.BorderTop;
    //        xlStyle.Font = rows[nRow].cells[nCol].xlStyle.Font;
    //        xlStyle.Diagonal = rows[nRow].cells[nCol].xlStyle.Diagonal;
    //        xlStyle.DiagonalColor = rows[nRow].cells[nCol].xlStyle.DiagonalColor;
    //        xlStyle.DiagonalStyle = rows[nRow].cells[nCol].xlStyle.DiagonalStyle;
    //        xlStyle.ForeColor = rows[nRow].cells[nCol].xlStyle.ForeColor;
    //        xlStyle.Format = rows[nRow].cells[nCol].xlStyle.Format;
    //        xlStyle.Locked = rows[nRow].cells[nCol].xlStyle.Locked;
    //        xlStyle.PatternColor = rows[nRow].cells[nCol].xlStyle.PatternColor;
    //        xlStyle.Rotation = rows[nRow].cells[nCol].xlStyle.Rotation;
    //        xlStyle.WordWrap = rows[nRow].cells[nCol].xlStyle.WordWrap;

    //        xlSheet[nRow, nCol].Style = xlStyle;
    //        // 对列的属性设置
    //        xlSheet.Columns[nCol].Width = rows[nRow].cells[nCol].Width;
    //        XLStyle xlStyleColumn = new XLStyle(c1XLBook);

    //        xlStyleColumn.AlignHorz = rows[nRow].cells[nCol].xlStyleColumn.AlignHorz;
    //        xlStyleColumn.AlignVert = rows[nRow].cells[nCol].xlStyleColumn.AlignVert;
    //        xlStyleColumn.BackColor = rows[nRow].cells[nCol].xlStyleColumn.BackColor;
    //        xlStyleColumn.BackPattern = rows[nRow].cells[nCol].xlStyleColumn.BackPattern;
    //        xlStyleColumn.BorderBottom = rows[nRow].cells[nCol].xlStyleColumn.BorderBottom;
    //        xlStyleColumn.BorderColorBottom = rows[nRow].cells[nCol].xlStyleColumn.BorderColorBottom;
    //        xlStyleColumn.BorderColorLeft = rows[nRow].cells[nCol].xlStyleColumn.BorderColorLeft;
    //        xlStyleColumn.BorderColorRight = rows[nRow].cells[nCol].xlStyleColumn.BorderColorRight;
    //        xlStyleColumn.BorderColorTop = rows[nRow].cells[nCol].xlStyleColumn.BorderColorTop;
    //        xlStyleColumn.BorderLeft = rows[nRow].cells[nCol].xlStyleColumn.BorderLeft;
    //        xlStyleColumn.BorderRight = rows[nRow].cells[nCol].xlStyleColumn.BorderRight;
    //        xlStyleColumn.BorderTop = rows[nRow].cells[nCol].xlStyleColumn.BorderTop;
    //        xlStyleColumn.Font = rows[nRow].cells[nCol].xlStyleColumn.Font;
    //        xlStyleColumn.Diagonal = rows[nRow].cells[nCol].xlStyleColumn.Diagonal;
    //        xlStyleColumn.DiagonalColor = rows[nRow].cells[nCol].xlStyleColumn.DiagonalColor;
    //        xlStyleColumn.DiagonalStyle = rows[nRow].cells[nCol].xlStyleColumn.DiagonalStyle;
    //        xlStyleColumn.ForeColor = rows[nRow].cells[nCol].xlStyleColumn.ForeColor;
    //        xlStyleColumn.Format = rows[nRow].cells[nCol].xlStyleColumn.Format;
    //        xlStyleColumn.Locked = rows[nRow].cells[nCol].xlStyleColumn.Locked;
    //        xlStyleColumn.PatternColor = rows[nRow].cells[nCol].xlStyleColumn.PatternColor;
    //        xlStyleColumn.Rotation = rows[nRow].cells[nCol].xlStyleColumn.Rotation;
    //        xlStyleColumn.WordWrap = rows[nRow].cells[nCol].xlStyleColumn.WordWrap;

    //        xlSheet.Columns[nCol].Style = xlStyleColumn;

    //    }
    //}
}
