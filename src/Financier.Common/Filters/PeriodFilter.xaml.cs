using Financier.Common.Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Financier.Common.Filters
{
    public partial class PeriodFilter : UserControl
    {
        public PeriodFilter()
        {
            InitializeComponent();
            Loaded += (o, _) =>
            {
                SelectedPeriodType = PeriodType.Today;
            };
        }

        public static readonly DependencyProperty SelectedPeriodTypeProperty =
            DependencyProperty.Register("SelectedPeriodType",
                typeof(PeriodType),
                typeof(PeriodFilter),
                typeMetadata: new FrameworkPropertyMetadata(defaultValue: PeriodType.Custom, propertyChangedCallback: PropertyChangedCallback));

        public PeriodType SelectedPeriodType
        {
            get => (PeriodType)GetValue(SelectedPeriodTypeProperty);
            set => SetValue(SelectedPeriodTypeProperty, value);
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dates = UpdatePeriod((PeriodType)e.NewValue);
            var filter = (PeriodFilter)d;
            filter.FromDatePicker.SelectedDate = dates.from;
            filter.ToDatePicker.SelectedDate = dates.to;
        }

        private static (DateTime? from, DateTime? to) UpdatePeriod(PeriodType type)
        {
            DateTime? from = default;
            DateTime? to = default;
            switch (type)
            {
                case PeriodType.Custom:
                    break;
                case PeriodType.Today:
                    {
                        from = DateTime.Today;
                        to = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.Yesterday:
                    {
                        from = DateTime.Today.AddDays(-1);
                        to = DateTime.Today.AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousWeek:
                    {
                        DayOfWeek weekStart = DayOfWeek.Monday;
                        DateTime startingDate = DateTime.Today;

                        while (startingDate.DayOfWeek != weekStart)
                            startingDate = startingDate.AddDays(-1);

                        from = startingDate.AddDays(-7);
                        to = startingDate.AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousMonth:
                    {
                        var today = DateTime.Today;

                        from = new DateTime(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1);
                        to = new DateTime(today.Year, today.Month, 1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.CurrentWeek:
                    {
                        DayOfWeek weekStart = DayOfWeek.Monday;
                        DateTime startingDate = DateTime.Today;

                        while (startingDate.DayOfWeek != weekStart)
                            startingDate = startingDate.AddDays(-1);

                        from = startingDate;
                        to = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.CurrentMonth:
                    {
                        var today = DateTime.Today;

                        from = new DateTime(today.Year, today.Month, 1);
                        to = new DateTime(today.AddMonths(1).Year, today.AddMonths(1).Month, 1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousAndCurrentWeek:
                    {
                        DayOfWeek weekStart = DayOfWeek.Monday;
                        DateTime startingDate = DateTime.Today;

                        while (startingDate.DayOfWeek != weekStart)
                            startingDate = startingDate.AddDays(-1);

                        from = startingDate.AddDays(-7);
                        to = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousAndCurrentMonth:
                    {
                        var today = DateTime.Today;

                        from = new DateTime(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1);
                        to = new DateTime(today.AddMonths(1).Year, today.AddMonths(1).Month, 1).AddMilliseconds(-1);
                    }
                    break;
                default:
                    break;
            }

            return (from, to);
        }
    }
}
