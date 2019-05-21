using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using pendaftaran.models;
using System.Globalization;
using System.Threading;
using PCSC;
using PCSC.Iso7816;
using pendaftaran.Mifare;
using pendaftaran.Utils;

namespace pendaftaran.views
{
    /// <summary>
    /// Interaction logic for daftar_baru.xaml
    /// </summary>
    public partial class daftar_baru : Page
    {
        //private static MySqlConnection MsqlConn = null;
        private int _noOfErrorsOnScreen = 0;
        private MDaftarBaru _mDaftarBaru = new MDaftarBaru(" ", " ", " ", " ", " ");

        private IsoReader isoReader;
        private const byte Msb = 0x00;
        private MifareCard card;

        private byte blockIdPasien = 12;
        private byte blockNoRekamMedis = 13;
        private byte blockNamaFrom = 14;
        private byte blockNamaTo = 17;
        private byte blockAlamatForm = 18;
        private byte blockAlamatTo = 22;
        private byte blockNoTelp = 24;
        private byte blockTglLahir = 25;
        private byte blockJenisKelamin = 26;

        private byte[] key = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};

        #region constructor

        public daftar_baru()
        {
            InitializeComponent();

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

            var cbp = new List<ComboboxPairs>();
            DataContext = new MDaftarBaru(" ", " ", " ", " ", " ");
            //DataContext = new MDaftarBaru("123", "123", "ad", "123", " 123123");

            try
            {
                if (DBAccess.DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Open))
                {
                    var command = new MySqlCommand("select * from poliklinik", DBAccess.DBConnection.dbConnection());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(),
                                reader["kode_poliklinik"].ToString()));
                    }
                }
                else
                {
                    DBAccess.DBConnection.dbConnection().Open();
                    var command = new MySqlCommand("select * from poliklinik", DBAccess.DBConnection.dbConnection());
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(),
                                reader["kode_poliklinik"].ToString()));
                    }
                }
            }
            catch (MySqlException ex)
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
                            continue;
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

            if (checkTextBoxValue() && dtTanggalLahir.SelectedDate != null)
            {
                var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
                ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                Thread.CurrentThread.CurrentCulture = ci;

                var cbp = (ComboboxPairs) cbPoliklinik.SelectedItem;
                var policode = cbp.nama_poliklinik;
                //DateTime dt = DateTime.ParseExact(, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                var norm = TxtNoRm.Text.ToString();
                var identitas = TxtNoIdentitas.Text.ToString();
                var namaPasien = TxtNamaPasien.Text.ToString();
                var noTelp = TxtNoTelp.Text.ToString();
                var alamat = TextAlamat.Text.ToString();
                var tglLahir = dtTanggalLahir.SelectedDate.Value.Date.ToShortDateString();
                var jenisKelamin = cbJenisKelamin.Text.ToString();
                var poliklinik = policode;

                try
                {
                    var query = "select count(*) from pasien where no_identitas = '" + identitas + "';";
                    var cmd = new MySqlCommand(query, DBAccess.DBConnection.dbConnection());
                    var idExist = int.Parse(cmd.ExecuteScalar().ToString());

                    if (idExist >= 1)
                    {
                        MessageBox.Show("No indentitas sudah terdaftar.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                    else
                    {
                        query = "select count(*) from pasien where no_rekam_medis = '" + norm + "';";
                        cmd = new MySqlCommand(query, DBAccess.DBConnection.dbConnection());
                        var rm_exist = int.Parse(cmd.ExecuteScalar().ToString());

                        if (rm_exist >= 1)
                            MessageBox.Show("No rekam medis sudah terdaftar.", "Informasi", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        else
                            try
                            {
                                query =
                                    "insert into pasien (no_identitas, no_rekam_medis, nama, tanggal_lahir, jenis_kelamin, no_telp, alamat) values('" +
                                    identitas + "', '" + norm + "', '" + namaPasien + "', '" + tglLahir + "', '" +
                                    jenisKelamin + "', '" + noTelp + "', '" + alamat + "');";

                                var command = new MySqlCommand(query, DBAccess.DBConnection.dbConnection());
                                var res = command.ExecuteNonQuery();

                                if (res == 1)
                                {
                                    var last = "";
                                    var a = 0;
                                    //int.TryParse(last, out a);
                                    var no_urut = 0;
                                    var query_last = "select nomor_urut from antrian where poliklinik= '" + policode +
                                                     "' and DATE(tanggal_berobat) = '" +
                                                     DateTime.Now.ToString("yyyy-MM-dd") +
                                                     "' ORDER BY nomor_urut desc LIMIT 1;";
                                    //string query_last = "select nomor_urut from antrian where poliklinik= '003' and DATE(tanggal_berobat) = '" + DateTime.Now.ToString("yyyy-MM-dd") + "' ORDER BY nomor_urut desc LIMIT 1;";
                                    command = new MySqlCommand(query_last, DBAccess.DBConnection.dbConnection());
                                    var reader = command.ExecuteReader();

                                    if (reader.Read()) a = reader.GetInt32(0);

                                    if (last != null || last != "") no_urut = a + 1;
                                    else no_urut = 1;

                                    DBAccess.DBConnection.dbConnection().Close();
                                    DBAccess.DBConnection.dbConnection().Open();
                                    query = "insert into antrian(nomor_rm, nomor_urut, poliklinik, status) values('" +
                                            norm + "','" + no_urut + "','" + policode + "', 'Antri');";
                                    command = new MySqlCommand(query, DBAccess.DBConnection.dbConnection());

                                    res = command.ExecuteNonQuery();

                                    if (res == 1)
                                    {
                                        if (chkCetakKartu.IsChecked == true)
                                        {
                                            if (!string.IsNullOrEmpty(identitas))
                                            {
                                                if (WriteBlock(Msb, blockIdPasien, Util.ToArrayByte16(identitas)))
                                                {
                                                }
                                                else
                                                {
                                                    MessageBox.Show("ID pasien gagal ditulis");
                                                }
                                            }

                                            if (!string.IsNullOrEmpty(norm))
                                            {
                                                if (WriteBlock(Msb, blockNoRekamMedis, Util.ToArrayByte16(norm)))
                                                {
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Nomor rekam medis gagal ditulis");
                                                }
                                            }

                                            if (namaPasien.Length > 48)
                                                namaPasien = namaPasien.Substring(0, 47);

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

                                            if (!string.IsNullOrEmpty(jenisKelamin))
                                            {
                                                if (WriteBlock(Msb, blockJenisKelamin,
                                                    Util.ToArrayByte16(jenisKelamin)))
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
                                        }

                                        MessageBox.Show("Data pasien berhasil ditambahkan. \nNomor antri: " + no_urut,
                                            "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                                        //MessageBox.Show(last);
                                    }
                                    else
                                    {
                                        MessageBox.Show(
                                            "Data pasien berhasil ditambahkan. \nGagal menambahakan pasien ke antrian.",
                                            "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Data pasien gagal ditambahkan.", "Informasi", MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                                }
                            }
                            catch (MySqlException ex)
                            {
                                MessageBox.Show("Terjadi kesalahan pada database: " + ex.Message, "Informasi",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                //MessageBox.Show(tglLahir);
            }
            else
            {
                MessageBox.Show("Periksa kembali data yang akan di inputkan.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            DataContext = _mDaftarBaru;
            dtTanggalLahir.SelectedDate = null;
            cbJenisKelamin.SelectedIndex = 0;
            cbPoliklinik.SelectedIndex = 0;
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
                !string.IsNullOrWhiteSpace(TextAlamat.Text) && !string.IsNullOrWhiteSpace(dtTanggalLahir.SelectedDate.ToString()))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region members event buttons

        private void BtnCekKartu_OnClick(object sender, RoutedEventArgs e)
        {
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

            MessageBox.Show(msg.ToString(), "Informasi Kartu Pasien", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnHapusKartu_OnClick(object sender, RoutedEventArgs e)
        {
            ClearAllBlock();
        }

        #endregion
    }
}