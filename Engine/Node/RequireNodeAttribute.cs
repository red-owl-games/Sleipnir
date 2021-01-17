using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    /// <summary>
    /// Required node for a given Graph.
    /// Will automatically instantiate the node when the graph is first created.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequireNodeAttribute : Attribute
    {
        public Type Type { get; set; }
        public Vector2 Position { get; set; }
        
        /// <param name="type">Type of the required node</param>
        /// <param name="x">y position to creating</param>
        /// <param name="y">x position to creating</param>
        public RequireNodeAttribute(Type type, float x = 0, float y = 0)
        {
            Type = type;
            Position = new Vector2(x, y);

        }
    }
}