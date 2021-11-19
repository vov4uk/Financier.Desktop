namespace Financier.Common.Tests
{
    using System.Collections.Generic;
    using Financier.DataAccess.Data;

    public class PredefinedData
    {
        public static readonly Transaction Transaction = new Transaction
        {
            Accuracy = 0,
            AttachedPicture = null,
            BlobKey = null,
            Category = null,
            CategoryId = 2,
            DateTime = 1515338499910,
            FromAccount = null,
            FromAccountId = 3,
            FromAmount = -55000,
            Id = 3,
            IsCcardPayment = false,
            IsTemplate = false,
            LastRecurrence = 1515338499910,
            Latitude = 0,
            Location = null,
            LocationId = 0,
            Longitude = 0,
            Note = "ECMC5431 01.12.17 17:17 покупка 550р TEREMOK SCHUKA Баланс: 49820.45р",
            NotificationOptions = null,
            OriginalCurrency = null,
            OriginalCurrencyId = 0,
            OriginalFromAmount = 0,
            Parent = null,
            ParentId = 0,
            Payee = null,
            PayeeId = 0,
            PayeeString = null,
            Project = null,
            ProjectId = 0,
            Provider = null,
            RemoteKey = null,
            Status = "PN",
            SubTransactions = null,
            TemplateName = null,
            ToAccount = null,
            ToAccountId = 0,
            ToAmount = 0,
            UpdatedOn = 1515338499910,
        };

        public static readonly List<string> TransactionsColumnsOrder = new List<string>
        {
            "_id",
            "from_account_id",
            "to_account_id",
            "category_id",
            "project_id",
            "location_id",
            "note",
            "from_amount",
            "to_amount",
            "datetime",
            "accuracy",
            "latitude",
            "longitude",
            "is_template",
            "status",
            "is_ccard_payment",
            "last_recurrence",
            "payee_id",
            "parent_id",
            "updated_on",
            "original_currency_id",
            "original_from_amount",
        };
    }
}
