using System;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [Serializable]
    public class Sound
    {
        public string name;

        public AudioResource audioResource;
        public AudioMixerGroup audioMixerGroup;

        [Range(0f, 1f)]
        public float volume = 0.5f;

        [Tooltip("whether the pitch should be randomly selected from a range or not")]
        public bool randomizePitch;

        [Range(.1f, 3f)]
        [DisableField(nameof(randomizePitch))]
        public float pitch = 1f;

        [MinMaxSlider(.1f, 3f)]
        [EnableField(nameof(randomizePitch))]
        public Vector2 randomPitchRange;

        [Range(0f, 1f)]
        public float spatialBlend = 0f;

        public bool loop = false;
        public bool playOnAwake = false;

        [HideInInspector]
        public AudioSource source;
    }
}