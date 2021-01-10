using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public class GraphValueInPortNode : Node
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
                IsDefined = false;
            }
        }
        
        protected override void OnDefinition()
        {
            //Out.Name = name;
        }
    }
    
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public class GraphValueOutPortNode : Node
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
                IsDefined = false;
            }
        }
        
        protected override void OnDefinition()
        {
            //In.Name = name;
        }
    }
    
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public class GraphFlowInPortNode : Node
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
                IsDefined = false;
            }
        }
        
        protected override void OnDefinition()
        {
            //Out.Name = name;
        }
    }
    
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public class GraphFlowOutPortNode : Node
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
                IsDefined = false;
            }
        }

        protected override void OnDefinition()
        {
            //In.Name = name;
        }
    }
}