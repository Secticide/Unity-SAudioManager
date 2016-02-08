using UnityEngine;
using System;

namespace SAudioManager
{
    [Serializable]
    public class SceneAudioPackage : ScriptableObject
    {
        public AudioGroup[] audioGroups;
    }
}
