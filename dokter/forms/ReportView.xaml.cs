﻿using System.Data.SqlClient;
using System.Windows;
using dokter.DBAccess;
using Microsoft.Reporting.WinForms;

namespace dokter.forms
{
    /// <summary>
    ///     Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView : Window
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private string no_rm;

        public ReportView()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
        }

        public ReportView(string no_rm)
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            this.no_rm = no_rm;
            //MessageBox.Show(this.no_rm);
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