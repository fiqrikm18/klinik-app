using dokter.DBAccess;
using System;
using System.Windows;

namespace dokter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    //TODO: buat socket client antrian poli & apotik
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string role = Properties.Settings.Default.Role;
            lblHeader.Content = "Poli " + role;

            DBCommand cmd = new DBCommand(DBConnection.dbConnection());

            UserPreferences userPrefs = new UserPreferences();

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;
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

        private void btnAntrianPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.DaftarAntrian();
        }

        private void btnDataPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.DataPsien();
        }

        private void BtnPeriksaPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.ViewRekamMedis();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.KodeDokter = null;
            Environment.Exit(0);
        }
    }
}
