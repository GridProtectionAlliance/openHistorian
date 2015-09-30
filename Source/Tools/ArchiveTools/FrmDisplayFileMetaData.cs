using System.Data;
using System.Windows.Forms;

namespace ArchiveTools
{
    public partial class FrmDisplayFileMetaData : Form
    {
        public FrmDisplayFileMetaData(DataTable dataTable)
        {
            InitializeComponent();
            dataGridView1.DataSource = dataTable;
        }
    }
}
