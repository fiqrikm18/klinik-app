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
        Listener listenerPoli;
        Listener listenerApotik;

        public MainWindow()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            //Debug.WriteLine($"Kode Poli: {cmd.GetKodePoli()}");
            Debug.WriteLine(jenis_antrian);

            if (jenis_antrian == "Poliklinik")
            {
                //MessageBox.Show(jenis_antrian);
                lbTitleNo_Antri.Content = "No. Urut sedang diperiksa";
                Title = "Antrian Poli " + poliklinik;
                lbNamaPoli.Text = "Poli " + poliklinik;
                tbjudul.Text += "Poli " + poliklinik;

                listenerPoli = new Listener(13000);
                listenerPoli.SocketAccepted += Listener_SocketAccepted;
                Loaded += MainWindow_Loaded;
            }
            else if (jenis_antrian == "Apotik")
            {
                //MessageBox.Show(jenis_antrian);
                lbTitleNo_Antri.Content = "No. Resep sedang buat";
                Title = "Antrian Apotik";
                lbNamaPoli.Text = "Klinik Bunda Mulya";
                tbjudul.Text += "Apotik";

                listenerApotik = new Listener(14000);
                listenerApotik.SocketAccepted += ListenerApotik_SocketAccepted;
                Loaded += MainWindow_Loaded;
            }

            Loaded += MainWindow_Loaded;

            LoadPeriksa();
        }

        private void ListenerApotik_SocketAccepted(System.Net.Sockets.Socket e)
        { 
            Client client = new Client(e);
            client.Received += Client_Received;
            client.Disconnected += Client_Disconnected;

            Debug.WriteLine("asd");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(jenis_antrian == "Apotik")
            {
                listenerApotik.Start();
            }
            else if(jenis_antrian == "Poliklinik")
            {
                listenerPoli.Start();
            }
        }

        private void Listener_SocketAccepted(System.Net.Sockets.Socket e)
        {
            Client client = new Client(e);
            client.Received += Client_Received;
            client.Disconnected += Client_Disconnected;

            Debug.WriteLine("asd");
        }

        private void Client_Disconnected(Client sender)
        {

        }

        private void Client_Received(Client sender, byte[] data)
        {
            Dispatcher.Invoke(() =>
            {
                if (Encoding.ASCII.GetString(data) == "Update")
                {
                    LoadPeriksa();
                }
            });
        }

        public void LoadPeriksa()
        {
            if (jenis_antrian == "Poliklinik")
            {
                txtNoAntri.Content = cmd.GetNoAntriPeriksa().ToString();
                txtTotalAntri.Content = "Total Pasien Antri: " + cmd.GetTotalPasien().ToString();
                DisPlayDataGridAntrian();
            }
            else if (jenis_antrian == "Apotik")
            {
                txtNoAntri.Content = cmd.GetNoAntriApotik().ToString();
                txtTotalAntri.Content = "Total antrian apotik: " + cmd.GetTotalApotik().ToString();
                DisPlayDataGridAntrian();
            }
        }

        private void DisPlayDataGridAntrian()
        {
            if (jenis_antrian == "Poliklinik")
            {
                var antrian = cmd.GetAntrianPoli();
                dtgAntrian.ItemsSource = antrian;
            }
            else if (jenis_antrian == "Apotik")
            {
                var antrian = cmd.GetAntrianApotik();
                dtgAntrian.ItemsSource = antrian;
            }
        }
    }
}