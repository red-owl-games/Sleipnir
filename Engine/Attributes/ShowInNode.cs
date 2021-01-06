using System;
using Sirenix.OdinInspector;

namespace RedOwl.Sleipnir.Engine
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
#if ODIN_INSPECTOR
    public class ShowInNode : ShowInInspectorAttribute
#else
    public class ShowInNode : Attribute
#endif
    {
        /// <summary>
        /// Display name of the editable field.
        ///
        /// If not supplied, this will be inferred based on the field name.
        /// </summary>
        public string Name { get; set; }

        public ShowInNode(string name = null)
        {
            Name = name;
        }
    }
}