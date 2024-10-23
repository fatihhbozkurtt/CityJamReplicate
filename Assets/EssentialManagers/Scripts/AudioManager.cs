using System;
using UnityEngine;

namespace EssentialManagers.Scripts
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance; // Singleton instance

        public Sound[] sounds; // Array to hold all sound clips

        void Awake()
        {
            // Initialize all sounds
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>(); // Add AudioSource for each sound
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }

        // Play a sound by its name
        public void Play(string soundName)
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return;
            }

            s.source.Play();
        }

        // Stop a sound by its name
        public void Stop(string soundName)
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return;
            }

            s.source.Stop();
        }

        // Adjust the volume for a specific sound
        public void SetVolume(string soundName, float volume)
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
                return;
            }

            s.source.volume = volume;
        }
    }
}

[Serializable]
public class Sound
{
    public string name; // Name of the sound

    public AudioClip clip; // Audio clip for the sound

    [Range(0f, 1f)] public float volume = 0.7f; // Default volume

    [Range(0.1f, 3f)] public float pitch = 1f; // Default pitch

    public bool loop; // Should the sound loop?

    [HideInInspector] public AudioSource source; // Audio source assigned at runtime
}