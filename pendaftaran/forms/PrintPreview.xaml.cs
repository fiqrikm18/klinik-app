using System.Data.SqlClient;
using System.Windows;
using Microsoft.Reporting.WinForms;
using pendaftaran.DBAccess;

namespace pendaftaran.forms
{
    /// <summary>
    ///     Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : Window
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private readonly string no_rm;

        public PrintPreview()
        {
            InitializeComponent();
            DisplayReport();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
            DisplayReport();
        }

        public PrintPreview(string no_rm)
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            this.no_rm = no_rm;
            DisplayReport();
        }

        public void DisplayReport()
        {
            rpt.Reset();
            var dt = cmd.GetReportDataPasien(no_rm);
            var ds = new ReportDataSource("DataPasien", dt);
            rpt.LocalReport.DataSources.Add(ds);
            rpt.LocalReport.ReportPath = @"report\LabelPrint.rdlc";
            rpt.SetDisplayMode(DisplayMode.PrintLayout);
            rpt.RefreshReport();
        }
    }
}