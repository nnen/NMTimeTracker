using NMTimeTracker.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for NewModifierWindow.xaml
    /// </summary>
    public partial class NewModifierWindow : Window
    {
        public Modifier? Modifier
        {
            get
            {
                if (DataContext is Model.ModifierViewModel vm)
                {
                    return vm.Modifier;
                }
                return null;
            }
            set
            {
                if (DataContext is Model.ModifierViewModel vm)
                {
                    vm.Modifier = value;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        public NewModifierWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Model.ModifierViewModel vm)
            {
                vm.AddModifier();
            }
            
            this.DialogResult = true;
            Close();
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Model.ModifierViewModel vm)
            {
                vm.Date = DateTime.Today;
            }
        }

        private void YesterdayButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Model.ModifierViewModel vm)
            {
                vm.Date = DateTime.Today - TimeSpan.FromDays(1);
            }
        }

        private void Add8HoursButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Model.ModifierViewModel vm)
            {
                vm.Time += new TimeSpan(8, 0, 0);
            }
        }

        private void Add4HoursButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Model.ModifierViewModel vm)
            {
                vm.Time += new TimeSpan(4, 0, 0);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Model.ModifierViewModel vm)
            {
                vm.ApplyChanges();
            }

            this.DialogResult = true;
            Close();
        }
    }
}
