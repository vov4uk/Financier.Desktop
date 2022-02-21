﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.Data
{
    [DebuggerDisplay("{Title}")]
    [Table(Backup.LOCATIONS_TABLE)]
    public class Location : Tag
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("datetime")]
        public long Date { get; set; }

        [Column("provider")]
        public string Provider { get; set; }

        [Column("accuracy")]
        public string Accuracy { get; set; }

        [Column("latitude")]
        public string Latitude { get; set; }

        [Column("longitude")]
        public string Longitude { get; set; }

        [Column("is_payee")]
        public bool IsPayee { get; set; }

        [Column("resolved_address")]
        public string Address { get; set; }

        [Column("count")]
        public int Count { get; set; }

    }
}