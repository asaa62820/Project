using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace UniversalAnalyse
{
    class RectangleObj : DrawBase
    {
        private Point m_Start;
        private Point m_End;
        private bool m_Solid;
        public RectangleObj(Point start, Point end)
        {
            this.m_Start = start;
            this.m_End = end;
        }
        public bool Solid
        {
            get { return m_Solid; }
            set { m_Solid = value; }
        }
        public override System.Drawing.Rectangle GetBound()
        {
            int x = this.m_Start.X < this.m_End.X ? this.m_Start.X : this.m_End.X;
            int y = this.m_Start.Y < this.m_End.Y ? this.m_Start.Y : this.m_End.Y;
            int r = this.m_Start.X < this.m_End.X ? this.m_End.X : this.m_Start.X;
            int b = this.m_Start.Y < this.m_End.Y ? this.m_End.Y : this.m_Start.Y;
            return Rectangle.FromLTRB(x, y, r, b);
        }

        public override void Draw(Graphics g)
        {
            Rectangle bound = this.GetBound();
            if (this.m_Solid)
            {
                using (SolidBrush brush = new SolidBrush(this.m_BackColor))
                {
                    g.FillRectangle(brush, bound);
                }
            }
            using (Pen pen = new Pen(this.m_ForeColor))
            {
                g.DrawRectangle(pen, bound);
            }
        }

        public override bool Near(int x, int y)
        {
            Rectangle bound = this.GetBound();
            Rectangle inner = bound;
            Rectangle outer = bound;
            inner.Inflate(-m_HalfGrab, -m_HalfGrab);
            outer.Inflate(m_HalfGrab, m_HalfGrab);
            Region reg = new Region(outer);
            reg.Exclude(inner);
            return reg.IsVisible(x, y);
        }

        public override void SetBound(Rectangle bound)
        {
            this.m_Start = new Point(bound.X, bound.Y);
            this.m_End = new Point(bound.Right, bound.Bottom);
        }
    }
}
