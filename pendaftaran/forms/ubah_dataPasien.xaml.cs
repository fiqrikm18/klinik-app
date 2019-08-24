using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PCSC;
using PCSC.Iso7816;
using pendaftaran.DBAccess;
using pendaftaran.Mifare;
using pendaftaran.models;
using pendaftaran.Utils;
using pendaftaran.views;

namespace pendaftaran.forms
{
    /// <summary>
    ///     Interaction logic for ubah_dataPasien.xaml
    /// </summary>
    public partial class ubah_dataPasien : Window
    {
        private const byte Msb = 0x00;
        private readonly byte blockAlamatForm = 8;

        private readonly byte blockAlamatTo = 12;

        //private readonly byte blockGolDarah = 13;
        //private readonly byte blockIdPasien = 1;
        //private readonly byte blockJenisId = 18;
        private readonly byte blockJenisKelamin = 17;
        private readonly byte blockNamaFrom = 4;

        private readonly byte blockNamaTo = 6;

        //private readonly byte blockNoRekamMedis = 2;
        private readonly byte blockNoTelp = 14;
        //private readonly byte blockTglLahir = 16;

        private readonly IContextFactory contextFactory = ContextFactory.Instance;
        private readonly daftar_ulang du;

        private readonly string jk;

        private readonly byte[] key = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
        private readonly string nfcReader;

