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

namespace SimpleAppUI.Pages
{
    /// <summary>
    /// Interaction logic for CustomYesNoDialog.xaml
    /// </summary>
    public partial class CustomYesNoDialog : Window
    {
        public string DialogTitle { get; set; }
        public string DialogQuestion { get; set; }

        public CustomYesNoDialog(string title, string question)
        {
            InitializeComponent();
            DialogTitle = title;
            DialogQuestion = question;
            DataContext = this; // Bind the properties to the UI
        }

        private void OnYesClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnNoClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
