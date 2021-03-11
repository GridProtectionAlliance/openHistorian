using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using openHistorian.UI.WPF.ViewModels;

namespace openHistorian.UI.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for CompressionSettingUserControl.xaml
    /// </summary>
    public partial class CompressionSettingUserControl : UserControl
    {
        #region [ Members ]

        // Fields
        private readonly CompressionSettingsViewModel m_viewModel;
        private DataGridColumn m_sortColumn;
        private string m_sortMemberPath;
        private ListSortDirection m_sortDirection;

        #endregion

        #region [ Constructors ]

        public CompressionSettingUserControl()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                m_viewModel = new CompressionSettingsViewModel();
                m_viewModel.PropertyChanged += ViewModel_PropertyChanged;
                DataContext = m_viewModel;
            }
        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Methods ]

        private void MeasurementPagerButton_Click(object sender, RoutedEventArgs e)
        {
            MeasurementPagerPopup.IsOpen = true;
        }

        private void ButtonSelectMeasurement_Click(object sender, RoutedEventArgs e)
        {
            m_viewModel.CurrentItem.PointID = MeasurementPager.CurrentItem.PointID;
            MeasurementPagerPopup.IsOpen = false;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            MeasurementPagerPopup.IsOpen = false;
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DataGrid dataGrid = sender as DataGrid;
                if (dataGrid.SelectedItems.Count > 0)
                {
                    if (MessageBox.Show("Are you sure you want to delete " + dataGrid.SelectedItems.Count + " selected item(s)?", "Delete Selected Items", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        e.Handled = true;
                }
            }
        }

        private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            string sortMember;

            if (e.Column.SortMemberPath != m_sortMemberPath)
                m_sortDirection = ListSortDirection.Ascending;
            else if (m_sortDirection == ListSortDirection.Ascending)
                m_sortDirection = ListSortDirection.Descending;
            else
                m_sortDirection = ListSortDirection.Ascending;

            m_sortColumn = e.Column;
            m_sortMemberPath = e.Column.SortMemberPath;

            switch (m_sortMemberPath)
            {
                case "OperationDescription":
                    sortMember = "Operation";
                    break;

                case "SeverityName":
                    sortMember = "Severity";
                    break;

                default:
                    sortMember = m_sortMemberPath;
                    break;
            }

            m_viewModel.SortData(sortMember, m_sortDirection);
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ItemsSource")
                Dispatcher.BeginInvoke(new Action(SortDataGrid));
        }

        private void SortDataGrid()
        {
            if (m_sortColumn != null)
            {
                m_sortColumn.SortDirection = m_sortDirection;
                DataGridList.Items.SortDescriptions.Clear();
                DataGridList.Items.SortDescriptions.Add(new SortDescription(m_sortMemberPath, m_sortDirection));
                DataGridList.Items.Refresh();
            }
        }

        private void GridDetailView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (m_viewModel.IsNewRecord)
                DataGridList.SelectedIndex = -1;
        }

        #endregion

        #region [ Operators ]

        #endregion

        #region [ Static ]

        // Static Fields

        // Static Constructor

        // Static Properties

        // Static Methods

        #endregion
    }
}
