using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using dokter.DBAccess;
using dokter.forms;
using dokter.models;

namespace dokter.views
{
    /// <summary>
    ///     Interaction logic for DataPsien.xaml
    /// </summary>
    public partial class DataPsien : Page
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;

        public DataPsien()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            DisplayDataPasien();
        }

        public void DisplayDataPasien(string no_id = null)
        {
            var pasien = cmd.GetDataPasien();

            if (string.IsNullOrEmpty(no_id))
            {
                dtgDataPasien.ItemsSource = pasien;
            }
            else
            {
                var filtered = pasien.Where(x => x.id.Contains(no_id));
                dtgDataPasien.ItemsSource = filtered;
            }
        }

        private void TxtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nama = sender as TextBox;

            if (nama.Text != "No Identitas") DisplayDataPasien(nama.Text);
        }

        private void TxtSearchPasien_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            string no_rm = null;

            if (dtgDataPasien.SelectedItems.Count > 0)
            {
                foreach (ModelPasien mp in dtgDataPasien.SelectedItems) no_rm = mp.no_rm;

                var dp = new DetailPasien(no_rm);
                dp.Show();
            }
            else
            {
                MessageBox.Show("Pilih data pasien terlebih dahulu.", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var no_rm = "";
            if (dtgDataPasien.SelectedItems.Count > 0)
            {
                foreach (ModelPasien dp in dtgDataPasien.SelectedItems) no_rm = dp.no_rm;

                var rv = new ReportView(no_rm);
                rv.Show();
            }
            else
            {
                MessageBox.Show("Pilih data yang akan di print", "Perhatian", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }
}