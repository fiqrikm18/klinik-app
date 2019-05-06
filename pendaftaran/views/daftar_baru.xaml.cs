using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using pendaftaran.models;
using System.Windows.Controls.Primitives;
using System.Globalization;
using System.Threading;

namespace pendaftaran.views
{
    /// <summary>
    /// Interaction logic for daftar_baru.xaml
    /// </summary>
    public partial class daftar_baru : Page
    {
        //private static MySqlConnection MsqlConn = null;
        private int _noOfErrorsOnScreen = 0;
        private MDaftarBaru _mDaftarBaru = new MDaftarBaru(" ", " ", " ", " ", " ");

        public daftar_baru()
        {
            InitializeComponent();

            List<ComboboxPairs> cbp = new List<ComboboxPairs>();
            DataContext = new MDaftarBaru(" ", " ", " ", " ", " ");
            //DataContext = new MDaftarBaru("123", "123", "ad", "123", " 123123");
            
            try
            {
                if (DBAccess.DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Open))
                {
                    MySqlCommand command = new MySqlCommand("select * from poliklinik", DBAccess.DBConnection.dbConnection());
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(), reader["kode_poliklinik"].ToString()));
                        }
                    }
                }
                else
                {
                    DBAccess.DBConnection.dbConnection().Open();
                    MySqlCommand command = new MySqlCommand("select * from poliklinik", DBAccess.DBConnection.dbConnection());
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(), reader["kode_poliklinik"].ToString()));
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n"+ex.Message, "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            cbPoliklinik.DisplayMemberPath = "kode_poliklinik";
            cbPoliklinik.SelectedValuePath = "nama_poliklinik";
            cbPoliklinik.ItemsSource = cbp;
            cbPoliklinik.SelectedIndex = 0;
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
            _mDaftarBaru = new MDaftarBaru(" ", " ", " ", " ", " ");
            
            if (checkTextBoxValue() && dtTanggalLahir.SelectedDate != null)
            {

                CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
                ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                Thread.CurrentThread.CurrentCulture = ci;

                ComboboxPairs cbp = (ComboboxPairs)cbPoliklinik.SelectedItem;
                string policode = cbp.nama_poliklinik;
                //DateTime dt = DateTime.ParseExact(, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                string norm = TxtNoRm.Text.ToString();
                string identitas = TxtNoIdentitas.Text.ToString();
                string namaPasien = TxtNamaPasien.Text.ToString();
                string noTelp = TxtNoTelp.Text.ToString();
                string alamat = TextAlamat.Text.ToString();
                string tglLahir = dtTanggalLahir.SelectedDate.Value.Date.ToShortDateString();
                string jenisKelamin = cbJenisKelamin.Text.ToString();
                string poliklinik = policode;

                try
                {
                    string query = "select count(*) from pasien where no_identitas = '" + identitas + "';";
                    MySqlCommand cmd = new MySqlCommand(query, DBAccess.DBConnection.dbConnection());
                    int idExist = int.Parse(cmd.ExecuteScalar().ToString());

                    if (idExist >= 1)
                    {
                        MessageBox.Show("No indentitas sudah terdaftar.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        query = "select count(*) from pasien where no_rekam_medis = '" + norm + "';";
                        cmd = new MySqlCommand(query, DBAccess.DBConnection.dbConnection());
                        int rm_exist = int.Parse(cmd.ExecuteScalar().ToString());

                        if(rm_exist >= 1)
                        {
                            MessageBox.Show("No rekam medis sudah terdaftar.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            try
                            {
                                query = "insert into pasien (no_identitas, no_rekam_medis, nama, tanggal_lahir, jenis_kelamin, no_telp, alamat) values('" + identitas + "', '" + norm + "', '" + namaPasien + "', '" + tglLahir + "', '" + jenisKelamin + "', '" + noTelp + "', '" + alamat + "');";

                                MySqlCommand command = new MySqlCommand(query, DBAccess.DBConnection.dbConnection());
                                var res = command.ExecuteNonQuery();

                                if (res == 1)
                                {
                                    string last = "";
                                    int a = 0;
                                    //int.TryParse(last, out a);
                                    int no_urut = 0;
                                    string query_last = "select nomor_urut from antrian where poliklinik= '" + policode + "' and DATE(tanggal_berobat) = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' ORDER BY nomor_urut desc LIMIT 1;";
                                    //string query_last = "select nomor_urut from antrian where poliklinik= '003' and DATE(tanggal_berobat) = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' ORDER BY nomor_urut desc LIMIT 1;";
                                    command = new MySqlCommand(query_last, DBAccess.DBConnection.dbConnection());
                                    MySqlDataReader reader = command.ExecuteReader();

                                    if (reader.Read()) a = reader.GetInt32(0);

                                    if (last != null || last != "") no_urut = a + 1;
                                    else no_urut = 1;

                                    DBAccess.DBConnection.dbConnection().Close();
                                    DBAccess.DBConnection.dbConnection().Open();
                                    query = "insert into antrian(nomor_rm, nomor_urut, poliklinik) values('" + norm + "','" + no_urut + "','" + policode + "');";
                                    command = new MySqlCommand(query, DBAccess.DBConnection.dbConnection());

                                    res = command.ExecuteNonQuery();

                                    if (res == 1)
                                        MessageBox.Show("Data pasien berhasil ditambahkan. \nNomor antri: " + no_urut, "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                                    //MessageBox.Show(last);
                                    else
                                        MessageBox.Show("Data pasien berhasil ditambahkan. \nGagal menambahakan pasien ke antrian.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Data pasien gagal ditambahkan.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show("Terjadi kesalahan pada database: " + ex.Message, "Informasi", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }catch(MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                //MessageBox.Show(tglLahir);
            }
            else
            {
                MessageBox.Show("Periksa kembali data yang akan di inputkan.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            DataContext = _mDaftarBaru;
            dtTanggalLahir.SelectedDate = null;
            cbJenisKelamin.SelectedIndex = 0;
            cbPoliklinik.SelectedIndex = 0;
            e.Handled = true;
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox source = e.Source as TextBox;
            source.Clear();
        }

        private bool checkTextBoxValue()
        {
            if(TxtNoRm.Text == " " && TxtNoIdentitas.Text == " " && TxtNamaPasien.Text == " " && TxtNoTelp.Text == " " && TextAlamat.Text == " " && dtTanggalLahir.SelectedDate.ToString() == null)
            {
                return false;
            }

            return true;
        }
    }
}
