using System.ComponentModel;
using Financier.Common.Localization;

namespace Financier.Common.Attribute
{
    public class LocalizedDescriptionAttribute
         : DescriptionAttribute
    {
        private readonly string _resourceKey;

        public LocalizedDescriptionAttribute(string resourceKey)
        {
            _resourceKey = resourceKey;
        }

        public override string Description => GetDescription();

        private string GetDescription()
        {
            string result = LocalizationService.Instance[_resourceKey];
            return result;
        }
    }
}
