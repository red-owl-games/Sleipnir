using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public class ValuePortSettings : IPortReflectionData
    {
        public FieldInfo Info { get; }

        public string Name { get; }

        public PortDirection Direction { get; }

        public PortCapacity Capacity { get; }

        public bool ShowElement { get; }

        public ValuePortSettings(FieldInfo info, IValuePortAttribute attr)
        {
            Info = info;
            Name = attr.Name ?? info.Name;
            Direction = attr.Direction;
            Capacity = attr.Capacity;
            ShowElement = info.IsFamily;
        }

        public IValuePort GetOrCreatePort(INode node)
        {
            var port = (IValuePort)Info.GetValue(node);
            if (port == null)
            {
                port = (IValuePort)Activator.CreateInstance(Info.FieldType);
                Info.SetValue(node, port);
            }
            port.Definition(node, this);
            return port;
        }
    }
    
     public class FlowPortSettings : IPortReflectionData
     {
         private static readonly Type EnumerableType = typeof(IEnumerable);
         
         public FieldInfo Info { get; }

         public string Name { get; }

         public PortDirection Direction { get; }
         
         public PortCapacity Capacity { get; }
         
         public MethodInfo Callback { get; }
         
         public FlowPortSettings(FieldInfo fieldInfo, MethodInfo methodInfo, IFlowPortAttribute attr)
         {
             Info = fieldInfo;
             Name = attr.Name ?? fieldInfo.Name;
             Direction = attr.Direction;
             Capacity = attr.Capacity;
             Callback = methodInfo;
         }
         
         public IFlowPort GetOrCreatePort(INode node)
         {
             
             var port = (IFlowPort)Info.GetValue(node);
             if (port == null)
             {
                 // Debug.Log($"Creating Flowport: '{node}.{Name}'");
                 port = (IFlowPort)Activator.CreateInstance(Info.FieldType);
                 Info.SetValue(node, port);
             }
             // else
             // {
             //     Debug.Log($"Found Existing Flowport: '{node}.{Name}'");
             // }
             port.Definition(node, this);
             return port;
         }
     }

    public class SleipnirNodeReflection : ITypeStorage
    {
        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy;
        private static Type _nodeType = typeof(INode);
        private static readonly Vector2 DefaultSize = new Vector2(100, 300);
        
        public Type Type { get; set; }

        public IEnumerable<string> Path { get; set; }
        
        public string Name { get; set; }

        public string Help { get; set; }

        public bool Deletable { get; set; }

        public bool Moveable { get; set; }
        
        public Vector2 Size { get; set; }
        
        public bool IsFlowRoot { get; set; }
        
        public HashSet<string> Tags { get; set; }
        
        public List<ValuePortSettings> ValuePorts { get; set; }
        
        public List<FlowPortSettings> FlowPorts { get; set; }
        
        private Dictionary<string, MethodInfo> _methods;
        private Dictionary<ContextMenu, MethodInfo> _contextMethods;
        
        public IReadOnlyDictionary<ContextMenu, MethodInfo> ContextMethods => _contextMethods;

        public bool ShouldCache(Type type)
        {
            var attr = type.GetCustomAttribute<NodeAttribute>();
            
            ExtractSettings(type, attr);
            ExtractValuePorts(type);
            ExtractFlowPorts(type);
            ExtractContextMethods(type);

            return true;
        }

        private void ExtractSettings(Type type, NodeAttribute attr)
        {
            bool isNull = attr == null;
            Type = type;
            Name = isNull || string.IsNullOrEmpty(attr.Name) ? type.Name.Replace("Node", "").Replace(".", "/") : attr.Name;
            Path = isNull || string.IsNullOrEmpty(attr.Path) ? type.Namespace?.Replace(".", "/").Split('/') : attr.Path.Split('/');
            Help = isNull ? "" : attr.Tooltip;
            Tags = isNull ? new HashSet<string>() : new HashSet<string>(attr.Tags);
            Deletable = isNull || attr.Deletable;
            Moveable = isNull || attr.Moveable;
            IsFlowRoot = isNull ? false : attr.IsFlowRoot;
            Size = isNull ? DefaultSize : attr.Size;
        }

        private void ExtractValuePorts(Type type)
        {
            ValuePorts = new List<ValuePortSettings>();
            // This OrderBy sorts the fields by the order they are defined in the code with subclass fields first
            var infos = type.GetFields(BindingFlags).OrderBy(field => field.MetadataToken);
            foreach (var info in infos)
            {
                var attrs = info.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    switch (attr)
                    {
                        case ValueInAttribute input:
                            ValuePorts.Add(new ValuePortSettings(info, input));
                            break;
                        case ValueOutAttribute output:
                            ValuePorts.Add(new ValuePortSettings(info, output));
                            break;
                    }
                }
            }
        }

        private void ExtractFlowPorts(Type type)
        {
            FlowPorts = new List<FlowPortSettings>();
            // This OrderBy sorts the fields by the order they are defined in the code with subclass fields first
            var methodInfos = type.GetMethodTable(BindingFlags);
            methodInfos.Add(string.Empty, null);
            var fieldInfos = type.GetFields(BindingFlags).OrderBy(field => field.MetadataToken);
            foreach (var fieldInfo in fieldInfos)
            {
                var attrs = fieldInfo.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    if (!(attr is IFlowPortAttribute)) continue;
                    switch (attr)
                    {
                        case FlowInAttribute input:
                            FlowPorts.Add(new FlowPortSettings(fieldInfo, methodInfos[input.Callback], input));
                            break;
                        case FlowOutAttribute output:
                            FlowPorts.Add(new FlowPortSettings(fieldInfo, methodInfos[output.Callback], output));
                            break;
                    }
                }
            }
        }
        
        private void ExtractContextMethods(Type type)
        {
            _contextMethods = new Dictionary<ContextMenu, MethodInfo>();
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var contextAttr = method.GetCustomAttribute<ContextMenu>();
                if (contextAttr != null)
                {
                    _contextMethods.Add(contextAttr, method);
                }
            }
        }
    }

    public class SleipnirGraphReflection : ITypeStorage
    {
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

    public static class SleipnirGraphReflector
    {
        public static readonly TypeCache<INode, SleipnirNodeReflection> NodeCache = new TypeCache<INode, SleipnirNodeReflection>();
        
        public static readonly TypeCache<IGraph, SleipnirGraphReflection> GraphCache = new TypeCache<IGraph, SleipnirGraphReflection>();
    }
}