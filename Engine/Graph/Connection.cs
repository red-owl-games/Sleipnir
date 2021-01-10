using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable]
    public class PortCollection : BetterCollection<PortId> {}

    [Serializable]
    public class ConnectionsGraph : BetterDictionary<PortId, PortCollection>
    {
        public PortCollection SafeGet(PortId key)
        {
            try
            {
                return this[key];
            }
            catch (KeyNotFoundException)
            {
                return new PortCollection();
            }
        }
        
        public void Connect(PortId key, PortId port)
        {
            if (TryGetValue(key, out var collection))
            {
                collection.Add(port);
                return;
            }

            collection = new PortCollection{port};
            Add(key, collection);
        }

        public void Disconnect(PortId key, PortId port)
        {
            if (TryGetValue(key, out var collection))
            {
                collection.Remove(port);
                this[key] = collection;
            }
        }
    }
}
