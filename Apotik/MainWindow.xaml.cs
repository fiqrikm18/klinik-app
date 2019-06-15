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

            var userPrefs = new UserPreferences();

            this.Height = userPrefs.WindowHeight;
            this.Width = userPrefs.WindowWidth;
            this.Top = userPrefs.WindowTop;
            this.Left = userPrefs.WindowLeft;
            this.WindowState = userPrefs.WindowState;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var userPrefs = new UserPreferences();

            userPrefs.WindowHeight = this.Height;
            userPrefs.WindowWidth = this.Width;
            userPrefs.WindowTop = this.Top;
            userPrefs.WindowLeft = this.Left;
            userPrefs.WindowState = this.WindowState;

            userPrefs.Save();
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