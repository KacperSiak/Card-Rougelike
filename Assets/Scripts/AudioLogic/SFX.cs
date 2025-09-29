using UnityEngine;

public class SFX : MonoBehaviour, IAudioHandler
{
    protected AudioSource _sfxSource;
    protected AudioClip[] _sfxClips;
    protected int _currentClipIndex = 0;

    protected virtual void Awake()
    {
        _sfxSource = GetComponent<AudioSource>();
    }

    public virtual void Play()
    {
        _sfxSource.PlayOneShot(_sfxClips[_currentClipIndex]);
    }

}
