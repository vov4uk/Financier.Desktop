using System;
using System.Diagnostics;

namespace Financier.Desktop.Reports.Model
{
    [DebuggerDisplay("{Title} : {TotalNegativeAmount} : {TotalPositiveAmount}")]
    public class ReportRow
    {
        public int Id;

        public string Title { get; set; }

        public string CurrencySign { get; set; }

        public long TotalPositiveAmount { get; set; }

        public long TotalNegativeAmount { get; set; }

        public long GetAbsoluteMax()
        {
            return Math.Max(Math.Abs(TotalNegativeAmount), TotalPositiveAmount);
        }
    }
}
