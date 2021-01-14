namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control")]
    public class ExitNode : Node
    {
        [FlowIn(GraphPort = true)] public FlowPort Exit;
    }
}