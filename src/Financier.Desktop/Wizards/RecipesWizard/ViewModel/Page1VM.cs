using Financier.Desktop.Wizards.RecipesWizard.View;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Xceed.Wpf.Toolkit;

namespace Financier.Desktop.Wizards.RecipesWizard.ViewModel
{
    public class Page1VM : RecipesWizardPageVMBase
    {
        private DelegateCommand<RichTextBox> _highlightCommand;
        private string text;
        public Page1VM(double totalAmount)
        {
            TotalAmount = totalAmount;
        }

        public List<FinancierTransactionDto> Amounts { get; } = new List<FinancierTransactionDto>();
        public DelegateCommand<RichTextBox> HighlightCommand
        {
            get
            {
                return _highlightCommand ??= new DelegateCommand<RichTextBox>(HighLight, (_) => true);
            }
        }

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
                var lines = text.Split(Environment.NewLine);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var res = Regex.Match(line, RecipesFormatter.Pattern, RegexOptions.IgnoreCase);
                        if (res.Success)
                        {
                            var number = res.Value.Substring(0, res.Value.Length - 2);
                            var amount = GetDouble(number.Replace(",", ".").Trim());
                            tmp += amount;
                            if (amount != 0.0)
                            {
                                var note = line.Replace(res.Value, string.Empty);
                                Amounts.Add(new FinancierTransactionDto
                                {
                                    FromAmount = Convert.ToInt64(amount * -100.0),
                                    Note = string.IsNullOrWhiteSpace(note) ? string.Empty : note.TrimEnd(),
                                    Order = order++
                                });
                            }
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

            // Try parsing in the current culture
            if (!double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                // Then try in US english
                !double.TryParse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                // Then in neutral language
                !double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        private void HighLight(RichTextBox textBox)
        {
            textBox.BeginInit();
            textBox.EndInit();
            CalculateCurrentAmount();
        }
    }
}
