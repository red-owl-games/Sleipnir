using System.Collections;

namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control")]
    public class LoopNode : Node
    {
        [FlowIn(nameof(OnEnter))] public FlowPort Enter;
        [FlowOut] public FlowPort Exit;

        [ValueIn] public ValuePort<int> Count;

        private IEnumerator OnEnter(IFlow flow)
        {
            int times = 0;
            while (times < Count.Value)
            {
                yield return Exit;
                times += 1;
            }
        }
    }
}