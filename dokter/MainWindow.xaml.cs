using System;
using System.ComponentModel;
using System.Windows;
using dokter.DBAccess;
using dokter.Properties;
using dokter.views;
using System.IO.Ports;
using System.Net.Sockets;
using System.Data.SqlClient;
using System.Text;

namespace dokter
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO buat fungsi untuk remote control
        SerialPort sp;
        Socket sck;
        Socket sck2;
        DBCommand cmd;

        public MainWindow()
        {
            InitializeComponent();
            var role = Settings.Default.Role;
            lblHeader.Content = "Poli " + role;

            cmd = new DBCommand(DBConnection.dbConnection());
            InitSerialPort();

            try
            {
                sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sck.Connect(Settings.Default.SocketServerAntri, Settings.Default.SocketPortAntri);

                sck2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sck2.Connect(Settings.Default.ScoketServerApotik, Settings.Default.SockertPortApotik);

                sp.DataReceived += Sp_DataReceived;
                sp.ErrorReceived += Sp_ErrorReceived;
                sp.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                InitSerialPort();
            }

            var userPrefs = new UserPreferences();

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;
        }

        private void InitSerialPort()
        {
            sp = new SerialPort()
            {
                BaudRate = 9600,
                PortName = Settings.Default.SerialName
            };
        }

        private void Sp_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
            Dispatcher.Invoke(() =>
            {
                var a = sp.ReadLine().Replace("\r", "");
                if (a == "Update")
                {
                    if(cmd.UpdateAntrian())
                    {
                        sck.Send(Encoding.ASCII.GetBytes("Update"));
                    }
                }
            });
        }

        private void Window_Closing(object sender, CancelEventArgs e)
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

        private void btnAntrianPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarAntrian();
        }

        private void btnDataPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DataPsien();
        }

        private void BtnPeriksaPasien_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ViewRekamMedis();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Settings.Default.KodeDokter = null;
                var lg = new Login();
                lg.Show();
                Close();
                GC.SuppressFinalize(this);
            });
        }
    }
}