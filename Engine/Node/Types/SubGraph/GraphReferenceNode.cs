using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable]
    [Node("Common", Path = "Common", Tooltip = "Nest a Graph that comes from a Graph Asset")]
    public class GraphReferenceNode : Node
    {
        [SerializeField, HideInInspector]
        private GraphAsset asset;

        [ShowInNode]
        public GraphAsset Asset
        {
            get => asset;
            set
            {
                asset = value;
                IsDefined = false;
            }
        }
        
        protected override void OnDefinition()
        {
            if (Asset != null && Asset.Graph != null)
            {
                var graph = Asset.Graph;
                graph.Definition(Asset.Graph);
                foreach (var valueIn in graph.ValueInPorts.Values)
                {
                    ValueInPorts.Add(valueIn.Name, valueIn.GraphReferencePort(this));
                }
                foreach (var valueOut in graph.ValueOutPorts.Values)
                {
                    ValueOutPorts.Add(valueOut.Name, valueOut.GraphReferencePort(this));
                }
                foreach (var flowIn in graph.FlowInPorts.Values)
                {
                    FlowInPorts.Add(flowIn.Name, flowIn.GraphReferencePort(this));
                }
                foreach (var flowOut in graph.FlowOutPorts.Values)
                {
                    FlowOutPorts.Add(flowOut.Name, flowOut.GraphReferencePort(this));
                }
            }
        }

        protected override void OnInitialize(ref IFlow flow)
        {
            if (Asset != null && Asset.Graph != null)
            {
                foreach (var node in Asset.Graph.Nodes)
                {
                    if (node is IFlowNode flowNode) flowNode.Initialize(ref flow);
                }
            }
        }
    }
}