using Financier.DataAccess.Abstractions;
using Financier.Reports.Common;
using Financier.Reports.Reports;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Financier.Reports.Forms
{
    public class ReportsControlVM : BindableBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IFinancierDatabase financierDatabase;

        public ReportsControlVM(IFinancierDatabase financierDatabase)
        {
            this.financierDatabase = financierDatabase;
            BuildReportsTree();

            DbManual.ResetManuals();
            DbManual.Setup(financierDatabase).GetAwaiter().GetResult();
        }

        private void BuildReportsTree()
        {
            ReportNode income_outcome = new ReportNode("Income / expense for the period")
            {
                Child = new List<ReportNode>
                {
                    new ReportNode(typeof(ReportByPeriodMonthCrcVM)),
                }
            };

            ReportNode structure = new ReportNode("Structure")
            {
                Child = new List<ReportNode>
                {
                    new ReportNode(typeof(ReportStructureActivesVM)),
                    new ReportNode(typeof(ReportStructureIncomeExpenseVM)),
                    new ReportNode(typeof(ByCategoryReportVM)),
                }
            };

            ReportNode dynam = new ReportNode("Dynamics")
            {
                Child = new List<ReportNode>
                {
                    new ReportNode(typeof(ReportDynamicDebitCretitPayeeVM)),
                    new ReportNode(typeof(ReportDynamicRestVM))
                }
            };

            reportsInfo = new List<ReportNode>
            {
                income_outcome,
                structure,
                dynam
            };
        }

        private DelegateCommand _closeReportCommand;
        private DelegateCommand<string> _openReportCommand;
        private ObservableCollection<object> _reportsVM;
        private List<ReportNode> reportsInfo;
        private object _selectedReport;

        public ICommand CloseReportCommand => _closeReportCommand ?? (_closeReportCommand = new DelegateCommand(CloseReport));

        public ICommand OpenReportCommand => _openReportCommand ?? (_openReportCommand = new DelegateCommand<string>(OpenReport));

        public List<ReportNode> ReportsInfo
        {
            get
            {
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
                RaisePropertyChanged(nameof(ReportsVM));
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
                Logger.Info($"Current report -> {_selectedReport?.GetType()?.Name}");
                RaisePropertyChanged(nameof(SelectedReport));
            }
        }

        public void CloseReport()
        {
            Logger.Info($"Close report -> {_selectedReport?.GetType()?.Name}");
            ReportsVM.Remove(SelectedReport);
            SelectedReport = ReportsVM.FirstOrDefault();
        }

        public void OpenReport(string reportType)
        {
            object existingReport = ReportsVM.Where(p => p.GetType().ToString() == reportType).FirstOrDefault();
            if (existingReport != null)
            {
                SelectedReport = existingReport;
            }
            else
            {
                Type type = Type.GetType(reportType, false, true);
                if (type != null)
                {
                    ConstructorInfo constructor = type.GetConstructors().First();
                    if (constructor != null)
                    {
                        object newReport = constructor.Invoke(new[] { financierDatabase });
                        HeaderAttribute customAttribute = (HeaderAttribute)Attribute.GetCustomAttribute(type, typeof(HeaderAttribute));
                        newReport.GetType().GetProperty("Header").SetValue(newReport, customAttribute.Header, null);
                        ReportsVM.Add(newReport);
                        SelectedReport = newReport;
                    }
                }
            }
        }
    }
}