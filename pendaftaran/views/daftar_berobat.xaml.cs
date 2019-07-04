using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
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
        private readonly byte blockNoRekamMedis = 1;

        private readonly SqlConnection conn;

        private readonly SmartCardOperation sp;

        #region constructor

        public daftar_berobat()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            var cmd = new DBCommand(conn);
            sp = new SmartCardOperation();

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

                    if (cmd.InsertAntrian(norm, no_urut, policode))
                    {
                        MessageBox.Show("Pasien berhasil didaftarkan.\nNomor Antri: " + no_urut, "Informasi",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        txtIdPasien.Text = "";
                        cbPoliklinik.SelectedIndex = 0;
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