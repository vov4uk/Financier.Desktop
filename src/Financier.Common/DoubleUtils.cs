using System;

namespace Financier.Common
{
    public static class DoubleUtils
    {
        private const double AmountEpsilon = 1e-6;

        public static bool DoubleEqual(double valueA, double valueB)
        {
            return valueA == valueB; // Math.Abs(valueA - valueB) < AmountEpsilon;
        }

        public static bool DoubleNotEqual(double valueA, double valueB)
        {
            return valueA != valueB; //Math.Abs(valueA - valueB) >= AmountEpsilon;
        }
    }
}
