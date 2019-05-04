using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;

using MySql.Data.MySqlClient;

namespace ui_design
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);

            string last = "";
            string MyConnection2 = "datasource=localhost;database=db_klinik;port=3306;username=root;password=";
            MySqlConnection conn = new MySqlConnection(MyConnection2);
            conn.Open();
            string query = "select nomor_urut from antrian where poliklinik= '002' and DATE(tanggal_berobat) = '"+ DateTime.Now.ToString("yyyy-MM-dd")+"' ORDER BY nomor_urut desc LIMIT 1;";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader;

            reader = cmd.ExecuteReader();

            if (reader.Read()) last = reader.GetString(0);

            MessageBox.Show(last);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyLabel.Visibility = System.Windows.Visibility.Visible;

            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show("Show some data");
            MyLabel.Visibility = System.Windows.Visibility.Collapsed;

            dispatcherTimer.IsEnabled = true;
        }
    }
}
