﻿using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class ExchangeRateModel : BaseModel
    {
        public int FromCurrencyId { get; set; }

        public int ToCurrencyId { get; set; }

        public long Date { get; set; }

        public float Rate { get; set; }

        public CurrencyModel FromCurrency { get; set; }

        public CurrencyModel ToCurrency { get; set; }
    }
}
