#if ODIN_INSPECTOR
using System.Collections.Generic;
using RedOwl.Sleipnir.Engine;
using Sirenix.OdinInspector.Editor;

namespace RedOwl.Sleipnir.Editor
{
    public class UIXGraphNodePropertyProcessor<T> : OdinPropertyProcessor<T> where T : INode
    {
        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            var match = typeof(Port);
            for (int i = propertyInfos.Count - 1; i >= 0; i--)
            {
                var info = propertyInfos[i];
                // TODO: is this better - so we don't have to flag things with attributes?
                if (match.IsAssignableFrom(info.TypeOfValue)) propertyInfos.RemoveAt(i);
                // if (info.GetAttribute<FlowInAttribute>() != null) propertyInfos.RemoveAt(i);
                // if (info.GetAttribute<FlowOutAttribute>() != null) propertyInfos.RemoveAt(i);
                // if (info.GetAttribute<ValueInAttribute>() != null) propertyInfos.RemoveAt(i);
                // if (info.GetAttribute<ValueOutAttribute>() != null) propertyInfos.RemoveAt(i);
            }
        }
    }
}

#endif