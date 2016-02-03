// Written by Jak Boulton, 02-02-2016
using UnityEngine;

// Namespace declaration
namespace SectiAudio
{
    public class AudioManager : MonoBehaviour
    {
        /// <summary>
        /// Play method used to play an audio clip (2D)
        /// </summary>
        /// <param name="key">Audio clip key</param>
        public static void Play(string key)
        {
        }

        /// <summary>
        /// Play method used to play an audio clip (3D)
        /// </summary>
        /// <param name="key">Audio clip key</param>
        /// <param name="position">Position to play clip</param>
        public static void Play3D(string key, Vector3 position)
        {
        }

        /// <summary>
        /// Play method used to play an audio clip (3D)
        /// </summary>
        /// <param name="key">Audio clip key</param>
        /// <param name="parent">Parent, when you need the sound to follow an object</param>
        public static void Play3D(string key, GameObject parent)
        {
        }
    }
}