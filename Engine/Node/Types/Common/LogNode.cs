using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Node("Common", Path = "Common")]
    public class LogNode : Node
    {
        [FlowIn(nameof(OnEnter))] public FlowPort Enter;
        
        [ValueIn] public ValuePort<string> Message = "";

        private void OnEnter(IFlow flow) => Debug.Log(Message.Value);
    }
}
