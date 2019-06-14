using System;
using System.Linq;
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
         * SELECT tb_antrian_poli.no_rm, tb_pasien.nama, tb_antrian_poli.no_urut, tb_poliklinik.nama_poli
         * FROM tb_antrian_poli
         * LEFT JOIN tb_pasien ON tb_pasien.no_rekam_medis = tb_antrian_poli.no_rm
         * LEFT JOIN tb_poliklinik ON tb_antrian_poli.poliklinik = tb_poliklinik.kode_poli
         * WHERE tb_antrian_poli.tgl_berobat = '2019-05-06';
         * */

        private string policode = null;
        private string tgl = null;

        SqlConnection conn;

        public antrian()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();

            DBCommand dbcmd = new DBCommand(conn);
            List<ComboboxPairs> cbp = dbcmd.GetPoliklinik();

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
            DBCommand cmd = new DBCommand(conn);
            List<ModelAntrian> dataAntrian = cmd.GetDataAntrian(tgl);

            if (string.IsNullOrEmpty(antrian) || antrian == "Pilih")
            {
                dtgAntrian.ItemsSource = dataAntrian;
            }
            else
            {
                IEnumerable<ModelAntrian> filtered = dataAntrian.Where(x => x.poliklinik == antrian);
                dtgAntrian.ItemsSource = filtered;
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
                    int res = 1;

                    try
                    {
                        if (policode != "Pilih" || policode != "000")
                        {
                            for (int i = 0; i < dtgAntrian.Items.Count; i++)
                            {
                                SqlCommand cmd = new SqlCommand("delete from[tb_antrian_poli] where[tgl_berobat] = CONVERT(date, @date, 111) AND[poliklinik] = @poliklinik", DBConnection.dbConnection());
                                cmd.Parameters.AddWithValue("date", dtTanggalLahir.SelectedDate.Value.ToShortDateString());
                                cmd.Parameters.AddWithValue("poliklinik", policode);
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