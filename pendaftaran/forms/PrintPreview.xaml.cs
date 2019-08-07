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
using System.Windows.Shapes;
using Microsoft.Reporting.WinForms;
using pendaftaran.DBAccess;

namespace pendaftaran.forms
{
    /// <summary>
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : Window
    {
        private string no_rm;
        private SqlConnection conn;
        private DBCommand cmd;

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