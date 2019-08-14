using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly SqlConnection conn;

        private readonly ViewRekamMedis vmr;

        // TODO: input manual rekam medis
        private int _noOfErrorsOnScreen;
        private ModelRekamMedis mrm;
        private string no_rm = "";

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

            var lstDiagnosa = txtKodeDiagnosis.Text.Split(';').ToArray();
            var lstTindakan = txtKodeTindakan.Text.Split(';').ToArray();

            var riwayat_penyakit = "";
            if (txtRiwayat.Text == string.Empty)
                riwayat_penyakit = "-";
            else
                riwayat_penyakit = txtRiwayat.Text;

            var no_rm = txtRekamMedis.Text;
            var berat_badan = txtBeratBadan.Text;
            var alergi = txtAlergi.Text;
            var keluhan = textKeluhan.Text;
            var diagnosa = "";
            var tindakan = "";
            var id_dokter = Settings.Default.KodeDokter;
            var kode_poli = cmd.GetKodePoli();

            cmd.CloseConnection();

            var res = false;

            if (CheckTextBox())
            {
                if (!Regex.IsMatch(berat_badan, "^[A-Za-z]+$"))
                {
                    for (var i = 0; i < lstDiagnosa.Length - 1; i++)
                    {
                        diagnosa = lstDiagnosa[i];
                        for (var j = 0; j < lstTindakan.Length - 1; j++)
                        {
                            if (string.IsNullOrEmpty(alergi)) alergi = "-";

                            tindakan = lstTindakan[j];
                            if (cmd.InsertDataRekamMedis(no_rm, riwayat_penyakit, alergi, berat_badan, keluhan,
                                diagnosa, tindakan, id_dokter, kode_poli))
                            {
                                res = true;
                            }
                            else
                            {
                                res = false;
                                break;
                            }
                        }
                    }

                    if (res)
                        MessageBox.Show("Rekam medis berhasil di tambahkan.", "Informasi", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    else
                        MessageBox.Show("Rekam medis gagal di tambahkan.", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);

                    DataContext = mrm;
                    vmr.DisplayDataPasien(no_rm);
                    Close();
                }
                else
                {
                    MessageBox.Show("Berat badan harus berupa angka.", "Perhatian", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
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

        private void BtnSrcDiag_Click(object sender, RoutedEventArgs e)
        {
            var pd = new PopupDiagnosis(this);
            pd.Show();
        }

        private void BtnSrcTindakan_Click(object sender, RoutedEventArgs e)
        {
            var pt = new PopupTindakan(this);
            pt.Show();
        }

        public void FillDiagnosis(string kode, string desk)
        {
            txtKodeDiagnosis.Text = kode;
            textDiagnosa.Text = desk;
        }

        public void FillTindakan(string kode, string desk)
        {
            txtKodeTindakan.Text = kode;
            textTindakan.Text = desk;
        }

        private void TxtKodeDiagnosis_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKodeDiagnosis.Text == string.Empty) textDiagnosa.Text = string.Empty;
        }

        private void TxtKodeTindakan_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtKodeTindakan.Text == string.Empty) textTindakan.Text = string.Empty;
        }
    }
}