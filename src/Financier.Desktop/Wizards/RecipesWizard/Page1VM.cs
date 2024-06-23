using Financier.Common.Entities;
using Financier.Desktop.Helpers;
using Financier.Desktop.Wizards.RecipesWizard.View;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Xceed.Wpf.Toolkit;

namespace Financier.Desktop.Wizards.RecipesWizard.ViewModel
{
    public class Page1VM : RecipesWizardPageVMBase
    {
        private readonly string pattern = RecipesFormatter.Pattern + @"(\t|\n|\r|$)"; //TODO fix '100600 Балтика' case then it calculates (100600 Б), no more symbols after a|b
        private readonly char[] charactersToRemove = { ':', '/', '?', '#', '[', ']', '@', '*', '.', ',', '\"', '&', '\'' };
        private DelegateCommand<RichTextBox> _highlightCommand;
        private string text;
        public Page1VM(double totalAmount)
        {
            TotalAmount = totalAmount;
        }

        public List<FinancierTransactionDto> Amounts { get; } = new List<FinancierTransactionDto>();

        public DelegateCommand<RichTextBox> HighlightCommand => _highlightCommand ??= new DelegateCommand<RichTextBox>(HighLight);

        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                this.RaisePropertyChanged(nameof(this.Text));
            }
        }

        public override string Title => "Paste text";

        public void CalculateCurrentAmount()
        {
            Amounts.Clear();

            if (!string.IsNullOrEmpty(text))
            {
                double tmp = 0.0;
                int order = 1;
                var lines = text.Split(Environment.NewLine).Where(line => !string.IsNullOrWhiteSpace(line));
                foreach (var line in lines)
                {
                    var findNumber = Regex.Match(line, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));
                    if (findNumber.Success)
                    {
                        var amount = GetDouble(findNumber.Value.Substring(0, findNumber.Value.Length - 2).Replace(",", ".").Trim());
                        tmp += amount;

                        if (amount != 0.0)
                        {
                            var note = line.Replace(findNumber.Value, string.Empty);
                            var lineResult = ParseLine(note);

                            Amounts.Add(new FinancierTransactionDto
                            {
                                FromAmount = Convert.ToInt64(amount * -100.0),
                                Note = lineResult.note,
                                CategoryId = lineResult.categoryId,
                                Order = order++
                            });
                        }
                    }
                }

                CalculatedAmount = Math.Abs(tmp) * -1.0;
            }
        }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Text);

        private static double GetDouble(string value, double defaultValue = 0.0)
        {
            double result;
            if (!double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                !double.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                !double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        private void HighLight(RichTextBox textBox)
        {
            Text = RecipiesHelper.FormatText(Text);
            textBox.BeginInit();
            textBox.EndInit();
            CalculateCurrentAmount();
        }

        private static bool ContainsString(string title, string[] description)
        {
            if (!string.IsNullOrEmpty(title) && description != null)
            {
                return description.Any(x => title.Contains(x, StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }

        private static bool TryParseCategory(string[] desc, out int categoryId)
        {
            var category = DbManual.Category
                    .Where(x => x.Id > 0)
                    .FirstOrDefault(l => ContainsString(l.Title, desc));
            if (category != null)
            {
                categoryId = category.Id.Value;
                return true;
            }
            categoryId = 0;
            return false;
        }

        private (string note, int categoryId) ParseLine(string note)
        {
            int categoryId = 0;
            if (!string.IsNullOrWhiteSpace(note))
            {
                foreach (char c in charactersToRemove)
                {
                    note = note.Replace(c.ToString(), string.Empty);
                }

                var arr = note.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Where(x => x.Length > 2)
                              .Select(x => x.Trim('-').Trim().ToLowerInvariant())
                              .ToArray();

                TryParseCategory(arr, out categoryId);

                note = string.Join(" ", arr);
            }
            note = string.IsNullOrWhiteSpace(note) ? string.Empty : note.TrimEnd();
            return (note, categoryId);
        }
    }
}
