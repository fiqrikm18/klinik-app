using pendaftaran.DBAccess;
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

namespace pendaftaran.forms
{
    /// <summary>
    /// Interaction logic for PVPasien.xaml
    /// </summary>
    public partial class PVPasien : Window
    {
        string no_rm;
        SqlConnection conn;
        DBCommand cmd;

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
            var dt = cmd.GetDetailPasien(no_rm);
            var ds = new ReportDataSource("DetailPasien", dt);
            rpt.LocalReport.DataSources.Add(ds);
            rpt.LocalReport.ReportPath = @"report\ReportDataPasien.rdlc";
            rpt.SetDisplayMode(DisplayMode.PrintLayout);
            rpt.RefreshReport();
        }
    }
}
