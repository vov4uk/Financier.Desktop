using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class YearMonths : BaseModel
    {
        [Column("year")]
        public long? Year { get; set; }

        [Column("month")]
        public long? Month { get; set; }

        public string Name => string.Format("{0} {1}",
            Month == null ?
                          string.Empty :
                          CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName((int)Month),
            Year);
    }
}