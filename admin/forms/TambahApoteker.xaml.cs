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
    ///     Interaction logic for TambahApoteker.xaml
    /// </summary>
    public partial class TambahApoteker : Window
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
        private readonly DBCommand cmd;

        private readonly SqlConnection conn;
        private readonly DaftarApoteker da;

        private readonly SmartCardOperation sp;
        private MApoteker _mDaftarBaru = new MApoteker(" ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;

        private string id = "";

        public TambahApoteker(DaftarApoteker da)
        {
            InitializeComponent();

            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            this.da = da;
            DataContext = _mDaftarBaru;

            sp = new SmartCardOperation();

            if (sp.IsReaderAvailable())
            {
            }
            else
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBatal_OnClick(object sender, RoutedEventArgs e)
        {
            da.displayDataApoteker();
            Close();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            var pa = new PVApoteker(id);
            pa.Show();
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

        private void AddApoteker_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddApoteker_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mDaftarBaru = new MApoteker(" ", " ", " ", " ", " ");

            if (checkTextBoxValue())
            {
                var id = txtidDokter.Text.ToUpper();
                var nama = txtNamaDokter.Text;
                this.id = id;
                var alamat = TextAlamat.Text;
                var no_telp = txtTelpDokter.Text;
                var jenis_kelamin = cbJenisKelamin.Text;
                var password = txtPassword.Text.ToUpper();

                if (cmd.CheckApotekerExsist(id) == 1)
                {
                    MessageBox.Show("Id sudah terdaftar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (!Regex.IsMatch(no_telp, "^[A-Za-z]+$") && Regex.IsMatch(nama, "^[A-Za-z]+$"))
                    {
                        if (cmd.InsertDataApoteker(id, nama, no_telp, alamat, jenis_kelamin, password))
                        {
                            var isPrinted = false;

                            if (chkCetakKartu.IsChecked == true)
                                while (!isPrinted)
                                    try
                                    {
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

                                        if (!string.IsNullOrEmpty(no_telp))
                                        {
                                            if (sp.WriteBlock(Msb, BlockTelp, Util.ToArrayByte16(no_telp)))
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

                                        if (!string.IsNullOrEmpty(jenis_kelamin))
                                        {
                                            if (sp.WriteBlock(Msb, BlockJenisKelamin,
                                                Util.ToArrayByte16(jenis_kelamin)))
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

                            MessageBox.Show("Data apoteker berhasil disimpan.", "Informasi", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                            da.displayDataApoteker();
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Data apoteker gagal disimpan.", "Error", MessageBoxButton.OK,
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
                MessageBox.Show("Periksa kembali data yang akan di inputkan.", "Perhatian", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            e.Handled = true;
        }

        private bool checkTextBoxValue()
        {
            //            if (txtidDokter.Text == " " && txtNamaDokter.Text == " " && txtTelpDokter.Text == " " &&
            //                txtSpesialisai.Text == " " && TextAlamat.Text == " ") return false;

            if (!string.IsNullOrWhiteSpace(txtidDokter.Text) && !string.IsNullOrWhiteSpace(txtNamaDokter.Text) &&
                !string.IsNullOrWhiteSpace(txtTelpDokter.Text) && !string.IsNullOrWhiteSpace(TextAlamat.Text) &&
                !string.IsNullOrWhiteSpace(txtPassword.Text) && cbJenisKelamin.SelectedIndex != 0)
                return true;

            return false;
        }

        #endregion
    }
}