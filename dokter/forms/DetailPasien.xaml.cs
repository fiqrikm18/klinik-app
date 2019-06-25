using dokter.DBAccess;
using dokter.models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace dokter.forms
{
    /// <summary>
    /// Interaction logic for DetailPasien.xaml
    /// </summary>
    public partial class DetailPasien : Window
    {
        string no_rm;
        private SqlConnection conn;
        private DBCommand cmd;

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

            List<ModelPasien> pasien = cmd.GetDataPasien();

            if (no_rm != null)
            {
                if (no_rm == cmd.GetNoRmByNoUrut())
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
                else
                {
                    MessageBox.Show("Pasien tidak ada di antrian atau belum saatnya dipanggil.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void DisplayDataRekamMedis(string no_rn = null)
        {
            List<ModelRekamMedis> rekamMedis = cmd.GetAllDataRekamMedisFrom();

            if (no_rn != null)
            {
                IEnumerable<ModelRekamMedis> fRekamMedis = rekamMedis.Where(x => x.no_rm.Contains(no_rn));
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
            // TODO: Print data pasien
        }
    }
}
