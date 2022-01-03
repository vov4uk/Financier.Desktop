using Financier.Desktop.Data;
using Prism.Commands;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class EntityWithTitleVM : DialogBaseVM
    {
        private DelegateCommand _clearTitleCommand;

        public EntityWithTitleVM(EntityWithTitleDTO entity)
        {
            this.Entity = entity;
        }

        public DelegateCommand ClearTitleCommand
        {
            get { return _clearTitleCommand ??= new DelegateCommand(() => { Entity.Title = default; }); }
        }

        public EntityWithTitleDTO Entity { get; }
        public override object OnRequestSave()
        {
            return Entity;
        }
    }
}
