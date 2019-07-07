using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using Antrian.DBAccess;
using Antrian.Properties;
using Antrian.SckServer;
using System.Text;

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
        Listener listener;

        public MainWindow()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            //Debug.WriteLine($"Kode Poli: {cmd.GetKodePoli()}");

            if (jenis_antrian == "Poliklinik")
            {
                //MessageBox.Show(jenis_antrian);
                Title = "Antrian Poli " + poliklinik;
                lbNamaPoli.Text = "Poli " + poliklinik;
                tbjudul.Text += "Poli " + poliklinik;
            }

            listener = new Listener(13000);
            listener.SocketAccepted += Listener_SocketAccepted;
            Loaded += MainWindow_Loaded;

            LoadPeriksa();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            listener.Start();
        }

        private void Listener_SocketAccepted(System.Net.Sockets.Socket e)
        {
            Client client = new Client(e);
            client.Received += Client_Received;
            client.Disconnected += Client_Disconnected;
        }

        private void Client_Disconnected(Client sender)
        {
            
        }

        private void Client_Received(Client sender, byte[] data)
        {
            Dispatcher.Invoke(()=>
            {
                if(Encoding.ASCII.GetString(data) == "Update")
                {
                    LoadPeriksa();
                }
            });
        }

        public void LoadPeriksa()
        {
            txtNoAntri.Content = cmd.GetNoAntriPeriksa().ToString();
            txtTotalAntri.Content = "Total Pasien Antri: " + cmd.GetTotalPasien().ToString();
            DisPlayDataGridAntrian();
        }

        private void DisPlayDataGridAntrian()
        {
            var antrian = cmd.GetAntrianPoli();
            dtgAntrian.ItemsSource = antrian;
        }
    }
}