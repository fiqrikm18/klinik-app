using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using Apotik.DBAccess;
using Apotik.mifare;
using Apotik.Properties;
using Apotik.Utils;
using PCSC;
using PCSC.Monitoring;
using PCSC.Reactive;
using PCSC.Reactive.Events;

namespace Apotik
{
    /// <summary>
    ///     Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly byte BlockId = 12;
        private readonly byte BlockPasswordFrom = 25;
        private readonly byte BlockPasswordTo = 26;
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
                //sp.isoReaderInit();
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
                    var user = sp.ReadBlock(0x00, BlockId);
                    var pass = sp.ReadBlockRange(0x00, BlockPasswordFrom, BlockPasswordTo);

                    if (cmd.Login(Util.ToASCII(user, 0, user.Length, false), Util.ToASCII(pass, 0, pass.Length, false)))
                        Dispatcher.Invoke(() => { sh(); });
                    else
                        MessageBox.Show("Staff apoteker tidak terdaftar, hubungi administrator untuk mendaftar.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var user = sp.ReadBlock(0x00, BlockId);
            Settings.Default.KodeApoteker = Util.ToASCII(user, 0, user.Length, false);
            Dispose();
            subscription?.Dispose();
            var lg = new MainWindow();
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
            Settings.Default.KodeApoteker = null;
            Environment.Exit(0);
        }
    }
}