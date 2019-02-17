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
        
        private List<WeakReference<Signal<T>>> dependencies;

        public Cell(T value)
        {
            val = value;
            dependencies = new List<WeakReference<Signal<T>>>();
        }

        public void AddDependency(Signal<T> signal)
        {
            foreach(var dependency in dependencies)
            {
                if (dependency.TryGetTarget(out Signal<T> target))
                {
                    if (target == signal)
                        return;
                }
            }

            dependencies.Add(new WeakReference<Signal<T>>(signal));
        }

        private void NotifyValueChanged()
        {
            foreach(var dependancy in dependencies)
            {
                if (dependancy.TryGetTarget(out Signal<T> target))
                    target.ReEvaluate();
            }
        }
    }
}
