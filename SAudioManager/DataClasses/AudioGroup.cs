// Written by Jak Boulton, 04-02-2016
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
        public AudioChannel[] channels;
        public AudioGroup[] audioGroups;
    }
}
