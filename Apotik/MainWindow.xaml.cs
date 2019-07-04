using System;
using System.ComponentModel;
using System.Windows;
using Apotik.views;

namespace Apotik
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var userPrefs = new UserPreferences();

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var userPrefs = new UserPreferences();

            userPrefs.WindowHeight = Height;
            userPrefs.WindowWidth = Width;
            userPrefs.WindowTop = Top;
            userPrefs.WindowLeft = Left;
            userPrefs.WindowState = WindowState;

            userPrefs.Save();
        }

        private void TambahObat(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new TambahObat();
        }

        private void DaftarObat(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarObat();
        }

        private void DaftarResep(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarResep();
        }

        private void BuatResep(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new BuatResep();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //Properties.Settings.Default.IDStaff = null;
                var lg = new Login();
                lg.Show();
                Close();
                GC.SuppressFinalize(this);
            });
        }
    }
}