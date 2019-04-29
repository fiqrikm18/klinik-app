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

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using pendaftaran.models;

namespace pendaftaran.views
{
    /// <summary>
    /// Interaction logic for daftar_baru.xaml
    /// </summary>
    public partial class daftar_baru : Page
    {
        private static MySqlConnection MsqlConn = null;

        public daftar_baru()
        {
            InitializeComponent();

            List<ComboboxPairs> cbp = new List<ComboboxPairs>();

            try
            {
                if (dbConnection().State.Equals(System.Data.ConnectionState.Open))
                {
                    MySqlCommand command = new MySqlCommand("select * from poliklinik", dbConnection());
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(), reader["kode_poliklinik"].ToString()));
                        }
                    }
                }
                else
                {
                    dbConnection().Open();
                    MySqlCommand command = new MySqlCommand("select * from poliklinik", dbConnection());
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cbp.Add(new ComboboxPairs(reader["nama_poliklinik"].ToString(), reader["kode_poliklinik"].ToString()));
                        }
                    }
                }
            }
            catch (MySqlException)
            {
                MessageBox.Show("Koneksi ke database gagal, periksa kembali database anda...", "Perhatian", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            cbPoliklinik.DisplayMemberPath = "kode_poliklinik";
            cbPoliklinik.SelectedValuePath = "nama_poliklinik";
            cbPoliklinik.ItemsSource = cbp;
            cbPoliklinik.SelectedIndex = 0;
        }

        /// <summary>
        /// create connection to the database
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection dbConnection()
        {
            if (MsqlConn == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["klinikDatabaseConeection"].ConnectionString;
                MsqlConn = new MySqlConnection(connectionString);
            }

            return MsqlConn;
        }

        //public void Test(object sender, RoutedEventArgs e)
        //{
        //    ComboboxPairs cbp = (ComboboxPairs)cbPoliklinik.SelectedItem;

        //    string policode = cbp.nama_poliklinik;

        //    MessageBox.Show(policode.ToString());
        //}
    }
}
