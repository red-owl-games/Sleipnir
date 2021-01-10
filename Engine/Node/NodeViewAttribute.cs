using System;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NodeViewAttribute : PreserveAttribute
    {
        public bool IsRuntimeView { get; set; }
        public Type NodeType { get; set; }

        public NodeViewAttribute(Type nodeType, bool isRuntimeView = false)
        {
            NodeType = nodeType;
            IsRuntimeView = isRuntimeView;
        }
    }
}