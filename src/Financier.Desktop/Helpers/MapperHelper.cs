using Financier.DataAccess.Data;
using Financier.Desktop.Converters;
using Financier.Desktop.ViewModel.Dialog;

namespace Financier.Desktop.Helpers
{
    public static class MapperHelper
    {
        public static void MapTransfer(TransferDTO dto, Transaction tr)
        {
            tr.Id = dto.Id;
            tr.FromAccountId = dto.FromAccountId;
            tr.ToAccountId = dto.ToAccountId;
            tr.Note = dto.Note;
            tr.FromAmount = System.Math.Abs(dto.FromAmount) * -1;
            tr.ToAmount = System.Math.Abs(dto.ToAmount);
            tr.DateTime = UnixTimeConverter.ConvertBack(dto.DateTime);
            tr.OriginalCurrencyId = dto.FromAccount.CurrencyId;
            tr.OriginalFromAmount = System.Math.Abs(dto.FromAmount) * -1;
            tr.CategoryId = 0;
            tr.Category = default;
        }

        public static void MapTransaction(TransactionDTO dto, Transaction tr)
        {
            tr.Id = dto.Id;
            tr.FromAccountId = dto.AccountId;
            tr.FromAmount = dto.RealFromAmount;
            tr.OriginalFromAmount = dto.OriginalFromAmount ?? 0;
            tr.OriginalCurrencyId = dto.OriginalCurrencyId ?? 0;
            tr.CategoryId = dto.CategoryId ?? 0;
            tr.Category = default;
            tr.PayeeId = dto.PayeeId ?? 0;
            tr.LocationId = dto.LocationId ?? 0;
            tr.ProjectId = dto.CategoryId == Category.Split.Id ? 0 : (dto.ProjectId ?? 0);
            tr.Note = dto.Note;
            tr.DateTime = UnixTimeConverter.ConvertBack(dto.DateTime);
        }
    }
}
