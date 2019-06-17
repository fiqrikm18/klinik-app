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
using System.Data.SqlClient;
using dokter.DBAccess;
using dokter.models;
using dokter.mifare;
using dokter.Utils;

namespace dokter.views
{
    /// <summary>
    /// Interaction logic for ViewRekamMedis.xaml
    /// </summary>
    public partial class ViewRekamMedis : Page
    {
        private SqlConnection conn;
        private DBCommand cmd;
        private byte Msb = 0x00;

        private byte blockNoRekamMedis = 1;

        private SmartCardOperation sp = new SmartCardOperation();

        public ViewRekamMedis()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
        }

        public ViewRekamMedis(string no_rm)
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
        }

        private void BtnBrowsePasien_Click(object sender, RoutedEventArgs e)
        {
            string no_rm = "";
            if(chkScanKartu.IsChecked ?? true)
            {
                sp = new SmartCardOperation();

                if (sp.IsReaderAvailable())
                {
                    try
                    {

                        sp.isoReaderInit();
                        //card = new MifareCard(isoReader);

                        var readData = sp.ReadBlock(Msb, blockNoRekamMedis);
                        if (readData != null)
                            no_rm = Util.ToASCII(readData, 0, 16, false);

                        DisplayDataPasien(no_rm);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Pastikan reader sudah terpasang dan kartu sudah berada pada jangkauan reader.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        sp.isoReaderInit();
                    }
                }
                else
                {
                    MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                no_rm = cmd.GetNoRmByNoUrut();
                DisplayDataPasien(no_rm);
            }
        }

        public void DisplayDataPasien(string no_rm = null)
        {
            List<ModelPasien> pasien = cmd.GetDataPasien();

            if(no_rm != null)
            {
                var fPasien = pasien.Where(x => x.no_rm.Contains(no_rm)).ToList().First();
                //MessageBox.Show(fPasien.jenis_kelamin);
                txtNoRekamMedis.Text = fPasien.no_rm;
                txtNamaPasien.Text = fPasien.nama;
                txtGolDarah.Text = fPasien.golongan_darah;
                TextAlamat.Text = fPasien.alamat;
                txtJenisKelamin.Text = fPasien.jenis_kelamin;
                txtTglLahir.Text = DateTime.Parse(fPasien.tgl_lahir).ToString("dd MMM yyyy");
                txtNoTelp.Text = fPasien.no_telp;
                DisplaDataRekamMedis(no_rm);
            }
        }

        public void DisplaDataRekamMedis(string no_rn = null)
        {
            List<ModelRekamMedis> rekamMedis = cmd.GetAllDataRekamMedisFrom();

            if (no_rn != null)
            {
                IEnumerable<ModelRekamMedis> fRekamMedis = rekamMedis.Where(x => x.no_rm.Contains(no_rn));
                dtgDataRekamMedis.ItemsSource = fRekamMedis;
            }
            else
            {
                dtgDataRekamMedis.ItemsSource = rekamMedis;
            }
        }

        private void ChkScanKartu_Checked(object sender, RoutedEventArgs e)
        {
            btnBrowsePasien.Content = "Scan kartu";
        }

        private void ChkScanKartu_Unchecked(object sender, RoutedEventArgs e)
        {
            btnBrowsePasien.Content = "Ambil data";
        }
    }
}
