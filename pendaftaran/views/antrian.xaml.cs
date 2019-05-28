using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using pendaftaran.DBAccess;
using pendaftaran.models;

namespace pendaftaran.views
{
    /// <summary>
    ///     Interaction logic for antrian.xaml
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

        private string policode = null;
        private string tgl = null;

        public antrian()
        {
            InitializeComponent();

            var cbp = new List<ComboboxPairs>();
            cbp.Add(new ComboboxPairs("Pilih", "Pilih"));

            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                {
                    DBConnection.dbConnection().Open();
                    var command = new SqlCommand("select * from tb_poliklinik", DBConnection.dbConnection());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            cbp.Add(new ComboboxPairs(reader["nama_poli"].ToString(),
                                reader["kode_poli"].ToString()));
                    }

                    DBConnection.dbConnection().Close();
                }
                else
                {
                    var command = new SqlCommand("select * from tb_poliklinik", DBConnection.dbConnection());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            cbp.Add(new ComboboxPairs(reader["nama_poli"].ToString(),
                                reader["kode_poli"].ToString()));
                    }

                    DBConnection.dbConnection().Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message,
                    "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            cbPoliklinik.DisplayMemberPath = "kode_poliklinik";
            cbPoliklinik.SelectedValuePath = "nama_poliklinik";
            cbPoliklinik.ItemsSource = cbp;
            cbPoliklinik.SelectedIndex = 0;

