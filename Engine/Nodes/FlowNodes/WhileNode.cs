using System.Collections;

namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control")]
    public class WhileNode : Node
    {
        [FlowIn(nameof(OnEnter))] public FlowPort Enter;
        [FlowOut] public FlowPort True;
        [FlowOut] public FlowPort Exit;
        
        [ValueIn] public ValuePort<bool> Condition = true;
        
        private IEnumerator OnEnter(IFlow flow)
        {
            while (Condition.Value)
            {
                yield return True;
            }

            yield return Exit;
        }
    }
}