using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using Antrian.DBAccess;
using Antrian.Properties;

namespace Antrian
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DBCommand cmd;

        private readonly SqlConnection conn;
        private readonly string jenis_antrian = Settings.Default.antrian;
        private readonly string poliklinik = Settings.Default.poliklinik;

        //TODO: buat socket server 
        public MainWindow()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            Debug.WriteLine($"Kode Poli: {cmd.GetKodePoli()}");

            if (jenis_antrian == "Poliklinik")
            {
                //MessageBox.Show(jenis_antrian);
                Title = "Antrian Poli " + poliklinik;
                lbNamaPoli.Text = "Poli " + poliklinik;
                tbjudul.Text += "Poli " + poliklinik;
            }

            LoadPeriksa();
        }

        public void LoadPeriksa()
        {
            txtNoAntri.Content = cmd.GetNoAntriPeriksa().ToString();
            txtTotalAntri.Content += cmd.GetTotalPasien().ToString();
            DisPlayDataGridAntrian();
        }

        private void DisPlayDataGridAntrian()
        {
            var antrian = cmd.GetAntrianPoli();
            dtgAntrian.ItemsSource = antrian;
        }
    }
}