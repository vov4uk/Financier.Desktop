using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class ProjectsVM : EntityBaseVM<Project>
    {
        public ProjectsVM(IEnumerable<Project> items) : base(items) {}
    }
}
