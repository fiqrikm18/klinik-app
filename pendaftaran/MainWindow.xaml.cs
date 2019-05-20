using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MySql.Data.MySqlClient;
using PCSC;

namespace pendaftaran
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //static MySqlConnection MsqlConn = null;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                if (DBAccess.DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                    DBAccess.DBConnection.dbConnection().Open();
            }
            catch (MySqlException)
            {
                MessageBox.Show("Periksa kembali koneksi database anda...", "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            var contextFactory = ContextFactory.Instance;
            var ctx = contextFactory.Establish(SCardScope.System);
            var readerNames = ctx.GetReaders();

            if (NoReaderAvailable(readerNames))
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //Environment.Exit(0);
            }
            else
            {
                var nfcReader = readerNames[0];
                if (string.IsNullOrEmpty(nfcReader))
                {
                    MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool NoReaderAvailable(ICollection<string> readerNames) 
            => readerNames == null || readerNames.Count < 1;

        private void daftar_baru(object sender, RoutedEventArgs e) => MainFrame.Content = new views.daftar_baru();
        private void daftar_pasien(object sender, RoutedEventArgs e) => MainFrame.Content = new views.daftar_ulang();
        private void daftar_berobat(object sender, RoutedEventArgs e) => MainFrame.Content = new views.daftar_berobat();
        private void daftar_antrian(object sender, RoutedEventArgs e) => MainFrame.Content = new views.antrian();
    }
}
