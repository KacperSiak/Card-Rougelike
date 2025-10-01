using UnityEngine;

public class Ambient : MonoBehaviour, IAudioHandler
{
    [SerializeField] private AudioSource _ambientSource;

    void Awake()
    {
       if(_ambientSource.clip && !_ambientSource.isPlaying) Play();
    }

    public void AddClip(AudioClip clip)
    {
        _ambientSource.clip = clip;
        Play();
    }

    public void Play()
    {
        _ambientSource.Play();
    }
}
