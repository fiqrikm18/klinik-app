using System;
using System.ComponentModel;
using System.Windows;
using dokter.DBAccess;
using dokter.Properties;
using dokter.views;

namespace dokter
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    // TODO: buat socket client antrian poli & apotik
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var role = Settings.Default.Role;
            lblHeader.Content = "Poli " + role;

            var cmd = new DBCommand(DBConnection.dbConnection());

            var userPrefs = new UserPreferences();

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;
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
        }

        private void btnAntrianPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarAntrian();
        }

        private void btnDataPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DataPsien();
        }

        private void BtnPeriksaPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ViewRekamMedis();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Settings.Default.KodeDokter = null;
                var lg = new Login();
                lg.Show();
                Close();
                GC.SuppressFinalize(this);
            });
        }
    }
}