using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common;
using Financier.Common.Entities;
using Financier.Common.Localization;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Views.Controls;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class CategoriesVM : EntityBaseVM<CategoryTreeModel>
    {
        private readonly List<CategoryTreeModel> _nodes = new();
        private IAsyncCommand _moveBottomCommand;
        private IAsyncCommand _moveDownCommand;
        private IAsyncCommand _moveTopCommand;
        private IAsyncCommand _moveUpCommand;
        private int _restoreSelectedId;

        private IAsyncCommand _sortByTitleCommand;

        public CategoriesVM(IFinancierDatabase db, IDialogWrapper dialogWrapper)
                    : base(db, dialogWrapper)
        {
        }

        public IAsyncCommand MoveBottomCommand => _moveBottomCommand ??= new AsyncCommand(
            () => MoveAsync((siblings, pos) => { if (pos >= siblings.Count - 1) return; var n = siblings[pos]; siblings.RemoveAt(pos); siblings.Add(n); }),
            () => IsAtPosition(SelectedValue, atStart: true));

        public IAsyncCommand MoveDownCommand => _moveDownCommand ??= new AsyncCommand(
            () => MoveAsync((siblings, pos) => { if (pos >= siblings.Count - 1) return; (siblings[pos], siblings[pos + 1]) = (siblings[pos + 1], siblings[pos]); }),
            () => IsAtPosition(SelectedValue, atStart: true));

        public IAsyncCommand MoveTopCommand => _moveTopCommand ??= new AsyncCommand(
            () => MoveAsync((siblings, pos) => { if (pos <= 0) return; var n = siblings[pos]; siblings.RemoveAt(pos); siblings.Insert(0, n); }),
            () => IsAtPosition(SelectedValue, atStart: false));

        public IAsyncCommand MoveUpCommand => _moveUpCommand ??= new AsyncCommand(
            () => MoveAsync((siblings, pos) => { if (pos <= 0) return; (siblings[pos], siblings[pos - 1]) = (siblings[pos - 1], siblings[pos]); }),
            () => IsAtPosition(SelectedValue, atStart: false));

        public IAsyncCommand SortByTitleCommand => _sortByTitleCommand ??= new AsyncCommand(
            () => MoveAsync((siblings, _) => SortByTitle(siblings)),
            () => SelectedValue != null && _nodes.Contains(SelectedValue));

        protected override Task OnAdd() => OpenCategoryDialogAsync(0);

        protected override Task OnDelete(CategoryTreeModel item) => throw new NotImplementedException();

        protected override Task OnEdit(CategoryTreeModel item) => OpenCategoryDialogAsync(item.Id);

        protected override void OnSelectedValueChanged()
        {
            base.OnSelectedValueChanged();
            MoveTopCommand.RaiseCanExecuteChanged();
            MoveUpCommand.RaiseCanExecuteChanged();
            MoveDownCommand.RaiseCanExecuteChanged();
            MoveBottomCommand.RaiseCanExecuteChanged();
            SortByTitleCommand.RaiseCanExecuteChanged();
        }

        protected override Task RefreshData()
        {
            var expandedIds = CollectExpandedIds(_nodes);
            _nodes.Clear();
            InitializeNodes(_nodes, DbManual.Category.Where(x => x.Id > 0).OrderBy(x => x.Left).ToList(), 0);
            RestoreExpandedIds(_nodes, expandedIds);
            CategoryTreeModel restoredNode = null;
            if (_restoreSelectedId > 0)
            {
                restoredNode = FindAndMarkSelected(_nodes, _restoreSelectedId);
                _restoreSelectedId = 0;
            }
            Entities = new ObservableCollection<CategoryTreeModel>(_nodes);
            if (restoredNode != null)
                SelectedValue = restoredNode;
            return Task.CompletedTask;
        }

        private static HashSet<int> CollectExpandedIds(List<CategoryTreeModel> nodes)
        {
            var result = new HashSet<int>();
            foreach (var node in nodes)
            {
                if (node.IsExpanded) result.Add(node.Id);
                if (node.SubCategoties?.Count > 0)
                    result.UnionWith(CollectExpandedIds(node.SubCategoties));
            }
            return result;
        }

        private static CategoryTreeModel FindAndMarkSelected(List<CategoryTreeModel> nodes, int id)
        {
            foreach (var node in nodes)
            {
                if (node.Id == id) { node.IsSelected = true; return node; }
                if (node.SubCategoties?.Count > 0)
                {
                    var found = FindAndMarkSelected(node.SubCategoties, id);
                    if (found != null) return found;
                }
            }
            return null;
        }

        private static (List<CategoryTreeModel> siblings, int pos) FindSiblingsInSubTree(List<CategoryTreeModel> nodes, CategoryTreeModel target)
        {
            foreach (var node in nodes)
            {
                var pos = node.SubCategoties?.IndexOf(target) ?? -1;
                if (pos >= 0) return (node.SubCategoties, pos);
                if (node.SubCategoties?.Count > 0)
                {
                    var result = FindSiblingsInSubTree(node.SubCategoties, target);
                    if (result.siblings != null) return result;
                }
            }
            return (null, -1);
        }

        private static int ReIndex(List<CategoryTreeModel> nodes, int left)
        {
            foreach (var node in nodes)
            {
                node.Left = left;
                node.Right = node.SubCategoties?.Count > 0
                    ? ReIndex(node.SubCategoties, left + 1)
                    : left + 1;
                left = node.Right + 1;
            }
            return left;
        }

        private static void RestoreExpandedIds(List<CategoryTreeModel> nodes, HashSet<int> expandedIds)
        {
            foreach (var node in nodes)
            {
                node.IsExpanded = expandedIds.Contains(node.Id);
                if (node.SubCategoties?.Count > 0)
                    RestoreExpandedIds(node.SubCategoties, expandedIds);
            }
        }
        private static void SortByTitle(List<CategoryTreeModel> nodes)
        {
            nodes.Sort((a, b) => string.Compare(
                a.Title?.TrimStart('-') ?? string.Empty,
                b.Title?.TrimStart('-') ?? string.Empty,
                StringComparison.CurrentCultureIgnoreCase));
            foreach (var node in nodes.Where(n => n.SubCategoties?.Count > 0))
                SortByTitle(node.SubCategoties);
        }

        private static void UpdateCategoriesFromTree(List<CategoryTreeModel> nodes, List<Category> allCategories)
        {
            foreach (var node in nodes)
            {
                var cat = allCategories.FirstOrDefault(x => x.Id == node.Id);
                if (cat != null) { cat.Left = node.Left; cat.Right = node.Right; }
                if (node.SubCategoties?.Count > 0)
                    UpdateCategoriesFromTree(node.SubCategoties, allCategories);
            }
        }

        private (List<CategoryTreeModel> siblings, int pos) FindSiblings(CategoryTreeModel node)
        {
            int pos = _nodes.IndexOf(node);
            if (pos >= 0) return (_nodes, pos);
            return FindSiblingsInSubTree(_nodes, node);
        }

        private void InitializeNodes(List<CategoryTreeModel> nodes, List<CategoryModel> categories, int level)
        {
            foreach (var (category, subNode) in from category in categories.OrderBy(x => x.Left)
                                                where !nodes.Exists(x => x.Right > category.Left)
                                                let subNode = new CategoryTreeModel
                                                {
                                                    Id = category.Id ?? 0,
                                                    Left = category.Left,
                                                    Right = category.Right,
                                                    Title = new string('-', level) + category.Title,
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

        private async Task InsertCategoryAsync(CategoryDto dto)
        {
            using var uow = db.CreateUnitOfWork();
            var repo = uow.GetRepository<Category>();
            var allCategories = await repo.GetAllAsync();

            int newLeft, newRight;

            if (dto.ParentId > 0)
            {
                var parent = allCategories.FirstOrDefault(x => x.Id == dto.ParentId);
                if (parent != null)
                {
                    int pr = parent.Right;
                    newLeft = pr;
                    newRight = pr + 1;

                    foreach (var cat in allCategories)
                    {
                        bool changed = false;
                        if (cat.Left >= pr) { cat.Left += 2; changed = true; }
                        if (cat.Right >= pr) { cat.Right += 2; changed = true; }
                        if (changed) await repo.UpdateAsync(cat);
                    }
                }
                else
                {
                    int maxRight = allCategories.Any() ? allCategories.Max(x => x.Right) : 0;
                    newLeft = maxRight + 1;
                    newRight = maxRight + 2;
                }
            }
            else
            {
                int maxRight = allCategories.Any() ? allCategories.Max(x => x.Right) : 0;
                newLeft = maxRight + 1;
                newRight = maxRight + 2;
            }

            var newCategory = new Category
            {
                Id = 0,
                Title = dto.Title,
                Type = dto.IsIncome ? 1 : 0,
                IsActive = true,
                Left = newLeft,
                Right = newRight,
                UpdatedOn = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                LastProjectId = 0,
                LastLocationId = 0,
            };

            await repo.AddAsync(newCategory);
            await uow.SaveChangesAsync();
        }

        private bool IsAtPosition(CategoryTreeModel node, bool atStart)
        {
            if (node == null) return false;
            var (siblings, pos) = FindSiblings(node);
            if (siblings == null) return false;
            return atStart ? pos < siblings.Count - 1 : pos > 0;
        }

        private async Task MoveAsync(Action<List<CategoryTreeModel>, int> moveOp)
        {
            if (SelectedValue == null) return;
            var (siblings, pos) = FindSiblings(SelectedValue);
            if (siblings == null) return;
            _restoreSelectedId = SelectedValue.Id;
            moveOp(siblings, pos);
            ReIndex();
            await SaveTreeAsync();
            DbManual.ResetManuals(nameof(Category));
            await DbManual.SetupAsync(db);
            await RefreshData();
        }

        private async Task OpenCategoryDialogAsync(int id)
        {
            var allCategories = DbManual.Category.Where(x => x.Id > 0).ToList();

            Category category;
            int parentId = 0;

            if (id != 0)
            {
                category = await db.GetOrCreateAsync<Category>(id);
                var parentModel = allCategories
                    .Where(x => x.Left < category.Left && x.Right > category.Right)
                    .OrderByDescending(x => x.Left)
                    .FirstOrDefault();
                parentId = parentModel?.Id ?? 0;
            }
            else
            {
                category = new Category { Id = 0 };
            }

            var dto = new CategoryDto(category, parentId);

            List<CategoryModel> availableParents;
            if (id == 0)
            {
                availableParents = new List<CategoryModel>(allCategories);
            }
            else
            {
                availableParents = allCategories
                    .Where(x => x.Left < category.Left || x.Right > category.Right)
                    .ToList();
            }
            availableParents.Insert(0, new CategoryModel());

            var vm = new CategoryControlVM(dto, availableParents);
            var result = dialogWrapper.ShowDialog<CategoryControl>(vm, 300, 400, LocalizationService.Instance.category);

            var updated = result as CategoryDto;
            if (updated != null)
            {
                if (id == 0)
                {
                    await InsertCategoryAsync(updated);
                }
                else
                {
                    category.Title = updated.Title;
                    category.Type = updated.IsIncome ? 1 : 0;
                    await db.InsertOrUpdateAsync(new[] { category });
                }

                DbManual.ResetManuals(nameof(Category));
                await DbManual.SetupAsync(db);
                await RefreshData();
            }
        }
        private void ReIndex()
        {
            if (!_nodes.Any()) return;
            var minLeft = _nodes.Min(n => n.Left);
            ReIndex(_nodes, minLeft > 0 ? minLeft : 1);
        }
        private async Task SaveTreeAsync()
        {
            using var uow = db.CreateUnitOfWork();
            var repo = uow.GetRepository<Category>();
            var allCategories = await repo.GetAllAsync();
            UpdateCategoriesFromTree(_nodes, allCategories);
            foreach (var cat in allCategories.Where(c => c.Id > 0))
                await repo.UpdateAsync(cat);
            await uow.SaveChangesAsync();
        }
    }
}
