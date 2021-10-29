using System;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using Xceed.Wpf.Toolkit;

namespace Financier.Desktop.Wizards.RecipesWizard.View
{
    public class MyFormatter : ITextFormatter
    {
        public const string Pattern = @"((\+|\-)?)\d+(?:(\.|\,)?\d+)(\s+)A";

        public string GetText(FlowDocument document)
        {
            return new TextRange(document.ContentStart, document.ContentEnd).Text;
        }

        public void SetText(FlowDocument document, string text)
        {
            string reg = @"\d+";
            text = text.Replace("а", "a").Replace("А","A");

            Paragraph p = new Paragraph();

            var lines = text.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (Regex.IsMatch(line, Pattern, RegexOptions.IgnoreCase))
                    {
                        var words = line.Split(" ");
                        foreach (var word in words)
                        {
                            if (Regex.IsMatch(word, reg, RegexOptions.IgnoreCase))
                            {
                                p.Inlines.Add(new Run
                                {
                                    Text = word + " ",
                                    Foreground = System.Windows.Media.Brushes.DarkRed,
                                    Background = System.Windows.Media.Brushes.Yellow
                                });
                            }
                            else
                            {
                                p.Inlines.Add(new Run
                                {
                                    Text = word + " "
                                });
                            }
                        }
                    }
                    else
                    {
                        var words = line.Split(" ");
                        foreach (var word in words)
                        {
                            if (Regex.IsMatch(word, @"\d+", RegexOptions.IgnoreCase))
                            {
                                p.Inlines.Add(new Run
                                {
                                    Text = word + " ",
                                    Foreground = System.Windows.Media.Brushes.DarkViolet,
                                    Background = System.Windows.Media.Brushes.LightGreen
                                });
                            }
                            else
                            {
                                p.Inlines.Add(new Run
                                {
                                    Text = word + " "
                                });
                            }
                        }
                    }

                    p.Inlines.Add(new Run
                    {
                        Text = Environment.NewLine
                    });
                }
            }

            document.Blocks.Clear();
            document.PageWidth = 2500;


            document.Blocks.Add(p);


        }
    }

}
