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
    /// Interaction logic for VDokter.xaml
    /// </summary>
    public partial class VDokter : Window
    {
        string id;
        SqlConnection conn;
        DBCommand cmd;

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
