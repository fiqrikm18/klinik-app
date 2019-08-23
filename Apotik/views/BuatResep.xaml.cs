using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Apotik.DBAccess;
using Apotik.mifare;
using Apotik.models;
using Apotik.Properties;
using Apotik.Utils;
using SimpleTCP;

namespace Apotik.views
{
    /// <summary>
    ///     Interaction logic for BuatResep.xaml
    /// </summary>
    public partial class BuatResep : Page
    {
        private readonly byte blockRekamMedis = 2;
        private readonly SimpleTcpClient clientApotik;
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;

        private readonly SmartCardOperation sp;
        private string kode_resep;

        public BuatResep()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
            sp = new SmartCardOperation();

            var apoteker = cmd.GetDataApoteker().ToList().First();
            lbApoteker.Content += "\t" + apoteker.nama;

            //MessageBox.Show(cmd.GetKodeResepByRm("RM00"));

            try
            {
                clientApotik = new SimpleTcpClient();
                clientApotik.Connect(Settings.Default.SocketAntriApotik,
                    Settings.Default.PortAntriApotik);
                clientApotik.DataReceived += ClientApotik_DataReceived;
            }
            catch (Exception)
            {
            }
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

        private void ClientApotik_DataReceived(object sender, Message e)
        {
            //throw new NotImplementedException();
        }

        public void DisplayData(string kode_resep = null)
        {
            var dataResep = cmd.GetDataResep();

            if (kode_resep != null || string.IsNullOrEmpty(kode_resep) || string.IsNullOrWhiteSpace(kode_resep))
                if (cmd.CountAntrianApotik() > 0)
                {
                    if (kode_resep == cmd.GetKodeResepByNoUrut())
                    {
                        var fResep = dataResep.Where(x => x.kode_resep.Equals(kode_resep)).ToList();

                        //MessageBox.Show(fResep.no_rm);

                        foreach (var mr in fResep)
                        {
                            txtKodeResep.Text = mr.kode_resep;
                            txtNamaDokter.Text = "Dr. " + mr.nama_dokter;
                            txtNamaPasien.Text = mr.nama_pasien;
                            txtNoRm.Text = mr.no_rm;
                        }

                        DisplayDetailResep(kode_resep);

                        var total = 0;

                        foreach (ModelDetailResep dr in dtgDetailResep.ItemsSource) total += dr.sub_total;

                        Debug.WriteLine(total);
                        txtTotal.Text = total.ToString("C", new CultureInfo("id-ID"));
                    }
                    else
                    {
                        MessageBox.Show("Kode resep tidak terdaftar, atau belum saatnya dipanggil.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
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
            var total = 0;

            foreach (ModelDetailResep dr in dtgDetailResep.ItemsSource) total += dr.sub_total;

            //MessageBox.Show(total.ToString());
            if (cmd.CreateTransactionResep(Settings.Default.KodeApoteker,
                txtKodeResep.Text, total))
                if (cmd.UpdateStatusAntrianApotik(kode_resep))
                {
                    try
                    {
                        clientApotik.WriteLine("Update");
                    }
                    catch (Exception)
                    {
                    }

                    ClearData();
                }
        }

        private void ClearData()
        {
            txtKodeResep.Text = string.Empty;
            txtNamaDokter.Text = string.Empty;
            txtNamaPasien.Text = string.Empty;
            txtTotal.Text = string.Empty;
            txtNoRm.Text = string.Empty;
            dtgDetailResep.ItemsSource = null;
        }

        private void btnBrowseResep_Click(object sender, RoutedEventArgs e)
        {
            var cntAntrian = cmd.CountAntrianApotik();

            if (cntAntrian >= 1)
            {
                if (chkScanKartu.IsChecked ?? true)
                {
                    if (sp.IsReaderAvailable())
                        try
                        {
                            sp.isoReaderInit();
                            var readData = sp.ReadBlock(0x00, blockRekamMedis);
                            var asciiData = "";

                            if (readData != null) asciiData = Util.ToASCII(readData, 0, 16, false);

                            Debug.WriteLine(asciiData);

                            kode_resep = cmd.GetKodeResepByRm(asciiData);

                            Debug.WriteLine($"Kode resep: {kode_resep}");
                            DisplayData(kode_resep);
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
                    kode_resep = cmd.GetKodeResepByNoUrut();
                    DisplayData(kode_resep);
                }
            }
            else
            {
                MessageBox.Show("Tidak ada data antrian pasien.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Information);
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