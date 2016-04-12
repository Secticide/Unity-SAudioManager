// Written by Jak Boulton, 04-02-2016
using UnityEngine;
using System;
using SAudioManager;

namespace SAudioManager
{
    [Serializable]
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
