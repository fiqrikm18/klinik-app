using Apotik.DBAccess;
using Apotik.models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Apotik.views
{
    /// <summary>
    ///     Interaction logic for TambahObat.xaml
    /// </summary>
    public partial class TambahObat : Page
    {
        private int _noOfErrorsOnScreen;
        private ModelObat _mObat = new ModelObat(" ", " ", " ", " ", " ", " ", " ");

        public TambahObat()
        {
            InitializeComponent();
            DataContext = new ModelObat(" ", " ", " ", " ", " ", " ", " ");
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

                DBCommand cmd = new DBCommand(DBConnection.dbConnection());
                var kodeExist = cmd.GetCountDataObat(kode_obat);
                
                if(kodeExist >= 1)
                {
                    MessageBox.Show("Kode sudah digunakan.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    var res = cmd.InsertDataObat(kode_obat, nama_obat, satuan, stok, harga_jual, harga_beli, harga_resep);

                    if(res)
                        MessageBox.Show("Data obat berhasil disimpan.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show("Data obat gagal disimpan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Pastikan data yang diinput sudah benar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _mObat = new ModelObat(" ", " ", " ", " ", " ", " ", " ");
            DataContext = _mObat;
            e.Handled = true;
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            if (string.IsNullOrEmpty(source.Text) || string.IsNullOrWhiteSpace(source.Text))
            {
                source.Clear();
            }
        }

        private bool checkTextBoxValue()
        {
            if (!string.IsNullOrWhiteSpace(txtKodeObat.Text) && !string.IsNullOrWhiteSpace(txtNamaObat.Text) 
                && !string.IsNullOrWhiteSpace(txtHargaBeli.Text) && !string.IsNullOrWhiteSpace(txtHargaJual.Text)
                && !string.IsNullOrWhiteSpace(txtHargaResep.Text)&& !string.IsNullOrWhiteSpace(txtStok.Text))
                return true;

            return false;
        }
    }
}