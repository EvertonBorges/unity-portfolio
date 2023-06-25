using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFX_Sound : MonoBehaviour
{

    [SerializeField] private AudioSource _audioSource;

    public void Setup(SO_Sound sound)
    {
        var clip = sound.clip[Random.Range(0, sound.clip.Length)];

        transform.name = $"[{sound.type}_Sound] - {clip.name}";

        _audioSource.loop = sound.loop;

        _audioSource.spatialBlend = sound.spatialBlend ? 1f : 0f;

        _audioSource.clip = clip;

        _audioSource.pitch = sound.pitch;

        _audioSource.volume = Mathf.Clamp01(sound.volume);

        _audioSource.Play();

        if (!sound.loop)
            MonoBehaviourHelper.StartCoroutine(WaitSound());
    }

    private IEnumerator WaitSound()
    {
        while(_audioSource.isPlaying)
            yield return null;

        Manager_Events.Sound.OnReleaseSfx.Notify(this);
    }

}
