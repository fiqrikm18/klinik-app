using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using PCSC;
using pendaftaran.views;
using pendaftaran.Mifare;

namespace pendaftaran
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //static MySqlConnection MsqlConn = null;
        SmartCardOperation sp;

        public MainWindow()
        {
            InitializeComponent();
            sp = new SmartCardOperation();

            var userPrefs = new UserPreferences();

            this.Height = userPrefs.WindowHeight;
            this.Width = userPrefs.WindowWidth;
            this.Top = userPrefs.WindowTop;
            this.Left = userPrefs.WindowLeft;
            this.WindowState = userPrefs.WindowState;

            if (sp.IsReaderAvailable()) { }
            else
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var userPrefs = new UserPreferences();

            userPrefs.WindowHeight = this.Height;
            userPrefs.WindowWidth = this.Width;
            userPrefs.WindowTop = this.Top;
            userPrefs.WindowLeft = this.Left;
            userPrefs.WindowState = this.WindowState;

            userPrefs.Save();
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