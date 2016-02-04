// Written by Jak Boulton, 02-02-2016
using UnityEngine;
using System;
using System.Collections.Generic;

namespace SAudioManager
{
    public class AudioManager
    {
        // ATTRIBUTES
        private static AudioManager instance;

        private AudioSourcePool audioSourcePool;
        private SceneAudioPackage currentAudioPackage;
        private Dictionary<string, AudioGroup> groupCollection;
        private Dictionary<string, AudioClip> clipCollection;

        // METHODS

        /// <summary>
        /// Initialise is used to create the single instance of the class
        /// </summary>
        public static void Initialise(int concurrentSoundLimit)
        {
            instance = new AudioManager();
            instance.audioSourcePool = new AudioSourcePool(concurrentSoundLimit);
        }

        /// <summary>
        /// Load audio from an audio package
        /// NOTE: Forces a GC.Collect
        /// </summary>
        /// <param name="audioPackage">Scene audio package to load</param>
        public static void LoadAudioPackage(SceneAudioPackage audioPackage)
        {
            ReleaseAudioPackage();
            instance.currentAudioPackage = audioPackage;
            instance.PopulateCollections();
        }

        /// <summary>
        /// Releases the currently loaded audio package
        /// NOTE: Forces a GC.Collect
        /// </summary>
        public static void ReleaseAudioPackage()
        {
            instance.clipCollection = null;
            instance.groupCollection = null;
            instance.currentAudioPackage = null;
            GC.Collect();
        }

        /// <summary>
        /// Play method used to play an audio clip or group (2D)
        /// </summary>
        /// <param name="key">Audio clip or group key</param>
        public static void Play(string key)
        {
            if(instance == null)
            {
                Debug.Log("SAudioManager: AudioManager not initialised.");
                return;
            }

            if(instance.currentAudioPackage == null)
            {
                Debug.Log("SAudioManager: No AudioPackage loaded.");
                return;
            }

            if(instance.clipCollection.ContainsKey(key))
            {
                AudioClip clip;
                instance.clipCollection.TryGetValue(key, out clip);
                AudioSourceController source = instance.audioSourcePool.Request();
                source.Play(clip);
            }
            else if(instance.groupCollection.ContainsKey(key))
            {
                Debug.Log("SAudioManager: Group play not implemented yet.");
            }
            else
            {
                Debug.Log("SAudioManager: Key could not be found in the current audio package.");
            }
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

        // INTERNAL
        // Force singleton
        private AudioManager() {}

        private void PopulateCollections()
        {
            if(currentAudioPackage.audioGroups != null && currentAudioPackage.audioGroups.Length > 0)
            {
                for(int i = 0; i < currentAudioPackage.audioGroups.Length; ++i)
                {
                    LoadAudioGroup(currentAudioPackage.audioGroups[i]);
                }
            }
        }

        private void LoadAudioGroup(AudioGroup audioGroup)
        {
            if(!groupCollection.ContainsKey(audioGroup.name))
            {
                groupCollection.Add(audioGroup.name, audioGroup);

                if(audioGroup.audioClips != null && audioGroup.audioClips.Length > 0)
                {
                    for(int i = 0; i < audioGroup.audioClips.Length; ++i)
                    {
                        if(!clipCollection.ContainsKey(audioGroup.keys[i]))
                        {
                            clipCollection.Add(audioGroup.keys[i], audioGroup.audioClips[i]);
                        }
                    }
                }
            }
                
            if(audioGroup.audioGroups != null && audioGroup.audioGroups.Length > 0)
            {
                for(int i = 0; i < audioGroup.audioGroups.Length; ++i)
                {
                    instance.LoadAudioGroup(audioGroup.audioGroups[i]);
                }
            }
        }
    }
}