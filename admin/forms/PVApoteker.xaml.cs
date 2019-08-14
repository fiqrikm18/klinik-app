using System.Data.SqlClient;
using System.Windows;
using admin.DBAccess;
using Microsoft.Reporting.WinForms;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for PVApoteker.xaml
    /// </summary>
    public partial class PVApoteker : Window
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private string id;

        public PVApoteker()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
        }

        public PVApoteker(string id)
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
            var dt = cmd.GetDataApoteker(id);
            var ds = new ReportDataSource("DataApoteker", dt);
            rpt.LocalReport.DataSources.Add(ds);
            rpt.LocalReport.ReportPath = @"report\LabelApoteker.rdlc";
            rpt.SetDisplayMode(DisplayMode.PrintLayout);
            rpt.RefreshReport();
        }
    }
}