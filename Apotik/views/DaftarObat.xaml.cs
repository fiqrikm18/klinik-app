using System.Windows.Controls;
using System.Data.SqlClient;
using Apotik.DBAccess;
using System.Windows.Input;
using System.Windows.Data;
using System;
using System.Data;
using System.Windows;

namespace Apotik.views
{
    /// <summary>
    ///     Interaction logic for DaftarObat.xaml
    /// </summary>
    public partial class DaftarObat : Page
    {
        SqlConnection conn;
        public DaftarObat()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            DisplayDataObat();
        }

        public void DisplayDataObat(string nama = null)
        {
            DBCommand cmd = new DBCommand(conn);

            if (nama == null)
            {
                var data = cmd.GetDataObat();
                dtgDataObat.ItemsSource = data;
            }
            else
            {
                SqlCommand command = new SqlCommand("SELECT * FROM tb_obat WHERE nama_obat LIKE '%"+nama+"%'", conn);
                //command.Parameters.AddWithValue("nama", nama);
                var adapter = new SqlDataAdapter(command);
                var dt = new DataTable();

                adapter.Fill(dt);
                dtgDataObat.ItemsSource = dt.DefaultView;
            }
        }

        private void TxtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nama = sender as TextBox;

            if (nama.Text != "Nama Obat")
                DisplayDataObat(nama.Text);
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void btn_add_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var to = new TambahObat();
            NavigationService.Navigate(to);
        }

        private void btn_edit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var kode_obat = "";
            var nama_obat = "";
            var stok = "";
            var satuan = "";
            var harga_jual = "";
            var harga_beli = "";
            var harga_resep = "";

            if (dtgDataObat.SelectedItems.Count > 0)
            {
                for (var i = 0; i < dtgDataObat.SelectedItems.Count; i++)
                {
                    kode_obat =
                        (dtgDataObat.SelectedCells[0].Column
                            .GetCellContent(dtgDataObat.SelectedItems[i]) as TextBlock)
                        .Text;
                    nama_obat =
                        (dtgDataObat.SelectedCells[1].Column
                            .GetCellContent(dtgDataObat.SelectedItems[i]) as TextBlock)
                        .Text;
                    stok =
                        (dtgDataObat.SelectedCells[2].Column
                            .GetCellContent(dtgDataObat.SelectedItems[i]) as TextBlock)
                        .Text;
                    satuan =
                        (dtgDataObat.SelectedCells[3].Column
                            .GetCellContent(dtgDataObat.SelectedItems[i]) as TextBlock)
                        .Text;
                    harga_beli =
                        (dtgDataObat.SelectedCells[4].Column
                            .GetCellContent(dtgDataObat.SelectedItems[i]) as TextBlock)
                        .Text;
                    harga_jual =
                        (dtgDataObat.SelectedCells[5].Column
                            .GetCellContent(dtgDataObat.SelectedItems[i]) as TextBlock)
                        .Text;
                    harga_resep =
                        (dtgDataObat.SelectedCells[6].Column
                            .GetCellContent(dtgDataObat.SelectedItems[i]) as TextBlock)
                        .Text;
                }

                var uo = new forms.UpdateObat(kode_obat, nama_obat, stok, satuan, harga_jual, harga_beli, harga_resep, this);
                uo.Show();
            }
            else
            {
                MessageBox.Show("Pilih data yang ingin diubah.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_hapus_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}