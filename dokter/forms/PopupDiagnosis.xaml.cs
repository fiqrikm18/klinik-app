using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using dokter.DBAccess;
using dokter.models;

namespace dokter.forms
{
    /// <summary>
    ///     Interaction logic for PopupDiagnosis.xaml
    /// </summary>
    public partial class PopupDiagnosis : Window
    {
        private readonly DBCommand cmd;

        private readonly SqlConnection conn;
        private readonly InputRekamMedis mw;
        private string desk;
        private string kode;

        public PopupDiagnosis(InputRekamMedis mw)
        {
            InitializeComponent();
            this.mw = mw;

            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            LoadData();
        }

        public void LoadData(string src = null)
        {
            if (string.IsNullOrEmpty(src) || string.IsNullOrWhiteSpace(src))
                dtgTindakan.ItemsSource = cmd.GetDataDiagnosis();
            else
                dtgTindakan.ItemsSource = cmd.GetDataDiagnosis()
                    .Where(x => x.kode.Contains(src.ToUpper()) || x.desk.Contains(src));
        }

        private void TxtSrc_TextChanged(object sender, TextChangedEventArgs e)
        {
            var src = e.Source as TextBox;
            if (src.Text != "Kode ICD/Diagnosa" || string.IsNullOrEmpty(src.Text)) LoadData(src.Text);
        }

        private void DtgTindakan_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (ModelDiagnosis md in dtgTindakan.SelectedItems)
                if (kode == null && desk == null)
                {
                    kode = md.kode;
                    desk = md.desk;
                }
                else
                {
                    kode += ";" + md.kode;
                    desk += ";\n" + md.desk;
                }
        }

        private void BtnDOne_Click(object sender, RoutedEventArgs e)
        {
            mw.FillDiagnosis(kode + ";", desk + ";");
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TxtSrc_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtSrc.Text == "Kode ICD/Diagnosa" || !string.IsNullOrEmpty(txtSrc.Text) ||
                !string.IsNullOrWhiteSpace(txtSrc.Text))
                txtSrc.Text = string.Empty;
        }
    }
}