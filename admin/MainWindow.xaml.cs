using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using admin.Mifare;
using admin.Properties;
using admin.views;

namespace admin
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string role = Settings.Default.role;
        private readonly SmartCardOperation sp;

        public MainWindow()
        {
            InitializeComponent();
            sp = new SmartCardOperation();

            var userPrefs = new UserPreferences();

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;
            CheckRole();
        }

        private void CheckRole()
        {
            if (role == "admin")
                lblJudul.Content = "Admin";
            else if (role == "keuangan")
                lblJudul.Content = "Keuangan";
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

            userPrefs.Save();
            Settings.Default.role = "";
        }

        private bool NoReaderAvailable(ICollection<string> readerNames)
        {
            return readerNames == null || readerNames.Count < 1;
        }

        private void BtnDaftarDokter_OnClick(object sender, RoutedEventArgs e)
        {
            if (role != "keuangan")
                MainFrame.Content = new DaftarDokter();
            else
                MessageBox.Show("Anda tidak memiliki akses.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnStaffPendaftaran_OnClick(object sender, RoutedEventArgs e)
        {
            if (role != "keuangan")
                MainFrame.Content = new DaftarPendaftaran();
            else
                MessageBox.Show("Anda tidak memiliki akses.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnDaftarApoteker_OnClick(object sender, RoutedEventArgs e)
        {
            if (role != "keuangan")
                MainFrame.Content = new DaftarApoteker();
            else
                MessageBox.Show("Anda tidak memiliki akses.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnDaftarPoliklinik_OnClick(object sender, RoutedEventArgs e)
        {
            if (role != "keuangan")
                MainFrame.Content = new DaftarPoliklinik();
            else
                MessageBox.Show("Anda tidak memiliki akses.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Settings.Default.role = null;
                var lg = new Login();
                lg.Show();
                Close();
                GC.SuppressFinalize(this);
            });
        }

        private void BtnDataTransaksi_Click(object sender, RoutedEventArgs e)
        {
            if (role != "admin")
                MainFrame.Content = new LaporanTransaksiApotik();
            else
                MessageBox.Show("Anda tidak memiliki akses.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnDaftarKeuangan_Click(object sender, RoutedEventArgs e)
        {
            if (role != "keuangan")
                MainFrame.Content = new DaftarKeuangan();
            else
                MessageBox.Show("Anda tidak memiliki akses.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}