using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace fcrd
{
    public static class DB
    {
        private static SQLiteConnection _connection;
        private static List<TableInfo> _tablesInfo;

        public static List<TableInfo> TablesInfo => DB._tablesInfo ?? (DB._tablesInfo = DB.LoadTablesInfo());

        public static SQLiteConnection Connection
        {
            get
            {
                if (DB._connection == null)
                    DB._connection = DB.CreateConnection();
                if (DB._connection.State != ConnectionState.Open)
                    DB._connection.Open();
                return DB._connection;
            }
        }

        private static SQLiteConnection CreateConnection() => new SQLiteConnection("Data Source= " + ExSettings.DbPath + ";Version=3;");

        public static void ExecuteNonQuery(string sqlCommand)
        {
            using (SQLiteCommand sqLiteCommand = new SQLiteCommand(DB.Connection))
            {
                sqLiteCommand.CommandText = sqlCommand;
                sqLiteCommand.CommandType = CommandType.Text;
                sqLiteCommand.ExecuteNonQuery();
            }
        }

        private static List<TableInfo> LoadTablesInfo()
        {
            List<TableInfo> tableInfoList = new List<TableInfo>();
            using (SQLiteCommand sqLiteCommand1 = new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", DB.Connection))
            {
                using (SQLiteCommand sqLiteCommand2 = new SQLiteCommand(DB.Connection))
                {
                    SQLiteDataReader sqLiteDataReader1 = sqLiteCommand1.ExecuteReader();
                    while (sqLiteDataReader1.Read())
                    {
                        string tableName = sqLiteDataReader1.GetString(0);
                        List<ColumnInfo> columnsInfo = new List<ColumnInfo>();
                        sqLiteCommand2.CommandText = string.Format("PRAGMA table_info([{0}])", tableName);
                        SQLiteDataReader sqLiteDataReader2 = sqLiteCommand2.ExecuteReader();
                        while (sqLiteDataReader2.Read())
                            columnsInfo.Add(new ColumnInfo(sqLiteDataReader2.GetString(1), sqLiteDataReader2.GetString(2)));
                        sqLiteDataReader2.Close();
                        tableInfoList.Add(new TableInfo(tableName, columnsInfo));
                    }
                    sqLiteDataReader1.Close();
                }
            }
            return tableInfoList;
        }

        public static void GetData<T>(string sqlText, ObservableCollection<T> data) where T : BaseReportM, new()
        {
            data.Clear();
            using (SQLiteCommand sqLiteCommand = new SQLiteCommand(sqlText, DB.Connection))
            {
                SQLiteDataReader reader = sqLiteCommand.ExecuteReader();
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
            foreach (TableInfo tableInfo in DB.TablesInfo)
                stringBuilder.AppendFormat("delete from [{0}];", tableInfo.TableName);
            DB.ExecuteNonQuery(stringBuilder.ToString());
        }
    }
}