using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    public interface IValuePort : IPort
    {
        object DefaultValue { get; }
        object WeakValue { get; }
        void Definition(INode node, IValuePortAttribute info);
        IValuePort GraphPort(IGraph graph);
        IValuePort GraphReferencePort(GraphReferenceNode node);
    }

    [Preserve]
    public class ValuePort<T> : Port, IValuePort
    {
        public object DefaultValue { get; private set; }
        public object WeakValue => Flow != null && Flow.ContainsKey(Id) ? Flow.Get<object>(Id) : DefaultValue;
        
        public T Value
        {
            get => Flow != null && Flow.ContainsKey(Id) ? Flow.Get<T>(Id) : (T) DefaultValue;
            set
            {
                if (Flow == null)
                {
                    DefaultValue = value;
                }
                else
                {
                    Flow.Set(Id, value);
                }
            }
        }
        
        [Preserve]
        public ValuePort()
        {
            Value = default;
        }
        
        public ValuePort(T defaultValue)
        {
            Value = defaultValue;
        }

        public void SetDefault(T defaultValue) => DefaultValue = defaultValue;
        
        public void Definition(INode node, IValuePortAttribute info)
        {
            Id = new PortId(node.NodeId, info.Name);
            Node = node;
            Name = info.Name;
            Direction = info.Direction;
            Capacity = info.Capacity;
            ValueType = typeof(T);
        }
        
        public IValuePort GraphPort(IGraph graph)
        {
            var newName = $"{Name} {Direction.Flip()}";
            var port = new ValuePort<T>()
            {
                Id = new PortId(graph.NodeId, newName),
                Node = graph,
                Name = newName,
                Direction = Direction.Flip(),
                Capacity = Capacity,
                Flow = Flow,
            };

            return port;
        }

        public IValuePort GraphReferencePort(GraphReferenceNode node)
        {
            Id = new PortId(node.NodeId, Name);
            return this;
        }

        public override void Initialize(ref IFlow flow)
        {
            Flow = flow;
            Flow.Set(Id, (T)DefaultValue);
        }

        public static implicit operator T(ValuePort<T> self) => self.Value;
        public static implicit operator ValuePort<T>(T self) => new ValuePort<T>(self);
    }
    
    // TODO: Create GraphValuePort?
    
    /*
    public ValuePort(ValuePort<T> other, INode node, PortDirection direction, string name)
    {
        Id = new PortId(node.NodeId, name);;
        Name = name;
        Direction = direction;
        Capacity = other.Capacity;
        ValueType = typeof(T);
        Flow = other.Flow;
    }
     
    public IValuePort Clone(INode node)
    {
        return new ValuePort<T>(this, node, Direction, Name);
    }
     

    public static class ValuePortExtensions
    {
        public static ValuePort<T> Flip<T>(this ValuePort<T> self, INode node, string name)
        {
            return new ValuePort<T>(self, node, self.Direction.Flip(), name);
        }
    }
    
    */
}