using System;
using ClickTwice.Publisher.Core;
using Shouldly;
using Xunit;

namespace ClickTwice.Tests
{
    public class CoreExtensionsTests
    {
        [Theory]
        [InlineData("0.1.2.3", "0.1.2.3")]
        [InlineData("0.1.2", "0.1.2.%2a")]
        [InlineData("0.1", "0.1.%2a.%2a")]
        public void Should_Pad_Short_Versions(string input, string expanded)
        {
            var version = input.ToVersionString();
            version.ShouldBe(expanded);
        }
    }
}
