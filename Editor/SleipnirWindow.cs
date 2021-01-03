using RedOwl.Sleipnir.Engine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedOwl.Sleipnir.Editor
{
    // TODO: what happens when we delete the GraphReference this is current viewing?!
    public class SleipnirWindow : EditorWindow
    {
        #region AutoOpen

        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (!(EditorUtility.InstanceIDToObject(instanceID) is GraphAsset asset)) return false;
            GetOrCreate().Load(asset);
            return true;
        }
        
        public static void Open(GraphAsset asset) => GetOrCreate().Load(asset);

        #endregion

        public static SleipnirWindow GetOrCreate()
        {
            var window = GetWindow<SleipnirWindow>();
            window.titleContent = new GUIContent("Graph Editor");
            return window;
        }
        
        [SerializeReference] private GraphAsset _lastAsset;
        private SleipnirGraphView _view;

        private Toolbar _toolbar;

        public void OnEnable()
        {
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("SleipnirWindow"));
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            Undo.undoRedoPerformed += Reload;
            Reload();
        }

        public void OnDisable()
        {
            Undo.undoRedoPerformed -= Reload;
        }

        private void Reload()
        {
            // TODO: this resets the view on undo/redo so we need to save the view "position"
            Load(AssetDatabase.LoadAssetAtPath<GraphAsset>(AssetDatabase.GetAssetPath(_lastAsset)));
        }

        internal void Load(GraphAsset asset)
        {
            if (asset == null) return;
            EnsureLastGraphSaved();
            _lastAsset = asset;
            EditorUtility.SetDirty(_lastAsset);
            if (_lastAsset.graph == null) _lastAsset.graph = new Graph();
            Cleanup();
            CreateView();
            CreateToolbar();
        }

        private void EnsureLastGraphSaved()
        {
            if (_lastAsset == null) return;
            EditorUtility.SetDirty(_lastAsset);
            AssetDatabase.SaveAssets();
        }

        private void Cleanup()
        {
            if (_toolbar != null) rootVisualElement.Remove(_toolbar);
            if (_view != null) rootVisualElement.Remove(_view);
        }

        private void CreateView()
        {
            // TODO: Make GraphView work differently during "runtime" (IE no edits?)
            _view = new SleipnirGraphView(_lastAsset);
            rootVisualElement.Add(_view);
            _view.StretchToParentSize();
        }

        private void CreateToolbar()
        {
            _toolbar = new Toolbar();

            // TODO: Graph Window Toolbar
            // var clearBtn = new Button(() => { }) {text = "Clear"};
            // _toolbar.Add(clearBtn);
            //
            var saveBtn = new Button(_view.SaveAsset) {text = "Save"};
            _toolbar.Add(saveBtn);
            
            var executeBtn = new Button(Execute) {text = "Execute"};
            _toolbar.Add(executeBtn);
            //
            // var loadBtn = new Button(() =>
            // {
            //     _view.Clean();
            //     _view.Load(JsonUtility.FromJson<CacophonyGraph>(_save));
            // });
            // loadBtn.text = "Load";
            // toolbar.Add(loadBtn);
            
            rootVisualElement.Add(_toolbar);
        }

        private void Execute()
        {
            if (_lastAsset.graph != null) new Flow(_lastAsset.graph).Execute();
        }
    }
}