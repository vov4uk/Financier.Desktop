using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace fcrd
{
    internal class ReportsControlVM : BaseViewModel
    {
        private RelayCommand _closeReportCommand;
        private RelayCommand _openReportCommand;
        private ObservableCollection<object> _reportsVM;
        private object _selectedReport;
        public ICommand CloseReportCommand => this._closeReportCommand ?? (this._closeReportCommand = new RelayCommand(p => this.CloseReport()));

        public ICommand OpenReportCommand => this._openReportCommand ?? (this._openReportCommand = new RelayCommand(p => this.OpenReport((string)p)));

        public ObservableCollection<ReportNode> ReportsInfo
        {
            get
            {
                ObservableCollection<ReportNode> reportsInfo = new ObservableCollection<ReportNode>();
                ObservableCollection<ReportNode> observableCollection1 = reportsInfo;
                ReportNode reportNode1 = new ReportNode("Все отчеты");
                ReportNode reportNode2 = reportNode1;
                ObservableCollection<ReportNode> observableCollection2 = new ObservableCollection<ReportNode>();
                ObservableCollection<ReportNode> observableCollection3 = observableCollection2;
                ReportNode reportNode3 = new ReportNode("Приход/расход за период");
                ReportNode reportNode4 = reportNode3;
                ObservableCollection<ReportNode> observableCollection4 = new ObservableCollection<ReportNode>();
                observableCollection4.Add(new ReportNode(typeof(ReportByPeriodMonthCrcVM)));
                ObservableCollection<ReportNode> observableCollection5 = observableCollection4;
                reportNode4.Child = observableCollection5;
                ReportNode reportNode5 = reportNode3;
                observableCollection3.Add(reportNode5);
                ObservableCollection<ReportNode> observableCollection6 = observableCollection2;
                ReportNode reportNode6 = new ReportNode("Структура");
                ReportNode reportNode7 = reportNode6;
                ObservableCollection<ReportNode> observableCollection7 = new ObservableCollection<ReportNode>();
                observableCollection7.Add(new ReportNode(typeof(ReportStructureActivesVM)));
                observableCollection7.Add(new ReportNode(typeof(ReportStructureDebitVM)));
                observableCollection7.Add(new ReportNode(typeof(ReportStructureCreditVM)));
                ObservableCollection<ReportNode> observableCollection8 = observableCollection7;
                reportNode7.Child = observableCollection8;
                ReportNode reportNode8 = reportNode6;
                observableCollection6.Add(reportNode8);
                ObservableCollection<ReportNode> observableCollection9 = observableCollection2;
                ReportNode reportNode9 = new ReportNode("Динамика");
                ReportNode reportNode10 = reportNode9;
                ObservableCollection<ReportNode> observableCollection10 = new ObservableCollection<ReportNode>();
                observableCollection10.Add(new ReportNode(typeof(ReportDynamicDebitCretitPayeeVM)));
                observableCollection10.Add(new ReportNode(typeof(ReportDynamicRestVM)));
                ObservableCollection<ReportNode> observableCollection11 = observableCollection10;
                reportNode10.Child = observableCollection11;
                ReportNode reportNode11 = reportNode9;
                observableCollection9.Add(reportNode11);
                ObservableCollection<ReportNode> observableCollection12 = observableCollection2;
                reportNode2.Child = observableCollection12;
                ReportNode reportNode12 = reportNode1;
                observableCollection1.Add(reportNode12);
                return reportsInfo;
            }
        }

        public ObservableCollection<object> ReportsVM
        {
            get => this._reportsVM ?? (this._reportsVM = new ObservableCollection<object>());
            set
            {
                if (this._reportsVM == value)
                    return;
                this._reportsVM = value;
                this.OnPropertyChanged(nameof(ReportsVM));
            }
        }

        public object SelectedReport
        {
            get => this._selectedReport;
            set
            {
                if (this._selectedReport == value)
                    return;
                this._selectedReport = value;
                this.OnPropertyChanged(nameof(SelectedReport));
            }
        }

        public void CloseReport()
        {
            this.ReportsVM.Remove(this.SelectedReport);
            this.SelectedReport = this.ReportsVM.FirstOrDefault();
        }

        public void OpenReport(string reportType)
        {
            object obj1 = this.ReportsVM.Where(p => p.GetType().ToString() == reportType).FirstOrDefault();
            if (obj1 != null)
            {
                this.SelectedReport = obj1;
            }
            else
            {
                Type type = Type.GetType(reportType, false, true);
                if (type != null)
                {
                    ConstructorInfo constructor = type.GetConstructor(new Type[0]);
                    if (constructor != null)
                    {
                        object obj2 = constructor.Invoke(new object[0]);
                        HeaderAttribute customAttribute = (HeaderAttribute)Attribute.GetCustomAttribute(type, typeof(HeaderAttribute));
                        obj2.GetType().GetProperty("Header").SetValue(obj2, customAttribute.Header, null);
                        this.ReportsVM.Add(obj2);
                        this.SelectedReport = obj2;
                    }
                }
            }
        }
    }
}