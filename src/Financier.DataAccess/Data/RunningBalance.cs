﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.Data
{
    [DebuggerDisplay("(TranId = {TransactionId}) : {Balance}")]
    [Table("running_balance")]
    public class RunningBalance : Entity
    {
        [ForeignKey("Transaction")]
        [Key, Column("transaction_id")]
        public int TransactionId { get; set; }

        [ForeignKey("Account")]
        [Key, Column("account_id")]
        public int AccountId { get; set; }

        public Account Account { get; set; }

        public Transaction Transaction { get; set; }

        [Column("datetime")]
        public long Datetime { get; set; }

        [Column("balance")]
        public long Balance { get; set; }
    }
}