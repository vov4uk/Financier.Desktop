using Financier.DataAccess.Data;
using Prism.Mvvm;

namespace Financier.Desktop.Data
{
    public class CategoryDto : BindableBase
    {
        private string title;
        private bool isIncome;
        private int parentId;

        public CategoryDto() { }

        public CategoryDto(Category category, int parentId)
        {
            Id = category.Id;
            Left = category.Left;
            Right = category.Right;
            Title = category.Title;
            IsIncome = (category.Type & 1) != 0;
            this.parentId = parentId;
        }

        public int Id { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public string Title
        {
            get => title;
            set { SetProperty(ref title, value, nameof(Title)); }
        }

        public bool IsIncome
        {
            get => isIncome;
            set { SetProperty(ref isIncome, value, nameof(IsIncome)); }
        }

        public int ParentId
        {
            get => parentId;
            set { SetProperty(ref parentId, value, nameof(ParentId)); }
        }
    }
}
