using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IGraphFlowPortNode
    {
        string Name { get; }
    }
    
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports", IsFlowRoot = true)]
    public class FlowInNode : Node, IGraphFlowPortNode
    {
        [FlowOut(GraphPort = true)] public FlowPort Out;

        [SerializeField, HideInInspector]
        private string name = "In";

        [ShowInNode]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                // IsDefined = false;
            }
        }
    }

    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public class FlowOutNode : Node, IGraphFlowPortNode
    {
        [FlowIn(GraphPort = true)] public FlowPort In;

        [SerializeField, HideInInspector]
        private string name = "Out";

        [ShowInNode]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                // IsDefined = false;
            }
        }
    }
}