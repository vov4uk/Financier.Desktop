using System.Collections.ObjectModel;

namespace fcrd
{
    public static class DbManual
    {
        private static ObservableCollection<Account> _account;
        private static ObservableCollection<Category> _category;
        private static ObservableCollection<Currency> _currencies;
        private static ObservableCollection<Payee> _payee;
        private static ObservableCollection<Project> _project;
        private static ObservableCollection<YearMonths> _yearMonths;
        private static ObservableCollection<Years> _years;
        public static ObservableCollection<Account> Account
        {
            get
            {
                if (_account == null)
                {
                    DB.GetData("select _id, " +
                        "title " +
                        "from account " +
                        "where title is not null " +
                        "order by 2 desc", _account = new ObservableCollection<Account>());
                    _account.Insert(0, new Account());
                }
                return _account;
            }
        }

        public static ObservableCollection<Category> Category
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
"\r\n                    order by left, sort_order", _category = new ObservableCollection<Category>());
                    _category.Insert(0, new Category());
                }
                return _category;
            }
        }

        public static ObservableCollection<Currency> Currencies
        {
            get
            {
                if (_currencies == null)
                {
                    DB.GetData("select * " +
                        "from currency", _currencies = new ObservableCollection<Currency>());
                    _currencies.Insert(0, new Currency()
                    {
                        Name = "Все валюты"
                    });
                }
                return _currencies;
            }
        }

        public static ObservableCollection<Payee> Payee
        {
            get
            {
                if (_payee == null)
                {
                    DB.GetData("select _id, " +
                        "title " +
                        "from payee " +
                        "where title is not null " +
                        "order by 2 desc", _payee = new ObservableCollection<Payee>());
                    _payee.Insert(0, new Payee());
                }
                return _payee;
            }
        }

        public static ObservableCollection<Project> Project
        {
            get
            {
                if (_project == null)
                {
                    DB.GetData("select _id, " +
                        "title " +
                        "from project " +
                        "where title is not null " +
                        "order by 2 desc", _project = new ObservableCollection<Project>());
                    _project.Insert(0, new Project());
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