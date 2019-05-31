using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PCSC;
using PCSC.Iso7816;
using pendaftaran.DBAccess;
using pendaftaran.forms;
using pendaftaran.Mifare;
using pendaftaran.Utils;

namespace pendaftaran.views
{
    /// <summary>
    ///     Interaction logic for daftar_ulang.xaml
    /// </summary>
    public partial class daftar_ulang : Page
    {
        private const byte Msb = 0x00;
        private readonly byte blockAlamatForm = 18;
        private readonly byte blockAlamatTo = 22;

        private readonly byte blockIdPasien = 12;
        private readonly byte blockJenisKelamin = 26;
        private readonly byte blockNamaFrom = 14;
        private readonly byte blockNamaTo = 17;
        private readonly byte blockNoRekamMedis = 13;
        private readonly byte blockNoTelp = 24;
        private readonly byte blockTglLahir = 25;

        private readonly byte[] key = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        private MifareCard card;

        private readonly IContextFactory contextFactory = ContextFactory.Instance;
        private IsoReader isoReader;
        private readonly string nfcReader;

        public string alamat;
        public string jenisK;
        public string namaP;
        public string noidP;
        public string normP;
        public string noTelp;
        public string tglLahir;

        public daftar_ulang()
        {
            InitializeComponent();
            displayDataPasien();

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
        }

