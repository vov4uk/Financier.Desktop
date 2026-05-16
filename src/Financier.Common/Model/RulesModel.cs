using Financier.DataAccess.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class RulesModel : BaseModel
    {
        [DisplayName("Id")]
        public int? Id { get; set; }

        [DisplayName("Condition")]
        public string Condition { get; set; }

        [DisplayName("Action")]
        public string Action { get; set; }

        public RulesModel() { }
    }
}