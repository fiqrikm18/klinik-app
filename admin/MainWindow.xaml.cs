using System.Data;
using System.Windows;
using admin.DBAccess;
using admin.views;
using MySql.Data.MySqlClient;

namespace admin
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();
            }
            catch (MySqlException)
            {
                MessageBox.Show("Periksa kembali koneksi database anda...", "Perhatian", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
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
    }
}