﻿using Financier.Common.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports
{
    public class ReportDynamicRestModel : BaseModel
    {
        [Column("year")]
        public int Year { get; protected set; }

        [Column("month")]
        public int Month { get; protected set; }

        [Column("day")]
        public int Day { get; protected set; }

        [DisplayName("Date")]
        public string Title => $"{Year}.{Month:00}.{Day:00}";

        [DisplayName("Total (home currency)")]
        [Column("total")]
        public double? Total { get; protected set; }
    }
}