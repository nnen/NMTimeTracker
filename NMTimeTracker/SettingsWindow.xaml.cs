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

namespace NMTimeTracker
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public Settings Settings { get; set; }

        public int SelectedFirstDayOfWeek
        {
            get => (int)Settings.FirstDayOfWeek;
            set => Settings.FirstDayOfWeek = (DayOfWeek)value;
        }

        public SettingsWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Settings.Save();
            Close();
        }
    }
}
