using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using PCSC;
using PCSC.Iso7816;
using pendaftaran.DBAccess;
using pendaftaran.Mifare;
using pendaftaran.models;
using pendaftaran.Utils;

namespace pendaftaran.views
{
    /// <summary>
    ///     Interaction logic for daftar_berobat.xaml
    /// </summary>
    public partial class daftar_berobat : Page
    {
        private const byte Msb = 0x00;
        private readonly byte blockNoRekamMedis = 13;
        private SqlCommand cmd;

        private readonly byte[] key = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        private MifareCard card;

        private readonly IContextFactory contextFactory = ContextFactory.Instance;
        private IsoReader isoReader;
        private readonly string nfcReader;

        #region constructor

        public daftar_berobat()
        {
            InitializeComponent();
            var cbp = new List<ComboboxPairs>();

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

            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                {
                    DBConnection.dbConnection().Open();
                    var command = new SqlCommand("select * from tb_poliklinik", DBConnection.dbConnection());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            cbp.Add(new ComboboxPairs(reader["nama_poli"].ToString(),
                                reader["kode_poli"].ToString()));
                    }

                    DBConnection.dbConnection().Close();
                }
                else
                {
                    var command = new SqlCommand("select * from tb_poliklinik", DBConnection.dbConnection());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            cbp.Add(new ComboboxPairs(reader["nama_poli"].ToString(),
                                reader["kode_poli"].ToString()));
                    }

                    DBConnection.dbConnection().Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message,
                    "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            cbPoliklinik.DisplayMemberPath = "kode_poliklinik";
            cbPoliklinik.SelectedValuePath = "nama_poliklinik";
            cbPoliklinik.ItemsSource = cbp;
            cbPoliklinik.SelectedIndex = 0;
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

        #region member CRUD operations

        private void tambah_antrian(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("as");
            var cbp = (ComboboxPairs) cbPoliklinik.SelectedItem;
            var policode = cbp.nama_poliklinik;
            var poliklinik = policode;
            var norm = txtIdPasien.Text;
            //string norm = "444";

            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();

                var query = "select count(*) from tb_pasien where no_rekam_medis = '" + norm + "';";
                cmd = new SqlCommand(query, DBConnection.dbConnection());
                var rm_exist = int.Parse(cmd.ExecuteScalar().ToString());
                DBConnection.dbConnection().Close();

                if (rm_exist >= 1)
                {
                    if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                        DBConnection.dbConnection().Open();

                    var last = "";
                    var a = 0;
                    //int.TryParse(last, out a);
                    var no_urut = 0;
                    //var query_last = "select nomor_urut from antrian where poliklinik= '" + policode +
                    //                 "' and DATE(tanggal_berobat) = '" + DateTime.Now.ToString("yyyy-MM-dd") +
                    //                 "' ORDER BY nomor_urut desc LIMIT 1;";
                    //string query_last = "select nomor_urut from antrian where poliklinik= '003' and DATE(tanggal_berobat) = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' ORDER BY nomor_urut desc LIMIT 1;";
                    var query_last = "select top 1 no_urut from tb_antrian_poli where poliklinik = '" + policode +
                                     "' and tgl_berobat = convert(varchar(10), getdate(), 111) order by 1 desc";
                    cmd = new SqlCommand(query_last, DBConnection.dbConnection());
                    var reader = cmd.ExecuteReader();

                    if (reader.Read()) a = reader.GetInt32(0);

                    if (last != null || last != "") no_urut = a + 1;
                    else no_urut = 1;

                    DBConnection.dbConnection().Close();
                    DBConnection.dbConnection().Open();
                    query = "insert into tb_antrian_poli(no_rm, no_urut, poliklinik, status) values('" + norm + "','" +
                            no_urut + "','" + policode + "','Antri');";
                    cmd = new SqlCommand(query, DBConnection.dbConnection());

                    var res = cmd.ExecuteNonQuery();

                    if (res == 1)
                        MessageBox.Show("Pasien berhasil ditambahkan. \nNomor antri: " + no_urut, "Informasi",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    //MessageBox.Show(last);
                    else
                        MessageBox.Show("Data pasien berhasil ditambahkan. \nGagal menambahakan pasien ke antrian.",
                            "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Rekam medis pasien belum teraftar, periksa kemabali data pasien.", "Perhatian",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...\n" + ex.Message,
                    "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
                isoReaderInit();
                card = new MifareCard(isoReader);

                var readData = ReadBlock(Msb, blockNoRekamMedis);
                if (readData != null) txtIdPasien.Text = Util.ToASCII(readData, 0, 16, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                isoReaderInit();
            }
        }

        #endregion
    }
}