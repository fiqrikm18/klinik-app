using dokter.DBAccess;
using dokter.models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace dokter.views
{
    /// <summary>
    /// Interaction logic for DaftarAntrian.xaml
    /// </summary>
    public partial class DaftarAntrian : Page
    {
        SqlConnection conn;

        public DaftarAntrian()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            DisplayDataAntrian();
        }

        public void DisplayDataAntrian()
        {
            try
            {
                DBCommand cmd = new DBCommand(conn);
                List<ModelAntrian> data = cmd.GetDataAntrian(DateTime.Now.ToString("yyyy-MM-dd"));
                dtgAntrianPasien.ItemsSource = data;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void BtnPeriksa_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
