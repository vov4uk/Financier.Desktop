namespace Financier.Tests.Common
{
    using System.Linq;
    using AutoFixture;
    using AutoFixture.AutoMoq;

    public class DataFixture : Fixture
    {
        public DataFixture()
        {
            Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => Behaviors.Remove(b));
            Behaviors.Add(new OmitOnRecursionBehavior(1));
            Customize(new AutoMoqCustomization());
        }
    }
}
