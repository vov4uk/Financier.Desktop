using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace fcrd
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
            this.ReportData = new ObservableCollection<T>();
        }

        public Project Project
        {
            get => this._project ??= DbManual.Project.FirstOrDefault(p => !p.ID.HasValue);
            set
            {
                this._project = value;
                this.OnPropertyChanged(nameof(Project));
            }
        }

        public Category Category
        {
            get => this._category ??= DbManual.Category.FirstOrDefault(p => !p.ID.HasValue);
            set
            {
                this._category = value;
                this.OnPropertyChanged(nameof(Category));
            }
        }

        public Account Account
        {
            get => this._account ??= DbManual.Account.FirstOrDefault(p => !p.ID.HasValue);
            set
            {
                this._account = value;
                this.OnPropertyChanged(nameof(Account));
            }
        }

        public Payee Payee
        {
            get => this._payee ?? (this._payee = DbManual.Payee.FirstOrDefault(p => !p.ID.HasValue));
            set
            {
                this._payee = value;
                this.OnPropertyChanged(nameof(Payee));
            }
        }

        public Currency CurentCurrency
        {
            get => this._curentCurrency ?? (this._curentCurrency = DbManual.Currencies.FirstOrDefault(p => !p.ID.HasValue));
            set
            {
                this._curentCurrency = value;
                this.OnPropertyChanged(nameof(CurentCurrency));
            }
        }

        public YearMonths StartYearMonths
        {
            get => this._startYearMonths ?? (this._startYearMonths = DbManual.YearMonths.FirstOrDefault(p => !p.Year.HasValue && !p.Month.HasValue));
            set
            {
                this._startYearMonths = value;
                this.OnPropertyChanged(nameof(StartYearMonths));
            }
        }

        public YearMonths EndYearMonths
        {
            get => this._endYearMonths ?? (this._endYearMonths = DbManual.YearMonths.FirstOrDefault(p => !p.Year.HasValue && !p.Month.HasValue));
            set
            {
                this._endYearMonths = value;
                this.OnPropertyChanged(nameof(EndYearMonths));
            }
        }

        public ICommand RefreshDataCommand => (this._refreshDataCommand ?? (this._refreshDataCommand = new RelayCommand(param => this.RefreshData())));

        private void RefreshData()
        {
            string sql = this.GetSql();
            if (string.IsNullOrEmpty(sql))
                return;
            DB.GetData<T>(sql, this.ReportData);
        }

        protected abstract string GetSql();

        protected string GetStandartTrnFilter()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (this.StartYearMonths.Year.HasValue)
                stringBuilder.Append(string.Format(" ((date_year = {0} and date_month >= {1}) or date_year > {0})", this.StartYearMonths.Year, this.StartYearMonths.Month));
            if (this.EndYearMonths.Year.HasValue)
                stringBuilder.Append(string.Format(" {2} ((date_year = {0} and date_month <= {1}) or date_year < {0})", this.EndYearMonths.Year, this.EndYearMonths.Month, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (this.Payee.ID.HasValue)
                stringBuilder.Append(string.Format(" {1} ( payee_id = {0} )", this.Payee.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (this.Category.ID.HasValue)
                stringBuilder.Append(string.Format("{1} category_id in ( select _id from category ctx," +
"\r\n                                (select xxx.left, xxx.right from category xxx where xxx._id = {0}) root " +
"\r\n                                where ctx.left >= root.left and ctx.right <= root.right)", this.Category.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (this.Project.ID.HasValue)
                stringBuilder.Append(string.Format(" {1} ( project_id = {0} )", this.Project.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            if (this.Account.ID.HasValue)
                stringBuilder.Append(string.Format(" {1} ( from_account_id = {0} )", this.Account.ID, stringBuilder.Length != 0 ? " and " : string.Empty));
            return stringBuilder.ToString();
        }
    }
}