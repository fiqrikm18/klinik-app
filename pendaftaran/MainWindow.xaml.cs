using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using PCSC;
using pendaftaran.DBAccess;
using pendaftaran.views;

namespace pendaftaran
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //static MySqlConnection MsqlConn = null;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();
            }
            catch (SqlException)
            {
                MessageBox.Show("Periksa kembali koneksi database anda...", "Perhatian", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            var contextFactory = ContextFactory.Instance;
            var ctx = contextFactory.Establish(SCardScope.System);
            var readerNames = ctx.GetReaders();

            if (NoReaderAvailable(readerNames))
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                //Environment.Exit(0);
            }
            else
            {
                var nfcReader = readerNames[0];
                if (string.IsNullOrEmpty(nfcReader))
                    MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool NoReaderAvailable(ICollection<string> readerNames)
        {
            return readerNames == null || readerNames.Count < 1;
        }

        private void daftar_baru(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new daftar_baru();
        }

        private void daftar_pasien(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new daftar_ulang();
        }

        private void daftar_berobat(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new daftar_berobat();
        }

        private void daftar_antrian(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new antrian();
        }
    }
}