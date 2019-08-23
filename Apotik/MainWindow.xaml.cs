using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Windows;
using Apotik.DBAccess;
using Apotik.Properties;
using Apotik.views;
using SimpleTCP;

namespace Apotik
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SimpleTcpClient clientApotik;
        private readonly DBCommand cmd;

        //Socket sck;

        private readonly SqlConnection conn;
        private SerialPort sp;

        public MainWindow()
        {
            InitializeComponent();

            var userPrefs = new UserPreferences();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            try
            {
                InitSerialPort();
                sp.DataReceived += Sp_DataReceived;
                sp.ErrorReceived += Sp_ErrorReceived;
                sp.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                InitSerialPort();
            }

            try
            {
                //sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //sck.Connect(Properties.Settings.Default.SocketAntriApotik, Properties.Settings.Default.PortAntriApotik);
                clientApotik = new SimpleTcpClient();
                clientApotik.Connect(Settings.Default.SocketAntriApotik,
                    Settings.Default.PortAntriApotik);
                clientApotik.DataReceived += ClientApotik_DataReceived;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //InitSerialPort();
            }

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;
        }

        private void ClientApotik_DataReceived(object sender, Message e)
        {
            //throw new NotImplementedException();
            //Debug.WriteLine(e.MessageString);
            var a = e.MessageString.Replace("\u0013", "");
            if (a == "Connected")
            {
                //Debug.WriteLine(a);
                Settings.Default.IsRemoteConnected = true;
                if (Settings.Default.IsRemoteConnected)
                    MessageBox.Show("Connection Successful");
                //Debug.WriteLine(Settings.Default.IsRemoteConnected);
            }
            else if (a == "Disconnected")
            {
                //Debug.WriteLine(a);
                Settings.Default.IsRemoteConnected = false;
                if (!Settings.Default.IsRemoteConnected)
                    MessageBox.Show("Disconnecting Successful");
                //Debug.WriteLine(Settings.Default.IsRemoteConnected);
            }
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
                try
                {
                    //Debug.WriteLine(sp.ReadLine().Replace("\r", ""));
                    //Debug.WriteLine(sp.ReadLine());
                    var a = sp.ReadLine().Replace("\r", "");
                    //Debug.WriteLine(Properties.Settings.Default.IsRemoteConnected);
                    if (!Settings.Default.IsRemoteConnected)
                    {
                        //int v = 0;
                        if (int.TryParse(a, out var v)) clientApotik.WriteLineAndGetReply(a, TimeSpan.FromSeconds(0));
                    }
                    else
                    {
                        if (int.TryParse(a, out var v)) clientApotik.WriteLine(a);

                        if (a == ">>|")
                            if (cmd.UpdateAntrian())
                                clientApotik.WriteLine("Update");

                        if (a == "|<<")
                            if (cmd.UpdateAntrianPrev())
                                clientApotik.WriteLine("Update");
                    }

                    //var a = sp.ReadLine().Replace("\r", "");
                    //if (a == "Update")
                    //{
                    //    if (cmd.UpdateAntrian())
                    //    {
                    //        //sck.Send(Encoding.ASCII.GetBytes("Update"));
                    //        clientApotik.WriteLine("Update");
                    //    }
                    //}
                }
                catch (Exception)
                {
                    //ignore
                }
            });
        }

        private void InitSerialPort()
        {
            sp = new SerialPort
            {
                BaudRate = 9600,
                PortName = Settings.Default.SerialPortName
            };
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
            Settings.Default.IsRemoteConnected = false;
            userPrefs.Save();
        }

        private void TambahObat(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new TambahObat();
        }

        private void DaftarObat(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarObat();
        }

        private void DaftarResep(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarResep();
        }

        private void BuatResep(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new BuatResep();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                //Properties.Settings.Default.IDStaff = null;
                var lg = new Login();
                lg.Show();
                Close();
                GC.SuppressFinalize(this);
            });
        }
    }
}