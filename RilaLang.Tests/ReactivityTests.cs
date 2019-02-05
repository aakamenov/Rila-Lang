using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xunit;
using Xunit.Sdk;
using RilaLang.Runtime;
using RilaLang.Runtime.Reactive;

namespace RilaLang.Tests
{
    public class ReactivityTests
    {
        [Fact]
        public void ProofOfConcept()
        {
            var a = new Cell<int>(5);
            var b = new Cell<int>(5);

            var signal = new Signal<int>(() => a.Value + b.Value + 1, new Cell<int>[] { a, b });

            Assert.True(signal.Value == 11);

            a.Value = 2;
            Assert.True(signal.Value == 8);

            a.Value = 3;
            b.Value = 3;

            Assert.True(signal.Value == 7);
        }
    }
}
