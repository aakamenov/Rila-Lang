using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler
{
    using DLR = System.Linq.Expressions;

    public class GenScope
    {
        public bool IsLoop { get; private set; }
        public bool IsLambda { get; private set; }

        public GenScope Parent { get; private set; }
        public DLR.LabelTarget BreakTarget { get; private set; }
        public DLR.LabelTarget ContinueTarget { get; private set; }

        public Dictionary<string, DLR.ParameterExpression> Definitions { get; }

        private GenScope(GenScope parent)
        {
            Parent = parent;
            Definitions = new Dictionary<string, DLR.ParameterExpression>();
        }

        public static GenScope CreateLoop(GenScope parent)
        {
            return new GenScope(parent)
            {
                IsLoop = true,
                BreakTarget = DLR.Expression.Label(),
                ContinueTarget = DLR.Expression.Label()
            };
        }

        public static GenScope CreateLambda(GenScope parent)
        {
            return new GenScope(parent)
            {
                IsLambda = true
            };
        }

        public static GenScope CreateRoot()
        {
            return new GenScope(null);
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
    }
}
