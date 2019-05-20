using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
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
        private readonly MifareCard card;
        private readonly IsoReader isoReader;
        private readonly byte[] key = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
        private MySqlCommand cmd;

        #region constructor

        public daftar_berobat()
        {
            InitializeComponent();
            var cbp = new List<ComboboxPairs>();

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

            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                {
                    DBConnection.dbConnection().Open();
                    var command = new MySqlCommand("select * from poliklinik", DBConnection.dbConnection());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(),
                                reader["kode_poliklinik"].ToString()));
                    }

                    DBConnection.dbConnection().Close();
                }
                else
                {
                    var command = new MySqlCommand("select * from poliklinik", DBConnection.dbConnection());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(),
                                reader["kode_poliklinik"].ToString()));
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

        private byte[] ReadBlock(byte msb, byte lsb)
        {
            var readBinary = new byte[16];

            if (card.LoadKey(KeyStructure.VolatileMemory, 0x00, key))
                if (card.Authenticate(msb, lsb, KeyType.KeyA, 0x00))
                    readBinary = card.ReadBinary(msb, lsb, 16);

            return readBinary;
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

                var query = "select count(*) from pasien where no_rekam_medis = '" + norm + "';";
                cmd = new MySqlCommand(query, DBConnection.dbConnection());
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
                    var query_last = "select nomor_urut from antrian where poliklinik= '" + policode +
                                     "' and DATE(tanggal_berobat) = '" + DateTime.Now.ToString("yyyy-MM-dd") +
                                     "' ORDER BY nomor_urut desc LIMIT 1;";
                    //string query_last = "select nomor_urut from antrian where poliklinik= '003' and DATE(tanggal_berobat) = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' ORDER BY nomor_urut desc LIMIT 1;";
                    cmd = new MySqlCommand(query_last, DBConnection.dbConnection());
                    var reader = cmd.ExecuteReader();

                    if (reader.Read()) a = reader.GetInt32(0);

                    if (last != null || last != "") no_urut = a + 1;
                    else no_urut = 1;

                    DBConnection.dbConnection().Close();
                    DBConnection.dbConnection().Open();
                    query = "insert into antrian(nomor_rm, nomor_urut, poliklinik, status) values('" + norm + "','" +
                            no_urut + "','" + policode + "','Antri');";
                    cmd = new MySqlCommand(query, DBConnection.dbConnection());

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
            var readData = ReadBlock(Msb, blockNoRekamMedis);
            if (readData != null) txtIdPasien.Text = Util.ToASCII(readData, 0, 16, false);
        }

        #endregion
    }
}