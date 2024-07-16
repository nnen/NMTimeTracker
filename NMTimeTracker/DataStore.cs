using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMTimeTracker.Model;

namespace NMTimeTracker
{
    public class DataStore : IDisposable
    {
        private static readonly string IntervalsTableName = "IntervalsTable";
        private static readonly string ModifiersTableName = "ModifiersTable";


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
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {IntervalsTableName} (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Start DATETIME, " +
                "StartReason INTEGER, " +
                "End DATETIME, " + 
                "EndReason INTEGER" +
                ")";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {ModifiersTableName} (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "Date DATE, " +
                "Time INTEGER, " +
                "Comment TEXT" +
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
        
        public static string DateToSQLite(DateTime? value)
        {
            if (!value.HasValue)
            {
                return "NULL";
            }
            return $"'{value.Value.ToString("yyyy-MM-dd")}'";
        }

        public static T FromSQLite<T>(object value)
        {
            if (value == DBNull.Value)
            {
                return default(T);
            }
            return (T)value;
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
            var interval = new Interval(m_connection.LastInsertRowId, start, startReason, end, endReason);

            foreach (var day in GetDaysForInterval(interval))
            {
                day.OnIntervalAdded(interval);
            }

            return interval;
        }

        public void UpdateInterval(Interval interval)
        {
            var sql = $"UPDATE IntervalsTable SET Start={ToSQLite(interval.Start)}, StartReason={(int)interval.StartReason}, End={ToSQLite(interval.End)}, EndReason={(int)interval.EndReason} WHERE Id={interval.Id};";
            var cmd = m_connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();

            foreach (var day in GetDaysForInterval(interval))
            {
                day.InvalidateTime();
            }
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
            cmd.CommandText = $"SELECT * FROM {IntervalsTableName} WHERE date(Start) <= '{dateStr}' AND date(End) >= '{dateStr}'";
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                yield return CreateInterval(reader);
            }

            yield break;
        }

        public bool DeleteInterval(Interval interval)
        {
            var cmd = m_connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {IntervalsTableName} WHERE Id = {interval.Id}";
            bool removed = (cmd.ExecuteNonQuery() > 0);

            foreach (var day in GetDaysForInterval(interval))
            {
                day.OnIntervalDeleted(interval);
            }

            return removed;
        }


        public Modifier CreateModifier(DateTime date, TimeSpan time, string? comment = null)
        {
            var sql = $"INSERT INTO {ModifiersTableName}(Date, Time, Comment) VALUES (@date, @time, @comment);";
            var cmd = new SQLiteCommand(sql, m_connection);
            cmd.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@time", time.TotalSeconds);
            cmd.Parameters.AddWithValue("@comment", comment);
            cmd.ExecuteNonQuery();
            var id = m_connection.LastInsertRowId;
            var modifier = new Modifier(id, date.Date, time, comment);
            var day = GetCachedDay(date);
            if (day != null) 
            {
                day.OnModifierAdded(modifier);
            }
            return modifier;
        }

        private Modifier CreateModifier(SQLiteDataReader reader)
        {
            var id = (long)reader["Id"];
            var date = (DateTime)reader["Date"];
            var timeSeconds = (long)reader["Time"];
            var comment = FromSQLite<string?>(reader["Comment"]);
            
            return new Modifier(id, date, TimeSpan.FromSeconds(timeSeconds), comment);
        }

        public void UpdateModifier(Modifier modifier)
        {
            var sql = $"UPDATE {ModifiersTableName} SET Date=@date, Time=@time, Comment=@comment WHERE Id=@id";
            var cmd = new SQLiteCommand(sql, m_connection);
            cmd.Parameters.AddWithValue("@date", modifier.Date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@time", modifier.Time.TotalSeconds);
            cmd.Parameters.AddWithValue("@comment", modifier.Comment);
            cmd.Parameters.AddWithValue("@id", modifier.Id);
            cmd.ExecuteNonQuery();

            var day = GetCachedDay(modifier.Date);
            if (day != null)
            {
                day.InvalidateTime();
            }
        }

        public bool DeleteModifier(Modifier modifier)
        {
            var cmd = m_connection.CreateCommand();
            cmd.CommandText = $"DELETE FROM {ModifiersTableName} WHERE Id = {modifier.Id}";
            bool removed = (cmd.ExecuteNonQuery() > 0);

            var day = GetCachedDay(modifier.Date);
            if (day != null)
            {
                day.OnModifierDeleted(modifier);
            }

            return removed;
        }
        
        public IEnumerable<Modifier> GetModifiersForDay(DateTime day) 
        {
            var dateStr = day.ToString("yyyy-MM-dd");
            var cmd = m_connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {ModifiersTableName} WHERE Date = '{dateStr}'";

            //var sql = $"SELECT * FROM {ModifiersTableName} WHERE Date = @day";
            //var cmd = new SQLiteCommand(sql, m_connection);
            //cmd.Parameters.AddWithValue("@day", day.ToString("yyyy-MM-dd"));
            var reader = cmd.ExecuteReader();
            while (reader.Read()) 
            {
                yield return CreateModifier(reader);
            }
        }


        public DayModel? GetCachedDay(DateTime day)
        {
            var date = day.Date;
            if (m_days.TryGetValue(date, out var weakDay))
            {
                if (weakDay.TryGetTarget(out var existingModel))
                {
                    return existingModel;
                }
            }
            return null;
        }

        public DayModel GetDay(DateTime day)
        {
            var date = day.Date;
            DayModel? model = GetCachedDay(date);
            var intervals = GetIntervalsInDay(date);
            var modifiers = GetModifiersForDay(date);
            if (model == null)
            {
                model = new DayModel(date, intervals, modifiers);
                m_days[date] = new WeakReference<DayModel>(model);
            }
            else
            {
                model.UpdateIntervals(intervals);
                model.UpdateModifiers(modifiers);
            }
            return model;
        }

        public IEnumerable<DayModel> GetDaysForInterval(Interval interval)
        {
            var startDate = interval.Start.Date;
            var endDate = interval.End.Date;

            yield return GetDay(startDate);
            if (startDate != endDate)
            {
                yield return GetDay(endDate);
            }
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
