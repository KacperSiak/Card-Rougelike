using UnityEngine;

public class ShuffleSFX : SFX
{
    protected override void Awake()
    {
        base.Awake();
        _sfxClips = Resources.LoadAll<AudioClip>("AudioResources/SFX/Cards/Shuffle");
    }
}
