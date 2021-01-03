namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control", Deletable = false, Moveable = false, IsFlowRoot = true)]
    public class StartNode : Node
    {
        [FlowOut] public FlowPort Start;
    }
}