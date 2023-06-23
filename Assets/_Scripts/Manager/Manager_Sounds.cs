using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(AudioSource))]
public class Manager_Sounds : Singleton<Manager_Sounds>
{

    [Header("Music Parameters")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _musicDefault;
    [SerializeField] private AudioClip _musicArea1;
    [SerializeField] private AudioClip _musicArea2;

    [Header("SFX Parameters")]
    [SerializeField] private SFX_Sound _prefabSfxSound;

    private ObjectPool<SFX_Sound> _poolSfxSounds;

    protected override void Init()
    {
        base.Init();

        DontDestroyOnLoad(gameObject);

        _audioSource ??= GetComponent<AudioSource>();

        _audioSource.clip = _musicDefault;

        _poolSfxSounds = new(
            () => Instantiate(_prefabSfxSound, transform),
            (obj) => obj.gameObject.SetActive(true),
            (obj) => obj.gameObject.SetActive(false),
            (obj) => Destroy(obj.gameObject),
            true,
            2,
            50
        );
    }

    private void OnPlaySfx(AudioClip clip)
    {
        var sfx = _poolSfxSounds.Get();

        sfx.Setup(clip);
    }

    private void OnReleaseSfx(SFX_Sound sfx)
    {
        _poolSfxSounds.Release(sfx);
    }

    void OnEnable()
    {
        Manager_Events.Sound.OnPlaySfx += OnPlaySfx;

        Manager_Events.Sound.OnReleaseSfx += OnReleaseSfx;
    }

    void OnDisable()
    {
        Manager_Events.Sound.OnPlaySfx -= OnPlaySfx;

        Manager_Events.Sound.OnReleaseSfx -= OnReleaseSfx;
    }


}
