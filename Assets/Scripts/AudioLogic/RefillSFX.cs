using UnityEngine;

public class RefillSFX : SFX
{
    protected override void Awake()
    {
        base.Awake();
        _sfxClips = Resources.LoadAll<AudioClip>("AudioResources/SFX/Cards/CardTake");
    }

    public override void Play()
    {
        _currentClipIndex = Random.Range(0, _sfxClips.Length);
        base.Play();
    }
}
