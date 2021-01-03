#if ODIN_INSPECTOR
using System.Collections.Generic;
using RedOwl.Sleipnir.Engine;
using Sirenix.OdinInspector.Editor;

namespace RedOwl.Sleipnir.Editor
{
    public class GraphNodePropertyProcessor<T> : OdinPropertyProcessor<T> where T : INode
    {
        public override void ProcessMemberProperties(List<InspectorPropertyInfo> propertyInfos)
        {
            var match = typeof(Port);
            for (int i = propertyInfos.Count - 1; i >= 0; i--)
            {
                var info = propertyInfos[i];
                if (match.IsAssignableFrom(info.TypeOfValue)) propertyInfos.RemoveAt(i);
            }
        }
    }
}

#endif