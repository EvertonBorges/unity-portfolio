using UnityEngine;
using UnityEngine.Audio;

public class Manager_Settings : Singleton<Manager_Settings>
{

    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private UI_Settings_Slider _musicSlider;
    [SerializeField] private UI_Settings_Slider _sfxSlider;
    [SerializeField] private UI_Settings_Dropdown _languageDropdown;

    public int LanguageIndex { get; private set; }

    protected override void Init()
    {
        base.Init();

        Manager_Events.Settings.OnChangeMusicVolume += OnChangeMusicVolume;
        Manager_Events.Settings.OnChangeSfxVolume += OnChangeSfxVolume;
        Manager_Events.Settings.OnChangeLanguage += OnChangeLanguage;
    }

    protected override void StartInit()
    {
        base.StartInit();

        LoadLastValues();
    }

    private void LoadLastValues()
    {
        var music = PlayerPrefsUtils.GetFloat(PlayerPrefsUtils.SettingsKeys.VOLUME_MUSIC, 80f);
        var sfx = PlayerPrefsUtils.GetFloat(PlayerPrefsUtils.SettingsKeys.VOLUME_SFX, 80f);
        var language = PlayerPrefsUtils.GetInt(PlayerPrefsUtils.SettingsKeys.LANGUAGE);

        _musicSlider.Setup(music);
        _sfxSlider.Setup(sfx);
        _languageDropdown.Setup(language);

        UpdateMusicVolume(music);
        UpdateSfxVolume(sfx);
        UpdateLanguage(language);
    }

    private void OnChangeMusicVolume(float value)
    {
        UpdateMusicVolume(value);
        PlayerPrefsUtils.SetFloat(PlayerPrefsUtils.SettingsKeys.VOLUME_MUSIC, value);
    }

    private void OnChangeSfxVolume(float value)
    {
        UpdateSfxVolume(value);
        PlayerPrefsUtils.SetFloat(PlayerPrefsUtils.SettingsKeys.VOLUME_SFX, value);
    }

    private void OnChangeLanguage(int value)
    {
        UpdateLanguage(value);
        PlayerPrefsUtils.SetInt(PlayerPrefsUtils.SettingsKeys.LANGUAGE, value);
    }

    private void UpdateMusicVolume(float value)
    {
        UpdateVolume("music", value);
    }

    private void UpdateSfxVolume(float value)
    {
        UpdateVolume("sfx", value);
    }

    private void UpdateVolume(string name, float value)
    {
        _audioMixer.SetFloat(name, value);
    }

    private void UpdateLanguage(int value)
    {
        LanguageIndex = value;
    }

    protected override void OnDestroy()
    {
        Manager_Events.Settings.OnChangeMusicVolume -= OnChangeMusicVolume;
        Manager_Events.Settings.OnChangeSfxVolume -= OnChangeSfxVolume;
        Manager_Events.Settings.OnChangeLanguage -= OnChangeLanguage;

        base.OnDestroy();
    }

}
