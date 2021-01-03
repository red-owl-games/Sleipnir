using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    // TODO: Have a bool when True it will expose this valueNode as a port?  What about input vs output?

    [Serializable]
    [Node("Common", Path = "Common/Value")]
    public abstract class ValueNode<TValue> : Node
    {
        [ValueOut] public ValuePort<TValue> Value;

        [SerializeField]
        private TValue defaultValue;
        protected ValueNode(TValue defaultValue = default)
        {
            this.defaultValue = defaultValue;
        }
        
        protected override void OnDefinition()
        {
            Value.SetDefault(defaultValue);
        }

        protected override void OnInitialize(ref IFlow flow)
        {
            Value.SetDefault(defaultValue);
        }
    }
    
    // Primatives
    
    public class StringValueNode : ValueNode<string>
    {
        public StringValueNode() : this(default) {}
        public StringValueNode(string defaultValue) : base(defaultValue) { }
    }
    
    public class CharValueNode : ValueNode<char>
    {
        public CharValueNode() : this(default) {}
        public CharValueNode(char defaultValue) : base(defaultValue) { }
    }

    public class BoolValueNode : ValueNode<bool>
    {
        public BoolValueNode() : this(default) {}
        public BoolValueNode(bool defaultValue) : base(defaultValue) { }
    }
    
    public class FloatValueNode : ValueNode<float>
    {
        public FloatValueNode() : this(default) {}
        public FloatValueNode(float defaultValue) : base(defaultValue) { }
    }
    
    public class DoubleValueNode : ValueNode<double>
    {
        public DoubleValueNode() : this(default) {}
        public DoubleValueNode(double defaultValue) : base(defaultValue) { }
    }
    
    public class ShortValueNode : ValueNode<short>
    {
        public ShortValueNode() : this(default) {}
        public ShortValueNode(short defaultValue) : base(defaultValue) { }
    }
    
    public class IntValueNode : ValueNode<int>
    {
        public IntValueNode() : this(default) {}
        public IntValueNode(int defaultValue) : base(defaultValue) { }
    }
    
    public class LongValueNode : ValueNode<long>
    {
        public LongValueNode() : this(default) {}
        public LongValueNode(long defaultValue) : base(defaultValue) { }
    }
    
    public class UShortValueNode : ValueNode<ushort>
    {
        public UShortValueNode() : this(default) {}
        public UShortValueNode(ushort defaultValue) : base(defaultValue) { }
    }
    
    public class UIntValueNode : ValueNode<uint>
    {
        public UIntValueNode() : this(default) {}
        public UIntValueNode(uint defaultValue) : base(defaultValue) { }
    }
    
    public class ULongValueNode : ValueNode<ulong>
    {
        public ULongValueNode() : this(default) {}
        public ULongValueNode(ulong defaultValue) : base(defaultValue) { }
    }
    
    public class SByteValueNode : ValueNode<sbyte>
    {
        public SByteValueNode() : this(default) {}
        public SByteValueNode(sbyte defaultValue) : base(defaultValue) { }
    }
    
    public class ByteValueNode : ValueNode<byte>
    {
        public ByteValueNode() : this(default) {}
        public ByteValueNode(byte defaultValue) : base(defaultValue) { }
    }
    
    // Unity Types
    
    public class ColorValueNode : ValueNode<Color>
    {
        public ColorValueNode() : this(default) {}
        public ColorValueNode(Color defaultValue) : base(defaultValue) { }
    }
    
    public class Color32ValueNode : ValueNode<Color32>
    {
        public Color32ValueNode() : this(default) {}
        public Color32ValueNode(Color32 defaultValue) : base(defaultValue) { }
    }
    
    public class Vector2ValueNode : ValueNode<Vector2>
    {
        public Vector2ValueNode() : this(default) {}
        public Vector2ValueNode(Vector2 defaultValue) : base(defaultValue) { }
    }
    
    public class Vector3ValueNode : ValueNode<Vector3>
    {
        public Vector3ValueNode() : this(default) {}
        public Vector3ValueNode(Vector3 defaultValue) : base(defaultValue) { }
    }
    
    public class Vector4ValueNode : ValueNode<Vector4>
    {
        public Vector4ValueNode() : this(default) {}
        public Vector4ValueNode(Vector4 defaultValue) : base(defaultValue) { }
    }
    
    public class Vector2IntValueNode : ValueNode<Vector2Int>
    {
        public Vector2IntValueNode() : this(default) {}
        public Vector2IntValueNode(Vector2Int defaultValue) : base(defaultValue) { }
    }
    
    public class Vector3IntValueNode : ValueNode<Vector3Int>
    {
        public Vector3IntValueNode() : this(default) {}
        public Vector3IntValueNode(Vector3Int defaultValue) : base(defaultValue) { }
    }
    
    public class QuaternionValueNode : ValueNode<Quaternion>
    {
        public QuaternionValueNode() : this(default) {}
        public QuaternionValueNode(Quaternion defaultValue) : base(defaultValue) { }
    }
    
    public class BoundsValueNode : ValueNode<Bounds>
    {
        public BoundsValueNode() : this(default) {}
        public BoundsValueNode(Bounds defaultValue) : base(defaultValue) { }
    }
    
    public class BoundsIntValueNode : ValueNode<BoundsInt>
    {
        public BoundsIntValueNode() : this(default) {}
        public BoundsIntValueNode(BoundsInt defaultValue) : base(defaultValue) { }
    }
    
    public class RectValueNode : ValueNode<Rect>
    {
        public RectValueNode() : this(default) {}
        public RectValueNode(Rect defaultValue) : base(defaultValue) { }
    }
    
    public class RectIntValueNode : ValueNode<RectInt>
    {
        public RectIntValueNode() : this(default) {}
        public RectIntValueNode(RectInt defaultValue) : base(defaultValue) { }
    }
}