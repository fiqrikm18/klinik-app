using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.models;
using MySql.Data.MySqlClient;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for UbahDokter.xaml
    /// </summary>
    public partial class UbahDokter : Window
    {
        private views.DaftarDokter dd;
        private int _noOfErrorsOnScreen = 0;
        private MDokter _mDaftarBaru = new MDokter(" ", " ", " ", " ", " ");

        #region constructor
        
        public UbahDokter(string id, string nama, string telp, string alamat, string spesialisasi, string jenisK, views.DaftarDokter dd)
        {
            InitializeComponent();
            DataContext = new MDokter(id, nama, telp, spesialisasi, alamat);
            this.dd = dd;

            if (jenisK == "Pria") cbJenisKelamin.SelectedIndex = 0;
            else if (jenisK == "Wanita") cbJenisKelamin.SelectedIndex = 1;
        }

        #endregion

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
            _mDaftarBaru = new MDokter(" ", " ", " ", " ", " ");

            if (checkTextBoxValue())
            {
                var nama = txtNamaDokter.Text;
                var id = txtidDokter.Text;
                var telp = txtTelpDokter.Text;
                var alamat = TextAlamat.Text;
                var spesialisasi = txtSpesialisai.Text;
                var jenisK = cbJenisKelamin.Text;

                try
                {
                    if(DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    string query = "update dokter set nama='"+nama+"', alamat='"+alamat+"', telp='"+telp+"', spesialisasi='"+spesialisasi+"', jenis_kelamin='"+jenisK+"' where id='"+id+"'";
                    MySqlCommand cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var res = cmd.ExecuteNonQuery();

                    if (res >= 1)
                    {
                        MessageBox.Show("Berhasil memperbarui data dokter", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        dd.displayDataDokter();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Gagal memperbarui data dokter", "Error", MessageBoxButton.OK,
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
                !string.IsNullOrWhiteSpace(txtTelpDokter.Text) && !string.IsNullOrWhiteSpace(txtSpesialisai.Text) &&
                !string.IsNullOrWhiteSpace(TextAlamat.Text))
            {
                return true;
            }

            return false;
        }

        #endregion
        
        private void BtnBatal_OnClick(object sender, RoutedEventArgs e)
        {
            dd.displayDataDokter();
            Close();
        }
    }
}