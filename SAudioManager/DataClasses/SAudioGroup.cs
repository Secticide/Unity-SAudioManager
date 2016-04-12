// Written by Jak Boulton, 04-02-2016
using UnityEngine;
using System;

namespace SAudioManager
{
    [Serializable]
    public class SAudioGroup
    {
        public string key;
        public string[] clipKeys;
        public string[] groupKeys;
    }

    [Serializable]
    public class SAudioClip
    {
    	public string key;
    	public AudioClip clip;
    	public SAudioChannel channel;
    }
}
