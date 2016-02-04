﻿using UnityEngine;
using System;

namespace SAudioManager
{
    public class AudioSourceController : MonoBehaviour
    {
        // ATTRIBUTES
        private AudioClip audioClip;
        private AudioSource audioSource;
        private Action completeCallback;

        private bool paused = false;

        private bool decaying = false;
        private float initialVolume = 1.0f;
        private float decayDuration = 0.0f;
        private float decayTimer = 0.0f;

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
        public void Play(AudioClip audioClip, ulong delay = 0, float volume = 1.0f, Action callback = null)
        {
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
        /// <param name="decayVolume"></param>
        /// <param name="decayDuration"></param>
        public void Stop(bool decayVolume = false, float decayDuration = 1.0f)
        {
            if(!decayVolume) {
                audioSource.Stop();
                if(completeCallback != null) {
                    completeCallback();
                }
            } else {
                decaying = true;
            }
        }

        // INTERNAL
        protected void Awake()
        {
            audioSource = this.gameObject.AddComponent<AudioSource>();
        }

        protected void OnEnable()
        {
            audioSource.volume = 1.0f;
            audioSource.clip = null;
            decayTimer = 0.0f;
            decayDuration = 0.0f;
            decaying = false;
            paused = false;
        }

        protected void Update()
        {
            if(!paused) {
                if(decaying) {
                    decayTimer += Time.deltaTime;
                    if(decayTimer >= decayDuration) {
                        audioSource.Stop();
                        if(completeCallback != null) {
                            completeCallback();
                        }
                    }
                    else
                    {
                        audioSource.volume = Mathf.Lerp(initialVolume, 0.0f, decayTimer / decayDuration);
                    }
                } else if(audioSource.isPlaying && completeCallback != null) {
                    completeCallback();
                }
            }
        }
    }
}