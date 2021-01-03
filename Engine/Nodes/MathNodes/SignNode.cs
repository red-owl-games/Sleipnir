using Unity.Mathematics;

namespace RedOwl.Sleipnir.Engine
{
    public class SignNode : MathNode<double, double>
    {
        protected override void Calculate(IFlow flow)
        {
            Output.Value = math.sign(Input.Value);
        }
    }
}
