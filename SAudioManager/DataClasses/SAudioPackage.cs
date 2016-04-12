// Written by Jak Boulton, 04-02-2016
using UnityEngine;
using System;

namespace SAudioManager
{
    [Serializable]
    public class SAudioPackage : ScriptableObject
    {
        public SAudioGroup[] audioGroups;
        public SAudioClip[] audioClips;
    }
}
