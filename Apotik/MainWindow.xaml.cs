using Apotik.DBAccess;
using Apotik.views;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
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
        Socket sck;

        SqlConnection conn;
        DBCommand cmd;

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

                sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sck.Connect(Properties.Settings.Default.SocketAntriApotik, Properties.Settings.Default.PortAntriApotik);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                InitSerialPort();
            }

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;
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
                    if (cmd.UpdateAntrian())
                    {
                        sck.Send(Encoding.ASCII.GetBytes("Update"));
                    }
                }
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