namespace RilaLang.Runtime.Binding.Utils
{
    public class UnresolvedType
    {
        public string Name { get; }

        internal UnresolvedType(string name)
        {
            Name = name;
        }
    }
}
