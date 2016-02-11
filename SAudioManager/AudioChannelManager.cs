// Written by Jak Boulton, 10-02-2016
using UnityEngine;
using System;
using System.Collections.Generic;
using SAudioManager;

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

        public void Play(string playId, string channelKey, AudioClip clip, ulong delay, float volume, Action<SAudioSource> callback)
        {
            SAudioSource source = audioSourcePool.Request();
            if(source != null)
            {
                Add(source);
                source.Play(playId, channelKey, clip, delay, volume, callback);
            }
        }

        public void CompletePlay(SAudioSource source)
        {
            Remove(source);
            audioSourcePool.Collect(source);
        }

        private void Add(SAudioSource source)
        {
            if(audioChannels.ContainsKey(source.channel))
            {
                List<SAudioSource> channel;
                audioChannels.TryGetValue(source.channel, out channel);
                channel.Add(source);
            }
            else
            {
                List<SAudioSource> channel = new List<SAudioSource>();
                channel.Add(source);
                audioChannels.Add(source.channel, channel);
                UpdateChannelAudio();
            }
        }

        private void Remove(SAudioSource source)
        {
            if(audioChannels.ContainsKey(source.channel))
            {
                List<SAudioSource> channel;
                audioChannels.TryGetValue(source.channel, out channel);
                channel.Remove(source);

                if(channel.Count == 0)
                {
                    audioChannels.Remove(source.channel);
                    UpdateChannelAudio();
                }
            }
        }

        private void UpdateChannelAudio()
        {

        }
    }
}