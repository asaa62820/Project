using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace UniversalAnalyse
{
    /// <summary>
    /// 定义图像的基类
    /// </summary>
    abstract class DrawBase
    {
        internal Color m_BackColor;
        internal Color m_ForeColor;
        internal static int m_HalfGrab;

        public static int HalfGrab
        {
            get { return DrawBase.m_HalfGrab; }
            set { DrawBase.m_HalfGrab = value; }
        }

        public Color BackColor
        {
            get { return m_BackColor; }
            set { m_BackColor = value; }
        }

        public Color ForeColor
        {
            get { return m_ForeColor; }
            set { m_ForeColor = value; }
        }

        public abstract Rectangle GetBound();
        public abstract void Draw(Graphics g);
        public abstract bool Near(int x, int y);
        public abstract void SetBound(Rectangle bound);

        //public void DrawSelected(Graphics g)
        //{
        //    int width = 6;
        //    Rectangle tmp = GetBound();
        //    tmp.Inflate(4, 4);

        //    if (m_IsSelected)
        //    {
        //        using (Pen pen2 = new Pen(Color.Gray, 1))
        //        {
        //            pen2.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        //            g.DrawRectangle(pen2, tmp);

        //            //DrawSmallWhiteBlock(g, new Point(tmp.X + tmp.Width/2 - width / 2, tmp.Y - width / 2), width);//上中

        //            DrawSmallWhiteBlock(g, new Point(tmp.X - width / 2, tmp.Y - width / 2), width);//左上
        //            //DrawSmallWhiteBlock(g, new Point(tmp.X - width / 2, tmp.Y + tmp.Height/2 - width / 2), width);//左中
        //            DrawSmallWhiteBlock(g, new Point(tmp.X - width / 2, tmp.Y + tmp.Height - width / 2), width);//左下

        //            DrawSmallWhiteBlock(g, new Point(tmp.X + tmp.Width - width / 2, tmp.Y - width / 2), width);//右上
        //            //DrawSmallWhiteBlock(g, new Point(tmp.X + tmp.Width - width / 2, tmp.Y + tmp.Height / 2 - width / 2), width);
        //            DrawSmallWhiteBlock(g, new Point(tmp.X + tmp.Width - width / 2, tmp.Y + tmp.Height - width / 2), width);

        //            //DrawSmallWhiteBlock(g, new Point(tmp.X + tmp.Width/2 - width / 2, tmp.Y + tmp.Height - width / 2), width);//下中
        //            DrawSmallWhiteBlock(g, new Point(tmp.X + tmp.Width - width / 2, tmp.Y + tmp.Height - width / 2), width);

        //            //DrawSmallWhiteBlock(g, new Point(tmp.X + tmp.Width/2 - width / 2, tmp.Y + tmp.Height - width / 2), width);//下中
        //        }
        //    }
        //}
    }
}
