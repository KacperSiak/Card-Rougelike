using UnityEngine;

public class Ambient : MonoBehaviour, IAudioHandler
{
    [SerializeField] private AudioSource _ambientSource;

    void Awake()
    {
        Play();
    }

    public void Play()
    {
        _ambientSource.Play();
    }
}
