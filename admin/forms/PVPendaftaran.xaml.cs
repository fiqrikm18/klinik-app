using admin.DBAccess;
using Microsoft.Reporting.WinForms;
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

namespace admin.forms
{
    /// <summary>
    /// Interaction logic for PVPendaftaran.xaml
    /// </summary>
    public partial class PVPendaftaran : Window
    {
        string id;
        SqlConnection conn;
        DBCommand cmd;

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
