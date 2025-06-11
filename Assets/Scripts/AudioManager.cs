using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager main;

    public static AudioSource audioSource, musicSource, clickSource, tornadoSource, sirenSource;

    [Header("SOUND EFFECTS")]
    public AudioClip buy;
    public AudioClip construction;
    public AudioClip chop;
    public AudioClip sell;
    public AudioClip explosion;
    public AudioClip click;
    public AudioClip tornado;
    public AudioClip siren;

    [Header("SOUNDTRACKS")]
    public AudioClip music;
    public AudioClip ambience;

    private void Awake()
    {
        main = this;
        audioSource = GetComponent<AudioSource>();
        musicSource = transform.Find("MusicHandler").GetComponent<AudioSource>();
        clickSource = transform.Find("ClickHandler").GetComponent<AudioSource>();
        tornadoSource = transform.Find("TornadoHandler").GetComponent<AudioSource>();
        sirenSource = transform.Find("SirenHandler").GetComponent<AudioSource>();
    }

    public static void PlayAudio(AudioClip clip, float volume = 1f)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

    public static void PlayOverlayAudio(AudioClip clip, float volume = 1f)
    {
        clickSource.clip = clip;
        clickSource.volume = volume;
        clickSource.Play();
    }

    public void PlayTornadoAudio(AudioClip clip, AudioClip secondClip, float volume = 1f)
    {
        tornadoSource.volume = volume;
        tornadoSource.clip = clip;
        tornadoSource.Play();

        sirenSource.volume = 0.13f;
        sirenSource.clip = secondClip;
        sirenSource.Play();
    }

    public static void StartMusic(AudioClip clip, float volume = 1f)
    {
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();
    }

    public IEnumerator AudioFadeOut(AudioSource source, float delay)
    {
        if (source.volume > .01f){

            float newVolume = source.volume - (delay);

            if (newVolume < 0f)
            {
                newVolume = 0f;
            }

            source.volume = newVolume;

            yield return new WaitForSeconds(Time.deltaTime);

            StartCoroutine(AudioFadeOut(source, delay));
        }
        else
        {
            source.Stop();
        }
    }

    
}
