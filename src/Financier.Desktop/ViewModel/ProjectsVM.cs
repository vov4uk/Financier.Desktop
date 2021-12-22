using System.Collections.Generic;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    public class ProjectsVM : EntityBaseVM<Project>
    {
        public ProjectsVM(IEnumerable<Project> items) : base(items) {}
    }
}
