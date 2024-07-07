using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMTimeTracker
{
    public class DataStore : IDisposable
    {
        private readonly SQLiteConnection m_connection;

        private readonly Dictionary<DateTime, WeakReference<DayModel>> m_days = new Dictionary<DateTime, WeakReference<DayModel>>();


        private DataStore(SQLiteConnection connection)
        {
            m_connection = connection;
        }

        public void Dispose()
        {
            m_connection.Close();
        }

        private void CreateSchema()
        {
            var cmd = m_connection.CreateCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS IntervalsTable (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Start DATETIME, " +
                "StartReason INTEGER, " +
                "End DATETIME, " + 
                "EndReason INTEGER" +
                ")";
            cmd.ExecuteNonQuery();
        }


        public static string ToSQLite(DateTime? value)
        {
            if (!value.HasValue)
            {
                return "NULL";
            }
            return $"'{value.Value.ToString("yyyy-MM-dd HH:mm:ss")}'";
        }


        public Interval CreateInterval(DateTime start, TimeTrackerEvents startReason)
        {
            return CreateInterval(start, startReason, start, TimeTrackerEvents.None);
        }

        public Interval CreateInterval(DateTime start, TimeTrackerEvents startReason, DateTime end, TimeTrackerEvents endReason)
        {
            var sql = $"INSERT INTO IntervalsTable(Start, StartReason, End, EndReason) VALUES({ToSQLite(start)}, {(int)startReason}, {ToSQLite(end)}, {(int)endReason});";
            var cmd = m_connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            return new Interval(m_connection.LastInsertRowId, start, startReason, end, endReason);
        }

        public void UpdateInterval(Interval interval)
        {
            var sql = $"UPDATE IntervalsTable SET Start={ToSQLite(interval.Start)}, StartReason={(int)interval.StartReason}, End={ToSQLite(interval.End)}, EndReason={(int)interval.EndReason} WHERE Id={interval.Id};";
            var cmd = m_connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        private Interval CreateInterval(SQLiteDataReader reader)
        {
            var id = (long)reader["Id"];
            var start = (DateTime)reader["Start"];
            var startReason = (TimeTrackerEvents)(long)reader["StartReason"];
            var end = (DateTime)reader["End"];
            var endReason = (TimeTrackerEvents)(long)reader["EndReason"];

            return new Interval(id, start, startReason, end, endReason);
        }

        public IEnumerable<Interval> GetIntervalsInDay(DateTime day)
        {
            var dateStr = day.ToString("yyyy-MM-dd");
            var cmd = m_connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM IntervalsTable WHERE date(Start) <= '{dateStr}' AND date(End) >= '{dateStr}'";
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return CreateInterval(reader);
            }

            yield break;
        }

        public DayModel GetDay(DateTime day)
        {
            var date = day.Date;
            DayModel? model = null;
            if (m_days.TryGetValue(date, out var weakDay))
            {
                if (weakDay.TryGetTarget(out var existingModel))
                {
                    model = existingModel;
                }
            }
            var intervals = GetIntervalsInDay(date);
            if (model == null)
            {
                model = new DayModel(date, intervals);
                m_days[date] = new WeakReference<DayModel>(model);
            }
            else
            {
                model.UpdateIntervals(intervals);
            }
            return model;
        }
        
        public WeekModel GetWeek(DateTime aWeekDay, DayOfWeek startOfWeek)
        {
            var firstDay = Utils.GetStartOfWeek(aWeekDay, startOfWeek);
            var currentDay = firstDay;
            var days = new DayModel[7];

            for (int i = 0; i < 7; ++i)
            {
                days[i] = GetDay(currentDay);
                currentDay = currentDay.AddDays(1);
            }
            
            return new WeekModel(days);
        }


        public static DataStore Create() 
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appName = typeof(DataStore).Assembly.GetName().Name;
            var dbPathDir = Path.Combine(appData, appName);
            Directory.CreateDirectory(dbPathDir);
            var dbPath = Path.Combine(dbPathDir, "data.sqlite");

            try
            {
                var connection = new SQLiteConnection($"Data Source={dbPath};New=True;Compress=True;");
                connection.Open();
                var store = new DataStore(connection);
                store.CreateSchema();
                return store;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
