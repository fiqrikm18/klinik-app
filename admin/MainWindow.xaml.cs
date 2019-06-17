using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using admin.DBAccess;
using admin.views;
using PCSC;
using PCSC.Iso7816;
using admin.Mifare;

namespace admin
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            throw new NotImplementedException();
        }
    }
}