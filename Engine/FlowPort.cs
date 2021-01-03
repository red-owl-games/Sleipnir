using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    public interface IFlowPort : IPort
    {
        void Definition(INode node, FlowPortSettings settings);
        IEnumerator Execute(IFlow flow);
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

        private bool _hasCallback;
        private Action<IFlow> _simpleCallback;
        private Func<IFlow, IFlowPort> _syncCallback;
        private Func<IFlow, IEnumerator> _asyncCallback;
        
        [Preserve]
        public FlowPort() {}

        public void Definition(INode node, FlowPortSettings settings)
        {
            Id = new PortId(node.NodeId, settings.Name);
            Name = settings.Name;
            Direction = settings.Direction;
            Capacity = settings.Capacity;
            if (settings.Callback == null) return;
            var parameters = settings.Callback.GetParameters();
            if (parameters.Length != 1)
            {
                Debug.LogWarning($"FlowPort Callback for '{node.NodeTitle}.{settings.Callback.Name}' has {parameters.Length} parameter(s).  Can only accept 1 parameter of type 'IFlow'");
                return;
            }

            var paramType = parameters[0].ParameterType;
            if (paramType != FlowType)
            {
                Debug.LogWarning($"FlowPort Callback for '{node.NodeTitle}.{settings.Callback.Name}' has 1 parameter that takes type '{paramType}'.  Can only accept 1 parameter of type 'IFlow'");
                return;
            }
            _hasCallback = false;
            if (VoidType.IsAssignableFrom(settings.Callback.ReturnType))
            {
                _simpleCallback = (Action<IFlow>)settings.Callback.CreateDelegate(SimpleCallbackType, node);
                _hasCallback = true;
            }
            if (FlowPortType.IsAssignableFrom(settings.Callback.ReturnType))
            {
                _syncCallback = (Func<IFlow, IFlowPort>)settings.Callback.CreateDelegate(SyncCallbackType, node);
                _hasCallback = true;
            }
            if (EnumeratorType.IsAssignableFrom(settings.Callback.ReturnType))
            {
                _asyncCallback = (Func<IFlow, IEnumerator>)settings.Callback.CreateDelegate(AsyncCallbackType, node);
                _hasCallback = true;
            }
            if (!_hasCallback) Debug.LogWarning($"FlowPort Callback for '{node.NodeTitle}.{settings.Callback.Name}' did not have one of the following method signatures [Action<IFlow>, Func<IFlow, IFlowPort>, Func<IFlow, IEnumerator>]");
            // TODO: Log about bad callback setup?
        }

        public IEnumerator Execute(IFlow flow)
        {
            if (_hasCallback)
            {
                _simpleCallback?.Invoke(flow);

                if (_syncCallback != null)
                {
                    yield return _syncCallback(flow);
                }

                if (_asyncCallback != null)
                {
                    var enumerator = _asyncCallback(flow);
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
            }
        }
    }
}