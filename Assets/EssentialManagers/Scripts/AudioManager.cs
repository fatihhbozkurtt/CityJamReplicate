using System;
using System.Collections;
using Data;
using UnityEngine;

namespace EssentialManagers.Scripts
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        public SoundData[] sounds; // Array to hold all sound clips

        protected override void Awake()
        {
            base.Awake();

            // Initialize all sounds
            foreach (SoundData s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>(); // Add AudioSource for each sound
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
                s.source.spatialBlend = 0f;
                s.source.playOnAwake = false;
            }
        }

        // Play a sound by its name
        public void Play(SoundTag soundTag, float delay = 0)
        {
            SoundData s = Array.Find(sounds, sound => sound.tag == soundTag);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + soundTag + " not found!");
                return;
            }

            IEnumerator PlayRoutine()
            {
                yield return new WaitForSeconds(delay);

                s.source.Play();
            }

            StartCoroutine(PlayRoutine());
        }

        // Stop a sound by its name
        public void Stop(SoundTag soundTag)
        {
            SoundData s = Array.Find(sounds, sound => sound.tag == soundTag);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + soundTag + " not found!");
                return;
            }

            s.source.Stop();
        }

        // Adjust the volume for a specific sound
        public void SetVolume(SoundTag soundTag, float volume)
        {
            SoundData s = Array.Find(sounds, sound => sound.tag == soundTag);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + soundTag + " not found!");
                return;
            }

            s.source.volume = volume;
        }
    }
}

[Serializable]
public class SoundData
{
    public SoundTag tag; // Tag of the sound

    public AudioClip clip; // Audio clip for the sound

    [Range(0f, 1f)] public float volume = 0.7f; // Default volume

    [Range(0.1f, 3f)] public float pitch = 1f; // Default pitch

    public bool loop; // Should the sound loop?

    [HideInInspector] public AudioSource source; // Audio source assigned at runtime
}