using admin.Mifare;
using admin.views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace admin
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SmartCardOperation sp;

        public MainWindow()
        {
            InitializeComponent();
            sp = new SmartCardOperation();

            UserPreferences userPrefs = new UserPreferences();

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            UserPreferences userPrefs = new UserPreferences
            {
                WindowHeight = Height,
                WindowWidth = Width,
                WindowTop = Top,
                WindowLeft = Left,
                WindowState = WindowState
            };

            userPrefs.Save();
        }

        private bool NoReaderAvailable(ICollection<string> readerNames)
        {
            return readerNames == null || readerNames.Count < 1;
        }

        private void BtnDaftarDokter_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarDokter();
        }

        private void BtnStaffPendaftaran_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarPendaftaran();
        }

        private void BtnDaftarApoteker_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarApoteker();
        }

        private void BtnDaftarPoliklinik_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarPoliklinik();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //Properties.Settings.Default.IDStaff = null;
                Login lg = new Login();
                lg.Show();
                Close();
                GC.SuppressFinalize(this);
            });
        }

        private void BtnDataTransaksi_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new LaporanTransaksiApotik();
        }
    }
}