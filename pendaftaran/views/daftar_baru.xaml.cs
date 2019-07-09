﻿using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using pendaftaran.DBAccess;
using pendaftaran.Mifare;
using pendaftaran.models;
using pendaftaran.Utils;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;

namespace pendaftaran.views
{
    /// <summary>
    ///     Interaction logic for daftar_baru.xaml
    /// </summary>
    public partial class daftar_baru : Page
    {
        private const byte Msb = 0x00;

        private readonly byte blockAlamatForm = 8;
        private readonly byte blockAlamatTo = 12;
        private readonly byte blockGolDarah = 13;
        private readonly byte blockIdPasien = 1;
        private readonly byte blockJenisKelamin = 17;
        private readonly byte blockNamaFrom = 4;
        private readonly byte blockNamaTo = 6;
        private readonly byte blockNoRekamMedis = 2;
        private readonly byte blockNoTelp = 14;
        private readonly byte blockTglLahir = 16;
        private readonly byte[] key = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        private MDaftarBaru _mDaftarBaru = new MDaftarBaru(" ", " ", " ", " ", " ");

        //private static MySqlConnection MsqlConn = null;
        private int _noOfErrorsOnScreen;

        private readonly SqlConnection conn;

        private readonly SmartCardOperation sp;
        private string no_rm;
        Socket sck;
        Socket sck2;

        int no_urut = 0;
        string poli = "";

        #region constructor

