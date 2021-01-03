namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control")]
    public class BranchNode : Node
    {
        [FlowIn(nameof(OnEnter))] public FlowPort Enter;

        [FlowOut] public FlowPort True;
        [FlowOut] public FlowPort False;
        
        [ValueIn] public ValuePort<bool> Condition = false;
        
        private IFlowPort OnEnter(IFlow flow)
        {
            return Condition.Value ? True : False;
        }
    }
}
