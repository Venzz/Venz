using System;

namespace Venz.Sqlite
{
    public class NonQuery
    {
        private DatabaseConnection Connection;
        private Type Table;
        private String Operation;
        private String Details;
        private String Conditions;

        public NonQuery(DatabaseConnection connection, String operation, Type table, String operationDetails, params Object[] args)
        {
            Connection = connection;
            Table = table;
            Operation = operation;
            Details = operationDetails;
        }

        public NonQuery(DatabaseConnection connection, String operation, Type table)
        {
            Connection = connection;
            Table = table;
            Operation = operation;
        }

        public NonQuery Where(String conditions, params Object[] args)
        {
            Conditions = String.Format(conditions, args);
            return this;
        }

        public Int32 Execute()
        {
            var operation = "";
            if (Operation == "DELETE")
                operation = String.Format("DELETE FROM {0}", Table.Name);
            else if (Operation == "INSERT")
                operation = String.Format("INSERT INTO {0} VALUES ({1})", Table.Name, Details);
            else if (Operation == "UPDATE")
                operation = String.Format("UPDATE {0} SET {1}", Table.Name, Details);

            if (Conditions != null)
                operation = String.Format("{0} WHERE {1}", operation, Conditions);
            return Connection.NonQuery(operation);
        }

        public enum OperationType { Insert, Update, Delete }
    }
}