        public daftar_baru()
        {
            InitializeComponent();

            sp = new SmartCardOperation();

            if (sp.IsReaderAvailable())
            {
            }
            else
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            conn = DBConnection.dbConnection();

            DataContext = new MDaftarBaru(" ", " ", " ", " ", " ");
            conn = DBConnection.dbConnection();

            var dbcmd = new DBCommand(conn);
            var cbp = dbcmd.GetPoliklinik();

            cbPoliklinik.DisplayMemberPath = "kode_poliklinik";
            cbPoliklinik.SelectedValuePath = "nama_poliklinik";
            cbPoliklinik.ItemsSource = cbp;
            cbPoliklinik.SelectedIndex = 0;

            try
            {
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sck.Connect(Properties.Settings.Default.SocketServerAntrianPoli, Properties.Settings.Default.SocketPortAntriaPoli);

                sck2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sck2.Connect(Properties.Settings.Default.SocketServerPAntrian, Properties.Settings.Default.SocketPortPAntrian);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Apliasi antrian tidak aktif, pastikan aplikasi antrian aktif.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        private void btnPrintLabel_Click(object sender, RoutedEventArgs e)
        {
            forms.PrintPreview pv = new forms.PrintPreview(no_rm);
            pv.Show();
        }

        #region member UI control & CRUD operations

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;
        }

        private void AddPasien_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddPasien_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mDaftarBaru = new MDaftarBaru(" ", " ", " ", " ", " ");
            var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = ci;

            var cmd = new DBCommand(conn);

            if (checkTextBoxValue() && dtTanggalLahir.SelectedDate != null)
            {
                var cbp = (ComboboxPairs)cbPoliklinik.SelectedItem;
                var policode = cbp.nama_poliklinik;
                //DateTime dt = DateTime.ParseExact(, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                var norm = TxtNoRm.Text.ToUpper();
                this.no_rm = norm;
                var identitas = TxtNoIdentitas.Text;
                var namaPasien = TxtNamaPasien.Text;
                var noTelp = TxtNoTelp.Text;
                var alamat = TextAlamat.Text;
                var tglLahir = dtTanggalLahir.SelectedDate.Value.Date.ToString("yyyy-MM-dd");
                var jenisKelamin = cbJenisKelamin.Text;
                var poliklinik = policode;
                var golDarah = cbGolDarah.Text;

                if (cmd.CountIdPasienExists(identitas) != 1)
                {
                    if (cmd.CountRmPasienExists(norm) != 1)
                    {
                        if (cmd.InsertDataPasien(identitas, norm, namaPasien, tglLahir, jenisKelamin, noTelp, alamat,
                            golDarah))
                        {
                            var last = cmd.GetLastNoUrut(policode);
                            var no_urut = 0;

                            if (last == 0)
                                no_urut = 1;
                            else
                                no_urut = last + 1;

                            this.no_urut = no_urut;
                            this.poli = cbp.kode_poliklinik;

                            if (cmd.InsertAntrian(norm, no_urut, policode))
                            {
                                var isPrinted = false;

                                if (chkCetakKartu.IsChecked == true)
                                {
                                    while (!isPrinted)
                                        try
                                        {
                                            if (!string.IsNullOrEmpty(identitas))
                                            {
                                                if (sp.WriteBlock(Msb, blockIdPasien, Util.ToArrayByte16(identitas)))
                                                    isPrinted = true;
                                                else
                                                    MessageBox.Show("ID pasien gagal ditulis");
                                            }

                                            if (!string.IsNullOrEmpty(golDarah))
                                            {
                                                if (sp.WriteBlock(Msb, blockGolDarah,
                                                    Util.ToArrayByte16(" " + golDarah)))
                                                {
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Golongan Darah gagal ditulis");
                                                }
                                            }

                                            if (!string.IsNullOrEmpty(norm))
                                            {
                                                if (sp.WriteBlock(Msb, blockNoRekamMedis, Util.ToArrayByte16(norm)))
                                                    isPrinted = true;
                                                else
                                                    MessageBox.Show("Nomor rekam medis gagal ditulis");
                                            }

                                            if (namaPasien.Length > 48)
                                                namaPasien = namaPasien.Substring(0, 47);

                                            if (!string.IsNullOrEmpty(namaPasien))
                                            {
                                                if (sp.WriteBlockRange(Msb, blockNamaFrom, blockNamaTo,
                                                    Util.ToArrayByte48(namaPasien)))
                                                    isPrinted = true;
                                                else
                                                    MessageBox.Show("Nama pasien gagal ditulis");
                                            }

                                            if (!string.IsNullOrEmpty(tglLahir))
                                            {
                                                if (sp.WriteBlock(Msb, blockTglLahir, Util.ToArrayByte16(tglLahir)))
                                                    isPrinted = true;
                                                else
                                                    MessageBox.Show("Tanggal lahir pasien gagal ditulis");
                                            }

                                            if (!string.IsNullOrEmpty(jenisKelamin))
                                            {
                                                if (sp.WriteBlock(Msb, blockJenisKelamin,
                                                    Util.ToArrayByte16(jenisKelamin)))
                                                    isPrinted = true;
                                                else
                                                    MessageBox.Show("Jenis kelamin pasien gagal ditulis");
                                            }

                                            if (!string.IsNullOrEmpty(noTelp))
                                            {
                                                if (sp.WriteBlock(Msb, blockNoTelp, Util.ToArrayByte16(noTelp)))
                                                    isPrinted = true;
                                                else
                                                    MessageBox.Show("Nomor telepon pasien gagal ditulis");
                                            }

                                            if (alamat.Length > 64)
                                                alamat = alamat.Substring(0, 63);

                                            if (!string.IsNullOrEmpty(alamat))
                                            {
                                                if (sp.WriteBlockRange(Msb, blockAlamatForm, blockAlamatTo,
                                                    Util.ToArrayByte64(alamat)))
                                                    isPrinted = true;
                                                else
                                                    MessageBox.Show("Alamat pasien gagal ditulis");
                                            }

                                            isPrinted = true;
                                            if (isPrinted) break;
                                        }
                                        catch (Exception)
                                        {
                                            var ans = MessageBox.Show(
                                                "Penulisan kartu gagal, pastikan kartu sudah berada pada jangkauan reader.\nApakah anda ingin menulis kartu lain kali?",
                                                "Error",
                                                MessageBoxButton.YesNo, MessageBoxImage.Error);

                                            if (ans == MessageBoxResult.Yes)
                                                break;

                                            sp.isoReaderInit();
                                        }

                                    MessageBox.Show(
                                        "Pasien berhasil didaftarkan.\nKartu pasien berhasil ditulis.\nNomor Antri: " +
                                        no_urut + "", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                                    DataContext = _mDaftarBaru;
                                    cbPoliklinik.SelectedIndex = 0;
                                    cbGolDarah.SelectedIndex = 0;
                                    cbJenisKelamin.SelectedIndex = 0;
                                    cbPoliklinik.SelectedIndex = 0;

                                    PrintDocument pd = new PrintDocument();
                                    PaperSize ps = new PaperSize("", 100, 200);

                                    pd.PrintPage += Pd_PrintPage;
                                    pd.PrintController = new StandardPrintController();
                                    pd.DefaultPageSettings.Margins.Left = 0;
                                    pd.DefaultPageSettings.Margins.Right = 0;
                                    pd.DefaultPageSettings.Margins.Top = 0;
                                    pd.DefaultPageSettings.Margins.Bottom = 0;
                                    pd.DefaultPageSettings.PaperSize = ps;
                                    pd.Print();

                                    try
                                    {
                                        sck.Send(Encoding.ASCII.GetBytes("Update"));
                                        sck2.Send(Encoding.ASCII.GetBytes("Update"));
                                    }
                                    catch { }
                                }
                                else
                                {
                                    MessageBox.Show("Pasien berhasil didaftarkan.\nNomor Antri: " + no_urut,
                                        "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                                    DataContext = _mDaftarBaru;
                                    cbJenisKelamin.SelectedIndex = 0;
                                    cbPoliklinik.SelectedIndex = 0;

                                    try
                                    {
                                        sck.Send(Encoding.ASCII.GetBytes("Update"));
                                        sck2.Send(Encoding.ASCII.GetBytes("Update"));
                                    }
                                    catch { }

                                    PrintDocument pd = new PrintDocument();
                                    PaperSize ps = new PaperSize("", 100, 200);

                                    pd.PrintPage += Pd_PrintPage;
                                    pd.PrintController = new StandardPrintController();
                                    pd.DefaultPageSettings.Margins.Left = 0;
                                    pd.DefaultPageSettings.Margins.Right = 0;
                                    pd.DefaultPageSettings.Margins.Top = 0;
                                    pd.DefaultPageSettings.Margins.Bottom = 0;
                                    pd.DefaultPageSettings.PaperSize = ps;
                                    pd.Print();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Data berhasil didaftarkan.", "Informasi", MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                                try
                                {
                                    sck.Send(Encoding.ASCII.GetBytes("Update"));
                                    sck2.Send(Encoding.ASCII.GetBytes("Update"));
                                }
                                catch { }

                                PrintDocument pd = new PrintDocument();
                                    PaperSize ps = new PaperSize("", 100, 200);

                                    pd.PrintPage += Pd_PrintPage;
                                    pd.PrintController = new StandardPrintController();
                                    pd.DefaultPageSettings.Margins.Left = 0;
                                    pd.DefaultPageSettings.Margins.Right = 0;
                                    pd.DefaultPageSettings.Margins.Top = 0;
                                    pd.DefaultPageSettings.Margins.Bottom = 0;
                                    pd.DefaultPageSettings.PaperSize = ps;
                                    pd.Print();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Data pasien gagal didaftartkan.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No rekam medis sudah terdaftar.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("No indentitas sudah terdaftar.", "Informasi", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Harap periksa kembali data yang ingin di inputkan, pastikan semua sudah diisi.",
                    "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            e.Handled = true;
        }

        private void Pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Font font = new Font("Courier New", 10);
            float fontHeight = font.GetHeight();
            int startX = 50;
            int startY = 55;
            int Offset = 40;
            graphics.DrawString("KLINIK BUNDA MULYA", new Font("Courier New", 14), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 20;
            graphics.DrawString("Jl. Somawinata Ruko Dream Square 7B", new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 20;
            graphics.DrawString("Tlp. 022-86121090", new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 20;
            String underLine = "------------------------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 20;
            //String Source= this.source; 
            graphics.DrawString(this.no_urut.ToString(), new Font("Courier New", 26, System.Drawing.FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 20;
            String Grosstotal = "";

            Offset = Offset + 20;
            underLine = "------------------------------------------";
            graphics.DrawString(underLine, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 20;

            graphics.DrawString(Grosstotal, new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + Offset);
            Offset = Offset + 20;
            //String DrawnBy = this.drawnBy;
            graphics.DrawString("Poliklinik " + this.poli, new Font("Courier New", 12), new SolidBrush(Color.Black), startX, startY + Offset);
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            if (string.IsNullOrEmpty(source.Text) || string.IsNullOrWhiteSpace(source.Text) || source.Text == " ")
                source.Clear();
        }

        private bool checkTextBoxValue()
        {
            //            if (TxtNoRm.Text == " " && TxtNoIdentitas.Text == " " && TxtNamaPasien.Text == " " &&
            //                TxtNoTelp.Text == " " && TextAlamat.Text == " " &&
            //                dtTanggalLahir.SelectedDate.ToString() == null) return false;

            if (!string.IsNullOrWhiteSpace(TxtNoRm.Text) && !string.IsNullOrWhiteSpace(TxtNoIdentitas.Text) &&
                !string.IsNullOrWhiteSpace(TxtNamaPasien.Text) && !string.IsNullOrWhiteSpace(TxtNoTelp.Text) &&
                !string.IsNullOrWhiteSpace(TextAlamat.Text) && cbGolDarah.SelectedIndex != 0 &&
                !string.IsNullOrWhiteSpace(dtTanggalLahir.SelectedDate.ToString()) && cbPoliklinik.SelectedIndex != 0 &&
                cbJenisKelamin.SelectedIndex != 0)
                return true;

            return false;
        }

        #endregion

        #region members event buttons

        private void BtnCekKartu_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.isoReaderInit();
                //card = new MifareCard(isoReader);

                var msg = "";
                var rm = sp.ReadBlock(Msb, blockNoRekamMedis);
                if (rm != null)
                    msg += "Nomor Rekam Medis \t: " + Util.ToASCII(rm, 0, 16, false);

                var nId = sp.ReadBlock(Msb, blockIdPasien);
                if (rm != null)
                    msg += "\nNomor ID Pasien \t\t: " + Util.ToASCII(nId, 0, 16, false);

                var namaP = sp.ReadBlockRange(Msb, blockNamaFrom, blockNamaTo);
                if (namaP != null)
                    msg += "\nNama Pasien \t\t: " + Util.ToASCII(namaP, 0, 48, false);

                var gDarah = sp.ReadBlock(Msb, blockGolDarah);
                if (gDarah != null)
                    msg += "\nGolongan Darah \t\t: " + Util.ToASCII(gDarah, 0, 16, false);

                var nTelp = sp.ReadBlock(Msb, blockNoTelp);
                if (nTelp != null)
                    msg += "\nNomor Telepon Pasien \t: " + Util.ToASCII(nTelp, 0, 16, false);

                var alamatP = sp.ReadBlockRange(Msb, blockAlamatForm, blockAlamatTo);
                if (alamatP != null)
                    msg += "\nAlamat Pasien \t\t: " + Util.ToASCII(alamatP, 0, 64, false);

                var tglHarie = sp.ReadBlock(Msb, blockTglLahir);
                if (tglHarie != null)
                    msg += "\nTanggal Lahir \t\t: " + Util.ToASCII(tglHarie, 0, 16, false);

                var jk = sp.ReadBlock(Msb, blockJenisKelamin);
                if (jk != null)
                    msg += "\nJenis Kelamin \t\t: " + Util.ToASCII(jk, 0, 16, false);

                MessageBox.Show(msg, "Informasi Kartu Pasien", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.\n",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                sp.isoReaderInit();
            }
        }

        private void BtnHapusKartu_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.isoReaderInit();
                //card = new MifareCard(isoReader);

                if (sp.ClearAllBlock())
                    MessageBox.Show("Data pada kartu berhasil dihapus.", "Info", MessageBoxButton.OK,
                        MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.\n",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                sp.isoReaderInit();
            }
        }

        #endregion
    }
}