using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Apotik.DBAccess;
using Apotik.SckServer;
using System.Text;
using System.Net.Sockets;
using System;

namespace Apotik.views
{
    /// <summary>
    ///     Interaction logic for DaftarResep.xaml
    /// </summary>
    public partial class DaftarResep : Page
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        Listener listener;

        public DaftarResep()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            listener = new Listener(15000);
            listener.SocketAccepted += Listener_SocketAccepted;
            Loaded += DaftarResep_Loaded;

            DisplayDataAntrianApotek();
        }

        private void DaftarResep_Loaded(object sender, RoutedEventArgs e)
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
                    DisplayDataAntrianApotek();
                }
            });
        }

        public void DisplayDataAntrianApotek()
        {
            var antrian = cmd.GetDataAntrianApotik();
            dtgResep.ItemsSource = antrian;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var kode_resep = "";

            if (dtgResep.SelectedItems.Count > 0)
            {
                for (var i = 0; i < dtgResep.SelectedItems.Count; i++)
                    kode_resep = (dtgResep.SelectedCells[2].Column
                            .GetCellContent(dtgResep.SelectedItems[i]) as TextBlock)
                        .Text;

                Debug.WriteLine(kode_resep);

                var br = new BuatResep(kode_resep);
                NavigationService.Navigate(br);
            }
        }
    }
}