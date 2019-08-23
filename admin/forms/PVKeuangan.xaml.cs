using System.Data.SqlClient;
using System.Windows;
using admin.DBAccess;
using Microsoft.Reporting.WinForms;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for PVKeuangan.xaml
    /// </summary>
    public partial class PVKeuangan : Window
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private string id;

        public PVKeuangan()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
        }

        public PVKeuangan(string id)
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
            var dt = cmd.GetDatakeuangan(id);
            var ds = new ReportDataSource("keuangan", dt);
            rpt.LocalReport.DataSources.Add(ds);
            rpt.LocalReport.ReportPath = @"report\LabelKeuangan.rdlc";
            rpt.SetDisplayMode(DisplayMode.PrintLayout);
            rpt.RefreshReport();
        }
    }
}