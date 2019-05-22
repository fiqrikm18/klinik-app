using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.models;
using admin.views;
using MySql.Data.MySqlClient;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for UbahApoteker.xaml
    /// </summary>
    public partial class UbahApoteker : Window
    {
        private readonly DaftarApoteker da;
        private MApoteker _mDaftarBaru = new MApoteker(" ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;

        public UbahApoteker(string id, string nama, string alamat, string no_telp, string jenisK, DaftarApoteker ua)
        {
            InitializeComponent();

            DataContext = new MApoteker(id, nama, no_telp, alamat, " ");
            da = ua;

            if (jenisK == "Pria") cbJenisKelamin.SelectedIndex = 0;
            else if (jenisK == "Wanita") cbJenisKelamin.SelectedIndex = 1;
        }

        private void BtnBatal_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region members UI Control & CRUD Operations

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void AddApoteker_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddApoteker_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mDaftarBaru = new MApoteker(" ", " ", " ", " ", " ");

            if (checkTextBoxValue())
            {
                var nama = txtNamaDokter.Text;
                var id = txtidDokter.Text;
                var telp = txtTelpDokter.Text;
                var alamat = TextAlamat.Text;
                var jenisK = cbJenisKelamin.Text;

                try
                {
                    if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    var query = "update apoteker set nama_apoteker='" + nama + "', no_telp='" + telp + "', alamat='" +
                                alamat + "', jenis_kelamin='" + jenisK + "' where id_apoteker='" + id + "'";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var res = cmd.ExecuteNonQuery();

                    if (res >= 1)
                    {
                        MessageBox.Show("Berhasil memperbarui data apoteker", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        da.displayDataApoteker();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Gagal memperbarui data apoteker", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Periksa kembali data yang akan di inputkan.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            DataContext = _mDaftarBaru;
            e.Handled = true;
        }

        private bool checkTextBoxValue()
        {
//            if (txtidDokter.Text == " " && txtNamaDokter.Text == " " && txtTelpDokter.Text == " " &&
//                txtSpesialisai.Text == " " && TextAlamat.Text == " ") return false;

            if (!string.IsNullOrWhiteSpace(txtidDokter.Text) && !string.IsNullOrWhiteSpace(txtNamaDokter.Text) &&
                !string.IsNullOrWhiteSpace(txtTelpDokter.Text) && !string.IsNullOrWhiteSpace(TextAlamat.Text))
                return true;

            return false;
        }

        #endregion
    }
}