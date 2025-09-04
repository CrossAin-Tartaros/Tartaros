using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Sound Settigns")] 
    [SerializeField] private float soundEffectVolume;
    [SerializeField] private float musicVolume;
    public float MusicVolume { get => musicVolume; set=> musicVolume=value;}
    public float SoundEffectVolume { get => soundEffectVolume; set=> soundEffectVolume=value;}
    
    public AudioSource musicAudioSource;
    public AudioClip musicClip;

    [Header("VFX Clips")] 
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip damagedClip;
    [SerializeField] private AudioClip parryClip;

    public GameObject soundSourcePrefab;

    private Dictionary<string, SoundSource> playingSounds = new();

    private void Awake()
    {
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
        PrefCheck();
    }

    private void Start()
    {
        if (TryGetComponent(out AudioSource audioSource))
        {
            musicAudioSource = audioSource;
        }
        else
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();    
        }
        
        ChangeBackGroundMusic(musicClip);
        musicAudioSource.volume = musicVolume;
    }

    public void PrefCheck()
    {
        if (PlayerPrefs.HasKey("BgmVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("BgmVolume");
        }
        else
        {
            musicVolume = 1.0f;
        }

        if (PlayerPrefs.HasKey("SfxVolume"))
        {
            soundEffectVolume = PlayerPrefs.GetFloat("SfxVolume");
        }
        else
        {
            soundEffectVolume = 1.0f;
        }
    }

    public AudioClip GetCurrentBGM()
    {
        return musicAudioSource.clip;
    }
    
    public void BgmSliderChanged(float changedData)
    {
        musicVolume = changedData;
        musicAudioSource.volume = musicVolume;
        PlayerPrefs.SetFloat("BgmVolume", musicVolume);
    }

    public void SfxSliderChanged(float changedData)
    {
        soundEffectVolume = changedData;
        PlayerPrefs.SetFloat("SfxVolume", soundEffectVolume);
    }

    public void ChangeBackGroundMusic(AudioClip clip)
    {
        musicAudioSource.Stop();
        musicAudioSource.clip = clip;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }
    public AudioSource PlayClip(AudioClip clip, bool loop, bool dontDestroy = false)
    {
        if (clip == null)
        {
            Debug.LogError("Clip is null");
            return null;
        }
        GameObject obj = Instantiate(soundSourcePrefab);
        SoundSource soundSource = obj.GetComponent<SoundSource>();
        return soundSource.Play(clip, Instance.SoundEffectVolume, loop, dontDestroy);
    }


    public SoundSource GetPlayingClip(string key)
    {
        playingSounds.TryGetValue(key, out SoundSource clip);
        if (clip != null) return clip.GetComponent<SoundSource>();
        else return null;
    }
    
    public void StartClip(string key, AudioClip clip, bool loop = false)
    {
        if (GetPlayingClip(key) != null) return;
        playingSounds.Add(key, PlayClip(clip, loop, true).GetComponent<SoundSource>());
    }

    public void StopClip(string key)
    {
        if (GetPlayingClip(key) == null) return;
        Destroy(playingSounds[key].gameObject);
        playingSounds.Remove(key);
    }

    public void AttackClip()
    {
        PlayClip(attackClip, false);
    }
    
    public void DamagedClip()
    {
        PlayClip(damagedClip, false);
    }
}
