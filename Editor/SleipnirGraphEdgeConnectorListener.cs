using System.Collections.Generic;
using RedOwl.Sleipnir.Engine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using PortView = UnityEditor.Experimental.GraphView.Port;

namespace RedOwl.Sleipnir.Editor
{
    /// <summary>
    /// Custom connector listener so that we can link up nodes and 
    /// open a search box when the user drops an edge into the canvas
    /// </summary>
    public class SleipnirGraphEdgeConnectorListener : IEdgeConnectorListener
    {
        private SleipnirGraphViewBase view;
        
        private GraphViewChange _graphViewChange;
        private List<Edge> _edgesToCreate;
        private List<GraphElement> _edgesToDelete;
    
        public SleipnirGraphEdgeConnectorListener(SleipnirGraphViewBase view)
        {
            this.view = view;
            _edgesToCreate = new List<Edge>();
            _edgesToDelete = new List<GraphElement>();
            _graphViewChange.edgesToCreate = _edgesToCreate;
        }

        /// <summary>
        /// Handle connecting nodes when an edge is dropped between two ports
        /// </summary>
        public void OnDrop(GraphView graphView, Edge edge)
        {
            _edgesToCreate.Clear();
            _edgesToCreate.Add(edge);
            _edgesToDelete.Clear();
            if (edge.input.capacity == PortView.Capacity.Single)
            {
                foreach (Edge connection in edge.input.connections)
                {
                    if (connection != edge)
                        _edgesToDelete.Add(connection);
                }
            }
            if (edge.output.capacity == PortView.Capacity.Single)
            {
                foreach (Edge connection in edge.output.connections)
                {
                    if (connection != edge)
                        _edgesToDelete.Add(connection);
                }
            }
            if (_edgesToDelete.Count > 0) graphView.DeleteElements(_edgesToDelete);
            List<Edge> edgesToCreate = _edgesToCreate;
            if (graphView.graphViewChanged != null) edgesToCreate = graphView.graphViewChanged(_graphViewChange).edgesToCreate;
            foreach (Edge edge1 in edgesToCreate)
            {
                graphView.AddElement(edge1);
                edge.input.Connect(edge1);
                edge.output.Connect(edge1);
            }
        }

        /// <summary>
        /// Activate the search dialog when an edge is dropped on an arbitrary location
        /// </summary>
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            var screenPosition = GUIUtility.GUIToScreenPoint(
                Event.current.mousePosition
            );
            
            if (edge.output != null)
            {
                view.OpenSearch(
                    screenPosition, 
                    edge.output.edgeConnector.edgeDragHelper.draggedPort as PortView
                );
            }
            else if (edge.input != null)
            {
                view.OpenSearch(
                    screenPosition, 
                    edge.input.edgeConnector.edgeDragHelper.draggedPort as PortView
                );
            }
        }
    }
}