            displayDataAntrian();
            dtTanggalLahir.Text = DateTime.Now.ToString();
        }

        private void TambahPasien(object sender, RoutedEventArgs e)
        {
            var db = new daftar_berobat();
            NavigationService.Navigate(db);
        }

        public void displayDataAntrian(string antrian = null, string tgl = null)
        {
            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();

                string query = null;

                if (antrian != null && antrian != "Pilih")
                {
                    if (tgl == DateTime.Now.ToShortDateString() || tgl == null)
                        query =
                            "select tb_antrian_poli.no_rm as no_rm, tb_pasien.nama as nama_pasien,  tb_antrian_poli.no_urut as no_urut, tb_poliklinik.nama_poli as poliklinik FROM tb_antrian_poli left join tb_pasien on tb_pasien.no_rekam_medis = tb_antrian_poli.no_rm left join tb_poliklinik on tb_antrian_poli.poliklinik = tb_poliklinik.kode_poli where tb_antrian_poli.tgl_berobat = CONVERT(date, getdate(), 111) and poliklinik = '" +
                            antrian + "' and status = 'Antri';";
                    else
                        query =
                            "select tb_antrian_poli.no_rm as no_rm, tb_pasien.nama as nama_pasien,  tb_antrian_poli.no_urut as no_urut, tb_poliklinik.nama_poli as poliklinik FROM tb_antrian_poli left join tb_pasien on tb_pasien.no_rekam_medis = tb_antrian_poli.no_rm left join tb_poliklinik on tb_antrian_poli.poliklinik = tb_poliklinik.kode_poli where tb_antrian_poli.tgl_berobat = CONVERT(date, '" +
                            tgl + "', 111) and poliklinik = '" + antrian + "' and status = 'Antri';";
                }
                else
                {
                    if (tgl == DateTime.Now.ToShortDateString() || tgl == null)
                        query =
                            "select tb_antrian_poli.no_rm as no_rm, tb_pasien.nama as nama_pasien,  tb_antrian_poli.no_urut as no_urut, tb_poliklinik.nama_poli as poliklinik FROM tb_antrian_poli left join tb_pasien on tb_pasien.no_rekam_medis = tb_antrian_poli.no_rm left join tb_poliklinik on tb_antrian_poli.poliklinik = tb_poliklinik.kode_poli where tb_antrian_poli.tgl_berobat = CONVERT(varchar(10), getdate(), 111) and status = 'Antri';";
                    else
                        query =
                            "select tb_antrian_poli.no_rm as no_rm, tb_pasien.nama as nama_pasien,  tb_antrian_poli.no_urut as no_urut, tb_poliklinik.nama_poli as poliklinik FROM tb_antrian_poli left join tb_pasien on tb_pasien.no_rekam_medis = tb_antrian_poli.no_rm left join tb_poliklinik on tb_antrian_poli.poliklinik = tb_poliklinik.kode_poli where tb_antrian_poli.tgl_berobat = '" +
                            tgl + "' and status = 'Antri';";
                }

                //string query = "SELECT * FROM antrian LEFT JOIN pasien ON pasien.no_rekam_medis = antrian.nomor_rm LEFT JOIN poliklinik ON antrian.poliklinik = poliklinik.kode_poliklinik WHERE DATE(antrian.tanggal_berobat) = '2019-05-06';";
                var cmd = new SqlCommand(query, DBConnection.dbConnection());
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();

                adapter.Fill(dt);
                dtgAntrian.ItemsSource = dt.DefaultView;

                DBConnection.dbConnection().Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message,
                    "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DtgAntrian_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbp = (ComboboxPairs)cbPoliklinik.SelectedItem;
            policode = cbp.nama_poliklinik;

            displayDataAntrian(policode, tgl);
        }

        private void dtTanggalLahir_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = ci;

            //tgl = dtTanggalLahir.SelectedDate.Value.Year.ToString() + "-" + dtTanggalLahir.SelectedDate.Value.Month.ToString() + "-" + dtTanggalLahir.SelectedDate.Value.Day.ToString();
            tgl = dtTanggalLahir.SelectedDate.Value.Date.ToShortDateString();
            displayDataAntrian(policode, tgl);
        }

        private void HapusDataPasien(object sender, RoutedEventArgs e)
        {
            var cbp = (ComboboxPairs)cbPoliklinik.SelectedItem;
            policode = cbp.nama_poliklinik;
            int res = 1;

            if (DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                DBConnection.dbConnection().Open();

            if (dtTanggalLahir.SelectedDate.Value.ToShortDateString() != DateTime.Now.ToShortDateString())
            {
                if (dtgAntrian.SelectedItems.Count != 0)
                {
                    if (policode != "Pilih" || policode != "000")
                    {

                    }
                    else
                    {

                    }
                }
                else
                {
                    try
                    {
                        if (policode != "Pilih" || policode != "000")
                        {
                            for (int i = 0; i < dtgAntrian.Items.Count; i++)
                            {
                                var query = "delete from [tb_antrian_poli] where [tgl_berobat] = CONVERT(date, '"+ dtTanggalLahir.SelectedDate.Value.ToShortDateString() + "', 111) AND [poliklinik] = '"+policode+"';";
                                SqlCommand cmd = new SqlCommand(query, DBConnection.dbConnection());
                                //cmd.Parameters.AddWithValue("date", dtTanggalLahir.SelectedDate.Value.ToShortDateString());
                                //cmd.Parameters.AddWithValue("poliklinik", "006");
                                res = cmd.ExecuteNonQuery();
                            }

                            MessageBox.Show(res.ToString());

                            if (res == 1)
                            {
                                MessageBox.Show($"Data antrian pada tanggal {dtTanggalLahir.SelectedDate.Value.ToShortDateString()} berhasil dihapus.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                            }

                            displayDataAntrian();
                        }
                        else
                        {
                            for (int i = 0; i < dtgAntrian.Items.Count; i++)
                            {
                                SqlCommand cmd = new SqlCommand("delete from [tb_antrian_poli] where [tgl_berobat] = CONVERT(date, @date, 111);");
                                cmd.Parameters.AddWithValue("date", dtTanggalLahir.SelectedDate.Value.ToShortDateString());
                                res = cmd.ExecuteNonQuery();
                            }

                            if (res == 0)
                            {
                                MessageBox.Show($"Data antrian pada tanggal {dtTanggalLahir.SelectedDate.Value.ToShortDateString()} berhasil dihapus.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                                displayDataAntrian();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Data antrian pada tanggal {dtTanggalLahir.SelectedDate.Value.ToShortDateString()} gagal dihapus.\n"+ex.Message, "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("Data antrian pada hari ini tidak dapat dihapus.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}