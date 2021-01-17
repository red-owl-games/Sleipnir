using System;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NodeViewAttribute : PreserveAttribute
    {
        // These cache the INodeType => NodeViewAttribute based on if the attribute is on a custom INodeView class so that we can lookup the View by INodeType
        public static readonly TypeCache<INodeView, NodeViewAttribute> EditorViews = new TypeCache<INodeView, NodeViewAttribute>(EditorPredicate);
        public static readonly TypeCache<INodeView, NodeViewAttribute> RuntimeViews = new TypeCache<INodeView, NodeViewAttribute>(RuntimePredicate);

        public Type NodeType { get; }
        
        public bool IsRuntimeView { get; set; }
        
        public Type ViewType { get; private set; }

        public NodeViewAttribute(Type nodeType, bool isRuntimeView = false)
        {
            NodeType = nodeType;
            IsRuntimeView = isRuntimeView;
        }

        private static bool EditorPredicate(Type type, ref NodeViewAttribute attribute, out Type key)
        {
            attribute.ViewType = type;
            key = attribute.NodeType;
            return true;
        }
        
        private static bool RuntimePredicate(Type type, ref NodeViewAttribute attribute, out Type key)
        {
            attribute.ViewType = type;
            key = attribute.IsRuntimeView ? attribute.NodeType : null;
            return key != null;
        }
    }
}