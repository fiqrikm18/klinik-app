using dokter.DBAccess;
using dokter.models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace dokter.views
{
    /// <summary>
    /// Interaction logic for DataPsien.xaml
    /// </summary>
    public partial class DataPsien : Page
    {
        DBCommand cmd;
        SqlConnection conn;

        public DataPsien()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            DisplayDataPasien();
        }

        public void DisplayDataPasien(string no_id = null)
        {
            List<ModelPasien> pasien = cmd.GetDataPasien();

            if (string.IsNullOrEmpty(no_id))
            {
                dtgDataPasien.ItemsSource = pasien;
            }
            else
            {
                IEnumerable<ModelPasien> filtered = pasien.Where(x => x.id.Contains(no_id));
                dtgDataPasien.ItemsSource = filtered;
            }
        }

        private void TxtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nama = sender as TextBox;

            if (nama.Text != "No Identitas")
            {
                DisplayDataPasien(nama.Text);
            }
        }

        private void TxtSearchPasien_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void btnDetail_Click(object sender, RoutedEventArgs e)
        {
            // TODO: buat fungsi untuk melihat data detail pasien
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            // TODO: buat fungsi untuk print data pasien
        }
    }
}
