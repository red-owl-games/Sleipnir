using RedOwl.Sleipnir.Engine;
using UnityEngine.UIElements;

namespace RedOwl.Sleipnir.Editor
{
    [CustomNodeView(typeof(StartNode))]
    [CustomNodeView(typeof(UpdateNode))]
    [CustomNodeView(typeof(LateUpdateNode))]
    [CustomNodeView(typeof(FixedUpdateNode))]
    public class FlowRootNodeView : SleipnirNodeViewBase
    {
        private Button button;
        
        protected override void OnInitialize()
        {
            base.OnInitialize();
            button = new Button(OnClickOpenGraph) {text = "Execute"};
            titleButtonContainer.Add(button);
        }

        private void OnClickOpenGraph()
        {
            switch (Node)
            {
                case StartNode startNode:
                    new Flow(Node.Graph, startNode).Execute();
                    break;
                case UpdateNode updateNode:
                    new Flow(Node.Graph, updateNode).Execute();
                    break;
                case LateUpdateNode lateUpdateNode:
                    new Flow(Node.Graph, lateUpdateNode).Execute();
                    break;
                case FixedUpdateNode fixedUpdateNode:
                    new Flow(Node.Graph, fixedUpdateNode).Execute();
                    break;
            }
        }
    }
}