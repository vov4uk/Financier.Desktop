using Financier.DataAccess.Abstractions;
using Mvvm.Async;
using OxyPlot;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Reports.Common
{
    public abstract class BaseReportVM<T> : BindableBase where T : BaseReportModel, new()
    {
        private readonly IFinancierDatabase financierDatabase;
        private ProjectModel _project;
        private CategoryModel _category;
        private CategoryModel _topCategory;
        private AccountModel _account;
        private PayeeModel _payee;
        private CurrencyModel _curentCurrency;
        private YearMonths _startYearMonths;
        private YearMonths _endYearMonths;
        private DateTime? _date;
        private DateTime? _from;
        private DateTime? _to;
        private IAsyncCommand _refreshDataCommand;
        private ObservableCollection<T> reportData;

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

        public ObservableCollection<T> ReportData
        {
            get => reportData;
            set
            {
                reportData = value;
                RaisePropertyChanged(nameof(ReportData));
            }
        }

        protected BaseReportVM(IFinancierDatabase financierDatabase)
        {
            this.financierDatabase = financierDatabase;
            ReportData = new ObservableCollection<T>();
            PlotModel = new PlotModel();
        }

        public ProjectModel Project
        {
            get => _project ??= DbManual.Project.FirstOrDefault(p => !p.ID.HasValue);
            set
            {
                _project = value;
                RaisePropertyChanged(nameof(Project));
            }
        }

        public CategoryModel Category
        {
            get => _category ??= DbManual.Category.FirstOrDefault(p => !p.ID.HasValue);
            set
            {
                _category = value;
                RaisePropertyChanged(nameof(Category));
            }
        }

        public CategoryModel TopCategory
        {
            get => _topCategory ??= DbManual.TopCategories.FirstOrDefault(p => !p.ID.HasValue);
            set
            {
                _topCategory = value;
                RaisePropertyChanged(nameof(TopCategory));
            }
        }

        public AccountModel Account
        {
            get => _account ??= DbManual.Account.FirstOrDefault(p => !p.ID.HasValue);
            set
            {
                _account = value;
                RaisePropertyChanged(nameof(Account));
            }
        }

        public PayeeModel Payee
        {
            get => _payee ?? (_payee = DbManual.Payee.FirstOrDefault(p => !p.ID.HasValue));
            set
            {
                _payee = value;
                RaisePropertyChanged(nameof(Payee));
            }
        }

        public CurrencyModel CurentCurrency
        {
            get => _curentCurrency ?? (_curentCurrency = DbManual.Currencies.FirstOrDefault(p => !p.ID.HasValue));
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

        public IAsyncCommand RefreshDataCommand => _refreshDataCommand ?? (_refreshDataCommand = new AsyncCommand(RefreshData));

        private async Task RefreshData()
        {
            string sql = GetSql();
            if (!string.IsNullOrEmpty(sql))
            {
                var data = await financierDatabase.ExecuteQuery<T>(sql);
                ReportData = new ObservableCollection<T>(data);
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
            if (Payee.ID.HasValue)
                stringBuilder.Append(string.Format(" {1} ( payee_id = {0} )", Payee.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (Category.ID.HasValue)
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
)", Category.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (Project.ID.HasValue)
                stringBuilder.Append(string.Format(" {1} ( project_id = {0} )", Project.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (Account.ID.HasValue)
                stringBuilder.Append(string.Format(" {1} ( from_account_id = {0} )", Account.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            return stringBuilder.ToString();
        }
    }
}