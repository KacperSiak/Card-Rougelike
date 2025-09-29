using UnityEngine;

public class AttackSFX : SFX
{
    protected override void Awake()
    {
        base.Awake();
        _sfxClips = Resources.LoadAll<AudioClip>("AudioResources/SFX/Cards/Attack");
    }
}
