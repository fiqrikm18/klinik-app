using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Text;
using System.Windows;
using dokter.DBAccess;
using dokter.Properties;
using dokter.views;
using SimpleTCP;

namespace dokter
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Socket sck;
        //private Socket sck2;
        private readonly SimpleTcpClient clientAntrianPoli;
        private readonly DBCommand cmd;
        private SerialPort sp;

        public MainWindow()
        {
            InitializeComponent();
            var role = Settings.Default.Role;
            lblHeader.Content = "Poli " + role;

            cmd = new DBCommand(DBConnection.dbConnection());
            InitSerialPort();

            try
            {
                //sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //sck.Connect(Settings.Default.SocketServerAntri, Settings.Default.SocketPortAntri);
                clientAntrianPoli = new SimpleTcpClient();
                clientAntrianPoli.StringEncoder = Encoding.UTF8;
                clientAntrianPoli.DataReceived += ClientAntrianPoli_DataReceived;
                clientAntrianPoli.Connect(Settings.Default.SocketServerAntri, Settings.Default.SocketPortAntri);
            }
            catch (Exception)
            {
                // ignored
            }

            try
            {
                sp.DataReceived += Sp_DataReceived;
                sp.ErrorReceived += Sp_ErrorReceived;
                sp.Open();
            }
            catch (Exception)
            {
                InitSerialPort();
            }

            var userPrefs = new UserPreferences();

            Height = userPrefs.WindowHeight;
            Width = userPrefs.WindowWidth;
            Top = userPrefs.WindowTop;
            Left = userPrefs.WindowLeft;
            WindowState = userPrefs.WindowState;
        }

        private void ClientAntrianPoli_DataReceived(object sender, Message e)
        {
            //throw new NotImplementedException();
            Dispatcher.Invoke(() =>
            {
                var a = e.MessageString.Replace("\u0013", "");
                //Debug.WriteLine(a);
                if (a == "Connected")
                {
                    Settings.Default.IsRemoteConnected = true;
                    if (Settings.Default.IsRemoteConnected) MessageBox.Show("Connection successfull");
                }
                else if (a == "Disconnected")
                {
                    Settings.Default.IsRemoteConnected = false;
                    if (!Settings.Default.IsRemoteConnected) MessageBox.Show("Disconnecting successfull");
                }
            });
        }

        private void InitSerialPort()
        {
            sp = new SerialPort
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
            //Debug.Write(sp.ReadLine());
            Dispatcher.Invoke(() =>
            {
                var a = sp.ReadLine().Replace("\r", "");
                //sck.Send(Encoding.ASCII.GetBytes(a));
                //Debug.WriteLine(a);
                if (Settings.Default.IsRemoteConnected)
                {
                    if (int.TryParse(a, out var v))
                        //Debug.WriteLine(a);
                        clientAntrianPoli.WriteLineAndGetReply(a, TimeSpan.FromSeconds(0));

                    if (a == ">>|")
                    {
                        if (cmd.UpdateAntrian())
                            try
                            {
                                //Debug.WriteLine(a);
                                clientAntrianPoli.WriteLine("Update");
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                    }
                    else if (a == "|<<")
                    {
                        //Debug.WriteLine(a);
                        if (cmd.UpdateAntrianPrev())
                            try
                            {
                                //Debug.WriteLine(a);
                                clientAntrianPoli.WriteLine("Update");
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                    }
                }
                else
                {
                    int v;
                    if (int.TryParse(a, out v))
                        if (v == int.Parse(cmd.GetKodePoli()))
                            //Debug.WriteLine(v);
                            //sck.Send(Encoding.ASCII.GetBytes(a));
                            try
                            {
                                clientAntrianPoli.WriteLineAndGetReply(v.ToString(), TimeSpan.FromSeconds(0));
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                }
            });
        }

        //private bool SocketConnected(Socket s)
        //{
        //    bool a = sck.Poll(1000, SelectMode.SelectRead);
        //    bool b = (s.Available == 0);

        //    if (a && b)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

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