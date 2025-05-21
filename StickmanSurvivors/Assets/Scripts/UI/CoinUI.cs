using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    public TextMeshProUGUI coinLabel;

    void Start()
    {
        if (coinLabel == null)
            coinLabel = GetComponent<TextMeshProUGUI>();

        // ─────────────  DODAJ sprawdzenie  ─────────────
        if (CurrencySystem.Instance)
        {
            CurrencySystem.Instance.onCoinsChanged.AddListener(UpdateLabel);
            UpdateLabel(CurrencySystem.Instance.Coins);   // init        ← ZMIANA
        }
        else
        {
            UpdateLabel(0);  // awaryjnie wyświetl 0 monet
        }
    }

    private void UpdateLabel(int newTotal)
    {
        coinLabel.text = newTotal.ToString();
    }
}
