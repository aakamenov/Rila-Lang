using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xunit;
using Xunit.Sdk;
using Microsoft.Scripting;
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
    }
}
