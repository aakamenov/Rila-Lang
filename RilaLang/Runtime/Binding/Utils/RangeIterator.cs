using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Runtime.Binding.Utils
{
    public class RangeIterator<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        private T start;
        private T end;

        public RangeIterator(T start, T end)
        {
            if (start.CompareTo(0) < 0 || end.CompareTo(0) < 0)
                throw new ArgumentException("Operator \"..\" doesn't accept negative values!");
            else if (end.CompareTo(start) < 0)
                throw new ArgumentException("Right-hand side argument is less than the left-hand side argument supplied to operator \"..\"");

            this.start = start;
            this.end = end;
        }

        public IEnumerator GetEnumerator() 
        {          
            switch(Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int32:
                    return new IntRange(Convert.ToInt32(start), Convert.ToInt32(end));
                default:
                    throw new ArgumentException($"{typeof(RangeIterator<>)} only works with numeric types!");
            }
        }
    }

    internal abstract class Range<T> : IEnumerator where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        public T Current => current;

        object IEnumerator.Current => current;

        protected readonly T start;
        protected readonly T end;
        protected T current;

        public Range(T start, T end)
        {
            this.end = end;
            this.start = start;
            
            current = start;
        }

        public abstract bool MoveNext();

        public abstract void Reset();
    }

    internal class IntRange : Range<int>
    {
        public IntRange(int start, int end) : base(start, end) { }

        public override bool MoveNext()
        {
            if (current == end)
                return false;

            current++;

            return true;
        }

        public override void Reset()
        {
            current = start;
        }
    }
}
