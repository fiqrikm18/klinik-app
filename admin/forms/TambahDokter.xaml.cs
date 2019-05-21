using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.models;
using MySql.Data.MySqlClient;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for TambahDokter.xaml
    /// </summary>
    public partial class TambahDokter : Window
    {
        private MDokter _mDaftarBaru = new MDokter(" ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;
        private views.DaftarDokter dd;

        #region constructor

        public TambahDokter(views.DaftarDokter du)
        {
            InitializeComponent();

            this.dd = du;

            var cbp = new List<ComboboxPairs>();
            DataContext = new MDokter(" ", " ", " ", " ", " ");
            //DataContext = new MDaftarBaru("123", "123", "ad", "123", " 123123");

            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Open))
                {
                    var command = new MySqlCommand("select * from poliklinik", DBConnection.dbConnection());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(),
                                reader["kode_poliklinik"].ToString()));
                    }
                }
                else
                {
                    DBConnection.dbConnection().Open();
                    var command = new MySqlCommand("select * from poliklinik", DBConnection.dbConnection());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(),
                                reader["kode_poliklinik"].ToString()));
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message,
                    "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            cbPoliklinik.DisplayMemberPath = "kode_poliklinik";
            cbPoliklinik.SelectedValuePath = "nama_poliklinik";
            cbPoliklinik.ItemsSource = cbp;
            cbPoliklinik.SelectedIndex = 0;
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
                var cbp = (ComboboxPairs) cbPoliklinik.SelectedItem;
                var policode = cbp.nama_poliklinik;

                var nama = txtNamaDokter.Text;
                var id = txtidDokter.Text;
                var telp = txtTelpDokter.Text;
                var alamat = TextAlamat.Text;
                var spesialisasi = txtSpesialisai.Text;
                var jenisK = cbJenisKelamin.Text;

                try
                {
                    var query = "select count(*) from dokter where id='+ id +'";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var idExist = int.Parse(cmd.ExecuteScalar().ToString());

                    if (idExist >= 1)
                        MessageBox.Show("No id sudah terdaftar.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    else
                        query =
                            "insert into dokter(id, nama, telp, alamat, spesialisasi, tugas, jenis_kelamin) values('" +
                            id + "', '" + nama + "', '" + telp + "', '" + alamat + "', '" + spesialisasi + "', '" +
                            policode + "', '" + jenisK + "')";
                    var command = new MySqlCommand(query, DBConnection.dbConnection());
                    var res = command.ExecuteNonQuery();

                    if (res == 1)
                    {
                        MessageBox.Show("Data dokter berhasil disimpan.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        dd.displayDataDokter();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Data dokter gagal disimpan.", "Error", MessageBoxButton.OK,
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
            cbJenisKelamin.SelectedIndex = 0;
            cbPoliklinik.SelectedIndex = 0;
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
            Close();
        }
    }
}