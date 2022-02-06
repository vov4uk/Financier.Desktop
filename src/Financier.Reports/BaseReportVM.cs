using Financier.Common;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Reports
{
    public abstract class BaseReportVM<T> : BaseViewModel<T>
        where T : BaseModel, new()
    {
        private ProjectModel _project;
        private CategoryModel _category;
        private CategoryModel _topCategory;
        private AccountFilterModel _account;
        private PayeeModel _payee;
        private CurrencyModel _curentCurrency;
        private YearMonths _startYearMonths;
        private YearMonths _endYearMonths;
        private DateTime? _date;
        private DateTime? _from;
        private DateTime? _to;

        private PlotModel plotModel;

        public string Header { get; set; }

        public PlotModel PlotModel
        {
            get => plotModel;
            private set
            {
                plotModel = value;
                RaisePropertyChanged(nameof(PlotModel));
            }
        }

        protected BaseReportVM(IFinancierDatabase financierDatabase)
            : base(financierDatabase)
        {
            PlotModel = new PlotModel();
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

        public CategoryModel Category
        {
            get => _category ??= DbManual.Category.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _category = value;
                RaisePropertyChanged(nameof(Category));
            }
        }

        public CategoryModel TopCategory
        {
            get => _topCategory ??= DbManual.TopCategories.FirstOrDefault(p => !p.Id.HasValue);
            set
            {
                _topCategory = value;
                RaisePropertyChanged(nameof(TopCategory));
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

        public PayeeModel Payee
        {
            get => _payee ?? (_payee = DbManual.Payee.FirstOrDefault(p => !p.Id.HasValue));
            set
            {
                _payee = value;
                RaisePropertyChanged(nameof(Payee));
            }
        }

        public CurrencyModel CurentCurrency
        {
            get => _curentCurrency ?? (_curentCurrency = DbManual.Currencies.FirstOrDefault(p => !p.Id.HasValue));
            set
            {
                _curentCurrency = value;
                RaisePropertyChanged(nameof(CurentCurrency));
            }
        }

        public YearMonths StartYearMonths
        {
            get => _startYearMonths ?? (_startYearMonths = DbManual.YearMonths.FirstOrDefault(p => !p.Year.HasValue && !p.Month.HasValue));
            set
            {
                _startYearMonths = value;
                RaisePropertyChanged(nameof(StartYearMonths));
            }
        }

        public YearMonths EndYearMonths
        {
            get => _endYearMonths ?? (_endYearMonths = DbManual.YearMonths.FirstOrDefault(p => !p.Year.HasValue && !p.Month.HasValue));
            set
            {
                _endYearMonths = value;
                RaisePropertyChanged(nameof(EndYearMonths));
            }
        }

        public DateTime? DateFilter
        {
            get => _date;
            set
            {
                if (SetProperty(ref _date, value))
                {
                    RaisePropertyChanged(nameof(DateFilter));
                }
            }
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

        protected override async Task RefreshData()
        {
            string sql = GetSql();
            if (!string.IsNullOrEmpty(sql))
            {
                var data = await base.financierDatabase.ExecuteQuery<T>(sql);
                Entities = new ObservableCollection<T>(data);
                PlotModel = GetPlotModel(data);
            }
        }

        protected abstract string GetSql();

        protected abstract PlotModel GetPlotModel(List<T> list);

        protected string GetStandartTrnFilter()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (StartYearMonths.Year.HasValue)
                stringBuilder.Append(string.Format(" ((date_year = {0} and date_month >= {1}) or date_year > {0})", StartYearMonths.Year, StartYearMonths.Month));
            if (DateFilter.HasValue)
                stringBuilder.Append($" date(\"{DateFilter.Value.ToString("yyyy-MM-dd")}\")");
            if (EndYearMonths.Year.HasValue)
                stringBuilder.Append(string.Format(" {2} ((date_year = {0} and date_month <= {1}) or date_year < {0})", EndYearMonths.Year, EndYearMonths.Month, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (Payee.Id.HasValue)
                stringBuilder.Append(string.Format(" {1} ( payee_id = {0} )", Payee.Id, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (Category.Id.HasValue)
                stringBuilder.Append(string.Format(@"
{1} category_id IN
(
       SELECT _id
       FROM   category ctx,
              (
                     SELECT xxx.LEFT,
                            xxx.RIGHT
                     FROM   category xxx
                     WHERE  xxx._id = {0}) root
       WHERE  ctx.LEFT >= root.LEFT
       AND    ctx.RIGHT <= root.RIGHT
)", Category.Id, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (Project.Id.HasValue)
                stringBuilder.Append(string.Format(" {1} ( project_id = {0} )", Project.Id, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (Account.Id.HasValue)
                stringBuilder.Append(string.Format(" {1} ( from_account_id = {0} )", Account.Id, stringBuilder.Length != 0 ? " and " : string.Empty));
            return stringBuilder.ToString();
        }
    }
}