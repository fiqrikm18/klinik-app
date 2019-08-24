using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Apotik.DBAccess;

namespace Apotik.views
{
    /// <summary>
    ///     Interaction logic for DaftarResep.xaml
    /// </summary>
    public partial class DaftarResep : Page
    {
        private readonly DBCommand cmd;

        private readonly SqlConnection conn;
        //private Listener listener;

        public DaftarResep()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            DisplayDataAntrianApotek();
        }

        public void DisplayDataAntrianApotek()
        {
            var antrian = cmd.GetDataAntrianApotik();
            dtgResep.ItemsSource = antrian;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var kode_resep = "";

            if (dtgResep.SelectedItems.Count > 0)
            {
                for (var i = 0; i < dtgResep.SelectedItems.Count; i++)
                    kode_resep = (dtgResep.SelectedCells[2].Column
                            .GetCellContent(dtgResep.SelectedItems[i]) as TextBlock)
                        .Text;

                Debug.WriteLine(kode_resep);

                var br = new BuatResep(kode_resep);
                NavigationService.Navigate(br);
            }
        }
    }
}