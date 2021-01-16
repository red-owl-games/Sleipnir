using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    public interface IValuePort : IPort
    {
        object DefaultValue { get; }
        object WeakValue { get; }
        T GetValue<T>();
        void Definition(INode node, IValuePortAttribute info);
        IValuePort Clone(IGraph graph);
        void MarkDirty();
        IEnumerable Execute();
    }

    [Preserve]
    public class ValuePort<T> : Port, IValuePort
    {
        private static readonly Type VoidType = typeof(void);
        private static readonly Type SimpleCallbackType = typeof(Action);
        private static readonly Type ValueCallbackType = typeof(Func<T>);
        
        public enum CallbackTypes
        {
            None,
            Simple,
            Value,
        }
        
        private IValuePort Linked { get; set; }
        
        private CallbackTypes _callbackType;
        private Action _simpleCallback;
        private Func<T> _valueCallback;
        
        
        public object DefaultValue { get; private set; }
        public object WeakValue => Value;
        
        
        
        public T Value
        {
            get
            {
                switch (Direction)
                {
                    case PortDirection.Input:
                        var connections = Graph.ValueInConnections.SafeGet(Id);
                        if (connections.Count == 0) return GetValue<T>();
                        foreach (var connection in connections)
                        {
                            var nextNode = Graph.GetNode(connection.Node); //  TODO: Needs saftey check?
                            var nextPort = nextNode?.ValueOutPorts?[connection.Port]; // TODO: Needs saftey check?
                            if (nextNode != null && nextPort != null)
                            {
                                return nextPort.GetValue<T>();
                            }
                        }

                        break;
                    case PortDirection.Output:
                        return GetValue<T>();
                }

                return Flow != null && Flow.ContainsKey(Id) ? Flow.Get<T>(Id) : (T) DefaultValue;
            }
            set
            {
                if (Flow == null)
                {
                    DefaultValue = value;
                }
                else
                {
                    Flow.Set(Id, value);
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                switch (Direction)
                {
                    case PortDirection.Input:
                        return Graph.ValueInConnections.SafeGet(Id).Count > 0;
                    case PortDirection.Output:
                        // TODO: Broken / Needs To be Fixed
                        return Graph.ValueInConnections.SafeGet(Id).Count > 0;
                }

                return false;
            }
        }

        public TValue GetValue<TValue>()
        {
            switch (_callbackType)
            {
                case CallbackTypes.Simple:
                    _simpleCallback?.Invoke();
                    // WHAT DO WE RETURN?
                    break;
                case CallbackTypes.Value:
                    // TODO: is there a better way?
                    return (TValue)(object) _valueCallback();
            }

            return (TValue) DefaultValue;
        }

        [Preserve]
        public ValuePort()
        {
            Value = default;
        }
        
        public ValuePort(T defaultValue)
        {
            Value = defaultValue;
        }

        public void SetDefault(T defaultValue) => DefaultValue = defaultValue;
        
        public void Definition(INode node, IValuePortAttribute info)
        {
            Id = new PortId(node.NodeId, info.Name);
            Node = node;
            Name = info.Name;
            Direction = info.Direction;
            Capacity = info.Capacity;
            GraphPort = info.GraphPort;
            ValueType = typeof(T);
            if (info.CallbackInfo == null) return;
            var parameters = info.CallbackInfo.GetParameters();
            if (parameters.Length > 0)
            {
                Debug.LogWarning($"ValuePort Callback for '{node}.{info.CallbackInfo.Name}' has {parameters.Length} parameter(s).  Callback cannot accept any parameters");
                return;
            }
            if (VoidType.IsAssignableFrom(info.CallbackInfo.ReturnType))
            {
                _simpleCallback = (Action)info.CallbackInfo.CreateDelegate(SimpleCallbackType, node);
                _callbackType = CallbackTypes.Simple;
            }
            if (ValueType.IsAssignableFrom(info.CallbackInfo.ReturnType))
            {
                _valueCallback = (Func<T>)info.CallbackInfo.CreateDelegate(ValueCallbackType, node);
                _callbackType = CallbackTypes.Value;
            }
            if (_callbackType == CallbackTypes.None) Debug.LogWarning($"ValuePort Callback for '{node}.{info.CallbackInfo.Name}' did not have one of the following method signatures [Action, Func<T>]");
        }

        public IValuePort Clone(IGraph graph)
        {
            string newName = (Node is IGraphValuePortNode graphPortNode) ? graphPortNode.Name : $"{Name} {Direction.Flip()}";
            var port = new ValuePort<T>()
            {
                Id = new PortId(graph.NodeId, newName),
                Node = graph,
                Name = newName,
                Direction = Direction.Flip(),
                Capacity = Capacity,
                ValueType = ValueType,
                DefaultValue = DefaultValue,
                Flow = Flow,
                Linked = this,
            };
            return port;
        }

        public override void Initialize(ref IFlow flow)
        {
            Flow = flow;
            Flow.Set(Id, (T)DefaultValue);
        }

        public void MarkDirty()
        {
            switch (Direction)
            {
                case PortDirection.Input:
                    Debug.Log("Fix This");
                    break;
                case PortDirection.Output:
                    // TODO: It is very tough to walk this direction in Value Ports - need to fix this
                    foreach (var connection in Graph.ValueInConnections)
                    {
                        foreach (var connected in connection.Value)
                        {
                            if (connected.Node == Id.Node && connected.Port == Id.Port) Graph.GetNode(connection.Key.Node)?.MarkDirty();
                        }
                    }
                    break;
            }
        }

        public IEnumerable Execute()
        {
            // TODO: Refactor This - There has to be a better way
            switch (Direction)
            {
                case PortDirection.Input:
                    foreach (var connection in Graph.ValueInConnections.SafeGet(Id))
                    {
                        var nextNode = Graph.GetNode(connection.Node); //  TODO: Needs saftey check?
                        var nextPort = nextNode?.ValueOutPorts[connection.Port]; // TODO: Needs saftey check?
                        if (nextNode != null && nextPort != null)
                        {
                            yield return nextPort;
                            Flow.Set(Id, nextPort.WeakValue);
                        }
                    }
                    if (Linked != null)
                    {
                        // Debug.Log($"[Input] Has Linked Port | {this} => {Linked}");
                        Flow.Set(Linked.Id, Value);
                    }
                    break;
                case PortDirection.Output:
                    if (Linked != null)
                    {
                        foreach (var connection in Linked.Graph.ValueInConnections.SafeGet(Linked.Id))
                        {
                            var nextNode = Linked.Graph.GetNode(connection.Node); //  TODO: Needs saftey check?
                            var nextPort = nextNode?.ValueOutPorts[connection.Port]; // TODO: Needs saftey check?
                            if (nextNode != null && nextPort != null)
                            {
                                yield return nextPort;
                                Flow.Set(Id, nextPort.WeakValue);
                            }
                        }
                    }
                    else
                    {
                        Flow.Set(Id, Value);
                    }
                    break;
            }
        }

        public static implicit operator T(ValuePort<T> self) => self.Value;
        public static implicit operator ValuePort<T>(T self) => new ValuePort<T>(self);
    }
}