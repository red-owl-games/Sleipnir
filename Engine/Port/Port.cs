using System;

namespace RedOwl.Sleipnir.Engine
{
    public interface IPort
    {
        PortId Id { get; set; }
        
        IGraph Graph { get; }
        INode Node { get; }
        
        string Name { get; }

        PortDirection Direction { get; }
         
        PortCapacity Capacity { get; }
        bool GraphPort { get; }
        
        Type ValueType { get; }

        IFlow Flow { get; }
        
        void Initialize(ref IFlow flow);
    }

    public abstract class Port : IPort
    {
        public PortId Id { get; set; }
        public IGraph Graph => Node.Graph;
        public INode Node { get; internal set; }
        public string Name { get; internal set; }
        public PortDirection Direction { get; protected set; }
        public PortCapacity Capacity { get; protected set; }
        public bool GraphPort { get; protected set; }
        public Type ValueType { get; protected set; }
        public IFlow Flow { get; protected set; }
        public abstract void Initialize(ref IFlow flow);
        
        public override string ToString() => $"{Node} {GetType().Name}[{Name} | {Direction} | {Capacity}]";
    }
}
