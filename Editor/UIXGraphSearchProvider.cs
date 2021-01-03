using System;
using System.Collections.Generic;
using System.Reflection;
using RedOwl.Sleipnir.Engine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RedOwl.Sleipnir.Editor
{
    public struct SearchGroupKey
    {
        private readonly string _name;
        private readonly int _depth;

        public SearchGroupKey(string name, int depth)
        {
            _name = name;
            _depth = depth;
        }

        public override int GetHashCode() => _name.GetHashCode() + _depth.GetHashCode();
    }
    
    public class SearchGroup
    {
        public SearchTreeGroupEntry Section { get; }
        public List<SearchTreeEntry> Entries { get; }
        
        public SearchGroup(string name, int depth)
        {

            Section = new SearchTreeGroupEntry(new GUIContent(name), depth);
            Entries = new List<SearchTreeEntry>();
        }

        public void Add(SleipnirNodeReflection data)
        {
            Entries.Add(new SearchTreeEntry(new GUIContent(data.Name)){ userData = data, level = Section.level + 1});
        }
    }
    
    public class UIXGraphSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private SleipnirGraphView _view;
        
        private SleipnirGraphReflection _graphTypeData;
        private bool _useGraphTagMatching;

        public void Initialize(SleipnirGraphView view)
        {
            _view = view;
            bool found = SleipnirGraphReflector.GraphCache.Get(_view.Graph.GetType(), out _graphTypeData);
            _useGraphTagMatching = found && _graphTypeData?.Tags.Count > 0;
        }
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            };
            foreach (var group in GetSearchGroups())
            {
                tree.Add(group.Section);
                foreach (var entry in group.Entries)
                {
                    tree.Add(entry);
                }
            }
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            _view.CreateNode((SleipnirNodeReflection)entry.userData, context.screenMousePosition);
            return true;
        }

        private IEnumerable<SearchGroup> GetSearchGroups()
        {
            Dictionary<SearchGroupKey, SearchGroup> groups = new Dictionary<SearchGroupKey, SearchGroup>();
            foreach (var node in SleipnirGraphReflector.NodeCache.All)
            {
                if (_useGraphTagMatching && !_graphTypeData.Tags.Overlaps(node.Tags)) continue;
                SearchGroup searchGroup = null;
                int depth = 1;
                
                foreach (string subsection in node.Path)
                {
                    var key = new SearchGroupKey(subsection, depth);
                    if (!groups.TryGetValue(key, out searchGroup))
                    {
                        searchGroup = new SearchGroup(subsection, depth);
                        groups.Add(key, searchGroup);
                    }
                    depth++;
                }

                searchGroup?.Add(node);
            }

            var data = new List<SearchGroup>(groups.Values);
            data.Sort((a, b) => string.Compare(a.Section.name, b.Section.name, StringComparison.Ordinal));
            foreach (var group in data)
            {
                yield return group;
            }
        }
    }
}