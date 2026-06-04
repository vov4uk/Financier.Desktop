using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.Pages.Dialogs;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class RulesVM : EntityBaseVM<RuleModel>
    {
        public RulesVM(IFinancierDatabase db, IDialogWrapper dialogWrapper) : base(db, dialogWrapper)
        {
        }

        protected override Task OnAdd() => OpenRulesDialogAsync(0);

        protected override Task OnDelete(RuleModel item) => OnRuleDelete(item.Id ?? 0);

        protected override Task OnEdit(RuleModel item) => OpenRulesDialogAsync(item.Id ?? 0);

        protected override async Task RefreshData()
        {
            await DbManual.SaveRulesAsync();
            await DbManual.LoadRulesAsync();
            Entities = new ObservableCollection<RuleModel>(DbManual.Rules.OrderBy(r => r.Created));
            foreach (var item in Entities)
            {
                item.UpdateTitle();
            }
        }

        private async Task OnRuleDelete(int id)
        {
            DbManual.Rules.Remove(DbManual.Rules.FirstOrDefault(r => r.Id == id));
            await RefreshData();
        }

        private async Task OpenRulesDialogAsync(int id)
        {

            RuleDTO rule = null;

            if (id != 0)
            {
                var ruleModel = DbManual.Rules.FirstOrDefault(r => r.Id == id);
                if (ruleModel != null)
                {
                    rule = new RuleDTO
                    {
                        CategoryId = ruleModel.CategoryId,
                        LocationId = ruleModel.LocationId,
                        PayeeId = ruleModel.PayeeId,
                        ProjectId = ruleModel.ProjectId,
                        Description = ruleModel.Description,
                        Condition = ruleModel.Condition,
                        Created = ruleModel.Created,
                        IsActive = ruleModel.IsActive
                    };
                }
                else
                {
                    return;
                }

            }
            else
            {
                rule = new RuleDTO()
                {
                    Description = "Description here",
                    Condition = "Description contains",
                    Created = DateTime.Now,
                    IsActive = true
                };
            }

            RuleControlVM ruleVm = new RuleControlVM(rule);

            var result = dialogWrapper.ShowDialog<RuleControl>(ruleVm, 380, 400, "Rule");

            var updatedItem = result as RuleDTO;
            if (updatedItem != null)
            {
                int newId = 0;
                if (id == 0)
                {
                    newId = DbManual.Rules.Select(r => r.Id).Max().Value + 1;
                }
                else
                {
                    var existingRule = DbManual.Rules.FirstOrDefault(r => r.Id == id);
                    DbManual.Rules.Remove(existingRule);
                    newId = id;
                }
                DbManual.Rules.Add(new RuleModel
                {
                    Description = updatedItem.Description,
                    CategoryId = updatedItem.CategoryId,
                    Condition = updatedItem.Condition,
                    Created = updatedItem.Created,
                    Id = newId,
                    IsActive = updatedItem.IsActive,
                    LocationId = updatedItem.LocationId,
                    PayeeId = updatedItem.PayeeId,
                    ProjectId = updatedItem.ProjectId
                });

                await RefreshData();
            }
        }
    }
}
