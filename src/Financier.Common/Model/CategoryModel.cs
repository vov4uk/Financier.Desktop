using Financier.DataAccess.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Common.Model
{
    public class CategoryModel : BaseModel
    {
        [Column("_id")]
        public long? Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        //public string Title => (title ?? string.Empty).PadLeft((title ?? string.Empty).Length + (int)Level, '-');

        [Column("level")]
        public long Level { get; set; }

        [Column("left")]
        public long Left { get; set; }

        [Column("right")]
        public long Right { get; set; }

        [Column("type")]
        public long Type { get; set; }

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