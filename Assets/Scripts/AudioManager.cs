using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager main;

    private static AudioSource audioSource, musicSource;

    [Header("SOUND EFFECTS")]
    public AudioClip buy;
    public AudioClip construction;
    public AudioClip chop;
    public AudioClip sell;
    public AudioClip explosion;
    public AudioClip click;

    [Header("SOUNDTRACKS")]
    public AudioClip music;
    public AudioClip ambience;

    private void Awake()
    {
        main = this;
        audioSource = GetComponent<AudioSource>();
        musicSource = transform.Find("MusicHandler").GetComponent<AudioSource>();
    }

    public static void PlayAudio(AudioClip clip, float volume = 1f)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

    public static void PlayOverlayAudio(AudioClip clip, float volume = 1f)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.PlayOneShot(clip);
    }

    public static void StartMusic(AudioClip clip, float volume = 1f)
    {
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();
    }

    
}
