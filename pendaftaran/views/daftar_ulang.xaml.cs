using System;
using System.Collections.Generic;
using System.Data;
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

using MySql.Data.MySqlClient;
using pendaftaran.DBAccess;

namespace pendaftaran.views
{
    /// <summary>
    /// Interaction logic for daftar_ulang.xaml
    /// </summary>
    public partial class daftar_ulang : Page
    {
        public daftar_ulang()
        {
            InitializeComponent();

            try
            {
                if (DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                {
                    DBConnection.dbConnection().Open();
                }

                string query = "select * from pasien";
                MySqlCommand cmd = new MySqlCommand(query, DBConnection.dbConnection());
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                adapter.Fill(dt);
                dtgDataPasien.ItemsSource = dt.DefaultView;
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n"+ex.Message, "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox source = e.Source as TextBox;
            source.Clear();
        }

        private void UbahDataPasien(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ubah data pasien");
        }

        private void TambahPasien(object sender, RoutedEventArgs e)
        {
            daftar_baru db = new daftar_baru();
            NavigationService.Navigate(db);
        }

        private void HapusDataPasien(object sender, RoutedEventArgs e)
        {
            if(dtgDataPasien.SelectedCells.Count > 0)
            {
                //object item = dtgDataPasien.SelectedItem;
                //string id = (dtgDataPasien.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text.ToString();

                //MessageBox.Show(id);
                for(int i =0; i < dtgDataPasien.SelectedItems.Count; i++)
                {
                    MessageBox.Show((this.dtgDataPasien.SelectedCells[0].Column.GetCellContent(this.dtgDataPasien.SelectedItems[i]) as TextBlock).Text);
                }
            }
        }
    }
}
