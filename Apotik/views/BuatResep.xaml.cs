using Apotik.DBAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
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
using Apotik.mifare;

namespace Apotik.views
{
    /// <summary>
    /// Interaction logic for BuatResep.xaml
    /// </summary>
    public partial class BuatResep : Page
    {
        SqlConnection conn;
        DBCommand cmd;
        string kode_resep;
        private byte blockRekamMedis = 1;

        SmartCardOperation sp;

        public BuatResep()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
            sp = new SmartCardOperation();

            var apoteker = cmd.GetDataApoteker().ToList().First();
            lbApoteker.Content += "\t" + apoteker.nama;

            //MessageBox.Show(cmd.GetKodeResepByRm("RM00"));
        }

        public BuatResep(string kode_resep)
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
            sp = new SmartCardOperation();

            var apoteker = cmd.GetDataApoteker().ToList().First();
            lbApoteker.Content += "\t" + apoteker.nama;

            this.kode_resep = kode_resep;
            DisplayData(kode_resep);
        }

        public void DisplayData(string kode_resep = null)
        {
            var dataResep = cmd.GetDataResep();

            if (kode_resep != null || string.IsNullOrEmpty(kode_resep) || string.IsNullOrWhiteSpace(kode_resep))
            {
                if (kode_resep == cmd.GetKodeResepByNoUrut())
                {
                    var fResep = dataResep.Where(x => x.kode_resep.Contains(kode_resep)).ToList().First();
                    txtKodeResep.Text = fResep.kode_resep;
                    txtNamaDokter.Text = fResep.nama_dokter;
                    txtNomorResep.Text = fResep.no_resep;
                    txtNoRm.Text = fResep.no_rm;
                    DisplayDetailResep(fResep.kode_resep);

                    var total = 0;

                    foreach (models.ModelDetailResep dr in dtgDetailResep.ItemsSource)
                    {
                        total += dr.sub_total;
                    }

                    Debug.WriteLine(total);
                    txtTotal.Text = total.ToString("C", new CultureInfo("id-ID"));
                }
                else
                {
                    MessageBox.Show("Kode resep tidak terdaftar, atau belum saatnya dipanggil.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void DisplayDetailResep(string kode_resep = null)
        {
            if (kode_resep != null)
            {
                var dataResep = cmd.GetDataDetailResep(kode_resep);
                dtgDetailResep.ItemsSource = dataResep;
            }
        }

        private void btnSelesai_Click(object sender, RoutedEventArgs e)
        {
            if (cmd.UpdateStatusAntrianApotik(kode_resep))
            {
                ClearData();
            }
        }

        private void ClearData()
        {
            txtKodeResep.Text = string.Empty;
            txtNamaDokter.Text = string.Empty;
            txtNomorResep.Text = string.Empty;
            txtNoRm.Text = string.Empty;
            dtgDetailResep.ItemsSource = null;
        }

        private void btnBrowseResep_Click(object sender, RoutedEventArgs e)
        {
            var cntAntrian = cmd.CountAntrianApotik();

            if (chkScanKartu.IsChecked ?? true)
            {
                if (sp.IsReaderAvailable())
                {
                    try
                    {
                        sp.isoReaderInit();
                        var readData = sp.ReadBlock(0x00, blockRekamMedis);
                        var asciiData = "";

                        if (readData != null)
                        {
                            asciiData = Utils.Util.ToASCII(readData, 0, 16, false);
                        }

                        kode_resep = cmd.GetKodeResepByRm(asciiData);

                        Debug.WriteLine($"Kode resep: {kode_resep}");
                        DisplayData(kode_resep);
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
                if (cntAntrian >= 1)
                {
                    kode_resep = cmd.GetKodeResepByNoUrut();
                    DisplayData(kode_resep);
                }
            }
        }

        private void ChkScanKartu_Checked(object sender, RoutedEventArgs e)
        {
            btnBrowseResep.Content = "Scan kartu";
        }

        private void ChkScanKartu_Unchecked(object sender, RoutedEventArgs e)
        {
            btnBrowseResep.Content = "Ambil data";
        }
    }
}
