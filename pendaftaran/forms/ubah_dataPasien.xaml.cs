using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySql.Data.MySqlClient;
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
        private readonly byte blockAlamatForm = 18;
        private readonly byte blockAlamatTo = 22;
        private readonly byte blockJenisKelamin = 26;
        private readonly byte blockNamaFrom = 14;
        private readonly byte blockNamaTo = 17;
        private readonly byte blockNoTelp = 24;
        private readonly MifareCard card;
        private readonly daftar_ulang du;

        private readonly IsoReader isoReader;
        private readonly string jk;

        private readonly byte[] key = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
        private MDaftarBaru _mDaftarBaru = new MDaftarBaru(" ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;
        private string alamat;

        private byte blockIdPasien = 12;
        private byte blockNoRekamMedis = 13;
        private byte blockTglLahir = 25;
        private string idP;
        private string nama;

        private string norm;
        private string telp;

        public ubah_dataPasien(string norm, string idp, string nama, string jk, string notlp, string alamat,
            daftar_ulang du)
        {
            InitializeComponent();
            var cbp = new List<ComboboxPairs>();
            DataContext = new MDaftarBaru(norm, idp, nama, notlp, alamat);

            this.norm = norm;
            idP = idp;
            this.nama = nama;
            this.jk = jk;
            telp = notlp;
            this.alamat = alamat;
            this.du = du;

            if (jk == "Pria") cbJenisKelamin.SelectedIndex = 0;
            else if (jk == "Wanita") cbJenisKelamin.SelectedIndex = 1;

            var contextFactory = ContextFactory.Instance;
            var ctx = contextFactory.Establish(SCardScope.System);
            var readerNames = ctx.GetReaders();

            if (NoReaderAvailable(readerNames))
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var nfcReader = readerNames[0];
                if (string.IsNullOrEmpty(nfcReader))
                    MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                try
                {
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
        }

        private bool NoReaderAvailable(ICollection<string> readerNames)
        {
            return readerNames == null || readerNames.Count < 1;
        }

        private bool WriteBlock(byte msb, byte lsb, byte[] data)
        {
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
            byte i;
            var count = 0;
            var blockData = new byte[16];

            for (i = blockFrom; i < blockTo; i++)
            {
                if ((i + 1) % 4 == 0) continue;

                Array.Copy(data, count * 16, blockData, 0, 16);

                if (WriteBlock(msb, i, blockData)) count++;
                else return false;
            }

            return true;
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

            if (checkTextBoxValue())
            {
                var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
                ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                Thread.CurrentThread.CurrentCulture = ci;

                //DateTime dt = DateTime.ParseExact(, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                var identitas = TxtNoIdentitas.Text;
                var namaPasien = TxtNamaPasien.Text;
                var noTelp = TxtNoTelp.Text;
                var alamat = TextAlamat.Text;
                var jenisKelamin = cbJenisKelamin.Text;

                try
                {
                    if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    var query = "update pasien set nama='" + namaPasien + "', jenis_kelamin='" + jenisKelamin +
                                "', no_telp='" + noTelp + "', alamat='" + alamat + "' where no_identitas='" +
                                identitas + "';";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var res = cmd.ExecuteNonQuery();

                    if (res == 1)
                    {
                        if (chkUpdateKartu.IsChecked == true)
                        {
                            if (namaPasien.Length > 48)
                                namaPasien = namaPasien.Substring(0, 47);

                            if (!string.IsNullOrEmpty(namaPasien))
                            {
                                if (WriteBlockRange(Msb, blockNamaFrom, blockNamaTo, Util.ToArrayByte48(namaPasien)))
                                {
                                }
                                else
                                {
                                    MessageBox.Show("Nama pasien gagal ditulis");
                                }
                            }

                            if (alamat.Length > 64)
                                alamat = alamat.Substring(0, 63);

                            if (!string.IsNullOrEmpty(alamat))
                            {
                                if (WriteBlockRange(Msb, blockAlamatForm, blockAlamatTo, Util.ToArrayByte64(alamat)))
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
                        }

                        MessageBox.Show("Berhasil memperbarui data pasien.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        du.displayDataPasien();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Gagal memperbarui data pasein.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        DataContext = _mDaftarBaru;
                        if (jk == "Pria") cbJenisKelamin.SelectedIndex = 0;
                        else if (jk == "Wanita") cbJenisKelamin.SelectedIndex = 1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan.\n" + ex.Message, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Periksa kembali data yang akan di input.", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            DataContext = _mDaftarBaru;
            if (jk == "Pria") cbJenisKelamin.SelectedIndex = 0;
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
    }
}