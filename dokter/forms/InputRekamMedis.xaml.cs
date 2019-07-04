using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using dokter.DBAccess;
using dokter.models;
using dokter.Properties;
using dokter.views;

namespace dokter.forms
{
    /// <summary>
    ///     Interaction logic for InputRekamMedis.xaml
    /// </summary>
    public partial class InputRekamMedis : Window
    {
        private int _noOfErrorsOnScreen;
        private readonly SqlConnection conn;
        private ModelRekamMedis mrm;
        private string no_rm = "";
        private readonly ViewRekamMedis vmr;

        public InputRekamMedis()
        {
            InitializeComponent();
            mrm = new ModelRekamMedis(int.Parse(" "), " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ");
            DataContext = mrm;
            conn = DBConnection.dbConnection();
        }

        public InputRekamMedis(string no_rm, ViewRekamMedis vrm)
        {
            InitializeComponent();
            this.no_rm = no_rm;
            conn = DBConnection.dbConnection();

            vmr = vrm;
            txtRekamMedis.Text = no_rm;
            mrm = new ModelRekamMedis(0, " ", " ", " ", " ", " ", " ", " ", " ", " ", DateTime.Now.ToShortDateString(),
                " ", " ", " ");
            //mrm = new models.ModelRekamMedis(" ", " ", " ");
            DataContext = mrm;
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            if (string.IsNullOrEmpty(source.Text) || string.IsNullOrWhiteSpace(source.Text) || source.Text == " ")
                source.Clear();
        }

        private void BtnBatal_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _noOfErrorsOnScreen++;
            else
                _noOfErrorsOnScreen--;
        }

        private void AddRekamMedis_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _noOfErrorsOnScreen == 0;
            e.Handled = true;
        }

        private void AddRekamMedis_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mrm = new ModelRekamMedis(0, " ", " ", " ", " ", " ", " ", " ", " ", " ", DateTime.Now.ToShortDateString(),
                " ", " ", " ");
            var cmd = new DBCommand(conn);

            var no_rm = txtRekamMedis.Text;
            var riwayat_penyakit = txtRiwayat.Text;
            var berat_badan = txtBeratBadan.Text;
            var alergi = txtAlergi.Text;
            var keluhan = textKeluhan.Text;
            var diagnosa = textDiagnosa.Text;
            var tindakan = textTindakan.Text;
            var id_dokter = Settings.Default.KodeDokter;
            var kode_poli = cmd.GetKodePoli();

            cmd.CloseConnection();

            if (CheckTextBox())
            {
                if (cmd.InsertDataRekamMedis(no_rm, riwayat_penyakit, alergi, berat_badan, keluhan, diagnosa, tindakan,
                    id_dokter, kode_poli))
                {
                    MessageBox.Show("Rekam medis berhasil di tambahkan.", "Informasi", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    DataContext = mrm;
                    vmr.DisplayDataPasien(no_rm);
                    Close();
                }
                else
                {
                    MessageBox.Show("Rekam medis gagal di tambahkan.", "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Pastikan data yang diinputkan sudah benar.", "Perhatian", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            e.Handled = true;
        }

        private bool CheckTextBox()
        {
            if (!string.IsNullOrWhiteSpace(textDiagnosa.Text) && !string.IsNullOrWhiteSpace(textTindakan.Text) &&
                !string.IsNullOrWhiteSpace(textDiagnosa.Text)) return true;

            return false;
        }
    }
}