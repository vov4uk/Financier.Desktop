namespace Financier.Common.Tests
{
    using System.Linq;
    using AutoFixture;
    using AutoFixture.AutoMoq;

    public class DataFixture : Fixture
    {
        public DataFixture()
        {
            this.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => this.Behaviors.Remove(b));
            this.Behaviors.Add(new OmitOnRecursionBehavior(1));
            this.Customize(new AutoMoqCustomization());
        }
    }
}
