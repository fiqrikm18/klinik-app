using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using pendaftaran.DBAccess;
using MySql.Data.MySqlClient;
using pendaftaran.models;
using System.Data;

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

            displayDataAntrian();
        }

        private void TambahPasien(object sender, RoutedEventArgs e)
        {
            daftar_berobat db = new daftar_berobat();
            NavigationService.Navigate(db);
        }

        public void displayDataAntrian(string antrian = null)
        {
            try
            {
                if (DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                {
                    DBConnection.dbConnection().Open();
                }

                string query = null;

                if (antrian != null)
                {
                    query = "SELECT antrian.nomor_rm as no_rm, pasien.nama as nama_pasien, antrian.nomor_urut as no_urut, poliklinik.nama_poliklinik as poliklinik " +
                    "FROM antrian " +
                    "LEFT JOIN pasien ON pasien.no_rekam_medis = antrian.nomor_rm " +
                    "LEFT JOIN poliklinik ON antrian.poliklinik = poliklinik.kode_poliklinik" +
                    " WHERE DATE(antrian.tanggal_berobat) = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND poliklinik = '"+ antrian +"';";
                }
                else
                {
                    query = "SELECT antrian.nomor_rm as no_rm, pasien.nama as nama_pasien, antrian.nomor_urut as no_urut, poliklinik.nama_poliklinik as poliklinik " +
                    "FROM antrian " +
                    "LEFT JOIN pasien ON pasien.no_rekam_medis = antrian.nomor_rm " +
                    "LEFT JOIN poliklinik ON antrian.poliklinik = poliklinik.kode_poliklinik" +
                    " WHERE DATE(antrian.tanggal_berobat) = '" + DateTime.Now.ToString("yyyy-MM-dd") + "';";
                }

                //string query = "SELECT * FROM antrian LEFT JOIN pasien ON pasien.no_rekam_medis = antrian.nomor_rm LEFT JOIN poliklinik ON antrian.poliklinik = poliklinik.kode_poliklinik WHERE DATE(antrian.tanggal_berobat) = '2019-05-06';";
                MySqlCommand cmd = new MySqlCommand(query, DBConnection.dbConnection());
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                adapter.Fill(dt);
                dtgAntrian.ItemsSource = dt.DefaultView;

                DBConnection.dbConnection().Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message, "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DtgAntrian_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboboxPairs cbp = (ComboboxPairs)cbPoliklinik.SelectedItem;
            string policode = cbp.nama_poliklinik;

            displayDataAntrian(policode);
        }

        private void HapusDataPasien(object sender, RoutedEventArgs e)
        {
            if (dtgAntrian.SelectedCells.Count > 0)
            {
                //object item = dtgDataPasien.SelectedItem;
                //string id = (dtgDataPasien.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text.ToString();
                //MessageBox.Show(id);

                MessageBoxResult a = MessageBox.Show("Anda yakin ingin menghapus data pasien?", "Konfirmasi", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (a == MessageBoxResult.Yes)
                {
                    string query;
                    DBConnection.dbConnection().Open();
                    int res = 1;

                    try
                    {
                        for (int i = 0; i < dtgAntrian.SelectedItems.Count; i++)
                        {
                            //MessageBox.Show((this.dtgDataPasien.SelectedCells[0].Column.GetCellContent(this.dtgDataPasien.SelectedItems[i]) as TextBlock).Text);
                            query = "delete from pasien where nomor_rm = '" + (dtgAntrian.SelectedCells[0].Column.GetCellContent(this.dtgAntrian.SelectedItems[i]) as TextBlock).Text + "';";
                            MySqlCommand command = new MySqlCommand(query, DBConnection.dbConnection());
                            res = command.ExecuteNonQuery();
                        }

                        if (res == 1)
                        {
                            MessageBox.Show("Data pasien berhasil dihapus.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                            MessageBox.Show("Data pasien gagal dihapus.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Data pasien gagal dihapus.\n" + ex.Message, "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                }

                displayDataAntrian();
                DBConnection.dbConnection().Close();
            }
            else
            {
                MessageBox.Show("Pilih data pasien yang akan dihapus.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
