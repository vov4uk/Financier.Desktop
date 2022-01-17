using System.Collections.Generic;

namespace fcrd
{
    public class TableInfo
    {
        public string TableName { get; private set; }

        public List<ColumnInfo> ColumnsInfo { get; private set; }

        public TableInfo(string tableName, List<ColumnInfo> columnsInfo)
        {
            this.TableName = tableName;
            this.ColumnsInfo = columnsInfo;
        }
    }
}