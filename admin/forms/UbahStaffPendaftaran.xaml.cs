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
    ///     Interaction logic for UbahStaffPendaftaran.xaml
    /// </summary>
    public partial class UbahStaffPendaftaran : Window
    {
        private MPendaftaran _mDaftarBaru = new MPendaftaran(" ", " ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;
        private readonly DaftarPendaftaran df;

        public UbahStaffPendaftaran(string id, string nama, string alamat, string telp, string jenisK,
            DaftarPendaftaran df)
        {
            InitializeComponent();
            DataContext = new MPendaftaran(id, nama, alamat, telp, " ", jenisK);

            this.df = df;
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

        private void AddDokter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddDokter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mDaftarBaru = new MPendaftaran(" ", " ", " ", " ", " ", " ");

            if (checkTextBoxValue())
            {
                var nama = txtNamaDokter.Text;
                var id = txtidDokter.Text.ToUpper();
                var telp = txtTelpDokter.Text;
                var alamat = TextAlamat.Text;
                var jenisK = cbJenisKelamin.Text;

                try
                {
                    if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    var query =
                        "update pendaftar set nama='" + nama + "', alamat='" + alamat + "', telp='" + telp +
                        "', jenis_kelamin='" + jenisK + "' where id='" + id + "'";
                    var command = new MySqlCommand(query, DBConnection.dbConnection());
                    var res = command.ExecuteNonQuery();

                    if (res == 1)
                    {
                        MessageBox.Show("Data staff berhasil disimpan.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        df.displayDataPendaftar();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Data staff gagal disimpan.", "Error", MessageBoxButton.OK,
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

            cbJenisKelamin.SelectedIndex = 0;
            e.Handled = true;
        }

        private bool checkTextBoxValue()
        {
//            if (txtidDokter.Text == " " && txtNamaDokter.Text == " " && txtTelpDokter.Text == " " &&
//                txtSpesialisai.Text == " " && TextAlamat.Text == " ") return false;

            if (!string.IsNullOrWhiteSpace(txtidDokter.Text) && !string.IsNullOrWhiteSpace(txtNamaDokter.Text) &&
                !string.IsNullOrWhiteSpace(txtTelpDokter.Text) &&
                !string.IsNullOrWhiteSpace(TextAlamat.Text))
                return true;

            return false;
        }

        #endregion
    }
}