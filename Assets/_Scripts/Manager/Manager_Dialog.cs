using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Manager_Dialog : Singleton<Manager_Dialog>
{

    [SerializeField] private TextMeshProUGUI _txtCharacter;
    [SerializeField] private TextMeshProUGUI _txtDialog;

    [SerializeField] private UI_FadeEffect _fadeEffect;

    private bool m_show = false;
    public bool Show => m_show;

    private readonly List<string> m_dialogs = new();

    protected override void StartInit()
    {
        base.StartInit();

        DontDestroyOnLoad(gameObject);

        _fadeEffect.HideForced();
    }

    private void ShowDialog(string[] dialogs)
    {
        m_dialogs.Clear();

        m_dialogs.AddRange(dialogs);

        if (m_dialogs.IsEmpty())
            return;

        NextDialog();

        _fadeEffect.FadeIn();
    }

    private void NextDialog()
    {
        if (m_dialogs.IsEmpty())
        {
            HideDialog();

            return;
        }

        var dialog = m_dialogs[0];

        m_dialogs.RemoveAt(0);

        _txtDialog.SetText(dialog);

        m_show = true;
    }

    private void HideDialog()
    {
        _fadeEffect.FadeOut();

        m_show = false;
    }

    void OnEnable()
    {
        Manager_Events.Dialog.ShowDialog += ShowDialog;

        Manager_Events.Dialog.NextDialog += NextDialog;
    }

    void OnDisable()
    {
        Manager_Events.Dialog.ShowDialog -= ShowDialog;

        Manager_Events.Dialog.NextDialog -= NextDialog;
    }

}
