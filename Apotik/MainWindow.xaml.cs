﻿using System;
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

namespace Apotik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TambahObat(object sender, RoutedEventArgs e) => MainFrame.Content = new views.TambahObat();
        private void DaftarObat(object sender, RoutedEventArgs e) => MainFrame.Content = new views.DaftarObat();
        private void DaftarResep(object sender, RoutedEventArgs e) => MainFrame.Content = new views.DaftarResep();
    }
}
