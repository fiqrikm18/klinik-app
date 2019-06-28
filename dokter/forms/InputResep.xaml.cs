﻿using dokter.DBAccess;
using dokter.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace dokter.forms
{
    /// <summary>
    /// Interaction logic for InputResep.xaml
    /// </summary>
    public partial class InputResep : Window
    {
        private views.ViewRekamMedis vrm;
        SqlConnection conn;
        DBCommand cmd;

        private string kode_obat;
        private string nama_obat;
        private string no_rm;
        private int lstNoResep = 0;
        private ObservableCollection<ModelDetailResep> dataObat;
        private string kode_dokter = Properties.Settings.Default.KodeDokter;

        private ModelDetailResep _mDetailResep;
        private int _noOfErrorsOnScreen;

        public InputResep()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            var dataDokter = cmd.GetDataDokter();
            lbNamaDokter.Content = "Dokter:\t Dr. " + dataDokter.First().nama;
            dataObat = new ObservableCollection<ModelDetailResep>();

            _mDetailResep = new ModelDetailResep(" ", " ", " ", " ", " ", " ");
            DataContext = _mDetailResep;
            LoadResep();
        }

        public InputResep(string kode_obat, string nama_obat)
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
            dataObat = new ObservableCollection<ModelDetailResep>();

            this.kode_obat = kode_obat;
            this.nama_obat = nama_obat;

            new ModelDetailResep(" ", kode_obat, " ", " ", " ", " ");
            DataContext = _mDetailResep;

            var dataDokter = cmd.GetDataDokter();
            lbNamaDokter.Content = "Dokter:\t Dr. " + dataDokter.First().nama;

            lstNoResep = cmd.GetLastNoResep(no_rm);

            if (lstNoResep == 0)
            {
                lstNoResep = 1;
            }
            else
            {
                lstNoResep += 1;
            }
            var no = (no_rm + '-' + lstNoResep).ToString();
            txtKodeResep.Text = no;

            LoadResep();
        }

        public InputResep(string no_rm, views.ViewRekamMedis vrm)
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            this.vrm = vrm;
            dataObat = new ObservableCollection<ModelDetailResep>();

            var dataDokter = cmd.GetDataDokter();
            lbNamaDokter.Content = "Dokter:\t Dr. " + dataDokter.First().nama;
            this.no_rm = no_rm;
            lbNoRM.Text = no_rm;

            _mDetailResep = new ModelDetailResep(" ", " ", " ", " ", " ", " ");
            DataContext = _mDetailResep;

            lstNoResep = cmd.GetLastNoResep(no_rm);

            if (lstNoResep == 0)
            {
                lstNoResep = 1;
            }
            else
            {
                lstNoResep += 1;
            }
            var no = (no_rm + '-' + lstNoResep).ToString();
            txtKodeResep.Text = no;

            LoadResep();
        }

        ~InputResep() { }

        public void FillTextBox(string kode_obat, string nama_obat)
        {
            txtObat.Text = nama_obat;
            this.kode_obat = kode_obat;
        }

        public void LoadResep(ModelDetailResep mo = null)
        {
            if (mo != null)
            {
                dataObat.Add(mo);
            }

            dgListObat.ItemsSource = dataObat;
        }

        private void btnAddToList_Click(object sender, RoutedEventArgs e)
        {
            ModelDetailResep mdr = new ModelDetailResep(txtKodeResep.Text, kode_obat, txtObat.Text, txtPemakaian.Text, txtKeterangan.Text, txtJumlah.Text);
            LoadResep(mdr);
            ClearTextBox();
        }

        private void ClearTextBox()
        {
            txtJumlah.Text = string.Empty;
            txtObat.Text = string.Empty;
            txtPemakaian.Text = string.Empty;
            txtKeterangan.Text = string.Empty;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            PopUpObat po = new PopUpObat(this);
            po.Show();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSaveRecive_Click(object sender, RoutedEventArgs e)
        {
            var lastNoResep = cmd.GetLastNoResep(no_rm);
            var kode_resep = txtKodeResep.Text.ToString().ToUpper();
            var kode_obat = "";
            var jumlah = "";
            var pengunaan = "";
            var ket = "";

            //MessageBox.Show(lastNoResep.ToString());

            if (lastNoResep == 0)
            {
                lastNoResep = 1;
            }
            else
            {
                lastNoResep += 1;
            }

            if (!cmd.IsDataNomorResepExist(no_rm, kode_resep))
            {
                if (cmd.InsertDataResep(kode_resep, no_rm, lastNoResep.ToString(), kode_dokter))
                {
                    bool res = false;
                    foreach (ModelDetailResep dr in dgListObat.ItemsSource)
                    {
                        kode_obat = dr.kode_obat;
                        jumlah = dr.jumlah;
                        pengunaan = dr.dosis;
                        ket = dr.ket;

                        if (cmd.InsertDetailResep(kode_resep, kode_obat, int.Parse(jumlah), ket, pengunaan))
                        {
                            res = true;
                        }
                    }

                    if (res)
                    {
                        var no_urut = cmd.GetLastNoUrutApotik();

                        if (no_urut == 0)
                        {
                            no_urut = 1;
                        }
                        else
                        {
                            no_urut += 1;
                        }

                        if (cmd.InsertAntrianApotik(no_rm, kode_resep, no_urut.ToString(), "Antri"))
                        {
                            MessageBox.Show("Resep berhasil dibuat. Silahkan ambil resep diapotik.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Resep gagal dibuat.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Resep gagal dibuat.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Kode resep sudah terdaftar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;
        }

        private void AddDetailResep_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddDetailResep_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mDetailResep = new ModelDetailResep(" ", " ", " ", " ", " ", " ");

            if (CheckTextBoxEmpty())
            {
                ModelDetailResep mdr = new ModelDetailResep(txtKodeResep.Text, kode_obat, txtObat.Text, txtPemakaian.Text, txtKeterangan.Text, txtJumlah.Text);
                LoadResep(mdr);
                ClearTextBox();
            }
            else
            {
                MessageBox.Show("Pastikan data yang di inputkan sudah benar.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            e.Handled = true;
        }

        private bool CheckTextBoxEmpty()
        {
            if (!string.IsNullOrEmpty(txtKodeResep.Text) && !string.IsNullOrEmpty(txtObat.Text) && !string.IsNullOrEmpty(txtJumlah.Text)
                && !string.IsNullOrEmpty(txtPemakaian.Text))
            {
                return true;
            }

            return false;
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            if (string.IsNullOrEmpty(source.Text) || string.IsNullOrWhiteSpace(source.Text) || source.Text == " ")
            {
                source.Clear();
            }
        }
    }
}