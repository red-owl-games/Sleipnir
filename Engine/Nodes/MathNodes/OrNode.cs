namespace RedOwl.Sleipnir.Engine
{
    public class OrNode : MathNode<bool, bool, bool>
    {
        protected override void Calculate(IFlow flow)
        {
            Output.Value = Left.Value || Right.Value;
        }
    }
}