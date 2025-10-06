using UnityEngine;

public class DiceSFX : SFX
{
    protected override void Awake()
    {
        base.Awake();
        _sfxClips = Resources.LoadAll<AudioClip>("AudioResources/SFX/Dice");
    }

    public override void Play()
    {
        _currentClipIndex = Random.Range(0, _sfxClips.Length);
        base.Play();
    }
}
