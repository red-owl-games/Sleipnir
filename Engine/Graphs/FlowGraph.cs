namespace RedOwl.Sleipnir.Engine
{
    [Graph("Common", "Flow")]
    [RequireNode(typeof(StartNode), 200, 100)]
    [RequireNode(typeof(UpdateNode), 400, 100)]
    public class FlowGraph : Graph
    {
        
    }
}