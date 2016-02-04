using UnityEngine;
using System;

namespace SAudioManager
{
    [Serializable]
    public class AudioGroup
    {
        public string name;
        public string[] keys;
        public AudioClip[] audioClips;
        public AudioGroup[] audioGroups;
    }
}
