namespace RedOwl.Sleipnir.Engine
{
    public enum PortDirection
    {
        Input,
        Output,
    }
    
    public static class PortDirectionExtensions
    {
        public static PortDirection Flip(this PortDirection self)
        {
            return self == PortDirection.Input ? PortDirection.Output : PortDirection.Input;
        }
    }
}