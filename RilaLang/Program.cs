using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using RilaLang.Runtime;

[assembly: InternalsVisibleTo("RilaLang.Tests")]
namespace RilaLang
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("No file provided. Exiting...");
                return;
            }

            try
            {
                var engine = Rila.CreateRilaEngine();
                engine.ExecuteFile(args[0]);
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
}
