using System.ComponentModel;
using Financier.Common.Attribute;
using Financier.Converters;

namespace Financier.Common.Entities
{
    [TypeConverter(typeof(EnumDescritpionTypeConverter))]
    public enum PeriodType
    {

        [LocalizedDescription("period_type_all_time")]
        AllTime,

        [LocalizedDescription("period_type_today")]
        Today,

        [LocalizedDescription("period_type_yesterday")]
        Yesterday,

        [LocalizedDescription("period_type_current_week")]
        CurrentWeek,

        [LocalizedDescription("period_type_previous_week")]
        PreviousWeek,

        [LocalizedDescription("period_type_previous_and_current_week")]
        PreviousAndCurrentWeek,

        [LocalizedDescription("period_type_current_month")]
        CurrentMonth,

        [LocalizedDescription("period_type_previous_month")]
        PreviousMonth,

        [LocalizedDescription("period_type_previous_and_current_month")]
        PreviousAndCurrentMonth,

        [LocalizedDescription("period_type_custom")]
        Custom,
    }
}
