//******************************************************************************************************
//  ColorWheel.cs - Gbtc
//
//  Copyright © 2014, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/15/2012 - Steven E. Chisholm
//       Generated original version of source code. 
//
//******************************************************************************************************

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace openVisN.Components
{
    public partial class ColorWheel : Component
    {
        private const int width = 2;
        private static readonly List<Pen> m_pens;

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
            Pen pen = m_pens[index % m_pens.Count];
            lock (pen)
            {
                return (Pen)pen.Clone();
            }
        }
    }

    public static class ColorWheelExtensions
    {
        private static readonly Pen s_nullPen = new Pen(Color.Black, 2);

        public static Pen TryGetPen(this ColorWheel wheel, int index)
        {
            if (wheel is null)
                return s_nullPen;
            return wheel.GetPen(index);
        }
    }
}