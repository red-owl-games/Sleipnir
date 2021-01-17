using System.IO;
using RedOwl.Sleipnir.Engine;
using UnityEditor;
using UnityEngine;

namespace RedOwl.Sleipnir.Editor
{
    public static class GraphAssetExtensions
    {
        public static void Save(this IGraph graph, string relativeFolder = null, string name = null)
        {
            string EnsureFolderExits(string path)
            {
                string current = "Assets";
                foreach (string folder in path.Replace("\\", "/").Split('/'))
                {
                    string next = $"{current}/{folder}";
                    if (!AssetDatabase.IsValidFolder(next))
                    {
                        Debug.Log($"Creating Folder: '{next}'");
                        AssetDatabase.CreateFolder(current, folder);
                    }
                    current = next;
                }

                return current;
            }
            var asset = ScriptableObject.CreateInstance<GraphAsset>();
            asset.Graph = graph;
            name = string.IsNullOrEmpty(name) ? graph.GetType().Name : name;
            relativeFolder = string.IsNullOrEmpty(relativeFolder) ? "Resources" : relativeFolder;
            string filepath = Path.Combine(EnsureFolderExits(relativeFolder), name);
            Debug.Log($"Saving Graph Asset @ '{filepath}.asset'");
            AssetDatabase.CreateAsset(asset, $"{filepath}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}