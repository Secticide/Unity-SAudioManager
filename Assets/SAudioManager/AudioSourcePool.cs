// Written by Jak Boulton, 02-02-2016
using UnityEngine;
using System.Collections.Generic;

public class AudioSourcePool
{
    private GameObject poolParent;
    private AudioSource[] pool;
    private Stack<int> freeStack;
    private Dictionary<int, int> indexLookup;

    /// <summary>
    /// Used to initialise the audio source pool with maximum size
    /// </summary>
    /// <param name="size">The maximum size of the pool</param>
    public AudioSourcePool(int size)
    {
        freeStack = new Stack<int>(size);
        indexLookup = new Dictionary<int, int>(size);
        poolParent = new GameObject("AudioSources");
        for(int i = 0; i < size; ++i)
        {
            pool[i] = new GameObject(string.Concat("AudioSource", i)).AddComponent<AudioSource>();
            pool[i].transform.SetParent(poolParent.transform);
            freeStack.Push(i);
            indexLookup.Add(pool[i].GetInstanceID(), i);
        }
    }

    /// <summary>
    /// Request an audio source
    /// </summary>
    /// <returns>An audio source from the stack, or null if there are none</returns>
    public AudioSource Request()
    {
        if(freeStack.Count > 0)
        {
            return pool[freeStack.Pop()];
        }
        return null;
    }

    /// <summary>
    /// Send an audio source back to the stack
    /// </summary>
    /// <param name="source">The audio source to collect</param>
    public void Collect(AudioSource source)
    {
        int index;
        if(indexLookup.TryGetValue(source.GetInstanceID(), out index))
        {
            pool[index].gameObject.transform.SetParent(poolParent.transform);
            pool[index].gameObject.SetActive(false);
            freeStack.Push(index);
        }
    }
}
