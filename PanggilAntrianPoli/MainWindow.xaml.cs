using PanggilAntrianPoli.DBAccess;
using System.Data.SqlClient;
using System.Windows;

namespace PanggilAntrianPoli
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection conn;
        DBCommand cmd;

        public MainWindow()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            tbjudul.Text = "Antrian Poli " + Properties.Settings.Default.Poliklinik;
            LoadDataAntrian();

            //MessageBox.Show(cmd.GetLastNoUrut().ToString());

            var userPrefs = new UserPreferences();

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void LoadDataAntrian()
        {
            txtNoAntri.Content = cmd.GetNoAntriPeriksa();
            txtTotalAntri.Content = "Total Pasien Antri: " + cmd.GetTotalPasien();
            var antrian = cmd.GetAntrianPoli();
            dtgAntrian.ItemsSource = antrian;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            var last = int.Parse(txtNoAntri.Content.ToString());
            
            if(cmd.UpdateStatusAntrian(last+1))
            {
                LoadDataAntrian();
                //MessageBox.Show(last.ToString());
            }
        }

        private void btnPanggil_Click(object sender, RoutedEventArgs e)
        {
            var last = cmd.GetLastNoUrut();

            if(cmd.UpdateStatusAntrian(last))
            {
                LoadDataAntrian();
            }

            btnPanggil.IsEnabled = false;
        }
    }
}