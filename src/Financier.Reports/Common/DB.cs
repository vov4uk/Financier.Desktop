using System.Collections.ObjectModel;
using System.Data;

namespace Financier.Reports.Common
{
    public static class DB
    {
        //private static SqliteConnection _connection;

        //public static SqliteConnection Connection
        //{
        //    get
        //    {
        //        if (_connection == null)
        //            _connection = CreateConnection();
        //        if (_connection.State != ConnectionState.Open)
        //            _connection.Open();
        //        return _connection;
        //    }
        //}

      //  private static SqliteConnection CreateConnection() => new SqliteConnection("");

        public static void ExecuteNonQuery(string SqlCommand)
        {
            //using (SqliteCommand SqliteCommand = new SqliteCommand())
            //{
            //    SqliteCommand.Connection = Connection;
            //    SqliteCommand.CommandText = SqlCommand;
            //    SqliteCommand.CommandType = CommandType.Text;
            //    SqliteCommand.ExecuteNonQuery();
            //}
        }

        public static void GetData<T>(string SqlText, ObservableCollection<T> data) where T : BaseReportModel, new()
        {
            //data.Clear();
            //using (SqliteCommand SqliteCommand = new SqliteCommand(SqlText, Connection))
            //{
            //    SqliteDataReader reader = SqliteCommand.ExecuteReader();
            //    try
            //    {
            //        while (reader.Read())
            //        {
            //            T obj = new T();
            //            obj.Init(reader);
            //            data.Add(obj);
            //        }
            //    }
            //    finally
            //    {
            //        reader.Close();
            //    }
            //}
        }
    }
}