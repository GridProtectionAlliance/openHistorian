using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace openVisN.Containers
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    public partial class MultiControlDisplay : UserControl
    {
        public MultiControlDisplay()
        {
            base.SetStyle(ControlStyles.ContainerControl, true);
            base.SetStyle(ControlStyles.Selectable, false);
            InitializeComponent();
        }
    }
}
