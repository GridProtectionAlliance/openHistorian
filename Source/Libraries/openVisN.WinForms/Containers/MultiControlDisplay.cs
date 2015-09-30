using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

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