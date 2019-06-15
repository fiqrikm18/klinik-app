using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PCSC;
using PCSC.Iso7816;
using pendaftaran.DBAccess;
using pendaftaran.forms;
using pendaftaran.Mifare;
using pendaftaran.models;
using pendaftaran.Utils;

namespace pendaftaran.views
{
    /// <summary>
    ///     Interaction logic for daftar_ulang.xaml
    /// </summary>
    public partial class daftar_ulang : Page
    {
        private const byte Msb = 0x00;
        private readonly byte blockAlamatForm = 8;
        private readonly byte blockAlamatTo = 12;
        private readonly byte blockIdPasien = 1;
        private readonly byte blockGolDarah = 13;
        private readonly byte blockJenisKelamin = 17;
        private readonly byte blockNamaFrom = 4;
        private readonly byte blockNamaTo = 6;
        private readonly byte blockNoRekamMedis = 2;
        private readonly byte blockNoTelp = 14;
        private readonly byte blockTglLahir = 16;

        public string alamat;
        public string golDarah;
        public string jenisK;
        public string namaP;
        public string noidP;
        public string normP;
        public string noTelp;
        public string tglLahir;

        SqlConnection conn;
        SmartCardOperation sp;

        public daftar_ulang()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();

            DisplayDataPasien();
            sp = new SmartCardOperation();
            sp = new SmartCardOperation();

