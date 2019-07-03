using admin.Mifare;
using admin.views;
using System;
using System.Collections.Generic;
using System.Windows;

namespace admin
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SmartCardOperation sp;

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
            Environment.Exit(0);
        }
    }
}