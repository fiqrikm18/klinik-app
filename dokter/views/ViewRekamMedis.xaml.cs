using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using dokter.DBAccess;
using dokter.forms;
using dokter.mifare;
using dokter.models;
using dokter.Utils;

namespace dokter.views
{
    /// <summary>
    ///     Interaction logic for ViewRekamMedis.xaml
    /// </summary>
    public partial class ViewRekamMedis : Page
    {
        private readonly byte blockNoRekamMedis = 1;
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private int id;
        private readonly byte Msb = 0x00;
        private string no_rm = "";

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

            this.no_rm = no_rm;
            DisplayDataPasien(no_rm);
        }

        private void BtnBrowsePasien_Click(object sender, RoutedEventArgs e)
        {
            if (cmd.CountDataAntrian() >= 1)
            {
                if (chkScanKartu.IsChecked ?? true)
                {
                    sp = new SmartCardOperation();

                    if (sp.IsReaderAvailable())
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
                            MessageBox.Show(
                                "Pastikan reader sudah terpasang dan kartu sudah berada pada jangkauan reader.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            sp.isoReaderInit();
                        }
                    else
                        MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    no_rm = cmd.GetNoRmByNoUrut();
                    DisplayDataPasien(no_rm);
                }
            }
            else
            {
                MessageBox.Show("Tidak ada data antrian pasien.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        public void DisplayDataPasien(string no_rm = null)
        {
            var pasien = cmd.GetDataPasien();

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
                    MessageBox.Show("Pasien tidak ada di antrian atau belum saatnya dipanggil.", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void DisplayDataRekamMedis(string no_rn = null)
        {
            var rekamMedis = cmd.GetAllDataRekamMedisFrom();

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
                var irm = new InputRekamMedis(txtNoRekamMedis.Text, this);
                irm.Show();
            }
            else
            {
                MessageBox.Show("Pastikan nomor rekam medis sudah terisi.", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void BtnHapusRM_Click(object sender, RoutedEventArgs e)
        {
            if (dtgDataRekamMedis.SelectedItems.Count > 0)
            {
                id = 0;
                foreach (ModelRekamMedis md in dtgDataRekamMedis.SelectedItems) id = md.id;

                if (cmd.DeleteRekamMedis(id))
                {
                    MessageBox.Show("Rekam medis berhasil dihapus.", "Informasi", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    DisplayDataRekamMedis(txtNoRekamMedis.Text);
                }
                else
                {
                    MessageBox.Show("Rekam medis gagal dihapus.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Pilih data yang akan dihapus terlebih dahulu.", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnBuatResep_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNoRekamMedis.Text) || !string.IsNullOrEmpty(txtNoRekamMedis.Text))
            {
                var irs = new InputResep(txtNoRekamMedis.Text, this);
                irs.Show();
            }
            else
            {
                MessageBox.Show("Pastikan nomor rekam medis sudah terisi.", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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