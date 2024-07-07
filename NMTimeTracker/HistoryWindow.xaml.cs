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
        //public static readonly DependencyProperty TrackerProperty =
        //    DependencyProperty.Register(nameof(Tracker), typeof(TimeTracker), typeof(HistoryWindow), new PropertyMetadata(null));
        //public static readonly DependencyProperty SelectedDateProperty =
        //    DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime), typeof(HistoryWindow), new PropertyMetadata());

        //public static readonly DependencyProperty SelectedDayProperty =
        //    DependencyProperty.Register(nameof(SelectedDay), typeof(DayModel), typeof(HistoryWindow), new PropertyMetadata());
        //public static readonly DependencyProperty SelectedWeekProperty =
        //    DependencyProperty.Register(nameof(SelectedWeek), typeof(WeekModel), typeof(HistoryWindow), new PropertyMetadata());

        //private DateTime m_selectedDate;
        //private DayModel m_selectedDay;
        //private WeekModel m_selectedWeek;

        //public TimeTracker? Tracker { get; set; }
        
        //[System.ComponentModel.Bindable(true, System.ComponentModel.BindingDirection.TwoWay)]
        //public DateTime SelectedDate
        //{
        //    get => (DateTime)GetValue(SelectedDateProperty);
        //    set => SetValue(SelectedDateProperty, value);
        //    //get => m_selectedDate;
        //    //set
        //    //{
        //    //    if (SetProperty(ref m_selectedDate, value, nameof(SelectedDate)))
        //    //    {
        //    //        UpdateFromSelectedDate();
        //    //    }
        //    //}
        //}

        //public DayModel SelectedDay 
        //{ 
        //    get => m_selectedDay; 
        //    set//////
        //    {
        //        SetProperty(ref m_selectedDay, value, nameof(SelectedDay));
        //    }
        //}

        //public int SelectedDayIndex
        //{
        //    get//////////////////////////
        //    {
        //        if (SelectedWeek != null)
        //        {
        //            for (int i = 0; i < SelectedWeek.Days.Count; i++)
        //            {
        //                if (SelectedWeek.Days[i].Date == SelectedDate)
        //                {
        //                    return i;
        //                }
        //            }
        //        }
        //        return -1;
        //    }
        //    set////////////////
        //    {
        //        if ((SelectedWeek != null) && (value >= 0) && (value < SelectedWeek.Days.Count))
        //        {
        //            SelectedDate = SelectedWeek.Days[value].Date;
        //            return;
        //        }
        //        SelectedDate = DateTime.Today;
        //    }
        //}

        //public WeekModel SelectedWeek 
        //{ 
        //    get => m_selectedWeek; 
        //    set//////
        //    {
        //        SetProperty(ref m_selectedWeek, value, nameof(SelectedWeek));
        //    }
        //}


        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    base.OnPropertyChanged(e);

        //    if (e.Property == SelectedDateProperty)
        //    {
        //        UpdateFromSelectedDate();
        //    }
        //}

        //private void UpdateFromSelectedDate()
        //{
        //    if (Tracker != null)
        //    {
        //        var app = App.Current;
        //        var store = app.Store;
        //        var selectedDate = SelectedDate;
        //        SelectedWeek = store.GetWeek(selectedDate, app.Settings.FirstDayOfWeek);
        //        SelectedDay = store.GetDay(selectedDate);
        //    }
        //}

        private void SetToToday()
        {
            if (DataContext is HistoryViewModel viewModel)
            {
                viewModel.SelectedDate = DateTime.Today;
            }
            //SelectedDate = DateTime.Today;
        }


        public HistoryWindow()
        {
            InitializeComponent();

            //var viewModel = new HistoryViewModel();
            //DataContext = viewModel;
            //dayList.DataContext = viewModel;
            //intervalList.DataContext = viewModel;
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
