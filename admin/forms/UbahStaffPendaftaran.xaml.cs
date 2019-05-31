using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.Mifare;
using admin.models;
using admin.Utils;
using admin.views;
using PCSC;
using PCSC.Iso7816;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for UbahStaffPendaftaran.xaml
    /// </summary>
    public partial class UbahStaffPendaftaran : Window
    {
        private const byte Msb = 0x00;
        private readonly byte BlockAlamatFrom = 18;
        private readonly byte BlockAlamatTo = 22;

        private readonly byte BlockId = 12;
        private readonly byte BlockJenisKelamin = 24;
        private readonly byte BlockNamaFrom = 13;
        private readonly byte BlockNamaTo = 16;
        private readonly byte BlockPasswordFrom = 25;
        private readonly byte BlockPasswordTo = 26;
        private readonly byte BlockTelp = 17;
        private readonly DaftarPendaftaran df;
        private MPendaftaran _mDaftarBaru = new MPendaftaran(" ", " ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;

        private readonly byte[] key = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        private MifareCard card;

        private readonly IContextFactory contextFactory = ContextFactory.Instance;
        private IsoReader isoReader;
        private readonly string nfcReader;

        public UbahStaffPendaftaran(string id, string nama, string alamat, string telp, string jenisK,
            DaftarPendaftaran df)
        {
            InitializeComponent();
            DataContext = new MPendaftaran(id, nama, alamat, telp, " ", jenisK);

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

                isoReaderInit();
                card = new MifareCard(isoReader);
            }

            this.df = df;
            if (jenisK == "Pria") cbJenisKelamin.SelectedIndex = 0;
            else if (jenisK == "Wanita") cbJenisKelamin.SelectedIndex = 1;
        }

        private void BtnBatal_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region members UI Control & CRUD Operations

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void AddDokter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddDokter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mDaftarBaru = new MPendaftaran(" ", " ", " ", " ", " ", " ");

            if (checkTextBoxValue())
            {
                var nama = txtNamaDokter.Text;
                var id = txtidDokter.Text.ToUpper();
                var telp = txtTelpDokter.Text;
                var alamat = TextAlamat.Text;
                var jenisK = cbJenisKelamin.Text;

                try
                {
                    if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    var query =
                        "update tb_pendaftaran set nama='" + nama + "', alamat='" + alamat + "', telp='" + telp +
                        "', jenis_kelamin='" + jenisK + "' where id='" + id + "'";
                    var command = new SqlCommand(query, DBConnection.dbConnection());
                    var res = command.ExecuteNonQuery();

                    bool isPrinted = false;

                    if (res == 1)
                    {
                        if (chkCetakKartu.IsChecked == true)
                        {
                            while (!isPrinted)
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(id))
                                    {
                                        if (WriteBlock(Msb, BlockId, Util.ToArrayByte16(id)))
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("Id gagal ditulis.");
                                        }
                                    }

                                    if (nama.Length > 48)
                                        nama = nama.Substring(0, 47);

                                    if (!string.IsNullOrEmpty(nama))
                                    {
                                        if (WriteBlockRange(Msb, BlockNamaFrom, BlockNamaTo, Util.ToArrayByte48(nama)))
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("Nama gagal ditulis.");
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(telp))
                                    {
                                        if (WriteBlock(Msb, BlockTelp, Util.ToArrayByte16(telp)))
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("telp gagal ditulis.");
                                        }
                                    }

                                    if (alamat.Length > 64)
                                        alamat = alamat.Substring(0, 67);

                                    if (!string.IsNullOrEmpty(alamat))
                                    {
                                        if (WriteBlockRange(Msb, BlockAlamatFrom, BlockAlamatTo, Util.ToArrayByte64(alamat)))
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("alamat gagal ditulis.");
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(jenisK))
                                    {
                                        if (WriteBlock(Msb, BlockJenisKelamin, Util.ToArrayByte16(jenisK)))
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("Jenis kelamin gagal ditulis.");
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(id))
                                    {
                                        if (WriteBlockRange(Msb, BlockPasswordFrom, BlockPasswordTo,
                                            Util.ToArrayByte32(Encryptor.MD5Hash(id))))
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("Password gagal ditulis.");
                                        }
                                    }

                                    isPrinted = true;
                                    if (isPrinted) break;
                                }
                                catch (Exception)
                                {
                                    var ans = MessageBox.Show("Penulisan kartu gagal, pastikan kartu sudah berada pada jangkauan reader.\nApakah anda ingin menulis kartu lain kali?", "Error",
                                        MessageBoxButton.YesNo, MessageBoxImage.Error);

                                    if (ans == MessageBoxResult.Yes)
                                        break;

                                    isoReaderInit();
                                }
                            }
                        }

                        MessageBox.Show("Data staff berhasil disimpan.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        df.displayDataPendaftar();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Data staff gagal disimpan.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Periksa kembali data yang akan di inputkan.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            cbJenisKelamin.SelectedIndex = 0;
            e.Handled = true;
        }

        private bool checkTextBoxValue()
        {
//            if (txtidDokter.Text == " " && txtNamaDokter.Text == " " && txtTelpDokter.Text == " " &&
//                txtSpesialisai.Text == " " && TextAlamat.Text == " ") return false;

            if (!string.IsNullOrWhiteSpace(txtidDokter.Text) && !string.IsNullOrWhiteSpace(txtNamaDokter.Text) &&
                !string.IsNullOrWhiteSpace(txtTelpDokter.Text) &&
                !string.IsNullOrWhiteSpace(TextAlamat.Text))
                return true;

            return false;
        }

        #endregion

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

                if (WriteBlock(msb, i, blockData)) count++;
                else return false;
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