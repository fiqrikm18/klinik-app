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

namespace dokter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string role = Properties.Settings.Default.Role;
            lblHeader.Content = "Poli " + role;

            DBAccess.DBCommand cmd = new DBAccess.DBCommand(DBAccess.DBConnection.dbConnection());
            cmd.OpenConnection();
        }

        private void btnAntrianPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.DaftarAntrian();
        }

        private void btnRekamMedisPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.DaftarRekamMedis();
        }

        private void btnDataPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.DataPsien();
        }
    }
}
