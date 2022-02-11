using Financier.DataAccess.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class CategoryModel : BaseModel
    {
        [Column("_id")]
        public int? Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("level")]
        public int Level { get; set; }

        [Column("left")]
        public int Left { get; set; }

        [Column("right")]
        public int Right { get; set; }

        [Column("type")]
        public int Type { get; set; }

        public CategoryModel() { }

        public CategoryModel(Category cat)
        {
            Id = cat.Id;
            Title = cat.Title;
            Type = cat.Type;
            Left = cat.Left;
            Right = cat.Right;
        }
    }
}