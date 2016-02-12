using UnityEngine;
using System;
using SAudioManager;

namespace SAudioManager
{
    public class SAudioChannelCollection : ScriptableObject
    {
        public SAudioChannel[] channels;
    }

    [Serializable]
    public class SAudioChannel
    {
        public string name;
        public int index;
        public string id;
    }
}
