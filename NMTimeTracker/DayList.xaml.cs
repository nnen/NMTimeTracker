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
    public partial class DayList : System.Windows.Controls.UserControl
    {
        public static readonly DependencyProperty DaysProperty =
            DependencyProperty.Register(nameof(Days), typeof(IEnumerable<DayModel>), typeof(DayList), new PropertyMetadata());

        public static readonly DependencyProperty SelectedDayProperty =
            DependencyProperty.Register(nameof(SelectedDay), typeof(DayModel), typeof(DayList), new PropertyMetadata());

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

        public DayList()
        {
            InitializeComponent();
        }
    }
}
