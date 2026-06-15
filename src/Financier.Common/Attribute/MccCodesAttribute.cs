using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Attribute
{
    [ExcludeFromCodeCoverage]
    public class MccCodesAttribute : System.Attribute
    {
        public int[] Codes { get; set; }

        public MccCodesAttribute(params int[] codes) => Codes = codes;
    }
}
