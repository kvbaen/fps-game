using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.output;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    public void Play(string name, GameObject sourceGO = null)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        if (sourceGO != null)
        {
            if (sourceGO.TryGetComponent(out AudioSource audioSource))
            {
                audioSource.Play();
            }
            else
            {
                audioSource = sourceGO.AddComponent<AudioSource>();
                audioSource.clip = s.source.clip;
                audioSource.outputAudioMixerGroup = s.source.outputAudioMixerGroup;

                audioSource.volume = s.source.volume;
                audioSource.pitch = s.source.pitch;
                audioSource.loop = s.source.loop;
                audioSource.playOnAwake = s.source.playOnAwake;
                audioSource.Play();
            }
        }
        else
        {
            s.source.Play();
        }
    }
}
