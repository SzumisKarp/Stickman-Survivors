using UnityEngine;

public class PassiveEffectsManager : MonoBehaviour
{
    [Header("Per-Level Bonuses")]
    public float speedPerLevel = 1f;
    public int healthPerLevel = 1;
    public int regenPerLevel = 1;
    public float regenInterval = 1f;

    [Header("Passive #2 Bonuses")]
    public int expPerLevel = 1;               // ile dodatkowego EXP dajemy za poziom
    public float magnetRadiusPerLevel = 1f;   // dodatkowy zasięg magnesu na poziom

    void Start()
    {
        // 1) Speed Boost
        int speedLvl = SaveData.GetLvl("SpeedBoost");
        if (speedLvl > 0 && PlayerController.Instance != null)
            PlayerController.Instance.speed += speedLvl * speedPerLevel;

        // 2) Extra Health
        int healthLvl = SaveData.GetLvl("ExtraHealth");
        if (healthLvl > 0 && PlayerHealth.Instance != null)
        {
            var ph = PlayerHealth.Instance;
            ph.maxHP += healthLvl * healthPerLevel;
            ph.currentHP = Mathf.Min(ph.currentHP + healthLvl * healthPerLevel, ph.maxHP);
        }

        // 3) Regeneration
        int regenLvl = SaveData.GetLvl("Regeneration");
        if (regenLvl > 0 && PlayerHealth.Instance != null)
            StartCoroutine(RegenerationRoutine(regenLvl));

        // istniejące Pasywne #1…
        int expLvl = SaveData.GetLvl("ExpBonus");
        if (expLvl > 0)
            LevelSystem.Instance.AddExpBonus(expLvl * expPerLevel);

        int magLvl = SaveData.GetLvl("Magnet");
        if (magLvl > 0)
            SetupMagnet(magLvl);

    }

    private void SetupMagnet(int lvl)
    {
        var go = new GameObject("PickupMagnet");
        go.transform.SetParent(PlayerController.Instance.transform, false);
        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = lvl * magnetRadiusPerLevel;

        // ← dodaj Rigidbody2D, żeby trigger działał
        var rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;

        go.AddComponent<PickupMagnet>();
    }



    private System.Collections.IEnumerator RegenerationRoutine(int lvl)
    {
        var ph = PlayerHealth.Instance;
        while (ph != null && ph.currentHP > 0)
        {
            yield return new WaitForSeconds(regenInterval);
            ph.Heal(lvl * regenPerLevel);
        }
    }
}
