using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.Desktop.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModel
{
    public class CategoriesVM : EntityBaseVM<CategoryTreeModel>
    {
        private readonly List<CategoryTreeModel> _nodes = new List<CategoryTreeModel>();

        public CategoriesVM(IFinancierDatabase db, IDialogWrapper dialogWrapper)
            : base(db, dialogWrapper)
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
            foreach (var category in categories.OrderBy(x => x.Left))
            {
                if (!nodes.Any(x => x.Right > category.Left))
                {
                    var subNode = new CategoryTreeModel
                    {
                        Id = (int)category.Id,
                        Title = new string('-', level) + category.Title,
                        Right = (int)category.Right,
                        SubCategoties = new()
                    };
                    nodes.Add(subNode);

                    var sub = categories.Where(x => x.Left > category.Left && x.Right < category.Right).ToList();
                    if (sub.Any())
                    {
                        InitializeNodes(subNode.SubCategoties, sub, level + 1);
                    }
                }
            }
        }
    }
}
