using NMTimeTracker;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTrackerTests
{
    public class DataStoreUnitTest
    {
        private DataStore? m_store;

        [SetUp]
        public void Setup()
        {
            m_store = DataStore.Create("Data Source=:memory:");
        }
        
        [TearDown]
        public void TearDown()
        {
            if (m_store != null)
            {
                m_store.Dispose();
                m_store = null;
            }
        }


        [Test]
        public void CreateInterval()
        {
            Assert.NotNull(m_store);

            var date = new DateTime(2021, 1, 1);
            var day = m_store.GetDay(date);

            Assert.AreEqual(0, (int)day.Time.TotalMinutes);

            m_store.CreateInterval(
                new DateTime(2021, 1, 1, 1, 0, 0), TimeTrackerEvents.UserStart,
                new DateTime(2021, 1, 1, 2, 0, 0), TimeTrackerEvents.UserStop);

            Assert.AreEqual(60, (int)day.Time.TotalMinutes);

            m_store.CreateInterval(
                new DateTime(2021, 1, 1, 3, 0, 0), TimeTrackerEvents.UserStart,
                new DateTime(2021, 1, 1, 4, 0, 0), TimeTrackerEvents.UserStop);

            Assert.AreEqual(2 * 60, (int)day.Time.TotalMinutes);
        }

        [Test]
        public void CreateModifier()
        {
            Assert.NotNull(m_store);

            var date = new DateTime(2021, 1, 1);
            var day = m_store.GetDay(date);

            Assert.AreEqual(0, (int)day.Time.TotalMinutes);

            m_store.CreateModifier(date, new TimeSpan(1, 0, 0));

            Assert.AreEqual(60, (int)day.Time.TotalMinutes);

            m_store.CreateModifier(date, new TimeSpan(0, 30, 0));

            Assert.AreEqual(60 + 30, (int)day.Time.TotalMinutes);
        }

        [Test]
        public void UpdateModifier()
        {
            Assert.NotNull(m_store);

            var date = new DateTime(2021, 1, 1);
            var day = m_store.GetDay(date);

            Assert.AreEqual(0, (int)day.Time.TotalMinutes);
            
            var modifier = m_store.CreateModifier(date, new TimeSpan(1, 0, 0));
            
            Assert.AreEqual(60, (int)day.Time.TotalMinutes);

            modifier.Time = new TimeSpan(2, 0, 0);
            m_store.UpdateModifier(modifier);
            
            Assert.AreEqual(120, (int)day.Time.TotalMinutes);
        }

        [Test]
        public void UpdateModifier_Date()
        {
            Assert.NotNull(m_store);

            var date = new DateTime(2021, 1, 1);
            var day1 = m_store.GetDay(date);
            var day2 = m_store.GetDay(new DateTime(2021, 1, 2));

            Assert.AreEqual(0, (int)day1.Time.TotalMinutes);
            
            var modifier = m_store.CreateModifier(date, new TimeSpan(1, 0, 0));
            
            Assert.AreEqual(60, (int)day1.Time.TotalMinutes);
            Assert.AreEqual(0, (int)day2.Time.TotalMinutes);

            modifier.Date = day2.Date;
            m_store.UpdateModifier(modifier);

            Assert.AreEqual(0, (int)day1.Time.TotalMinutes);
            Assert.AreEqual(60, (int)day2.Time.TotalMinutes);
        }

        [Test]
        public void OverlappingInterval()
        {
            Assert.NotNull(m_store);

            var day1Date = new DateTime(2021, 1, 1);
            var day2Date = new DateTime(2021, 1, 2);
            var start = new DateTime(2021, 1, 1, 23, 0, 0);
            var end = new DateTime(2021, 1, 2, 1, 0, 0);

            var interval = m_store.CreateInterval(
                start, TimeTrackerEvents.UserStart, 
                end, TimeTrackerEvents.UserStop);

            var day1 = m_store.GetDay(day1Date);
            var day2 = m_store.GetDay(day2Date);
            
            Assert.AreEqual(1, day1.Intervals.Count);
            Assert.AreEqual(1, day2.Intervals.Count);
            Assert.AreEqual(60 * 60, (int)day1.Time.TotalSeconds);
            Assert.AreEqual(60 * 60, (int)day2.Time.TotalSeconds);
        }
    }
}