            if (sp.IsReaderAvailable()) { }
            else
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DisplayDataPasien(string nama = null)
        {
            DBCommand cmd = new DBCommand(conn);
            List<ModelPasien> pasien = cmd.GetDataPasien();

            if (string.IsNullOrEmpty(nama))
            {
                dtgDataPasien.ItemsSource = pasien;
            }
            else
            {
                IEnumerable<ModelPasien> filtered = pasien.Where(x => x.nama.ToLower().Contains(nama.ToLower()));
                dtgDataPasien.ItemsSource = filtered;
            }
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void UbahDataPasien(object sender, RoutedEventArgs e)
        {
            //(dtgDataPasien.SelectedCells[0].Column.GetCellContent(this.dtgDataPasien.SelectedItems[i]) as TextBlock).Text
            if (dtgDataPasien.SelectedItems.Count > 0)
            {
                for (var i = 0; i < dtgDataPasien.SelectedItems.Count; i++)
                {
                    normP =
                        (dtgDataPasien.SelectedCells[1].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    noidP =
                        (dtgDataPasien.SelectedCells[0].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    namaP =
                        (dtgDataPasien.SelectedCells[2].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    golDarah =
                        (dtgDataPasien.SelectedCells[3].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    jenisK =
                        (dtgDataPasien.SelectedCells[4].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    noTelp =
                        (dtgDataPasien.SelectedCells[5].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    alamat =
                        (dtgDataPasien.SelectedCells[6].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                    tglLahir =
                        (dtgDataPasien.SelectedCells[7].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                }

                var ud = new ubah_dataPasien(normP, noidP, namaP, jenisK, noTelp, alamat, golDarah, this);
                ud.Show();
            }
            else
            {
                MessageBox.Show("Pilih data yang ingin diubah", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TambahPasien(object sender, RoutedEventArgs e)
        {
            var db = new daftar_baru();
            NavigationService.Navigate(db);
        }

        private void HapusDataPasien(object sender, RoutedEventArgs e)
        {
            if (dtgDataPasien.SelectedCells.Count > 0)
            {
                //object item = dtgDataPasien.SelectedItem;
                //string id = (dtgDataPasien.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text.ToString();
                //MessageBox.Show(id);
                DBCommand cmd = new DBCommand(conn);

                var a = MessageBox.Show("Anda yakin ingin menghapus data pasien?", "Konfirmasi", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (a == MessageBoxResult.Yes)
                {
                    var res = false;

                    for (var i = 0; i < dtgDataPasien.SelectedItems.Count; i++)
                    {
                        if (cmd.DeleteDataPasien((dtgDataPasien.SelectedCells[1].Column.GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)?.Text))
                        {
                            res = true;
                        }
                    }

                    if (res)
                        MessageBox.Show("Data pasien berhasil dihapus.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    else
                        MessageBox.Show("Data pasien gagal dihapus.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }

                DisplayDataPasien();
            }
            else
            {
                MessageBox.Show("Pilih data pasien yang akan dihapus.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void TxtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nama = sender as TextBox;

            if (nama.Text != "Nama Pasien")
                DisplayDataPasien(nama.Text);
        }

        private void Btn_cetak_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.isoReaderInit();
                //card = new MifareCard(isoReader);

                var ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
                ci.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
                Thread.CurrentThread.CurrentCulture = ci;

                if (dtgDataPasien.SelectedItems.Count > 0)
                {
                    for (var i = 0; i < dtgDataPasien.SelectedItems.Count; i++)
                    {
                        normP =
                        (dtgDataPasien.SelectedCells[1].Column
                            .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                        .Text;
                        noidP =
                            (dtgDataPasien.SelectedCells[0].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        namaP =
                            (dtgDataPasien.SelectedCells[2].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        golDarah =
                            (dtgDataPasien.SelectedCells[3].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        jenisK =
                            (dtgDataPasien.SelectedCells[4].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        noTelp =
                            (dtgDataPasien.SelectedCells[5].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        alamat =
                            (dtgDataPasien.SelectedCells[6].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                        tglLahir =
                            (dtgDataPasien.SelectedCells[7].Column
                                .GetCellContent(dtgDataPasien.SelectedItems[i]) as TextBlock)
                            .Text;
                    }

                    if (!string.IsNullOrEmpty(noidP))
                    {
                        if (sp.WriteBlock(Msb, blockIdPasien, Util.ToArrayByte16(noidP)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("ID pasien gagal ditulis");
                        }
                    }

                    if (!string.IsNullOrEmpty(golDarah))
                    {
                        if (sp.WriteBlock(Msb, blockGolDarah, Util.ToArrayByte16(golDarah)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Golongan Darah gagal ditulis");
                        }
                    }

                    if (!string.IsNullOrEmpty(normP))
                    {
                        if (sp.WriteBlock(Msb, blockNoRekamMedis, Util.ToArrayByte16(normP)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Nomor rekam medis gagal ditulis");
                        }
                    }

                    if (namaP.Length > 48)
                        namaP = namaP.Substring(0, 47);

                    if (!string.IsNullOrEmpty(namaP))
                    {
                        if (sp.WriteBlockRange(Msb, blockNamaFrom, blockNamaTo,
                            Util.ToArrayByte48(namaP)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Nama pasien gagal ditulis");
                        }
                    }

                    if (!string.IsNullOrEmpty(tglLahir))
                    {
                        if (sp.WriteBlock(Msb, blockTglLahir, Util.ToArrayByte16(tglLahir)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Tanggal lahir pasien gagal ditulis");
                        }
                    }

                    if (!string.IsNullOrEmpty(jenisK))
                    {
                        if (sp.WriteBlock(Msb, blockJenisKelamin,
                            Util.ToArrayByte16(jenisK)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Jenis kelamin pasien gagal ditulis");
                        }
                    }

                    if (!string.IsNullOrEmpty(noTelp))
                    {
                        if (sp.WriteBlock(Msb, blockNoTelp, Util.ToArrayByte16(noTelp)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Nomor telepon pasien gagal ditulis");
                        }
                    }

                    if (alamat.Length > 64)
                        alamat = alamat.Substring(0, 63);

                    if (!string.IsNullOrEmpty(alamat))
                    {
                        if (sp.WriteBlockRange(Msb, blockAlamatForm, blockAlamatTo,
                            Util.ToArrayByte64(alamat)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Alamat pasien gagal ditulis");
                        }
                    }

                    MessageBox.Show("Data pasien berhasil ditulis", "Informasi", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Pilih data yang ingin dicetak", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                sp.isoReaderInit();
            }
        }

        private void Btn_cekData_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.isoReaderInit();
                //card = new MifareCard(isoReader);

                var msg = "";
                var rm = sp.ReadBlock(Msb, blockNoRekamMedis);
                if (rm != null)
                    msg += "Nomor Rekam Medis \t: " + Util.ToASCII(rm, 0, 16, false);

                var nId = sp.ReadBlock(Msb, blockIdPasien);
                if (rm != null)
                    msg += "\nNomor ID Pasien \t\t: " + Util.ToASCII(nId, 0, 16, false);

                var namaP = sp.ReadBlockRange(Msb, blockNamaFrom, blockNamaTo);
                if (namaP != null)
                    msg += "\nNama Pasien \t\t: " + Util.ToASCII(namaP, 0, 48, false);

                var nTelp = sp.ReadBlock(Msb, blockNoTelp);
                if (nTelp != null)
                    msg += "\nNomor Telepon Pasien \t: " + Util.ToASCII(nTelp, 0, 16, false);

                var alamatP = sp.ReadBlockRange(Msb, blockAlamatForm, blockAlamatTo);
                if (alamatP != null)
                    msg += "\nAlamat Pasien \t\t: " + Util.ToASCII(alamatP, 0, 64, false);

                var tglHarie = sp.ReadBlock(Msb, blockTglLahir);
                if (tglHarie != null)
                    msg += "\nTanggal Lahir \t\t: " + Util.ToASCII(tglHarie, 0, 16, false);

                var jk = sp.ReadBlock(Msb, blockJenisKelamin);
                if (jk != null)
                    msg += "\nJenis Kelamin \t\t: " + Util.ToASCII(jk, 0, 16, false);

                var gol = sp.ReadBlock(Msb, blockGolDarah);
                if (gol != null)
                    msg += "\nGolongan Darah \t\t: " + Util.ToASCII(gol, 0, 16, false);

                MessageBox.Show(msg, "Informasi Kartu Pasien", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                sp.isoReaderInit();
            }
        }
    }
}