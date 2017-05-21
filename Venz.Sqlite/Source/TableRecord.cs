using SQLite;
using System;

namespace Venz.Sqlite
{
    public class TableRecord
    {
        public Boolean IsInserted => DatabaseId != 0;

        [PrimaryKey, AutoIncrement]
        public Int32 DatabaseId { get; set; }
    }
}
