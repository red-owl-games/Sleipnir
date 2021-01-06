using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface IPortReflectionData {}

    public enum PortDirection
    {
        Input,
        Output,
    }

    public static class PortDirectionExtensions
    {
        public static PortDirection Flip(this PortDirection self)
        {
            return self == PortDirection.Input ? PortDirection.Output : PortDirection.Input;
        }
    }

    public enum PortCapacity
    {
        Single,
        Multi,
    }
    
    public interface IPort
    {
        PortId Id { get; }
        
        IGraph Graph { get; }
        
        string Name { get; }

        PortDirection Direction { get; }
         
        PortCapacity Capacity { get; }
        
        Type ValueType { get; }
        
        void Initialize(ref IFlow flow);
    }

    [Serializable]
    public struct PortId
    {
        [field: SerializeField]
        public string Node { get; private set; }

        [field: SerializeField]
        public string Port { get; private set; }

        public PortId(string node, string port)
        {
            Node = node;
            Port = port;
        }

        public override string ToString() => $"{Node}.{Port}";
    }

    public abstract class Port
    {
        public PortId Id { get; protected set; }
        public IGraph Graph { get; protected set; }
        public string Name { get; protected set; }
        public PortDirection Direction { get; protected set; }
        public PortCapacity Capacity { get; protected set; }
        
        public IFlow Flow { get; protected set; }
        
        public override string ToString() => $"{GetType().Name}[{Name} | {Direction} | {Capacity}]";
    }
}
