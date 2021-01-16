using System;
using System.Diagnostics;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RedOwl.Sleipnir.Engine
{
    public interface IGraphAsset
    {
        string name { get; }
        IGraph Graph { get; set; }
    }

    public abstract class GraphAsset<T> : GraphAssetObject, IGraphAsset where T : IGraph, new()
    {
        [SerializeReference, InlineEditor()] 
        private IGraph _graph = new T();
        
        public IGraph Graph
        {
            get => _graph;
            set => _graph = value;
        }
    }
    
    [CreateAssetMenu(menuName = "Red Owl/Graph", fileName = "Graph")]
    public class GraphAsset : GraphAsset<Graph>
    {
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
            asset.Graph = graph;
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