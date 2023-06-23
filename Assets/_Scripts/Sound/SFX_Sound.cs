using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFX_Sound : MonoBehaviour
{

    [SerializeField] private AudioSource _audioSource;

    public void Setup(AudioClip clip)
    {
        transform.name = $"[SFX_Sound] - {clip.name}";

        _audioSource.loop = false;

        _audioSource.spatialBlend = 0f;

        _audioSource.clip = clip;

        _audioSource.Play();

        MonoBehaviourHelper.StartCoroutine(WaitSound());
    }

    private IEnumerator WaitSound()
    {
        while(_audioSource.isPlaying)
            yield return null;

        Manager_Events.Sound.OnReleaseSfx.Notify(this);
    }

}