        public void displayDataPasien(string nama = null)
        {
            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();

                string query;

                if (nama != null)
                {
                    query = "select * from tb_pasien where nama like '%" + nama + "%';";
                    var cmd = new SqlCommand(query, DBConnection.dbConnection());
                    var adapter = new SqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataPasien.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
                else
                {
                    query = "select * from tb_pasien";
                    var cmd = new SqlCommand(query, DBConnection.dbConnection());
                    var adapter = new SqlDataAdapter(cmd);
                    var dt = new DataTable();

                    adapter.Fill(dt);
                    dtgDataPasien.ItemsSource = dt.DefaultView;

                    DBConnection.dbConnection().Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message,
                    "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void UbahDataPasien(object sender, RoutedEventArgs e)
        {
            //(dtgDataPasien.SelectedCells[0].Column.GetCellContent(this.dtgDataPasien.SelectedItems[i]) as TextBlock).Text
            if (dtgDataPasien.SelectedItems.Count > 0)
            {
                for (var i = 0; i < dtgDataPasien.SelectedItems.Count; i++)
                {
                    normP =
                        (dtgDataPasien.SelectedCells[1].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    noidP =
                        (dtgDataPasien.SelectedCells[0].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    namaP =
                        (dtgDataPasien.SelectedCells[2].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    jenisK =
                        (dtgDataPasien.SelectedCells[3].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    noTelp =
                        (dtgDataPasien.SelectedCells[4].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    alamat =
                        (dtgDataPasien.SelectedCells[5].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    tglLahir =
                        (dtgDataPasien.SelectedCells[6].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                }

                var ud = new ubah_dataPasien(normP, noidP, namaP, jenisK, noTelp, alamat, this);
                ud.Show();
            }
            else
            {
                MessageBox.Show("Pilih data yang ingin diubah", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TambahPasien(object sender, RoutedEventArgs e)
        {
            var db = new daftar_baru();
            NavigationService.Navigate(db);
        }

        private void HapusDataPasien(object sender, RoutedEventArgs e)
        {
            if (dtgDataPasien.SelectedCells.Count > 0)
            {
                //object item = dtgDataPasien.SelectedItem;
                //string id = (dtgDataPasien.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text.ToString();
                //MessageBox.Show(id);

                var a = MessageBox.Show("Anda yakin ingin menghapus data pasien?", "Konfirmasi", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (a == MessageBoxResult.Yes)
                {
                    string query;
                    DBConnection.dbConnection().Open();
                    var res = 1;

                    try
                    {
                        for (var i = 0; i < dtgDataPasien.SelectedItems.Count; i++)
                        {
                            //MessageBox.Show((this.dtgDataPasien.SelectedCells[0].Column.GetCellContent(this.dtgDataPasien.SelectedItems[i]) as TextBlock).Text);
                            query = "delete from tb_pasien where no_rekam_medis = '" +
                                    (dtgDataPasien.SelectedCells[1].Column
                                        .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)?.Text + "';";
                            var command = new SqlCommand(query, DBConnection.dbConnection());
                            res = command.ExecuteNonQuery();
                        }

                        if (res == 1)
                            MessageBox.Show("Data pasien berhasil dihapus.", "Informasi", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        else
                            MessageBox.Show("Data pasien gagal dihapus.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    }
                    catch (SqlException)
                    {
                        MessageBox.Show("Data pasien gagal dihapus.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }

                displayDataPasien();
                DBConnection.dbConnection().Close();
            }
            else
            {
                MessageBox.Show("Pilih data pasien yang akan dihapus.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void TxtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nama = sender as TextBox;

            if (nama.Text != "Nama Pasien")
                displayDataPasien(nama.Text);
        }

        private void Btn_cetak_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                isoReaderInit();
                card = new MifareCard(isoReader);

                var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
                ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                Thread.CurrentThread.CurrentCulture = ci;

                if (dtgDataPasien.SelectedItems.Count > 0)
                {
                    for (var i = 0; i < dtgDataPasien.SelectedItems.Count; i++)
                    {
                        normP =
                            (dtgDataPasien.SelectedCells[1].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        noidP =
                            (dtgDataPasien.SelectedCells[0].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        namaP =
                            (dtgDataPasien.SelectedCells[2].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        jenisK =
                            (dtgDataPasien.SelectedCells[3].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        noTelp =
                            (dtgDataPasien.SelectedCells[4].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        alamat =
                            (dtgDataPasien.SelectedCells[5].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        tglLahir =
                            (dtgDataPasien.SelectedCells[6].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                    }

                    if (!string.IsNullOrEmpty(noidP))
                    {
                        if (WriteBlock(Msb, blockIdPasien, Util.ToArrayByte16(noidP)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("ID pasien gagal ditulis");
                        }
                    }

                    if (!string.IsNullOrEmpty(normP))
                    {
                        if (WriteBlock(Msb, blockNoRekamMedis, Util.ToArrayByte16(normP)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Nomor rekam medis gagal ditulis");
                        }
                    }

                    if (namaP.Length > 48)
                        namaP = namaP.Substring(0, 47);

                    if (!string.IsNullOrEmpty(namaP))
                    {
                        if (WriteBlockRange(Msb, blockNamaFrom, blockNamaTo,
                            Util.ToArrayByte48(namaP)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Nama pasien gagal ditulis");
                        }
                    }

                    if (!string.IsNullOrEmpty(tglLahir))
                    {
                        if (WriteBlock(Msb, blockTglLahir, Util.ToArrayByte16(tglLahir)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Tanggal lahir pasien gagal ditulis");
                        }
                    }

                    if (!string.IsNullOrEmpty(jenisK))
                    {
                        if (WriteBlock(Msb, blockJenisKelamin,
                            Util.ToArrayByte16(jenisK)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Jenis kelamin pasien gagal ditulis");
                        }
                    }

                    if (!string.IsNullOrEmpty(noTelp))
                    {
                        if (WriteBlock(Msb, blockNoTelp, Util.ToArrayByte16(noTelp)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Nomor telepon pasien gagal ditulis");
                        }
                    }

                    if (alamat.Length > 64)
                        alamat = alamat.Substring(0, 63);

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

                    MessageBox.Show("Data pasien berhasil ditulis", "Informasi", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Pilih data yang ingin dicetak", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isoReaderInit();
            }
        }

        private void Btn_cekData_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                isoReaderInit();
                card = new MifareCard(isoReader);

                var msg = "";
                var rm = ReadBlock(Msb, blockNoRekamMedis);
                if (rm != null)
                    msg += "Nomor Rekam Medis \t: " + Util.ToASCII(rm, 0, 16, false);

                var nId = ReadBlock(Msb, blockIdPasien);
                if (rm != null)
                    msg += "\nNomor ID Pasien \t\t: " + Util.ToASCII(nId, 0, 16, false);

                var namaP = ReadBlockRange(Msb, blockNamaFrom, blockNamaTo);
                if (namaP != null)
                    msg += "\nNama Pasien \t\t: " + Util.ToASCII(namaP, 0, 48, false);

                var nTelp = ReadBlock(Msb, blockNoTelp);
                if (nTelp != null)
                    msg += "\nNomor Telepon Pasien \t: " + Util.ToASCII(nTelp, 0, 16, false);

                var alamatP = ReadBlockRange(Msb, blockAlamatForm, blockAlamatTo);
                if (alamatP != null)
                    msg += "\nAlamat Pasien \t\t: " + Util.ToASCII(alamatP, 0, 64, false);

                var tglHarie = ReadBlock(Msb, blockTglLahir);
                if (tglHarie != null)
                    msg += "\nTanggal Lahir \t\t: " + Util.ToASCII(tglHarie, 0, 16, false);

                var jk = ReadBlock(Msb, blockJenisKelamin);
                if (jk != null)
                    msg += "\nJenis Kelamin \t\t: " + Util.ToASCII(jk, 0, 16, false);

                MessageBox.Show(msg, "Informasi Kartu Pasien", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isoReaderInit();
            }
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