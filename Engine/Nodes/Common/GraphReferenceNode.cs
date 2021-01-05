namespace RedOwl.Sleipnir.Engine
{
    [Node("Common", Path = "Common", Tooltip = "Nest a Graph that comes from a Graph Asset")]
    public class GraphReferenceNode : Node
    {
        public GraphAsset Asset;
        
        protected override void OnDefinition()
        {
            base.OnDefinition();
            // TODO: Generate Dynamic Ports based on graph's PortNodes - maybe use a function on Graph that does the same for itself
        }
    }
}