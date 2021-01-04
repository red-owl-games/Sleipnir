using RedOwl.Sleipnir.Engine;
using UnityEngine;

namespace RedOwl.Sleipnir.Editor
{
    [CustomNodeView(typeof(GraphReferenceNode))]
    public class GraphReferenceNodeView : SleipnirNodeViewBase
    {
        protected override void OnInitialize()
        {
            base.OnInitialize();
            Debug.Log("Hello World Custom NodeView!");
        }
    }
}