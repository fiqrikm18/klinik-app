using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using pendaftaran.DBAccess;
using pendaftaran.Mifare;
using pendaftaran.models;
using pendaftaran.Properties;
using pendaftaran.Utils;
using FontStyle = System.Drawing.FontStyle;

namespace pendaftaran.views
{
    /// <summary>
    ///     Interaction logic for daftar_berobat.xaml
    /// </summary>
    public partial class daftar_berobat : Page
    {
        private const byte Msb = 0x00;
        private readonly byte blockNoRekamMedis = 2;

        private readonly SqlConnection conn;
        private readonly Socket sck;

        private readonly SmartCardOperation sp;

        private int no_urut;

        private string poli = "";
        //private Socket sck2;

        #region constructor

        public daftar_berobat()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            var cmd = new DBCommand(conn);
            sp = new SmartCardOperation();
            try
            {
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sck.Connect(Settings.Default.SocketServerAntrianPoli,
                    Settings.Default.SocketPortAntriaPoli);
            }
            catch (Exception)
            {
                //MessageBox.Show("Apliasi antrian tidak aktif, pastikan aplikasi antrian aktif.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (sp.IsReaderAvailable())
            {
            }
            else
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var cbp = cmd.GetPoliklinik();

            cbPoliklinik.DisplayMemberPath = "kode_poliklinik";
            cbPoliklinik.SelectedValuePath = "nama_poliklinik";
            cbPoliklinik.ItemsSource = cbp;
            cbPoliklinik.SelectedIndex = 0;
        }

        #endregion

        #region member CRUD operations

        private void tambah_antrian(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtIdPasien.Text) && !string.IsNullOrEmpty(txtIdPasien.Text) &&
                cbPoliklinik.SelectedIndex != 0)
            {
                var cbp = (ComboboxPairs) cbPoliklinik.SelectedItem;
                var policode = cbp.nama_poliklinik;
                var norm = txtIdPasien.Text;
                var no_urut = 0;

                var cmd = new DBCommand(conn);

                if (cmd.CountRmPasienExists(norm) == 1)
                {
                    var last = cmd.GetLastNoUrut(policode);

                    if (last == 0)
                        no_urut = 1;
                    else
                        no_urut = last + 1;

                    this.no_urut = no_urut;
                    poli = cbp.kode_poliklinik;

                    if (cmd.InsertAntrian(norm, no_urut, policode))
                    {
                        MessageBox.Show("Pasien berhasil didaftarkan.\nNomor Antri: " + no_urut, "Informasi",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        txtIdPasien.Text = "";
                        cbPoliklinik.SelectedIndex = 0;

                        try
                        {
                            sck.Send(Encoding.ASCII.GetBytes("Update"));
                        }
                        catch (Exception)
                        {
                        }

                        var pd = new PrintDocument();
                        var ps = new PaperSize("", 300, 540);

                        pd.PrintPage += Pd_PrintPage;
                        pd.PrintController = new StandardPrintController();
                        pd.DefaultPageSettings.Margins.Left = 0;
                        pd.DefaultPageSettings.Margins.Right = 0;
                        pd.DefaultPageSettings.Margins.Top = 0;
                        pd.DefaultPageSettings.Margins.Bottom = 0;
                        pd.DefaultPageSettings.PaperSize = ps;
                        pd.Print();
                    }
                    else
                    {
                        MessageBox.Show("Pasien gagal didaftarkan.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Rekam medis pasien belum teraftar, periksa kembali data pasien.", "Perhatian",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Isikan data dengan benar, pastikan semua data telah benar.", "Perhatian",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            var graphics = e.Graphics;
            var font = new Font("Courier New", 10);
            var fontHeight = font.GetHeight();
            var startX = 50;
            var startY = 55;
            var Offset = 40;
            graphics.DrawString("SELAMAT DATANG DI", new Font("Courier New", 14), new SolidBrush(Color.Black), startX,
                startY + Offset);
            Offset = Offset + 20;
            graphics.DrawString("KLINIK BUNDA MULYA", new Font("Courier New", 14), new SolidBrush(Color.Black), startX,
                startY + Offset);
            //Offset = Offset + 20;
            //graphics.DrawString("Jl. Somawinata Ruko Dream Square 7B", new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            //Offset = Offset + 20;
            //graphics.DrawString("Tlp. 022-86121090", new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 20;
            var underLine = "-------------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX,
                startY + Offset);

            Offset = Offset + 20;
            graphics.DrawString("No. Antrian", new Font("Courier New", 14), new SolidBrush(Color.Black), startX,
                startY + Offset);

            Offset = Offset + 20;
            //String Source= this.source; 
            graphics.DrawString(no_urut.ToString(), new Font("Courier New", 40, FontStyle.Bold),
                new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 20;
            var Grosstotal = "";

            Offset = Offset + 30;
            underLine = "-------------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX,
                startY + Offset);
            Offset = Offset + 20;

            graphics.DrawString(Grosstotal, new Font("Courier New", 10), new SolidBrush(Color.Black), startX,
                startY + Offset);
            Offset = Offset + 20;
            //String DrawnBy = this.drawnBy;
            graphics.DrawString("Poliklinik " + poli, new Font("Courier New", 12), new SolidBrush(Color.Black),
                startX, startY + Offset);
            Offset = Offset + 20;
            graphics.DrawString(DateTime.Now.ToShortDateString(), new Font("Courier New", 14),
                new SolidBrush(Color.Black), startX, startY + Offset);
        }

        private void Checkscan_OnUnchecked(object sender, RoutedEventArgs e)
        {
            txtIdPasien.IsEnabled = true;
        }

        private void Checkscan_OnChecked(object sender, RoutedEventArgs e)
        {
            txtIdPasien.IsEnabled = false;
        }

        private void BtnScanKartu_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.isoReaderInit();
                //card = new MifareCard(isoReader);

                var readData = sp.ReadBlock(Msb, blockNoRekamMedis);
                if (readData != null) txtIdPasien.Text = Util.ToASCII(readData, 0, 16, false);
            }
            catch (Exception)
            {
                MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                sp.isoReaderInit();
            }
        }

        #endregion
    }
}