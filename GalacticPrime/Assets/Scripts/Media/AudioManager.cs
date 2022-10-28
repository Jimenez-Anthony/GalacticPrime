using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public string background = "Background1";
    public AudioMixerGroup music;
    public AudioMixerGroup sfx;

    public Sound[] sounds;

    void Awake() {
        DontDestroyOnLoad(gameObject);

        // Singleton Pattern
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;

        foreach (Sound sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            if (sound.loop) {
                sound.source.outputAudioMixerGroup = music;
            }
            else {
                sound.source.outputAudioMixerGroup = sfx;
            }
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    void Start() {
        //background = GameMaster.instance.GetComponent<LevelLoader>().level.backgroundMusic;
        //StartBackground();
    }

    public void StartBackground() {
        Play(background);
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogError("Sound: " + name + " cannot be found.");
            return;
        }
        if (s.pitchShift != 0f) {
            s.source.pitch = s.pitch + UnityEngine.Random.Range(-1f, 1f) * s.pitchShift;
        }
        s.source.Play();
    }

    public void Stop(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogError("Sound: " + name + " cannot be found.");
            return;
        }
        s.source.Stop();
    }

    public void SetTime(float time) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogError("Sound: " + name + " cannot be found.");
            return;
        }
        s.source.time = time;
    }

    public void StopSounds() {
        foreach (Sound sound in sounds) {
            sound.source.Stop();
        }
    }

    public void PauseSounds() {
        foreach (Sound sound in sounds) {
            sound.source.Pause();
        }
    }

    public void ResumeSounds() {
        foreach (Sound sound in sounds) {
            sound.source.UnPause();
        }
    }

    public void Mute(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogError("Sound: " + name + " cannot be found.");
        }
        s.source.volume = 0f;
    }

    public void Unmute(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogError("Sound: " + name + " cannot be found.");
        }
        s.source.volume = s.volume;
    }

    public bool IsPlaying(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogError("Sound: " + name + " cannot be found.");
        }
        return s.source.isPlaying;
    }

}
