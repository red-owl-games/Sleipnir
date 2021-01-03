using System;
using System.Collections.Generic;
using System.Linq;

namespace RedOwl.Sleipnir.Engine
{
    public interface IFlow
    {
        IGraph Graph { get; }
        IFlowNode[] RootNodes { get; }
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
        public IFlowNode[] RootNodes { get; }
        
        public Flow(IGraph graph, IFlowNode[] nodes = null)
        {
            Graph = graph;
            RootNodes = nodes ?? new List<IFlowNode>(graph.RootNodes).ToArray();
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

        public void Execute() => Execute(this);

        #region Static
        
        public static void Execute(IFlow flow)
        {
            flow.Graph.Initialize(ref flow);
            foreach (var node in flow.RootNodes)
            {
                flow.Clear();
                WalkFlowPorts(ref flow, node);
            }
        }

        public static void ExecutePort(ref IFlow flow, IFlowPort port)
        {
            // TODO: This might need to be a custom while loop executor to handle supporting Yield Instructions
            while (port.Execute(flow).MoveNext()) {}
        }
        
        public static void WalkValuePorts(ref IFlow flow, INode node)
        {
            // Should only ever be called on "Nodes" that are about to have its FlowIn port executed
            // Recursively walks back up the value input ports connections to make sure values are "set" into the flow
            foreach (var port in node.ValueInPorts.Values)
            {
                WalkValueIn(ref flow, node, port);
            }
        }

        public static void WalkValueIn(ref IFlow flow, INode node, IValuePort input)
        {
            // Debug.Log($"Walking ValueIn '[{node}]{input}'");
            foreach (var connection in flow.Graph.ValueInConnections.SafeGet(input.Id))
            {
                var nextNode = flow.Graph.GetNode(connection.Node); //  TODO: Needs saftey check?
                var output = nextNode.ValueOutPorts[connection.Port]; // TODO: Needs saftey check?
                // Debug.Log($"Pulled Value for '[{node}]{input}' from '[{nextNode}]{output}'");
                flow.Set(input.Id, output.WeakValue);
            }
        }

        public static void WalkFlowPorts(ref IFlow flow, IFlowNode node)
        {
            // Should only ever be called on "Root Nodes" or nodes that "fork" the flow
            // This will walk recursive down the nodes flow out ports
            foreach (var port in node.FlowOutPorts.Values)
            {
                WalkFlowOut(ref flow, node, port);
            }
        }

        public static void WalkFlowOut(ref IFlow flow, IFlowNode node, IFlowPort output)
        {
            //Debug.Log($"Walking FlowOut '[{node}]{output}'");
            if (node.IsFlowRoot) WalkValuePorts(ref flow, node);
            ExecutePort(ref flow, output);
            foreach (var input in flow.Graph.FlowOutConnections.SafeGet(output.Id))
            {
                var nextNode = (IFlowNode)flow.Graph.GetNode(input.Node);
                var nextPort = nextNode?.FlowInPorts[input.Port];
                //Debug.Log($"Traversing Towards '[{nextNode}]{nextPort}'");
                if (nextNode != null && nextPort != null) WalkFlowIn(ref flow, nextNode, nextPort);
            }
        }

        public static void WalkFlowIn(ref IFlow flow, IFlowNode node, IFlowPort input)
        {
            //Debug.Log($"Walking FlowIn '[{node}]{input}'");
            WalkValuePorts(ref flow, node);
            var enumerator = input.Execute(flow);
            while (enumerator.MoveNext())
            {
                // TODO: Handle Yield Instructions / Custom Yield Instructions
                if (enumerator.Current is IFlowPort nextPort) // TODO: saftey check for being given a FlowOut port?
                {
                    //Debug.Log($"Traversing Towards '[{node}]{nextPort}'");
                    WalkFlowOut(ref flow, node, nextPort);
                }
            }
        }
        
        #endregion
    }

    public class Flow<T> : Flow where T : IFlowNode
    {
        public Flow(IGraph graph) : base(graph, graph.GetNodes(typeof(T)).Cast<IFlowNode>().ToArray()) {}
    }
    
    // TODO: AsyncFlow<T> ?
}
