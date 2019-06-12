using dokter.DBAccess;
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
    /// Interaction logic for DaftarRekamMedis.xaml
    /// </summary>
    public partial class DaftarRekamMedis : Page
    {
        SqlConnection conn = new SqlConnection();

        public DaftarRekamMedis()
        {
            this.conn = DBConnection.dbConnection();
            DBCommand cmd = new DBCommand(conn);
            cmd.OpenConnection();

            InitializeComponent();
            DisplayDataRekamMedis();
        }

        public void DisplayDataRekamMedis(string no_rm = null)
        {
            DBCommand cmd = new DBCommand(conn);
            var dataRekamMedis = cmd.GetDataRekamMedis();
            dtgDataRekamMedis.ItemsSource = dataRekamMedis;
        }
    }
}
