namespace Financier.Adapter.Tests.Common
{
    using System;
    using AutoFixture;
    using AutoFixture.Xunit2;

    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(CreateFixture)
        {
        }

        protected AutoMoqDataAttribute(Func<IFixture> fixtureFactory)
            : base(fixtureFactory)
        {
        }

        public static IFixture CreateFixture() => new DataFixture();
    }
}
