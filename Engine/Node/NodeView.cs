using UnityEngine;
using UnityEngine.UIElements;

namespace RedOwl.Sleipnir.Engine
{
    public interface INodeView
    {
        INode Node { get; }
        bool IsMoveable { get; }
        VisualElement ValueInPortContainer { get; }
        VisualElement ValueOutPortContainer { get; }
        VisualElement FlowInPortContainer { get; }
        VisualElement FlowOutPortContainer { get; }

        void Initialize(INode node, NodeInfo info);
        Rect GetPosition();
        void SetPosition(Rect position);
    }

    public abstract class RuntimeNodeView : INodeView
    {
        public INode Node { get; }
        public bool IsMoveable { get; }
        public VisualElement ValueInPortContainer { get; }
        public VisualElement ValueOutPortContainer { get; }
        public VisualElement FlowInPortContainer { get; }
        public VisualElement FlowOutPortContainer { get; }

        public void Initialize(INode node, NodeInfo info)
        {
            
        }
        
        public Rect GetPosition()
        {
            return new Rect();
        }
        
        public void SetPosition(Rect position)
        {
            
        }
    }
}