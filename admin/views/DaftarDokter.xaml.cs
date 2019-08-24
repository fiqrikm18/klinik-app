using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.forms;
using admin.Mifare;
using admin.models;
using admin.Utils;

namespace admin.views
{
    /// <summary>
    ///     Interaction logic for DaftarDokter.xaml
    /// </summary>
    public partial class DaftarDokter : Page
    {
        private const byte Msb = 0x00;
        private readonly byte BlockAlamatFrom = 18;
        private readonly byte BlockAlamatTo = 22;

        private readonly byte BlockId = 12;
        private readonly byte BlockJenisKelamin = 26;
        private readonly byte BlockNamaFrom = 13;
        private readonly byte BlockNamaTo = 16;
        private readonly byte BlockPasswordFrom = 28;
        private readonly byte BlockPasswordTo = 29;
        private readonly byte BlockSpesialisasi = 24;
        private readonly byte BlockTelp = 17;
        private readonly byte BlockTugas = 25;
        private readonly DBCommand cmd;

        private readonly SqlConnection conn;

        private readonly SmartCardOperation sp;

        public DaftarDokter()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            sp = new SmartCardOperation();

            displayDataDokter();

            if (sp.IsReaderAvailable())
            {
            }
            else
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void displayDataDokter(string nama = null)
        {
            var dokter = cmd.GetDataDokter();

            if (string.IsNullOrEmpty(nama))
            {
                dtgDataDokter.ItemsSource = dokter;
            }
            else
            {
                var filtered = dokter.Where(x => x.nama.ToLower().Contains(nama.ToLower()));
                dtgDataDokter.ItemsSource = filtered;
            }
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void TxtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nama = sender as TextBox;

            if (nama.Text != "Nama Dokter")
                displayDataDokter(nama.Text);
        }

        private void BtnTambahDokter_OnClick(object sender, RoutedEventArgs e)
        {
            var td = new TambahDokter(this);
            td.Show();
        }

