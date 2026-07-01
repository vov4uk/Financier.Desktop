using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Financier.Converters;
using Financier.Desktop.Data;

namespace Financier.Desktop.ViewModel.Dialog
{
    [ExcludeFromCodeCoverage]
    public class CurrencyControlVM : DialogBaseVM
    {
        private DecimalSeparator _selectedDecimalSeparator;
        private GroupSeparator _selectedGroupSeparator;
        private Decimals _selectedDecimals;

        public CurrencyControlVM(CurrencyDto currency)
        {
            Entity = currency;
            SelectedDecimalSeparator = ParseDecimalSeparator(currency.DecimalSeparator);
            SelectedGroupSeparator = ParseGroupSeparator(currency.GroupSeparator);
            SelectedDecimals = (Decimals)currency.Decimals;
        }

        public CurrencyDto Entity { get; }

        public DecimalSeparator SelectedDecimalSeparator
        {
            get
            {
                return _selectedDecimalSeparator;
            }
            set
            {
                _selectedDecimalSeparator = value;
                RaisePropertyChanged(nameof(SelectedDecimalSeparator));
            }
        }

        public GroupSeparator SelectedGroupSeparator
        {
            get
            {
                return _selectedGroupSeparator;
            }
            set
            {
                _selectedGroupSeparator = value;
                RaisePropertyChanged(nameof(SelectedGroupSeparator));
            }
        }

        public Decimals SelectedDecimals
        {
            get => _selectedDecimals;
            set
            {
                _selectedDecimals = value;
                RaisePropertyChanged(nameof(SelectedDecimals));
            }
        }

        public static GroupSeparator ParseGroupSeparator(string value)
        {
            switch (value)
            {
                case ".":
                case "'.'":
                    return GroupSeparator.PERIOD;
                case ",":
                case "','":
                    return GroupSeparator.COMMA;
                case " ":
                case "' '":
                    return GroupSeparator.SPACE;
                case "":
                case "''":
                    return GroupSeparator.NONE;
                default:
                    return GroupSeparator.PERIOD;
            }
        }

        private static DecimalSeparator ParseDecimalSeparator(string value)
        {
            switch (value)
            {
                case ".":
                case "'.'":
                    return DecimalSeparator.PERIOD;
                case ",":
                case "','" :
                    return DecimalSeparator.COMMA;
                case " ":
                case "' '":
                    return DecimalSeparator.SPACE;
                default:
                    return DecimalSeparator.PERIOD;
            }
        }

        public override object OnRequestSave()
        {
            Entity.DecimalSeparator = SelectedDecimalSeparator.ToString();
            Entity.GroupSeparator = SelectedGroupSeparator.ToString();
            Entity.Decimals = (int)SelectedDecimals;
            return Entity;
        }

    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum DecimalSeparator
    {
        [Description("'.'")]
        PERIOD,
        [Description("','")]
        COMMA,
        [Description("' '")]
        SPACE
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum GroupSeparator
    {
        [Description("'.'")]
        PERIOD,
        [Description("','")]
        COMMA,
        [Description("' '")]
        SPACE,
        [Description("''")]
        NONE
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum Decimals
    {
        [Description("0")]
        Zero = 0,
        [Description("1")]
        One = 1,
        [Description("2")]
        Two = 2
    }
}
