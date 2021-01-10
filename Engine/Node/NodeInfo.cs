using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public class NodeInfo : ITypeStorage
    {
        public static readonly TypeCache<INode, NodeInfo> Cache = new TypeCache<INode, NodeInfo>();
        
        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy;
        
        public Type Type { get; set; }

        public IEnumerable<string> Path { get; set; }
        
        public string Name { get; set; }

        public string Help { get; set; }

        public bool Deletable { get; set; }

        public bool Moveable { get; set; }
        
        public Vector2 Size { get; set; }
        
        public bool IsFlowRoot { get; set; }
        
        public HashSet<string> Tags { get; set; }
        
        public List<IValuePortAttribute> ValuePorts { get; set; }
        
        public List<IFlowPortAttribute> FlowPorts { get; set; }
        
        public Type EditorNodeView { get; private set; }
        public Type RuntimeNodeView { get; private set; }
        
        private Dictionary<ContextMenu, MethodInfo> _contextMethods;
        
        public IReadOnlyDictionary<ContextMenu, MethodInfo> ContextMethods => _contextMethods;

        public bool ShouldCache(Type type)
        {
            var attr = type.GetCustomAttribute<NodeAttribute>();
            if (attr == null) return false;
            Type = type;
            
            // Debug.Log($"Caching Node '{Type}'");
            
            ExtractSettings(attr);
            ExtractValuePorts();
            ExtractFlowPorts();
            ExtractContextMethods();
            ExtractViews();

            return true;
        }

        private void ExtractSettings(NodeAttribute attr)
        {
            Name = string.IsNullOrEmpty(attr.Name) ? Type.Name.Replace("Node", "").Replace(".", "/") : attr.Name;
            Path = string.IsNullOrEmpty(attr.Path) ? Type.Namespace?.Replace(".", "/").Split('/') : attr.Path.Split('/');
            Help = attr.Tooltip;
            Tags = new HashSet<string>(attr.Tags);
            Deletable = attr.Deletable;
            Moveable = attr.Moveable;
            IsFlowRoot = attr.IsFlowRoot;
            Size = attr.Size;
        }

        private void ExtractValuePorts()
        {
            ValuePorts = new List<IValuePortAttribute>();
            // This OrderBy sorts the fields by the order they are defined in the code with subclass fields first
            foreach (var info in Type.GetFields(BindingFlags).OrderBy(field => field.MetadataToken))
            {
                foreach (var attribute in info.GetCustomAttributes<ValuePortAttribute>(true))
                {
                    attribute.SetInfo(info);
                    // Debug.Log($"Extracting Value Port '{attribute.Name} {attribute.Direction}'");
                    ValuePorts.Add(attribute);
                }
            }
        }

        private void ExtractFlowPorts()
        {
            FlowPorts = new List<IFlowPortAttribute>();
            // This OrderBy sorts the fields by the order they are defined in the code with subclass fields first
            var methodInfos = Type.GetMethodTable(BindingFlags);
            methodInfos.Add(string.Empty, null);
            foreach (var fieldInfo in Type.GetFields(BindingFlags).OrderBy(field => field.MetadataToken))
            {
                foreach (var attribute in fieldInfo.GetCustomAttributes<FlowPortAttribute>(true))
                {
                    attribute.SetInfo(fieldInfo);
                    // Debug.Log($"Extracting Flow Port '{attribute.Name} {attribute.Direction}'");
                    attribute.SetCallbackInfo(methodInfos[attribute.Callback]);
                    FlowPorts.Add(attribute);
                }
            }
        }
        
        private void ExtractContextMethods()
        {
            _contextMethods = new Dictionary<ContextMenu, MethodInfo>();
            foreach (var method in Type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var contextAttr = method.GetCustomAttribute<ContextMenu>();
                if (contextAttr != null)
                {
                    _contextMethods.Add(contextAttr, method);
                }
            }
        }

        private static Dictionary<Type, Type> _editorNodeViews;
        private static Dictionary<Type, Type> _runtimeNodeViews;
        
        private void CacheViewClasses()
        {
            if (_editorNodeViews != null && _runtimeNodeViews != null) return;
            _editorNodeViews = new Dictionary<Type, Type>(50);
            _runtimeNodeViews = new Dictionary<Type, Type>(50);
            foreach (var type in TypeExtensions.GetAllTypes<INodeView>())
            {
                foreach (var attr in type.GetCustomAttributes<NodeViewAttribute>(false))
                {
                    _editorNodeViews.Add(attr.NodeType, type);
                    if (attr.IsRuntimeView)
                    {
                        _runtimeNodeViews.Add(attr.NodeType, type);
                    }
                }
            }
        }

        private void ExtractViews()
        {
            CacheViewClasses();
            EditorNodeView = _editorNodeViews.TryGetValue(Type, out var editorType) ? editorType : null;
            RuntimeNodeView = _editorNodeViews.TryGetValue(Type, out var runtimeType) ? runtimeType : null;
        }
    }
}