using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using admin.DBAccess;

namespace admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                if (DBAccess.DBConnection.dbConnection().State.Equals(System.Data.ConnectionState.Closed))
                    DBAccess.DBConnection.dbConnection().Open();
            }
            catch (MySqlException)
            {
                MessageBox.Show("Periksa kembali koneksi database anda...", "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDaftarDokter_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.DaftarDokter();
        }

        private void BtnStaffPendaftaran_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.DaftarPendaftaran();
        }

        private void BtnDaftarApoteker_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.DaftarApoteker();
        }

        private void BtnDaftarPoliklinik_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.DaftarPoliklinik();
        }
    }
}
