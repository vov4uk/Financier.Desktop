using Financier.Common.Entities;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Financier.Common.Filters
{
    [ExcludeFromCodeCoverage]
    public partial class PeriodFilter : UserControl
    {
        public PeriodFilter()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedPeriodTypeProperty =
            DependencyProperty.Register("SelectedPeriodType",
                typeof(PeriodType),
                typeof(PeriodFilter),
                typeMetadata: new FrameworkPropertyMetadata(defaultValue: PeriodType.AllTime, propertyChangedCallback: PropertyChangedCallback));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation",
                typeof(Orientation),
                typeof(PeriodFilter),
                typeMetadata: new FrameworkPropertyMetadata(defaultValue: Orientation.Horizontal));

        public PeriodType SelectedPeriodType
        {
            get => (PeriodType)GetValue(SelectedPeriodTypeProperty);
            set => SetValue(SelectedPeriodTypeProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
            var filter = (PeriodFilter)d;
            var dates = UpdatePeriod((PeriodType)e.NewValue, filter.FromDatePicker.SelectedDate, filter.ToDatePicker.SelectedDate);
            filter.FromDatePicker.SelectedDate = dates.from;
            filter.ToDatePicker.SelectedDate = dates.to;
        }

        private static (DateTime? from, DateTime? to) UpdatePeriod(PeriodType type, DateTime? currentFrom, DateTime? currentTo)
        {
            DateTime? from = currentFrom;
            DateTime? to = currentTo;
            switch (type)
            {
                case PeriodType.Custom:
                case PeriodType.AllTime:
                    {
                        from = default;
                        to = default;
                    }
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

                        from = new DateTime(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1,0,0,0, DateTimeKind.Local);
                        to = new DateTime(today.Year, today.Month, 1,0,0,0, DateTimeKind.Local).AddMilliseconds(-1);
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

                        from = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Local);
                        to = new DateTime(today.AddMonths(1).Year, today.AddMonths(1).Month, 1, 0, 0, 0, DateTimeKind.Local).AddMilliseconds(-1);
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

                        from = new DateTime(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1, 0, 0, 0, DateTimeKind.Local);
                        to = new DateTime(today.AddMonths(1).Year, today.AddMonths(1).Month, 1, 0, 0, 0, DateTimeKind.Local).AddMilliseconds(-1);
                    }
                    break;
                default:
                    break;
            }

            return (from, to);
        }
    }
}
