using System;

namespace Financier.DataAccess.Utils
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}
