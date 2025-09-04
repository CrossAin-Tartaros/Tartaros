using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    private AudioSource _audioSource;
    private bool dontDestroy;

    public AudioSource Play(AudioClip clip, float soundEffectVolume, bool loop, bool dontDestroy = false)
    {
        this.dontDestroy = dontDestroy;
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();

        CancelInvoke();
        _audioSource.clip = clip;
        _audioSource.volume = soundEffectVolume;
        _audioSource.loop = loop;
        _audioSource.Play();

        if(!loop) Invoke("Disable", clip.length + 2);
        return _audioSource;
    }

    public void Disable()
    {
        _audioSource?.Stop();
        if(!dontDestroy)
            Destroy(this.gameObject);
    }
}
