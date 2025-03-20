using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    
    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.audioSource = gameObject.AddComponent<AudioSource>();
            s.audioSource.clip = s.audioClip;
            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(String soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s.audioSource.Play();
    }
    
    public void StartPlayingOnLoop(String soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s.audioSource.loop = true;
        if (!s.audioSource.isPlaying)
        {
            s.audioSource.Play();
        }
    }
    
    public void StopPlayingOnLoop(String soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s.audioSource.loop = true;
        if (s.audioSource.isPlaying)
        {
            s.audioSource.Stop();
        }
    }
}
