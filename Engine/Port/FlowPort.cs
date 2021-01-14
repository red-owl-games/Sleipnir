using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    public interface IFlowPort : IPort
    {
        void Definition(INode node, IFlowPortAttribute info);
        IFlowPort Clone(IGraph graph);
        IEnumerable Execute();
        
    }

    public enum CallbackTypes
    {
        None,
        Simple,
        Sync,
        Async,
    }

    [Preserve]
    public class FlowPort : Port, IFlowPort
    {
        private static readonly Type VoidType = typeof(void);
        private static readonly Type FlowType = typeof(IFlow);
        private static readonly Type FlowPortType = typeof(IFlowPort);
        private static readonly Type EnumerableType = typeof(IEnumerable);
        private static readonly Type SimpleCallbackType = typeof(Action<IFlow>);
        private static readonly Type SyncCallbackType = typeof(Func<IFlow, IFlowPort>);
        private static readonly Type AsyncCallbackType = typeof(Func<IFlow, IEnumerable>);
        
        private IFlowPort Linked { get; set; }

        private CallbackTypes _callbackType;
        private Action<IFlow> _simpleCallback;
        private Func<IFlow, IFlowPort> _syncCallback;
        private Func<IFlow, IEnumerable> _asyncCallback;

        [Preserve]
        public FlowPort()
        {
            ValueType = FlowPortType;
        }

        public void Definition(INode node, IFlowPortAttribute info)
        {
            Id = new PortId(node.NodeId, info.Name);
            Node = node;
            Name = info.Name;
            Direction = info.Direction;
            Capacity = info.Capacity;
            GraphPort = info.GraphPort;
            if (info.CallbackInfo == null) return;
            var parameters = info.CallbackInfo.GetParameters();
            if (parameters.Length != 1)
            {
                Debug.LogWarning($"FlowPort Callback for '{node}.{info.CallbackInfo.Name}' has {parameters.Length} parameter(s).  Can only accept 1 parameter of type 'IFlow'");
                return;
            }

            var paramType = parameters[0].ParameterType;
            if (paramType != FlowType)
            {
                Debug.LogWarning($"FlowPort Callback for '{node}.{info.CallbackInfo.Name}' has 1 parameter that takes type '{paramType}'.  Can only accept 1 parameter of type 'IFlow'");
                return;
            }
            if (VoidType.IsAssignableFrom(info.CallbackInfo.ReturnType))
            {
                _simpleCallback = (Action<IFlow>)info.CallbackInfo.CreateDelegate(SimpleCallbackType, node);
                _callbackType = CallbackTypes.Simple;
            }
            if (FlowPortType.IsAssignableFrom(info.CallbackInfo.ReturnType))
            {
                _syncCallback = (Func<IFlow, IFlowPort>)info.CallbackInfo.CreateDelegate(SyncCallbackType, node);
                _callbackType = CallbackTypes.Sync;
            }
            if (EnumerableType.IsAssignableFrom(info.CallbackInfo.ReturnType))
            {
                _asyncCallback = (Func<IFlow, IEnumerable>)info.CallbackInfo.CreateDelegate(AsyncCallbackType, node);
                _callbackType = CallbackTypes.Async;
            }
            if (_callbackType == CallbackTypes.None) Debug.LogWarning($"FlowPort Callback for '{node}.{info.CallbackInfo.Name}' did not have one of the following method signatures [Action<IFlow>, Func<IFlow, IFlowPort>, Func<IFlow, IEnumerable>]");
        }

        public IFlowPort Clone(IGraph graph)
        {
            string newName = (Node is IGraphFlowPortNode graphPortNode) ? graphPortNode.Name : $"On {Name}";
            var port = new FlowPort
            {
                Id = new PortId(graph.NodeId, newName),
                Node = graph,
                Name = newName,
                Direction = Direction.Flip(),
                Capacity = Capacity,
                Flow = Flow,
            };

            switch (Direction)
            {
                case PortDirection.Input:
                    Linked = port;
                    break;
                case PortDirection.Output:
                    port.Linked = this;
                    break;
            }

            return port;
        }

        public override void Initialize(ref IFlow flow)
        {
            Flow = flow;
        }

        public virtual IEnumerable Execute()
        {
            switch (Direction)
            {
                case PortDirection.Input:
                    switch (_callbackType)
                    {
                        case CallbackTypes.Simple:
                            _simpleCallback?.Invoke(Flow);
                            break;
                        case CallbackTypes.Sync:
                            yield return _syncCallback(Flow);
                            break;
                        case CallbackTypes.Async:
                            foreach (var item in _asyncCallback(Flow))
                            {
                                yield return item;
                            }
                            break;
                    }

                    if (Linked != null)
                    {
                        //Debug.Log($"Linked Port '{Node} {this}' => '{Linked.Node} {Linked}'");
                        yield return Linked;
                    }
                    break;
                case PortDirection.Output:
                    switch (_callbackType)
                    {
                        // TODO: Validate FlowOut ports shouldn't support the other callbacks
                        case CallbackTypes.Simple:
                            _simpleCallback?.Invoke(Flow);
                            break;
                    }
                    foreach (var input in Graph.FlowOutConnections.SafeGet(Id))
                    {
                        var nextNode = (IFlowNode)Graph.GetNode(input.Node);
                        var nextPort = nextNode?.FlowInPorts[input.Port];
                        if (nextNode != null && nextPort != null)
                        {
                            // Debug.Log($"Traversing Towards Port {nextPort}");
                            yield return nextPort;
                        }
                    }
                    break;
            }
        }
    }
}