﻿// Written by Jak Boulton, 04-02-2016
using UnityEngine;
using System;

namespace SAudioManager
{
    public class AudioSourceController : MonoBehaviour
    {
        // ATTRIBUTES
        private AudioClip audioClip;
        private AudioSource audioSource;
        private Action<AudioSourceController, int> completeCallback;

        private bool paused = false;

        private bool decaying = false;
        private float initialVolume = 1.0f;
        private float decayDuration = 0.0f;
        private float decayTimer = 0.0f;

        private int queueIndex = -1;

        // METHODS

        public AudioSource source
        {
            get { return audioSource; }
        }

        /// <summary>
        /// Play function to play the clip with a collection of settings
        /// </summary>
        /// <param name="audioClip">Audio clip to play</param>
        /// <param name="delay">Delay before the audio is played</param>
        /// <param name="volume">Volume of the audio source</param>
        /// <param name="callback"></param>
        public void Play(AudioClip audioClip, ulong delay = 0, float volume = 1.0f, Action<AudioSourceController, int> callback = null, int queue = -1)
        {
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            audioSource.Play(delay);
            initialVolume = volume;
            completeCallback = callback;
            queueIndex = queue;
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
                    completeCallback(this, queueIndex);
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
            queueIndex = -1;
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
                            completeCallback(this, queueIndex);
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
                        completeCallback(this, queueIndex);
                    }
                }
            }
        }
    }
}