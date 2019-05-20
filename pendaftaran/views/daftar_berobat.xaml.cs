using pendaftaran.models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using pendaftaran.DBAccess;
using MySql.Data.MySqlClient;

namespace pendaftaran.views
{
    /// <summary>
    /// Interaction logic for daftar_berobat.xaml
    /// </summary>
    public partial class daftar_berobat : Page
    {
        MySqlCommand cmd;

        public daftar_berobat()
        {
            InitializeComponent();
            List<ComboboxPairs> cbp = new List<ComboboxPairs>();

            try
            {
                if (DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                {
                    DBConnection.dbConnection().Open();
                    MySqlCommand command = new MySqlCommand("select * from poliklinik", DBConnection.dbConnection());
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(), reader["kode_poliklinik"].ToString()));
                        }
                    }

                    DBConnection.dbConnection().Close();
                }
                else
                {
                    MySqlCommand command = new MySqlCommand("select * from poliklinik", DBConnection.dbConnection());
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(), reader["kode_poliklinik"].ToString()));
                        }
                    }

                    DBConnection.dbConnection().Close();
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message, "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            cbPoliklinik.DisplayMemberPath = "kode_poliklinik";
            cbPoliklinik.SelectedValuePath = "nama_poliklinik";
            cbPoliklinik.ItemsSource = cbp;
            cbPoliklinik.SelectedIndex = 0;
        }

        private void tambah_antrian(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("as");
            ComboboxPairs cbp = (ComboboxPairs)cbPoliklinik.SelectedItem;
            string policode = cbp.nama_poliklinik;
            string poliklinik = policode;
            string norm = txtIdPasien.Text.ToString();
            //string norm = "444";

            try
            {
                if (DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                    DBConnection.dbConnection().Open();

                string query = "select count(*) from pasien where no_rekam_medis = '" + norm + "';";
                cmd = new MySqlCommand(query, DBConnection.dbConnection());
                int rm_exist = int.Parse(cmd.ExecuteScalar().ToString());
                DBConnection.dbConnection().Close();

                if (rm_exist >= 1)
                {
                    if (DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    string last = "";
                    int a = 0;
                    //int.TryParse(last, out a);
                    int no_urut = 0;
                    string query_last = "select nomor_urut from antrian where poliklinik= '" + policode + "' and DATE(tanggal_berobat) = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' ORDER BY nomor_urut desc LIMIT 1;";
                    //string query_last = "select nomor_urut from antrian where poliklinik= '003' and DATE(tanggal_berobat) = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' ORDER BY nomor_urut desc LIMIT 1;";
                    cmd = new MySqlCommand(query_last, DBConnection.dbConnection());
                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) a = reader.GetInt32(0);

                    if (last != null || last != "") no_urut = a + 1;
                    else no_urut = 1;

                    DBConnection.dbConnection().Close();
                    DBConnection.dbConnection().Open();
                    query = "insert into antrian(nomor_rm, nomor_urut, poliklinik, status) values('" + norm + "','" + no_urut + "','" + policode + "','Antri');";
                    cmd = new MySqlCommand(query, DBConnection.dbConnection());

                    int res = cmd.ExecuteNonQuery();

                    if (res == 1)
                        MessageBox.Show("Pasien berhasil ditambahkan. \nNomor antri: " + no_urut, "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                    //MessageBox.Show(last);
                    else
                        MessageBox.Show("Data pasien berhasil ditambahkan. \nGagal menambahakan pasien ke antrian.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Rekam medis pasien belum teraftar, periksa kemabali data pasien.", "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message, "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }

        private void Checkscan_OnUnchecked(object sender, RoutedEventArgs e)
        {
            this.txtIdPasien.IsEnabled = true;
        }

        private void Checkscan_OnChecked(object sender, RoutedEventArgs e)
        {
            this.txtIdPasien.IsEnabled = false;
        }
    }
}
