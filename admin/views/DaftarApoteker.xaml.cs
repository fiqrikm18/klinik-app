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
    ///     Interaction logic for DaftarApoteker.xaml
    /// </summary>
    public partial class DaftarApoteker : Page
    {
        public DaftarApoteker()
        {
            InitializeComponent();
            displayDataApoteker();
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void TxtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nama = sender as TextBox;

            if (nama.Text != "Nama Apoteker")
                displayDataApoteker(nama.Text);
        }

        public void displayDataApoteker(string nama = null)
        {
            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();

                string query;

                if (!string.IsNullOrEmpty(nama))
                {
                    query = "select * from apoteker where nama_apoteker like '%" + nama + "%';";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataApoteker.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
                else
                {
                    query = "select * from apoteker";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataApoteker.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message,
                    "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnTambahApoteker_OnClick(object sender, RoutedEventArgs e)
        {
            var ta = new TambahApoteker(this);
            ta.Show();
        }

        private void BtnUbahApoteker_OnClick(object sender, RoutedEventArgs e)
        {
            var id = "";
            var nama = "";
            var telp = "";
            var alamat = "";
            var jenisK = "";

            if (dtgDataApoteker.SelectedItems.Count > 0)
                for (var i = 0; i < dtgDataApoteker.SelectedItems.Count; i++)
                {
                    id = (dtgDataApoteker.SelectedCells[0].Column
                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock).Text;
                    nama = (dtgDataApoteker.SelectedCells[1].Column
                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock).Text;
                    jenisK = (dtgDataApoteker.SelectedCells[2].Column
                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock).Text;
                    telp = (dtgDataApoteker.SelectedCells[3].Column
                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock).Text;
                    alamat = (dtgDataApoteker.SelectedCells[4].Column
                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock).Text;
                }

            var ua = new UbahApoteker(id, nama, alamat, telp, jenisK, this);
            ua.Show();
        }

        private void BtnHapusApoteker_OnClick(object sender, RoutedEventArgs e)
        {
            if (dtgDataApoteker.SelectedItems.Count > 0)
            {
                var a = MessageBox.Show("Anda yakin ingin menghapus data apoteker?", "Konfirmasi",
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
                        for (var i = 0; i < dtgDataApoteker.SelectedItems.Count; i++)
                        {
                            query = "delete from apoteker where id_apoteker = '" +
                                    (dtgDataApoteker.SelectedCells[0].Column
                                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock)?.Text + "';";
                            var command = new MySqlCommand(query, DBConnection.dbConnection());
                            res = command.ExecuteNonQuery();
                        }

                        if (res == 1)
                            MessageBox.Show("Data apoteker berhasil dihapus.", "Informasi", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        else
                            MessageBox.Show("Data apoteker gagal dihapus.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                displayDataApoteker();
                DBConnection.dbConnection().Close();
            }
            else
            {
                MessageBox.Show("Pilih data apoteker yang akan dihapus.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}