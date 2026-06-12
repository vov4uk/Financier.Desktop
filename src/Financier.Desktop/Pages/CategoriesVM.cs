using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.Desktop.Helpers;
using Financier.Desktop.Localization;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class CategoriesVM : EntityBaseVM<CategoryTreeModel>
    {
        private readonly List<CategoryTreeModel> _nodes = new List<CategoryTreeModel>();

        public CategoriesVM(IFinancierDatabase db, IDialogWrapper dialogWrapper, LocalizationManager localizationManager)
            : base(db, dialogWrapper, localizationManager)
        {
        }

        protected override Task OnAdd() => throw new System.NotImplementedException();

        protected override Task OnDelete(CategoryTreeModel item) => throw new System.NotImplementedException();

        protected override Task OnEdit(CategoryTreeModel item) => throw new System.NotImplementedException();

        protected override Task RefreshData()
        {
            InitializeNodes(_nodes, DbManual.Category.Where(x => x.Id > 0).OrderBy(x => x.Left).ToList(), 0);
            Entities = new ObservableCollection<CategoryTreeModel>(_nodes);
            return Task.CompletedTask;
        }

        private void InitializeNodes(List<CategoryTreeModel> nodes, List<CategoryModel> categories, int level)
        {
            foreach (var (category, subNode) in from category in categories.OrderBy(x => x.Left)
                                                where !nodes.Exists(x => x.Right > category.Left)
                                                let subNode = new CategoryTreeModel
                                                {
                                                    Id = category.Id ?? 0,
                                                    Title = new string('-', level) + category.Title,
                                                    Right = category.Right,
                                                    SubCategoties = new()
                                                }
                                                select (category, subNode))
            {
                nodes.Add(subNode);

                var sub = categories.Where(x => x.Left > category.Left && x.Right < category.Right).ToList();
                if (sub.Count > 0)
                {
                    InitializeNodes(subNode.SubCategoties, sub, level + 1);
                }
            }
        }
    }
}
