namespace RedOwl.Sleipnir.Engine
{
    public interface IGraphView
    {
        void Load(GraphAsset asset);
        void Reload();
        void Save();
    }

    public abstract class RuntimeGraphView : IGraphView
    {
        public void Load(GraphAsset asset) {}

        public void Reload() {}

        public void Save() {}
    }
}