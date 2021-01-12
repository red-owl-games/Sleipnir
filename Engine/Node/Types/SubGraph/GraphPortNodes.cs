using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IGraphPortNode
    {
        string Name { get; }
    }
    
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public class GraphValueInPortNode : Node, IGraphPortNode
    {
        [ValueOut(GraphPort = true)] public ValuePort<string> Out;
        
        [SerializeField, HideInInspector]
        private string name;

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
    public class GraphValueOutPortNode : Node, IGraphPortNode
    {
        [ValueIn(GraphPort = true)] public ValuePort<string> In;
        
        [SerializeField, HideInInspector]
        private string name;

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
    public class GraphFlowInPortNode : Node, IGraphPortNode
    {
        [FlowOut(GraphPort = true)] public FlowPort Out;

        [SerializeField, HideInInspector]
        private string name;

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
    public class GraphFlowOutPortNode : Node, IGraphPortNode
    {
        [FlowIn(GraphPort = true)] public FlowPort In;

        [SerializeField, HideInInspector]
        private string name;

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