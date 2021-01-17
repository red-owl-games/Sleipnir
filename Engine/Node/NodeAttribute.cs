using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class NodeAttribute : PreserveAttribute
    {
        public static readonly TypeCache<INode, NodeAttribute> Cache = new TypeCache<INode, NodeAttribute>((Type nodeType, ref NodeAttribute attribute, out Type key) =>
        {
            attribute.Initialize(nodeType);
            key = nodeType;
            return true;
        });

        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy;

        #region User

        /// <summary>
        /// Display name of the node.
        ///
        /// If not supplied, this will be inferred based on the class name.
        /// </summary>
        public string Name { get; set; }
        
        public string Tooltip { get; set; }

        /// <summary>
        /// Slash-delimited path to categorize this node in the search window.
        /// </summary>
        public string Path { get; set; }
        
        public HashSet<string> Tags { get; }
        
        public bool Deletable { get; set; } = true;
        
        public bool Moveable { get; set; } = true;
        
        public bool IsFlowRoot { get; set; } = false;

        public float MinWidth { get; set; } = 50;
        
        public float MinHeight { get; set; } = 10;
        
        public Vector2 MinSize => new Vector2(MinWidth, MinHeight);

        #endregion

        #region Internal

        public Type Type { get; private set; }
        public List<IValuePortAttribute> ValuePorts { get; private set; }
        
        public List<IFlowPortAttribute> FlowPorts { get; private set; }
        
        public Type EditorView { get; private set; }
        public Type RuntimeView { get; private set; }

        #endregion

        public NodeAttribute(params string[] tags)
        {
            Tags = new HashSet<string>(tags);
        }

        #region Initialization

        private void Initialize(Type type)
        {
            Type = type;
            
            var methodTable = Type.GetMethodTable(BindingFlags);
            methodTable.Add(string.Empty, null);
            
            ExtractSettings();
            ExtractValuePorts(methodTable);
            ExtractFlowPorts(methodTable);
            ExtractViews();
        }

        private void ExtractSettings()
        {
            Name = string.IsNullOrEmpty(Name) ? Type.Name.Replace("Node", "").Replace(".", "/") : Name;
            Path = string.IsNullOrEmpty(Path) ? Type.Namespace?.Replace(".", "/") : Path;

        }
        
        private void ExtractValuePorts(Dictionary<string, MethodInfo> methodTable)
        {
            ValuePorts = new List<IValuePortAttribute>();
            // This OrderBy sorts the fields by the order they are defined in the code with subclass fields first
            foreach (var info in Type.GetFields(BindingFlags).OrderBy(field => field.MetadataToken))
            {
                foreach (var attribute in info.GetCustomAttributes<ValuePortAttribute>(true))
                {
                    attribute.SetInfo(info);
                    attribute.SetCallbackInfo(methodTable[attribute.Callback]);
                    // Debug.Log($"Extracting Value Port '{attribute.Name} {attribute.Direction}'");
                    ValuePorts.Add(attribute);
                }
            }
        }

        private void ExtractFlowPorts(Dictionary<string, MethodInfo> methodTable)
        {
            FlowPorts = new List<IFlowPortAttribute>();
            // This OrderBy sorts the fields by the order they are defined in the code with subclass fields first
            foreach (var fieldInfo in Type.GetFields(BindingFlags).OrderBy(field => field.MetadataToken))
            {
                foreach (var attribute in fieldInfo.GetCustomAttributes<FlowPortAttribute>(true))
                {
                    attribute.SetInfo(fieldInfo);
                    attribute.SetCallbackInfo(methodTable[attribute.Callback]);
                    // Debug.Log($"Extracting Flow Port '{attribute.Name} {attribute.Direction}'");
                    FlowPorts.Add(attribute);
                }
            }
        }

        private void ExtractViews()
        {
            EditorView = NodeViewAttribute.EditorViews.TryGet(Type, out var editor) ? editor.ViewType : null;
            RuntimeView = NodeViewAttribute.RuntimeViews.TryGet(Type, out var runtime) ? runtime.ViewType : null;
        }
        
        // TODO: Allow for nodes to add ContextMenu items when right clicking on a node
        /*
        private Dictionary<ContextMenu, MethodInfo> _contextMethods;
        
        public IReadOnlyDictionary<ContextMenu, MethodInfo> ContextMethods => _contextMethods;

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
        */

        #endregion
    }
}