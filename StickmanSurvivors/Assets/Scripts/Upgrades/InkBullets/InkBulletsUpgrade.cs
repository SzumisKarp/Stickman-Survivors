using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns short-lived “ink splatter” prefabs behind the player on a timed cycle.
/// Add this component to an empty GameObject that you parent under the Player.
/// </summary>
public class InkBulletsUpgrade : MonoBehaviour
{
    public static InkBulletsUpgrade Instance { get; private set; }

    [Header("Setup")]
    public GameObject splatterPrefab;          // prefab with SpriteRenderer + CircleCollider2D + InkSplatter.cs
    public Transform player;                   // drag Player here (or left null to grab singleton)
    public float spawnInterval = .10f;         // how densely to “paint” while active

    [Header("Current level (1‒3)")]
    public int currentLevel = 0;
    public int maxLevel = 3;

    // --- level-specific stats ---
    private readonly float[] cooldown = { 7f, 6f, 5f };
    private readonly float[] activeTime = { 1f, 2f, 3f };
    private readonly float[] groundTime = { 2f, 3f, 4f };
    private readonly int[] dps = { 1, 2, 3 };

    private Coroutine _cycleRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        if (player == null && PlayerController.Instance != null)
            player = PlayerController.Instance.transform;
    }

    /// <summary>Called from UpgradeChoiceUI when the player picks this upgrade.</summary>
    public void ApplyUpgrade()
    {
        if (currentLevel >= maxLevel) return;
        currentLevel++;

        if (_cycleRoutine != null) StopCoroutine(_cycleRoutine);
        _cycleRoutine = StartCoroutine(CycleRoutine());
    }

    private IEnumerator CycleRoutine()
    {
        while (true)
        {
            // 1) wait for cooldown
            yield return new WaitForSeconds(cooldown[currentLevel - 1]);

            // 2) paint for <activeTime> seconds
            float t = 0f;
            while (t < activeTime[currentLevel - 1])
            {
                SpawnSplatter();
                t += spawnInterval;
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    private void SpawnSplatter()
    {
        if (!splatterPrefab || !player) return;

        // utworzenie + przypięcie do obiektu z tym skryptem
        GameObject s = Instantiate(splatterPrefab,
                                   player.position,
                                   Quaternion.identity,
                                   transform);        // <-- NEW

        var ctrl = s.GetComponent<InkSplatter>();
        ctrl.Initialize(dps[currentLevel - 1],
                        groundTime[currentLevel - 1]);
    }

}
