using System;
using UnityEngine;
using DG.Tweening;

public class ManageAudio : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;
    private readonly static System.Random rand = new System.Random();
    public static ManageAudio Instance;

    private void Awake() {
        foreach (var sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
        Instance = this;
    }

    public void Play(string name, bool loop = false) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound != null) {
            sound.source.loop = loop;
            sound.source.Play();
        }
    }

    public void Stop(string name) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound != null) {
            sound.source.Stop();
        }
    }

    public void Mute(string name, bool fade) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound != null) {
            if (fade) {
                sound.source.DOFade(0, 0.25f).SetEase(Ease.InOutSine);
            } else {
                sound.source.volume = 0f;
            }
        }
    }

    public void Unmute(string name, bool fade) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if (sound != null) {
            if (fade) {
                sound.source.DOFade(1, 0.25f).SetEase(Ease.InOutSine);
            } else {
                sound.source.volume = 1f;
            }
        }
    }

    public void PlayVariedPitch(string name, float variance) {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        float deltaPitch = (float)rand.NextDouble() * 2 * variance - variance;
        sound.source.pitch = sound.pitch + deltaPitch;

        Play(name);
    }

    [Serializable]
    public class Sound {
        public string name;
        [HideInInspector] public AudioSource source;

        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
        [Range(0.1f, 3f)] public float pitch;
    }
}