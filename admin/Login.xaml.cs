using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using admin.DBAccess;
using admin.Mifare;
using admin.Utils;
using PCSC;
using PCSC.Monitoring;
using PCSC.Reactive;
using PCSC.Reactive.Events;

namespace admin
{
    /// <summary>
    ///     Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly byte BlockId = 1;
        private readonly byte BlockPasswordFrom = 2;
        private readonly byte BlockPasswordTo = 4;
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private readonly SmartCardOperation sp = new SmartCardOperation();

        private readonly IDisposable subscription;

        public Login()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
            var readers = GetReaders();

            //MessageBox.Show(Properties.Settings.Default.IDStaff);
            if (sp.IsReaderAvailable())
            {
            }
            else
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }

            var monitorFactory = MonitorFactory.Instance;
            subscription = monitorFactory.CreateObservable(SCardScope.System, readers)
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
                    //Debug.WriteLine(ev.ToString());
                    var user = sp.ReadBlock(0x00, BlockId);
                    var pass = sp.ReadBlockRange(0x00, BlockPasswordFrom, BlockPasswordTo);


                    //MessageBox.Show(Utils.Util.ToASCII(user, 0, user.Length, false));
                    //MessageBox.Show(Utils.Util.ToASCII(pass, 0, pass.Length, false));

                    if (cmd.Login(Util.ToASCII(user, 0, user.Length), Util.ToASCII(pass, 0, pass.Length)))
                        Dispatcher.Invoke(() => { sh(); });
                    else
                        MessageBox.Show("Admin tidak terdaftar, hubungi administrator untuk mendaftar.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
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
            //byte[] user = sp.ReadBlock(0x00, BlockId);
            //Properties.Settings.Default.KodeDokter = Utils.Util.ToASCII(user, 0, user.Length, false);
            var lg = new MainWindow();
            Dispose(true);
            subscription?.Dispose();
            lg.Show();
            Close();
        }

        private void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            subscription?.Dispose();
        }

        private string[] GetReaders()
        {
            var contectFactory = ContextFactory.Instance;
            using (var ctx = contectFactory.Establish(SCardScope.System))
            {
                return ctx.GetReaders();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Properties.Settings.Default.KodeDokter = null;
            Environment.Exit(0);
        }
    }
}