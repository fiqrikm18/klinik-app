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
    ///     Interaction logic for DaftarKeuangan.xaml
    /// </summary>
    public partial class DaftarKeuangan : Page
    {
        private const byte Msb = 0x00;
        private readonly byte BlockAlamatFrom = 18;
        private readonly byte BlockAlamatTo = 22;

        private readonly byte BlockId = 12;
        private readonly byte BlockJenisKelamin = 24;
        private readonly byte BlockNamaFrom = 13;
        private readonly byte BlockNamaTo = 16;
        private readonly byte BlockPasswordFrom = 25;
        private readonly byte BlockPasswordTo = 26;
        private readonly byte BlockTelp = 17;
        private readonly SmartCardOperation sp;
        private DBCommand cmd;
        private SqlConnection conn;

        public DaftarKeuangan()
        {
            InitializeComponent();


            sp = new SmartCardOperation();

            LoadData();

            if (!sp.IsReaderAvailable())
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void LoadData(string nama = null)
        {
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            var ku = cmd.GetDataKeuangan();

            if (string.IsNullOrEmpty(nama))
            {
                dtgDataKeuangan.ItemsSource = ku;
            }
            else
            {
                var fill = ku.Where(x => x.nama.ToLower().Contains(nama.ToLower()));
                dtgDataKeuangan.ItemsSource = fill;
            }
        }

        private void TxtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var source = sender as TextBox;
            if (source.Text != "Nama") LoadData(source.Text);
        }

        private void TxtSearchPasien_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void BtnTambahApoteker_Click(object sender, RoutedEventArgs e)
        {
            var tk = new TambahKeuangan(this);
            tk.Show();
        }

        private void BtnUbahApoteker_Click(object sender, RoutedEventArgs e)
        {
            var ku = new ModelKeuangan();
            if (dtgDataKeuangan.SelectedItems.Count > 0)
            {
                foreach (ModelKeuangan k in dtgDataKeuangan.SelectedItems)
                {
                    ku.id = k.id;
                    ku.nama = k.nama;
                    ku.alamat = k.alamat;
                    ku.telp = k.telp;
                    ku.jenis_kelamin = k.jenis_kelamin;
                    ku.password = k.password;
                }

                var uk = new UpdateKeuangan(this, ku);
                uk.Show();
            }
            else
            {
                MessageBox.Show("Pilih data yang ingin diubah.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnHapusApoteker_Click(object sender, RoutedEventArgs e)
        {
            var res = false;
            if (dtgDataKeuangan.SelectedItems.Count > 0)
            {
                foreach (ModelKeuangan k in dtgDataKeuangan.SelectedItems)
                {
                    if (cmd.DeleteDatakeuangan(k.id)) res = true;

                    if (!res) break;
                }

                if (res)
                {
                    MessageBox.Show("Data berhasil dihapus.", "Informasi", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Pilih data yang ingin dihapus.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (dtgDataKeuangan.SelectedItems.Count > 0)
            {
                var id = "";
                foreach (ModelKeuangan k in dtgDataKeuangan.SelectedItems) id = k.id;
                var pv = new PVKeuangan(id);
                pv.Show();
            }
            else
            {
                MessageBox.Show("Pilih data yang ingin dicetak.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCetakKartu_Click(object sender, RoutedEventArgs e)
        {
            sp.isoReaderInit();
            try
            {
                var id = "";
                var nama = "";
                var alamat = "";
                var telp = "";
                var jenis_kelamin = "";
                var password = "";

                if (dtgDataKeuangan.SelectedItems.Count == 1)
                {
                    foreach (ModelKeuangan ku in dtgDataKeuangan.SelectedItems)
                    {
                        id = ku.id;
                        nama = ku.nama;
                        alamat = ku.alamat;
                        telp = ku.telp;
                        jenis_kelamin = ku.jenis_kelamin;
                        password = Encryptor.MD5Hash(id);
                    }

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

                    if (alamat.Length > 64) alamat = alamat.Substring(0, 67);

                    if (!string.IsNullOrEmpty(alamat))
                    {
                        if (sp.WriteBlockRange(Msb, BlockAlamatFrom, BlockAlamatTo, Util.ToArrayByte64(alamat)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Alamat gagal ditulis.");
                        }
                    }

                    if (!string.IsNullOrEmpty(telp))
                    {
                        if (sp.WriteBlock(Msb, BlockTelp, Util.ToArrayByte16(telp)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Telp gagal ditulis.");
                        }
                    }

                    if (!string.IsNullOrEmpty(jenis_kelamin))
                    {
                        if (sp.WriteBlock(Msb, BlockJenisKelamin, Util.ToArrayByte16(jenis_kelamin)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Jenis kelamin gagal ditulis.");
                        }
                    }

                    if (!string.IsNullOrEmpty(password))
                    {
                        if (sp.WriteBlockRange(Msb, BlockPasswordFrom, BlockPasswordTo, Util.ToArrayByte32(password)))
                        {
                        }
                        else
                        {
                            MessageBox.Show("Password gagal ditulis.");
                        }
                    }

                    MessageBox.Show("Kartu staff berhasil ditulis.", "Informasi", MessageBoxButton.OK,
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

        private void BtnCekKartu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

                var pass = sp.ReadBlockRange(Msb, BlockPasswordFrom, BlockPasswordTo);
                if (pass != null) msg += "\nPassword: " + Util.ToASCII(pass, 0, 32);

                MessageBox.Show(msg, "Informasi Kartu", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("Terjadi kesalahan, pastikan kartu sudah berada pada jangkauan reader.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                sp.isoReaderInit();
            }
        }

        private void BtnHapusKartu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sp.isoReaderInit();

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
    }
}