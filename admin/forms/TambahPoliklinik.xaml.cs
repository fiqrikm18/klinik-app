using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.models;
using admin.views;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for TambahPoliklinik.xaml
    /// </summary>
    public partial class TambahPoliklinik : Window
    {
        private readonly DBCommand cmd;

        private readonly SqlConnection conn;
        private readonly DaftarPoliklinik dp;
        private MPoliklinik _mDaftarBaru = new MPoliklinik(" ", " ");
        private int _noOfErrorsOnScreen;

        public TambahPoliklinik(DaftarPoliklinik dp)
        {
            InitializeComponent();
            this.dp = dp;

            DataContext = _mDaftarBaru;

            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
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
            if (string.IsNullOrEmpty(source.Text) || string.IsNullOrWhiteSpace(source.Text) || source.Text == " ")
                source.Clear();
        }

        private void AddPoliklinik_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddPoliklinik_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mDaftarBaru = new MPoliklinik(" ", " ");

            if (checkTextBoxValue())
            {
                if (Regex.IsMatch(txtNamaDokter.Text, @"^[a-zA-Z\s]*$"))
                {
                    var nama = txtNamaDokter.Text;
                    var id = txtidDokter.Text.ToUpper();

                    if (cmd.CheckPoliExsist(id) == 1)
                    {
                        MessageBox.Show("Kode poliklinik sudah terdaftar.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }

                    else
                    {
                        if (cmd.InsertDataPoliklinik(id, nama))
                        {
                            MessageBox.Show("Data poliklinik berhasil disimpan.", "Informasi", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                            dp.displayDataPoliklinik();
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Data poliklinik gagal disimpan.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Periksa kembali data yang akan di inputkan.", "Informasi", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Periksa kembali data yang akan di inputkan.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            e.Handled = true;
        }

        private bool checkTextBoxValue()
        {
//            if (txtidDokter.Text == " " && txtNamaDokter.Text == " " && txtTelpDokter.Text == " " &&
//                txtSpesialisai.Text == " " && TextAlamat.Text == " ") return false;

            if (!string.IsNullOrWhiteSpace(txtidDokter.Text) && !string.IsNullOrWhiteSpace(txtNamaDokter.Text))
                return true;

            return false;
        }

        #endregion
    }
}