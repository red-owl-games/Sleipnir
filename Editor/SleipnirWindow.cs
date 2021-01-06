using System.Collections.Generic;
using RedOwl.Sleipnir.Engine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedOwl.Sleipnir.Editor
{
    // TODO: what happens when we delete the GraphReference this is current viewing?!
    public class SleipnirWindow : EditorWindow
    {
        private Stack<GraphAsset> _graphs;
        private GraphAsset _current;
        private IGraphView _view;

        private Toolbar _toolbar;
        private ToolbarBreadcrumbs _toolbarBreadcrumbs;

        public void OnEnable()
        {
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("SleipnirWindow"));
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            _graphs = new Stack<GraphAsset>(5);
            CreateView();
            CreateToolbar();
            Undo.undoRedoPerformed += Reload;
            Reload();
        }

        public void OnDisable()
        {
            Undo.undoRedoPerformed -= Reload;
        }

        private void Reload()
        {
            _view.Reload();
        }

        private void HandleBreadcrumbClick(int index)
        {
            _view.Save();
            while (_graphs.Count > index)
            {
                _graphs.Pop();
                _toolbarBreadcrumbs.PopItem();
            }

            _current = _graphs.Peek();
            LoadCurrent();
        }

        internal void Load(GraphAsset asset, bool nested = false)
        {
            if (asset == null) return;
            _view.Save();
            
            if (!nested)
            {
                _graphs.Clear();
                while (_toolbarBreadcrumbs.childCount > 0) _toolbarBreadcrumbs.PopItem();
            }
            _current = asset;
            _graphs.Push(asset);
            int lastIndex = _graphs.Count;
            _toolbarBreadcrumbs.PushItem(_current.name, () => { HandleBreadcrumbClick(lastIndex); });

            LoadCurrent();
        }

        internal void LoadCurrent()
        {
            _view.Load(_current);
        }

        private void CreateView()
        {
            _view = EditorApplication.isPlayingOrWillChangePlaymode ? (IGraphView) new SleipnirGraphViewPlaymode() : new SleipnirGraphViewEditmode();
            if (_view is VisualElement element)
            {
                rootVisualElement.Add(element);
                element.StretchToParentSize();
            }
        }

        private void CreateToolbar()
        {
            _toolbar = new Toolbar();

            _toolbarBreadcrumbs = new ToolbarBreadcrumbs();
            _toolbar.Add(_toolbarBreadcrumbs);

            var spacer = new ToolbarSpacer {flex = true};
            _toolbar.Add(spacer);
            
            var saveBtn = new Button(_view.Save) {text = "Save"};
            _toolbar.Add(saveBtn);
            
            var executeBtn = new Button(Execute) {text = "Execute All"};
            _toolbar.Add(executeBtn);

            var search = new ToolbarSearchField();
            _toolbar.Add(search);
            
            var clearBtn = new Button(Clear) {text = "Clear"};
            _toolbar.Add(clearBtn);

            rootVisualElement.Add(_toolbar);
        }

        private void Execute()
        {
            if (_current != null &&  _current.Graph != null) new Flow(_current.Graph).Execute();
        }

        private const string ClearTitle = "Clear Graph?";
        private const string ClearMessage = "Clear all nodes and connections from this graph?";

        private void Clear()
        {
            if (EditorUtility.DisplayDialog(ClearTitle, ClearMessage, "Yes") && _current != null)
            {
                _current.Graph?.Clear();
                Reload();
            }
        }
    }
}