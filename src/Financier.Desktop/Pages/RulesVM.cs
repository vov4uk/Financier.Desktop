using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.Desktop.Helpers;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class RulesVM : EntityBaseVM<RulesModel>
    {
        public RulesVM(IFinancierDatabase db, IDialogWrapper dialogWrapper) : base(db, dialogWrapper)
        {
        }

        protected override Task OnAdd() => throw new System.NotImplementedException();

        protected override Task OnDelete(RulesModel item) => throw new System.NotImplementedException();

        protected override Task OnEdit(RulesModel item) => throw new System.NotImplementedException();

        protected override Task RefreshData()
        {
            return Task.CompletedTask;
        }
    }
}
