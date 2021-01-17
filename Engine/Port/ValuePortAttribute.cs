using System;
using System.Reflection;

namespace RedOwl.Sleipnir.Engine
{
    public interface IValuePortAttribute : IPortAttribute
    {
        string Callback { get; }
        MethodInfo CallbackInfo { get; }
        void SetCallbackInfo(MethodInfo callbackInfo);
    }

    public abstract class ValuePortAttribute : PortAttribute, IValuePortAttribute
    {
        public string Callback { get; set; } = string.Empty;

        public MethodInfo CallbackInfo { get; private set; } = null;
        
        public void SetCallbackInfo(MethodInfo callbackInfo)
        {
            CallbackInfo = callbackInfo;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValueInAttribute : ValuePortAttribute
    {
        public override PortDirection Direction => PortDirection.Input;
        
        public override PortCapacity Capacity { get; set; } = PortCapacity.Single;
        
        public ValueInAttribute() {}
        public ValueInAttribute(string callback = null)
        {
            Callback = callback;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ValueOutAttribute : ValuePortAttribute
    {
        public override PortDirection Direction => PortDirection.Output;
        
        public override PortCapacity Capacity { get; set; } = PortCapacity.Multi;

        public ValueOutAttribute() {}
        public ValueOutAttribute(string callback = null)
        {
            Callback = callback;
        }
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