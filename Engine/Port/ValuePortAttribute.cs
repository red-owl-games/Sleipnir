using System;

namespace RedOwl.Sleipnir.Engine
{
    public interface IValuePortAttribute : IPortAttribute {}

    public abstract class ValuePortAttribute : PortAttribute, IValuePortAttribute { }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValueInAttribute : ValuePortAttribute
    {
        public override PortDirection Direction => PortDirection.Input;
        
        public override PortCapacity Capacity { get; set; } = PortCapacity.Single;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValueOutAttribute : ValuePortAttribute
    {
        public override PortDirection Direction => PortDirection.Output;
        
        public override PortCapacity Capacity { get; set; } = PortCapacity.Multi;
    }

    public static class ValueAttributeExtensions
    {
        public static IValuePort GetOrCreatePort(this IValuePortAttribute self, INode node)
        {
            var port = (IValuePort)((IPortAttribute)self).GetOrCreatePort(node);
            port.Definition(node, self);
            return port;
        }
    }
}