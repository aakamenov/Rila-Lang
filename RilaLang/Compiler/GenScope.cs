using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Runtime;

namespace RilaLang.Compiler
{
    using DLR = System.Linq.Expressions;

    public class GenScope
    {
        public Rila Runtime { get; }
        
        public bool IsLoop { get; private set; }
        public bool IsLambda { get; private set; }

        public GenScope Parent { get; private set; }
        public DLR.LabelTarget BreakTarget { get; private set; }
        public DLR.LabelTarget ContinueTarget { get; private set; }

        public Dictionary<string, DLR.ParameterExpression> Definitions { get; }

        private GenScope(Rila runtime, GenScope parent) : this(runtime)
        {
            Parent = parent;
        }

        private GenScope(Rila runtime)
        {
            Runtime = runtime;
            Definitions = new Dictionary<string, DLR.ParameterExpression>();
        }

        public static GenScope CreateRoot(Rila runtime)
        {
            return new GenScope(runtime)
            {
                BreakTarget = DLR.Expression.Label(),
                ContinueTarget = DLR.Expression.Label()
            };
        }

        public GenScope CreateLoop()
        {
            return new GenScope(Runtime, this)
            {
                IsLoop = true,
                BreakTarget = DLR.Expression.Label(),
                ContinueTarget = DLR.Expression.Label()
            };
        }

        public GenScope CreateLambda()
        {
            return new GenScope(Runtime, this)
            {
                IsLambda = true
            };
        }

        public GenScope CreateChild()
        {
            return new GenScope(Runtime, this);
        }

        public bool IsInLoop()
        {
            if (IsLoop)
                return true;

            if (Parent is null)
                return false;

            return Parent.IsInLoop();
        }

        public GenScope GetFirstLoopScope()
        {
            if (IsLoop)
                return this;

            if (Parent is null)
                return null;

            return Parent.GetFirstLoopScope();
        }

        public bool IsInLambda()
        {
            if (IsLambda)
                return true;

            if (Parent is null)
                return false;

            return Parent.IsInLambda();
        }

        public bool TryGetVariable(string identifier, out DLR.ParameterExpression variable)
        {
            if (Definitions.TryGetValue(identifier, out DLR.ParameterExpression result))
            {
                variable = result;
                return true;
            }

            if (Parent is null)
            {
                variable = null;
                return false;
            }

            return Parent.TryGetVariable(identifier, out variable);
        }
    }
}
