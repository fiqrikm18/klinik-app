using Apotik.DBAccess;
using Apotik.views;
using SimpleTCP;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;

namespace Apotik
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private SerialPort sp;
        private SimpleTcpClient clientApotik;

        //Socket sck;

        private SqlConnection conn;
        private DBCommand cmd;

        public MainWindow()
        {
            InitializeComponent();

            UserPreferences userPrefs = new UserPreferences();
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
                clientApotik.Connect(Properties.Settings.Default.SocketAntriApotik, Properties.Settings.Default.PortAntriApotik);
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
                Debug.WriteLine(a);
                Properties.Settings.Default.IsRemoteConnected = true;
                if (Properties.Settings.Default.IsRemoteConnected)
                {
                    MessageBox.Show("Connection Successful");
                    Debug.WriteLine(Properties.Settings.Default.IsRemoteConnected);
                }
            }
            else if (a == "Disconnected")
            {
                Debug.WriteLine(a);
                Properties.Settings.Default.IsRemoteConnected = false;
                if (!Properties.Settings.Default.IsRemoteConnected)
                {
                    MessageBox.Show("Disconnecting Successful");
                    Debug.WriteLine(Properties.Settings.Default.IsRemoteConnected);
                }
            }
        }

        private void Sp_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Debug.WriteLine(sp.ReadLine().Replace("\r", ""));
            //throw new NotImplementedException();
            Dispatcher.Invoke(() =>
            {
                //Debug.WriteLine(sp.ReadLine());
                string a = sp.ReadLine().Replace("\r", "");
                //Debug.WriteLine(Properties.Settings.Default.IsRemoteConnected);
                if (!Properties.Settings.Default.IsRemoteConnected)
                {
                    //int v = 0;
                    if(int.TryParse(a, out int v))
                    {
                        clientApotik.WriteLineAndGetReply(a, TimeSpan.FromSeconds(0));
                        Debug.WriteLine(a);
                    }
                }
                else
                {
                    if (int.TryParse(a, out int v))
                    {
                        clientApotik.WriteLine(a);
                    }
                    
                    if(a == ">>|")
                    {
                        if (cmd.UpdateAntrian())
                        {
                            Debug.WriteLine(a);
                            //sck.Send(Encoding.ASCII.GetBytes("Update"));
                            clientApotik.WriteLine("Update");
                        }
                    }
                    if(a == "|<<")
                    {
                        if (cmd.UpdateAntrianPrev())
                        {
                            Debug.WriteLine(a);
                            clientApotik.WriteLine("Update");
                        }
                    }
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
            });
        }

        private void InitSerialPort()
        {
            sp = new SerialPort()
            {
                BaudRate = 9600,
                PortName = Properties.Settings.Default.SerialPortName
            };
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            UserPreferences userPrefs = new UserPreferences
            {
                WindowHeight = Height,
                WindowWidth = Width,
                WindowTop = Top,
                WindowLeft = Left,
                WindowState = WindowState
            };
            Properties.Settings.Default.IsRemoteConnected = false;
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
                Login lg = new Login();
                lg.Show();
                Close();
                GC.SuppressFinalize(this);
            });
        }
    }
}