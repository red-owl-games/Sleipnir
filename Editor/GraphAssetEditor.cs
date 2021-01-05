using RedOwl.Sleipnir.Engine;
using UnityEditor;
using UnityEngine;

namespace RedOwl.Sleipnir.Editor
{
    [CustomEditor(typeof(GraphAsset))] 
    public class GraphAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI ()
        {
            if (GUILayout.Button("Open Editor", GUILayout.Height(40))) SleipnirEditor.Open((GraphAsset) serializedObject.targetObject);
        }
    }
}