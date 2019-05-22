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
    ///     Interaction logic for DaftarDokter.xaml
    /// </summary>
    public partial class DaftarDokter : Page
    {
        public DaftarDokter()
        {
            InitializeComponent();
            displayDataDokter();
        }

        public void displayDataDokter(string nama = null)
        {
            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();

                string query;

                if (!string.IsNullOrEmpty(nama))
                {
                    query =
                        "select dokter.nama as nama, dokter.telp as telp, dokter.id as id, dokter.alamat as alamat, dokter.spesialisasi as spesialisasi, poliklinik.nama_poliklinik as tugas, dokter.jenis_kelamin as jenis_kelamin  from dokter LEFT JOIN poliklinik on poliklinik.kode_poliklinik = dokter.tugas where nama like '%" +
                        nama + "%';";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataDokter.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
                else
                {
                    query =
                        "select dokter.nama as nama, dokter.telp as telp, dokter.id as id, dokter.alamat as alamat, dokter.spesialisasi as spesialisasi, poliklinik.nama_poliklinik as tugas, dokter.jenis_kelamin as jenis_kelamin  from dokter LEFT JOIN poliklinik on poliklinik.kode_poliklinik = dokter.tugas;";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataDokter.ItemsSource = dt.DefaultView;

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

            if (nama.Text != "Nama Dokter")
                displayDataDokter(nama.Text);
        }

        private void BtnTambahDokter_OnClick(object sender, RoutedEventArgs e)
        {
            var td = new TambahDokter(this);
            td.Show();
        }

        private void BtnUbahDokter_OnClick(object sender, RoutedEventArgs e)
        {
            var id = "";
            var nama = "";
            var telp = "";
            var alamat = "";
            var spesialis = "";
            var tugas = "";
            var jenisK = "";

            if (dtgDataDokter.SelectedItems.Count > 0)
            {
                for (var i = 0; i < dtgDataDokter.SelectedItems.Count; i++)
                {
                    id = (dtgDataDokter.SelectedCells[0].Column
                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
                    nama = (dtgDataDokter.SelectedCells[1].Column
                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
                    jenisK = (dtgDataDokter.SelectedCells[2].Column
                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
                    telp = (dtgDataDokter.SelectedCells[3].Column
                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
                    alamat = (dtgDataDokter.SelectedCells[4].Column
                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
                    spesialis = (dtgDataDokter.SelectedCells[5].Column
                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
                    tugas = (dtgDataDokter.SelectedCells[6].Column
                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
                }

                var ud = new UbahDokter(id, nama, telp, alamat, spesialis, jenisK, this);
                ud.Show();
            }
        }

        private void BtnHapusDokter_OnClick(object sender, RoutedEventArgs e)
        {
            if (dtgDataDokter.SelectedItems.Count > 0)
            {
                var a = MessageBox.Show("Anda yakin ingin menghapus data dokter?", "Konfirmasi", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (a == MessageBoxResult.Yes)
                {
                    string query;
                    var res = 0;

                    if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    try
                    {
                        for (var i = 0; i < dtgDataDokter.SelectedItems.Count; i++)
                        {
                            query = "delete from dokter where id = '" +
                                    (dtgDataDokter.SelectedCells[0].Column
                                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock)?.Text + "';";
                            var command = new MySqlCommand(query, DBConnection.dbConnection());
                            res = command.ExecuteNonQuery();
                        }

                        if (res == 1)
                            MessageBox.Show("Data dokter berhasil dihapus.", "Informasi", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        else
                            MessageBox.Show("Data dokter gagal dihapus.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                displayDataDokter();
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