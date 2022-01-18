using System.Collections.ObjectModel;

namespace Financier.Reports.Common
{
    public static class DbManual
    {
        private static ObservableCollection<AccountModel> _account;
        private static ObservableCollection<CategoryModel> _category;
        private static ObservableCollection<CurrencyModel> _currencies;
        private static ObservableCollection<PayeeModel> _payee;
        private static ObservableCollection<ProjectModel> _project;
        private static ObservableCollection<YearMonths> _yearMonths;
        private static ObservableCollection<Years> _years;

        public static ObservableCollection<AccountModel> Account
        {
            get
            {
                if (_account == null)
                {
                    DB.GetData("select _id, " +
                        "title " +
                        "from account " +
                        "where title is not null " +
                        "order by 2 desc", _account = new ObservableCollection<AccountModel>());
                    _account.Insert(0, new AccountModel());
                }
                return _account;
            }
        }

        public static ObservableCollection<CategoryModel> Category
        {
            get
            {
                if (_category == null)
                {
                    DB.GetData(
"\r\n                    select" +
"\r\n                    _id," +
"\r\n                    title," +
"\r\n                    (select count(*) from category x where x.left < ctx.left and x.[right] > ctx.[right] ) as level" +
"\r\n                    from category ctx" +
"\r\n                    order by left, sort_order", _category = new ObservableCollection<CategoryModel>());
                    _category.Insert(0, new CategoryModel());
                }
                return _category;
            }
        }

        public static ObservableCollection<CurrencyModel> Currencies
        {
            get
            {
                if (_currencies == null)
                {
                    DB.GetData("select * " +
                               "from currency",
                        _currencies = new ObservableCollection<CurrencyModel>());
                    _currencies.Insert(0, new CurrencyModel()
                    {
                        Name = "Все валюты"
                    });
                }
                return _currencies;
            }
        }

        public static ObservableCollection<PayeeModel> Payee
        {
            get
            {
                if (_payee == null)
                {
                    DB.GetData("select _id, " +
                        "title " +
                        "from payee " +
                        "where title is not null " +
                        "order by 2 desc", _payee = new ObservableCollection<PayeeModel>());
                    _payee.Insert(0, new PayeeModel());
                }
                return _payee;
            }
        }

        public static ObservableCollection<ProjectModel> Project
        {
            get
            {
                if (_project == null)
                {
                    DB.GetData("select _id, " +
                        "title " +
                        "from project " +
                        "where title is not null " +
                        "order by 2 desc", _project = new ObservableCollection<ProjectModel>());
                    _project.Insert(0, new ProjectModel());
                }
                return _project;
            }
        }

        public static ObservableCollection<YearMonths> YearMonths
        {
            get
            {
                if (_yearMonths == null)
                {
                    DB.GetData("select distinct date_year as year, " +
                        "date_month as month " +
                        "from transactions " +
                        "order by 1 desc, " +
                        "2 asc", _yearMonths = new ObservableCollection<YearMonths>());
                    _yearMonths.Insert(0, new YearMonths());
                }
                return _yearMonths;
            }
        }

        public static ObservableCollection<Years> Years
        {
            get
            {
                if (_years == null)
                {
                    DB.GetData("select distinct date_year as year " +
                        "from transactions " +
                        "order by 1 desc", _years = new ObservableCollection<Years>());
                    _years.Insert(0, new Years());
                }
                return _years;
            }
        }

        public static void ResetManuals()
        {
            _project = null;
            _category = null;
            _account = null;
            _currencies = null;
            _yearMonths = null;
            _years = null;
        }
    }
}