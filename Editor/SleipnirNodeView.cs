using RedOwl.Sleipnir.Engine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using NodeView = UnityEditor.Experimental.GraphView.Node;
using PortView = UnityEditor.Experimental.GraphView.Port;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace RedOwl.Sleipnir.Editor
{
    public class SleipnirNodeView : NodeView
    {
        public VisualElement FlowInPortContainer { get; private set; }
        public VisualElement FlowOutPortContainer { get; private set; }
        public INode Model => (INode) userData;

        public SleipnirNodeView(INode node)
        {
            userData = node;
            Initialize(node);
        }

        private void Initialize(INode node)
        {
            name = node.NodeId;
            title = node.NodeTitle;
            SetPosition(node.NodeRect);
            // TODO: map capabilities AND if we use Resizeable we'll need to update the data's Rect with the changes.
            //capabilities -= Capabilities.Collapsible;
            //capabilities = Capabilities.Movable;
            //capabilities = Capabilities.Resizable;
            //this.IsResizable();
            //style.width = Model.NodeRect.width;
            //style.height = Model.NodeRect.height;

            CreateBody(node);
            CreateFlowPortContainers();
            if (node is IFlowNode flowNode) CreateFlowPorts(flowNode);
            AttachFlowPortContainers();
            CreateValuePorts(node);
            RefreshExpandedState();
            RefreshPorts();
        }
        
        private void CreateBody(INode node)
        {
#if ODIN_INSPECTOR
            var tree = Sirenix.OdinInspector.Editor.PropertyTree.Create(node);
            bool useUndo = node is Object;
            extensionContainer.Add(new IMGUIContainer(() => tree.Draw(useUndo)) { name = "OdinTree"});
#else
            // TODO: Draw Property Field
#endif
        }
        
        private PortView CreatePortView(IPort valuePort, Orientation orientation)
        {
            var view = PortView.Create<Edge>(orientation, ConvertDirection(valuePort.Direction), ConvertCapacity(valuePort.Capacity), valuePort.ValueType);
            view.name = valuePort.Id.Port;
            view.userData = valuePort;
            view.portName = valuePort.Name;
            return view;
        }
        
        private void CreateValuePorts(INode node)
        {
            foreach (var valuePort in node.ValueInPorts.Values)
            {
                inputContainer.Add(CreatePortView(valuePort, Orientation.Horizontal));
            }
            
            foreach (var valuePort in node.ValueOutPorts.Values)
            {
                outputContainer.Add(CreatePortView(valuePort, Orientation.Horizontal));
            }
        }

        private void CreateFlowPortContainers()
        {
            FlowInPortContainer = new VisualElement {name = "FlowPorts"};
            FlowInPortContainer.AddToClassList("FlowInPorts");
            FlowOutPortContainer = new VisualElement {name = "FlowPorts"};
            FlowOutPortContainer.AddToClassList("FlowOutPorts");
        }

        private void CreateFlowPorts(IFlowNode node)
        {
            foreach (var flowPort in node.FlowInPorts.Values)
            {
                FlowInPortContainer.Add(CreatePortView(flowPort, Orientation.Vertical));
            }
            
            foreach (var flowPort in node.FlowOutPorts.Values)
            {
                FlowOutPortContainer.Add(CreatePortView(flowPort, Orientation.Vertical));
            }
        }

        private void AttachFlowPortContainers()
        {
            if (FlowInPortContainer.childCount > 0) mainContainer.parent.Insert(0, FlowInPortContainer);
            if (FlowOutPortContainer.childCount > 0) mainContainer.parent.Add(FlowOutPortContainer);
        }

        private Direction ConvertDirection(PortDirection value) => value == PortDirection.Input ? Direction.Input : Direction.Output;
        private PortView.Capacity ConvertCapacity(PortCapacity value) => value == PortCapacity.Single ? PortView.Capacity.Single : PortView.Capacity.Multi;
    }
    
    public static class NodeViewExtensions
    {
        //public static INode INode(this NodeView self) => (INode) self.userData;
        // public static string Id(this NodeView self) => self.INode().NodeId;
        // public static uint Id(this PortView self) => (uint)self.userData;
    }
}