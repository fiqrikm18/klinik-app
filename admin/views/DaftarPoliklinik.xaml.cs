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
    ///     Interaction logic for DaftarPoliklinik.xaml
    /// </summary>
    public partial class DaftarPoliklinik : Page
    {
        public DaftarPoliklinik()
        {
            InitializeComponent();
            displayDataPoliklinik();
        }

        public void displayDataPoliklinik(string nama = null)
        {
            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();

                string query;

                if (!string.IsNullOrEmpty(nama))
                {
                    query =
                        "select * from poliklinik where nama_poliklinik like '%" + nama + "%';";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataPoliklinik.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
                else
                {
                    query =
                        "select * from poliklinik";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataPoliklinik.ItemsSource = dt.DefaultView;

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

            if (nama.Text != "Poliklinik")
                displayDataPoliklinik(nama.Text);
        }

        private void BtnTambahPoli_OnClick(object sender, RoutedEventArgs e)
        {
            var tp = new TambahPoliklinik(this);
            tp.Show();
        }

        private void BtnHapusPoli_OnClick(object sender, RoutedEventArgs e)
        {
            if (dtgDataPoliklinik.SelectedItems.Count > 0)
            {
                var a = MessageBox.Show("Anda yakin ingin menghapus data poliklinik?", "Konfirmasi",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (a == MessageBoxResult.Yes)
                {
                    string query;
                    var res = 0;

                    if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    try
                    {
                        for (var i = 0; i < dtgDataPoliklinik.SelectedItems.Count; i++)
                        {
                            query = "delete from poliklinik where kode_poliklinik = '" +
                                    (dtgDataPoliklinik.SelectedCells[0].Column
                                        .GetCellContent(dtgDataPoliklinik.SelectedItems[i]) as TextBlock)?.Text + "';";
                            var command = new MySqlCommand(query, DBConnection.dbConnection());
                            res = command.ExecuteNonQuery();
                        }

                        if (res == 1)
                            MessageBox.Show("Data poliklinik berhasil dihapus.", "Informasi", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        else
                            MessageBox.Show("Data poliklinik gagal dihapus.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                displayDataPoliklinik();
                DBConnection.dbConnection().Close();
            }
            else
            {
                MessageBox.Show("Pilih data poliklinik yang akan dihapus.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}