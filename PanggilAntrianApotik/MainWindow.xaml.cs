using PanggilAntrianApotik.DBAccess;
using PanggilAntrianApotik.SckServer;
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

namespace PanggilAntrianApotik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Listener listener;
        Socket sck;

        SqlConnection conn;
        DBCommand cmd;

        public MainWindow()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            listener = new Listener(17000);
            listener.SocketAccepted += Listener_SocketAccepted;
            Loaded += MainWindow_Loaded;

            try
            {
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sck.Connect(Properties.Settings.Default.SocketServer, Properties.Settings.Default.SocketPort);
            }
            catch(Exception) { }

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

            Debug.WriteLine("Client connected");
        }

        private void Client_Disconnected(Client sender)
        {
            //throw new NotImplementedException();
        }

        private void Client_Received(Client sender, byte[] data)
        {
            Dispatcher.Invoke(()=>
            {
                Debug.WriteLine(Encoding.ASCII.GetString(data));
                if(Encoding.ASCII.GetString(data) == "Update")
                {
                    LoadData();
                }
            });
        }

        public void LoadData()
        {
            txtNoAntri.Content = cmd.GetNoAntriPeriksa();
            txtTotalAntri.Content = "Total Antrian: " + cmd.GetTotalPasien();
            
            var antrian =  cmd.GetDataAntrian();
            dtgAntrian.ItemsSource = antrian;
        }

        private void btnPanggil_Click(object sender, RoutedEventArgs e)
        {
            var last = cmd.GetLastNoUrut();

            if (cmd.UpdateStatusAntrian(last))
            {
                LoadData();
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

            btnPanggil.IsEnabled = false;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            var last = cmd.GetLastNoUrut();

            Debug.WriteLine(last);
            if (cmd.UpdateStatusAntrian(last))
            {
                LoadData();
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
    }
}
