using System;
using EditorAttributes;
using PrimeTween;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _audioManager;
        public static AudioManager Instance => _audioManager;

        [SerializeField]
        public Sound[] sounds;

        public AudioMixerGroup defaultAudioMixerGroup;

        public void Awake()
        {
            if (_audioManager == null)
            {
                _audioManager = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            
            

            foreach (var s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.outputAudioMixerGroup = s.audioMixerGroup ? s.audioMixerGroup : defaultAudioMixerGroup;
                s.source.resource = s.audioResource;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;

                s.source.loop = s.loop;
                s.source.spatialBlend = s.spatialBlend;
                s.source.playOnAwake = s.playOnAwake;

                if (s.playOnAwake) s.source.Play();
            }
        }

        public void PlaySound(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning($"Sound with name {name} not found!");
                return;
            }

            if (s.source.isPlaying) return;

            s.source.volume = s.volume;
            s.source.pitch = s.randomizePitch ? Random.Range(s.randomPitchRange.x, s.randomPitchRange.y) : s.pitch;
            s.source.Play();
        }

        public void CrossFadeToNewLoopingSound(string name, float duration)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            Sound currentlyPlaying = Array.Find(sounds, x => x.source.loop && x.source.isPlaying);

            s.source.Play();
            Tween.AudioVolume(currentlyPlaying.source, 0f, duration)
                .OnComplete(() => currentlyPlaying.source.Stop());
            Tween.AudioVolume(s.source, 0f, s.volume, duration);
        }


        [Button("TestSound0")]
        public void TestSound()
        {
            PlaySound(sounds[0].name);
        }
    }
}