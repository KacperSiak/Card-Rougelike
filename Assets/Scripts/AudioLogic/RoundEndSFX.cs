using UnityEngine;

public class RoundEndSFX : SFX
{
    protected override void Awake()
    {
        base.Awake();
        _sfxClips = Resources.LoadAll<AudioClip>("AudioResources/SFX/RoundEnd");
    }

}
