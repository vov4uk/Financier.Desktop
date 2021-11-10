namespace Financier.Adapter.Tests.Common
{
    using System.Collections.Generic;
    using Financier.DataAccess.Data;

    internal class TestComparer : IEqualityComparer<Transaction>
    {
        public bool Equals(Transaction x, Transaction y)
        {
            return x.Accuracy == y.Accuracy
                   && x.AttachedPicture == y.AttachedPicture
                   && x.BlobKey == y.BlobKey
                   && x.Category == y.Category
                   && x.CategoryId == y.CategoryId
                   && x.DateTime == y.DateTime
                   && x.FromAccount == y.FromAccount
                   && x.FromAccountId == y.FromAccountId
                   && x.FromAmount == y.FromAmount
                   && x.Id == y.Id
                   && x.IsCcardPayment == y.IsCcardPayment
                   && x.IsTemplate == y.IsTemplate
                   && x.LastRecurrence == y.LastRecurrence
                   && x.Latitude == y.Latitude
                   && x.Location == y.Location
                   && x.LocationId == y.LocationId
                   && x.Longitude == y.Longitude
                   && x.Note == y.Note
                   && x.NotificationOptions == y.NotificationOptions
                   && x.OriginalCurrency == y.OriginalCurrency
                   && x.OriginalCurrencyId == y.OriginalCurrencyId
                   && x.OriginalFromAmount == y.OriginalFromAmount
                   && x.Parent == y.Parent
                   && x.ParentId == y.ParentId
                   && x.Payee == y.Payee
                   && x.PayeeId == y.PayeeId
                   && x.PayeeString == y.PayeeString
                   && x.Project == y.Project
                   && x.ProjectId == y.ProjectId
                   && x.Provider == y.Provider
                   && x.RemoteKey == y.RemoteKey
                   && x.Status == y.Status
                   && x.SubTransactions == y.SubTransactions
                   && x.TemplateName == y.TemplateName
                   && x.ToAccount == y.ToAccount
                   && x.ToAccountId == y.ToAccountId
                   && x.ToAmount == y.ToAmount
                   && x.UpdatedOn == y.UpdatedOn;
        }

        public int GetHashCode(Transaction obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
