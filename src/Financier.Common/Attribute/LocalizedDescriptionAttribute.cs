using System.ComponentModel;
using System.Resources;
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

    public class LocalizedMccDescriptionAttribute
         : DescriptionAttribute
    {
        private static ResourceManager _resourceManager;
        private readonly string _resourceKey;

        static LocalizedMccDescriptionAttribute()
        {
            _resourceManager = new ResourceManager(typeof(ResourcesMcc));
        }

        public LocalizedMccDescriptionAttribute(string resourceKey)
        {
            _resourceKey = resourceKey;
        }

        public override string Description => GetDescription();

        private string GetDescription()
        {
            return _resourceManager.GetString(_resourceKey, LocalizationService.Instance.CurrentCulture)!; ;
        }
    }
}
