using System.Linq;
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

}
