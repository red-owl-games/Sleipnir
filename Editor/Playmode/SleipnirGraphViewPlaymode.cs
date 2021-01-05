using RedOwl.Sleipnir.Engine;

namespace RedOwl.Sleipnir.Editor
{
    public class SleipnirGraphViewPlaymode : SleipnirGraphViewBase<SleipnirNodeViewPlaymode>, IGraphView
    {
        public void Load(GraphAsset asset)
        {
            CreateGridBackground();
        }
        
        public void Reload() {}

        public override void Save() {}
    }
}