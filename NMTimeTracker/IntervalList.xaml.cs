using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NMTimeTracker.Model;

namespace NMTimeTracker
{
    /// <summary>
    /// Interaction logic for DayHistory.xaml
    /// </summary>
    public partial class IntervalList : System.Windows.Controls.UserControl, INotifyPropertyChanged 
    {
        public static readonly DependencyProperty IntervalsProperty =
            DependencyProperty.Register(nameof(Intervals), typeof(IEnumerable<Interval>), typeof(IntervalList), new PropertyMetadata());
        
        public static readonly DependencyProperty SelectedIntervalProperty =
            DependencyProperty.Register(nameof(SelectedInterval), typeof(Interval), typeof(IntervalList), new PropertyMetadata());

        [System.ComponentModel.Bindable(true)]
        public IEnumerable<Interval> Intervals
        {
            get { return (IEnumerable<Interval>)GetValue(IntervalsProperty); }
            set { SetValue(IntervalsProperty, value); }
        }

        [System.ComponentModel.Bindable(true)]
        public Interval SelectedInterval
        {
            get { return (Interval)GetValue(SelectedIntervalProperty); }
            set { SetValue(SelectedIntervalProperty, value); }
        }

        
        public IntervalList()
        {
            InitializeComponent();
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void MenuRemoveInterval_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
