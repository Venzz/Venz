using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Venz.Sqlite
{
	public class DatabaseConnection
	{
		SQLiteConnectionString _connectionString;
        SQLiteOpenFlags _openFlags;
        SQLiteConnectionWithLock conn;

        public DatabaseConnection(string databasePath, bool storeDateTimeAsTicks = false): this(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks)
        {
        }

        public DatabaseConnection(string databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = false)
        {
            _openFlags = openFlags;
            _connectionString = new SQLiteConnectionString(databasePath, storeDateTimeAsTicks);
        }

		SQLiteConnectionWithLock GetConnection()
		{
            if (conn == null)
                conn = SQLiteConnectionPool.Shared.GetConnection(_connectionString, _openFlags);
            return conn;
		}

		public Int32 CreateTable<T>() where T: new ()
		{
            var conn = GetConnection();
            using (conn.Lock())
                return conn.CreateTable(typeof(T));
		}

		public Int32 Insert(Object item)
		{
            var conn = GetConnection();
            using (conn.Lock())
                return conn.Insert(item);
		}

		public Int32 Update(Object item)
		{
            var conn = GetConnection();
            using (conn.Lock())
                return conn.Update(item);
		}

		public Int32 Delete(Object item)
		{
            var conn = GetConnection();
            using (conn.Lock())
                return conn.Delete(item);
		}

		public List<T> Query<T>(String query, params Object[] args) where T: new ()
		{
            var conn = GetConnection();
            using (conn.Lock())
                return conn.Query<T>(query, args);
		}

        public Int32 NonQuery(String command)
        {
            var conn = GetConnection();
            using (conn.Lock())
                return conn.Execute(command);
        }

        public T ExecuteScalar<T>(String command)
        {
            var conn = GetConnection();
            using (conn.Lock())
                return conn.ExecuteScalar<T>(command);
        }

        public void RunInTransaction(Action action)
        {
            var conn = GetConnection();
            using (conn.Lock())
            {
                conn.BeginTransaction();
                try
                {
                    action();
                    conn.Commit();
                }
                catch (Exception)
                {
                    conn.Rollback();
                    throw;
                }
            }
        }

        public T RunInTransaction<T>(Func<T> action)
        {
            if (typeof(T) == typeof(Task))
            {
                Debug.WriteLine("RunInTransaction doesn't allow passing Tasks as an argument.");
                throw new ArgumentException("RunInTransaction doesn't allow passing Tasks as an argument.");
            }

            var conn = GetConnection();
            using (conn.Lock())
            {
                conn.BeginTransaction();
                try
                {
                    var result = action();
                    conn.Commit();
                    return result;
                }
                catch (Exception)
                {
                    conn.Rollback();
                    throw;
                }
            }
        }

        public Int32 GetVersion()
        {
            var conn = GetConnection();
            using (conn.Lock())
            {
                var versionRecords = conn.Query<VersionTable>("pragma user_version");
                return (versionRecords.Count > 0) ? versionRecords[0].user_version : 0;
            }
        }

        public void SetVersion(Int32 version)
        {
            var conn = GetConnection();
            using (conn.Lock())
                conn.Execute($"pragma user_version = {version}");
        }

        public void Dispose()
        {
            var conn = GetConnection();
            conn.Dispose();
        }

        private class VersionTable
        {
            public Int32 user_version { get; set; }
        }

        private class SQLiteConnectionPool
        {
            class Entry
            {
                public SQLiteConnectionString ConnectionString { get; private set; }
                public SQLiteConnectionWithLock Connection { get; private set; }

                public Entry(SQLiteConnectionString connectionString, SQLiteOpenFlags openFlags)
                {
                    ConnectionString = connectionString;
                    Connection = new SQLiteConnectionWithLock(connectionString, openFlags);
                }

                public void OnApplicationSuspended()
                {
                    Connection.Dispose();
                    Connection = null;
                }
            }

            readonly Dictionary<string, Entry> _entries = new Dictionary<string, Entry>();
            readonly object _entriesLock = new object();

            static readonly SQLiteConnectionPool _shared = new SQLiteConnectionPool();

            /// <summary>
            /// Gets the singleton instance of the connection tool.
            /// </summary>
            public static SQLiteConnectionPool Shared
            {
                get
                {
                    return _shared;
                }
            }

            public SQLiteConnectionWithLock GetConnection(SQLiteConnectionString connectionString, SQLiteOpenFlags openFlags)
            {
                lock (_entriesLock)
                {
                    Entry entry;
                    string key = connectionString.ConnectionString;

                    if (!_entries.TryGetValue(key, out entry))
                    {
                        entry = new Entry(connectionString, openFlags);
                        _entries[key] = entry;
                    }

                    return entry.Connection;
                }
            }

            /// <summary>
            /// Closes all connections managed by this pool.
            /// </summary>
            public void Reset()
            {
                lock (_entriesLock)
                {
                    foreach (var entry in _entries.Values)
                    {
                        entry.OnApplicationSuspended();
                    }
                    _entries.Clear();
                }
            }

            /// <summary>
            /// Call this method when the application is suspended.
            /// </summary>
            /// <remarks>Behaviour here is to close any open connections.</remarks>
            public void ApplicationSuspended()
            {
                Reset();
            }
        }

        private class SQLiteConnectionWithLock: SQLiteConnection
        {
            readonly object _lockPoint = new object();

            public SQLiteConnectionWithLock(SQLiteConnectionString connectionString, SQLiteOpenFlags openFlags): base(connectionString.DatabasePath, openFlags, connectionString.StoreDateTimeAsTicks)
            {
            }

            public IDisposable Lock()
            {
                return new LockWrapper(_lockPoint);
            }

            private class LockWrapper : IDisposable
            {
                object _lockPoint;

                public LockWrapper(object lockPoint)
                {
                    _lockPoint = lockPoint;
                    Monitor.Enter(_lockPoint);
                }

                public void Dispose()
                {
                    Monitor.Exit(_lockPoint);
                }
            }
        }
	}
}

