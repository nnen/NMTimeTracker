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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NMTimeTracker.View
{
    /// <summary>
    /// Interaction logic for TimeSpanControl.xaml
    /// </summary>
    public partial class TimeSpanControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(TimeSpan), typeof(TimeSpanControl), new PropertyMetadata());

        public event PropertyChangedEventHandler? PropertyChanged;

        [System.ComponentModel.Bindable(true)]
        public TimeSpan Value
        {
            get => (TimeSpan)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }


        public int Hours
        {
            get => Value.Hours;
            set
            {
                var oldSpan = Value;
                Value = new TimeSpan(value, oldSpan.Minutes, oldSpan.Seconds);
            }
        }


        public int Minutes
        {
            get => Value.Minutes;
            set
            {
                var oldSpan = Value;
                Value = new TimeSpan(oldSpan.Hours, value, oldSpan.Seconds);
            }
        }


        public int Seconds
        {
            get => Value.Seconds;
            set
            {
                var oldSpan = Value;
                Value = new TimeSpan(oldSpan.Hours, oldSpan.Minutes, value);
            }
        }


        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                NotifyPropertyChanged(nameof(Hours));
                NotifyPropertyChanged(nameof(Minutes));
                NotifyPropertyChanged(nameof(Seconds));
            }

            base.OnPropertyChanged(e);
        }


        public TimeSpanControl()
        {
            InitializeComponent();
        }

        private void AddTime(int hours, int minutes, int seconds)
        {
            Value += new TimeSpan(hours, minutes, seconds);
        }

        private void addHourButton_Click(object sender, RoutedEventArgs e)
        {
            AddTime(1, 0, 0);
        }

        private void subtractHourButton_Click(object sender, RoutedEventArgs e)
        {
            AddTime(-1, 0, 0);
        }

        private void addMinuteButton_Click(object sender, RoutedEventArgs e)
        {
            AddTime(0, 1, 0);
        }

        private void subtractMinuteButton_Click(object sender, RoutedEventArgs e)
        {
            AddTime(0, -1, 0);
        }

        private void addSecondButton_Click(object sender, RoutedEventArgs e)
        {
            AddTime(0, 0, 1);
        }

        private void subtractSecondButton_Click(object sender, RoutedEventArgs e)
        {
            AddTime(0, 0, -1);
        }
    }
}
