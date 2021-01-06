using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    public interface IFlowPort : IPort
    {
        void Definition(INode node, FlowPortSettings settings);
        void Link(IFlowPort other);
        IFlowPort Clone(INode node);
        IEnumerator Execute();
    }

    public enum CallbackTypes
    {
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
        private static readonly Type EnumeratorType = typeof(IEnumerator);
        private static readonly Type SimpleCallbackType = typeof(Action<IFlow>);
        private static readonly Type SyncCallbackType = typeof(Func<IFlow, IFlowPort>);
        private static readonly Type AsyncCallbackType = typeof(Func<IFlow, IEnumerator>);
        
        public Type ValueType => FlowPortType;

        private CallbackTypes _callbackType;
        private Action<IFlow> _simpleCallback;
        private Func<IFlow, IFlowPort> _syncCallback;
        private Func<IFlow, IEnumerator> _asyncCallback;
        
        [Preserve]
        public FlowPort() {}
        
        public FlowPort(FlowPort other, INode node, PortDirection direction, string name)
        {
            Id = new PortId(node.NodeId, name);
            Graph = node.Graph;
            Name = name;
            Direction = direction;
            Capacity = other.Capacity;
            Flow = other.Flow;
        }

        public void Link(IFlowPort other)
        {
            _callbackType = CallbackTypes.Async;
            _asyncCallback = (flow) => CreateCallback(other);
        }

        private IEnumerator CreateCallback(IFlowPort other)
        {
            yield return new TraverseIntoNestedGraph(other.Graph);
            yield return other;
        }

        public void Definition(INode node, FlowPortSettings settings)
        {
            Id = new PortId(node.NodeId, settings.Name);
            Graph = node.Graph;
            Name = settings.Name;
            Direction = settings.Direction;
            Capacity = settings.Capacity;
            if (settings.Callback == null) return;
            var parameters = settings.Callback.GetParameters();
            if (parameters.Length != 1)
            {
                Debug.LogWarning($"FlowPort Callback for '{node}.{settings.Callback.Name}' has {parameters.Length} parameter(s).  Can only accept 1 parameter of type 'IFlow'");
                return;
            }

            var paramType = parameters[0].ParameterType;
            if (paramType != FlowType)
            {
                Debug.LogWarning($"FlowPort Callback for '{node}.{settings.Callback.Name}' has 1 parameter that takes type '{paramType}'.  Can only accept 1 parameter of type 'IFlow'");
                return;
            }
            if (VoidType.IsAssignableFrom(settings.Callback.ReturnType))
            {
                _simpleCallback = (Action<IFlow>)settings.Callback.CreateDelegate(SimpleCallbackType, node);
                _callbackType = CallbackTypes.Simple;
            }
            if (FlowPortType.IsAssignableFrom(settings.Callback.ReturnType))
            {
                _syncCallback = (Func<IFlow, IFlowPort>)settings.Callback.CreateDelegate(SyncCallbackType, node);
                _callbackType = CallbackTypes.Sync;
            }
            if (EnumeratorType.IsAssignableFrom(settings.Callback.ReturnType))
            {
                _asyncCallback = (Func<IFlow, IEnumerator>)settings.Callback.CreateDelegate(AsyncCallbackType, node);
                _callbackType = CallbackTypes.Async;
            }
            // if (!_hasCallback) Debug.LogWarning($"FlowPort Callback for '{node}.{settings.Callback.Name}' did not have one of the following method signatures [Action<IFlow>, Func<IFlow, IFlowPort>, Func<IFlow, IEnumerator>]");
            // TODO: Log about bad callback setup?
        }
        
        public void Initialize(ref IFlow flow)
        {
            Flow = flow;
        }

        public IFlowPort Clone(INode node)
        {
            return new FlowPort(this, node, Direction, Name)
            {
                _callbackType = CallbackTypes.Async,
                _asyncCallback = _asyncCallback,
            };
        }

        public IEnumerator Execute()
        {
            switch (_callbackType)
            {
                case CallbackTypes.Simple:
                    _simpleCallback?.Invoke(Flow);
                    break;
                case CallbackTypes.Sync:
                    yield return _syncCallback(Flow);
                    break;
                case CallbackTypes.Async:
                    var enumerator = _asyncCallback(Flow);
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                    break;
            }
        }
    }
    
    public static class FlowPortExtensions
    {
        public static FlowPort Flip(this FlowPort self, INode node, string name)
        {
            return new FlowPort(self, node, self.Direction.Flip(), name);
        }
    }
}