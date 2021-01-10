using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RedOwl.Sleipnir.Engine
{
    public class GraphInfo : ITypeStorage
    {
        public static readonly TypeCache<IGraph, GraphInfo> Cache = new TypeCache<IGraph, GraphInfo>();
        
        public HashSet<string> Tags { get; set; }
        
        public RequireNodeAttribute[] RequiredNodes { get; set; }
        
        public bool ShouldCache(Type type)
        {
            var attr = type.GetCustomAttribute<GraphAttribute>();
            
            ExtractSettings(type, attr);

            RequiredNodes = type.GetCustomAttributes<RequireNodeAttribute>().ToArray();

            return true;
        }
        
        private void ExtractSettings(Type type, GraphAttribute attr)
        {
            bool isNull = attr == null;
            Tags = isNull ? new HashSet<string>() : new HashSet<string>(attr.Tags);
        }
    }
}