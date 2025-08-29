using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private bool isMuted = false;
    private float lastMusicVolume = 1f;
    private float lastSFXVolume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume)
    {
        
        if (isMuted)
        {
            lastMusicVolume = volume;
            return;
        }
         
        lastMusicVolume = volume;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    public void SetSFXVolume(float volume)
    {

        if (isMuted)
        {
            lastSFXVolume = volume;
            return;
        }

        lastSFXVolume = volume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f);
    }

    public void ToggleMute(bool mute)
    {
        isMuted = mute;

        if (mute)
        {
            audioMixer.SetFloat("MusicVolume", -80f);
            audioMixer.SetFloat("SFXVolume", -80f);
        }
        else
        {
            SetMusicVolume(lastMusicVolume);
            SetSFXVolume(lastSFXVolume);
        }
    }

    public float GetMusicVolume() => lastMusicVolume;
    public float GetSFXVolume() => lastSFXVolume;
    public bool IsMusicMuted() => isMuted;
}
