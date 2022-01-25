using System.Collections.Generic;

namespace Financier.Reports.Controls
{
    public class ByCategoryReportRow : ReportRow
    {
        public int Left { get; set; }
        public int Right { get; set; }
        public List<ByCategoryReportRow> SubCategoties { get; set; }
    }
}
