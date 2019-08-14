using System.Data.SqlClient;
using System.Windows;
using Microsoft.Reporting.WinForms;
using pendaftaran.DBAccess;

namespace pendaftaran.forms
{
    /// <summary>
    ///     Interaction logic for PVPasien.xaml
    /// </summary>
    public partial class PVPasien : Window
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private string no_rm;

        public PVPasien()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
        }

        public PVPasien(string no_rm)
        {
            InitializeComponent();
            this.no_rm = no_rm;

            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            DisplayReport(no_rm);
        }

        private void DisplayReport(string no_rm)
        {
            rpt.Reset();
            var dt = cmd.GetDataPasien(no_rm);
            var dt1 = cmd.GetDataRekamMedis(no_rm);
            var ds = new ReportDataSource("DataPasien", dt);
            var ds1 = new ReportDataSource("DataRekamMedis", dt1);
            rpt.LocalReport.DataSources.Add(ds);
            rpt.LocalReport.DataSources.Add(ds1);
            rpt.LocalReport.ReportPath = @"report\ReportDataPasien.rdlc";
            rpt.SetDisplayMode(DisplayMode.PrintLayout);
            rpt.RefreshReport();
        }
    }
}