using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
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
    ///     Interaction logic for InputResep.xaml
    /// </summary>
    public partial class InputResep : Window
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private readonly ObservableCollection<ModelDetailResep> dataObat;
        private readonly string kode_dokter = Settings.Default.KodeDokter;
        private readonly int lstNoResep;
        private readonly string no_rm;
        private ModelDetailResep _mDetailResep;
        private int _noOfErrorsOnScreen;

        private string kode_obat;
        private string nama_obat;
        private ViewRekamMedis vrm;

        public InputResep()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            var dataDokter = cmd.GetDataDokter();
            lbNamaDokter.Content = "Dokter:\t Dr. " + dataDokter.First().nama;
            dataObat = new ObservableCollection<ModelDetailResep>();

            _mDetailResep = new ModelDetailResep(" ", " ", " ", " ", " ", " ", " ");
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

            new ModelDetailResep(" ", kode_obat, " ", " ", " ", " ", " ");
            DataContext = _mDetailResep;

            var dataDokter = cmd.GetDataDokter();
            lbNamaDokter.Content = "Dokter:\t Dr. " + dataDokter.First().nama;

            lstNoResep = cmd.GetLastNoResep(no_rm);

            if (lstNoResep == 0)
                lstNoResep = 1;
            else
                lstNoResep += 1;

            var no = no_rm + '-' + lstNoResep;
            txtKodeResep.Text = no;

            LoadResep();
        }

        public InputResep(string no_rm, ViewRekamMedis vrm)
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

            _mDetailResep = new ModelDetailResep(" ", " ", " ", " ", " ", " ", " ");
            DataContext = _mDetailResep;

            lstNoResep = cmd.GetLastNoResep(no_rm);

            if (lstNoResep == 0)
                lstNoResep = 1;
            else
                lstNoResep += 1;

            var no = no_rm + '-' + lstNoResep;
            txtKodeResep.Text = no;

            LoadResep();
        }

        ~InputResep()
        {
        }

        public void FillTextBox(string kode_obat, string nama_obat)
        {
            txtObat.Text = nama_obat;
            this.kode_obat = kode_obat;
        }

        public void LoadResep(ModelDetailResep mo = null)
        {
            if (mo != null) dataObat.Add(mo);

            dgListObat.ItemsSource = dataObat;
        }

        private void btnAddToList_Click(object sender, RoutedEventArgs e)
        {
            
            var dosis = txtDosis + " " + cbSatuanDosis.Text;
            var pemakaian = cbAturanPakai.Text + " " + cbSediaanObat.Text + " " + txtJumPemakaian + " " +
                            cbWaktuPemakaian.Text;

            MessageBox.Show(dosis);

            //LoadResep(mdr);
            //ClearTextBox();
        }

        private void ClearTextBox()
        {
            txtJumlah.Text = string.Empty;
            txtObat.Text = string.Empty;
            cbTempatPemakaian.SelectedIndex = 0;
            cbSediaanObat.SelectedIndex = 0;
            cbWaktuPemakaian.SelectedIndex = 0;
            txtDosis.Text = string.Empty;
            txtJumPemakaian.Text = string.Empty;
            cbAturanPakai.SelectedIndex = 0;
            cbSatuanDosis.SelectedIndex = 0;
            //txtPemakaian.Text = string.Empty;
            //txtKeterangan.Text = string.Empty;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var po = new PopUpObat(this);
            po.Show();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSaveRecive_Click(object sender, RoutedEventArgs e)
        {
            var lastNoResep = cmd.GetLastNoResep(no_rm);
            var kode_resep = txtKodeResep.Text.ToUpper();
            var kode_obat = "";
            var jumlah = "";
            var ket = "";
            var dosis = "";
            var pemakaian = "";

            //MessageBox.Show(lastNoResep.ToString());

            if (lastNoResep == 0)
                lastNoResep = 1;
            else
                lastNoResep += 1;

            if (dgListObat.Items.Count > 0)
            {
                if (!cmd.IsDataNomorResepExist(no_rm, kode_resep))
                {
                    if (cmd.InsertDataResep(kode_resep, no_rm, lastNoResep.ToString(), kode_dokter))
                    {
                        var res = false;
                        foreach (ModelDetailResep dr in dgListObat.ItemsSource)
                        {
                            kode_obat = dr.kode_obat;
                            jumlah = dr.jumlah;
                            ket = dr.ket;
                            dosis = dr.dosis;
                            pemakaian = dr.pemakaian;

                            if (cmd.InsertDetailResep(kode_resep, kode_obat, int.Parse(jumlah), ket, dosis, pemakaian))
                                res = true;
                        }

                        if (res)
                        {
                            var no_urut = cmd.GetLastNoUrutApotik();

                            if (no_urut == 0)
                                no_urut = 1;
                            else
                                no_urut += 1;

                            if (cmd.InsertAntrianApotik(no_rm, kode_resep, no_urut.ToString(), "Antri"))
                            {
                                MessageBox.Show("Resep berhasil dibuat. Silahkan ambil resep diapotik.", "Informasi",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                                Close();
                            }
                            else
                            {
                                MessageBox.Show("Resep gagal dibuat.", "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
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
            else
            {
                MessageBox.Show("Tidak ada list obat dalam resep", "Perhatian", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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
            _mDetailResep = new ModelDetailResep(" ", " ", " ", " ", " ", " ", " ");

            if (CheckTextBoxEmpty())
            {
                if (cbWaktuPemakaian.SelectedIndex == 0)
                {
                    cbWaktuPemakaian.Text = "";
                }
                
                var dosis = txtDosis.Text + " " + cbSatuanDosis.Text;
                var pemakaian = cbAturanPakai.Text + " " + cbSediaanObat.Text + " " + txtJumPemakaian.Text + " " +
                                cbWaktuPemakaian.Text;
                var ket = "";

                if (cbTempatPemakaian.SelectedIndex == 0)
                    ket = "-";
                else
                    ket = cbTempatPemakaian.Text;

                //Debug.WriteLine(pemakaian);
                var mdr = new ModelDetailResep(txtKodeResep.Text, kode_obat, txtObat.Text, dosis,
                    pemakaian, ket, txtJumlah.Text);

                LoadResep(mdr);
                ClearTextBox();
            }
            else
            {
                MessageBox.Show("Pastikan data yang di inputkan sudah benar.", "Warning", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            e.Handled = true;
        }

        private bool CheckTextBoxEmpty()
        {
            if (!string.IsNullOrEmpty(txtKodeResep.Text) && !string.IsNullOrEmpty(txtObat.Text) &&
                !string.IsNullOrWhiteSpace(txtObat.Text) &&
                !string.IsNullOrEmpty(txtJumlah.Text)
                && !string.IsNullOrEmpty(txtDosis.Text) && !string.IsNullOrEmpty(txtJumPemakaian.Text)
                && cbSatuanDosis.SelectedIndex != 0 && cbAturanPakai.SelectedIndex != 0
                && cbSediaanObat.SelectedIndex != 0)
                return true;

            return false;
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            if (string.IsNullOrEmpty(source.Text) || string.IsNullOrWhiteSpace(source.Text) || source.Text == " ")
                source.Clear();
        }
    }
}