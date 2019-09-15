using System.Collections.Generic;
using System.Data;

namespace NKMCore.Extensions
{
    public static class Sqlite
    {
        public static List<Dictionary<string, string>> Select(this IDbConnection conn, string query)
        {
            var rows = new List<Dictionary<string, string>>();

            conn.Open();
            IDbCommand dbcmd = conn.CreateCommand();
            dbcmd.CommandText = query;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                var row = new Dictionary<string, string>();
                int fieldCount = reader.FieldCount;
                for (int i = 0; i < fieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    string value = reader.GetValue(i).ToString();
                    row.Add(columnName, value);
                }

                rows.Add(row);
            }

            reader.Close();
            dbcmd.Dispose();
            conn.Close();
            return rows;
        }
    }
}