using System.Collections;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Node("Flow", Path = "Flow Control")]
    public class TypeWriterNode : Node
    {
        [FlowIn(Callback = nameof(WriteText))] public FlowPort Start;
        [FlowOut] public FlowPort Typed;
        [FlowOut] public FlowPort Complete;

        [ValueIn] public ValuePort<string> Text = "Hello World";
        [ValueIn] public ValuePort<float> Delay = 0.2f;
        [ValueOut] public ValuePort<string> Current = "";

        private IEnumerable WriteText(IFlow flow)
        {
            string startingText = Text.Value;
            float wait = Delay.Value;
            int count = 1;
            
            while (Current.Value.Length < startingText.Length)
            {
                Current.Value = startingText.Substring(0, count);

                yield return Typed;

                yield return new WaitForSeconds(wait);

                count++;
            }

            yield return Complete;
        }
    }
}