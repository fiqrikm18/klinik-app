using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MySql.Data.MySqlClient;

namespace ui_design
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);

            var last = "";
            var MyConnection2 = "datasource=localhost;database=db_klinik;port=3306;username=root;password=";
            var conn = new MySqlConnection(MyConnection2);
            conn.Open();
            var query = "select nomor_urut from antrian where poliklinik= '002' and DATE(tanggal_berobat) = '" +
                        DateTime.Now.ToString("yyyy-MM-dd") + "' ORDER BY nomor_urut desc LIMIT 1;";

            var cmd = new MySqlCommand(query, conn);
            MySqlDataReader reader;

            reader = cmd.ExecuteReader();

            if (reader.Read()) last = reader.GetString(0);

            MessageBox.Show(last);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyLabel.Visibility = Visibility.Visible;

            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //MessageBox.Show("Show some data");
            MyLabel.Visibility = Visibility.Collapsed;

            dispatcherTimer.IsEnabled = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            Title = textBox.Text + "[Length = " + textBox.Text.Length + "]";
        }
    }
}