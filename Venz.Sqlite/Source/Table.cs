using System;
using System.Collections.Generic;

namespace Venz.Sqlite
{
    public class Table<T> where T: TableRecord, new()
    {
        private Boolean IsExist;
        protected DatabaseConnection Connection;

        protected Table(DatabaseConnection connection)
        {
            Connection = connection;
        }

        public TResult ExecuteScalar<TResult>(String command)
        {
            EnsureTableExists();
            return Connection.ExecuteScalar<TResult>(command);
        }

        public void RunInTransaction(Action action)
        {
            Connection.RunInTransaction(action);
        }

        public void Upsert(T record) => Upsert(new T[] { record });

        public void Upsert(IEnumerable<T> records)
        {
            EnsureTableExists();
            foreach (var record in records)
            {
                if (record.IsInserted)
                    Connection.Update(record);
                else
                    Connection.Insert(record);
            }
        }

        public void Wipe() => Delete().Execute();



        protected Query<T> Select(String details)
        {
            EnsureTableExists();
            return new Query<T>(Connection, typeof(T), details);
        }

        protected NonQuery Insert(String details)
        {
            EnsureTableExists();
            return new NonQuery(Connection, "INSERT", typeof(T), details);
        }

        protected NonQuery Update(String details)
        {
            EnsureTableExists();
            return new NonQuery(Connection, "UPDATE", typeof(T), details);
        }

        protected NonQuery Delete()
        {
            EnsureTableExists();
            return new NonQuery(Connection, "DELETE", typeof(T));
        }

        private void EnsureTableExists()
        {
            if (!IsExist)
            {
                Connection.CreateTable<T>();
                IsExist = true;
            }
        }
    }
}
