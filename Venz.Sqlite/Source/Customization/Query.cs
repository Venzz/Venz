using System;
using System.Collections.Generic;

namespace Venz.Sqlite
{
    public class Query<T> where T: new()
    {
        private DatabaseConnection Connection;
        private Type Table;
        private String Details;
        private String Conditions;
        private String OrderingField;
        private UInt32? LimitValue;
        private String Options = "";

        public Query(DatabaseConnection connection, Type table, String operationDetails)
        {
            Connection = connection;
            Table = table;
            Details = operationDetails;
        }

        public Query<T> Where(String conditions)
        {
            Conditions = conditions;
            return this;
        }

        public Query<T> OrderBy(String field, Boolean descending = false)
        {
            OrderingField = field;
            if (descending)
                OrderingField += " DESC";
            return this;
        }

        public Query<T> OrderBy(String field1, String field2, Boolean descending = false)
        {
            OrderingField = field1;
            if (descending)
                OrderingField += " DESC";
            OrderingField += $", {field2}";
            if (descending)
                OrderingField += " DESC";
            return this;
        }

        public Query<T> Limit(UInt32 value)
        {
            LimitValue = value;
            return this;
        }

        public Query<T> NoCase()
        {
            Options += " COLLATE NOCASE";
            return this;
        }

        public IReadOnlyCollection<T> Execute()
        {
            var operation = $"SELECT {Details} FROM {Table.Name}";
            if (Conditions != null)
                operation = $"{operation} WHERE {Conditions}";
            if (OrderingField != null)
                operation = $"{operation} ORDER BY {OrderingField}";
            if (LimitValue.HasValue)
                operation = $"{operation} LIMIT {LimitValue.Value}";
            operation += Options;
            return Connection.Query<T>(operation);
        }
    }
}
