using Financier.Desktop.Wizards.RecipesWizard.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Financier.Desktop.Helpers
{
    public static class RecipiesHelper
    {
        private const int maxLineLength = 150;

        public static string FormatText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            var array = text.Split(new[] { Environment.NewLine }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var singleLine = string.Join(' ', array);
            Regex numberRegex = new Regex(RecipesFormatter.Pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var matches = numberRegex.Matches(singleLine);

            int currentPosition = 0;

            StringBuilder sb = new StringBuilder();

            foreach (Match match in matches)
            {
                var line = singleLine.Substring(currentPosition, match.Index - currentPosition);
                currentPosition = match.Index + match.Length;

                var lines = line.Chunk(maxLineLength)
                    .Select(x => new string(x))
                    .ToArray();

                if (lines.Any())
                {
                    lines[lines.Length - 1] = lines[lines.Length - 1] + match.Value.Replace(" ", "-");

                    foreach (var item in lines)
                    {
                        sb.AppendLine(item.Trim());
                    }
                }
                else
                {
                    sb.AppendLine(match.Value.Replace(" ", "-"));
                }
            }

            sb.AppendLine(singleLine.Substring(currentPosition).Trim());

            return sb.ToString();
        }
    }
}
