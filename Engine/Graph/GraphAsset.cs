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
    public class GraphAsset : GraphAsset<Graph> {}
}