        private MDaftarBaru _mDaftarBaru = new MDaftarBaru(" ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;
        private string alamat;
        private MifareCard card;

        private SqlConnection conn;
        private string golDarah;

        private string idP;
        private IsoReader isoReader;
        private string nama;

        private string norm;
        private string telp;

        public ubah_dataPasien(string norm, string idp, string nama, string jk, string notlp, string alamat,
            string golDarah,
            daftar_ulang du)
        {
            InitializeComponent();
            var cbp = new List<ComboboxPairs>();
            DataContext = new MDaftarBaru(norm, idp, nama, notlp, alamat);

            this.norm = norm;
            idP = idp;
            this.golDarah = golDarah;
            this.nama = nama;
            this.jk = jk;
            telp = notlp;
            this.alamat = alamat;
            this.du = du;

            if (jk == "Pria")
                cbJenisKelamin.SelectedIndex = 0;
            else if (jk == "Wanita") cbJenisKelamin.SelectedIndex = 1;

            cbGolDarah.Text = golDarah;

            var ctx = contextFactory.Establish(SCardScope.System);
            var readerNames = ctx.GetReaders();

            if (NoReaderAvailable(readerNames))
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                nfcReader = readerNames[0];
                if (string.IsNullOrEmpty(nfcReader))
                    MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
            _mDaftarBaru = new MDaftarBaru(" ", "", "", " ", " ");

            var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = ci;

            //DateTime dt = DateTime.ParseExact(, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var identitas = TxtNoIdentitas.Text;
            var namaPasien = TxtNamaPasien.Text;
            var noTelp = TxtNoTelp.Text;
            var alamat = TextAlamat.Text;
            var gd = cbGolDarah.Text;
            var jenisKelamin = cbJenisKelamin.Text;
            conn = DBConnection.dbConnection();
            var cmd = new DBCommand(conn);

            if (!Regex.IsMatch(identitas, "^[A-Za-z]+$") && !Regex.IsMatch(noTelp, "^[A-Za-z]+$") &&
                Regex.IsMatch(namaPasien, @"^[a-zA-Z\s]*$"))
            {
                if (checkTextBoxValue())
                {
                    if (cmd.UpdateDataPasien(namaPasien, noTelp, jenisKelamin, alamat, identitas, gd))
                    {
                        var isPrinted = false;
                        if (chkUpdateKartu.IsChecked == true)
                            while (!isPrinted)
                                try
                                {
                                    if (namaPasien.Length > 48) namaPasien = namaPasien.Substring(0, 47);

                                    if (!string.IsNullOrEmpty(namaPasien))
                                    {
                                        if (WriteBlockRange(Msb, blockNamaFrom, blockNamaTo,
                                            Util.ToArrayByte48(namaPasien)))
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("Nama pasien gagal ditulis");
                                        }
                                    }

                                    if (alamat.Length > 64) alamat = alamat.Substring(0, 63);

                                    if (!string.IsNullOrEmpty(alamat))
                                    {
                                        if (WriteBlockRange(Msb, blockAlamatForm, blockAlamatTo,
                                            Util.ToArrayByte64(alamat)))
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("Alamat pasien gagal ditulis");
                                        }
                                    }

                                    if (string.IsNullOrEmpty(noTelp))
                                    {
                                        if (WriteBlock(Msb, blockNoTelp, Util.ToArrayByte16(noTelp)))
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("Nomor telepon pasien gagal ditulis");
                                        }
                                    }

                                    if (string.IsNullOrEmpty(jenisKelamin))
                                    {
                                        if (WriteBlock(Msb, blockJenisKelamin, Util.ToArrayByte16(jenisKelamin)))
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("Jenis Kelamin pasien gagal ditulis");
                                        }
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

                                    if (ans == MessageBoxResult.Yes) break;

                                    isoReaderInit();
                                }

                        MessageBox.Show("Berhasil memperbarui data pasien.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        du.DisplayDataPasien();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Harap periksa kembali data yang diinputkan.", "Warning", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Periksa kembali data yang akan di input.", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }


            //DataContext = _mDaftarBaru;
            if (jk == "Pria")
                cbJenisKelamin.SelectedIndex = 0;
            else if (jk == "Wanita") cbJenisKelamin.SelectedIndex = 1;

            e.Handled = true;
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private bool checkTextBoxValue()
        {
            //            if (TxtNoRm.Text == " " && TxtNoIdentitas.Text == " " && TxtNamaPasien.Text == " " &&
            //                TxtNoTelp.Text == " " && TextAlamat.Text == " " &&
            //                dtTanggalLahir.SelectedDate.ToString() == null) return false;

            if (!string.IsNullOrWhiteSpace(TxtNoRm.Text) && !string.IsNullOrWhiteSpace(TxtNoIdentitas.Text) &&
                !string.IsNullOrWhiteSpace(TxtNamaPasien.Text) && !string.IsNullOrWhiteSpace(TxtNoTelp.Text) &&
                !string.IsNullOrWhiteSpace(TextAlamat.Text))
                return true;

            return false;
        }

        private void Batal(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region member smart card operations

        private bool NoReaderAvailable(ICollection<string> readerNames)
        {
            return readerNames == null || readerNames.Count < 1;
        }

        private bool WriteBlock(byte msb, byte lsb, byte[] data)
        {
            isoReaderInit();
            card = new MifareCard(isoReader);

            if (card.LoadKey(KeyStructure.VolatileMemory, 0x00, key))
            {
                if (card.Authenticate(msb, lsb, KeyType.KeyA, 0x00))
                    if (card.UpdateBinary(msb, lsb, data))
                        return true;

                return false;
            }

            return false;
        }

        private bool WriteBlockRange(byte msb, byte blockFrom, byte blockTo, byte[] data)
        {
            isoReaderInit();
            card = new MifareCard(isoReader);

            byte i;
            var count = 0;
            var blockData = new byte[16];

            for (i = blockFrom; i <= blockTo; i++)
            {
                if ((i + 1) % 4 == 0) continue;

                Array.Copy(data, count * 16, blockData, 0, 16);

                if (WriteBlock(msb, i, blockData))
                    count++;
                else
                    return false;
            }

            return true;
        }

        private byte[] ReadBlock(byte msb, byte lsb)
        {
            isoReaderInit();
            card = new MifareCard(isoReader);

            var readBinary = new byte[16];

            if (card.LoadKey(KeyStructure.VolatileMemory, 0x00, key))
                if (card.Authenticate(msb, lsb, KeyType.KeyA, 0x00))
                    readBinary = card.ReadBinary(msb, lsb, 16);

            return readBinary;
        }

        //        private byte[] ReadBlockRange(byte msb, byte blockFrom, byte blockTo)
        //        {
        //            byte i;
        //            int nBlock = 0;
        //            int count = 0;
        //
        //            for (i = blockFrom; i <= blockTo;)
        //            {
        //                if ((i + 1) % 4 == 0) continue;
        //                nBlock++;
        //            }
        //
        //            var dataOut = new byte[nBlock * 16];
        //            for (i = blockFrom; i <= blockTo; i++)
        //            {
        //                if ((i + 1) % 4 == 0) continue;
        //                Array.Copy(ReadBlock(msb, i), 0, dataOut, count * 16, 16);
        //                count++;
        //            }
        //
        //            return dataOut;
        //        }


        public void connect()
        {
            var ctx = new SCardContext();
            ctx.Establish(SCardScope.System);
            var reader = new SCardReader(ctx);
            reader.Connect(nfcReader, SCardShareMode.Shared, SCardProtocol.Any);
        }

        private void isoReaderInit()
        {
            try
            {
                var ctx = contextFactory.Establish(SCardScope.System);
                isoReader = new IsoReader(
                    ctx,
                    nfcReader,
                    SCardShareMode.Shared,
                    SCardProtocol.Any,
                    false);

                card = new MifareCard(isoReader);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private byte[] ReadBlockRange(byte msb, byte blockFrom, byte blockTo)
        {
            isoReaderInit();
            card = new MifareCard(isoReader);

            byte i;
            var nBlock = 0;
            var count = 0;

            for (i = blockFrom; i <= blockTo; i++)
            {
                if ((i + 1) % 4 == 0) continue;

                nBlock++;
            }

            var dataOut = new byte[nBlock * 16];
            for (i = blockFrom; i <= blockTo; i++)
            {
                if ((i + 1) % 4 == 0) continue;

                Array.Copy(ReadBlock(msb, i), 0, dataOut, count * 16, 16);
                count++;
            }

            return dataOut;
        }

        public void ClearAllBlock()
        {
            var res = MessageBox.Show("Apakah anda yakin ingin menghapus data kartu? ", "Warning",
                MessageBoxButton.YesNo);

            if (res == MessageBoxResult.Yes)
            {
                isoReaderInit();
                card = new MifareCard(isoReader);

                var data = new byte[16];
                if (card.LoadKey(KeyStructure.VolatileMemory, 0x00, key))
                {
                    for (byte i = 1; i <= 63; i++)
                        if ((i + 1) % 4 == 0)
                        {
                        }
                        else
                        {
                            if (card.Authenticate(Msb, i, KeyType.KeyA, 0x00))
                            {
                                Array.Clear(data, 0, 16);
                                if (WriteBlock(Msb, i, data))
                                {
                                }
                                else
                                {
                                    MessageBox.Show("Data gagal dihapus");
                                    break;
                                }
                            }
                        }

                    MessageBox.Show("Data berhasil dihapus", "Informasi", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }

        #endregion
    }
}