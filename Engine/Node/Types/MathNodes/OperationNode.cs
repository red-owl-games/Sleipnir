using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public class OperationNode : MathNode<double, double, double>
    {
        public enum Operation
        {
            Add,
            Subtract,
            Multiply,
            Divide,
        }

        public Operation operation = Operation.Add;
        
        protected override void Calculate(IFlow flow)
        {
            switch (operation)
            {
                case Operation.Add:
                    Debug.Log($"Doing Calculation '{Left.Value} + {Right.Value}'");
                    Output.Value = Left.Value + Right.Value;
                    break;
                case Operation.Subtract:
                    Output.Value = Left.Value - Right.Value;
                    break;
                case Operation.Multiply:
                    Output.Value = Left.Value * Right.Value;
                    break;
                case Operation.Divide:
                    var rhs = Right.Value;
                    Output.Value = Left.Value / rhs == 0 ? 1 : rhs;
                    break;
            }
        }
    }
}