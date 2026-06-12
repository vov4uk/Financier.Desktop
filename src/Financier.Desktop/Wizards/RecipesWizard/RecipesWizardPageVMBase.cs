namespace Financier.Desktop.Wizards.RecipesWizard.ViewModel
{
    public abstract class RecipesWizardPageVMBase : WizardPageBaseVM
    {
        private double calculatedAmount;
        private double totalAmount;
        public double CalculatedAmount
        {
            get => calculatedAmount;
            protected set
            {
                calculatedAmount = value;
                this.RaisePropertyChanged(nameof(this.CalculatedAmount));
                this.RaisePropertyChanged(nameof(this.Diff));
                this.RaisePropertyChanged(nameof(this.FormattedTotal));
            }
        }

        public double Diff => TotalAmount - CalculatedAmount;

        public string FormattedTotal =>
            string.Format(
                LocalizationManager?.reciept_wizard_total_format ?? " Total : {0:F2} Current : {1:F2} Diff : {2:F2}",
                TotalAmount, CalculatedAmount, Diff);

        public double TotalAmount
        {
            get => totalAmount;
            protected set
            {
                totalAmount = value;
                this.RaisePropertyChanged(nameof(this.TotalAmount));
                this.RaisePropertyChanged(nameof(this.Diff));
                this.RaisePropertyChanged(nameof(this.FormattedTotal));
            }
        }
    }
}
