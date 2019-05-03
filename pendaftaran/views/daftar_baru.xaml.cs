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
using System.Windows.Controls.Primitives;

namespace pendaftaran.views
{
    /// <summary>
    /// Interaction logic for daftar_baru.xaml
    /// </summary>
    public partial class daftar_baru : Page
    {
        private static MySqlConnection MsqlConn = null;
        private int _noOfErrorsOnScreen = 0;
        private MDaftarBaru _mDaftarBaru = new MDaftarBaru(" ", " ", " ", " ", " ");

        public daftar_baru()
        {
            InitializeComponent();

            List<ComboboxPairs> cbp = new List<ComboboxPairs>();
            DataContext = new MDaftarBaru(" ", " ", " ", " ", " ");

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

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;
        }

        private void AddPasien_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (checkTextBoxValue() && _noOfErrorsOnScreen == 0)
            {
                e.CanExecute = _noOfErrorsOnScreen == 0;
            }
            else e.CanExecute = _noOfErrorsOnScreen == 1; 

            e.Handled = true;
        }

        private void AddPasien_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mDaftarBaru = new MDaftarBaru(" ", " ", " ", " ", " ");
            DataContext = _mDaftarBaru;
            e.Handled = true;
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

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox source = e.Source as TextBox;
            source.Clear();
        }

        private bool checkTextBoxValue()
        {
            if(TxtNoRm.Text == " " && TxtNoIdentitas.Text == " " && TxtNamaPasien.Text == " " && TxtNoTelp.Text == " ")
            {
                return false;
            }

            return true;
        }

        //public void Test(object sender, RoutedEventArgs e)
        //{
        //    ComboboxPairs cbp = (ComboboxPairs)cbPoliklinik.SelectedItem;

        //    string policode = cbp.nama_poliklinik;

        //    MessageBox.Show(policode.ToString());
        //}
    }
}
