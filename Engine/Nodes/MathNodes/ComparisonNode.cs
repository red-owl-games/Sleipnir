using Unity.Mathematics;

namespace RedOwl.Sleipnir.Engine
{
    public class ComparisonNode : MathNode<double, double, bool>
    {
        public enum Comparison
        {
            Equal,
            NotEqual,
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThenOrEqual
        }

        public Comparison comparison = Comparison.Equal;
        
        protected override void Calculate(IFlow flow)
        {
            switch (comparison)
            {
                case Comparison.Equal:
                    Output.Value = Approximately(Left.Value, Right.Value);
                    break;
                case Comparison.NotEqual:
                    Output.Value = !Approximately(Left.Value, Right.Value);
                    break;
                case Comparison.GreaterThan:
                    Output.Value = Left.Value > Right.Value;
                    break;
                case Comparison.GreaterThanOrEqual:
                    Output.Value = Left.Value >= Right.Value;
                    break;
                case Comparison.LessThan:
                    Output.Value = Left.Value < Right.Value;
                    break;
                case Comparison.LessThenOrEqual:
                    Output.Value = Left.Value <= Right.Value;
                    break;
            }
        }

        private bool Approximately(double a, double b) => math.abs(b - a) < math.EPSILON_DBL;
    }
}