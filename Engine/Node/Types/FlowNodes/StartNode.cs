namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control", IsFlowRoot = true)]
    public class StartNode : Node
    {
        [FlowOut(GraphPort = true)] public FlowPort Start;
    }
}