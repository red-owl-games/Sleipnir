namespace RedOwl.Sleipnir.Engine
{
    public interface IGraphView
    {
        void Load(IGraphAsset asset);
        void Reload();
        void Save();
    }

    public abstract class RuntimeGraphView : IGraphView
    {
        public void Load(IGraphAsset asset) {}

        public void Reload() {}

        public void Save() {}
    }
}