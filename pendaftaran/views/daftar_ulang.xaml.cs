using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pendaftaran.views
{
    /// <summary>
    /// Interaction logic for daftar_ulang.xaml
    /// </summary>
    public partial class daftar_ulang : Page
    {
        public daftar_ulang()
        {
            InitializeComponent();
        }

        private void TextBoxFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox source = e.Source as TextBox;
            source.Clear();
        }
    }
}
