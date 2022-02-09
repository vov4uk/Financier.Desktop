namespace Financier.Desktop.ViewModel
{
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.Converters;
    using Financier.DataAccess.Abstractions;
    using Financier.DataAccess.Utils;
    using Financier.DataAccess.View;
    using Mvvm.Async;
    using Prism.Commands;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class BlotterVM : EntityBaseVM<BlotterModel>
    {
        private DelegateCommand _addTemplateCommand;
        private DelegateCommand _addTransferCommand;
        private DelegateCommand _duplicateCommand;
        private IAsyncCommand _clearFiltersCommand;
        private DelegateCommand _infoCommand;
        private DateTime? _from;
        private DateTime? _to;
        private PeriodType _periodType;
        private AccountFilterModel _account;
        private CategoryModel _category;
        private PayeeModel _payee;
        private ProjectModel _project;
        private LocationModel _location;

        public BlotterVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        public DateTime? From
        {
            get => _from;
            set
            {
                if (SetProperty(ref _from, value))
                {
                    RaisePropertyChanged(nameof(From));
                }
            }
        }

        public DateTime? To
        {
            get => _to;
            set
            {
                if (SetProperty(ref _to, value))
                {
                    RaisePropertyChanged(nameof(To));
                }
            }
        }

        public PeriodType PeriodType
        {
            get => _periodType;
            set
            {
                if (SetProperty(ref _periodType, value))
                {
                    RaisePropertyChanged(nameof(PeriodType));
                }
            }
        }

        public AccountFilterModel Account
        {
            get => _account ??= DbManual.Account.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _account = value;
                RaisePropertyChanged(nameof(Account));
            }
        }

        public CategoryModel Category
        {
            get => _category ??= DbManual.Category.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _category = value;
                RaisePropertyChanged(nameof(Category));
            }
        }

        public PayeeModel Payee
        {
            get => _payee ??= DbManual.Payee.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _payee = value;
                RaisePropertyChanged(nameof(Payee));
            }
        }

        public ProjectModel Project
        {
            get => _project ??= DbManual.Project.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _project = value;
                RaisePropertyChanged(nameof(Project));
            }
        }

        public LocationModel Location
        {
            get => _location ??= DbManual.Location.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _location = value;
                RaisePropertyChanged(nameof(Location));
            }
        }

        public event EventHandler AddTemplateRaised;

        public event EventHandler AddTransferRaised;

        public event EventHandler<BlotterModel> DuplicateRaised;

        public event EventHandler<BlotterModel> InfoRaised;

        public DelegateCommand AddTemplateCommand => _addTemplateCommand ??= new DelegateCommand(() => AddTemplateRaised?.Invoke(this, EventArgs.Empty), () => false);

        public DelegateCommand AddTransferCommand => _addTransferCommand ??= new DelegateCommand(() => AddTransferRaised?.Invoke(this, EventArgs.Empty));

        public DelegateCommand DuplicateCommand => _duplicateCommand ??= new DelegateCommand(() => DuplicateRaised?.Invoke(this, SelectedValue), () => false);

        public IAsyncCommand ClearFiltersCommand => _clearFiltersCommand ??= new AsyncCommand(ClearFilters, () => true);

        public DelegateCommand InfoCommand => _infoCommand ??= new DelegateCommand(() => InfoRaised?.Invoke(this, SelectedValue), () => false);


        private async Task ClearFilters()
        {
            PeriodType = PeriodType.AllTime;
            Account = default;
            Category = default;
            Payee = default;
            Project = default;
            Location = default;
            await RefreshDataCommand.ExecuteAsync();
        }

        protected override async Task RefreshData()
        {
            using var uow = financierDatabase.CreateUnitOfWork();
            var repo = uow.GetRepository<BlotterTransactions>();
            var fromUnix = UnixTimeConverter.ConvertBack(From ?? DateTime.MinValue.ToLocalTime());
            var toUnix = UnixTimeConverter.ConvertBack(To ?? DateTime.MaxValue.ToLocalTime());

            Expression<Func<BlotterTransactions, bool>> predicate = x => x.datetime >= fromUnix && x.datetime <= toUnix;

            if (Account?.Id != null)
            {
                predicate = predicate.And(x => x.from_account_id == _account.Id || x.to_account_id == _account.Id);
            }

            if (Category?.Id != null)
            {
                predicate = predicate.And(x => x.category_id == _category.Id && x.to_account_id == null);
            }

            if (Project?.Id != null)
            {
                predicate = predicate.And(x => x.project_id == _project.Id);
            }

            if (Payee?.Id != null)
            {
                predicate = predicate.And(x => x.payee_id == _payee.Id);
            }

            if (Location?.Id != null)
            {
                predicate = predicate.And(x => x.location_id == _location.Id);
            }

            var items = await repo.FindManyAsync(
                predicate: predicate,
                projection: x => new BlotterModel
                {
                    Id = x._id,
                    FromAccountId  = x.from_account_id,
                    FromAccountTitle = x.from_account_title,
                    ToAccountId = x.to_account_id,
                    ToAccountTitle = x.to_account_title,
                    FromAccountCurrencyId = x.from_account_currency_id,
                    CategoryId = x.category_id,
                    CategoryTitle = x.category_title,
                    LocationId = x.location_id,
                    Location = x.location,
                    Payee = x.payee,
                    Note = x.note,
                    FromAmount = x.from_amount,
                    ToAmount = x.to_amount,
                    Datetime = x.datetime,
                    OriginalCurrencyId = x.original_currency_id,
                    OriginalFromAmount = x.original_from_amount,
                    FromAccountBalance = x.from_account_balance,
                    ToAccountBalance = x.to_account_balance,
                    FromAccountCurrency = new CurrencyModel
                    {
                        Id = x.from_account_currency.Id,
                        Name = x.from_account_currency.Name,
                        Symbol = x.from_account_currency.Symbol,
                    },
                    ToAccountCurrency = x.to_account_currency == null ? default : new CurrencyModel
                    {
                        Id = x.to_account_currency.Id,
                        Name = x.to_account_currency.Name,
                        Symbol = x.to_account_currency.Symbol,
                    },
                    OriginalCurrency = x.original_currency == null ? default : new CurrencyModel
                    {
                        Id = x.original_currency.Id,
                        Name = x.original_currency.Name,
                        Symbol = x.original_currency.Symbol,
                    }
                },
                includes: new Expression<Func<BlotterTransactions, object>>[] { x => x.from_account_currency, x => x.to_account_currency, x => x.original_currency });
            if (items != null)
            {
                Entities = new System.Collections.ObjectModel.ObservableCollection<BlotterModel>(items.OrderByDescending(x => x.Datetime));
            }
        }
    }
}
