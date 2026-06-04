using Financier.Desktop.Data;
using Prism.Commands;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class TagControlVM : DialogBaseVM
    {
        private DelegateCommand _clearTitleCommand;

        public TagControlVM(TagDTO entity)
        {
            this.Entity = entity;
        }

        public DelegateCommand ClearTitleCommand => _clearTitleCommand ??= new DelegateCommand(() => { Entity.Title = default; });

        public TagDTO Entity { get; }
        public override object OnRequestSave()
        {
            return Entity;
        }
    }
}
