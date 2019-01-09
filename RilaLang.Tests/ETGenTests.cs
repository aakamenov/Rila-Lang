using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Xunit;
using Xunit.Sdk;
using Microsoft.Scripting;
using Microsoft.CSharp.RuntimeBinder;
using RilaLang.Runtime;

namespace RilaLang.Tests
{
    public class ETGenTests
    {
        [Fact]
        public void ExecuteBasicOperations()
        {
            var code = File.ReadAllText("TestPrograms/ETGen/basic.rila");

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result == 100);
        }

        [Fact]
        public void ReferenceVariableAfterScopeShouldThrow()
        {
            var code = File.ReadAllText("TestPrograms/ETGen/reference_destroyed_variable.rila");

            var engine = Rila.CreateRilaEngine();

            Func<dynamic> wrapper = () => engine.Execute(code);
            Assert.Throws<SyntaxErrorException>(wrapper);
        }

        [Fact]
        public void BasicFunction()
        {
            var code = File.ReadAllText("TestPrograms/ETGen/basic_function.rila");

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result == 10);
        }

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
    }
}
