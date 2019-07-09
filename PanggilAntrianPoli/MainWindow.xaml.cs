using PanggilAntrianPoli.DBAccess;
using PanggilAntrianPoli.SckServer;
using System;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Text;
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
        Listener listener;
        Socket sck;

        public MainWindow()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            tbjudul.Text = "Antrian Poli " + Properties.Settings.Default.Poliklinik;
            lbNamaPoli.Text = "Poli " + Properties.Settings.Default.Poliklinik;
            LoadDataAntrian();

            try
            {
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sck.Connect(Properties.Settings.Default.SocketServer, Properties.Settings.Default.SocketPort);
            }
            catch (Exception)
            {
                //Do nothing
            }

            //MessageBox.Show(cmd.GetLastNoUrut().ToString());

            var userPrefs = new UserPreferences();
            listener = new Listener(14000);
            listener.SocketAccepted += Listener_SocketAccepted;
            Loaded += MainWindow_Loaded;

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;
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
            Dispatcher.Invoke(() =>
            {
                if (Encoding.ASCII.GetString(data) == "Update")
                {
                    LoadDataAntrian();
                }
            });
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

            if (cmd.UpdateStatusAntrian(last + 1))
            {
                LoadDataAntrian();
                //MessageBox.Show(last.ToString());

                try
                {
                    sck.Send(Encoding.ASCII.GetBytes("Update"));
                }
                catch(Exception)
                {
                    //Do nothing
                }
            }
        }

        private void btnPanggil_Click(object sender, RoutedEventArgs e)
        {
            var last = cmd.GetLastNoUrut();

            if (cmd.UpdateStatusAntrian(last))
            {
                LoadDataAntrian();
                try
                {
                    sck.Send(Encoding.ASCII.GetBytes("Update"));
                }
                catch(Exception)
                {
                    //Do nothing
                }
            }

            btnPanggil.IsEnabled = false;
        }
    }
}