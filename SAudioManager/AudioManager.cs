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
        private static AudioChannelManager channelManager;

        private Dictionary<string, SAudioGroup> groupCollection;
        private Dictionary<string, SAudioClip> clipCollection;

        private Dictionary<string, Queue<string>> audioQueues;

        // METHODS

        /// <summary>
        /// Initialise is used to create the single instance of the class
        /// </summary>
        public static void Initialise(int concurrentSoundLimit)
        {
            instance = new AudioManager();
            instance.audioQueues = new Dictionary<string, Queue<string>>();
            channelManager = new AudioChannelManager(concurrentSoundLimit);
        }

        /// <summary>
        /// Load audio from an audio package
        /// </summary>
        /// <param name="audioPackage">Scene audio package to load</param>
        public static void LoadAudioPackage(SAudioPackage audioPackage)
        {
            if(audioPackage.audioGroups != null && audioPackage.audioGroups.Length > 0)
            {
                if(instance.groupCollection == null)
                {
                    instance.groupCollection = new Dictionary<string, SAudioGroup>();
                }

                for(int i = 0; i < audioPackage.audioGroups.Length; ++i)
                {
                    instance.groupCollection.Add(audioPackage.audioGroups[i].key, audioPackage.audioGroups[i]);
                }
            }

            if(audioPackage.audioClips != null && audioPackage.audioClips.Length > 0)
            {
                if(instance.clipCollection == null)
                {
                    instance.clipCollection = new Dictionary<string, SAudioClip>();
                }

                for(int i = 0; i < audioPackage.audioClips.Length; ++i)
                {
                    instance.clipCollection.Add(audioPackage.audioClips[i].key, audioPackage.audioClips[i]);
                }
            }
        }

        /// <summary>
        /// Releases the currently loaded audio package
        /// NOTE: Forces a Garbage Collection and Resources Unload
        /// </summary>
        public static void ReleaseAudioPackage()
        {
            instance.clipCollection = null;
            instance.groupCollection = null;
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

            if(instance.groupCollection.ContainsKey(key))
            {
                SAudioGroup group;
                instance.groupCollection.TryGetValue(key, out group);
                Play(group.clipKeys);
            }
            else
            {
                string playId = Guid.NewGuid().ToString();
                InternalPlay(playId, key);
            }
        }

        /// <summary>
        /// Play method used to play a collection of audio clips or groups
        /// </summary>
        /// <param name="keys">The audio keys to play</param>
        public static string Play(string[] keys)
        {
            if(!CheckSetup())
            {
                return string.Empty;
            }

            string playId = Guid.NewGuid().ToString();
            Queue<string> audioQueue = new Queue<string>(keys);
            instance.audioQueues.Add(playId, audioQueue);
            InternalPlay(playId, audioQueue.Dequeue());
            return playId;
        }

        /// <summary>
        /// Stop method used to stop either a specific channel or all channels
        /// </summary>
        /// <param name="channel">The channel to stop</param>
        public static void Stop(string channel = null, bool decay = false, float decayDuration = 1.0f)
        {
            if(channel == null)
            {
                channelManager.Stop(null, decay, decayDuration);
            }
            else
            {
                channelManager.Stop(new string[]{channel}, decay, decayDuration);
            }
        }

        /// <summary>
        /// Stop method used to stop either a group of channels
        /// </summary>
        /// <param name="channels">The channels to stop</param>
        public static void Stop(string[] channels, bool decay = false, float decayDuration = 1.0f)
        {
            channelManager.Stop(channels, decay, decayDuration);
        }

        // INTERNAL
        // Force singleton
        private AudioManager() {}

        private void AudioCallback(SAudioSource source)
        {
            Queue<string> audioQueue;
            if(audioQueues.TryGetValue(source.id, out audioQueue))
            {
                if(audioQueue.Count > 0 && !source.isStopped)
                {
                    AudioManager.InternalPlay(source.id, audioQueue.Dequeue());
                }
                else
                {
                    audioQueues.Remove(source.id);
                }
            }

            channelManager.CompletePlay(source);
        }

        private static bool CheckSetup()
        {
            if(instance == null)
            {
                Debug.Log("SAudioManager: AudioManager not initialised.");
                return false;
            }

            return true;
        }

        private static void InternalPlay(string playId, string key)
        {
            if(instance.clipCollection.ContainsKey(key))
            {
                SAudioClip clip;
                instance.clipCollection.TryGetValue(key, out clip);
                channelManager.Play(playId, clip, 0, 1, instance.AudioCallback);
            }
            else
            {
                Debug.Log("SAudioManager: Key could not be found in the current audio package.");
            }
        }
    }
}