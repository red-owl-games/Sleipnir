using System;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CustomNodeViewAttribute : PreserveAttribute
    {
        public bool IsRuntimeView { get; set; }
        public Type NodeType { get; set; }

        public CustomNodeViewAttribute(Type nodeType, bool isRuntimeView = false)
        {
            NodeType = nodeType;
            IsRuntimeView = isRuntimeView;
        }
    }
}