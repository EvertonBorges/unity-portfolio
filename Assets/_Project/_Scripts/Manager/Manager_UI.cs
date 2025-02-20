using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Manager_UI : Singleton<Manager_UI>
{

    [SerializeField] private TextMeshProUGUI _txtCoin;
    [SerializeField] private GameObject _ctnSettings;
    [SerializeField] private GameObject _ctnFeedback;

    [SerializeField] private Button _btnShowFeedback;
    [SerializeField] private Button _btnCloseSendFeedback;

    protected override void Init()
    {
        base.Init();

        _btnShowFeedback.onClick.RemoveAllListeners();
        _btnShowFeedback.onClick.AddListener(BTN_ShowFeedback);

        _btnCloseSendFeedback.onClick.RemoveAllListeners();
        _btnCloseSendFeedback.onClick.AddListener(BTN_CloseSendFeedback);

        DontDestroyOnLoad(gameObject);
    }

    protected override void StartInit()
    {
        base.StartInit();

        UpdateCoins(0);
        OnPause(false);
    }

    private void UpdateCoins(int value)
    {
        _txtCoin.text = value.ToString();
    }

    private void OnPause(bool value)
    {
        _ctnSettings.SetActive(value);
        _ctnFeedback.SetActive(false);
    }

    private void BTN_ShowFeedback()
    {
        _ctnSettings.SetActive(false);
        _ctnFeedback.SetActive(true);
    }

    private void BTN_CloseSendFeedback()
    {
        _ctnSettings.SetActive(true);
        _ctnFeedback.SetActive(false);
    }

    void OnEnable()
    {
        Manager_Events.UI.UpdateCoins += UpdateCoins;
        Manager_Events.UI.OnPause += OnPause;
    }

    void OnDisable()
    {
        Manager_Events.UI.UpdateCoins -= UpdateCoins;
        Manager_Events.UI.OnPause -= OnPause;
    }

}
