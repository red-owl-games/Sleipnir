using System.Collections;

namespace RedOwl.Sleipnir.Engine
{
    public abstract class StateNode : Node
    {
        [FlowIn(nameof(HandleEnter))] public FlowPort Enter;
        [FlowOut(nameof(OnExit))] public FlowPort Exit;

        protected IEnumerator HandleEnter(IFlow flow)
        {
            OnEnter(flow);
            var enumerator = OnUpdate(flow);
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }

            yield return Exit;
        }

        protected abstract void OnEnter(IFlow flow);
        protected abstract IEnumerator OnUpdate(IFlow flow);
        protected abstract void OnExit(IFlow flow);
    }
}