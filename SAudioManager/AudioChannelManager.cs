﻿// Written by Jak Boulton, 10-02-2016
using UnityEngine;
using System;
using System.Collections.Generic;

namespace SAudioManager
{
    public class AudioChannelManager
    {
        private AudioSourcePool audioSourcePool;
        private Dictionary<string, List<SAudioSource>> audioChannels;

        public AudioChannelManager(int poolSize)
        {
            audioSourcePool = new AudioSourcePool(poolSize);
            audioChannels = new Dictionary<string, List<SAudioSource>>();
        }

        public void Play(string playId, SAudioClip clip, ulong delay, float volume, Action<SAudioSource> callback)
        {
            SAudioSource source = audioSourcePool.Request();
            if(source != null)
            {
                Add(source, clip.channel.name);
                source.Play(playId, clip, delay, volume, callback);
            }
        }

        public void Stop(string[] channels = null, bool decay = false, float decayDuration = 1.0f)
        {
            if(channels == null)
            {
                List<string> channelKeys = new List<string>(audioChannels.Keys);

                for(int i = 0; i < channelKeys.Count; ++i)
                {
                    StopChannel(channelKeys[i], decay, decayDuration);
                }
            }
            else if(channels.Length > 0)
            {
                for(int i = 0; i < channels.Length; ++i)
                {
                    if(!string.IsNullOrEmpty(channels[i]))
                    {
                        StopChannel(channels[i], decay, decayDuration);
                    }
                }
            }
        }

        public void Pause(string[] channels = null)
        {
            if(channels == null)
            {
                List<string> channelKeys = new List<string>(audioChannels.Keys);

                for(int i = 0; i < channelKeys.Length; ++i)
                {
                    PauseChannel(channelKeys[i]);
                }
            }
        }

        public void CompletePlay(SAudioSource source)
        {
            Remove(source, source.channelKey);
            audioSourcePool.Collect(source);
        }

        private void StopChannel(string channel, bool decay, float decayDuration)
        {
            if(audioChannels != null && audioChannels.Count > 0)
            {
                if(audioChannels.ContainsKey(channel))
                {
                    List<SAudioSource> channelSources;
                    audioChannels.TryGetValue(channel, out channelSources);

                    if(channelSources != null && channelSources.Count > 0)
                    {
                        for(int i = 0; i < channelSources.Count; ++i)
                        {
                            channelSources[i].Stop(decay, decayDuration);
                        }
                    }
                }
            }
        }

        private void PauseChannel(string channel)
        {
            if(audioChannels != null && audioChannels.Count > 0)
            {
                if(audioChannels.ContainsKey(channel))
                {
                    List<SAudioSource> channelSources;
                    audioChannels.TryGetValue(channel, out channelSources);

                    if(channelSources != null && channelSources.Count > 0)
                    {
                        for(int i = 0; i < channelSources.Count; ++i)
                        {
                            channelSources[i].Pause();
                        }
                    }
                }
            }
        }

        private void Add(SAudioSource source, string channelKey)
        {
            if(audioChannels.ContainsKey(channelKey))
            {
                List<SAudioSource> channel;
                audioChannels.TryGetValue(channelKey, out channel);
                channel.Add(source);
            }
            else
            {
                List<SAudioSource> channel = new List<SAudioSource>();
                channel.Add(source);
                audioChannels.Add(channelKey, channel);
                UpdateChannelAudio();
            }
        }

        private void Remove(SAudioSource source, string channelKey)
        {
            if(audioChannels.ContainsKey(source.channelKey))
            {
                List<SAudioSource> channel;
                audioChannels.TryGetValue(source.channelKey, out channel);
                channel.Remove(source);

                if(channel.Count == 0)
                {
                    audioChannels.Remove(source.channelKey);
                    UpdateChannelAudio();
                }
            }
        }

        private void UpdateChannelAudio()
        {

        }
    }
}