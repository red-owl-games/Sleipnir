#if !ODIN_INSPECTOR
using RedOwl.Sleipnir.Engine;
using UnityEditor;
using UnityEngine;

namespace RedOwl.Sleipnir.Editor
{
    [CustomEditor(typeof(GraphAssetObject), true)] 
    public class GraphAssetEditor : UnityEditor.Editor
    {
        bool expanded = true;
        
        public override void OnInspectorGUI ()
        {
            // My God Thank you Odin for making this much easier ... /facepalm
            if (GUILayout.Button("Open Editor", GUILayout.Height(40))) SleipnirEditor.Open((GraphAsset) serializedObject.targetObject);
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            
            expanded = EditorGUILayout.BeginFoldoutHeaderGroup(expanded, "Graph Settings");
            --EditorGUI.indentLevel;
            SerializedProperty iterator = serializedObject.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                if (iterator.name == "_graph") EditorGUILayout.PropertyField(iterator, GUIContent.none, true);
                
            }
            ++EditorGUI.indentLevel;
            EditorGUILayout.EndFoldoutHeaderGroup();
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
    }
}
#else
// TODO: Have Odin add a custom button for SleipnirEditor.Open(GraphAsset)
#endif