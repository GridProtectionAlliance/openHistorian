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
        static List<Pen> m_pens;
        static ColorWheel()
        {
            m_pens = new List<Pen>();
            m_pens.Add(new Pen(Color.Red, 2));
            m_pens.Add(new Pen(Color.LimeGreen, 2));
            m_pens.Add(new Pen(Color.Cyan, 2));
            m_pens.Add(new Pen(Color.Purple, 2));
            m_pens.Add(new Pen(Color.Brown, 2));
            m_pens.Add(new Pen(Color.Orange, 2));
            m_pens.Add(new Pen(Color.Magenta, 2));
            m_pens.Add(new Pen(Color.Blue, 2));
            m_pens.Add(new Pen(Color.Black, 2));
            m_pens.Add(new Pen(Color.Gray, 2));
            m_pens.Add(new Pen(Color.DarkGreen, 2));
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
            return m_pens[index % m_pens.Count];
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
