using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using dokter.DBAccess;

namespace dokter.forms
{
    /// <summary>
    ///     Interaction logic for DetailPasien.xaml
    /// </summary>
    public partial class DetailPasien : Window
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private string no_rm;

        public DetailPasien()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
        }

        public DetailPasien(string no_rm)
        {
            InitializeComponent();
            this.no_rm = no_rm;

            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            DisplayDataPasien(no_rm);
        }

        public void DisplayDataPasien(string no_rm = null)
        {
            var pasien = cmd.GetDataPasien();

            if (no_rm != null)
            {
                var fPasien = pasien.Where(x => x.no_rm.Contains(no_rm)).ToList().First();
                txtNoRekamMedis.Text = fPasien.no_rm;
                txtNamaPasien.Text = fPasien.nama;
                txtGolDarah.Text = fPasien.golongan_darah;
                TextAlamat.Text = fPasien.alamat;
                txtJenisKelamin.Text = fPasien.jenis_kelamin;
                txtTglLahir.Text = DateTime.Parse(fPasien.tgl_lahir).ToString("dd MMM yyyy");
                txtNoTelp.Text = fPasien.no_telp;
                DisplayDataRekamMedis(no_rm);
            }
        }

        private void DisplayDataRekamMedis(string no_rn = null)
        {
            var rekamMedis = cmd.GetAllDataRekamMedisFrom(no_rn);

            if (no_rn != null)
            {
                var fRekamMedis = rekamMedis.Where(x => x.no_rm.Contains(no_rn));
                dtgDataRekamMedis.ItemsSource = fRekamMedis;
            }
            else
            {
                rekamMedis.Clear();
                dtgDataRekamMedis.ItemsSource = rekamMedis;
            }
        }

        private void btnPrintData_Click(object sender, RoutedEventArgs e)
        {
            var no_rm = txtNoRekamMedis.Text;
            var rekamMedis = cmd.GetAllDataRekamMedisFrom();
            if (rekamMedis.Count < 1)
            {
                MessageBox.Show("Rekam medis pasien tidak tersedia", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            {
                var rv = new ReportView(no_rm);
                rv.Show();
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized) MainGrid.Margin = new Thickness(35);

            if (WindowState == WindowState.Normal) MainGrid.Margin = new Thickness(20);
        }
    }
}