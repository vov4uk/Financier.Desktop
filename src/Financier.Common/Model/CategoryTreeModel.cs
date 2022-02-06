using System.Collections.Generic;

namespace Financier.Common.Model
{
    public class CategoryTreeModel : BaseModel
    {
        public int Id { get; set; }
        public int Right { get; set; }
        public string Title { get; set; }
        public List<CategoryTreeModel> SubCategoties { get; set; }
    }
}
