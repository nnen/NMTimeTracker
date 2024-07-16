using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        private int m_notificationSuspensionCounter = 0;
        private bool m_notificationsPending = false;


        public ObservableCollectionEx()
        {
        }

        public ObservableCollectionEx(IEnumerable<T> collection) : base(collection)
        {
        }

        public ObservableCollectionEx(List<T> list) : base(list)
        {
        }


        public void AddRange(IEnumerable<T> values)
        {
            SuppressNotifications();
            try
            {
                foreach (var item in values)
                {
                    Add(item);
                }
            }
            finally
            {
                ResumeNotifications();
            }
        }


        public bool AreNotificationsSuspended => m_notificationSuspensionCounter > 0;

        public void SuppressNotifications()
        {
            ++m_notificationSuspensionCounter;
        }

        public void ResumeNotifications()
        {
            Debug.Assert(m_notificationSuspensionCounter > 0);
            if (m_notificationSuspensionCounter <= 0)
            {
                return;
            }

            --m_notificationSuspensionCounter;

            if (m_notificationSuspensionCounter == 0)
            {
                if (m_notificationsPending)
                {
                    m_notificationsPending = false;
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (AreNotificationsSuspended)
            {
                m_notificationsPending = true;
                return;
            }
            base.OnCollectionChanged(e);
        }
    }
}
