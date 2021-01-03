using System;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    public interface IFlowPortAttribute
    {
        string Name { get; }
        PortDirection Direction { get; }
        PortCapacity Capacity { get; }
        string Callback { get; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FlowInAttribute : PreserveAttribute, IFlowPortAttribute
    {
        public string Name { get; set; } = null;
        public PortDirection Direction => PortDirection.Input;
        public PortCapacity Capacity { get; set; } = PortCapacity.Multi;
        
        public string Callback { get; set; } = string.Empty;

        public FlowInAttribute() {}
        public FlowInAttribute(string callback = null)
        {
            Callback = callback;
        }

    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FlowOutAttribute : PreserveAttribute, IFlowPortAttribute
    {
        public string Name { get; set; } = null;
        public PortDirection Direction => PortDirection.Output;
        public PortCapacity Capacity { get; set; } = PortCapacity.Multi;
        
        public string Callback { get; set; } = string.Empty;
        
        public FlowOutAttribute() {}
        public FlowOutAttribute(string callback = null)
        {
            Callback = callback;
        }
    }
}