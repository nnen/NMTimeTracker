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

        public static T? FromSQLite<T>(object value)
        {
            if (value == DBNull.Value)
            {
                return default;
            }
            return (T)value;
        }


        public Interval CreateInterval(DateTime start, TimeTrackerEvents startReason)
        {
            return CreateInterval(start, startReason, start, TimeTrackerEvents.UnexpectedStop);
        }

        public Interval CreateInterval(DateTime start, TimeTrackerEvents startReason, DateTime end, TimeTrackerEvents endReason)
        {
            var sql = $"INSERT INTO IntervalsTable(Start, StartReason, End, EndReason) VALUES({ToSQLite(start)}, {(int)startReason}, {ToSQLite(end)}, {(int)endReason});";
            var cmd = m_connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            var interval = new Interval(m_connection.LastInsertRowId, start, startReason, end, endReason);

            foreach (var day in GetCachedDaysForInterval(interval))
            {
                day.OnIntervalAdded(interval);
            }

            return interval;
        }

        public void UpdateInterval(Interval interval)
        {
            var endReason = interval.EndReason;
            switch (endReason)
            {
            case TimeTrackerEvents.None:
            case TimeTrackerEvents.StillRunning:
                endReason = TimeTrackerEvents.UnexpectedStop;
                break;
            }
            
            var sql = $"UPDATE IntervalsTable SET Start={ToSQLite(interval.Start)}, StartReason={(int)interval.StartReason}, End={ToSQLite(interval.End)}, EndReason={(int)endReason} WHERE Id={interval.Id};";
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
            cmd.CommandText = $"SELECT * FROM {IntervalsTableName} WHERE date(Start) <= '{dateStr}' AND date(End) >= '{dateStr}' ORDER BY Start";
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


        private ModifierData GetModiferData(SQLiteDataReader reader)
        {
            var id = (long)reader["Id"];
            var date = (DateTime)reader["Date"];
            var timeSeconds = (long)reader["Time"];
            var comment = FromSQLite<string?>(reader["Comment"]);

            return new ModifierData()
            { 
                Id = id,
                Date = date,
                Time = TimeSpan.FromSeconds(timeSeconds),
                Comment = comment
            };
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
            var data = GetModiferData(reader);
            return new Modifier(data);
        }

        public void UpdateModifier(Modifier modifier)
        {
            var data = new ModifierData()
            {
                Id = modifier.Id,
                Date = modifier.Date,
                Time = modifier.Time,
                Comment = modifier.Comment,
            };
            UpdateModifier(in data);

            //var sql = $"UPDATE {ModifiersTableName} SET Date=@date, Time=@time, Comment=@comment WHERE Id=@id";
            //var cmd = new SQLiteCommand(sql, m_connection);
            //cmd.Parameters.AddWithValue("@date", modifier.Date.ToString("yyyy-MM-dd"));
            //cmd.Parameters.AddWithValue("@time", modifier.Time.TotalSeconds);
            //cmd.Parameters.AddWithValue("@comment", modifier.Comment);
            //cmd.Parameters.AddWithValue("@id", modifier.Id);
            //cmd.ExecuteNonQuery();

            //var day = GetCachedDay(modifier.Date);
            //if (day != null)
            //{
            //    day.InvalidateTime();
            //}
        }

        public bool UpdateModifier(in ModifierData data)
        {
            var transaction = m_connection.BeginTransaction();
            try
            {
                var oldData = GetModifier(data.Id);
                if (!oldData.HasValue)
                {
                    return false;
                }

                var sql = $"UPDATE {ModifiersTableName} SET Date=@date, Time=@time, Comment=@comment WHERE Id=@id";
                var cmd = new SQLiteCommand(sql, m_connection);
                cmd.Parameters.AddWithValue("@date", data.Date.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@time", data.Time.TotalSeconds);
                cmd.Parameters.AddWithValue("@comment", data.Comment);
                cmd.Parameters.AddWithValue("@id", data.Id);
                cmd.ExecuteNonQuery();

                if (oldData.Value.Date != data.Date)
                {
                    var day1 = GetCachedDay(oldData.Value.Date);
                    var day2 = GetCachedDay(data.Date);

                    if (day1 != null)
                    {
                        GetDay(day1.Date);
                    }

                    if (day2 != null)
                    {
                        GetDay(day2.Date);
                    }
                }
                else
                {
                    var day = GetCachedDay(data.Date);
                    if (day != null)
                    {
                        day.InvalidateTime();
                    }
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            return true;
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
        
        private ModifierData? GetModifier(long id)
        {
            var sql = $"SELECT * FROM {ModifiersTableName} WHERE Id=@id";
            var cmd = new SQLiteCommand(sql, m_connection);
            cmd.Parameters.AddWithValue("@id", id);
            
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return GetModiferData(reader);
            }
            return null;
        }
        
        public IEnumerable<Modifier> GetModifiersForDay(DateTime day) 
        {
            var dateStr = day.ToString("yyyy-MM-dd");
            var cmd = m_connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {ModifiersTableName} WHERE Date = '{dateStr}'";

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

        public void RefreshDay(DayModel day)
        {
            var intervals = GetIntervalsInDay(day.Date);
            var modifiers = GetModifiersForDay(day.Date);
            day.UpdateIntervals(intervals);
            day.UpdateModifiers(modifiers);
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

        public IEnumerable<DayModel> GetCachedDaysForInterval(Interval interval)
        {
            var startDate = interval.Start.Date;
            var endDate = interval.End.Date;

            var day = GetCachedDay(startDate);
            if (day != null)
            {
                yield return day;
            }

            if (startDate != endDate)
            {
                day = GetCachedDay(endDate);
                if (day != null)
                {
                    yield return day;
                }
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


        private static string? GetDatabaseDirectoryPath()
        {
            var appName = typeof(DataStore).Assembly.GetName().Name;
            if (appName == null)
            {
                return null;
            }
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dbPathDir = Path.Combine(appData, appName);
            return dbPathDir;
        }

        public static string? GetDatabaseFilePath()
        {
            var dbPathDir = GetDatabaseDirectoryPath();
            if (dbPathDir == null)
            {
                return null;
            }
            var dbPath = Path.Combine(dbPathDir, "data.sqlite");
            return dbPath;
        }

        public static DataStore? Create(string connectionString)
        {
            try
            {
                var connection = new SQLiteConnection(connectionString);
                connection.Open();
                var store = new DataStore(connection);
                store.CreateSchema();
                return store;
            }
            catch
            {
                return null;
            }
        }

        public static DataStore? Create() 
        {
            var dbPathDir = GetDatabaseDirectoryPath();
            if (dbPathDir == null)
            {
                return null;
            }

            Directory.CreateDirectory(dbPathDir);
            var dbPath = Path.Combine(dbPathDir, "data.sqlite");
            
            return Create($"Data Source={dbPath};New=True;Compress=True;");
        }
    }
}
