using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Apotik.DBAccess;
using Apotik.forms;
using Apotik.models;

namespace Apotik.views
{
    /// <summary>
    ///     Interaction logic for DaftarObat.xaml
    /// </summary>
    public partial class DaftarObat : Page
    {
        private readonly SqlConnection conn;

        public DaftarObat()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            DisplayDataObat();
        }

        public void DisplayDataObat(string nama = null)
        {
            try
            {
                var cmd = new DBCommand(conn);

                if (nama == null)
                {
                    var data = cmd.GetDataObat();
                    dtgDataObat.ItemsSource = data;
                }
                else
                {
                    //SqlCommand command = new SqlCommand("SELECT * FROM tb_obat WHERE nama_obat LIKE '%" + nama + "%'", conn);
                    ////command.Parameters.AddWithValue("nama", nama);
                    //var adapter = new SqlDataAdapter(command);
                    //var dt = new DataTable();

                    //adapter.Fill(dt);
                    //dtgDataObat.ItemsSource = dt.DefaultView;

                    var data = cmd.GetDataObat();
                    var dataFiltered = data.Where(x => x.nama_obat.Contains(nama));
                    dtgDataObat.ItemsSource = dataFiltered;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            var to = new TambahObat();
            NavigationService.Navigate(to);
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
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

                var uo = new UpdateObat(kode_obat, nama_obat, stok, satuan, harga_jual, harga_beli, harga_resep, this);
                uo.Show();
            }
            else
            {
                MessageBox.Show("Pilih data yang ingin diubah.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_hapus_Click(object sender, RoutedEventArgs e)
        {
            var cmd = new DBCommand(conn);
            var kode_obat = "";

            if (dtgDataObat.SelectedItems.Count > 0)
            {
                var msgResult = MessageBox.Show("Apakah anda yakin ingin menghapus data obat?", "Warning",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (msgResult == MessageBoxResult.Yes)
                {
                    foreach (ModelObat mo in dtgDataObat.SelectedItems) kode_obat = mo.kode_obat;

                    if (cmd.DeleteDataObat(kode_obat))
                    {
                        MessageBox.Show("Data obat berhasil dihapus.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        DisplayDataObat();
                    }
                    else
                    {
                        MessageBox.Show("Data obat gagal dihapus.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih data obat yang akan dihapus terlebih  dahulu.", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }
}