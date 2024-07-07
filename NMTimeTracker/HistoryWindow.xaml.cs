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
    }
}
