using System.Windows.Controls;
using Apotik.models;
using System.Collections.Generic;
using Apotik.DBAccess;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Apotik.views
{
    /// <summary>
    ///     Interaction logic for DaftarResep.xaml
    /// </summary>
    public partial class DaftarResep : Page
    {
        SqlConnection conn;
        DBCommand cmd;

        public DaftarResep()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            DisplayDataAntrianApotek();
        }

        public void DisplayDataAntrianApotek()
        {
            List<ModelAntrianApotik> antrian = cmd.GetDataAntrianApotik();
            dtgResep.ItemsSource = antrian;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var kode_resep = "";

            if (dtgResep.SelectedItems.Count > 0)
            {
                for (var i = 0; i < dtgResep.SelectedItems.Count; i++)
                {
                    kode_resep = (dtgResep.SelectedCells[2].Column
                            .GetCellContent(dtgResep.SelectedItems[i]) as TextBlock)
                        .Text;
                }

                Debug.WriteLine(kode_resep);

                views.BuatResep br = new views.BuatResep(kode_resep);
                NavigationService.Navigate(br);
            }
        }
    }
}