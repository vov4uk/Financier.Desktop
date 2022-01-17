using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Financier.Reports.Common
{
    public abstract class BaseReportVM<T> : BaseViewModel where T : BaseReportM, new()
    {
        private Project _project;
        private Category _category;
        private Account _account;
        private Payee _payee;
        private Currency _curentCurrency;
        private YearMonths _startYearMonths;
        private YearMonths _endYearMonths;
        private RelayCommand _refreshDataCommand;

        public string Header { get; set; }

        public ObservableCollection<T> ReportData { get; set; }

        protected BaseReportVM()
        {
            ReportData = new ObservableCollection<T>();
        }

        public Project Project
        {
            get => _project ??= DbManual.Project.FirstOrDefault(p => !p.ID.HasValue);
            set
            {
                _project = value;
                OnPropertyChanged(nameof(Project));
            }
        }

        public Category Category
        {
            get => _category ??= DbManual.Category.FirstOrDefault(p => !p.ID.HasValue);
            set
            {
                _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }

        public Account Account
        {
            get => _account ??= DbManual.Account.FirstOrDefault(p => !p.ID.HasValue);
            set
            {
                _account = value;
                OnPropertyChanged(nameof(Account));
            }
        }

        public Payee Payee
        {
            get => _payee ?? (_payee = DbManual.Payee.FirstOrDefault(p => !p.ID.HasValue));
            set
            {
                _payee = value;
                OnPropertyChanged(nameof(Payee));
            }
        }

        public Currency CurentCurrency
        {
            get => _curentCurrency ?? (_curentCurrency = DbManual.Currencies.FirstOrDefault(p => !p.ID.HasValue));
            set
            {
                _curentCurrency = value;
                OnPropertyChanged(nameof(CurentCurrency));
            }
        }

        public YearMonths StartYearMonths
        {
            get => _startYearMonths ?? (_startYearMonths = DbManual.YearMonths.FirstOrDefault(p => !p.Year.HasValue && !p.Month.HasValue));
            set
            {
                _startYearMonths = value;
                OnPropertyChanged(nameof(StartYearMonths));
            }
        }

        public YearMonths EndYearMonths
        {
            get => _endYearMonths ?? (_endYearMonths = DbManual.YearMonths.FirstOrDefault(p => !p.Year.HasValue && !p.Month.HasValue));
            set
            {
                _endYearMonths = value;
                OnPropertyChanged(nameof(EndYearMonths));
            }
        }

        public ICommand RefreshDataCommand => _refreshDataCommand ?? (_refreshDataCommand = new RelayCommand(param => RefreshData()));

        private void RefreshData()
        {
            string sql = GetSql();
            if (string.IsNullOrEmpty(sql))
                return;
            DB.GetData(sql, ReportData);
        }

        protected abstract string GetSql();

        protected string GetStandartTrnFilter()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (StartYearMonths.Year.HasValue)
                stringBuilder.Append(string.Format(" ((date_year = {0} and date_month >= {1}) or date_year > {0})", StartYearMonths.Year, StartYearMonths.Month));
            if (EndYearMonths.Year.HasValue)
                stringBuilder.Append(string.Format(" {2} ((date_year = {0} and date_month <= {1}) or date_year < {0})", EndYearMonths.Year, EndYearMonths.Month, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (Payee.ID.HasValue)
                stringBuilder.Append(string.Format(" {1} ( payee_id = {0} )", Payee.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (Category.ID.HasValue)
                stringBuilder.Append(string.Format("{1} category_id in ( select _id from category ctx," +
"\r\n                                (select xxx.left, xxx.right from category xxx where xxx._id = {0}) root " +
"\r\n                                where ctx.left >= root.left and ctx.right <= root.right)", Category.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (Project.ID.HasValue)
                stringBuilder.Append(string.Format(" {1} ( project_id = {0} )", Project.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (Account.ID.HasValue)
                stringBuilder.Append(string.Format(" {1} ( from_account_id = {0} )", Account.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            return stringBuilder.ToString();
        }
    }
}