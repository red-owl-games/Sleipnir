using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IFlow
    {
        IGraph Graph { get; }
        IEnumerable<INode> RootNodes { get; }
        bool ContainsKey(PortId id);
        bool ContainsKey(string key);
        T Get<T>(PortId id);
        T Get<T>(string key);
        void Set<T>(PortId id, T value);
        void Set<T>(string key, T value);
        void Clear();
        void Execute();
    }

    public class Flow : BetterDictionary<string, object>, IFlow
    {
        public IGraph Graph { get; }

        private readonly INode[] rootNodes;
        public IEnumerable<INode> RootNodes => rootNodes;

        public Flow(IGraph graph)
        {
            Graph = graph;
            rootNodes = graph.GetRootNodes() as INode[];
        }
        
        public Flow(IGraph graph, params INode[] nodes)
        {
            Graph = graph;
            rootNodes = nodes;
        }

        private string FormatKey(PortId id) => $"{id.Node}.{id.Port}";

        public bool ContainsKey(PortId id) => ContainsKey(FormatKey(id));
        
        public T Get<T>(PortId id) => Get<T>(FormatKey(id));
        public T Get<T>(string key)
        {
            var type = typeof(T);
            var value = this[key];
            switch (value)
            {
                case null when type.IsValueType:
                    throw new InvalidCastException($"Cannot cast null to value type `{type.FullName}`");
                case null:
                case T _: // Short circuit Convert.ChangeType if we can cast quicker
                    return (T)value;
                default: // Try for IConvertible support
                    try
                    {
                        return (T)Convert.ChangeType(value, type);
                    }
                    catch (Exception e)
                    {
                        throw new InvalidCastException($"Cannot cast `{value.GetType()}` to `{type}`. Error: {e}.");
                    }
            }
        }

        public void Set<T>(PortId id, T value) => Set(FormatKey(id), value);
        
        public void Set<T>(string key, T value)
        {
            if (ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                Add(key, value);
            }
        }

        public void Execute() => Walk(this);
        
        #region FlowAPI

        private void Walk(IFlow flow)
        {
            // TODO: there appears to be an initialization bug with ValueNodes and their default value not getting setup
            flow.Graph.Initialize(ref flow);
            foreach (var rootNode in flow.RootNodes)
            {
                WalkFlowPorts(rootNode);
            }
        }

        private void WalkFlowPorts(INode node)
        {
            WalkValuePorts(node);
            foreach (var port in node.FlowOutPorts.Values)
            {
                WalkFlowPort(port);
            }
        }
        
        private void WalkFlowPort(IFlowPort port)
        {
            foreach (var next in port.Execute())
            {
                // TODO: Handle Yield Instructions / Custom Yield Instructions
                if (next is IFlowPort nextPort)
                {
                    // Debug.Log($"Moving Towards Next Port {nextPort}");
                    if (nextPort.Direction == PortDirection.Input)
                    {
                        WalkValuePorts(nextPort.Node);
                    }
                    WalkFlowPort(nextPort);
                }
            }
        }

        private void WalkValuePorts(INode node)
        {
            // Debug.Log($"Walking Node '{node}' for Values");
            foreach (var port in node.ValueInPorts.Values)
            {
                WalkValuePort(port);
            }
        }

        private void WalkValuePort(IValuePort port)
        {
            // Debug.Log($"Walking Value Port {port}");
            foreach (var next in port.Execute())
            {
                // TODO: Handle Yield Instructions / Custom Yield Instructions
                if (next is IValuePort nextPort)
                {
                    // Debug.Log($"Moving Towards Next Port {nextPort}");
                    if (nextPort.Direction == PortDirection.Output)
                    {
                        WalkValuePort(nextPort);
                    }
                }
            }
        }

        #endregion
    }

    public class Flow<T> : Flow where T : INode
    {
        public Flow(IGraph graph) : base(graph, graph.GetNodes(typeof(T)).ToArray()) {}
    }
    
    // TODO: AsyncFlow<T> ?
}
