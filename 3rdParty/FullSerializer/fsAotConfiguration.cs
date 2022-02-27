#if !NO_UNITY
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GSSerializer
{
    [CreateAssetMenu(menuName = "Full Serializer AOT Configuration")]
    public class fsAotConfiguration : ScriptableObject
    {
        public enum AotState
        {
            Default,
            Enabled,
            Disabled
        }

        public List<Entry> aotTypes = new();
        public string outputDirectory = "Assets/AotModels";

        public bool TryFindEntry(Type type, out Entry result)
        {
            var searchFor = type.FullName;
            foreach (var entry in aotTypes)
                if (entry.FullTypeName == searchFor)
                {
                    result = entry;
                    return true;
                }

            result = default;
            return false;
        }

        public void UpdateOrAddEntry(Entry entry)
        {
            for (var i = 0; i < aotTypes.Count; ++i)
                if (aotTypes[i].FullTypeName == entry.FullTypeName)
                {
                    aotTypes[i] = entry;
                    return;
                }

            aotTypes.Add(entry);
        }

        [Serializable]
        public struct Entry
        {
            public AotState State;
            public string FullTypeName;

            public Entry(Type type)
            {
                FullTypeName = type.FullName;
                State = AotState.Default;
            }

            public Entry(Type type, AotState state)
            {
                FullTypeName = type.FullName;
                State = state;
            }
        }
    }
}
#endif