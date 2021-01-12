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
using System.Windows.Shapes;

namespace OSL.WPF.View
{
    /// <summary>
    /// Interaction logic for InfosDialog.xaml
    /// </summary>
    public partial class InfosDialog : Window
    {
        public InfosDialog()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
