using System;
using System.Data.SqlClient;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using Antrian.DBAccess;
using Antrian.Properties;
using SimpleTCP;

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
        private readonly SimpleTcpServer serverApotik;

        //private Listener listenerPoli;
        //private Listener listenerApotik;
        private readonly SimpleTcpServer serverPoli;

        public MainWindow()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            //Debug.WriteLine($"Kode Poli: {cmd.GetKodePoli()}");
            //Debug.WriteLine(jenis_antrian);

            if (jenis_antrian == "Poliklinik")
            {
                //MessageBox.Show(jenis_antrian);
                lbTitleNo_Antri.Content = "No. Urut sedang diperiksa";
                Title = "Antrian Poli " + poliklinik;
                lbNamaPoli.Text = "Poli " + poliklinik;
                tbjudul.Text += "Poli " + poliklinik;

                //listenerPoli = new Listener(13000);
                //listenerPoli.SocketAccepted += Listener_SocketAccepted;
                //Loaded += MainWindow_Loaded;
                serverPoli = new SimpleTcpServer();
                serverPoli.DataReceived += ServerPoli_DataReceived;
                serverPoli.ClientDisconnected += ServerPoli_ClientDisconnected;
                serverPoli.ClientConnected += ServerPoli_ClientConnected;
                Loaded += MainWindow_Loaded;
            }
            else if (jenis_antrian == "Apotik")
            {
                //MessageBox.Show(jenis_antrian);
                lbTitleNo_Antri.Content = "No. Resep sedang buat";
                Title = "Antrian Apotik";
                lbNamaPoli.Text = "Klinik Bunda Mulya";
                tbjudul.Text += "Apotik";

                //listenerApotik = new Listener(14000);
                //listenerApotik.SocketAccepted += ListenerApotik_SocketAccepted;
                //Loaded += MainWindow_Loaded;
                serverApotik = new SimpleTcpServer();
                serverApotik.DataReceived += ServerApotik_DataReceived;
                serverApotik.ClientDisconnected += ServerApotik_ClientDisconnected;
                serverApotik.ClientConnected += ServerApotik_ClientConnected;
                Loaded += MainWindow_Loaded;
            }

            //Loaded += MainWindow_Loaded;
            LoadPeriksa();
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //throw new System.NotImplementedException();
            Settings.Default.IsRemoteConnected = false;
        }

        private void ServerApotik_ClientConnected(object sender, TcpClient e)
        {
            //Debug.WriteLine("Connected {0}", e.Client);
        }

        private void ServerPoli_ClientConnected(object sender, TcpClient e)
        {
            //throw new System.NotImplementedException();
            //Debug.WriteLine("Connected {0}", e.Client);
        }

        private void ServerApotik_ClientDisconnected(object sender, TcpClient e)
        {
            //throw new System.NotImplementedException();
        }

        private void ServerApotik_DataReceived(object sender, Message e)
        {
            //throw new System.NotImplementedException();
            Dispatcher.Invoke(() =>
            {
                //Debug.WriteLine(e.MessageString);
                //e.ReplyLine(e.MessageString);

                var a = e.MessageString.Replace("\u0013", "");
                //Debug.WriteLine(a);
                if (!Settings.Default.IsRemoteConnected)
                {
                    if (int.Parse(a) == 4)
                    {
                        Settings.Default.IsRemoteConnected = true;
                        if (Settings.Default.IsRemoteConnected)
                        {
                            //Debug.WriteLine(Settings.Default.IsRemoteConnected);
                            e.ReplyLine("Connected");
                            LoadPeriksa();
                        }
                    }
                }
                else
                {
                    var v = 0;
                    if (int.TryParse(a, out v) && v == 0)
                    {
                        Settings.Default.IsRemoteConnected = false;
                        if (!Settings.Default.IsRemoteConnected)
                        {
                            //Debug.WriteLine(Settings.Default.IsRemoteConnected);
                            e.ReplyLine("Disconnected");
                            LoadPeriksa();
                        }
                    }

                    //Debug.WriteLine(a);
                    if (a == "Update") LoadPeriksa();
                }
            });
        }

        private void ServerPoli_ClientDisconnected(object sender, TcpClient e)
        {
            //throw new System.NotImplementedException();
        }

        private void ServerPoli_DataReceived(object sender, Message e)
        {
            //throw new System.NotImplementedException();
            Dispatcher.Invoke(() =>
            {
                //Debug.WriteLine(e.MessageString);
                //e.ReplyLine(e.MessageString);

                var a = e.MessageString.Replace("\u0013", "");
                //Debug.WriteLine(a);
                if (!Settings.Default.IsRemoteConnected)
                {
                    if (int.Parse(a) == int.Parse(cmd.GetKodePoli()))
                    {
                        Settings.Default.IsRemoteConnected = true;
                        e.ReplyLine("Connected");
                        //Debug.WriteLine(Settings.Default.IsRemoteConnected);
                        LoadPeriksa();
                    }
                }
                else
                {
                    if (int.TryParse(a, out var v) && v == 0)
                    {
                        Settings.Default.IsRemoteConnected = false;
                        if (!Settings.Default.IsRemoteConnected)
                        {
                            e.ReplyLine("Disconnected");
                            //Debug.WriteLine(Settings.Default.IsRemoteConnected);
                            LoadPeriksa();
                        }
                    }

                    if (a == "Update") LoadPeriksa();
                }
            });
        }

        //private void ListenerApotik_SocketAccepted(System.Net.Sockets.Socket e)
        //{
        //    Client client = new Client(e);
        //    client.Received += Client_Received;
        //    client.Disconnected += Client_Disconnected;

        //    Debug.WriteLine("Apotik");
        //}

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (jenis_antrian == "Apotik")
                serverApotik.Start(IPAddress.Parse("0.0.0.0"), 14000);
            else if (jenis_antrian == "Poliklinik") serverPoli.Start(IPAddress.Parse("0.0.0.0"), 13000);
        }

        //private void Listener_SocketAccepted(System.Net.Sockets.Socket e)
        //{
        //    Client client = new Client(e);
        //    client.Received += Client_Received;
        //    client.Disconnected += Client_Disconnected;

        //    Debug.WriteLine("Poliklinik");
        //}

        //private void Client_Disconnected(Client sender)
        //{

        //}

        //private void Client_Received(Client sender, byte[] data)
        //{
        //    Debug.WriteLine(Encoding.ASCII.GetString(data));
        //    Dispatcher.Invoke(() =>
        //    {

        //        if (Encoding.ASCII.GetString(data) == "Update")
        //        {
        //            LoadPeriksa();
        //            //MessageBox.Show(data.ToString());
        //        }
        //    });
        //}

        public void LoadPeriksa()
        {
            if (jenis_antrian == "Poliklinik")
            {
                txtNoAntri.Content = cmd.GetNoAntriPeriksa().ToString();
                txtTotalAntri.Content = "Total Pasien Antri: " + cmd.GetTotalPasien();
                DisPlayDataGridAntrian();
                var snd = new SoundPlayer(Properties.Resources.Two_Tone_Doorbell_SoundBible_com_1238551671);
                snd.Play();
            }
            else if (jenis_antrian == "Apotik")
            {
                txtNoAntri.Content = cmd.GetNoAntriApotik().ToString();
                txtTotalAntri.Content = "Total antrian apotik: " + cmd.GetTotalApotik();
                DisPlayDataGridAntrian();
                var snd = new SoundPlayer(Properties.Resources.Two_Tone_Doorbell_SoundBible_com_1238551671);
                snd.Play();
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