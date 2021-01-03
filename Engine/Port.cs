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

    public enum PortCapacity
    {
        Single,
        Multi,
    }
    
    public interface IPort
    {
        PortId Id { get; }
        
        string Name { get; }

        PortDirection Direction { get; }
         
        PortCapacity Capacity { get; }
        
        Type ValueType { get; }
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
        public PortId Id { get; internal set; }
        public string Name { get; protected set; }
        public PortDirection Direction { get; protected set; }
        public PortCapacity Capacity { get; protected set; }
        
        public override string ToString() => $"[{Name} | {Direction} | {Capacity}]";
    }
}
