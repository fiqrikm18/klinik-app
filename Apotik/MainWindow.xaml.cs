using System.Windows;
using Apotik.views;

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
            MainFrame.Content = new TambahObat();
        }

        private void DaftarObat(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarObat();
        }

        private void DaftarResep(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DaftarResep();
        }
    }
}