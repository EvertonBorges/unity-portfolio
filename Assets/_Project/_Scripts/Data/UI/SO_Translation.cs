using System;

[Serializable]
public class SO_Translation
{
    
    public string _value;
    public string _ptBrValue;

    public string GetText()
    {
        var language = Manager_Settings.Instance.LanguageIndex;

        return language switch
        {
            1 => _ptBrValue,
            _ => _value,
        };
    }

}
