namespace Financier.Adapter.Tests
{
    using System.Collections.Generic;
    using Financier.Tests.Common;
    using Xunit;

    public class EntityExtensionsTests
    {
        [Fact]
        public void ToBackupLines_TransfromTransactionToString_StringEquels()
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

            Dictionary<string, List<string>> entityColumnsOrder = new Dictionary<string, List<string>>();
            entityColumnsOrder.Add("transactions", PredefinedData.TransactionsColumnsOrder);

            var actualString = PredefinedData.Transaction.ToBackupLines(entityColumnsOrder);

            Assert.Equal(expectedString, actualString);
        }
    }
}
