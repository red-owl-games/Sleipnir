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
        void Initialize(ref IFlow flow);
    }

    [Preserve]
    public class ValuePort<T> : Port, IValuePort
    {
        private IFlow currentFlow;
        public object DefaultValue { get; private set; }
        public object WeakValue => currentFlow != null && currentFlow.ContainsKey(Id) ? currentFlow.Get<object>(Id) : DefaultValue;
        
        public T Value
        {
            get => currentFlow != null && currentFlow.ContainsKey(Id) ? currentFlow.Get<T>(Id) : (T) DefaultValue;
            set
            {
                if (currentFlow == null)
                {
                    DefaultValue = value;
                }
                else
                {
                    currentFlow.Set(Id, value);
                }
            }
        }

        public Type ValueType { get; protected set; }
        
        public ValuePort(T defaultValue)
        {
            Value = defaultValue;
        }
        
        [Preserve]
        public ValuePort() : this(default) {}

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
            currentFlow = flow;
            currentFlow.Set(Id, (T)DefaultValue);
        }

        public static implicit operator T(ValuePort<T> self) => self.Value;
        public static implicit operator ValuePort<T>(T self) => new ValuePort<T>(self);
    }
}