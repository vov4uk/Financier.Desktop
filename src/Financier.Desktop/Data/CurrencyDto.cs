using System;
using Financier.Common.Entities;
using Financier.DataAccess.Data;
using Prism.Mvvm;

namespace Financier.Desktop.Data
{
    public class CurrencyDto : BindableBase
    {
        private string title;
        private string name;
        private string symbol;
        private bool isDefault;
        private bool updateExchangeRate;
        private int decimals;
        private string decimalSeparator;
        private string groupSeparator;
        private SymbolFormat symbolFormat;
        private string numberFormat;

        public CurrencyDto() { }

        public CurrencyDto(Currency currency)
        {
            Id = currency.Id;
            Title = currency.Title;
            Name = currency.Name;
            Symbol = currency.Symbol;
            IsDefault = currency.IsDefault;
            UpdateExchangeRate = currency.UpdateExchangeRate;
            Decimals = currency.Decimals;
            DecimalSeparator = currency.DecimalSeparator;
            GroupSeparator = currency.GroupSeparator;
            if (Enum.TryParse<SymbolFormat>(currency.SymbolFormat, ignoreCase: true, out var parsedSymbolFormat))
            {
                SymbolFormat = parsedSymbolFormat;
            }
            else
            {
                SymbolFormat = SymbolFormat.RS;
            }
            NumberFormat = currency.NumberFormat;
        }

        public int Id { get; set; }

        public string Title
        {
            get => title;
            set { SetProperty(ref title, value, nameof(Title)); }
        }

        public string Name
        {
            get => name;
            set { SetProperty(ref name, value, nameof(Name)); }
        }

        public string Symbol
        {
            get => symbol;
            set { SetProperty(ref symbol, value, nameof(Symbol)); }
        }

        public bool IsDefault
        {
            get => isDefault;
            set { SetProperty(ref isDefault, value, nameof(IsDefault)); }
        }

        public bool UpdateExchangeRate
        {
            get => updateExchangeRate;
            set { SetProperty(ref updateExchangeRate, value, nameof(UpdateExchangeRate)); }
        }

        public int Decimals
        {
            get => decimals;
            set { SetProperty(ref decimals, value, nameof(Decimals)); }
        }

        public string DecimalSeparator
        {
            get => decimalSeparator;
            set { SetProperty(ref decimalSeparator, value, nameof(DecimalSeparator)); }
        }

        public string GroupSeparator
        {
            get => groupSeparator;
            set { SetProperty(ref groupSeparator, value, nameof(GroupSeparator)); }
        }

        public SymbolFormat SymbolFormat
        {
            get => symbolFormat;
            set { SetProperty(ref symbolFormat, value, nameof(SymbolFormat)); }
        }

        public string NumberFormat
        {
            get => numberFormat;
            set { SetProperty(ref numberFormat, value, nameof(NumberFormat)); }
        }
    }
}
