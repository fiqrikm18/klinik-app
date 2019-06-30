using Antrian.DBAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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

namespace Antrian
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string jenis_antrian = Properties.Settings.Default.antrian;
        string poliklinik = Properties.Settings.Default.poliklinik;

        SqlConnection conn;
        DBCommand cmd;

        //TODO: buat socket server 
        public MainWindow()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            Debug.WriteLine($"Kode Poli: {cmd.GetKodePoli()}");

            if(jenis_antrian == "Poliklinik")
            {
                //MessageBox.Show(jenis_antrian);
                this.Title = "Antrian Poli " + poliklinik;
                lbNamaPoli.Text = "Poli " + poliklinik;
                tbjudul.Text += ("Poli " + poliklinik);
            }

            LoadPeriksa();
        }

        public void LoadPeriksa()
        {
            txtNoAntri.Content = cmd.GetNoAntriPeriksa().ToString();
            txtTotalAntri.Content += cmd.GetTotalPasien().ToString();
            DisPlayDataGridAntrian();
        }

        private void DisPlayDataGridAntrian()
        {
            List<models.ModelAntrianPoli> antrian = cmd.GetAntrianPoli();
            dtgAntrian.ItemsSource = antrian;
        }
    }
}
