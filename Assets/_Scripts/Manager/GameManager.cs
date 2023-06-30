using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    private const int MIN_COINS = 0;
    private const int MAX_COINS = 9999;

    [Header("Musics and Ambients")]
    [SerializeField] private SO_Sound _SO_StartMusic;
    [SerializeField] private SO_Sound[] _SO_StartAmbients;

    [Header("Sounds")]
    [SerializeField] private SO_Sound _SO_Coin;

    private int m_coins = 0;

    protected override void Init()
    {
        base.Init();

        DontDestroyOnLoad(gameObject);
    }

    protected override void StartInit()
    {
        base.StartInit();

        Manager_Events.Sound.OnPlay.Notify(_SO_StartMusic);

        foreach (var so_ambient in _SO_StartAmbients)
            Manager_Events.Sound.OnPlay.Notify(so_ambient);

        Manager_Events.Camera.Events.OnGameStart.Notify();
    }

    private void AddCoin() => AddCoins();

    private void AddCoins(int value = 1)
    {
        m_coins += value;

        if (m_coins >= MAX_COINS)
            m_coins = MAX_COINS;

        Manager_Events.UI.UpdateCoins.Notify(m_coins);

        Manager_Events.Sound.OnPlay.Notify(_SO_Coin);
    }

    private void RemoveCoin() => RemoveCoins();

    private void RemoveCoins(int value = 1)
    {
        m_coins -= value;

        if (m_coins <= MIN_COINS)
            m_coins = MIN_COINS;

        Manager_Events.UI.UpdateCoins.Notify(m_coins);
    }

    void OnEnable()
    {
        Manager_Events.GameManager.AddCoin += AddCoin;
        
        Manager_Events.GameManager.AddCoins += AddCoins;
        
        Manager_Events.GameManager.RemoveCoin += RemoveCoin;

        Manager_Events.GameManager.RemoveCoins += RemoveCoins;
    }

    void OnDisable()
    {
        Manager_Events.GameManager.AddCoin -= AddCoin;

        Manager_Events.GameManager.AddCoins -= AddCoins;

        Manager_Events.GameManager.RemoveCoin -= RemoveCoin;
        
        Manager_Events.GameManager.RemoveCoins -= RemoveCoins;
    }

}
