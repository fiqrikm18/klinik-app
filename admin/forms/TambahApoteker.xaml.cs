using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.models;
using admin.Utils;
using admin.views;
using MySql.Data.MySqlClient;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for TambahApoteker.xaml
    /// </summary>
    public partial class TambahApoteker : Window
    {
        private readonly DaftarApoteker da;
        private MApoteker _mDaftarBaru = new MApoteker(" ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;

        public TambahApoteker(DaftarApoteker da)
        {
            InitializeComponent();
            this.da = da;
            DataContext = _mDaftarBaru;
        }

        private void BtnBatal_OnClick(object sender, RoutedEventArgs e)
        {
            da.displayDataApoteker();
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
                var id = txtidDokter.Text.ToUpper();
                var nama = txtNamaDokter.Text;
                var alamat = TextAlamat.Text;
                var no_telp = txtTelpDokter.Text;
                var jenis_kelamin = cbJenisKelamin.Text;
                var password = txtPassword.Text.ToUpper();

                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();

                try
                {
                    var query = "select count(*) from apoteker where id_apoteker = '" + id + "'";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var idExist = int.Parse(cmd.ExecuteScalar().ToString());

                    if (idExist >= 1)
                    {
                        MessageBox.Show("Id sudah terdaftar.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                    else
                    {
                        query =
                            "insert into apoteker(id_apoteker, nama_apoteker, no_telp, alamat, jenis_kelamin, password) VALUES('" +
                            id + "', '" + nama + "', '" + no_telp + "', '" + alamat + "', '" + jenis_kelamin + "', '" +
                            Encryptor.MD5Hash(password) + "')";
                        cmd = new MySqlCommand(query, DBConnection.dbConnection());
                        var res = cmd.ExecuteNonQuery();

                        if (res >= 0)
                        {
                            MessageBox.Show("Data dokter berhasil disimpan.", "Informasi", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                            da.displayDataApoteker();
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Data dokter gagal disimpan.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (!string.IsNullOrWhiteSpace(txtidDokter.Text) && !string.IsNullOrWhiteSpace(txtNamaDokter.Text) &&
                !string.IsNullOrWhiteSpace(txtTelpDokter.Text) && !string.IsNullOrWhiteSpace(TextAlamat.Text) &&
                !string.IsNullOrWhiteSpace(txtPassword.Text))
                return true;

            return false;
        }

        #endregion
    }
}