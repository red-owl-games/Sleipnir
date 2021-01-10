using System.Collections;

namespace RedOwl.Sleipnir.Engine
{
    [Node("Math", Path = "Math")]
    public abstract class MathNode<TInput, TOutput> : Node
    {
        [FlowIn(nameof(OnEnter))] public FlowPort Enter;
        [FlowOut] public FlowPort Exit;
        
        [ValueIn] public ValuePort<TInput> Input;

        [ValueOut] public ValuePort<TOutput> Output;

        protected void OnEnter(IFlow flow)
        {
            Calculate(flow);
        }
        
        protected abstract void Calculate(IFlow flow);
    }
    
    [Node("Math", Path = "Math")]
    public abstract class MathNode<TLeft, TRight, TOutput> : Node
    {
        [FlowIn(nameof(OnEnter))] public FlowPort Enter;
        [FlowOut] public FlowPort Exit;
        
        [ValueIn] public ValuePort<TLeft> Left;
        [ValueIn] public ValuePort<TRight> Right;

        [ValueOut] public ValuePort<TOutput> Output;

        protected IFlowPort OnEnter(IFlow flow)
        {
            Calculate(flow);
            return Exit;
        }
        
        protected abstract void Calculate(IFlow flow);
    }
}