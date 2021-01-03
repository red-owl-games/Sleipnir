namespace RedOwl.Sleipnir.Engine
{
    public class AndNode : MathNode<bool, bool, bool>
    {
        protected override void Calculate(IFlow flow)
        {
            Output.Value = Left.Value && Right.Value;
        }
    }
}