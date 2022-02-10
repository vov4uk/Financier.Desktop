using System;
using Financier.DataAccess.Data;
using Financier.Converters;
using Financier.Desktop.Data;

namespace Financier.Desktop.Helpers
{
    public static class MapperHelper
    {
        public static void MapTransfer(TransferDto dto, Transaction tr)
        {
            tr.FromAccountId = (int)dto.FromAccountId;
            tr.FromAccount = null;
            tr.ToAccountId = (int)dto.ToAccountId;
            tr.ToAccount = null;
            tr.Note = dto.Note;
            tr.FromAmount = Math.Abs(dto.FromAmount) * -1;
            tr.ToAmount = Math.Abs(dto.ToAccountCurrency.Id == dto.ToAccountCurrency.Id ? dto.FromAmount : dto.ToAmount);
            tr.DateTime = UnixTimeConverter.ConvertBack(dto.DateTime);
            tr.LastRecurrence = UnixTimeConverter.ConvertBack(DateTime.Now);
            tr.OriginalCurrencyId = (int)dto.FromAccount.CurrencyId;
            tr.OriginalFromAmount = Math.Abs(dto.FromAmount) * -1;
            tr.CategoryId = 0;
            tr.Category = default;
        }

        public static void MapTransaction(TransactionDto dto, Transaction tr)
        {
            tr.FromAccountId = (int)dto.AccountId;

            if (dto.OriginalCurrencyId > 0 && dto.Account?.CurrencyId == dto.OriginalCurrencyId)
            {
                tr.FromAmount = dto.RealFromAmount;
                tr.OriginalCurrencyId = 0;
                tr.OriginalFromAmount = 0;
            }
            else
            {
                tr.FromAmount = Math.Abs(dto.FromAmount) * (dto.IsAmountNegative ? -1 : 1);
                tr.OriginalFromAmount = dto.OriginalFromAmount ?? 0;
                tr.OriginalCurrencyId = (int)(dto.OriginalCurrencyId ?? 0);
            }

            tr.CategoryId = (int)(dto.CategoryId ?? 0);
            tr.Category = default;
            tr.Location = default;
            tr.Project = default;
            tr.OriginalCurrency = default;
            tr.FromAccount = default;
            tr.ToAccount = default;
            tr.PayeeId = (int)(dto.PayeeId ?? 0);
            tr.LocationId = (int)(dto.LocationId ?? 0);
            tr.ProjectId = (int)(dto.CategoryId == Category.Split.Id ? 0 : (dto.ProjectId ?? 0));
            tr.Note = dto.Note;
            tr.DateTime = UnixTimeConverter.ConvertBack(dto.DateTime);
            tr.LastRecurrence = UnixTimeConverter.ConvertBack(DateTime.Now);
        }
    }
}
