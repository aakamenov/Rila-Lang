using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using RilaLang.Runtime.Binding.Utils;

namespace RilaLang.Runtime
{
    public class TypeProvider
    {
        private Dictionary<string, Type[]> namespaces;
        private HashSet<string> aliases;
        private IList<Assembly> assemblies;

        public TypeProvider(IList<Assembly> assemblies)
        {
            namespaces = new Dictionary<string, Type[]>();
            aliases = new HashSet<string>();
            this.assemblies = assemblies;
        }

        public void LoadNamespace(string @namespace, string alias)
        {
            var match = false;

            foreach(var asm in assemblies)
            {
                var matchingTypes = asm.ExportedTypes.Where(x => x.Namespace == @namespace).ToArray();

                if (!matchingTypes.Any())
                    continue;
                
                var name = string.IsNullOrEmpty(alias) ? @namespace : alias;

                try
                {
                    namespaces.Add(name, matchingTypes);
                    aliases.Add(alias);
                }
                catch(ArgumentException)
                {
                    throw new ArgumentException($"Namespace or alias \"{@namespace}\" has already been imported!");
                }

                match = true;
                break;
            }

            if (!match)
                throw new ArgumentException($"Could not find namespace \"{@namespace}\"");
        }

        public bool IsAlias(string alias)
        {
            return aliases.Contains(alias);
        }

        public bool TryGetType(UnresolvedType unresolved, out Type type)
        {
            var names = unresolved.Name.Split('.');
            var @namespace = string.Concat(names.Take(names.Length - 1));

            if (!string.IsNullOrEmpty(@namespace))
            {
                if (namespaces.TryGetValue(@namespace, out Type[] types)) //alias
                {
                    foreach (var t in types)
                    {
                        if (t.Name == names.Last())
                        {
                            type = t;
                            return true;
                        }
                    }
                }
            }
            else
            {
                foreach (var n in namespaces)
                {
                    foreach (var t in n.Value)
                    {
                        if (t.Name == unresolved.Name)
                        {
                            type = t;
                            return true;
                        }
                    }
                }
            }

            type = default(Type);

            return false;
        }
    }
}
