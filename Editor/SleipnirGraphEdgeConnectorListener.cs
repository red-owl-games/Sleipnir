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
    
        public SleipnirGraphEdgeConnectorListener(SleipnirGraphViewBase view)
        {
            this.view = view;
        }

        /// <summary>
        /// Handle connecting nodes when an edge is dropped between two ports
        /// </summary>
        public void OnDrop(GraphView graphView, Edge edge)
        {
            view.AddElement(edge);
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