using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Apotik.DBAccess;
using Apotik.models;
using Apotik.views;

namespace Apotik.forms
{
    /// <summary>
    ///     Interaction logic for UpdateObat.xaml
    /// </summary>
    public partial class UpdateObat : Window
    {
        private readonly DaftarObat daftarObat = new DaftarObat();
        private readonly ModelObat mo = new ModelObat(" ", " ", " ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;

        public UpdateObat(string kode_obat, string nama_obat, string stok, string satuan, string harga_jual,
            string harga_beli, string harga_resep, DaftarObat d)
        {
            InitializeComponent();
            mo = new ModelObat(kode_obat, nama_obat, harga_beli, harga_beli, harga_resep, stok, satuan);
            daftarObat = d;
            DataContext = mo;
            cbSatuan.Text = satuan;
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;
        }

        private void AddObat_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddObat_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (checkTextBoxValue())
            {
                var kode_obat = txtKodeObat.Text;
                var nama_obat = txtNamaObat.Text;
                var stok = txtStok.Text;
                var harga_beli = txtHargaBeli.Text;
                var harga_jual = txtHargaJual.Text;
                var harga_resep = txtHargaResep.Text;
                var satuan = cbSatuan.Text;

                if (!Regex.IsMatch(harga_beli, "^[A-Za-z]+$") && !Regex.IsMatch(harga_jual, "^[A-Za-z]+$") &&
                    !Regex.IsMatch(harga_resep, "^[A-Za-z]+$") && !Regex.IsMatch(stok, "^[A-Za-z]+$"))
                {
                    var cmd = new DBCommand(DBConnection.dbConnection());

                    var res = cmd.UpdateDataObat(kode_obat, nama_obat, satuan, stok, harga_jual, harga_beli,
                        harga_resep);

                    if (res)
                    {
                        MessageBox.Show("Data obat berhasil update.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        daftarObat.DisplayDataObat();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Data obat gagal update.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Pastikan data yang di inputkan sudah benar", "Perhatian", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Pastikan data yang diinput sudah benar.", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            //mo = new ModelObat(" ", " ", " ", " ", " ", " ", " ");
            DataContext = mo;
            e.Handled = true;
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            if (string.IsNullOrEmpty(source.Text) || string.IsNullOrWhiteSpace(source.Text)) source.Clear();
        }

        private bool checkTextBoxValue()
        {
            if (!string.IsNullOrEmpty(txtKodeObat.Text) && !string.IsNullOrEmpty(txtNamaObat.Text) &&
                !string.IsNullOrEmpty(txtHargaBeli.Text) && !string.IsNullOrEmpty(txtHargaJual.Text)
                && !string.IsNullOrEmpty(txtHargaResep.Text) && !string.IsNullOrEmpty(txtStok.Text))
                return true;

            return false;
        }
    }
}