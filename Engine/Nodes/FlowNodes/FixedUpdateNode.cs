namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control", IsFlowRoot = true)]
    public class FixedUpdateNode : Node
    {
        [FlowOut] public FlowPort FixedUpdate;
    }
}