using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Runtime.Reactive
{
    public class Signal<T>
    {
        public T Value { get; private set; }

        private Func<T> expression;

        /// <summary>
        /// Creates a new instance of the signal class
        /// </summary>
        /// <param name="expression">An expression which features Cell instances that will cause recomputation.</param>
        /// <param name="cells">A list of the Cell instances used in the provided expression.</param>
        public Signal(Func<T> expression, IList<Cell<T>> cells)
        {
            this.expression = expression;
            ReEvaluate();

            foreach (var cell in cells)
                cell.AddDependency(this);
        }

        public void ReEvaluate()
        {
            Value = expression();
        }
    }
}
