using UnityEngine;
using System;
using SAudioManager;

namespace SAudioManager
{
    public class AudioChannelPackage : ScriptableObject
    {
        public AudioChannel[] channels;
    }

    [Serializable]
    public class AudioChannel
    {
        public string name;
        public int index;
        public string id;
    }
}
