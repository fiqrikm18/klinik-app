using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using dokter.DBAccess;

namespace dokter.views
{
    /// <summary>
    ///     Interaction logic for DaftarAntrian.xaml
    /// </summary>
    public partial class DaftarAntrian : Page
    {
        private readonly SqlConnection conn;

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
                var cmd = new DBCommand(conn);
                var data = cmd.GetDataAntrian(DateTime.Now.ToString("yyyy-MM-dd"));
                dtgAntrianPasien.ItemsSource = data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void BtnPeriksa_Click(object sender, RoutedEventArgs e)
        {
            var noRm = "";

            if (dtgAntrianPasien.SelectedItems.Count > 0)
                for (var i = 0; i < dtgAntrianPasien.SelectedItems.Count; i++)
                    noRm = (dtgAntrianPasien.SelectedCells[1].Column
                            .GetCellContent(dtgAntrianPasien.SelectedItems[i]) as TextBlock)
                        .Text;

            var vrm = new ViewRekamMedis(noRm);
            NavigationService.Navigate(vrm);
        }
    }
}