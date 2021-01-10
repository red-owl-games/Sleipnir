using System;
using System.Reflection;

namespace RedOwl.Sleipnir.Engine
{
    public interface IFlowPortAttribute : IPortAttribute
    {
        string Callback { get; }
        MethodInfo CallbackInfo { get; }
        void SetCallbackInfo(MethodInfo callbackInfo);
    }

    public abstract class FlowPortAttribute : PortAttribute, IFlowPortAttribute
    {
        public string Callback { get; set; } = string.Empty;

        public MethodInfo CallbackInfo { get; private set; } = null;
        
        public void SetCallbackInfo(MethodInfo callbackInfo)
        {
            CallbackInfo = callbackInfo;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FlowInAttribute : FlowPortAttribute
    {
        public override PortDirection Direction => PortDirection.Input;

        public FlowInAttribute() {}
        public FlowInAttribute(string callback = null)
        {
            Callback = callback;
        }


    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FlowOutAttribute : FlowPortAttribute
    {
        public override PortDirection Direction => PortDirection.Output;

        public FlowOutAttribute() {}
        public FlowOutAttribute(string callback = null)
        {
            Callback = callback;
        }
    }

    public static class FlowAttributeExtensions
    {
        public static IFlowPort GetOrCreatePort(this IFlowPortAttribute self, INode node)
        {
            var port = (IFlowPort)((IPortAttribute)self).GetOrCreatePort(node);
            port.Definition(node, self);
            return port;
        }
    }
}