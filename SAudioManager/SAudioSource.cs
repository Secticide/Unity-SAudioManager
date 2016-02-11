﻿// Written by Jak Boulton, 04-02-2016
using UnityEngine;
using System;

namespace SAudioManager
{
    public class SAudioSource : MonoBehaviour
    {
        // ATTRIBUTES
        private string playId = string.Empty;
        private string channelKey = string.Empty;
        private AudioClip audioClip;
        private AudioSource audioSource;
        private Action<SAudioSource> completeCallback;

        private bool paused = false;
        private bool dimmed = false;

        private bool decaying = false;
        private float initialVolume = 1.0f;
        private float decayDuration = 0.0f;
        private float decayTimer = 0.0f;

        // METHODS

        public AudioSource source
        {
            get { return audioSource; }
        }

        public string id
        {
            get { return playId; }
        }

        public string channel
        {
            get { return channelKey; }
        }

        /// <summary>
        /// Play function to play the clip with a collection of settings
        /// </summary>
        /// <param name="audioClip">Audio clip to play</param>
        /// <param name="delay">Delay before the audio is played</param>
        /// <param name="volume">Volume of the audio source</param>
        /// <param name="callback"></param>
        public void Play(string id, string channel, AudioClip audioClip, ulong delay = 0, float volume = 1.0f, Action<SAudioSource> callback = null)
        {
            playId = id;
            channelKey = channel;
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play(delay);
            initialVolume = volume;
            completeCallback = callback;
        }

        /// <summary>
        /// Pause the audio source playback
        /// </summary>
        public void Pause()
        {
            paused = true;
            audioSource.Pause();
        }

        /// <summary>
        /// Unpause the audio source playback
        /// </summary>
        public void UnPause()
        {
            paused = false;
            audioSource.UnPause();
        }

        /// <summary>
        /// Stop the audio source playback with optional decay
        /// </summary>
        /// <param name="decayVolume">Decay audio before stop</param>
        /// <param name="decayDuration">The duration of the decay</param>
        public void Stop(bool decayVolume = false, float decayDuration = 1.0f)
        {
            if(!decayVolume)
            {
                audioSource.Stop();
                audioSource.clip = null;
                if(completeCallback != null)
                {
                    completeCallback(this);
                }
            }
            else
            {
                decaying = true;
            }
        }

        // INTERNAL
        protected void OnEnable()
        {
            if(audioSource == null)
            {
                audioSource = this.gameObject.AddComponent<AudioSource>();
            }

            audioSource.volume = 1.0f;
            decayTimer = 0.0f;
            decayDuration = 0.0f;
            playId = string.Empty;
            channelKey = string.Empty;
            decaying = false;
            paused = false;
        }

        protected void Update()
        {
            if(!paused)
            {
                if(decaying)
                {
                    decayTimer += Time.deltaTime;
                    if(decayTimer >= decayDuration)
                    {
                        audioSource.Stop();
                        audioSource.clip = null;
                        if(completeCallback != null)
                        {
                            completeCallback(this);
                        }
                    }
                    else
                    {
                        audioSource.volume = Mathf.Lerp(initialVolume, 0.0f, decayTimer / decayDuration);
                    }
                }
                else if(!audioSource.isPlaying)
                {
                    audioSource.clip = null;
                    if(completeCallback != null)
                    {
                        completeCallback(this);
                    }
                }
            }
        }
    }
}