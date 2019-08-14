using System.Data.SqlClient;
using System.Windows;
using admin.DBAccess;
using Microsoft.Reporting.WinForms;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for VDokter.xaml
    /// </summary>
    public partial class VDokter : Window
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private string id;

        public VDokter()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
        }

        public VDokter(string id)
        {
            InitializeComponent();
            this.id = id;

            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            DisplayReport(id);
        }

        private void DisplayReport(string id)
        {
            rpt.Reset();
            var dt = cmd.GetDataDokter(id);
            var ds = new ReportDataSource("DataDokter", dt);
            rpt.LocalReport.DataSources.Add(ds);
            rpt.LocalReport.ReportPath = @"report\LabelDokter.rdlc";
            rpt.SetDisplayMode(DisplayMode.PrintLayout);
            rpt.RefreshReport();
        }
    }
}