using RedOwl.Sleipnir.Engine;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedOwl.Sleipnir.Editor
{
    [CustomNodeView(typeof(Graph))]
    [CustomNodeView(typeof(GraphReferenceNode))]
    public class GraphReferenceNodeView : SleipnirNodeViewBase
    {
        private Button OpenGraphButton;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            OpenGraphButton = new Button(OnClickOpenGraph) {text = "Open"};
            titleButtonContainer.Add(OpenGraphButton);
        }

        private void OnClickOpenGraph()
        {
            switch (Node)
            {
                case GraphReferenceNode graphReference:
                    SleipnirEditor.Open(graphReference.Asset, true);
                    break;
                case Graph graph:
                    var asset = ScriptableObject.CreateInstance<GraphAsset>();
                    asset.name = graph.Title;
                    asset.Graph = graph;
                    SleipnirEditor.Open(asset, true);
                    break;
            }
            
        }
    }
}