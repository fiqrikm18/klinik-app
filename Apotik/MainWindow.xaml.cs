using System.Windows;
using Apotik.DBAccess;

namespace Apotik
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TambahObat(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.TambahObat();
        }

        private void DaftarObat(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.DaftarObat();
        }

        private void DaftarResep(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new views.DaftarResep();
        }
    }
}