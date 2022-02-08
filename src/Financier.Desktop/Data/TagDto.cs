using Financier.DataAccess.Data;
using Prism.Mvvm;

namespace Financier.Desktop.Data
{
    public class TagDto : BindableBase
    {
        private bool isActive;
        private string title;

        public TagDto(IActive proj)
        {
            this.Title = proj.Title;
            this.IsActive = proj.IsActive;
        }

        public TagDto(string title, bool isActive)
        {
            this.Title = title;
            this.IsActive = isActive;
        }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }
    }
}
