// using System;
// using UnityEngine;
//
// namespace RedOwl.UIX.Engine
// {
//     /// <summary>
//     /// Required node for a given Graph.
//     /// Will automatically instantiate the node when the graph is first created.
//     /// </summary>
//     [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
//     public class RequireNodeAttribute : Attribute
//     {
//         public Type Type { get; }
//         public string Title { get; }
//         public Vector2 Position { get; }
//         
//         /// <param name="type">Type of the required node</param>
//         /// <param name="nodeName">Title name of the node</param>
//         /// <param name="x">y position to creating</param>
//         /// <param name="y">x position to creating</param>
//         public RequireNodeAttribute(Type type, string title = "", float x = 0, float y = 0)
//         {
//             Type = type;
//             Title = title;
//             Position = new Vector2(x, y);
//
//         }
//     }
// }