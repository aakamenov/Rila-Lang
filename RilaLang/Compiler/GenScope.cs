﻿using System;
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

        public GenScopeRoot Root { get; protected set; }
        public GenScope Parent { get; private set; }
        public DLR.LabelTarget BreakTarget { get; protected set; }
        public DLR.LabelTarget ContinueTarget { get; protected set; }

        public Dictionary<string, DLR.ParameterExpression> Definitions { get; }

        private GenScope(Rila runtime, GenScope parent) : this(runtime)
        {
            Parent = parent;
            Root = parent.Root;
        }

        protected GenScope(Rila runtime)
        {
            Runtime = runtime;
            Definitions = new Dictionary<string, DLR.ParameterExpression>();
            BreakTarget = DLR.Expression.Label("@break");
            ContinueTarget = DLR.Expression.Label("@continue");
        }

        public GenScope CreateLoop()
        {
            return new GenScope(Runtime, this)
            {
                IsLoop = true,
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

        public IEnumerable<DLR.ParameterExpression> GetAllVariables()
        {
            return Definitions.Select(x => x.Value);
        }

        public bool TryGetVariable(string identifier, out DLR.ParameterExpression variable)
        {
            if (Definitions.TryGetValue(identifier, out variable))
                return true;

            if (Parent != null)
                return Parent.TryGetVariable(identifier, out variable);

            return false;
        }
    }

    public class GenScopeRoot : GenScope
    {
        public Dictionary<string, DLR.LambdaExpression> FunctionDefinitions { get; }

        public GenScopeRoot(Rila runtime) : base(runtime)
        {
            FunctionDefinitions = new Dictionary<string, DLR.LambdaExpression>();
            Root = this;
        }
    }
}
