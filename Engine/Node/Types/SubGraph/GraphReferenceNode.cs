using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable]
    [Node("Common", Path = "Common", Tooltip = "Nest a Graph that comes from a Graph Asset")]
    public class GraphReferenceNode : INode
    {
        [SerializeField, HideInInspector]
        private GraphAsset asset;

        [ShowInNode]
        public GraphAsset Asset
        {
            get
            {
                if (asset == null)
                {
                    // TODO: Need to Trigger Redraw of NodeView
                    asset = ScriptableObject.CreateInstance<GraphAsset>();
                    asset.Graph = new Graph();
                }
                return asset;
            }
            set
            {
                if (asset.Graph == null) asset.Graph = new Graph();
                asset = value;
                // TODO: Need to Trigger Redraw of NodeView to get updated Ports
            }
        }

        #region INode

        public IGraph Graph => Asset.Graph.Graph;
        public string NodeId => Asset.Graph.NodeId;
        public bool IsFlowRoot => false;
        public Rect NodeRect
        {
            get => Asset.Graph.NodeRect;
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(Asset);
#endif
                Asset.Graph.NodeRect = value;
            }
        }

        public Vector2 NodePosition
        {
            get => Asset.Graph.NodePosition;
            set
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(Asset);
#endif
                Asset.Graph.NodePosition = value;
            }
        }

        public Dictionary<string, IValuePort> ValueInPorts => Asset.Graph.ValueInPorts;
        public Dictionary<string, IValuePort> ValueOutPorts => Asset.Graph.ValueOutPorts;
        public Dictionary<string, IFlowPort> FlowInPorts => Asset.Graph.FlowInPorts;
        public Dictionary<string, IFlowPort> FlowOutPorts => Asset.Graph.FlowOutPorts;
        public void Definition(IGraph graph) => Asset.Graph.Definition(graph);
        public void MarkDirty() => Asset.Graph.MarkDirty();
        public void Initialize(ref IFlow flow) => Asset.Graph.Initialize(ref flow);

        #endregion

    }
}