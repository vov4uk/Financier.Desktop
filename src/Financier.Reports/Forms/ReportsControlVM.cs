using Financier.DataAccess.Abstractions;
using Financier.Reports.Common;
using Financier.Reports.Reports;
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
            ReportNode income_outcome = new ReportNode("Приход/расход за период")
            {
                Child = new List<ReportNode>
                {
                    new ReportNode(typeof(ReportByPeriodMonthCrcVM))
                }
            };

            ReportNode structure = new ReportNode("Структура")
            {
                Child = new List<ReportNode>
                {
                    new ReportNode(typeof(ReportStructureActivesVM)),
                    new ReportNode(typeof(ReportStructureDebitVM)),
                    new ReportNode(typeof(ReportStructureCreditVM)),
                }
            };

            ReportNode dynam = new ReportNode("Динамика")
            {
                Child = new List<ReportNode>
                {
                    new ReportNode(typeof(ReportDynamicDebitCretitPayeeVM)),
                    new ReportNode(typeof(ReportDynamicRestVM))
                }
            };

            reportsInfo = new List<ReportNode>
            {
                new ReportNode("Все отчеты")
                {
                    Child = new List<ReportNode>
                    {
                        income_outcome,
                        structure,
                        dynam
                    }
                }
            };
        }

        private RelayCommand _closeReportCommand;
        private RelayCommand _openReportCommand;
        private ObservableCollection<object> _reportsVM;
        private List<ReportNode> reportsInfo;
        private object _selectedReport;
        public ICommand CloseReportCommand => _closeReportCommand ?? (_closeReportCommand = new RelayCommand(p => CloseReport()));

        public ICommand OpenReportCommand => _openReportCommand ?? (_openReportCommand = new RelayCommand(p => OpenReport((string)p)));

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
                RaisePropertyChanged(nameof(SelectedReport));
            }
        }

        public void CloseReport()
        {
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