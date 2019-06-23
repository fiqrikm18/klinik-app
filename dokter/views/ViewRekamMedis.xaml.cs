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
using dokter.forms;

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
            if (cmd.CountDataAntrian() >= 1)
            {
                string no_rm = "";
                if (chkScanKartu.IsChecked ?? true)
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
            else
            {
                MessageBox.Show("Tidak ada data antrian pasien.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
                    DisplaDataRekamMedis(no_rm);
                }
                else
                {
                    MessageBox.Show("Pasien tidak ada di antrian atau belum saatnya dipanggil.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void DisplaDataRekamMedis(string no_rn = null)
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

        private void ChkScanKartu_Checked(object sender, RoutedEventArgs e)
        {
            btnBrowsePasien.Content = "Scan kartu";
        }

        private void ChkScanKartu_Unchecked(object sender, RoutedEventArgs e)
        {
            btnBrowsePasien.Content = "Ambil data";
        }

        private void BtnInputRM_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNoRekamMedis.Text) || !string.IsNullOrEmpty(txtNoRekamMedis.Text))
            {
                InputRekamMedis irm = new InputRekamMedis(txtNoRekamMedis.Text, this);
                irm.Show();
            }
            else
            {
                MessageBox.Show("Pastikan nomor rekam medis sudah terisi.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDetailRM_Click(object sender, RoutedEventArgs e)
        {
            // TODO: buat fungsi untuk melihat detail rekam medis
        }

        private void BtnEditRM_Click(object sender, RoutedEventArgs e)
        {
            // TODO: buat fungsi untuk update rekam medis
        }

        private void BtnHapusRM_Click(object sender, RoutedEventArgs e)
        {
            // TODO: buat fungsi untuk hapus rekam medis
        }

        private void BtnBuatResep_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNoRekamMedis.Text) || !string.IsNullOrEmpty(txtNoRekamMedis.Text))
            {
                InputResep irs = new InputResep(txtNoRekamMedis.Text, this);
                irs.Show();
            }
            else
            {
                MessageBox.Show("Pastikan nomor rekam medis sudah terisi.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnSelesaiPasien_Click(object sender, RoutedEventArgs e)
        {
            if (cmd.UpdateStatusAntrian(txtNoRekamMedis.Text))
            {
                txtNoRekamMedis.Text = string.Empty;
                txtNamaPasien.Text = string.Empty;
                txtGolDarah.Text = string.Empty;
                TextAlamat.Text = string.Empty;
                txtJenisKelamin.Text = string.Empty;
                txtNoTelp.Text = string.Empty;
                txtTglLahir.Text = string.Empty;
                txtNoTelp.Text = string.Empty;

                dtgDataRekamMedis.ItemsSource = null;
            }
        }
    }
}
