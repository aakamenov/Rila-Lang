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
    public class RuntimeTests
    {
        [Fact]
        public void ProgramAlwaysReturnsAValue()
        {
            var code = @"
a = 10

if a > 10
    a = 5
";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result is null);
        }

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
        public void Booleans()
        {
            var code = "not(not true and false)";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result == true);
        }

        [Fact]
        public void TypeOfReturnsCorrectType()
        {
            var code = @"
use System.Text

typeof(StringBuilder)
";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result.Name == "StringBuilder");
        }

        [Fact]
        public void TypeOfWorksWithAlias()
        {
            var code = @"
use System.Text as text

typeof(text.StringBuilder)
";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result.Name == "StringBuilder");
        }

        [Fact]
        public void GetIndex()
        {
            var code = @"
use System

a = Array.CreateInstance(typeof(Int32), 10)
a[5]
";
 
            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result == 0);
        }

        [Fact]
        public void SetIndex()
        {
            var code = @"
use System

a = Array.CreateInstance(typeof(Int32), 10)
a[4] = 50
a[4]
";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.True(result == 50);
        }

        [Fact]
        public void DotExpressionSetMember()
        {
            var code = @"
use System.Net

o = new Cookie()
o.Comment = ""test""
o.Comment
";

            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);
            
            Assert.True(result == "test");
        }

        [Fact]
        public void ImportingTheSameNamespaceTwiceThrows()
        {
            var code = @"
use System
use System
";
            var engine = Rila.CreateRilaEngine();
            Action action = () => engine.Execute(code);

            Assert.Throws<RilaRuntimeException>(action);
        }

        [Fact]
        public void DuplicateAliasThrows()
        {
            var code = @"
use System as sys
use System.Text as sys
";
            var engine = Rila.CreateRilaEngine();
            Action action = () => engine.Execute(code);

            Assert.Throws<RilaRuntimeException>(action);
        }

        [Fact]
        public void NewExpressionWithoutArguments()
        {
            var code = @"
use System.Text

a = new StringBuilder()
a.Append(""test"")
a.ToString()
";
            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);
            
            Assert.True(result == "test");
        }

        [Fact]
        public void NewExpressionWithArguments()
        {
            var code = @"
use System.Text

a = new StringBuilder(5, 10)
a
";
            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);
            
            Assert.IsType<StringBuilder>(result);
        }

        [Fact]
        public void NewExpressionWithAlias()
        {
            var code = @"
use System.Text as text

a = new text.StringBuilder(5, 10)
a
";
            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.IsType<StringBuilder>(result);
        }
/*
        [Fact]
        public void NewExpressionWithGenericArguments()
        {
            var code = @"
use System.Collections.Generic

a = new List`1()
a
";
            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);
            
            Assert.IsType<List<int>>(result);
        }
*/
        [Fact]
        public void StaticCall()
        {
            var code = @"
use System

a = Guid.NewGuid()
a
";
            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.IsType<Guid>(result);
        }

        [Fact]
        public void StaticCallWithAlias()
        {
            var code = @"
use System as sys

a = sys.Guid.NewGuid()
a
";
            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.IsType<Guid>(result);
        }

        [Fact]
        public void StaticMemberAccess()
        {
            var code = @"
use System

a = String.Empty
a
";
            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.IsType<string>(result);
        }

        [Fact]
        public void StaticMemberAccessWithAlias()
        {
            var code = @"
use System as sys

a = sys.String.Empty
a
";
            var engine = Rila.CreateRilaEngine();
            var result = engine.Execute(code);

            Assert.IsType<string>(result);
        }
    }
}
