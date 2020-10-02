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

namespace LauncherBeta1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            requestTextBox.Focus();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Close();
        }

        private void requestTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            suggestionsListBox.ScrollIntoView(0);
            switch (requestTextBox.Text)
            {
                case "":
                    Height = 40;
                    break;
                default:
                    Height = 440;
                    launcherViewModel.ProcessRequest(requestTextBox.Text);
                    break;
            }
        }
    }
}
