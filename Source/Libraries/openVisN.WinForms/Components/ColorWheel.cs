using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openVisN.Components
{
    public partial class ColorWheel : Component
    {
        const int width = 2;
        static List<Pen> m_pens;
        static ColorWheel()
        {
            m_pens = new List<Pen>();
            m_pens.Add(new Pen(Color.Red, width));
            m_pens.Add(new Pen(Color.LimeGreen, width));
            m_pens.Add(new Pen(Color.Cyan, width));
            m_pens.Add(new Pen(Color.Purple, width));
            m_pens.Add(new Pen(Color.Brown, width));
            m_pens.Add(new Pen(Color.Orange, width));
            m_pens.Add(new Pen(Color.Magenta, width));
            m_pens.Add(new Pen(Color.Blue, width));
            m_pens.Add(new Pen(Color.Black, width));
            m_pens.Add(new Pen(Color.Gray, width));
            m_pens.Add(new Pen(Color.DarkGreen, width));
        }

        public ColorWheel()
        {
            InitializeComponent();
        }

        public ColorWheel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public Pen GetPen(int index)
        {
            var pen = m_pens[index % m_pens.Count];
            lock (pen)
            {
                return (Pen)pen.Clone();
            }
        }

    }

    public static class ColorWheelExtensions
    {
        static Pen s_nullPen = new Pen(Color.Black, 2);
        public static Pen TryGetPen(this ColorWheel wheel, int index)
        {
            if (wheel == null)
                return s_nullPen;
            return wheel.GetPen(index);
        }
    }
}
