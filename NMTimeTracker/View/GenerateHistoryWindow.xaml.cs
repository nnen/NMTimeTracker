using NMTimeTracker.Model;
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

namespace NMTimeTracker.View
{
    /// <summary>
    /// Interaction logic for GenerateHistoryWindow.xaml
    /// </summary>
    public partial class GenerateHistoryWindow : Window
    {
        public GenerateHistoryWindow()
        {
            InitializeComponent();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is GenerateHistoryViewModel viewModel)
            {
                viewModel.Generate();
            }
        }
    }
}
