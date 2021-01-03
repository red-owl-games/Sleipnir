using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RedOwl.Sleipnir.Engine
{
    [CreateAssetMenu(menuName = "Red Owl/Graph", fileName = "Graph")]
    public class GraphAsset : ScriptableObject
    {
        [SerializeReference] 
        public IGraph graph;

        [Conditional("UNITY_EDITOR")]
        public static void Save<T>(T graph, string name = null, string relativeFolder = null) where T : IGraph
        {
#if UNITY_EDITOR
            string EnsureFolderExits(string path)
            {
                string current = "Assets";
                foreach (string folder in path.Replace("\\", "/").Split('/'))
                {
                    string next = $"{current}/{folder}";
                    if (!UnityEditor.AssetDatabase.IsValidFolder(next))
                    {
                        Debug.Log($"Creating Folder: '{next}'");
                        UnityEditor.AssetDatabase.CreateFolder(current, folder);
                    }
                    current = next;
                }

                return current;
            }
            var asset = CreateInstance<GraphAsset>();
            asset.graph = graph;
            name = string.IsNullOrEmpty(name) ? typeof(T).Name : name;
            relativeFolder = string.IsNullOrEmpty(relativeFolder) ? "Resources" : relativeFolder;
            string filepath = Path.Combine(EnsureFolderExits(relativeFolder), name);
            Debug.Log($"Saving Graph Asset @ '{filepath}.asset'");
            UnityEditor.AssetDatabase.CreateAsset(asset, $"{filepath}.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
    }
}