using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance { get; private set; }

    public int level = 1;
    public int exp = 0;
    public int expToNext = 10;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddExp(int amount)
    {
        exp += amount;
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
