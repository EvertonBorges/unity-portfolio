using UnityEngine;
using TMPro;

public class Manager_UI : Singleton<Manager_UI>
{

    protected override void Init()
    {
        base.Init();

        DontDestroyOnLoad(gameObject);
    }

    protected override void StartInit()
    {
        base.StartInit();

        UpdateCoins(0);
        OnPause(false);
    }

    [SerializeField] private TextMeshProUGUI _txtCoin;
    [SerializeField] private GameObject _ctnSettings;

    private void UpdateCoins(int value)
    {
        _txtCoin.text = value.ToString();
    }

    private void OnPause(bool value)
    {
        _ctnSettings.SetActive(value);
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
