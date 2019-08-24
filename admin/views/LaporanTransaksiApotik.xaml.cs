using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using admin.DBAccess;
using admin.forms;
using admin.models;

namespace admin.views
{
    /// <summary>
    ///     Interaction logic for LaporanTransaksiApotik.xaml
    /// </summary>
    public partial class LaporanTransaksiApotik : Page
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private string apoteker;
        private string tgl;

        public LaporanTransaksiApotik()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            var ap = cmd.GetDataApoteker();
            ap.Insert(0, new MApoteker("Pilih", "Pilih", "Pilih", "Pilih", "Pilih"));

            cbPoliklinik.SelectedIndex = 0;
            cbPoliklinik.DisplayMemberPath = "nama_apoteker";
            cbPoliklinik.SelectedValuePath = "id_apoteker";
            cbPoliklinik.ItemsSource = ap;

            dtTanggalLahir.Text = DateTime.Now.ToShortDateString();

            LoadData();
        }

        public void LoadData(string apoteker = null, string tgl = null)
        {
            var data = cmd.GetDataTransaksi();

            if (apoteker == null && tgl == null)
            {
                dtgAntrian.ItemsSource = data;
            }
            else if(apoteker == "Pilih" && tgl == DateTime.Now.ToShortDateString())
            {
                dtgAntrian.ItemsSource = data;
            }
            else
            {
                if (tgl != null && apoteker == null)
                    dtgAntrian.ItemsSource = data.Where(x => x.tgl_transaksi.Equals(tgl));

                if (tgl == null && apoteker != null)
                    dtgAntrian.ItemsSource = data.Where(x => x.id_apoteker.Equals(apoteker));

                dtgAntrian.ItemsSource = data.Where(x => x.tgl_transaksi.Equals(tgl) && x.id_apoteker.Equals(apoteker));
            }

            //Debug.WriteLine(data.Where(x=> x.tgl_transaksi.Equals(tgl) && x.id_apoteker.Equals(apoteker)));
        }

        private void DtTanggalLahir_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            Thread.CurrentThread.CurrentCulture = ci;

            tgl = dtTanggalLahir.SelectedDate.Value.ToShortDateString();
            LoadData(apoteker, tgl);
        }

        private void Btn_hapus_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Btn_Print_Click(object sender, RoutedEventArgs e)
        {
            var lt = new PVLaporanTransaksi(apoteker, tgl);
            lt.Show();
        }

        private void CbPoliklinik_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbp = cbPoliklinik.SelectedItem as MApoteker;
            //Debug.Write(cbp.id_apoteker);
            apoteker = cbp.id_apoteker;
            LoadData(apoteker, tgl);
        }
    }
}