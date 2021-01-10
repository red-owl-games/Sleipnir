// using System.Collections;
// using UnityEngine;
//
// namespace RedOwl.UIX.Engine
// {
//     [Node("Debug", Path = "Debug")]
//     public class KitchenSinkNode : Node
//     {
//         // Flow
//         [FlowIn(Callback = nameof(OnEnter))] public FlowPort Enter;
//         [FlowIn(Callback = nameof(OnEnter))] public FlowPort Passthrough;
//
//         [FlowOut] public FlowPort Start;
//         [FlowOut] public FlowPort Changed;
//         [FlowOut] public FlowPort Complete;
//         
//         // Primatives
//         
//         [ValueIn, ValueOut] public ValuePort<string> String;
//         [ValueIn, ValueOut] public ValuePort<char> Char;
//         [ValueIn, ValueOut] public ValuePort<bool> Bool;
//         [ValueIn, ValueOut] public ValuePort<float> Float;
//         [ValueIn, ValueOut] public ValuePort<double> Double;
//         // [ValueIn, ValueOut] public ValuePort Decimal; // Does Not Serialize
//         [ValueIn, ValueOut] public ValuePort<short> Short;
//         [ValueIn, ValueOut] public ValuePort<int> Int;
//         [ValueIn, ValueOut] public ValuePort<long> Long;
//         [ValueIn, ValueOut] public ValuePort<ushort> UShort;
//         [ValueIn, ValueOut] public ValuePort<uint> UInt;
//         [ValueIn, ValueOut] public ValuePort<ulong> ULong;
//         [ValueIn, ValueOut] public ValuePort<sbyte> SByte;
//         [ValueIn, ValueOut] public ValuePort<byte> Byte;
//         
//         // Array
//         // [ValueIn, ValueOut] public ValuePort StringArray; // Caused a Crash on Recompile
//         // [ValueIn, ValueOut] public ValuePort FloatArray; // Caused a Crash on Recompile
//         //
//         // // List
//         // [ValueIn, ValueOut] public ValuePort StringList; // Caused a Crash on Recompile
//         // [ValueIn, ValueOut] public ValuePort FloatList; // Caused a Crash on Recompile
//         
//         // Complex Types
//         // [ValueIn, ValueOut] public ValuePort Uri; // Does Not Serialize
//         // [ValueIn, ValueOut] public ValuePort Guid; // Does Not Serialize
//         // [ValueIn, ValueOut] public ValuePort DateTime; // Does Not Serialize
//         // [ValueIn, ValueOut] public ValuePort DateTimeOffset; // Does Not Serialize
//         // [ValueIn, ValueOut] public ValuePort TimeSpan; // Does Not Serialize
//         
//         // Unity Types
//         [ValueIn, ValueOut] public ValuePort<Color> Color;
//         [ValueIn, ValueOut] public ValuePort<Color32> Color32;
//         [ValueIn, ValueOut] public ValuePort<Vector2> Vector2;
//         [ValueIn, ValueOut] public ValuePort<Vector3> Vector3;
//         [ValueIn, ValueOut] public ValuePort<Vector4> Vector4;
//         [ValueIn, ValueOut] public ValuePort<Vector2Int> Vector2Int;
//         [ValueIn, ValueOut] public ValuePort<Vector3Int> Vector3Int;
//         [ValueIn, ValueOut] public ValuePort<Quaternion> Quaternion;
//         [ValueIn, ValueOut] public ValuePort<Bounds> Bounds;
//         [ValueIn, ValueOut] public ValuePort<BoundsInt> BoundsInt;
//         [ValueIn, ValueOut] public ValuePort<Rect> Rect;
//         [ValueIn, ValueOut] public ValuePort<RectInt> RectInt;
//         // [ValueIn, ValueOut] public ValuePort AnimationCurve; // Caused a Crash on Recompile
//         // [ValueIn, ValueOut] public ValuePort Gradient; // Caused a Crash on Recompile
//         // [ValueIn, ValueOut] public ValuePort LayerMask; // Caused a Crash on Recompile
//         // [ValueIn, ValueOut] public ValuePort Texture; // Does Not Serialize
//         // [ValueIn, ValueOut] public ValuePort Texture2D; // Does Not Serialize
//         // [ValueIn, ValueOut] public ValuePort Texture3D; // Does Not Serialize
//         // [ValueIn, ValueOut] public ValuePort RenderTexture; // Does Not Serialize
//         
//         // Unity Mathamatics
//         
//         // Field
//         public string Field = "Test";
//         
//         public KitchenSinkNode()
//         {
//             Enter = new FlowPort(this);
//             Passthrough = new FlowPort(this);
//             
//             Start = new FlowPort(this);
//             Changed = new FlowPort(this);
//             Complete = new FlowPort(this);
//             
//             String = new ValuePort<string>(this);
//             Char = new ValuePort<char>(this);
//             Bool = new ValuePort<bool>(this);
//             Float = new ValuePort<float>(this);
//             Double = new ValuePort<double>(this);
//             // Decimal = new ValuePort<decimal>(this);
//             Short = new ValuePort<short>(this);
//             Int = new ValuePort<int>(this);
//             Long = new ValuePort<long>(this);
//             UShort = new ValuePort<ushort>(this);
//             UInt = new ValuePort<uint>(this);
//             ULong = new ValuePort<ulong>(this);
//             SByte = new ValuePort<sbyte>(this);
//             Byte = new ValuePort<byte>(this);
//
//             // StringArray = new ValuePort<string[]>(this, new []{"Hello", "World"});
//             // FloatArray = new ValuePort<float[]>(this, new []{1.5f, 0.5f});
//             //
//             // StringList = new ValuePort<List<string>>(this, new List<string> {"Hello", "World"});
//             // FloatList = new ValuePort<List<float>>(this, new List<float> {1.5f, 0.5f});
//             
//             // Uri = new ValuePort<Uri>(this);
//             // Guid = new ValuePort<Guid>(this);
//             // DateTime = new ValuePort<DateTime>(this);
//             // DateTimeOffset = new ValuePort<DateTimeOffset>(this);
//             // TimeSpan = new ValuePort<TimeSpan>(this);
//             
//             Color = new ValuePort<Color>(this);
//             Color32 = new ValuePort<Color32>(this);
//             Vector2 = new ValuePort<Vector2>(this);
//             Vector3 = new ValuePort<Vector3>(this);
//             Vector4 = new ValuePort<Vector4>(this);
//             Vector2Int = new ValuePort<Vector2Int>(this);
//             Vector3Int = new ValuePort<Vector3Int>(this);
//             Quaternion = new ValuePort<Quaternion>(this);
//             Bounds = new ValuePort<Bounds>(this);
//             BoundsInt = new ValuePort<BoundsInt>(this);
//             Rect = new ValuePort<Rect>(this);
//             RectInt = new ValuePort<RectInt>(this);
//             // AnimationCurve = new ValuePort<AnimationCurve>(this);
//             // Gradient = new ValuePort<Gradient>(this);
//             // LayerMask = new ValuePort<LayerMask>(this);
//             // Texture = new ValuePort<Texture>(this);
//             // Texture2D = new ValuePort<Texture2D>(this);
//             // Texture3D = new ValuePort<Texture3D>(this);
//             // RenderTexture = new ValuePort<RenderTexture>(this);
//             
//             // Unity Mathamatics
//         }
//         
//         public override void Definition()
//         {
//             
//         }
//         
//         private IEnumerable OnEnter(IFlow flow)
//         {
//             yield return Start;
//
//             for (int i = 0; i < 5; i++)
//             {
//                 yield return Changed;
//                 yield return new WaitForSeconds(0.5f);
//             }
//
//             yield return Complete;
//         }
//
//         private IEnumerable OnPassthrough(IFlow flow)
//         {
//             yield return Complete;
//         }
//     }
// }
