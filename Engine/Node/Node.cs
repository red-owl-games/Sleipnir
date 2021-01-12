using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface INode
    {
        IGraph Graph { get; }
        string NodeId { get; }
        Rect NodeRect { get; set; }
        Vector2 NodePosition { get; set; }

        // TODO: Convert to KeyedCollection based on performance hit of using Dictionary.Values - which seems to create a ValueCollection every time?
        Dictionary<string, IValuePort> ValueInPorts { get; }
        Dictionary<string, IValuePort> ValueOutPorts { get; }
        
        void Definition(IGraph graph);
    }

    public interface IFlowNode : INode
    {
        bool IsFlowRoot { get; }
        
        Dictionary<string, IFlowPort> FlowInPorts { get; }
        Dictionary<string, IFlowPort> FlowOutPorts { get; }
        
        void Initialize(ref IFlow flow);
    }

    [Serializable]
    public abstract class Node : IFlowNode 
    {
        public IGraph Graph { get; private set; }
        
#if ODIN_INSPECTOR 
        [HideInInspector]
#endif
        [SerializeField] 
        private string nodeId = Guid.NewGuid().ToString();

        public string NodeId => nodeId;

#if ODIN_INSPECTOR 
        [HideInInspector]
#endif
        [SerializeField] 
        private Rect nodeRect;

        public Rect NodeRect
        {
            get => nodeRect;
            set => nodeRect = value;
        }

        public Vector2 NodePosition
        {
            get => nodeRect.position;
            set => nodeRect.position = value;
        }
        
        public Dictionary<string, IValuePort> ValueInPorts { get; protected set; }
        public Dictionary<string, IValuePort> ValueOutPorts { get; protected set; }
        
        public Dictionary<string, IFlowPort> FlowInPorts { get; protected set; }
        public Dictionary<string, IFlowPort> FlowOutPorts { get; protected set; }

        #region Definition
        
        public void Definition(IGraph graph)
        {
            // TODO: Increase Performance by Implementing an "IsDirty" system
            Graph = graph;
            try // TODO: can we scope this better?
            {
                DefineValuePorts();
                DefineFlowPorts();
                OnDefinition();
            }
            catch
            {
                Debug.LogWarning($"Failed to Define Node {GetType().FullName} | {NodeId}");
                throw;
            }
        }

        private void DefineValuePorts()
        {
            ValueInPorts = new Dictionary<string, IValuePort>();
            ValueOutPorts = new Dictionary<string, IValuePort>();
            if (!NodeInfo.Cache.Get(GetType(), out var data)) return;
            foreach (var valuePort in data.ValuePorts)
            {
                var port = valuePort.GetOrCreatePort(this);
                if (valuePort.GraphPort)
                {
                    var graphPort = port.Clone(Graph);
                    (valuePort.Direction == PortDirection.Input ? Graph.ValueOutPorts : Graph.ValueInPorts).Add(graphPort.Name, graphPort);
                }
                (valuePort.Direction == PortDirection.Input ? ValueInPorts : ValueOutPorts).Add(valuePort.Name, port);
                // Debug.Log($"{this} has Value Port '{valuePort.Name} | {valuePort.Direction}'");
            }
        }
        
        private void DefineFlowPorts()
        {
            FlowInPorts = new Dictionary<string, IFlowPort>();
            FlowOutPorts = new Dictionary<string, IFlowPort>();
            if (!NodeInfo.Cache.Get(GetType(), out var data)) return;
            foreach (var flowPort in data.FlowPorts)
            {
                var port = flowPort.GetOrCreatePort(this);
                if (flowPort.GraphPort)
                {
                    var graphPort = port.Clone(Graph);
                    (flowPort.Direction == PortDirection.Input ? Graph.FlowOutPorts : Graph.FlowInPorts).Add(graphPort.Name, graphPort);
                }
                (flowPort.Direction == PortDirection.Input ? FlowInPorts : FlowOutPorts).Add(flowPort.Name, port);
                // Debug.Log($"{this} has Flow Port '{flowPort.Name} | {flowPort.Direction}'");
            }
        }

        protected virtual void OnDefinition() {}

        #endregion

        #region Initialization
        
        public bool IsFlowRoot => NodeInfo.Cache.Get(GetType(), out var data) && data.IsFlowRoot;

        public void Initialize(ref IFlow flow)
        {
            try // TODO: can we scope this better?
            {
                InitializePorts(ref flow);
                OnInitialize(ref flow);
            }
            catch
            {
                Debug.LogWarning($"Failed to Initialize Node {this} | {NodeId}");
                throw;
            }
        }

        private void InitializePorts(ref IFlow flow)
        {
            foreach (var valueIn in ValueInPorts.Values)
            {
                valueIn.Initialize(ref flow);
            }

            foreach (var valueOut in ValueOutPorts.Values)
            {
                valueOut.Initialize(ref flow);
            }
            
            foreach (var flowIn in FlowInPorts.Values)
            {
                flowIn.Initialize(ref flow);
            }
            
            foreach (var flowOut in FlowOutPorts.Values)
            {
                flowOut.Initialize(ref flow);
            }
        }
        
        protected virtual void OnInitialize(ref IFlow flow) {}

        #endregion

        public override string ToString() => $"{GetType().Name}({NodeId.Substring(0,8)})";
    }
}
