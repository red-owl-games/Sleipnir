using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class GraphAttribute : PreserveAttribute
    {
        public static readonly TypeCache<IGraph, GraphAttribute> Cache = new TypeCache<IGraph, GraphAttribute>((Type graphType, ref GraphAttribute attribute, out Type key) =>
        {
            attribute.Initialize(graphType);
            key = graphType;
            return true;
        });

        
        public HashSet<string> Tags { get; }
        
        public RequireNodeAttribute[] RequiredNodes { get; private set; }

        public GraphAttribute(params string[] tags)
        {
            Tags = new HashSet<string>(tags);
        }
        
        private void Initialize(Type type)
        {
            RequiredNodes = type.GetCustomAttributes<RequireNodeAttribute>() as RequireNodeAttribute[];
        }
    }
}