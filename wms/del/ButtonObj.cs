using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace UniversalAnalyse
{
    /// <summary>
    /// windows controls
    /// </summary>
    class ButtonObj : DrawBase
    {
        private Point m_Start;
        private Point m_End;
        private string m_Text;
        public ButtonObj(Point start, Point end)
        {
            this.m_Start = start;
            this.m_End = end;
        }
        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
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
            using (Pen pen = new Pen(this.m_ForeColor))
            {
                ControlPaint.DrawButton(g, bound, ButtonState.Normal);
                using (SolidBrush brush = new SolidBrush(this.m_ForeColor))
                {
                    using (Font font = new Font("ËÎÌו", 10))
                    {
                        using (StringFormat format = new StringFormat())
                        {
                            format.Alignment = StringAlignment.Center;
                            format.LineAlignment = StringAlignment.Center;
                            g.DrawString(this.m_Text, font, brush, bound, format);
                        }
                    }
                }
            }
        }

        public override bool Near(int x, int y)
        {
            Rectangle bound = this.GetBound();
            Rectangle outer = bound;
            outer.Inflate(m_HalfGrab, m_HalfGrab);
            return outer.Contains(x, y);
        }

        public override void SetBound(Rectangle bound)
        {
            this.m_Start = new Point(bound.X, bound.Y);
            this.m_End = new Point(bound.Right, bound.Bottom);
        }
    }
}
