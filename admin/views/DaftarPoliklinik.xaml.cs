using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.forms;

namespace admin.views
{
    /// <summary>
    ///     Interaction logic for DaftarPoliklinik.xaml
    /// </summary>
    public partial class DaftarPoliklinik : Page
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;

        public DaftarPoliklinik()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            displayDataPoliklinik();
        }

        public void displayDataPoliklinik(string nama = null)
        {
            var poliklinik = cmd.GetDataPoliKlinik();

            if (string.IsNullOrEmpty(nama))
            {
                dtgDataPoliklinik.ItemsSource = poliklinik;
            }
            else
            {
                var cbp = poliklinik.Where(x => x.nama_poliklinik.ToLower().Contains(nama.ToLower()));
                dtgDataPoliklinik.ItemsSource = cbp;
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

            if (nama.Text != "Poliklinik") displayDataPoliklinik(nama.Text);
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

                var res = false;

                if (a == MessageBoxResult.Yes)
                    for (var i = 0; i < dtgDataPoliklinik.SelectedItems.Count; i++)
                        if (cmd.DeleteDataPoliklinik((dtgDataPoliklinik.SelectedCells[0].Column
                            .GetCellContent(dtgDataPoliklinik.SelectedItems[i]) as TextBlock)?.Text))
                            res = true;

                if (res)
                    MessageBox.Show("Data poliklinik berhasil dihapus.", "Info", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                else
                    MessageBox.Show("Data poliklinik gagal dihapus.", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                displayDataPoliklinik();
                DBConnection.dbConnection().Close();
            }
            else
            {
                MessageBox.Show("Pilih data poliklinik yang akan dihapus.", "Info", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}