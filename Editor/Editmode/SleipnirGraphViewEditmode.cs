using System;
using System.Collections.Generic;
using RedOwl.Sleipnir.Engine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using NodeView = UnityEditor.Experimental.GraphView.Node;
using PortView = UnityEditor.Experimental.GraphView.Port;

namespace RedOwl.Sleipnir.Editor
{
    public class SleipnirGraphViewEditmode : SleipnirGraphViewBase<SleipnirNodeViewEditmode>, IGraphView
    {
        public SleipnirGraphViewEditmode()
        {
            RegisterCallback<GeometryChangedEvent>(GeometryChangedCallback);
            RegisterCallback<KeyUpEvent>(OnKeyUp);
            
            graphViewChanged = OnGraphViewChanged;
            viewTransformChanged = OnViewTransformChanged;
            
            SetupZoom(ContentZoomer.DefaultMinScale * 0.5f, ContentZoomer.DefaultMaxScale);
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            CreateGridBackground();
            CreateMiniMap();
        }
        
        public override List<PortView> GetCompatiblePorts(PortView startPort, NodeAdapter nodeAdapter)
        {
            var compatible = new List<PortView>();
            ports.ForEach(p =>
            {
                if (startPort == p) return;
                if (startPort.node == p.node) return;
                if (startPort.direction == p.direction) return;
                if (p.direction == Direction.Input && !startPort.portType.IsCastableTo(p.portType, true)) return;
                if (p.direction == Direction.Output && !p.portType.IsCastableTo(startPort.portType, true)) return;
                compatible.Add(p);
            });
            return compatible;
        }

        public override void Save()
        {
            if (GraphAsset == null) return;
            // TODO: Purge Keys from connections tables that have no values in their port collection?
            EditorUtility.SetDirty(GraphAsset);
            AssetDatabase.SaveAssets();
        }
        
        public void Load(GraphAsset asset)
        {
            if (asset == null) return;
            if (asset.Graph == null) asset.Graph = new Graph();
            GraphAsset = asset;
            Reload();
        }

        public void Reload()
        {
            Cleanup();
            if (GraphAsset == null) return;
            Graph.Definition(Graph);
            _nodeViewCache = new Dictionary<string, INodeView>(Graph.NodeCount);
            CreateSearch();
            // TODO: If graph.NodeCount > 100 we need a loading bar and maybe an async process that does the below
            // https://docs.unity3d.com/ScriptReference/EditorUtility.DisplayProgressBar.html
            CreateNodeViews();
            CreateConnections();
        }

        protected void Cleanup()
        {
            graphViewChanged = null;
            DeleteElements(graphElements.ToList());
            graphViewChanged = OnGraphViewChanged;
        }

        private void CreateNodeViews()
        {
            foreach (var node in Graph.Nodes)
            {
                CreateNodeView(node);
            }
        }

        private void CreateConnections()
        {
            foreach (var node in Graph.Nodes)
            {
                var view = _nodeViewCache[node.NodeId];
                CreateValueConnections(view, node);
                if (node is IFlowNode flowNode) CreateFlowConnections(view, flowNode);
            }
        }
        
        private void CreateValueConnections(INodeView view, INode node)
        {
            foreach (var valueIn in node.ValueInPorts.Values)
            {
                foreach (var connection in Graph.ValueInConnections.SafeGet(valueIn.Id))
                {
                    if (!_nodeViewCache.TryGetValue(connection.Node, out var outputView)) continue;
                    var inputPort = view.ValueInPortContainer.Q<PortView>(valueIn.Id.Port);
                    var outputPort = outputView.ValueOutPortContainer.Q<PortView>(connection.Port);
                    if (inputPort != null && outputPort != null)
                        AddElement(outputPort.ConnectTo(inputPort));
                }
            }
        }

        private void CreateFlowConnections(INodeView view, IFlowNode flowNode)
        {
            foreach (var flowOut in flowNode.FlowOutPorts.Values)
            {
                foreach (var connection in Graph.FlowOutConnections.SafeGet(flowOut.Id))
                {
                    if (!_nodeViewCache.TryGetValue(connection.Node, out var inputView))
                    {
                        Debug.Log($"Unable To Find Node View for {connection.Node}");
                        continue;
                    }
                    var inputPort = inputView.FlowInPortContainer.Q<PortView>(connection.Port);
                    var outputPort = view.FlowOutPortContainer.Q<PortView>(flowOut.Id.Port);
                    if (inputPort != null && outputPort != null)
                        AddElement(outputPort.ConnectTo(inputPort));
                    else
                        Debug.Log("Unable To Make a Flow Port Connection");
                }
            }
        }

