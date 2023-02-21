using System;
using System.Collections.Generic;
using System.Text;

namespace Financier.DataAccess.Utils
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}
