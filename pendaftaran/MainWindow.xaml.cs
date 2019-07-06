using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using pendaftaran.Mifare;
using pendaftaran.Properties;
using pendaftaran.views;
using System.Net.Sockets;

namespace pendaftaran
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //static MySqlConnection MsqlConn = null;
        private readonly SmartCardOperation sp;
        Socket sck;

        public MainWindow()
        {
            InitializeComponent();
            sp = new SmartCardOperation();

            //MessageBox.Show(Properties.Settings.Default.IDStaff);
            //MessageBox.Show(Application.Current.Windows.Count.ToString());

            var userPrefs = new UserPreferences();

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;

            if (sp.IsReaderAvailable())
            {
            }
            else
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sck.Connect("192.168.1.105", 13000);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Apliasi antrian tidak aktif, pastikan aplikasi antrian aktif.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var userPrefs = new UserPreferences
            {
                WindowHeight = Height,
                WindowWidth = Width,
                WindowTop = Top,
                WindowLeft = Left,
                WindowState = WindowState
            };
            Settings.Default.IDStaff = null;

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

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Settings.Default.IDStaff = null;
                var lg = new Login();
                lg.Show();
                Close();
                GC.SuppressFinalize(this);
            });
        }
    }
}