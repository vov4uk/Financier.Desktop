﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.DataAccess.Data
{
    [Table(Backup.TRANSACTION_TABLE)]
    public class Transaction : Entity, IIdentity
    {
        [Column(IdColumn)]
        public int Id { get; set; } = -1;

        [ForeignKey("FromAccount")]
        [Column("from_account_id")]
        public int FromAccountId { get; set; }

        [ForeignKey("ToAccount")]
        [Column("to_account_id")]
        public int ToAccountId { get; set; }

        [ForeignKey("Category")]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("note")]
        public string Note { get; set; }

        [Column("datetime")]
        public long DateTime { get; set; }

        [Column("from_amount")]
        public long FromAmount { get; set; }

        [Column("to_amount")]
        public long ToAmount { get; set; }

        [ForeignKey("Payee")]
        [Column("payee_id")]
        public int PayeeId { get; set; }

        [Column("payee")]
        public string PayeeString { get; set; }

        [ForeignKey("Project")]
        [Column("project_id")]
        public int ProjectId { get; set; }

        [ForeignKey("Location")]
        [Column("location_id")]
        public int LocationId { get; set; }

        [ForeignKey("Parent")]
        [Column("parent_id")]
        public int ParentId { get; set; }

        [Column("blob_key")]
        public string BlobKey { get; set; }
        
        [Column("remote_key")]
        public string RemoteKey { get; set; }

        [ForeignKey("OriginalCurrency")]
        [Column("original_currency_id")]
        public int OriginalCurrencyId { get; set; }

        [Column("original_from_amount")]
        public long OriginalFromAmmount { get; set; }

        [Column(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }

        [Column("last_recurrence")]
        public long LastRecurrence { get; set; }

        [Column("is_ccard_payment")]
        public bool IsCcardPayment { get; set; }

        [Column("is_template")]
        public bool IsTemplate { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("latitude")]
        public string Latitude { get; set; }

        [Column("longitude")]
        public string Longitude { get; set; }

        [Column("accuracy")]
        public string Accuracy { get; set; }

        [Column("provider")]
        public string Provider { get; set; }

        [Column("template_name")]
        public string TemplateName { get; set; }

        [Column("notification_options")]
        public string NotificationOptions { get; set; }

        [Column("attached_picture")]
        public string AttachedPicture { get; set; }

        public Transaction Parent { get; set; }

        public Account FromAccount { get; set; }

        public Account ToAccount { get; set; }

        public Category Category { get; set; }

        public Payee Payee { get; set; }

        public Project Project { get; set; }

        public Location Location { get; set; }

        public Currency OriginalCurrency { get; set; }

        public List<Transaction> SubTransactions { get; set; }

    }
}
