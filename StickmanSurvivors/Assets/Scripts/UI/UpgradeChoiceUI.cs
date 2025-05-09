using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // for TextMeshProUGUI labels

// A simple data‐holder for one upgrade choice
[System.Serializable]
public class UpgradeOption
{
    public string title;
    [TextArea] public string description;
    public Sprite icon;
}

public class UpgradeChoiceUI : MonoBehaviour
{
    public static UpgradeChoiceUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject panel;            // Drag your UpgradePanel here
    public Button[] buttons;            // Size = 3, assign Button_1…3
    public Image[] icons;               // Optional: assign 3 Image components inside each button
    public TextMeshProUGUI[] labels;    // Assign the TMP label in each button

    [Header("All Upgrades")]
    public List<UpgradeOption> allUpgrades;  // Fill in Inspector: 5–10 entries

    private float _prevTimeScale;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        panel.SetActive(false);
    }

    /// <summary>
    /// Call this on level up to show three random choices.
    /// </summary>
    public void ShowUpgradeChoices()
    {
        // 1) Pause the game:
        _prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        // Also pause physics:
        Time.fixedDeltaTime = 0f;

        // 2) Populate and show UI:
        var pool = new List<UpgradeOption>(allUpgrades);
        for (int i = 0; i < buttons.Length; i++)
        {
            if (pool.Count == 0) break;
            var choice = pool[Random.Range(0, pool.Count)];
            pool.Remove(choice);

            labels[i].text = choice.title;
            if (icons != null && icons.Length > i && icons[i] != null && choice.icon != null)
                icons[i].sprite = choice.icon;

            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => OnUpgradeSelected(choice));
        }

        panel.SetActive(true);
    }

    private void OnUpgradeSelected(UpgradeOption choice)
    {
        ApplyUpgrade(choice);

        // 3) Close UI & resume:
        panel.SetActive(false);
        Time.timeScale = _prevTimeScale;
        Time.fixedDeltaTime = 0.02f;
    }

    private void ApplyUpgrade(UpgradeOption choice)
    {
        Debug.Log($"Applying upgrade: {choice.title}");
        // TODO: implement each upgrade’s effect here,
        // e.g. switch(choice.title) { case "Speed Boost": PlayerController.Instance.speed += 2; break; … }
        switch (choice.title)
        {
            case "Explosive Orbs (Orbit)":
                ExplosiveOrbsUpgrade.Instance.ApplyUpgrade();
                break;
            case "Ink Bullets (Trail)":
                InkBulletsUpgrade.Instance.ApplyUpgrade();
                break;
            case "Flying Drone":
                FlyingDroneUpgrade.Instance.ApplyUpgrade();
                break;
            default:
                Debug.LogWarning($"No handler for upgrade '{choice.title}'");
                break;
        }
    }
}
