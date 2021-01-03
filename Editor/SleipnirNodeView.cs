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

        public SleipnirNodeView(INode node, IEdgeConnectorListener listener)
        {
            userData = node;
            Initialize(node, listener);
        }

        private void Initialize(INode node, IEdgeConnectorListener listener)
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
            if (node is IFlowNode flowNode) CreateFlowPorts(flowNode, listener);
            AttachFlowPortContainers();
            CreateValuePorts(node, listener);
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
        
        private PortView CreatePortView(IPort valuePort, Orientation orientation, IEdgeConnectorListener listener)
        {
            var view = new SleipnirPortView(orientation, valuePort.Direction, valuePort.Capacity, valuePort.ValueType, listener);
            view.name = valuePort.Id.Port;
            view.userData = valuePort;
            view.portName = valuePort.Name;
            return view;
        }
        
        private void CreateValuePorts(INode node, IEdgeConnectorListener listener)
        {
            foreach (var valuePort in node.ValueInPorts.Values)
            {
                inputContainer.Add(CreatePortView(valuePort, Orientation.Horizontal, listener));
            }
            
            foreach (var valuePort in node.ValueOutPorts.Values)
            {
                outputContainer.Add(CreatePortView(valuePort, Orientation.Horizontal, listener));
            }
        }

        private void CreateFlowPortContainers()
        {
            FlowInPortContainer = new VisualElement {name = "FlowPorts"};
            FlowInPortContainer.AddToClassList("FlowInPorts");
            FlowOutPortContainer = new VisualElement {name = "FlowPorts"};
            FlowOutPortContainer.AddToClassList("FlowOutPorts");
        }

        private void CreateFlowPorts(IFlowNode node, IEdgeConnectorListener listener)
        {
            foreach (var flowPort in node.FlowInPorts.Values)
            {
                FlowInPortContainer.Add(CreatePortView(flowPort, Orientation.Vertical, listener));
            }
            
            foreach (var flowPort in node.FlowOutPorts.Values)
            {
                FlowOutPortContainer.Add(CreatePortView(flowPort, Orientation.Vertical, listener));
            }
        }

        private void AttachFlowPortContainers()
        {
            if (FlowInPortContainer.childCount > 0) mainContainer.parent.Insert(0, FlowInPortContainer);
            if (FlowOutPortContainer.childCount > 0) mainContainer.parent.Add(FlowOutPortContainer);
        }
    }
    
    public static class NodeViewExtensions
    {
        //public static INode INode(this NodeView self) => (INode) self.userData;
        // public static string Id(this NodeView self) => self.INode().NodeId;
        // public static uint Id(this PortView self) => (uint)self.userData;
    }
}