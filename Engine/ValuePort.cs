using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    public interface IValuePort : IPort
    {
        object DefaultValue { get; }
        object WeakValue { get; }
        void Definition(INode node, ValuePortSettings settings);
        IValuePort Clone(INode node);
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

        public Type ValueType { get; protected set; }
        
        [Preserve]
        public ValuePort()
        {
            Value = default;
        }
        
        public ValuePort(T defaultValue)
        {
            Value = defaultValue;
        }
        
        public ValuePort(ValuePort<T> other, INode node, PortDirection direction, string name)
        {
            Id = new PortId(node.NodeId, name);;
            Name = name;
            Direction = direction;
            Capacity = other.Capacity;
            ValueType = typeof(T);
            Flow = other.Flow;
        }

        public void SetDefault(T defaultValue) => DefaultValue = defaultValue;
        
        public void Definition(INode node, ValuePortSettings settings)
        {
            Id = new PortId(node.NodeId, settings.Name);
            Name = settings.Name;
            Direction = settings.Direction;
            Capacity = settings.Capacity;
            ValueType = typeof(T);
        }

        public void Initialize(ref IFlow flow)
        {
            Flow = flow;
            Flow.Set(Id, (T)DefaultValue);
        }
        
        public IValuePort Clone(INode node)
        {
            return new ValuePort<T>(this, node, Direction, Name);
        }

        public static implicit operator T(ValuePort<T> self) => self.Value;
        public static implicit operator ValuePort<T>(T self) => new ValuePort<T>(self);
    }

    public static class ValuePortExtensions
    {
        public static ValuePort<T> Flip<T>(this ValuePort<T> self, INode node, string name)
        {
            return new ValuePort<T>(self, node, self.Direction.Flip(), name);
        }
    }
}