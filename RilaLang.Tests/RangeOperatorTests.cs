using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.CSharp.RuntimeBinder;
using RilaLang.Runtime;

namespace RilaLang.Tests
{
    public class RangeOperatorTests
    {
        [Fact]
        public void RangeOperationReturnsEnumerable()
        {
            var code = "10..21";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result is IEnumerable<int>);
            Assert.True(result.Length == 12);
        }

        [Fact]
        public void RangeOperationWorksWithNestedExpressions()
        {
            var code = "0..5+5";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result is IEnumerable<int>);
            Assert.True(result.Length == 11);
        }

        [Fact]
        public void RangeOperationWorksWithVariables()
        {
            var code = @"
a = 5 + 5
0..a
";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result is IEnumerable<int>);
            Assert.True(result.Length == 11);
        }

        [Fact]
        public void RangeOperationWithEqualParamsReturnsEmptyArray()
        {
            var code = "5..5";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result is IEnumerable<int>);
            Assert.True(result.Length == 0);
        }

        [Fact]
        public void RangeOperationWithBadParamsThrows()
        {
            var code = "5..4";

            var engine = Rila.CreateRilaEngine();

            Func<dynamic> wrapper = () => engine.Execute(code);
            Assert.Throws<RuntimeBinderException>(wrapper);
        }

        [Fact]
        public void RangeOperationWithNonNumericalTypesThrows()
        {
            var code = "5..true";

            var engine = Rila.CreateRilaEngine();

            Func<dynamic> wrapper = () => engine.Execute(code);
            Assert.Throws<RuntimeBinderException>(wrapper);
        }
    }
}
