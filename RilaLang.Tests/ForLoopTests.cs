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
    public class ForLoopTests
    {
        [Fact]
        public void ForLoopWithRange()
        {
            var code = @"
a = 0

for i in 0..10
    a = i

a
";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result == 10);
        }
    }
}
