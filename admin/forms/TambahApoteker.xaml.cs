﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using admin.DBAccess;
using admin.Mifare;
using admin.models;
using admin.Utils;
using admin.views;
using MySql.Data.MySqlClient;
using PCSC;
using PCSC.Iso7816;

namespace admin.forms
{
    /// <summary>
    ///     Interaction logic for TambahApoteker.xaml
    /// </summary>
    public partial class TambahApoteker : Window
    {
        private readonly DaftarApoteker da;
        private MApoteker _mDaftarBaru = new MApoteker(" ", " ", " ", " ", " ");
        private int _noOfErrorsOnScreen;

        private const byte Msb = 0x00;
        private IsoReader isoReader;
        private MifareCard card;
        private byte[] key = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        private readonly byte BlockId = 12;
        private readonly byte BlockNamaFrom = 13;
        private readonly byte BlockNamaTo = 16;
        private readonly byte BlockTelp = 17;
        private readonly byte BlockAlamatFrom = 18;
        private readonly byte BlockAlamatTo = 22;
        private readonly byte BlockJenisKelamin = 24;
        private readonly byte BlockPasswordFrom = 25;
        private readonly byte BlockPasswordTo = 26;

        public TambahApoteker(DaftarApoteker da)
        {
            InitializeComponent();
            this.da = da;
            DataContext = _mDaftarBaru;

            var contextFactory = ContextFactory.Instance;
            var ctx = contextFactory.Establish(SCardScope.System);
            var readerNames = ctx.GetReaders();

            if (NoReaderAvailable(readerNames))
            {
                MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var nfcReader = readerNames[0];
                if (string.IsNullOrEmpty(nfcReader))
                    MessageBox.Show("Tidak ada reader tersedia, pastikan reader sudah terhubung dengan komputer",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                try
                {
                    isoReader = new IsoReader(
                        ctx,
                        nfcReader,
                        SCardShareMode.Shared,
                        SCardProtocol.Any,
                        false);

                    card = new MifareCard(isoReader);
                }
                catch (Exception)
                {
                    //MessageBox.Show(ex.Message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        #region member smart card operations

        private bool NoReaderAvailable(ICollection<string> readerNames)
        {
            return readerNames == null || readerNames.Count < 1;
        }

        private bool WriteBlock(byte msb, byte lsb, byte[] data)
        {
            if (card.LoadKey(KeyStructure.VolatileMemory, 0x00, key))
            {
                if (card.Authenticate(msb, lsb, KeyType.KeyA, 0x00))
                    if (card.UpdateBinary(msb, lsb, data))
                        return true;

                return false;
            }

            return false;
        }

        private bool WriteBlockRange(byte msb, byte blockFrom, byte blockTo, byte[] data)
        {
            byte i;
            var count = 0;
            var blockData = new byte[16];

            for (i = blockFrom; i <= blockTo; i++)
            {
                if ((i + 1) % 4 == 0) continue;

                Array.Copy(data, count * 16, blockData, 0, 16);

                if (WriteBlock(msb, i, blockData)) count++;
                else return false;
            }

            return true;
        }

        private byte[] ReadBlock(byte msb, byte lsb)
        {
            var readBinary = new byte[16];

            if (card.LoadKey(KeyStructure.VolatileMemory, 0x00, key))
                if (card.Authenticate(msb, lsb, KeyType.KeyA, 0x00))
                    readBinary = card.ReadBinary(msb, lsb, 16);

            return readBinary;
        }

        //        private byte[] ReadBlockRange(byte msb, byte blockFrom, byte blockTo)
        //        {
        //            byte i;
        //            int nBlock = 0;
        //            int count = 0;
        //
        //            for (i = blockFrom; i <= blockTo;)
        //            {
        //                if ((i + 1) % 4 == 0) continue;
        //                nBlock++;
        //            }
        //
        //            var dataOut = new byte[nBlock * 16];
        //            for (i = blockFrom; i <= blockTo; i++)
        //            {
        //                if ((i + 1) % 4 == 0) continue;
        //                Array.Copy(ReadBlock(msb, i), 0, dataOut, count * 16, 16);
        //                count++;
        //            }
        //
        //            return dataOut;
        //        }

        private byte[] ReadBlockRange(byte msb, byte blockFrom, byte blockTo)
        {
            byte i;
            var nBlock = 0;
            var count = 0;

            for (i = blockFrom; i <= blockTo; i++)
            {
                if ((i + 1) % 4 == 0) continue;
                nBlock++;
            }

            var dataOut = new byte[nBlock * 16];
            for (i = blockFrom; i <= blockTo; i++)
            {
                if ((i + 1) % 4 == 0) continue;
                Array.Copy(ReadBlock(msb, i), 0, dataOut, count * 16, 16);
                count++;
            }

            return dataOut;
        }

        public void ClearAllBlock()
        {
            var res = MessageBox.Show("ApaApakah anda yakin ingin menghapus data kartu? ", "Warning",
                MessageBoxButton.YesNo);

            if (res == MessageBoxResult.Yes)
            {
                var data = new byte[16];
                if (card.LoadKey(KeyStructure.VolatileMemory, 0x00, key))
                {
                    for (byte i = 1; i <= 63; i++)
                        if ((i + 1) % 4 == 0)
                        {
                        }
                        else
                        {
                            if (card.Authenticate(Msb, i, KeyType.KeyA, 0x00))
                            {
                                Array.Clear(data, 0, 16);
                                if (WriteBlock(Msb, i, data))
                                {
                                }
                                else
                                {
                                    MessageBox.Show("Data gagal dihapus");
                                    break;
                                }
                            }
                        }

                    MessageBox.Show("Data berhasil dihapus", "Informasi", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }

        #endregion

        private void BtnBatal_OnClick(object sender, RoutedEventArgs e)
        {
            da.displayDataApoteker();
            Close();
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
                var alamat = TextAlamat.Text;
                var no_telp = txtTelpDokter.Text;
                var jenis_kelamin = cbJenisKelamin.Text;
                var password = txtPassword.Text.ToUpper();

                if (DBConnection.dbConnection().State.Equals(ConnectionState.Closed))
                    DBConnection.dbConnection().Open();

                try
                {
                    var query = "select count(*) from apoteker where id_apoteker = '" + id + "'";
                    var cmd = new MySqlCommand(query, DBConnection.dbConnection());
                    var idExist = int.Parse(cmd.ExecuteScalar().ToString());

                    if (idExist >= 1)
                    {
                        MessageBox.Show("Id sudah terdaftar.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                    else
                    {
                        query =
                            "insert into apoteker(id_apoteker, nama_apoteker, no_telp, alamat, jenis_kelamin, password) VALUES('" +
                            id + "', '" + nama + "', '" + no_telp + "', '" + alamat + "', '" + jenis_kelamin + "', '" +
                            Encryptor.MD5Hash(password) + "')";
                        cmd = new MySqlCommand(query, DBConnection.dbConnection());
                        var res = cmd.ExecuteNonQuery();

                        if (res >= 1)
                        {
                            if (chkCetakKartu.IsChecked == true)
                            {
                                if (!string.IsNullOrEmpty(id))
                                {
                                    if (WriteBlock(Msb, BlockId, Util.ToArrayByte16(id)))
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
                                    if (WriteBlockRange(Msb, BlockNamaFrom, BlockNamaTo, Util.ToArrayByte48(nama)))
                                    {
                                    }
                                    else
                                    {
                                        MessageBox.Show("Nama gagal ditulis.");
                                    }
                                }

                                if (!string.IsNullOrEmpty(no_telp))
                                {
                                    if (WriteBlock(Msb, BlockTelp, Util.ToArrayByte16(no_telp)))
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
                                    if (WriteBlockRange(Msb, BlockAlamatFrom, BlockAlamatTo, Util.ToArrayByte64(alamat)))
                                    {
                                    }
                                    else
                                    {
                                        MessageBox.Show("alamat gagal ditulis.");
                                    }
                                }

                                if (!string.IsNullOrEmpty(jenis_kelamin))
                                {
                                    if (WriteBlock(Msb, BlockJenisKelamin, Util.ToArrayByte16(jenis_kelamin)))
                                    {
                                    }
                                    else
                                    {
                                        MessageBox.Show("Jenis kelamin gagal ditulis.");
                                    }
                                }

                                if (!string.IsNullOrEmpty(id))
                                {
                                    if (WriteBlockRange(Msb, BlockPasswordFrom, BlockPasswordTo, Util.ToArrayByte32(Encryptor.MD5Hash(id))))
                                    {
                                    }
                                    else
                                    {
                                        MessageBox.Show("Password gagal ditulis.");
                                    }
                                }
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                !string.IsNullOrWhiteSpace(txtPassword.Text))
                return true;

            return false;
        }

        #endregion
    }
}