        private void GeometryChangedCallback(GeometryChangedEvent evt)
        {
            MiniMap.SetPosition(new Rect(worldBound.width - 205, 25, 200, 100));
        }

        private void CleanupFlowConnectionElements(PortView port)
        {
            foreach (Edge connection in new List<Edge>(port.connections))
            {
                if ((connection.capabilities & Capabilities.Deletable) != 0)
                {
                    Graph.Disconnect((IPort) connection.output.userData, (IPort) connection.input.userData);
                    // Replicate what Unity is doing in their "DeleteElement" method
                    connection.output.Disconnect(connection);
                    connection.input.Disconnect(connection);
                    // connection.output = null;
                    // connection.input = null;
                    RemoveElement(connection);
                }
            }
        }
        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            RecordUndo("Graph Edit");
            bool changeMade = false;
            if (change.elementsToRemove != null)
            {
                foreach (var element in change.elementsToRemove)
                {
                    switch (element)
                    {
                        case INodeView view:
                            changeMade = true;
                            view.FlowInPortContainer.Query<PortView>().ForEach(CleanupFlowConnectionElements);
                            view.FlowOutPortContainer.Query<PortView>().ForEach(CleanupFlowConnectionElements);
                            _nodeViewCache.Remove(view.Node.NodeId);
                            Graph.Remove(view.Node);
                            break;
                        case Edge edge:
                            changeMade = true;
                            Graph.Disconnect((IPort)edge.output.userData, (IPort)edge.input.userData);
                            break;
                        default:
                            Debug.LogWarning($"Unhandeled GraphElement Removed: {element.GetType().FullName} | {element.name} | {element.title}");
                            break;
                    }
                }
            }
            
            if (change.movedElements != null)
            {
                foreach (var element in change.movedElements)
                {
                    switch (element)
                    {
                        case INodeView view:
                            changeMade = true; 
                            view.Node.NodeRect = view.GetPosition();
                            break;
                        default:
                            Debug.LogWarning($"Unhandeled GraphElement Moved: {element.GetType().FullName} | {element.name} | {element.title}");
                            break;
                    }
                }
            }
            
            if (change.edgesToCreate != null)
            {
                foreach (var edge in change.edgesToCreate)
                {
                    changeMade = true;
                    Graph.Connect((IPort)edge.output.userData, (IPort)edge.input.userData);
                }
            }

            if (changeMade)
            {
                Save();
            }
            
            return change;
        }
        
        private void OnViewTransformChanged(GraphView graphview)
        {
            
        }

        private void OnKeyUp(KeyUpEvent evt)
        {
            if (evt.target != this)
            {
                return;
            }

            switch (evt.keyCode)
            {
                case KeyCode.C when !evt.ctrlKey && !evt.commandKey:
                    // AddComment();
                    break;
                case KeyCode.M:
                    MiniMap.visible = !MiniMap.visible;
                    break;
                case KeyCode.H when !evt.ctrlKey && !evt.commandKey:
                    HorizontallyAlignSelectedNodes();
                    break;
                case KeyCode.V when !evt.ctrlKey && !evt.commandKey:
                    VerticallyAlignSelectedNodes();
                    break;
            }
        }
        
        protected void HorizontallyAlignSelectedNodes()
        {
            float sum = 0;
            int count = 0;
            
            // TODO: Implement a way to Align to "first selected" thing rather then average
            foreach (var selectable in selection)
            {
                if (selectable is NodeView node)
                {
                    sum += node.GetPosition().xMin;
                    count++;
                }
            }

            float xAvg = sum / count;
            foreach (var selectable in selection)
            {
                if (selectable is INodeView node && node.IsMoveable)
                {
                    var pos = node.GetPosition();
                    pos.xMin = xAvg;
                    node.SetPosition(pos);
                }
            }
        }
        

        protected void VerticallyAlignSelectedNodes()
        {
            float sum = 0;
            int count = 0;

            foreach (var selectable in selection)
            {
                if (selectable is INodeView node && node.IsMoveable)
                {
                    sum += node.GetPosition().yMin;
                    count++;
                }
            }

            float yAvg = sum / count;
            foreach (var selectable in selection)
            {
                if (selectable is NodeView node)
                {
                    var pos = node.GetPosition();
                    pos.yMin = yAvg;
                    node.SetPosition(pos);
                }
            }
        }
    }
}