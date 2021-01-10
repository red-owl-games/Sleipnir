// namespace RedOwl.UIX.Engine
// {
//     [Node("Flow", Path = "Flow Control")]
//     public class CoroutineNode : Node
//     {
//         [ValueIn] public float Interval { get; } = 1;
//         
//         [FlowIn]
//         private void Enter(Flow flow)
//         {
//             flow.StartRoutine(this, Exit, flow.Get(Interval));
//         }
//
//         [FlowOut] private ControlPort Exit;
//     }
// }