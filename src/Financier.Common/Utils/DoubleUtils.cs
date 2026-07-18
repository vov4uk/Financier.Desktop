using System;

namespace Financier.Common.Utils
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

        public static double GetDouble(string text)
        {
            double.TryParse(Convert.ToString(text).Replace(',', '.').Replace(" ", string.Empty), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out double retNum);
            return retNum;
        }
    }
}
