using Financier.Common.Attribute;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Financier.Reports
{
    public class ReportsControlVM : BindableBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly IFinancierDatabase financierDatabase;

        public ReportsControlVM(IFinancierDatabase financierDatabase)
        {
            this.financierDatabase = financierDatabase;
            BuildReportsTree();
        }

        private void BuildReportsTree()
        {
            TreeNode income_outcome = new TreeNode("Income / expense for the period")
            {
                Child = new List<TreeNode>
                {
                    new TreeNode(typeof(ReportByPeriodMonthCrcVM)),
                }
            };

            TreeNode structure = new TreeNode("Structure")
            {
                Child = new List<TreeNode>
                {
                    new TreeNode(typeof(ReportStructureActivesVM)),
                    new TreeNode(typeof(ReportStructureIncomeExpenseVM)),
                    new TreeNode(typeof(ByCategoryReportVM)),
                }
            };

            TreeNode dynam = new TreeNode("Dynamics")
            {
                Child = new List<TreeNode>
                {
                    new TreeNode(typeof(ReportDynamicDebitCretitPayeeVM)),
                    new TreeNode(typeof(ReportDynamicRestVM))
                }
            };

            reportsInfo = new List<TreeNode>
            {
                income_outcome,
                structure,
                dynam
            };
        }

        private DelegateCommand<BindableBase> _closeReportCommand;
        private DelegateCommand<string> _openReportCommand;
        private ObservableCollection<BindableBase> _reportsVM;
        private List<TreeNode> reportsInfo;
        private BindableBase _selectedReport;

        public ICommand CloseReportCommand => _closeReportCommand ??= new DelegateCommand<BindableBase>(CloseReport);

        public ICommand OpenReportCommand => _openReportCommand ??= new DelegateCommand<string>(OpenReport);

        public List<TreeNode> ReportsInfo
        {
            get
            {
                return reportsInfo;
            }
        }

        public ObservableCollection<BindableBase> ReportsVM
        {
            get => _reportsVM ??= new ObservableCollection<BindableBase>();
            set
            {
                if (_reportsVM == value)
                    return;
                _reportsVM = value;
                RaisePropertyChanged(nameof(ReportsVM));
            }
        }

        public BindableBase SelectedReport
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

        public void CloseReport(BindableBase selected)
        {
            if (selected != null)
            {
                Logger.Info($"Close report -> {selected.GetType().Name}");
                ReportsVM.Remove(selected);
            }
        }

        public void OpenReport(string reportType)
        {
            BindableBase existingReport = ReportsVM.FirstOrDefault(p => p.GetType().ToString() == reportType);
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
                        BindableBase newReport = (BindableBase)constructor.Invoke(new[] { financierDatabase });
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