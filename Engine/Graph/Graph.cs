using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IGraph : INode
    {
        IEnumerable<INode> Nodes { get; }
        int NodeCount { get; }
        INode GetNode(string id);
        IEnumerable<INode> GetNodes(Type type);
        T Add<T>(T node) where T : INode;
        bool Get(string id, out INode node);
        void Remove<T>(T node) where T : INode;
        void Clear();

        ConnectionsGraph ValueInConnections { get; }
        ConnectionsGraph FlowOutConnections { get; }
        void Connect(IPort output, IPort input);
        void Disconnect(IPort output, IPort input);
        
        void NotifyDirty();
    }

    [Serializable]
    [Graph]
    [Node("Common", Name = "SubGraph", Path = "Common")]
    public class Graph : Node, IGraph
    {
        [SerializeReference]
        [HideInInspector] 
        private List<INode> _nodes;

        public IEnumerable<INode> Nodes => _nodes;
        public int NodeCount => _nodes.Count;

        [SerializeField]
        [HideInInspector]
        private ConnectionsGraph valueInConnections;
        public ConnectionsGraph ValueInConnections => valueInConnections;
        
        [SerializeField]
        [HideInInspector]
        private ConnectionsGraph flowOutConnections;
        public ConnectionsGraph FlowOutConnections => flowOutConnections;

        public Graph()
        {
            Clear();
        }

        private void EnsureRequiredNodes(GraphAttribute data)
        {
            foreach (var attribute in data.RequiredNodes)
            {
                var nodes = new List<INode>(GetNodes(attribute.Type));
                if (nodes.Count == 0)
                {
                    var node = (INode)Activator.CreateInstance(attribute.Type);
                    // TODO: Change capabilities of required node to not be moveable or deleteable
                    Add(node); // Definition & Initialize are called here
                    node.NodePosition = attribute.Position;
                }
            }
        }

        protected override void OnDirty()
        {
            foreach (var node in _nodes)
            {
                node.MarkDirty();
            }
        }

        public void NotifyDirty()
        {
            OnGraphChanged();
        }
        
        protected virtual void OnGraphChanged() {}

        protected override void OnDefinition()
        {
            if (GraphAttribute.Cache.TryGet(GetType(), out var data))
            {
                EnsureRequiredNodes(data);
            }
            
            foreach (var node in _nodes)
            {
                node.Definition(this);
            }
        }

        protected override void OnInitialize(ref IFlow flow)
        {
            foreach (var node in _nodes)
            {
                node.Initialize(ref flow);
            }
        }

        [CanBeNull]
        public INode GetNode(string id)
        {
            foreach (var node in _nodes)
            {
                if (node.NodeId == id) return node;
            }
            Debug.Log($"Cannot Find Node '{id}' in graph");
            return null;
        }

        public IEnumerable<INode> GetNodes(Type nodeType)
        {
            foreach (var node in _nodes)
            {
                if (nodeType.IsInstanceOfType(node)) 
                    yield return node;
            }
        }

        public T Add<T>(T node) where T : INode
        {
            _nodes.Add(node);
            node.Definition(this);
            return node;
        }

        public bool Get(string id, out INode node)
        {
             foreach (var n in _nodes)
             {
                 if (n.NodeId != id) continue;
                 node = n;    
                 return true;
             }

             node = null;
             return false;
        }

        public void Remove<T>(T node) where T : INode
        {
            // if (node is IGraphPort) IsDefined = false;
            // TODO: If _nodes was a dictionary this would be easier
            for (int i = _nodes.Count - 1; i >= 0; i--)
            {
                var n = _nodes[i];
                if (n.NodeId == node.NodeId)
                {
                    CleanupFlowPortConnections(n);
                    CleanupValuePortConnections(n);
                    _nodes.RemoveAt(i);
                }
            }
        }

        public void Clear()
        {
            _nodes = new List<INode>();
            valueInConnections = new ConnectionsGraph();
            flowOutConnections = new ConnectionsGraph();
            // IsDefined = false;
        }

        private void CleanupFlowPortConnections(INode target)
        {
            foreach (var port in target.FlowOutPorts.Values)
            {
                FlowOutConnections.Remove(port.Id);
            }
        }

        private void CleanupValuePortConnections(INode target)
        {
            foreach (var port in target.ValueInPorts.Values)
            {
                ValueInConnections.Remove(port.Id);
            }
        }

        // Value Ports - Input -> [Output, Output]
        // Flow Port -  Output -> [Input, Input]
        public void Connect(IPort output, IPort input)
        {
            // TODO: if we can figure out what node each port came from we can stop storing the node ID with the port
            if (input is IValuePort valueIn && output is IValuePort valueOut)
                valueInConnections.Connect(valueIn.Id, valueOut.Id);
            if (input is IFlowPort flowIn && output is IFlowPort flowOut) 
                flowOutConnections.Connect(flowOut.Id, flowIn.Id);
        }

        public void Disconnect(IPort output, IPort input)
        {
            if (input is IValuePort valueIn && output is IValuePort valueOut)
                valueInConnections.Disconnect(valueIn.Id, valueOut.Id);
            if (input is IFlowPort flowIn && output is IFlowPort flowOut)
                flowOutConnections.Disconnect(flowOut.Id, flowIn.Id);
        }

        public override string ToString()
        {
            return $"{GetType().Name}({NodeId.Substring(0,8)})";
        }
    }
    
    public static class GraphExtensions
    {
        public static IEnumerable<T> GetNodes<T>(this IGraph graph) where T : INode
        {
            foreach(var node in graph.GetNodes(typeof(T)))
            {
                yield return (T) node;
            }
        }
        
        public static IEnumerable<INode> GetRootNodes(this IGraph graph)
        {
            foreach (var node in graph.Nodes)
            {
                if (node.IsFlowRoot) yield return node;
            }
        }
    }
}
