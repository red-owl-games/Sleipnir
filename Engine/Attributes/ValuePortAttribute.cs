using System;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    public interface IValuePortAttribute
    {
        string Name { get; }
        PortDirection Direction { get; }
        PortCapacity Capacity { get; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValueInAttribute : PreserveAttribute, IValuePortAttribute
    {
        public string Name { get; set; } = null;
        public PortDirection Direction => PortDirection.Input;
        public PortCapacity Capacity { get; set; } = PortCapacity.Multi;
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValueOutAttribute : PreserveAttribute, IValuePortAttribute
    {
        public string Name { get; set; } = null;
        public PortDirection Direction => PortDirection.Output;
        public PortCapacity Capacity { get; set; } = PortCapacity.Multi;
    }
}