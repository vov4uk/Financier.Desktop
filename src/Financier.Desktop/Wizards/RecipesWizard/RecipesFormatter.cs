using System;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace Financier.Desktop.Wizards.RecipesWizard.View
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class RecipesFormatter : ITextFormatter
    {
        public const string Pattern = @"((\+|\-)?)\d+(?:(\.|\,)?\d+)(\s+|-)(A|a|а|А|Б|б)";
        private const string NumbersRegex = @"\d+";
        public const string Space = " ";

        public string GetText(FlowDocument document)
        {
            return new TextRange(document.ContentStart, document.ContentEnd).Text;
        }

        public void SetText(FlowDocument document, string text)
        {
            Paragraph p = new Paragraph();

            foreach (var line in text.Split(Environment.NewLine))
            {
                var words = line.Trim().Split(Space);
                for (int i = 0; i < words.Length; i++)
                {
                    string word = words[i];
                    if (Regex.IsMatch(word, Pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000)))
                    {
                        p.Inlines.Add(GetRun(word, Brushes.DarkRed, Brushes.Yellow));
                        if (i != words.Length - 1) //not last word
                        {
                            p.Inlines.Add(GetRun(Space, Brushes.DarkRed, Brushes.Yellow));
                        }
                    }
                    else
                    {
                        p.Inlines.Add(GetRun(word, Brushes.DarkViolet, Brushes.LightGreen));
                        if (i != words.Length - 1) //not last word
                        {
                            p.Inlines.Add(GetRun(Space, Brushes.DarkRed, Brushes.Yellow));
                        }
                    }
                }
                p.Inlines.Add(GetDefaultRun(Environment.NewLine));
            }

            document.Blocks.Clear();
            document.PageWidth = 2500;
            document.Blocks.Add(p);
        }

        private static Run GetRun(string word, Brush foreground, Brush background)
        {
            if (Regex.IsMatch(word, NumbersRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000)))
            {
                return GetHiglightedRun(word, foreground, background);
            }
            return GetDefaultRun(word);
        }

        private static Run GetHiglightedRun(string word, Brush foreground, Brush background)
        {
            return new Run
            {
                Text = word,
                Foreground = foreground,
                Background = background
            };
        }

        private static Run GetDefaultRun(string word)
        {
            return new Run
            {
                Text = word
            };
        }
    }
}
