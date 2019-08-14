using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
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
        private readonly SqlConnection conn;
        /* query getting data for datagrid
         * --------------------------------------------------------------------------------------
         * SELECT tb_antrian_poli.no_rm, tb_pasien.nama, tb_antrian_poli.no_urut, tb_poliklinik.nama_poli
         * FROM tb_antrian_poli
         * LEFT JOIN tb_pasien ON tb_pasien.no_rekam_medis = tb_antrian_poli.no_rm
         * LEFT JOIN tb_poliklinik ON tb_antrian_poli.poliklinik = tb_poliklinik.kode_poli
         * WHERE tb_antrian_poli.tgl_berobat = '2019-05-06';
         * */

        private string policode;
        private string tgl;

        public antrian()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();

            var dbcmd = new DBCommand(conn);
            var cbp = dbcmd.GetPoliklinik();

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
            var cmd = new DBCommand(conn);
            var dataAntrian = cmd.GetDataAntrian(tgl);

            if (string.IsNullOrEmpty(antrian) || antrian == "Pilih")
            {
                dtgAntrian.ItemsSource = dataAntrian;
            }
            else
            {
                var filtered = dataAntrian.Where(x => x.poliklinik == antrian);
                dtgAntrian.ItemsSource = filtered;
            }
        }

        private void DtgAntrian_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbp = (ComboboxPairs) cbPoliklinik.SelectedItem;
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
            var cbp = (ComboboxPairs) cbPoliklinik.SelectedItem;
            policode = cbp.nama_poliklinik;
            var cmd = new DBCommand(conn);

            var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            if (dtTanggalLahir.Text != DateTime.Now.ToString("yyyy-MM-dd"))
            {
                var res = false;
                //MessageBox.Show("asdasd");
                foreach (ModelAntrian an in dtgAntrian.ItemsSource)
                    if (cmd.DetleDataAntrian(int.Parse(an.id)))
                        res = true;
                    else
                        break;

                if (res)
                    MessageBox.Show(
                        $"Daftar antrian pada tanggal {dtTanggalLahir.Text} berhasil dihapus.",
                        "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show(
                        $"Sebagian antriaan pada tanggal {dtTanggalLahir.Text} gagal dihapus.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                displayDataAntrian(null, dtTanggalLahir.Text);
            }
            else
            {
                MessageBox.Show("Data antrian pada hari ini tidak dapat dihapus", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}