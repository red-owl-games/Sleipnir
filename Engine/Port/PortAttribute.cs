using System;
using System.Reflection;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    public interface IPortAttribute
    {
        FieldInfo Info { get; }
        string Name { get; }
        PortDirection Direction { get; }
        PortCapacity Capacity { get; }
        bool GraphPort { get; }
        void SetInfo(FieldInfo info);
    }

    public abstract class PortAttribute : PreserveAttribute, IPortAttribute
    {
        public FieldInfo Info { get; private set; }
        public string Name { get; set; } = null;
        public abstract PortDirection Direction { get; }
        public PortCapacity Capacity { get; set; } = PortCapacity.Multi;
        public bool GraphPort { get; set; } = false;
        
        public void SetInfo(FieldInfo info)
        {
            Info = info;
            Name = Name ?? info.Name;
        }
    }
    
    public static class PortAttributeExtensions
    {
        public static IPort GetOrCreatePort(this IPortAttribute self, INode node)
        {
            var port = (IPort)self.Info.GetValue(node);
            if (port == null)
            {
                port = (IPort)Activator.CreateInstance(self.Info.FieldType);
                self.Info.SetValue(node, port);
            }
            return port;
        }
    }
}