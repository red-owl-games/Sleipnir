using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IGraph : IFlowNode
    {
        IEnumerable<INode> Nodes { get; }
        int NodeCount { get; }
        IEnumerable<IFlowNode> RootNodes { get; }
        INode GetNode(string id);
        IEnumerable<T> GetNodes<T>() where T : INode;
        IEnumerable<INode> GetNodes(Type type);
        T Add<T>(T node) where T : INode;
        bool Get(string id, out INode node);
        void Remove<T>(T node) where T : INode;
        void Clear();

        ConnectionsGraph ValueInConnections { get; }
        ConnectionsGraph FlowOutConnections { get; }
        void Connect(IPort output, IPort input);
        void Disconnect(IPort output, IPort input);
    }

    [Serializable]
    [Graph]
    [Node("Common", Name = "SubGraph", Path = "Common")]
    public class Graph : Node, IGraph
    {
        [SerializeField]
        private string title = "Sub Graph";

        public string Title => title;
        
        [SerializeReference, HideInInspector] 
        private List<INode> _nodes;

        public IEnumerable<INode> Nodes => _nodes;
        public int NodeCount => _nodes.Count;

        public IEnumerable<IFlowNode> RootNodes
        {
            get
            {
                foreach (var node in _nodes)
                {
                    if (node is IFlowNode flowNode && flowNode.IsFlowRoot) yield return flowNode;
                }
            }
        }

        [SerializeField, HideInInspector]
        private ConnectionsGraph valueInConnections;
        public ConnectionsGraph ValueInConnections => valueInConnections;
        
        [SerializeField, HideInInspector]
        private ConnectionsGraph flowOutConnections;
        public ConnectionsGraph FlowOutConnections => flowOutConnections;

        public Graph()
        {
            Clear();
        }

        private void EnsureRequiredNodes(SleipnirGraphInfo data)
        {
            foreach (var attribute in data.RequiredNodes)
            {
                var nodes = new List<INode>(GetNodes(attribute.Type));
                if (nodes.Count == 0)
                {
                    var node = (INode)Activator.CreateInstance(attribute.Type);
                    Add(node); // Definition & Initialize are called here
                    node.NodePosition = attribute.Position;
                }
            }
        }
        
        protected override void OnDefinition()
        {
            base.OnDefinition();
            if (SleipnirGraphReflector.GraphCache.Get(GetType(), out var data))
            {
                EnsureRequiredNodes(data);
            }
            
            // TODO: Generate Dynamic Ports based on graph's PortNodes
            foreach (var node in _nodes)
            {
                node.Definition(this);
            }
        }

        protected override void OnInitialize(ref IFlow flow)
        {
            foreach (var node in _nodes)
            {
                if (node is IFlowNode flowNode) flowNode.Initialize(ref flow);
            }
        }

        public INode GetNode(string id)
        {
            foreach (var node in _nodes)
            {
                if (node.NodeId == id) return node;
            }
            Debug.Log($"Cannot Find Node '{id}' in graph");
            return null;
        }
        
        public IEnumerable<T> GetNodes<T>() where T : INode
        {
            foreach(var node in GetNodes(typeof(T)))
            {
                yield return (T) node;
            }
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
            if (node is IFlowNode flowNode)
            {
                flowNode.Definition(this);
            }
            else
            {
                node.Definition(this);
            }
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
        }

        private void CleanupFlowPortConnections(INode target)
        {
            if (!(target is IFlowNode targetFlowNode)) return;
            foreach (var port in targetFlowNode.FlowOutPorts.Values)
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
    }
}
