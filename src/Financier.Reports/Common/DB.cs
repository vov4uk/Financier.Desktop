using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace Financier.Reports.Common
{
    public static class DB
    {
        private static SqliteConnection _connection;
        private static List<TableInfo> _tablesInfo;

        public static List<TableInfo> TablesInfo => _tablesInfo ?? (_tablesInfo = LoadTablesInfo());

        public static SqliteConnection Connection
        {
            get
            {
                if (_connection == null)
                    _connection = CreateConnection();
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();
                return _connection;
            }
        }

        private static SqliteConnection CreateConnection() => new SqliteConnection("Data Source= " + ExSettings.DbPath + ";Version=3;");

        public static void ExecuteNonQuery(string SqlCommand)
        {
            using (SqliteCommand SqliteCommand = new SqliteCommand())
            {
                SqliteCommand.Connection = Connection;
                SqliteCommand.CommandText = SqlCommand;
                SqliteCommand.CommandType = CommandType.Text;
                SqliteCommand.ExecuteNonQuery();
            }
        }

        private static List<TableInfo> LoadTablesInfo()
        {
            List<TableInfo> tableInfoList = new List<TableInfo>();
            using (SqliteCommand SqliteCommand1 = new SqliteCommand("SELECT name FROM Sqlite_master WHERE type='table'", Connection))
            {
                using (SqliteCommand SqliteCommand2 = new SqliteCommand())
                {
                    SqliteCommand2.Connection = Connection;
                    SqliteDataReader SqliteDataReader1 = SqliteCommand1.ExecuteReader();
                    while (SqliteDataReader1.Read())
                    {
                        string tableName = SqliteDataReader1.GetString(0);
                        List<ColumnInfo> columnsInfo = new List<ColumnInfo>();
                        SqliteCommand2.CommandText = string.Format("PRAGMA table_info([{0}])", tableName);
                        SqliteDataReader SqliteDataReader2 = SqliteCommand2.ExecuteReader();
                        while (SqliteDataReader2.Read())
                            columnsInfo.Add(new ColumnInfo(SqliteDataReader2.GetString(1), SqliteDataReader2.GetString(2)));
                        SqliteDataReader2.Close();
                        tableInfoList.Add(new TableInfo(tableName, columnsInfo));
                    }
                    SqliteDataReader1.Close();
                }
            }
            return tableInfoList;
        }

        public static void GetData<T>(string SqlText, ObservableCollection<T> data) where T : BaseReportM, new()
        {
            data.Clear();
            using (SqliteCommand SqliteCommand = new SqliteCommand(SqlText, Connection))
            {
                SqliteDataReader reader = SqliteCommand.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        T obj = new T();
                        obj.Init(reader);
                        data.Add(obj);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        public static void TruncateTables()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TableInfo tableInfo in TablesInfo)
                stringBuilder.AppendFormat("delete from [{0}];", tableInfo.TableName);
            ExecuteNonQuery(stringBuilder.ToString());
        }
    }
}