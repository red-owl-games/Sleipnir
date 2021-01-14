using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    #region Base
    
    public interface IGraphValuePortNode
    {
        string Name { get; }
    }
    
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public abstract class ValueInNode<T> : Node, IGraphValuePortNode
    {
        [ValueOut(GraphPort = true)] public ValuePort<T> Out = default;
        
        [SerializeField, HideInInspector]
        private string name = $"In {typeof(T).Name}";

        [ShowInNode]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                // IsDefined = false;
            }
        }
    }
    
    [Serializable]
    [Node("Common", Path = "Common/Graph Ports")]
    public abstract class ValueOutNode<T> : Node, IGraphValuePortNode
    {
        [ValueIn(GraphPort = true)] public ValuePort<T> In = default;
        
        [SerializeField, HideInInspector]
        private string name = $"Out {typeof(T).Name}";

        [ShowInNode]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                // IsDefined = false;
            }
        }
    }
    
    #endregion
    
    #region Primatives

    public class StringInNode : ValueInNode<string> {}
    public class StringOutNode : ValueOutNode<string> {}
    
    public class CharInNode : ValueInNode<char> {}
    public class CharOutNode : ValueOutNode<char> {}
    
    public class BoolInNode : ValueInNode<bool> {}
    public class BoolOutNode : ValueOutNode<bool> {}
    
    public class FloatInNode : ValueInNode<float> {}
    public class FloatOutNode : ValueOutNode<float> {}
    
    public class DoubleInNode : ValueInNode<double> {}
    public class DoubleOutNode : ValueOutNode<double> {}
    
    public class ShortInNode : ValueInNode<short> {}
    public class ShortOutNode : ValueOutNode<short> {}
    
    public class IntInNode : ValueInNode<int> {}
    public class IntOutNode : ValueOutNode<int> {}
    
    public class LongInNode : ValueInNode<long> {}
    public class LongOutNode : ValueOutNode<long> {}
    
    public class UShortInNode : ValueInNode<ushort> {}
    public class UShortOutNode : ValueOutNode<ushort> {}
    
    public class UIntInNode : ValueInNode<uint> {}
    public class UIntOutNode : ValueOutNode<uint> {}
    
    public class ULongInNode : ValueInNode<ulong> {}
    public class ULongOutNode : ValueOutNode<ulong> {}
    
    public class SByteInNode : ValueInNode<sbyte> {}
    public class SByteOutNode : ValueOutNode<sbyte> {}
    
    public class ByteInNode : ValueInNode<byte> {}
    public class ByteOutNode : ValueOutNode<byte> {}

    #endregion
    
    #region Unity
    
    public class ColorInNode : ValueInNode<Color> {}
    public class ColorOutNode : ValueOutNode<Color> {}
    
    public class Color32InNode : ValueInNode<Color32> {}
    public class Color32OutNode : ValueOutNode<Color32> {}
    
    public class Vector2InNode : ValueInNode<Vector2> {}
    public class Vector2OutNode : ValueOutNode<Vector2> {}
    
    public class Vector3InNode : ValueInNode<Vector3> {}
    public class Vector3OutNode : ValueOutNode<Vector3> {}
    
    public class Vector4InNode : ValueInNode<Vector4> {}
    public class Vector4OutNode : ValueOutNode<Vector4> {}
    
    public class Vector2IntInNode : ValueInNode<Vector2Int> {}
    public class Vector2IntOutNode : ValueOutNode<Vector2Int> {}
    
    public class Vector3IntInNode : ValueInNode<Vector3Int> {}
    public class Vector3IntOutNode : ValueOutNode<Vector3Int> {}
    
    public class QuaternionInNode : ValueInNode<Quaternion> {}
    public class QuaternionOutNode : ValueOutNode<Quaternion> {}
    
    public class BoundsInNode : ValueInNode<Bounds> {}
    public class BoundsOutNode : ValueOutNode<Bounds> {}
    
    public class BoundsIntInNode : ValueInNode<BoundsInt> {}
    public class BoundsIntOutNode : ValueOutNode<BoundsInt> {}
    
    public class RectInNode : ValueInNode<Rect> {}
    public class RectOutNode : ValueOutNode<Rect> {}
    
    public class RectIntInNode : ValueInNode<RectInt> {}
    public class RectIntOutNode : ValueOutNode<RectInt> {}

    #endregion
}