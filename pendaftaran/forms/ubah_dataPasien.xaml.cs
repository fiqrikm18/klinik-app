using MySql.Data.MySqlClient;
using pendaftaran.models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using pendaftaran.DBAccess;

namespace pendaftaran.forms
{
    /// <summary>
    /// Interaction logic for ubah_dataPasien.xaml
    /// </summary>
    public partial class ubah_dataPasien : Window
    {
        private int _noOfErrorsOnScreen = 0;
        private MDaftarBaru _mDaftarBaru = new MDaftarBaru(" ", " ", " ", " ", " ");

        string norm;
        string idP;
        string nama;
        string jk;
        string telp;
        string alamat;
        views.daftar_ulang du;

        public ubah_dataPasien(string norm, string idp, string nama, string jk, string notlp, string alamat, views.daftar_ulang du)
        {
            InitializeComponent();
            List<ComboboxPairs> cbp = new List<ComboboxPairs>();
            DataContext = new MDaftarBaru(norm, idp, nama, notlp, alamat);

            this.norm = norm;
            this.idP = idp;
            this.nama = nama;
            this.jk = jk;
            this.telp = notlp;
            this.alamat = alamat;
            this.du = du;

            if (jk == "Pria") cbJenisKelamin.SelectedIndex = 0;
            else if (jk == "Wanita") cbJenisKelamin.SelectedIndex = 1;
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;
        }

        private void AddPasien_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddPasien_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mDaftarBaru = new MDaftarBaru(norm, idP, nama, telp, alamat);

            if (checkTextBoxValue())
            {

                CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
                ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                Thread.CurrentThread.CurrentCulture = ci;

                //DateTime dt = DateTime.ParseExact(, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                string norm = TxtNoRm.Text.ToString();
                string identitas = TxtNoIdentitas.Text.ToString();
                string namaPasien = TxtNamaPasien.Text.ToString();
                string noTelp = TxtNoTelp.Text.ToString();
                string alamat = TextAlamat.Text.ToString();
                string jenisKelamin = cbJenisKelamin.Text.ToString();

                try
                {
                    if (DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                    {
                        DBConnection.dbConnection().Open();
                    }

                    string query = "update pasien set nama='" + namaPasien + "', jenis_kelamin='" + jenisKelamin + "', no_telp='" + noTelp + "', alamat='" + alamat + "' where no_identitas='" + identitas + "';";
                    MySqlCommand cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    int res = cmd.ExecuteNonQuery();

                    if (res == 1)
                    {
                        MessageBox.Show("Berhasil memperbarui data pasien.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                        du.displayDataPasien();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Gagal memperbarui data pasein.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        DataContext = _mDaftarBaru;
                        if (jk == "Pria") cbJenisKelamin.SelectedIndex = 0;
                        else if (jk == "Wanita") cbJenisKelamin.SelectedIndex = 1;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan.\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Tidak terdapat pembaruan data pasien.", "Infomasi");
            }

            DataContext = _mDaftarBaru;
            if (jk == "Pria") cbJenisKelamin.SelectedIndex = 0;
            else if (jk == "Wanita") cbJenisKelamin.SelectedIndex = 1;
            e.Handled = true;
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox source = e.Source as TextBox;
            source.Clear();
        }

        private bool checkTextBoxValue()
        {
            if (TxtNoRm.Text == norm && TxtNoIdentitas.Text == idP && TxtNamaPasien.Text == nama && TxtNoTelp.Text == telp && TextAlamat.Text == alamat)
            {
                return false;
            }

            return true;
        }

        private void Batal(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
