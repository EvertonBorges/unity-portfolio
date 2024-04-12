using System;

[Serializable]
public class SO_Dialog
{
    
    public InspectorEvent _preEvent;
    public string _dialog;
    public string _ptBrDialog;
    public InspectorEvent _postEvent;

    public string GetDialog()
    {
        var language = Manager_Settings.Instance.LanguageIndex;

        return language switch
        {
            1 => _ptBrDialog,
            _ => _dialog,
        };
    }

}