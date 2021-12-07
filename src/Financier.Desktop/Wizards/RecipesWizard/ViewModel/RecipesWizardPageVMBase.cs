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
            }
        }

        public double TotalAmount
        {
            get => totalAmount;
            protected set
            {
                totalAmount = value;
                this.RaisePropertyChanged(nameof(this.TotalAmount));
                this.RaisePropertyChanged(nameof(this.Diff));
            }
        }

        public double Diff => TotalAmount - CalculatedAmount;
    }
}
