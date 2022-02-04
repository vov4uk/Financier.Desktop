using System.ComponentModel;

namespace Financier.Common.Entities
{
    public enum PeriodType
    {
        [Description("Today")]
        Today,

        [Description("Yesterday")]
        Yesterday,

        [Description("Current week")]
        CurrentWeek,

        [Description("Previous week")]
        PreviousWeek,

        [Description("Previous and current week")]
        PreviousAndCurrentWeek,

        [Description("Current month")]
        CurrentMonth,

        [Description("Previous month")]
        PreviousMonth,

        [Description("Previous and current month")]
        PreviousAndCurrentMonth,

        [Description("Custom")]
        Custom,
    }
}