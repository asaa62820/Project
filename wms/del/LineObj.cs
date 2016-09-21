using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace UniversalAnalyse
{
    class LineObj : DrawBase
    {
        private Point m_Start;
        private Point m_End;
        public LineObj(Point start, Point end)
        {
            this.m_Start = start;
            this.m_End = end;
        }
        public override System.Drawing.Rectangle GetBound()
        {
            int x = this.m_Start.X < this.m_End.X ? this.m_Start.X : this.m_End.X;
            int y = this.m_Start.Y < this.m_End.Y ? this.m_Start.Y : this.m_End.Y;
            int r = this.m_Start.X < this.m_End.X ? this.m_End.X : this.m_Start.X;
            int b = this.m_Start.Y < this.m_End.Y ? this.m_End.Y : this.m_Start.Y;
            return Rectangle.FromLTRB(x, y, r, b);
        }

        public override void Draw(System.Drawing.Graphics g)
        {
            using (Pen pen = new Pen(this.m_ForeColor))
            {
                g.DrawLine(pen, this.m_Start, this.m_End);
            }
        }

        public override bool Near(int x, int y)
        {
            //点到直线的距离是否在抓取范围之内
            float A = this.m_End.Y - this.m_Start.Y;
            float B = this.m_End.X - this.m_Start.X;
            float C = B * this.m_Start.Y - A * this.m_Start.X;
            double D = (A * x - B * y + C) / (Math.Sqrt(A * A + B * B));
            if (D >= -m_HalfGrab && D <= m_HalfGrab)
            {
                RectangleF bounds = this.GetBound();
                bounds.Inflate(m_HalfGrab, m_HalfGrab);
                return bounds.Contains(x, y);
            }
            return false;
        }

        public override void SetBound(Rectangle bound)
        {
            this.m_Start = new Point(bound.X, bound.Y);
            this.m_End = new Point(bound.Right, bound.Bottom);
        }
    }
}
