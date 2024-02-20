using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading.Tasks;

public class Manager_Dialog : Singleton<Manager_Dialog>
{

    [SerializeField] private TextMeshProUGUI _txtCharacter;
    [SerializeField] private TextMeshProUGUI _txtDialog;

    [SerializeField] private UI_FadeEffect _fadeEffect;

    [SerializeField] private float _timePerCharacter;
    [SerializeField] private float _speedReading = 2f;

    public bool Show { get; private set; } = false;
    public bool Writing { get; private set; } = false;

    private readonly StringBuilder m_stringBuilder = new();
    private bool m_speedRead = false;
    private readonly List<SO_Dialog> m_dialogs = new();

    protected override void StartInit()
    {
        base.StartInit();

        DontDestroyOnLoad(gameObject);

        _fadeEffect.HideForced();
    }

    private void ShowDialog(SO_Dialogs dialogs)
    {
        _txtDialog.SetText("");

        m_dialogs.Clear();

        m_dialogs.AddRange(dialogs.dialogs);

        if (m_dialogs.IsEmpty())
            return;

        Show = true;

        _fadeEffect.FadeIn(NextDialog);
    }

    private async void NextDialog()
    {
        if (Writing)
        {
            m_speedRead = true;

            return;
        }

        if (m_dialogs.IsEmpty())
        {
            HideDialog();

            return;
        }

        var dialog = m_dialogs[0];

        dialog._preEvent.Notify();

        m_dialogs.RemoveAt(0);

        m_speedRead = false;
        Writing = false;

        await ShowDialog(dialog);

        dialog._postEvent.Notify();
    }

    private async Task ShowDialog(SO_Dialog dialog)
    {
        Writing = true;

        m_stringBuilder.Clear();

        foreach (var character in dialog._dialog)
        {
            m_stringBuilder.Append(character);

            _txtDialog.SetText(m_stringBuilder.ToString());

            float delay = _timePerCharacter * 1000 / (m_speedRead ? _speedReading : 1f);

            await Task.Delay(Mathf.FloorToInt(delay));
        }

        Writing = false;
    }

    private void HideDialog()
    {
        _fadeEffect.FadeOut();

        Show = false;
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
