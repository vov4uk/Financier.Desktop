using Financier.Desktop.MonoWizard.ViewModel;
using Financier.Desktop.Wizards.MonoWizard.ViewModel;
using Financier.Desktop.Wizards.RecipesWizard.View;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Xceed.Wpf.Toolkit;

namespace Financier.Desktop.Wizards.RecipesWizard.ViewModel
{
    public class Page1VM : WizardPageBaseVM
    {
        public override string Title => "Paste text";
        private string text;
        private DelegateCommand<RichTextBox> _highlightCommand;
        private double calculatedAmount;
        private double totalAmount;

        public override bool IsValid() => true;
        public List<FinancierTransactionVM> Amounts { get; } = new List<FinancierTransactionVM>();
        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                this.RaisePropertyChanged(nameof(this.Text));
            }
        }

        public double TotalAmount
        {
            get => totalAmount;
            set
            {
                totalAmount = value;
                this.RaisePropertyChanged(nameof(this.TotalAmount));
                this.RaisePropertyChanged(nameof(this.Diff));
            }
        }

        public double CalculatedAmount
        {
            get => calculatedAmount;
            set
            {
                calculatedAmount = value;
                this.RaisePropertyChanged(nameof(this.CalculatedAmount));
                this.RaisePropertyChanged(nameof(this.Diff));
            }
        }

        public double Diff => TotalAmount - CalculatedAmount;

        public DelegateCommand<RichTextBox> HighlightCommand
        {
            get
            {
                return _highlightCommand ??= new DelegateCommand<RichTextBox>(HighLight, (_) => true);
            }
        }

        private void HighLight(RichTextBox textBox)
        {
            textBox.BeginInit();
            textBox.EndInit();
            CalculateCurrentAmount();
        }

        public void CalculateCurrentAmount()
        {
            double tmp = 0.0;
            Amounts.Clear();
            int order = 1;
            var lines = text.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var res = Regex.Match(line, MyFormatter.Pattern, RegexOptions.IgnoreCase);
                    if (res.Success)
                    {
                        var number = res.Value.Substring(0, res.Value.Length - 2);
                        var amount = GetDouble(number);
                        Console.WriteLine($"number {number} -> {amount}");
                        tmp += amount;
                        if (amount != 0.0)
                        {
                            var note = line.Replace(res.Value, string.Empty);
                            Amounts.Add(new FinancierTransactionVM
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

        public static double GetDouble(string value, double defaultValue = 0.0)
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
    }
}
