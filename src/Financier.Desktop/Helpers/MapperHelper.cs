﻿using System;
using Financier.DataAccess.Data;
using Financier.Converters;
using Financier.Desktop.Data;

namespace Financier.Desktop.Helpers
{
    public static class MapperHelper
    {
        public static void MapTransfer(TransferDto dto, Transaction tr)
        {
            tr.FromAccountId = dto.FromAccountId;
            tr.FromAccount = null;
            tr.ToAccountId = dto.ToAccountId;
            tr.ToAccount = null;
            tr.Note = dto.Note;
            tr.FromAmount = Math.Abs(dto.FromAmount) * -1;
            tr.ToAmount = Math.Abs(dto.IsToAmountVisible ? dto.ToAmount : dto.FromAmount);
            tr.DateTime = UnixTimeConverter.ConvertBack(dto.DateTime);
            tr.LastRecurrence = UnixTimeConverter.ConvertBack(DateTime.Now);
            tr.OriginalCurrencyId = dto.FromAccountCurrency?.Id;
            tr.OriginalFromAmount = Math.Abs(dto.FromAmount) * -1;
            tr.CategoryId = 0;
            tr.Category = default;
        }

        public static void MapTransaction(TransactionDto dto, Transaction tr)
        {
            tr.FromAccountId = dto.FromAccountId;

            if (dto.OriginalCurrencyId > 0 && dto.FromAccount?.CurrencyId == dto.OriginalCurrencyId)
            {
                tr.FromAmount = dto.RealFromAmount;
                tr.OriginalCurrencyId = 0;
                tr.OriginalFromAmount = 0;
            }
            else
            {
                tr.FromAmount = Math.Abs(dto.FromAmount) * (dto.IsAmountNegative ? -1 : 1);
                tr.OriginalFromAmount = Math.Abs(dto.OriginalFromAmount ?? 0) * (dto.IsAmountNegative ? -1 : 1);
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
            tr.ProjectId = dto.CategoryId == -1 ? 0 : (dto.ProjectId ?? 0); // parent transaction don't have Project
            tr.Note = dto.Note;
            tr.DateTime = UnixTimeConverter.ConvertBack(dto.DateTime);
            tr.LastRecurrence = UnixTimeConverter.ConvertBack(DateTime.Now);
        }
    }
}
