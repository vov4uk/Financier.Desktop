using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace Financier.Desktop.Wizards.RecipesWizard.View
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class RecipesFormatter : ITextFormatter
    {
        public const string Pattern = @"((\+|\-)?)\d+(?:(\.|\,)?\d+)(\s+)(A|a|а|А)";
        private const string NumbersRegex = @"\d+";
        private const string Space = " ";

        public string GetText(FlowDocument document)
        {
            return new TextRange(document.ContentStart, document.ContentEnd).Text;
        }

        public void SetText(FlowDocument document, string text)
        {
            Paragraph p = new Paragraph();

            var lines = text.Split(Environment.NewLine).Where(x => !string.IsNullOrWhiteSpace(x));
            foreach (var line in lines)
            {
                if (Regex.IsMatch(line, Pattern, RegexOptions.IgnoreCase))
                {
                    p.Inlines.AddRange(line.Split(Space).Select(word => GetRun(word, Brushes.DarkRed, Brushes.Yellow)));
                }
                else
                {
                    p.Inlines.AddRange(line.Split(Space).Select(word => GetRun(word, Brushes.DarkViolet, Brushes.LightGreen)));
                }

                p.Inlines.Add(GetDefaultRun(Environment.NewLine));
            }

            document.Blocks.Clear();
            document.PageWidth = 2500;
            document.Blocks.Add(p);
        }

        private static Run GetRun(string word, Brush foreground, Brush background)
        {
            if (Regex.IsMatch(word, NumbersRegex, RegexOptions.IgnoreCase))
            {
                return GetHiglightedRun(word, foreground, background);
            }
            return GetDefaultRun(word);
        }

        private static Run GetHiglightedRun(string word, Brush foreground, Brush background)
        {
            return new Run
            {
                Text = word + Space,
                Foreground = foreground,
                Background = background
            };
        }

        private static Run GetDefaultRun(string word)
        {
            return new Run
            {
                Text = word + Space
            };
        }
    }
}
