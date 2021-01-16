using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace RedOwl.Sleipnir.Engine
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class NodeAttribute : PreserveAttribute
    {
        /// <summary>
        /// Display name of the node.
        ///
        /// If not supplied, this will be inferred based on the class name.
        /// </summary>
        public string Name { get; set; }
        
        public string Tooltip { get; set; }

        /// <summary>
        /// Slash-delimited path to categorize this node in the search window.
        /// </summary>
        public string Path { get; set; }
        
        public string[] Tags { get; }
        
        public bool Deletable { get; set; } = true;
        
        public bool Moveable { get; set; } = true;
        
        public bool IsFlowRoot { get; set; } = false;

        // TODO: Rename this to MinWidth/MinHeight
        public float Width { get; set; } = 50;
        
        public float Height { get; set; } = 10;
        
        public Vector2 Size => new Vector2(Width, Height);
        
        public NodeAttribute(params string[] tags)
        {
            Tags = tags;
        }
    }
}