﻿using dokter.DBAccess;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace dokter.forms
{
    /// <summary>
    /// Interaction logic for PopupTindakan.xaml
    /// </summary>
    public partial class PopupTindakan : Window
    {
        private InputRekamMedis mw;
        private string kode = null;
        private string desk = null;

        SqlConnection conn;
        DBCommand cmd;

        public PopupTindakan(InputRekamMedis mw)
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
            {
                dtgTindakan.ItemsSource = cmd.GetDataTindakan();
            }
            else
            {
                dtgTindakan.ItemsSource = cmd.GetDataTindakan()
                    .Where(x => x.kode.Contains(src.ToUpper()) || x.desk.Contains(src));
            }
        }

        private void DtgTindakan_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            foreach (models.ModelTindakan mt in dtgTindakan.SelectedItems)
            {
                if (kode == null && desk == null)
                {
                    kode = mt.kode;
                    desk = mt.desk;
                }
                else
                {
                    kode += ";" + mt.kode;
                    desk += ";\n" + mt.desk;
                }
            }
        }

        private void TxtSrc_TextChanged(object sender, TextChangedEventArgs e)
        {
            var src = e.Source as TextBox;

            if (src.Text != "Kode ICD/Prosedur Tindakan" || string.IsNullOrEmpty(src.Text) ||
                string.IsNullOrWhiteSpace(src.Text))
            {
                LoadData(src.Text);
            }
        }

        private void BtnDOne_Click(object sender, RoutedEventArgs e)
        {
            mw.FillTindakan(kode + ";", desk + ";");
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TxtSrc_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            if (txtSrc.Text == "Kode ICD/Prosedur Tindakan" || !string.IsNullOrEmpty(txtSrc.Text) ||
                !string.IsNullOrWhiteSpace(txtSrc.Text))
            {
                txtSrc.Text = string.Empty;
            }
        }
    }
}