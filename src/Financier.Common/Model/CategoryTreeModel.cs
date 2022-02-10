using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class CategoryTreeModel : BaseModel
    {
        public int Id { get; set; }
        public int Right { get; set; }
        public string Title { get; set; }
        public List<CategoryTreeModel> SubCategoties { get; set; }
    }
}
