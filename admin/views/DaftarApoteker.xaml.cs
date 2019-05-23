using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.forms;
using admin.Mifare;
using admin.Utils;
using MySql.Data.MySqlClient;
using PCSC;
using PCSC.Iso7816;

namespace admin.views
{
    /// <summary>
    ///     Interaction logic for DaftarApoteker.xaml
    /// </summary>
    public partial class DaftarApoteker : Page
    {
        private const byte Msb = 0x00;
        private IsoReader isoReader;
        private MifareCard card;
        private byte[] key = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};

        private readonly byte BlockId = 12;
        private readonly byte BlockNamaFrom = 13;
        private readonly byte BlockNamaTo = 16;
        private readonly byte BlockTelp = 17;
        private readonly byte BlockAlamatFrom = 18;
        private readonly byte BlockAlamatTo = 22;
        private readonly byte BlockJenisKelamin = 24;
        private readonly byte BlockPasswordFrom = 25;
        private readonly byte BlockPasswordTo = 26;

        public DaftarApoteker()
        {
            InitializeComponent();
            displayDataApoteker();

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

        #region member smart card operations

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

        private byte[] ReadBlockRange(byte msb, byte blockFrom, byte blockTo)
        {
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
            var res = MessageBox.Show("ApaApakah anda yakin ingin menghapus data kartu? ", "Warning",
                MessageBoxButton.YesNo);

            if (res == MessageBoxResult.Yes)
            {
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

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void TxtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nama = sender as TextBox;

            if (nama.Text != "Nama Apoteker")
                displayDataApoteker(nama.Text);
        }

        public void displayDataApoteker(string nama = null)
        {
            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();

                string query;

                if (!string.IsNullOrEmpty(nama))
                {
                    query = "select * from apoteker where nama_apoteker like '%" + nama + "%';";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataApoteker.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
                else
                {
                    query = "select * from apoteker";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var adapter = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataApoteker.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message,
                    "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnTambahApoteker_OnClick(object sender, RoutedEventArgs e)
        {
            var ta = new TambahApoteker(this);
            ta.Show();
        }

        private void BtnUbahApoteker_OnClick(object sender, RoutedEventArgs e)
        {
            var id = "";
            var nama = "";
            var telp = "";
            var alamat = "";
            var jenisK = "";

            if (dtgDataApoteker.SelectedItems.Count > 0)
                for (var i = 0; i < dtgDataApoteker.SelectedItems.Count; i++)
                {
                    id = (dtgDataApoteker.SelectedCells[0].Column
                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock).Text;
                    nama = (dtgDataApoteker.SelectedCells[1].Column
                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock).Text;
                    jenisK = (dtgDataApoteker.SelectedCells[2].Column
                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock).Text;
                    telp = (dtgDataApoteker.SelectedCells[3].Column
                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock).Text;
                    alamat = (dtgDataApoteker.SelectedCells[4].Column
                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock).Text;
                }

            var ua = new UbahApoteker(id, nama, alamat, telp, jenisK, this);
            ua.Show();
        }

        private void BtnHapusApoteker_OnClick(object sender, RoutedEventArgs e)
        {
            if (dtgDataApoteker.SelectedItems.Count > 0)
            {
                var a = MessageBox.Show("Anda yakin ingin menghapus data apoteker?", "Konfirmasi",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (a == MessageBoxResult.Yes)
                {
                    string query;
                    var res = 0;

                    if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    try
                    {
                        for (var i = 0; i < dtgDataApoteker.SelectedItems.Count; i++)
                        {
                            query = "delete from apoteker where id_apoteker = '" +
                                    (dtgDataApoteker.SelectedCells[0].Column
                                        .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock)?.Text + "';";
                            var command = new MySqlCommand(query, DBConnection.dbConnection());
                            res = command.ExecuteNonQuery();
                        }

                        if (res == 1)
                            MessageBox.Show("Data apoteker berhasil dihapus.", "Informasi", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        else
                            MessageBox.Show("Data apoteker gagal dihapus.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                displayDataApoteker();
                DBConnection.dbConnection().Close();
            }
            else
            {
                MessageBox.Show("Pilih data apoteker yang akan dihapus.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnCetakKartu_OnClick(object sender, RoutedEventArgs e)
        {
            var id = "";
            var nama = "";
            var telp = "";
            var alamat = "";
            var jenisK = "";

            if (dtgDataApoteker.SelectedItems.Count > 0)
            {
                for (var i = 0; i < dtgDataApoteker.SelectedItems.Count; i++)
                {
                    id =
                        (dtgDataApoteker.SelectedCells[0].Column
                            .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock)
                        .Text;
                    nama =
                        (dtgDataApoteker.SelectedCells[1].Column
                            .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock)
                        .Text;
                    jenisK =
                        (dtgDataApoteker.SelectedCells[2].Column
                            .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock)
                        .Text;
                    telp =
                        (dtgDataApoteker.SelectedCells[3].Column
                            .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock)
                        .Text;
                    alamat =
                        (dtgDataApoteker.SelectedCells[4].Column
                            .GetCellContent(dtgDataApoteker.SelectedItems[i]) as TextBlock)
                        .Text;
                }

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
                    if (WriteBlockRange(Msb, BlockPasswordFrom, BlockPasswordTo, Util.ToArrayByte32(Encryptor.MD5Hash(id))))
                    {
                    }
                    else
                    {
                        MessageBox.Show("Password gagal ditulis.");
                    }
                }

                MessageBox.Show("Kartu staff berhasil ditulis.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void BtnCekKartu_OnClick(object sender, RoutedEventArgs e)
        {
            var msg = "";
            var id = ReadBlock(Msb, BlockId);
            if (id != null) msg += "ID: " + Util.ToASCII(id, 0, 16);

            var nama = ReadBlockRange(Msb, BlockNamaFrom, BlockNamaTo);
            if (nama != null) msg += "\nNama: " + Util.ToASCII(nama, 0, 48);

            var telp = ReadBlock(Msb, BlockTelp);
            if (telp != null) msg += "\nTelp: " + Util.ToASCII(telp, 0, 16);

            var alamat = ReadBlockRange(Msb, BlockAlamatFrom, BlockAlamatTo);
            if (alamat != null) msg += "\nAlamat: " + Util.ToASCII(alamat, 0, 64);

            var jenisK = ReadBlock(Msb, BlockJenisKelamin);
            if (jenisK != null) msg += "\nJenis Kelamin: " + Util.ToASCII(jenisK, 0, 16);

            var pass = ReadBlockRange(Msb, BlockPasswordFrom, BlockPasswordTo);
            if (pass != null) msg += "\nPassword: " + Util.ToASCII(pass, 0, 32);

            MessageBox.Show(msg, "Informasi Kartu", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnHapusKartu_OnClick(object sender, RoutedEventArgs e)
        {
            ClearAllBlock();
        }
    }
}