using Antrian_Apotik.DBAccess;
using Antrian_Apotik.SckServer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
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

namespace Antrian_Apotik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        Listener listener;
        SqlConnection conn;
        DBCommand cmd;

        public MainWindow()
        {
            InitializeComponent();

            listener = new Listener(16000);
            listener.SocketAccepted += Listener_SocketAccepted;
            Loaded += MainWindow_Loaded;

            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            LoadData();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            listener.Start();
        }

        private void Listener_SocketAccepted(Socket e)
        {
            Client client = new Client(e);
            client.Received += Client_Received;
            client.Disconnected += Client_Disconnected;
        }

        private void Client_Disconnected(Client sender)
        {
            //throw new NotImplementedException();
        }

        private void Client_Received(Client sender, byte[] data)
        {
            Dispatcher.Invoke(()=>
            {
                //MessageBox.Show(data.ToString());
                Debug.WriteLine(Encoding.ASCII.GetString(data));
                if(Encoding.ASCII.GetString(data) == "Update")
                {
                    LoadData();
                }
            });
        }

        private void LoadData()
        {
            var antri = cmd.GetDataAntrian();

            txtNoAntri.Content = cmd.GetNoAntriPeriksa();
            txtTotalAntri.Content = "Total Antri: " + cmd.GetTotalPasien();
            dtgAntrian.ItemsSource = antri;
        }
    }
}
