using System;

namespace RedOwl.Sleipnir.Engine
{
    public interface IPort
    {
        PortId Id { get; }
        
        IGraph Graph { get; }
        INode Node { get; }
        
        string Name { get; }

        PortDirection Direction { get; }
         
        PortCapacity Capacity { get; }
        
        Type ValueType { get; }

        IFlow Flow { get; }
        
        void Initialize(ref IFlow flow);
    }

    public abstract class Port : IPort
    {
        public PortId Id { get; protected set; }
        public IGraph Graph => Node.Graph;
        public INode Node { get; protected set; }
        public string Name { get; protected set; }
        public PortDirection Direction { get; protected set; }
        public PortCapacity Capacity { get; protected set; }
        public Type ValueType { get; protected set; }
        public IFlow Flow { get; protected set; }
        public abstract void Initialize(ref IFlow flow);
        
        public override string ToString() => $"{Graph} {Node} {GetType().Name}[{Name} | {Direction} | {Capacity}]";
    }
}
