using PCSC;
using PCSC.Monitoring;
using PCSC.Reactive;
using PCSC.Reactive.Events;
using pendaftaran.DBAccess;
using pendaftaran.Mifare;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;

namespace pendaftaran
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private SmartCardOperation sp = new SmartCardOperation();
        private readonly byte BlockId = 12;
        private readonly byte BlockPasswordFrom = 25;
        private readonly byte BlockPasswordTo = 26;
        private SqlConnection conn;
        private DBCommand cmd;

        public Login()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
            string[] readers = GetReaders();

            //MessageBox.Show(Properties.Settings.Default.IDStaff);
            if (sp.IsReaderAvailable()) { }
            else
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            IMonitorFactory monitorFactory = MonitorFactory.Instance;
            IDisposable subsription = monitorFactory.CreateObservable(SCardScope.System, readers)
                .Subscribe(onNext, onError);
        }

        private void onError(Exception exception)
        {
            Debug.WriteLine("ERROR: {0}", exception.Message);
        }

        private void onNext(MonitorEvent ev)
        {
            try
            {
                if (ev.ToString() == "PCSC.Reactive.Events.CardInserted")
                {
                    byte[] user = sp.ReadBlock(0x00, BlockId);
                    byte[] pass = sp.ReadBlockRange(0x00, BlockPasswordFrom, BlockPasswordTo);

                    if (cmd.Login(Utils.Util.ToASCII(user, 0, user.Length, false), Utils.Util.ToASCII(pass, 0, pass.Length, false)))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            sh();
                        });
                    }
                    else
                    {
                        MessageBox.Show("Staf Pendaftaran tidak terdaftar, hubungi administrator untuk mendaftar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Pastikan reader sudah terpasang dan kartu sudah berada pada jangkauan reader.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                sp.isoReaderInit();
            }
        }

        private void sh()
        {
            byte[] user = sp.ReadBlock(0x00, BlockId);
            Properties.Settings.Default.IDStaff = Utils.Util.ToASCII(user, 0, user.Length, false);
            MainWindow lg = new MainWindow();
            lg.Show();
            Close();
        }

        private string[] GetReaders()
        {
            IContextFactory contectFactory = ContextFactory.Instance;
            using (ISCardContext ctx = contectFactory.Establish(SCardScope.System))
            {
                return ctx.GetReaders();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IDStaff = null;
            Environment.Exit(0);
        }
    }
}
