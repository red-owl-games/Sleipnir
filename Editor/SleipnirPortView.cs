using System;
using RedOwl.Sleipnir.Engine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using PortView = UnityEditor.Experimental.GraphView.Port;

namespace RedOwl.Sleipnir.Editor
{
    public class SleipnirPortView : PortView
    {
        public SleipnirPortView(Orientation orientation, PortDirection direction, PortCapacity capacity, Type type, IEdgeConnectorListener listener) : base(orientation, ConvertDirection(direction), ConvertCapacity(capacity), type)
        {
            m_EdgeConnector = new EdgeConnector<Edge>(listener);
            this.AddManipulator(m_EdgeConnector);
        }
        
        private static Direction ConvertDirection(PortDirection value) => value == PortDirection.Input ? Direction.Input : Direction.Output;
        private static Capacity ConvertCapacity(PortCapacity value) => value == PortCapacity.Single ? Capacity.Single : Capacity.Multi;
    }
}