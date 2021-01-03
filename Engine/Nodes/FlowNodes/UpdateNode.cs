namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control", IsFlowRoot = true)]
    public class UpdateNode : Node
    {
        [FlowOut] public FlowPort Update;
    }
}