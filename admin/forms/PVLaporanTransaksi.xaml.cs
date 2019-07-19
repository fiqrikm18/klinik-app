using admin.DBAccess;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
    /// Interaction logic for PVLaporanTransaksi.xaml
    /// </summary>
    public partial class PVLaporanTransaksi : Window
    {
        SqlConnection conn;
        DBCommand cmd;

        string apoteker = null;
        string tgl = null;
        public PVLaporanTransaksi(string apoteker = null, string tgl = null)
        {
            InitializeComponent();

            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            this.apoteker = apoteker;
            this.tgl = tgl;

            DisplayReport();
        }

        public void DisplayReport()
        {
            DataTable dt = null;
            if(apoteker == null && tgl == null)
            {
                dt = cmd.DataTableTransaksi();
            }

            if(apoteker == null & tgl != null)
            {
                dt = cmd.DataTableTransaksiByTgl(tgl);
            }

            if(apoteker != null & tgl == null)
            {
                dt = cmd.DataTableTransaksiByApoteker(apoteker);
            }

            if(apoteker != null & tgl != null)
            {
                dt = cmd.DataTableTransaksiByApotekerTgl(apoteker, tgl);
            }

            rpt.Reset();
            var ds = new ReportDataSource("DataTransaksi", dt);
            rpt.LocalReport.DataSources.Add(ds);
            rpt.LocalReport.ReportPath = @"report\LaporanTransaksi.rdlc";
            rpt.SetDisplayMode(DisplayMode.PrintLayout);
            rpt.RefreshReport();
        }
    }
}
