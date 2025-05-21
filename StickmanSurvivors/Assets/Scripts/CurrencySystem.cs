using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Globalny portfel gracza – przechowuje i zapisuje liczbê monet.
/// Attach w scenie Prototype pod GameManager (lub jako osobny GO ‘CurrencySystem’).
/// </summary>
public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance { get; private set; }

    [Header("Stan monet")]
    [SerializeField]           // podgl¹d w Inspectorze, ale nie public
    private int coins = 0;
    public int Coins => coins; // tylko do odczytu dla innych klas

    [Header("Events")]
    public UnityEvent<int> onCoinsChanged = new UnityEvent<int>();

    const string PREF_KEY = "Coins";

    void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // upewnij siê, ¿e CurrencySystem jest rootem, zanim przykleisz go do sceny
        if (transform.parent != null)
            transform.SetParent(null);

        DontDestroyOnLoad(gameObject);

        // inicjalizacja stanu monet (zamiast z PlayerPrefs, domyœlnie milion)
        coins = 1_000_000;
        onCoinsChanged.Invoke(coins);

        // nadpisanie PlayerPrefs
        PlayerPrefs.SetInt(PREF_KEY, coins);
        PlayerPrefs.Save();
    }

    void Save()
    {
        PlayerPrefs.SetInt(PREF_KEY, coins);
        PlayerPrefs.Save();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Save();
        onCoinsChanged.Invoke(coins);
    }
}
