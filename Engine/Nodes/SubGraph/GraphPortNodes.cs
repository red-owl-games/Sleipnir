using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine.Nodes.SubGraph
{
    public interface IGraphPort
    {
        string Name { get; }
        
    }

    public interface IGraphValuePort : IGraphPort
    {
        IValuePort Port { get; }
    }

    public interface IGraphFlowPort : IGraphPort
    {
        IFlowPort Port { get; }
    }
    
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public class GraphValueInPortNode : Node, IGraphValuePort
    {
        [ValueOut] public ValuePort<string> Out;

        public IValuePort Port => Out.Flip(Graph, name);

        [SerializeField, HideInInspector]
        private string name;

        [ShowInNode]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                IsDefined = false;
            }
        }
    }
    
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public class GraphValueOutPortNode : Node, IGraphValuePort
    {
        [ValueIn] public ValuePort<string> In;

        public IValuePort Port => In.Flip(Graph, name);

        [SerializeField, HideInInspector]
        private string name;

        [ShowInNode]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                IsDefined = false;
            }
        }
    }
    
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public class GraphFlowInPortNode : Node, IGraphFlowPort
    {
        [FlowOut] public FlowPort Out;

        public IFlowPort Port
        {
            get
            {
                var port = Out.Flip(Graph, name);
                port.Link(Out);
                return port;
            }
        }

        [SerializeField, HideInInspector]
        private string name;

        [ShowInNode]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                IsDefined = false;
            }
        }
    }
    
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public class GraphFlowOutPortNode : Node, IGraphFlowPort
    {
        [FlowIn] public FlowPort In;

        public IFlowPort Port
        {
            get
            {
                var port = In.Flip(Graph, name);
                In.Link(port);
                return port;
            }
        }

        [SerializeField, HideInInspector]
        private string name;

        [ShowInNode]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                IsDefined = false;
            }
        }
    }
}