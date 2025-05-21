using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PassiveShopUI : MonoBehaviour
{
    [Header("Setup")]
    public PassiveUpgrade[] upgrades;   // 5 ScriptableObjectów
    public GameObject itemPrefab;       // PassiveItem.prefab
    public Transform contentRoot;       // PassivePanel (Vertical Layout)

    private Dictionary<string, UIItem> _items = new();

    class UIItem
    {
        public PassiveUpgrade data;
        public TextMeshProUGUI title, desc, price;
        public Button buyBtn;
    }

    void Start()
    {
        BuildList();
        if (CurrencySystem.Instance)                       // CHANGED
            CurrencySystem.Instance.onCoinsChanged.AddListener(_ => RefreshAll());
    }

    /* ---------- budowanie listy ---------- */
    void BuildList()
    {
        foreach (var p in upgrades)
        {
            var go = Instantiate(itemPrefab, contentRoot);
            var ui = new UIItem
            {
                data = p,
                title = go.transform.Find("TextBlock/Title").GetComponent<TextMeshProUGUI>(),
                desc = go.transform.Find("TextBlock/Desc").GetComponent<TextMeshProUGUI>(),
                price = go.transform.Find("BuyArea/PriceLabel").GetComponent<TextMeshProUGUI>(),
                buyBtn = go.transform.Find("BuyArea/BuyBtn").GetComponent<Button>()
            };

            ui.title.text = p.title;
            ui.desc.text = p.description;
            go.transform.Find("Icon").GetComponent<Image>().sprite = p.icon;

            ui.buyBtn.onClick.AddListener(() => TryBuy(ui));
            _items.Add(p.id, ui);
        }
        RefreshAll();
    }

    /* ---------- odświeżanie / zakup ---------- */
    void RefreshAll()
    {
        int coins = CurrencySystem.Instance ?                  // CHANGED
                    CurrencySystem.Instance.Coins : 0;         // CHANGED

        foreach (var ui in _items.Values)
        {
            int lvl = SaveData.GetLvl(ui.data.id);
            bool max = lvl >= ui.data.maxLevel;
            int cost = max ? 0 : ui.data.price[lvl];

            ui.price.text = max ? "MAX" : $"{cost}";
            ui.buyBtn.interactable = !max && coins >= cost;    // CHANGED
        }
    }

    void TryBuy(UIItem ui)
    {
        if (!CurrencySystem.Instance) return;                  // CHANGED

        int lvl = SaveData.GetLvl(ui.data.id);
        if (lvl >= ui.data.maxLevel) return;

        int cost = ui.data.price[lvl];
        if (CurrencySystem.Instance.Coins < cost) return;      // CHANGED

        CurrencySystem.Instance.AddCoins(-cost);   // zapłać
        SaveData.SetLvl(ui.data.id, lvl + 1);      // zapisz poziom
        RefreshAll();
    }
}
