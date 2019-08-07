using Antrian_3_poli.DBAaccess;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Net;
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

namespace Antrian_3_poli
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection conn;
        DBCommand cmd;

        SimpleTcpServer serverPoli;

        public int ServerPoli_ClientConnected { get; }
        public int ServerPoli_ClientDisconnected { get; }

        public MainWindow()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            serverPoli = new SimpleTcpServer();
            serverPoli.DataReceived += ServerPoli_DataReceived;
            serverPoli.ClientDisconnected += ServerPoli_ClientDisconnected1;
            serverPoli.ClientConnected += ServerPoli_ClientConnected1;
            Loaded += MainWindow_Loaded;

            LoadDataAntrian();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            serverPoli.Start(IPAddress.Parse("0.0.0.0"), 13000);
        }

        private void ServerPoli_ClientConnected1(object sender, System.Net.Sockets.TcpClient e)
        {
            Debug.WriteLine(e.Client);
        }

        private void ServerPoli_ClientDisconnected1(object sender, System.Net.Sockets.TcpClient e)
        {
            //throw new NotImplementedException();
        }

        private void ServerPoli_DataReceived(object sender, Message e)
        {
            Dispatcher.Invoke(() =>
            {
                var a = e.MessageString.Replace("\u0013", "");
                Debug.WriteLine(a);
                if (a == "Update")
                {
                    LoadDataAntrian();
                }
            });
        }

        public void LoadDataAntrian()
        {
            var periksaUmum = cmd.GetNoAntriPeriksa("001");
            var periksaGigi = cmd.GetNoAntriPeriksa("002");
            var periksaObgyn = cmd.GetNoAntriPeriksa("003");

            txtNoAntri.Content = periksaUmum;
            txtNoAntriBidan.Content = periksaObgyn;
            txtNoAntriPoliGigi.Content = periksaGigi;

            SoundPlayer snd = new SoundPlayer(Properties.Resources.snd_call);
            snd.Play();
        }
    }
}

