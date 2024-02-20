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
    }

    [SerializeField] private TextMeshProUGUI _txtCoin;

    private void UpdateCoins(int value)
    {
        _txtCoin.text = value.ToString();
    }

    void OnEnable()
    {
        Manager_Events.UI.UpdateCoins += UpdateCoins;
    }

    void OnDisable()
    {
        Manager_Events.UI.UpdateCoins -= UpdateCoins;
    }

}
