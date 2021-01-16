using RedOwl.Sleipnir.Engine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RedOwl.Sleipnir.Editor
{
    public static class SleipnirEditor
    {
        private const string DialogTitle = "Open In New Window?";
        private const string DialogMessage = "Do you want to load this graph into the existing editor window?";

        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            if (!(EditorUtility.InstanceIDToObject(instanceID) is IGraphAsset asset)) return false;
            GetOrCreate().Load(asset);
            return true;
        }
        
        public static void Open(IGraphAsset asset, bool nested = false)
        {
            GetOrCreate().Load(asset, nested);
        }

        // private static bool WindowIsOpenAlready()
        // {
        //     Object[] objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof(SleipnirWindow));
        //     EditorWindow editorWindow = objectsOfTypeAll.Length != 0 ? (EditorWindow) objectsOfTypeAll[0] : null;
        //     return (bool) editorWindow;
        // }

        public static SleipnirWindow GetOrCreate()
        {
            // This was Quite Annoying
            // SleipnirWindow window = null;
            // if (WindowIsOpenAlready())
            // {
            //     if(EditorUtility.DisplayDialog(DialogTitle, DialogMessage, "Yes" , "No, Create New Window"))
            //     {
            //         window = EditorWindow.GetWindow<SleipnirWindow>();
            //     } 
            //     else
            //     {
            //         window = ScriptableObject.CreateInstance<SleipnirWindow>();
            //         window.Show();
            //     }
            // }
            // else
            // {
            //     window = EditorWindow.GetWindow<SleipnirWindow>();
            // }
            var window = EditorWindow.GetWindow<SleipnirWindow>();
            window.titleContent = new GUIContent("Graph Editor");
            return window;
        }
    }
}