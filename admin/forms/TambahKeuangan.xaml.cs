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
    ///     Interaction logic for TambahKeuangan.xaml
    /// </summary>
    public partial class TambahKeuangan : Window
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
        private readonly DaftarKeuangan dk;

        private readonly SmartCardOperation sp;
        private ModelKeuangan _mDaftarBaru = new ModelKeuangan(" ", " ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;

        public TambahKeuangan(DaftarKeuangan dk)
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            this.dk = dk;

            DataContext = _mDaftarBaru;
            sp = new SmartCardOperation();

            if (!sp.IsReaderAvailable())
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
        }

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
            _mDaftarBaru = new ModelKeuangan(" ", " ", " ", " ", " ", " ");

            if (checkTextBoxValue())
            {
                if (!Regex.IsMatch(txtTelpDokter.Text, "^[A-Za-z ]+$") &&
                    Regex.IsMatch(txtNamaDokter.Text, @"^[a-zA-Z\s]*$"))
                {
                    var id = txtidDokter.Text.ToUpper();
                    var nama = txtNamaDokter.Text;
                    //this.id = id;
                    var alamat = TextAlamat.Text;
                    var no_telp = txtTelpDokter.Text;
                    var jenis_kelamin = cbJenisKelamin.Text;
                    var password = txtPassword.Text.ToUpper();

                    var ku = new ModelKeuangan
                    {
                        id = id,
                        nama = nama,
                        alamat = alamat,
                        telp = no_telp,
                        jenis_kelamin = jenis_kelamin,
                        password = password
                    };

                    if (cmd.CheckApotekerExsist(id) == 1)
                    {
                        MessageBox.Show("Id sudah terdaftar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        if (!Regex.IsMatch(no_telp, "^[A-Za-z]+$"))
                        {
                            if (cmd.InsertDataKeuangan(ku))
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

                                MessageBox.Show("Data berhasil disimpan.", "Informasi", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                                dk.LoadData();
                                Close();
                            }
                            else
                            {
                                MessageBox.Show("Data gagal disimpan.", "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No. telepon harus berupa angkat.", "Peringatan", MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Periksa kembali data yang akan di inputkan.", "Informasi", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
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
                !string.IsNullOrWhiteSpace(txtTelpDokter.Text) && !string.IsNullOrWhiteSpace(TextAlamat.Text) &&
                !string.IsNullOrWhiteSpace(txtPassword.Text) && cbJenisKelamin.SelectedIndex != 0)
                return true;

            return false;
        }

        private void BtnBatal_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}