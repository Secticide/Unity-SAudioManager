// Written by Jak Boulton, 10-02-2016
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

        public void CompletePlay(SAudioSource source)
        {
            Remove(source, source.channelKey);
            audioSourcePool.Collect(source);
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