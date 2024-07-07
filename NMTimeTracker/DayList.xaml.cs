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

namespace NMTimeTracker
{
    /// <summary>
    /// Interaction logic for DayList.xaml
    /// </summary>
    public partial class DayList : System.Windows.Controls.UserControl //, INotifyPropertyChanged
    {
        //public event PropertyChangedEventHandler? PropertyChanged;

        public static readonly DependencyProperty DaysProperty =
            DependencyProperty.Register(nameof(Days), typeof(IEnumerable<DayModel>), typeof(DayList), new PropertyMetadata());

        public static readonly DependencyProperty SelectedDayProperty =
            DependencyProperty.Register(nameof(SelectedDay), typeof(DayModel), typeof(DayList), new PropertyMetadata());

        //public static readonly DependencyProperty SelectedDateProperty =
        //    DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime), typeof(DayList), new PropertyMetadata());

        //public static readonly DependencyProperty SelectedDayIndexProperty =
        //    DependencyProperty.Register(nameof(SelectedDayIndex), typeof(int), typeof(DayList), new PropertyMetadata());

        [System.ComponentModel.Bindable(true)]
        public IEnumerable<DayModel> Days
        {
            get { return (IEnumerable<DayModel>)GetValue(DaysProperty); }
            set { SetValue(DaysProperty, value); }
        }

        [System.ComponentModel.Bindable(true)]
        public DayModel SelectedDay
        {
            get { return (DayModel)GetValue(SelectedDayProperty); }
            set { SetValue(SelectedDayProperty, value); }
        }

        //[System.ComponentModel.Bindable(true, BindingDirection.TwoWay)]
        //public DateTime SelectedDate
        //{
        //    get { return (DateTime)GetValue(SelectedDateProperty); }
        //    set { SetValue(SelectedDateProperty, value); }
        //}

        //[System.ComponentModel.Bindable(true)]
        //public int SelectedDayIndex
        //{
        //    get => (int)GetValue(SelectedDayIndexProperty);
        //    set => SetValue(SelectedDayIndexProperty, value);
        //    //get
        //    //{
        //    //    var days = Days;
        //    //    if (days != null)
        //    //    {
        //    //        int index = 0;
        //    //        foreach (var day in days)
        //    //        {
        //    //            if (day.Date == SelectedDate)
        //    //            {
        //    //                return index;
        //    //            }
        //    //            ++index;
        //    //        }
        //    //    }
        //    //    return -1;
        //    //}
        //    //set
        //    //{
        //    //    int index = 0;
        //    //    foreach (var day in Days)
        //    //    {
        //    //        if (index == value)
        //    //        {
        //    //            SelectedDate = day.Date;
        //    //        }
        //    //        index++;
        //    //    }
        //    //    SelectedDate = Days.First().Date;
        //    //}
        //}


        public DayList()
        {
            InitializeComponent();
        }


        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    base.OnPropertyChanged(e);

        //    if (e.Property == SelectedDateProperty)
        //    {
        //        UpdateFromSelectedDate();
        //    }
        //    //else if (e.Property == SelectedDayIndexProperty)
        //    //{
        //    //    UpdateFromSelectedDayIndex();
        //    //}
        //}

        //private void UpdateFromSelectedDate()
        //{
        //    if (Days != null)
        //    {
        //        int index = 0;
        //        foreach (var day in Days)
        //        {
        //            if (day.Date == SelectedDate)
        //            {
        //                SelectedDayIndex = index;
        //                return;
        //            }
        //            ++index;
        //        }
        //    }
        //    SelectedDayIndex = -1;
        //}
        //        
        //private void UpdateFromSelectedDayIndex()
        //{
        //    if (Days != null)
        //    {
        //        int index = 0;
        //        foreach (var day in Days)
        //        {
        //            if (index == SelectedDayIndex)
        //            {
        //                SelectedDate = day.Date;
        //                return;
        //            }
        //            ++index;
        //        }
        //    }
        //}
    }
}
