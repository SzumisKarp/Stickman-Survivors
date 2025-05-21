using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance { get; private set; }

    public int level = 1;
    public int exp = 0;
    public int expToNext = 10;

    private int extraExpThisPickup = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddExpBonus(int bonus)
    {
        extraExpThisPickup = bonus;
    }
    public void AddExp(int amount)
    {
        int bonus = extraExpThisPickup;
        int total = amount + bonus;
        Debug.Log($"[EXP] base={amount}, bonus={bonus}, total={total}");
        extraExpThisPickup = 0;
        exp += total;
        while (exp >= expToNext)
        {
            exp -= expToNext;
            level++;
            expToNext = Mathf.RoundToInt(expToNext * 1.5f); // Example scaling
            Debug.Log($"Level Up! Now level {level}");
            // TODO: Trigger level-up UI, upgrades, etc.
            if (UpgradeChoiceUI.Instance != null)
                UpgradeChoiceUI.Instance.ShowUpgradeChoices();
        }
    }
}
