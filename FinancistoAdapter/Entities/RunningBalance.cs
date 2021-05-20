using System;
using System.Diagnostics;

namespace FinancistoAdapter.Entities
{
    [DebuggerDisplay("(TranId = {Transaction.Id}) : {Balance}")]
    public class RunningBalance
    {
        public Account Account { get; set; }

        public Transaction Transaction { get; set; }

        public DateTime Date { get; set; }

        public double Balance { get; set; }
    }
}
