using Financier.Desktop.Wizards.RecipesWizard.View;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Financier.Desktop.Helpers
{
    public static class RecipiesHelper
    {
        public static string FormatText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var array = text.Split(new[] { Environment.NewLine }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var singleLine = string.Join(' ', array);
            Regex numberRegex = new Regex(RecipesFormatter.Pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var matches = numberRegex.Matches(singleLine).Select(x => x.Value);

            foreach (var match in matches)
            {
                singleLine = singleLine.Replace(match + " ", match.Replace(" ", "-") + Environment.NewLine);
            }

            return singleLine;
        }
    }
}
