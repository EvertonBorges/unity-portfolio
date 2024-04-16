using UnityEngine;

public class Manager_Translation : Singleton<Manager_Translation>
{
    
    [SerializeField] private SO_Translations _translations;

    protected override void Init()
    {
        base.Init();
        
        DontDestroyOnLoad(gameObject);
    }

    public string FindTranslation(string key)
    {
        var translation = _translations.translations[key];

        if (translation == null)
            return "";
        
        return translation.GetText();
    }

    public string ConvertText(string text)
    {
        foreach (var key in _translations.translations.Keys)
        {
            var keyConverted = "{" + key + "}";
            if (text.Contains(keyConverted))
            {
                text = text.Replace(keyConverted, FindTranslation(key));
            }
        }

        return text;
    }

}
