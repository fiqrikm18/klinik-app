using System.Data;
using System.Data.SqlClient;
using System.Windows;
using admin.DBAccess;
using Microsoft.Reporting.WinForms;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for PVLaporanTransaksi.xaml
    /// </summary>
    public partial class PVLaporanTransaksi : Window
    {
        private readonly string apoteker;
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private readonly string tgl;

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
            if (apoteker == null && tgl == null) dt = cmd.DataTableTransaksi();

            if ((apoteker == null) & (tgl != null)) dt = cmd.DataTableTransaksiByTgl(tgl);

            if ((apoteker != null) & (tgl == null)) dt = cmd.DataTableTransaksiByApoteker(apoteker);

            if ((apoteker != null) & (tgl != null)) dt = cmd.DataTableTransaksiByApotekerTgl(apoteker, tgl);

            rpt.Reset();
            var ds = new ReportDataSource("DataTransaksi", dt);
            rpt.LocalReport.DataSources.Add(ds);
            rpt.LocalReport.ReportPath = @"report\LaporanTransaksi.rdlc";
            rpt.SetDisplayMode(DisplayMode.PrintLayout);
            rpt.RefreshReport();
        }
    }
}