        private void BtnUbahDokter_OnClick(object sender, RoutedEventArgs e)
        {
            var id = "";
            var nama = "";
            var telp = "";
            var alamat = "";
            var spesialis = "";
            var tugas = "";
            var jenisK = "";

            if (dtgDataDokter.SelectedItems.Count > 0)
            {
                foreach (MDokter data in dtgDataDokter.SelectedItems)
                {
                    id = data.id;
                    nama = data.nama;
                    jenisK = data.jenis_kelamin;
                    telp = data.telp;
                    alamat = data.alamat;
                    spesialis = data.spesialisasi;
                    tugas = data.tugas;
                }

//                for (var i = 0; i < dtgDataDokter.SelectedItems.Count; i++)
//                {
//                    id = (dtgDataDokter.SelectedCells[0].Column
//                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
//                    nama = (dtgDataDokter.SelectedCells[1].Column
//                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
//                    jenisK = (dtgDataDokter.SelectedCells[2].Column
//                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
//                    telp = (dtgDataDokter.SelectedCells[3].Column
//                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
//                    alamat = (dtgDataDokter.SelectedCells[4].Column
//                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
//                    spesialis = (dtgDataDokter.SelectedCells[5].Column
//                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
//                    tugas = (dtgDataDokter.SelectedCells[6].Column
//                        .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock).Text;
//                }

                var ud = new UbahDokter(id, nama, telp, alamat, spesialis, jenisK, tugas, this);
                ud.Show();
            }
            else
            {
                MessageBox.Show("Pilih data yang ingin di ubah terlebih dahulu.", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void BtnHapusDokter_OnClick(object sender, RoutedEventArgs e)
        {
            if (dtgDataDokter.SelectedItems.Count > 0)
            {
                var a = MessageBox.Show("Anda yakin ingin menghapus data dokter?", "Konfirmasi", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (a == MessageBoxResult.Yes)
                {
                    var res = false;

                    for (var i = 0; i < dtgDataDokter.SelectedItems.Count; i++)
                        if (cmd.DeleteDataDokter((dtgDataDokter.SelectedCells[0].Column
                            .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock)?.Text))
                            res = true;

                    if (res)
                        MessageBox.Show("Data dokter berhasil dihapus.", "Info", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    else
                        MessageBox.Show("Data dokter gagal dihapus.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }

                displayDataDokter();
                DBConnection.dbConnection().Close();
            }
            else
            {
                MessageBox.Show("Pilih data dokter yang akan dihapus.", "Info", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnCetakKartu_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.isoReaderInit();
                //card = new MifareCard(isoReader);

                var id = "";
                var nama = "";
                var telp = "";
                var alamat = "";
                var spesialis = "";
                var tugas = "";
                var jenisK = "";

                if (dtgDataDokter.SelectedItems.Count > 0)
                {
                    foreach (MDokter data in dtgDataDokter.SelectedItems)
                    {
                        id = data.id;
                        nama = data.nama;
                        jenisK = data.jenis_kelamin;
                        telp = data.telp;
                        alamat = data.alamat;
                        spesialis = data.spesialisasi;
                        tugas = data.tugas;
                    }

//                    for (var i = 0; i < dtgDataDokter.SelectedItems.Count; i++)
//                    {
//                        id =
//                            (dtgDataDokter.SelectedCells[0].Column
//                                .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock)
//                            .Text;
//                        nama =
//                            (dtgDataDokter.SelectedCells[1].Column
//                                .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock)
//                            .Text;
//                        jenisK =
//                            (dtgDataDokter.SelectedCells[2].Column
//                                .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock)
//                            .Text;
//                        telp =
//                            (dtgDataDokter.SelectedCells[3].Column
//                                .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock)
//                            .Text;
//                        alamat =
//                            (dtgDataDokter.SelectedCells[4].Column
//                                .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock)
//                            .Text;
//                        spesialis =
//                            (dtgDataDokter.SelectedCells[5].Column
//                                .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock)
//                            .Text;
//                        tugas =
//                            (dtgDataDokter.SelectedCells[6].Column
//                                .GetCellContent(dtgDataDokter.SelectedItems[i]) as TextBlock)
//                            .Text;
//                    }

                    if (!string.IsNullOrEmpty(id))
                    {
                        if (sp.WriteBlock(Msb, BlockId, Util.ToArrayByte16(id)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Id gagal ditulis.");
                        }
                    }

                    if (nama.Length > 48)
                        nama = nama.Substring(0, 47);

                    if (!string.IsNullOrEmpty(nama))
                    {
                        if (sp.WriteBlockRange(Msb, BlockNamaFrom, BlockNamaTo, Util.ToArrayByte48(nama)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Nama gagal ditulis.");
                        }
                    }

                    if (!string.IsNullOrEmpty(telp))
                    {
                        if (sp.WriteBlock(Msb, BlockTelp, Util.ToArrayByte16(telp)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("telp gagal ditulis.");
                        }
                    }

                    if (alamat.Length > 64)
                        alamat = alamat.Substring(0, 67);

                    if (!string.IsNullOrEmpty(alamat))
                    {
                        if (sp.WriteBlockRange(Msb, BlockAlamatFrom, BlockAlamatTo, Util.ToArrayByte64(alamat)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("alamat gagal ditulis.");
                        }
                    }

                    if (!string.IsNullOrEmpty(jenisK))
                    {
                        if (sp.WriteBlock(Msb, BlockJenisKelamin, Util.ToArrayByte16(jenisK)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Jenis kelamin gagal ditulis.");
                        }
                    }

                    if (!string.IsNullOrEmpty(id))
                    {
                        if (sp.WriteBlockRange(Msb, BlockPasswordFrom, BlockPasswordTo,
                            Util.ToArrayByte32(Encryptor.MD5Hash(id))))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Password gagal ditulis.");
                        }
                    }

                    if (!string.IsNullOrEmpty(spesialis))
                    {
                        if (sp.WriteBlock(Msb, BlockSpesialisasi, Util.ToArrayByte16(spesialis)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Spesialis gagal ditulis.");
                        }
                    }

                    if (!string.IsNullOrEmpty(tugas))
                    {
                        if (sp.WriteBlock(Msb, BlockTugas, Util.ToArrayByte16(tugas)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Spesialis gagal ditulis.");
                        }
                    }

                    MessageBox.Show("Kartu staff berhasil ditulis.", "Info", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Pilih data yang ingin di cetak", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                sp.isoReaderInit();
            }
        }

        private void BtnCekKartu_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.isoReaderInit();
                //card = new MifareCard(isoReader);

                var msg = "";
                var id = sp.ReadBlock(Msb, BlockId);
                if (id != null) msg += "ID: " + Util.ToASCII(id, 0, 16);

                var nama = sp.ReadBlockRange(Msb, BlockNamaFrom, BlockNamaTo);
                if (nama != null) msg += "\nNama: " + Util.ToASCII(nama, 0, 48);

                var telp = sp.ReadBlock(Msb, BlockTelp);
                if (telp != null) msg += "\nTelp: " + Util.ToASCII(telp, 0, 16);

                var alamat = sp.ReadBlockRange(Msb, BlockAlamatFrom, BlockAlamatTo);
                if (alamat != null) msg += "\nAlamat: " + Util.ToASCII(alamat, 0, 64);

                var jenisK = sp.ReadBlock(Msb, BlockJenisKelamin);
                if (jenisK != null) msg += "\nJenis Kelamin: " + Util.ToASCII(jenisK, 0, 16);

                var spesialis = sp.ReadBlock(Msb, BlockSpesialisasi);
                if (spesialis != null) msg += "\nSpesialisasi: " + Util.ToASCII(spesialis, 0, 16);

                var tugas = sp.ReadBlock(Msb, BlockTugas);
                if (tugas != null) msg += "\nPoliklinik: " + Util.ToASCII(tugas, 0, 16);

                var pass = sp.ReadBlockRange(Msb, BlockPasswordFrom, BlockPasswordTo);
                if (pass != null) msg += "\nPassword: " + Util.ToASCII(pass, 0, 32);

                MessageBox.Show(msg, "Informasi Kartu", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.", "Error",
                //    MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                sp.isoReaderInit();
            }
        }

        private void BtnHapusKartu_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.isoReaderInit();
                //card = new MifareCard(isoReader);

                if (sp.ClearAllBlock())
                    MessageBox.Show("Data Kartu berhasil dihapus.", "Info", MessageBoxButton.OK,
                        MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                sp.isoReaderInit();
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var id = "";
            if (dtgDataDokter.SelectedItems.Count > 0)
            {
                foreach (MDokter md in dtgDataDokter.SelectedItems) id = md.id;

                var vd = new VDokter(id);
                vd.Show();
            }
        }
    }
}