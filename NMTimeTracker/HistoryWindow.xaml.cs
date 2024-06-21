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
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : WindowBase
    {
        //public static readonly DependencyProperty TrackerProperty =
        //    DependencyProperty.Register(nameof(Tracker), typeof(TimeTracker), typeof(HistoryWindow), new PropertyMetadata(null));
        //public static readonly DependencyProperty SelectedDateProperty =
        //    DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime), typeof(HistoryWindow), new PropertyMetadata(null));

        //public static readonly DependencyProperty SelectedDayProperty =
        //    DependencyProperty.Register(nameof(SelectedDay), typeof(DayModel), typeof(HistoryWindow), new PropertyMetadata());
        //public static readonly DependencyProperty SelectedWeekProperty =
        //    DependencyProperty.Register(nameof(SelectedWeek), typeof(WeekModel), typeof(HistoryWindow), new PropertyMetadata());

        private DateTime m_selectedDate;
        private DayModel m_selectedDay;
        private WeekModel m_selectedWeek;

        public TimeTracker? Tracker { get; set; }
        
        public DateTime SelectedDate
        {
            get => m_selectedDate;
            set
            {
                if (SetProperty(ref m_selectedDate, value, nameof(SelectedDate)))
                {
                    if (Tracker != null)
                    {
                        SelectedWeek = App.Current.Store.GetWeek(m_selectedDate, DayOfWeek.Monday);
                        SelectedDay = App.Current.Store.GetDay(m_selectedDate);
                    }
                }
            }
        }

        public DayModel SelectedDay 
        { 
            get => m_selectedDay; 
            set
            {
                SetProperty(ref m_selectedDay, value, nameof(SelectedDay));
            }
        }

        public WeekModel SelectedWeek 
        { 
            get => m_selectedWeek; 
            set
            {
                SetProperty(ref m_selectedWeek, value, nameof(SelectedWeek));
            }
        }


        public HistoryWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedDate = DateTime.Today;
        }
    }
}
