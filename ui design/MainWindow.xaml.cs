using System.Windows;
using System.Windows.Controls;
using PCSC.Iso7816;
using ui_design.Mifare;

namespace ui_design
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const byte Msb = 0x00;
        private readonly MifareCard card;
        private readonly IsoReader isoReader;
        private byte[] key = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};

        public MainWindow()
        {
            InitializeComponent();
        }

        private void dp1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            textBox1.Text = dp1.SelectedDate.Value.Year + "-" + dp1.SelectedDate.Value.Month + "-" +
                            dp1.SelectedDate.Value.Day;
        }
    }
}