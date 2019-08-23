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
    ///     Interaction logic for PopUpObat.xaml
    /// </summary>
    public partial class PopUpObat : Window
    {
        private readonly DBCommand cmd;
        private readonly SqlConnection conn;
        private readonly InputResep ir;

        public PopUpObat()
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);

            DisplayDataObat();
        }

        public PopUpObat(InputResep ir)
        {
            InitializeComponent();
            conn = DBConnection.dbConnection();
            cmd = new DBCommand(conn);
            this.ir = ir;

            DisplayDataObat();
        }

        public void DisplayDataObat(string nama = null)
        {
            var data = cmd.GetDataObat();

            if (nama == null)
            {
                dgDataObat.ItemsSource = data;
            }
            else
            {
                var filter = data.Where(x => x.nama_obat.ToLower().Contains(nama.ToLower()));
                dgDataObat.ItemsSource = filter;
            }
        }

        private void txtSearchPasien_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as TextBox;
            source.Clear();
        }

        private void txtSearchPasien_TextChanged(object sender, TextChangedEventArgs e)
        {
            var nama = sender as TextBox;

            if (nama.Text != "Nama obat") DisplayDataObat(nama.Text);
        }

        private void dgDataObat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var data = dgDataObat.SelectedItem as ModelObat;
            if (data != null) ir.FillTextBox(data.kode_obat, data.nama_obat);

            ir.txtObat.Focus();
            Close();
        }
    }
}