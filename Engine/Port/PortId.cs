using System;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable]
    public struct PortId
    {
        [field: SerializeField]
        public string Node { get; private set; }

        [field: SerializeField]
        public string Port { get; private set; }

        public PortId(string node, string port)
        {
            Node = node;
            Port = port;
        }

        public override string ToString() => $"{Node}.{Port}";
    }
}