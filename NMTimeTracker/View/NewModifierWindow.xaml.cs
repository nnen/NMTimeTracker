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
    public partial class NewModifierWindow : Window, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register(nameof(Time), typeof(TimeSpan), typeof(NewModifierWindow), new PropertyMetadata());

        public event PropertyChangedEventHandler? PropertyChanged;


        private DateTime m_date = DateTime.Today;

        public DateTime Date
        {
            get => m_date;
            set
            {
                m_date = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Date)));
            }
        }


        [System.ComponentModel.Bindable(true)]
        public TimeSpan Time 
        {
            get => (TimeSpan)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }


        private string m_comment;

        public string Comment
        {
            get => m_comment;
            set
            {
                m_comment = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Comment)));
            }
        }


        public string Description
        {
            get
            {
                var time = Time;
                if (time.TotalSeconds < 0)
                {
                    time = time.Negate();
                    return $"Subtract {time.Hours} hours, {time.Minutes} minutes and {time.Seconds} seconds.";
                }
                return $"Add {time.Hours} hours, {time.Minutes} minutes and {time.Seconds} seconds.";
            }
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == TimeProperty)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Description)));
            }
            
            base.OnPropertyChanged(e);
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
            this.DialogResult = true;
            Close();
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            Date = DateTime.Today;
        }

        private void YesterdayButton_Click(object sender, RoutedEventArgs e)
        {
            Date = DateTime.Today - TimeSpan.FromDays(1);
        }
    }
}
