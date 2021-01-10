namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control", IsFlowRoot = true)]
    public class ExitNode : Node
    {
        [FlowIn(GraphPort = true)] public FlowPort Exit;
    }
}