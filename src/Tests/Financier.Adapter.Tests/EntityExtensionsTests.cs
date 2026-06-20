namespace Financier.Adapter.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using Financier.Tests.Common;
    using Xunit;

    public class EntityExtensionsTests
    {
        [Fact]
        public void WriteBackupLines_TransformTransactionToString_StringEquals()
        {
            var expectedString = @"$ENTITY:transactions
_id:3
from_account_id:3
to_account_id:0
category_id:2
project_id:0
location_id:0
note:ECMC5431 01.12.17 17:17 покупка 550р TEREMOK SCHUKA Баланс: 49820.45р
from_amount:-55000
to_amount:0
datetime:1515338499910
accuracy:0
latitude:0
longitude:0
is_template:0
status:PN
is_ccard_payment:0
last_recurrence:1515338499910
payee_id:0
parent_id:0
updated_on:1515338499910
original_currency_id:0
original_from_amount:0
$$
";

            var columnsOrder = new Dictionary<string, List<string>>
            {
                ["transactions"] = PredefinedData.TransactionsColumnsOrder,
            };

            var columnData = BuildColumnData(columnsOrder);

            using var sw = new StringWriter();
            PredefinedData.Transaction.WriteBackupLines(sw, columnData);

            Assert.Equal(
                expectedString.ReplaceLineEndings("\n"),
                sw.ToString().ReplaceLineEndings("\n"));
        }

        private static Dictionary<string, (Dictionary<string, int> Index, int Count)> BuildColumnData(
            Dictionary<string, List<string>> columnsOrder)
        {
            var result = new Dictionary<string, (Dictionary<string, int>, int)>(columnsOrder.Count);
            foreach (var (table, cols) in columnsOrder)
            {
                var index = new Dictionary<string, int>(cols.Count);
                for (int i = 0; i < cols.Count; i++)
                {
                    index[cols[i]] = i;
                }

                result[table] = (index, cols.Count);
            }

            return result;
        }
    }
}
