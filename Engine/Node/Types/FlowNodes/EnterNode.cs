namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control", IsFlowRoot = true)]
    public class EnterNode : Node
    {
        [FlowOut(GraphPort = true)] public FlowPort Enter;
    }
}