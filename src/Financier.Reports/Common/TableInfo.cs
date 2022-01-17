using System.Collections.Generic;

namespace Financier.Reports.Common
{
    public class TableInfo
    {
        public string TableName { get; private set; }

        public List<ColumnInfo> ColumnsInfo { get; private set; }

        public TableInfo(string tableName, List<ColumnInfo> columnsInfo)
        {
            TableName = tableName;
            ColumnsInfo = columnsInfo;
        }
    }
}