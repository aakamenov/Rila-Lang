using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.CSharp.RuntimeBinder;
using RilaLang.Runtime;
using RilaLang.Runtime.Binding.Utils;

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

            Assert.True(result is RangeIterator<int>);
            var enumerator = result.GetEnumerator();
            Assert.True(enumerator.Current == 0);

            while (enumerator.MoveNext()) { }

            Assert.True(enumerator.Current == 21);
        }

        [Fact]
        public void RangeOperationWorksWithNestedExpressions()
        {
            var code = "0..5+5";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result is RangeIterator<int>);

            var enumerator = result.GetEnumerator();
            Assert.True(enumerator.Current == 0);

            while(enumerator.MoveNext()) { }

            Assert.True(enumerator.Current == 10);
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

            Assert.True(result is RangeIterator<int>);

            var enumerator = result.GetEnumerator();
            Assert.True(enumerator.Current == 0);

            while (enumerator.MoveNext()) { }

            Assert.True(enumerator.Current == 10);
        }

        [Fact]
        public void RangeOperationWithEqualParamsCantMoveNext()
        {
            var code = "5..5";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result is RangeIterator<int>);

            var enumerator = result.GetEnumerator();

            enumerator.MoveNext();
            Assert.True(enumerator.Current == 5);
   
            Assert.True(enumerator.MoveNext() == false);
        }

        [Fact]
        public void RangeOperationWithBadParamsThrows()
        {
            var code = "5..4";

            var engine = Rila.CreateRilaEngine();

            Func<dynamic> wrapper = () => engine.Execute(code);
            Assert.Throws<ArgumentException>(wrapper);
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
