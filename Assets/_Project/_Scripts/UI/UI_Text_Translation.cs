using System.Linq;
using UnityEngine;
using TMPro;

public class UI_Text_Translation : MonoBehaviour
{

    [SerializeField] private string _key;
    [SerializeField] private string _preText;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _postText;

    void Awake()
    {
        Manager_Events.UI.OnChangeLanguage += OnChangeLanguage;
    }

    void Start()
    {
        OnChangeLanguage();
    }

    private void OnChangeLanguage()
    {
        var translation = Manager_Translation.Instance.FindTranslation(_key);
        var text = _preText + translation + _postText;
        _text.SetText(text);
    }

    void OnDestroy()
    {
        Manager_Events.UI.OnChangeLanguage -= OnChangeLanguage;
    }

}
