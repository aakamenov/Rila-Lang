using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xunit;
using Xunit.Sdk;
using RilaLang.Compiler;

namespace RilaLang.Tests
{
    public class ETGenTests
    {
        [Fact]
        public void ExecuteBasicOperations()
        {
            var program = File.ReadAllText("TestPrograms/ETGen/basic.rila");
            var lexer = new Lexer(program);
            var module = new RilaParser(lexer).Parse();
            var et = module.GenerateExpressionTree();
        }
    }
}
