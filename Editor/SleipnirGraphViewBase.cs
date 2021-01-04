using System;
using System.Collections.Generic;
using RedOwl.Sleipnir.Engine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using PortView = UnityEditor.Experimental.GraphView.Port;

namespace RedOwl.Sleipnir.Editor
{
    public abstract class SleipnirGraphViewBase : GraphView
    {
        public GraphAsset GraphAsset { get; protected set; }
        public IGraph Graph => GraphAsset.graph;
        public GridBackground GridBackground { get; private set; }
        public SleipnirGraphSearchProvider SearchProvider { get; private set; }
        public MiniMap MiniMap { get; private set; }
        public IEdgeConnectorListener EdgeConnectorListener { get; }
        
        protected Dictionary<string, INodeView> _nodeViewCache;

        protected SleipnirGraphViewBase()
        {
            EdgeConnectorListener = new SleipnirGraphEdgeConnectorListener(this);
        }
        
        protected void CreateGridBackground()
        {
            GridBackground = new GridBackground {name = "Grid"};
            Insert(0, GridBackground);
        }
        
        protected void CreateMiniMap()
        {
            MiniMap = new MiniMap {anchored = true, maxWidth = 200, maxHeight = 100, visible = false};
            Add(MiniMap);
        }

        protected void CreateSearch()
        {
            SearchProvider = ScriptableObject.CreateInstance<SleipnirGraphSearchProvider>();
            SearchProvider.Initialize(this);
            nodeCreationRequest = ctx => SearchWindow.Open(new SearchWindowContext(ctx.screenMousePosition), SearchProvider);
        }
        
        public void CreateNode(SleipnirNodeInfo data, Vector2 position)
        {
            Undo.RecordObject(GraphAsset, "Create Node");
            var node = (INode)Activator.CreateInstance(data.Type);
            Graph.Add(node); // Definition & Initialize are called here
            var window = EditorWindow.GetWindow<SleipnirWindow>();
            node.NodePosition = window.rootVisualElement.ChangeCoordinatesTo(contentViewContainer, position - window.position.position - new Vector2(3, 26));
            CreateNodeView(node);
            Save();
        }

        public void OpenSearch(Vector2 screenPosition, PortView port = null)
        {
            //_search.SourcePort = connectedPort;
            SearchWindow.Open(new SearchWindowContext(screenPosition), SearchProvider);
        }
        
        #region API

        protected abstract void CreateNodeView(INode node);
        public abstract void Save();

        #endregion
    }
        
    public abstract class SleipnirGraphViewBase<TNodeView> : SleipnirGraphViewBase where TNodeView : SleipnirNodeViewBase, new()
    {
        protected override void CreateNodeView(INode node)
        {
            var info = SleipnirGraphReflector.NodeCache.Get(node.GetType());
            GraphElement element = info.EditorNodeView != null ? (GraphElement)Activator.CreateInstance(info.EditorNodeView) : new TNodeView();
            if (element is IEditorNodeView editorView) editorView.EdgeListener = EdgeConnectorListener;
            if (element is INodeView nodeView)
            {
                nodeView.Initialize(node, info);
                _nodeViewCache.Add(node.NodeId, nodeView);
            }
            AddElement(element);
        }
    }
}