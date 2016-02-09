﻿// Written by Jak Boulton, 02-02-2016
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

        private List<Queue<string>> audioQueues;

        // METHODS

        /// <summary>
        /// Initialise is used to create the single instance of the class
        /// </summary>
        public static void Initialise(int concurrentSoundLimit)
        {
            instance = new AudioManager();
            instance.audioSourcePool = new AudioSourcePool(concurrentSoundLimit);
            instance.audioQueues = new List<Queue<string>>();
        }

        /// <summary>
        /// Load audio from an audio package
        /// NOTE: Forces a Garbage Collection and Resources Unload
        /// </summary>
        /// <param name="audioPackage">Scene audio package to load</param>
        public static void LoadAudioPackage(SceneAudioPackage audioPackage)
        {
            ReleaseAudioPackage();
            instance.currentAudioPackage = audioPackage;
            instance.ParseAudioPackage();
        }

        /// <summary>
        /// Creates a temporary audio package and loads audio from groups
        /// NOTE: Forces a Garbage Collection and Resources Unload
        /// </summary>
        /// <param name="audioGroups">Audio groups to load into new package</param>
        public static void LoadAudioPackage(AudioGroup[] audioGroups)
        {
            ReleaseAudioPackage();
            instance.currentAudioPackage = new SceneAudioPackage();
            instance.currentAudioPackage.audioGroups = audioGroups;
            instance.ParseAudioPackage();
        }

        /// <summary>
        /// Loads audio group into the currently loaded package
        /// </summary>
        /// <param name="audioGroup">Audio group to be loaded</param>
        public static void LoadAudioGroup(AudioGroup audioGroup)
        {
            instance.ParseAudioGroup(audioGroup);
        }

        /// <summary>
        /// Releases the currently loaded audio package
        /// NOTE: Forces a Garbage Collection and Resources Unload
        /// </summary>
        public static void ReleaseAudioPackage()
        {
            instance.clipCollection = null;
            instance.groupCollection = null;
            instance.currentAudioPackage = null;
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// Play method used to play an audio clip or group
        /// </summary>
        /// <param name="key">Audio clip or group key</param>
        public static void Play(string key)
        {
            if(!CheckSetup())
            {
                return;
            }

            InternalPlay(key);
        }

        /// <summary>
        /// Play method used to play a collection of audio clips or groups
        /// </summary>
        /// <param name="keys">The audio keys to play</param>
        public static void Play(string[] keys)
        {
            if(!CheckSetup())
            {
                return;
            }

            instance.audioQueues.Add(new Queue<string>(keys));
            int queueIndex = instance.audioQueues.Count - 1;
            for(int i = 0; i < keys.Length; ++i)
            {
                instance.audioQueues[queueIndex].Enqueue(keys[i]);
            }

            InternalPlay(instance.audioQueues[queueIndex].Dequeue(), queueIndex);
        }

        // INTERNAL
        // Force singleton
        private AudioManager() {}

        private void AudioCallback(AudioSourceController source, int queueIndex = -1)
        {
            audioSourcePool.Collect(source);

            if(queueIndex == -1)
            {
                return;
            }

            if(audioQueues != null && audioQueues.Count > queueIndex)
            {
                if(audioQueues[queueIndex].Count > 0)
                {
                    AudioManager.InternalPlay(audioQueues[queueIndex].Dequeue(), queueIndex);
                }
            }
        }

        private static bool CheckSetup()
        {
            if(instance == null)
            {
                Debug.Log("SAudioManager: AudioManager not initialised.");
                return false;
            }

            if(instance.currentAudioPackage == null)
            {
                Debug.Log("SAudioManager: No AudioPackage loaded.");
                return false;
            }

            return true;
        }

        private static void InternalPlay(string key, int queueIndex = -1)
        {
            if(instance.clipCollection.ContainsKey(key))
            {
                AudioClip clip;
                instance.clipCollection.TryGetValue(key, out clip);
                AudioSourceController source = instance.audioSourcePool.Request();
                if(source != null)
                {
                    source.Play(clip, 0, 1, instance.AudioCallback, queueIndex);
                }
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

        private void ParseAudioPackage()
        {
            groupCollection = new Dictionary<string, AudioGroup>();
            clipCollection = new Dictionary<string, AudioClip>();
            if(currentAudioPackage.audioGroups != null && currentAudioPackage.audioGroups.Length > 0)
            {
                for(int i = 0; i < currentAudioPackage.audioGroups.Length; ++i)
                {
                    LoadAudioGroup(currentAudioPackage.audioGroups[i]);
                }
            }
        }

        private void ParseAudioGroup(AudioGroup audioGroup)
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
                    instance.ParseAudioGroup(audioGroup.audioGroups[i]);
                }
            }
        }
    }
}