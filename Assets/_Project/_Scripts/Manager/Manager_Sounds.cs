using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class Manager_Sounds : Singleton<Manager_Sounds>
{

    [SerializeField] private SFX_Sound _prefabSfxSound;

    private readonly List<SFX_Sound> m_ambientSounds = new();
    private readonly List<SFX_Sound> m_musicSounds = new();
    private readonly List<SFX_Sound> m_sfxSounds = new();

    private ObjectPool<SFX_Sound> _poolSfxSounds;

    protected override void Init()
    {
        base.Init();

        DontDestroyOnLoad(gameObject);

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

    private void OnPlaySfx(SO_Sound so_sound)
    {
        var sfx = _poolSfxSounds.Get();

        switch(so_sound.type)
        {
            case SO_SoundType.MUSIC: OnReleaseByType(SO_SoundType.MUSIC); m_musicSounds.Add(sfx); break;
            case SO_SoundType.AMBIENT: m_ambientSounds.Add(sfx); break;
            case SO_SoundType.SFX: m_sfxSounds.Add(sfx); break;
            default: break;
        }

        sfx.Setup(so_sound);
    }

    private void OnReleaseByType(SO_SoundType type)
    {
        List<SFX_Sound> list = type switch
        {
            SO_SoundType.AMBIENT => m_ambientSounds,
            SO_SoundType.SFX => m_sfxSounds,
            _ => m_musicSounds,
        };

        var listToRelease = new List<SFX_Sound>();

        foreach (var sfxSound in list)
        {
            if (sfxSound.Sound == null || sfxSound.Sound.type != type)
                continue;

            listToRelease.Add(sfxSound);
        }

        foreach (var sfxSound in listToRelease)
        {
            sfxSound.StopSound();

            OnReleaseSfx(sfxSound);
        }
    }

    private void OnReleaseSfx(SFX_Sound sfx)
    {
        _poolSfxSounds.Release(sfx);
    }

    void OnEnable()
    {
        Manager_Events.Sound.OnPlay += OnPlaySfx;
        Manager_Events.Sound.OnReleaseSfx += OnReleaseSfx;
        Manager_Events.Sound.OnReleaseByType += OnReleaseByType;
    }

    void OnDisable()
    {
        Manager_Events.Sound.OnPlay -= OnPlaySfx;
        Manager_Events.Sound.OnReleaseSfx -= OnReleaseSfx;
        Manager_Events.Sound.OnReleaseByType -= OnReleaseByType;
    }


}
