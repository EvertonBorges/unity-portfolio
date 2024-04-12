using System.Linq;
using UnityEngine;
using TMPro;

public class UI_Text_Translation : MonoBehaviour
{

    [SerializeField] private string _key;
    [SerializeField] private string _preText;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _postText;

    void Start()
    {
        OnChangeLanguage();
    }

    private void OnChangeLanguage()
    {
        var translation = Manager_Settings.Instance.Translations.translations.First(x => x._key == _key);
        var text = _preText + translation.GetText() + _postText;
        _text.SetText(text);
    }

    void OnEnable()
    {
        Manager_Events.UI.OnChangeLanguage += OnChangeLanguage;
    }

    void OnDisable()
    {
        Manager_Events.UI.OnChangeLanguage -= OnChangeLanguage;
    }

}
