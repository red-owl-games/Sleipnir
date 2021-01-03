namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control", IsFlowRoot = true)]
    public class LateUpdateNode : Node
    {
        [FlowOut] public FlowPort LateUpdate;
    }
}