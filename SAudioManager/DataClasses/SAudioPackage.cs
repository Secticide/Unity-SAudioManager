using UnityEngine;
using System;

namespace SAudioManager
{
    [Serializable]
    public class SAudioPackage : ScriptableObject
    {
        public AudioGroup[] audioGroups;
    }
}
