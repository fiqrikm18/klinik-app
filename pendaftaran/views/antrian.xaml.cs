using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using pendaftaran.DBAccess;
using MySql.Data.MySqlClient;
using pendaftaran.models;

namespace pendaftaran.views
{
    /// <summary>
    /// Interaction logic for antrian.xaml
    /// </summary>
    public partial class antrian : Page
    {
        /* query getting data for datagrid
         * --------------------------------------------------------------------------------------
         * SELECT antrian.nomor_rm, pasien.nama, antrian.nomor_urut, poliklinik.nama_poliklinik
         * FROM antrian
         * LEFT JOIN pasien ON pasien.no_rekam_medis = antrian.nomor_rm
         * LEFT JOIN poliklinik ON antrian.poliklinik = poliklinik.kode_poliklinik
         * WHERE DATE(antrian.tanggal_berobat) = '2019-05-06';
         * */

        public antrian()
        {
            InitializeComponent();

            List<ComboboxPairs> cbp = new List<ComboboxPairs>();

            try
            {
                if (DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                {
                    DBConnection.dbConnection().Open();
                    MySqlCommand command = new MySqlCommand("select * from poliklinik", DBAccess.DBConnection.dbConnection());
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
                    MySqlCommand command = new MySqlCommand("select * from poliklinik", DBAccess.DBConnection.dbConnection());
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(), reader["kode_poliklinik"].ToString()));
                        }
                    }

                    DBConnection.dbConnection().Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message, "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            cbPoliklinik.DisplayMemberPath = "kode_poliklinik";
            cbPoliklinik.SelectedValuePath = "nama_poliklinik";
            cbPoliklinik.ItemsSource = cbp;
            cbPoliklinik.SelectedIndex = 0;
        }

        private void TambahPasien(object sender, RoutedEventArgs e)
        {
            daftar_baru db = new daftar_baru();
            NavigationService.Navigate(db);
        }
    }
}
