using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.Mifare;
using admin.models;
using admin.Utils;
using admin.views;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for TambahDokter.xaml
    /// </summary>
    public partial class TambahDokter : Window
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
        private readonly DaftarDokter dd;

        private readonly SmartCardOperation sp;
        private MDokter _mDaftarBaru = new MDokter(" ", " ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;

        private string id = "";

        #region constructor

        public TambahDokter(DaftarDokter du)
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
            sp = new SmartCardOperation();

            dd = du;

            DataContext = new MDokter(" ", " ", " ", " ", " ", " ");
            //DataContext = new MDaftarBaru("123", "123", "ad", "123", " 123123");

            if (sp.IsReaderAvailable())
            {
            }
            else
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var cbp = cmd.GetDataPoliklinik();

            cbPoliklinik.DisplayMemberPath = "nama_poliklinik";
            cbPoliklinik.SelectedValuePath = "kode_poliklinik";
            cbPoliklinik.ItemsSource = cbp;
            cbPoliklinik.SelectedIndex = 0;
        }

        #endregion

        private void BtnBatal_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var vd = new VDokter(id);
            vd.Show();
        }

        #region members UI Control & CRUD Operations

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            if (string.IsNullOrEmpty(source.Text) || string.IsNullOrWhiteSpace(source.Text) || source.Text == " ")
                source.Clear();
        }

        private void AddDokter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddDokter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mDaftarBaru = new MDokter(" ", " ", " ", " ", " ", " ");

            if (checkTextBoxValue())
            {
                var cbp = (ComboboxPairs) cbPoliklinik.SelectedItem;
                var poliname = cbp.nama_poliklinik;
                var policode = cbp.kode_poliklinik;

                var nama = txtNamaDokter.Text;
                var id = txtidDokter.Text.ToUpper();
                this.id = id;
                var telp = txtTelpDokter.Text;
                var alamat = TextAlamat.Text;
                var spesialisasi = txtSpesialisai.Text;
                var jenisK = cbJenisKelamin.Text;
                var password = txtPassword.Text.ToUpper();

                if (cmd.CheckDokterExsist(id) == 1)
                {
                    MessageBox.Show("No id sudah terdaftar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (!Regex.IsMatch(telp, "^[A-Za-z]+$") && Regex.IsMatch(nama, @"^[a-zA-Z\s]*$"))
                    {
                        if (cmd.InsertDataDokter(id, nama, telp, alamat, spesialisasi, policode, jenisK, password))
                        {
                            var isPrinted = false;

                            if (chkChekKartu.IsChecked == true)
                                while (!isPrinted)
                                    try
                                    {
                                        sp.isoReaderInit();
                                        //card = new MifareCard(isoReader);

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
                                            if (sp.WriteBlockRange(Msb, BlockNamaFrom, BlockNamaTo,
                                                Util.ToArrayByte48(nama)))
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
                                            if (sp.WriteBlockRange(Msb, BlockAlamatFrom, BlockAlamatTo,
                                                Util.ToArrayByte64(alamat)))
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

                                        if (!string.IsNullOrEmpty(spesialisasi))
                                        {
                                            if (sp.WriteBlock(Msb, BlockSpesialisasi, Util.ToArrayByte16(spesialisasi)))
                                            {
                                            }
                                            else
                                            {
                                                MessageBox.Show("Spesialis gagal ditulis.");
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(poliname))
                                        {
                                            if (sp.WriteBlock(Msb, BlockTugas, Util.ToArrayByte16(poliname)))
                                            {
                                            }
                                            else
                                            {
                                                MessageBox.Show("Spesialis gagal ditulis.");
                                            }
                                        }

                                        isPrinted = true;

                                        if (isPrinted) break;
                                    }
                                    catch (Exception)
                                    {
                                        var ans = MessageBox.Show(
                                            "Penulisan kartu gagal, pastikan kartu sudah berada pada jangkauan reader.\nApakah anda ingin menulis kartu lain kali?",
                                            "Error",
                                            MessageBoxButton.YesNo, MessageBoxImage.Error);

                                        if (ans == MessageBoxResult.Yes)
                                            break;

                                        sp.isoReaderInit();
                                    }

                            MessageBox.Show("Data dokter berhasil disimpan.", "Informasi", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                            dd.displayDataDokter();
                            DataContext = _mDaftarBaru;
                            cbJenisKelamin.SelectedIndex = 0;
                            cbPoliklinik.SelectedIndex = 0;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Data dokter gagal disimpan.", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Periksa kembali data yang akan di inputkan.", "Peringatan",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Periksa kembali data yang akan di inputkan.", "Informasi", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            e.Handled = true;
        }

        private bool checkTextBoxValue()
        {
            //            if (txtidDokter.Text == " " && txtNamaDokter.Text == " " && txtTelpDokter.Text == " " &&
            //                txtSpesialisai.Text == " " && TextAlamat.Text == " ") return false;

            if (!string.IsNullOrWhiteSpace(txtidDokter.Text) && !string.IsNullOrWhiteSpace(txtNamaDokter.Text) &&
                !string.IsNullOrWhiteSpace(txtTelpDokter.Text) && cbPoliklinik.SelectedIndex != 0 &&
                cbJenisKelamin.SelectedIndex != 0 &&
                !string.IsNullOrWhiteSpace(TextAlamat.Text) && !string.IsNullOrWhiteSpace(txtPassword.Text))
                return true;

            return false;
        }

        #endregion
    }
}