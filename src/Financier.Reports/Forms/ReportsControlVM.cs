using Financier.Reports.Common;
using Financier.Reports.Reports;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Financier.Reports.Forms
{
    internal class ReportsControlVM : BaseViewModel
    {
        private RelayCommand _closeReportCommand;
        private RelayCommand _openReportCommand;
        private ObservableCollection<object> _reportsVM;
        private object _selectedReport;
        public ICommand CloseReportCommand => _closeReportCommand ?? (_closeReportCommand = new RelayCommand(p => CloseReport()));

        public ICommand OpenReportCommand => _openReportCommand ?? (_openReportCommand = new RelayCommand(p => OpenReport((string)p)));

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
            get => _reportsVM ?? (_reportsVM = new ObservableCollection<object>());
            set
            {
                if (_reportsVM == value)
                    return;
                _reportsVM = value;
                OnPropertyChanged(nameof(ReportsVM));
            }
        }

        public object SelectedReport
        {
            get => _selectedReport;
            set
            {
                if (_selectedReport == value)
                    return;
                _selectedReport = value;
                OnPropertyChanged(nameof(SelectedReport));
            }
        }

        public void CloseReport()
        {
            ReportsVM.Remove(SelectedReport);
            SelectedReport = ReportsVM.FirstOrDefault();
        }

        public void OpenReport(string reportType)
        {
            object obj1 = ReportsVM.Where(p => p.GetType().ToString() == reportType).FirstOrDefault();
            if (obj1 != null)
            {
                SelectedReport = obj1;
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
                        ReportsVM.Add(obj2);
                        SelectedReport = obj2;
                    }
                }
            }
        }
    }
}