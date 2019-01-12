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
        public void Indexing()
        {
            var code = @"
a = 1..10
a[5]

";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result == 6);
        }
    }
}
