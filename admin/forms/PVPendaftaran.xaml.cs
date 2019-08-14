using System.Data.SqlClient;
using System.Windows;
using admin.DBAccess;
using Microsoft.Reporting.WinForms;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for PVPendaftaran.xaml
    /// </summary>
    public partial class PVPendaftaran : Window
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private string id;

        public PVPendaftaran()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
        }

        public PVPendaftaran(string id)
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            this.id = id;
            DisplayReport(id);
        }

        private void DisplayReport(string id)
        {
            rpt.Reset();
            var dt = cmd.GetDataStaffPendaftaran(id);
            var ds = new ReportDataSource("DataPendaftaran", dt);
            rpt.LocalReport.DataSources.Add(ds);
            rpt.LocalReport.ReportPath = @"report\LabelPendaftaran.rdlc";
            rpt.SetDisplayMode(DisplayMode.PrintLayout);
            rpt.RefreshReport();
        }
    }
}