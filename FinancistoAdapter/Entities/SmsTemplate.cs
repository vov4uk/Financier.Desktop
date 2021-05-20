namespace FinancistoAdapter.Entities
{
    [Entity(Backup.SMS_TEMPLATES_TABLE)]
    public class SmsTemplate : Entity
    {
        [EntityProperty(IdColumn)]
        public int Id { get; set; } = -1;

        [EntityProperty(IsActiveColumn )]
        public bool IsActive { get; set; }

        [EntityProperty(TitleColumn)]
        public string Title { get; set; }

        [EntityProperty("category_id")]
        public int CategoryId { get; set; }

        [EntityProperty("template")]
        public string Template { get; set; }

        [EntityProperty("account_id")]
        public int AccountId { get; set; }

        [EntityProperty("is_income")]
        public bool IsIncome { get; set; }

        [EntityProperty(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }
    }
}
