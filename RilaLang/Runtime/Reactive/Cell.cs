using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RilaLang.Runtime.Reactive
{
    public class Cell<T>
    {
        public T Value
        {
            get => val;
            set
            {
                val = value;
                NotifyValueChanged();
            }
        }

        private T val;
        
        private List<WeakReference<Signal<T>>> dependancies;

        public Cell(T value)
        {
            val = value;
            dependancies = new List<WeakReference<Signal<T>>>();
        }

        public void AddDependancy(Signal<T> signal)
        {
            foreach(var dependancy in dependancies)
            {
                if (dependancy.TryGetTarget(out Signal<T> target))
                {
                    if (target == signal)
                        return;
                }
            }

            dependancies.Add(new WeakReference<Signal<T>>(signal));
        }

        private void NotifyValueChanged()
        {
            foreach(var dependancy in dependancies)
            {
                if (dependancy.TryGetTarget(out Signal<T> target))
                    target.ReEvaluate();
            }
        }
    }
}
