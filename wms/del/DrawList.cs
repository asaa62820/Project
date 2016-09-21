using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace UniversalAnalyse
{
    class DrawList : List<DrawBase>
    {
        private Control m_Owner;
        public DrawList(Control owner)
        {
            this.m_Owner = owner;
        }
        internal DrawBase GetNear(int x, int y)
        {
            foreach (DrawBase draw in this)
            {
                if (draw.Near(x, y))
                {
                    return draw;
                }
            }
            return null;
        }

        internal void Draw(Graphics graphics)
        {
            foreach (DrawBase draw in this)
            {
                draw.Draw(graphics);
            }
        }
    }
}
