namespace RedOwl.Sleipnir.Engine
{
    [Node("Common", Path = "Common")]
    public class GraphReferenceNode : Node
    {
        public GraphAsset Graph;
        
        protected override void OnDefinition()
        {
            base.OnDefinition();
            // TODO: Generate Dynamic Ports based on graph's PortNodes - maybe use a function on Graph that does the same for itself
        }
    }
}