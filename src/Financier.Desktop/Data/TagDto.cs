using Financier.DataAccess.Data;
using Prism.Mvvm;

namespace Financier.Desktop.Data
{
    public class TagDto : BindableBase
    {
        private bool isActive;
        private string title;

        public TagDto(Tag proj)
        {
            this.Title = proj.Title;
            this.IsActive = proj.IsActive;
        }

        public TagDto(string title, bool isActive)
        {
            this.Title = title;
            this.IsActive = isActive;
        }

        public bool IsActive
        {
            get => isActive;
            set { SetProperty(ref isActive, value, nameof(IsActive)); }
        }

        public string Title
        {
            get => title;
            set { SetProperty(ref title, value, nameof(Title)); }
        }
    }
}
