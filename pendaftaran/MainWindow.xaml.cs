using pendaftaran.Mifare;
using pendaftaran.views;
using System;
using System.Collections.Generic;
using System.Windows;

namespace pendaftaran
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //static MySqlConnection MsqlConn = null;
        private SmartCardOperation sp;

        //TODO: buat socket client antrian poli
        public MainWindow()
        {
            InitializeComponent();
            sp = new SmartCardOperation();

            //MessageBox.Show(Properties.Settings.Default.IDStaff);
            //MessageBox.Show(Application.Current.Windows.Count.ToString());

            UserPreferences userPrefs = new UserPreferences();

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;

            if (sp.IsReaderAvailable()) { }
            else
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UserPreferences userPrefs = new UserPreferences
            {
                WindowHeight = Height,
                WindowWidth = Width,
                WindowTop = Top,
                WindowLeft = Left,
                WindowState = WindowState
            };
            Properties.Settings.Default.IDStaff = null;

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
            Properties.Settings.Default.IDStaff = null;
            Environment.Exit(0);

            //Dispatcher.Invoke(() =>
            //{
            //    Properties.Settings.Default.IDStaff = null;
            //    Login lg = new Login();
            //    lg.Show();
            //    Close();
            //});
        }
    }
}