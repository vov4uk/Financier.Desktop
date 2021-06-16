using Financier.DataAccess.Data;
using Financier.DataAccess.View;
using Microsoft.EntityFrameworkCore;

namespace Financier.DataAccess
{
    public class FinancierDataContext : DbContext
    {
        public FinancierDataContext(DbContextOptions<FinancierDataContext> options)
                : base(options)
        {
            this.ChangeTracker.AutoDetectChangesEnabled = false;
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyExchangeRate>().HasKey( x => new { x.FromCurrencyId, x.ToCurrencyId, x.Date });
            modelBuilder.Entity<RunningBalance>().HasKey( x => new { x.TransactionId, x.AccountId });
            modelBuilder.Entity<CategoryAttribute>().HasNoKey();
            modelBuilder.Entity<AllTransactions>().ToView("v_all_transactions").HasKey(x => x._id);
            modelBuilder.Entity<BlotterTransactions>().ToView("v_blotter").HasKey(x => x._id);
            modelBuilder.Entity<BlotterTransactionsForAccountWithSplits>().ToView("v_blotter_for_account_with_splits").HasKey(x => x._id);
            //modelBuilder.Entity<Transaction>()
            //    .HasOne(v => v.Parent)
            //    .WithMany(v => v.SubTransactions)
            //    .HasForeignKey(v => v.ParentId);

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AttributeDefinition> AttributeDefinitions { get; set; }
        public DbSet<CategoryAttribute> CategoryAttributes { get; set; }
        public DbSet<TransactionAttribute> TransactionAttributes { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<CCardClosingDate> CCardClosingDates { get; set; }
        public DbSet<SmsTemplate> SmsTemplates { get; set; }
        public DbSet<CurrencyExchangeRate> ExchangeRates { get; set; }
        public DbSet<RunningBalance> RunningBalance { get; set; }

        public DbSet<BlotterTransactionsForAccountWithSplits> BlotterTransactionsForAccountWithSplits { get; set; }
    }
}