using UnityEngine;

public class ReactionSound : Reaction
{
    [Header("Sound Configuration")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private bool _shouldSoundLoop;
    [SerializeField] private AudioClip _audioClip;

    protected override void React()
    {
        _audioSource.clip = _audioClip;
        _audioSource.loop = _shouldSoundLoop;
        _audioSource.Play();
    }
}
