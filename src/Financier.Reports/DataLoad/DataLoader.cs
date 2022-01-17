using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace fcrd
{
    public class DataLoader
    {
        private readonly string _backupDir;

        public DataLoader(string backupDir) => this._backupDir = backupDir;

        public void Start()
        {
            string str = FilePrepare.Prepare(this._backupDir);
            SQLiteTransaction sqLiteTransaction = DB.Connection.BeginTransaction();
            DB.TruncateTables();
            DataReader dataReader = new DataReader();
            dataReader.OnEntityRead += new DataReader.EntityReadDelegate(this.DataReaderOnEntityRead);
            dataReader.Start(str);
            File.Delete(str);
            this.PrepareData();
            sqLiteTransaction.Commit();
            DbManual.ResetManuals();
        }

        private void DataReaderOnEntityRead(string type, Dictionary<string, string> items)
        {
            TableInfo tableInfo = DB.TablesInfo.FirstOrDefault(p => p.TableName == type);
            if (tableInfo == null)
                return;
            StringBuilder stringBuilder1 = new StringBuilder(string.Format("INSERT INTO [{0}] (", type));
            StringBuilder stringBuilder2 = new StringBuilder("VALUES (");
            using (Dictionary<string, string>.Enumerator enumerator = items.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<string, string> item = enumerator.Current;
                    ColumnInfo columnInfo = tableInfo.ColumnsInfo.FirstOrDefault(p => p.ColName == item.Key);
                    if (columnInfo != null)
                    {
                        StringBuilder stringBuilder3 = stringBuilder1;
                        KeyValuePair<string, string> keyValuePair = item;
                        string key1 = keyValuePair.Key;
                        keyValuePair = items.First();
                        string key2 = keyValuePair.Key;
                        string format1 = key1 == key2 ? "[{0}] " : ",[{0}] ";
                        keyValuePair = item;
                        string key3 = keyValuePair.Key;
                        stringBuilder3.AppendFormat(format1, key3);
                        string str1;
                        if (columnInfo.Type.Substring(0, 3) == "DAT")
                        {
                            keyValuePair = item;
                            double result;
                            double.TryParse(keyValuePair.Value, out result);
                            str1 = UnixTimeStampToDateTime(result / 1000.0).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        else
                        {
                            keyValuePair = item;
                            str1 = keyValuePair.Value.Replace("'", "''");
                        }
                        StringBuilder stringBuilder4 = stringBuilder2;
                        keyValuePair = item;
                        string key4 = keyValuePair.Key;
                        keyValuePair = items.First();
                        string key5 = keyValuePair.Key;
                        string format2 = key4 == key5 ? "'{0}' " : ",'{0}' ";
                        string str2 = str1;
                        stringBuilder4.AppendFormat(format2, str2);
                    }
                }
            }
            stringBuilder1.Append(")");
            stringBuilder2.Append(")");
            DB.ExecuteNonQuery(stringBuilder1.AppendLine(stringBuilder2.ToString()).ToString());
        }

        private void PrepareData()
        {
            DB.ExecuteNonQuery(@"
UPDATE currency_exchange_rate
SET    rate_date = DATE(rate_date);

UPDATE currency_exchange_rate
SET    rate_date_end = (SELECT Min (rate_date)
                        FROM   currency_exchange_rate x
                        WHERE  x.from_currency_id = currency_exchange_rate.from_currency_id
                               AND x.to_currency_id = currency_exchange_rate.to_currency_id
                               AND x.rate_date > currency_exchange_rate.rate_date);

UPDATE currency_exchange_rate
SET    rate_date_end = '9999-12-31'
WHERE  rate_date_end IS NULL; 
"
);
            DB.ExecuteNonQuery(@"
/* ид валют счетов проводки */
UPDATE transactions
SET    from_account_crc_id = (SELECT currency_id
                              FROM   account acc
                              WHERE  acc._id = from_account_id);

UPDATE transactions
SET    to_account_crc_id = (SELECT currency_id
                            FROM   account acc
                            WHERE  acc._id = to_account_id);

/* сума проводки в домашней валюте*/
UPDATE transactions
SET    from_amount_default_crr =
        CASE (SELECT _id FROM currency WHERE is_default = 1)
        WHEN from_account_crc_id THEN from_amount
        ELSE Round( from_amount * (SELECT rate
                             FROM   currency_exchange_rate
                             WHERE  to_currency_id = (SELECT _id FROM currency WHERE is_default = 1)
                                    AND from_currency_id = from_account_crc_id
                                    AND datetime BETWEEN rate_date AND rate_date_end), 0 )
                             END;

UPDATE transactions
SET    to_amount_default_crr = CASE (SELECT _id FROM currency WHERE is_default = 1)
                                 WHEN to_account_crc_id THEN to_amount
                                 ELSE Round(to_amount * (SELECT rate FROM currency_exchange_rate
                                                         WHERE to_currency_id = (SELECT _id FROM currency WHERE is_default = 1)
                                                         AND from_currency_id = to_account_crc_id
                                                         AND datetime BETWEEN rate_date AND rate_date_end), 0)
                                 END;

/* дата проводки по частям */
UPDATE transactions
SET    date_year = Strftime('%Y', datetime),
       date_month = Strftime('%m', datetime),
       date_day = Strftime('%d', datetime),
       date_week = Strftime('%W', datetime),
       date_weekday = Strftime('%w', datetime);
");

            DB.ExecuteNonQuery(@"
UPDATE account
SET    total_amount_indef = CASE (SELECT _id FROM currency WHERE is_default = 1 )
                              WHEN currency_id THEN total_amount
                              ELSE Round(total_amount * (SELECT rate
                                                         FROM currency_exchange_rate
                                                         WHERE to_currency_id = (SELECT _id FROM currency WHERE is_default = 1 )
                                         AND from_currency_id = currency_id
                                         AND rate_date_end = '9999-12-31'), 0)
                            END; 
");
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) => new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTimeStamp).ToLocalTime();
    }
}