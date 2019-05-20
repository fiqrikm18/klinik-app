using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

using MySql.Data.MySqlClient;
using pendaftaran.DBAccess;

namespace pendaftaran.views
{
    /// <summary>
    /// Interaction logic for daftar_ulang.xaml
    /// </summary>
    public partial class daftar_ulang : Page
    {
        public string normP;
        public string noidP;
        public string namaP;
        public string jenisK;
        public string noTelp;
        public string alamat;

        public daftar_ulang()
        {
            InitializeComponent();
            displayDataPasien();
        }

        public void displayDataPasien(string nama = null)
        {
            try
            {
                if (DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                {
                    DBConnection.dbConnection().Open();
                }

                string query;

                if(nama != null)
                {
                    query = "select * from pasien where nama like '%" + nama + "%';";
                    MySqlCommand cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataPasien.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
                else
                {
                    query = "select * from pasien";
                    MySqlCommand cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataPasien.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message, "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox source = e.Source as TextBox;
            source.Clear();
        }

        private void UbahDataPasien(object sender, RoutedEventArgs e)
        {
            //(dtgDataPasien.SelectedCells[0].Column.GetCellContent(this.dtgDataPasien.SelectedItems[i]) as TextBlock).Text
            for (int i = 0; i < dtgDataPasien.SelectedItems.Count; i++)
            {
                normP = (dtgDataPasien.SelectedCells[1].Column.GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock).Text;
                noidP = (dtgDataPasien.SelectedCells[0].Column.GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock).Text;
                namaP = (dtgDataPasien.SelectedCells[2].Column.GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock).Text;
                jenisK = (dtgDataPasien.SelectedCells[3].Column.GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock).Text;
                noTelp = (dtgDataPasien.SelectedCells[4].Column.GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock).Text;
                alamat = (dtgDataPasien.SelectedCells[5].Column.GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock).Text;
            }

            forms.ubah_dataPasien ud = new forms.ubah_dataPasien(normP, noidP, namaP, jenisK, noTelp, alamat, this);
            ud.Show();
        }

        private void TambahPasien(object sender, RoutedEventArgs e)
        {
            daftar_baru db = new daftar_baru();
            NavigationService.Navigate(db);
        }

        private void HapusDataPasien(object sender, RoutedEventArgs e)
        {
            if (dtgDataPasien.SelectedCells.Count > 0)
            {
                //object item = dtgDataPasien.SelectedItem;
                //string id = (dtgDataPasien.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text.ToString();
                //MessageBox.Show(id);

                MessageBoxResult a = MessageBox.Show("Anda yakin ingin menghapus data pasien?", "Konfirmasi", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (a == MessageBoxResult.Yes)
                {
                    string query;
                    DBConnection.dbConnection().Open();
                    int res = 1;

                    try
                    {
                        for (int i = 0; i < dtgDataPasien.SelectedItems.Count; i++)
                        {
                            //MessageBox.Show((this.dtgDataPasien.SelectedCells[0].Column.GetCellContent(this.dtgDataPasien.SelectedItems[i]) as TextBlock).Text);
                            query = "delete from pasien where no_rekam_medis = '" + (dtgDataPasien.SelectedCells[0].Column.GetCellContent(this.dtgDataPasien.SelectedItems[i]) as TextBlock).Text + "';";
                            MySqlCommand command = new MySqlCommand(query, DBConnection.dbConnection());
                            res = command.ExecuteNonQuery();
                        }

                        if (res == 1)
                        {
                            MessageBox.Show("Data pasien berhasil dihapus.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                            MessageBox.Show("Data pasien gagal dihapus.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Data pasien gagal dihapus.\n" + ex.Message, "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                }

                displayDataPasien();
                DBConnection.dbConnection().Close();
            }
            else
            {
                MessageBox.Show("Pilih data pasien yang akan dihapus.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nama = sender as TextBox;

            if(nama.Text != "Nama Pasien")
                displayDataPasien(nama.Text.ToString());
        }
    }
}
