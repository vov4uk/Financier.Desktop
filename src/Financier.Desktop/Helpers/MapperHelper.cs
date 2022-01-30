using System;
using Financier.DataAccess.Data;
using Financier.Desktop.Converters;
using Financier.Desktop.Data;

namespace Financier.Desktop.Helpers
{
    public static class MapperHelper
    {
        public static void MapTransfer(TransferDto dto, Transaction tr)
        {
            tr.FromAccountId = dto.FromAccountId;
            tr.ToAccountId = dto.ToAccountId;
            tr.Note = dto.Note;
            tr.FromAmount = Math.Abs(dto.FromAmount) * -1;
            tr.ToAmount = Math.Abs(dto.ToAmount == 0 ? dto.FromAmount : dto.ToAmount);
            tr.DateTime = UnixTimeConverter.ConvertBack(dto.DateTime);
            tr.LastRecurrence = UnixTimeConverter.ConvertBack(DateTime.Now);
            tr.OriginalCurrencyId = dto.FromAccount.CurrencyId;
            tr.OriginalFromAmount = Math.Abs(dto.FromAmount) * -1;
            tr.CategoryId = 0;
            tr.Category = default;
        }

        public static void MapTransaction(TransactionDto dto, Transaction tr)
        {
            tr.FromAccountId = dto.AccountId;
            tr.FromAmount = dto.RealFromAmount;

            if (dto.OriginalCurrencyId > 0 && dto.Account.CurrencyId == dto.OriginalCurrencyId)
            {
                tr.OriginalCurrencyId = 0;
                tr.OriginalFromAmount = 0;
            }
            else
            {
                tr.OriginalFromAmount = dto.OriginalFromAmount ?? 0;
                tr.OriginalCurrencyId = dto.OriginalCurrencyId ?? 0;
            }

            tr.CategoryId = dto.CategoryId ?? 0;
            tr.Category = default;
            tr.Location = default;
            tr.Project = default;
            tr.OriginalCurrency = default;
            tr.FromAccount = default;
            tr.ToAccount = default;
            tr.PayeeId = dto.PayeeId ?? 0;
            tr.LocationId = dto.LocationId ?? 0;
            tr.ProjectId = dto.CategoryId == Category.Split.Id ? 0 : (dto.ProjectId ?? 0);
            tr.Note = dto.Note;
            tr.DateTime = UnixTimeConverter.ConvertBack(dto.DateTime);
            tr.LastRecurrence = UnixTimeConverter.ConvertBack(DateTime.Now);
        }
    }
}
