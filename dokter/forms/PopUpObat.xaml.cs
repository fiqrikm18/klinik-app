using dokter.DBAccess;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for PopUpObat.xaml
    /// </summary>
    public partial class PopUpObat : Window
    {
        SqlConnection conn;
        DBCommand cmd;
        InputResep ir;

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
                var filter = data.Where(x => x.nama_obat.Contains(nama));
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

            if(nama.Text != "Nama obat")
            {
                DisplayDataObat(nama.Text);
            }
        }

        private void dgDataObat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var data = dgDataObat.SelectedItem as models.ModelObat;
            if(data != null)
            {
                ir.FillTextBox(data.kode_obat, data.nama_obat);
            }

            Close();
        }
    }
}
