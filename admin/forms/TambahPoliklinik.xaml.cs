using System.Data;
using System.Data.SqlClient;
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
        private readonly DaftarPoliklinik dp;
        private MPoliklinik _mDaftarBaru = new MPoliklinik(" ", " ");
        private int _noOfErrorsOnScreen;


        public TambahPoliklinik(DaftarPoliklinik dp)
        {
            InitializeComponent();
            this.dp = dp;
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
                var nama = txtNamaDokter.Text;
                var id = txtidDokter.Text.ToUpper();

                try
                {
                    if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    var query = "select count(*) from tb_poliklinik where kode_poli='" + id + "'";
                    var cmd = new SqlCommand(query, DBConnection.dbConnection());
                    var idExist = int.Parse(cmd.ExecuteScalar().ToString());

                    if (idExist >= 1)
                    {
                        MessageBox.Show("Kode poliklinik sudah terdaftar.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                    else
                    {
                        query =
                            "insert into tb_poliklinik(kode_poli, nama_poli) values('" + id + "', '" + nama +
                            "')";
                        var command = new SqlCommand(query, DBConnection.dbConnection());
                        var res = command.ExecuteNonQuery();

                        if (res == 1)
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
                catch (SqlException ex)
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

            if (!string.IsNullOrWhiteSpace(txtidDokter.Text) && !string.IsNullOrWhiteSpace(txtNamaDokter.Text))
                return true;

            return false;
        }

        #endregion
    }
}