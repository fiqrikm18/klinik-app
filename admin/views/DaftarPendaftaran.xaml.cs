using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.forms;
using MySql.Data.MySqlClient;

namespace admin.views
{
    /// <summary>
    ///     Interaction logic for DaftarPendaftaran.xaml
    /// </summary>
    public partial class DaftarPendaftaran : Page
    {
        public DaftarPendaftaran()
        {
            InitializeComponent();
            displayDataPendaftar();
        }

        public void displayDataPendaftar(string nama = null)
        {
            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();

                string query;

                if (!string.IsNullOrEmpty(nama))
                {
                    query =
                        "select * from pendaftar where nama like '%" + nama + "%';";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataPendaftar.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
                else
                {
                    query =
                        "select * from pendaftar";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataPendaftar.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message,
                    "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void TxtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nama = sender as TextBox;

            if (nama.Text != "Nama Staff Pendaftaran")
                displayDataPendaftar(nama.Text);
        }

        private void BtnTambahStaff_OnClick(object sender, RoutedEventArgs e)
        {
            var tsp = new TambahStaffPendaftaran(this);
            tsp.Show();
        }

        private void BtnUbahStaff_OnClick(object sender, RoutedEventArgs e)
        {
            var id = "";
            var nama = "";
            var telp = "";
            var alamat = "";
            var jenisK = "";

            if (dtgDataPendaftar.SelectedItems.Count > 0)
            {
                for (var i = 0; i < dtgDataPendaftar.SelectedItems.Count; i++)
                {
                    id = (dtgDataPendaftar.SelectedCells[0].Column
                        .GetCellContent(dtgDataPendaftar.SelectedItems[i]) as TextBlock).Text;
                    nama = (dtgDataPendaftar.SelectedCells[1].Column
                        .GetCellContent(dtgDataPendaftar.SelectedItems[i]) as TextBlock).Text;
                    jenisK = (dtgDataPendaftar.SelectedCells[2].Column
                        .GetCellContent(dtgDataPendaftar.SelectedItems[i]) as TextBlock).Text;
                    telp = (dtgDataPendaftar.SelectedCells[3].Column
                        .GetCellContent(dtgDataPendaftar.SelectedItems[i]) as TextBlock).Text;
                    alamat = (dtgDataPendaftar.SelectedCells[4].Column
                        .GetCellContent(dtgDataPendaftar.SelectedItems[i]) as TextBlock).Text;
                }

                var ud = new UbahStaffPendaftaran(id, nama, alamat, telp, jenisK, this);
                ud.Show();
            }
        }

        private void BtnHapusStaff_OnClick(object sender, RoutedEventArgs e)
        {
            if (dtgDataPendaftar.SelectedItems.Count > 0)
            {
                var a = MessageBox.Show("Anda yakin ingin menghapus data staff?", "Konfirmasi", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (a == MessageBoxResult.Yes)
                {
                    string query;
                    var res = 0;

                    if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    try
                    {
                        for (var i = 0; i < dtgDataPendaftar.SelectedItems.Count; i++)
                        {
                            query = "delete from pendaftar where id = '" +
                                    (dtgDataPendaftar.SelectedCells[0].Column
                                        .GetCellContent(dtgDataPendaftar.SelectedItems[i]) as TextBlock)?.Text + "';";
                            var command = new MySqlCommand(query, DBConnection.dbConnection());
                            res = command.ExecuteNonQuery();
                        }

                        if (res == 1)
                            MessageBox.Show("Data staff berhasil dihapus.", "Informasi", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        else
                            MessageBox.Show("Data staff gagal dihapus.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                displayDataPendaftar();
                DBConnection.dbConnection().Close();
            }
            else
            {
                MessageBox.Show("Pilih data dokter yang akan dihapus.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}