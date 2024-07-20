using NMTimeTracker.Model;
using NMTimeTracker.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NMTimeTracker
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : WindowBase
    {
        private void SetToToday()
        {
            if (DataContext is HistoryViewModel viewModel)
            {
                viewModel.SelectedDate = DateTime.Today;
            }
        }


        public HistoryWindow()
        {
            InitializeComponent();
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            SetToToday();
        }
        
        private void WindowBase_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                SetToToday();
            }
        }

        private void Calendar_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UIElement originalElement = e.OriginalSource as UIElement;
            if ((originalElement is CalendarDayButton) || (originalElement is CalendarItem))
            {
                originalElement.ReleaseMouseCapture();
            }
        }


        private void DeleteSelectedIntervals()
        {
            if (DataContext is HistoryViewModel viewModel)
            {
                int count = IntervalsDataGrid.SelectedItems.Count;
                if (System.Windows.MessageBox.Show($"Do you really want to delete {count} interval(s)?", "Delete Interval", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var intervals = IntervalsDataGrid.SelectedItems.Cast<Interval>().ToArray();
                    viewModel.RemoveIntervals(intervals);
                }
            }
        }

        private void DeleteSelectedModifiers()
        {
            if (DataContext is HistoryViewModel viewModel)
            {
                int count = ModifiersDataGrid.SelectedItems.Count;
                if (System.Windows.MessageBox.Show($"Do you really want to delete {count} modifier(s)?", "Delete Modifier", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var modifiers = ModifiersDataGrid.SelectedItems.Cast<Modifier>().ToArray();
                    viewModel.RemoveModifiers(modifiers);
                }
            }
        }


        private void DeleteInterval_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedIntervals();
        }

        private void DeleteModifier_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedModifiers();
        }

        private void IntervalsDataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteSelectedIntervals();
            }
        }

        private void ModifiersDataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteSelectedModifiers();
            }
        }

        private void ModifiersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                if (row.Item is Modifier modifier)
                {
                    var window = new NewModifierWindow();
                    window.Modifier = modifier;
                    window.ShowDialog();
                }
            }
        }
    }
}
