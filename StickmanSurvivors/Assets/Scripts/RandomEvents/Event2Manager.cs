using System.Collections;
using UnityEngine;

public class Event2Manager : MonoBehaviour
{
    [Header("Spider Wave Settings")]
    public GameObject eventSpiderPrefab;     // Assign EventSpider prefab
    public Transform spidersContainer;       // Assign Event2_Spiders
    public int spiderCount = 12;             // How many in the ring
    public float spawnRadius = 8f;           // Distance from player

    [Header("Chest Reward Settings")]
    public GameObject chestPrefab;           // Your Chest prefab
    public Transform chestContainer;         // Assign Event2_Chests
    public int minGold = 25;
    public int maxGold = 50;

    [Header("Timing")]
    public float minInterval = 240f;
    public float maxInterval = 300f;

    private Transform _player;

    void Start()
    {
        _player = PlayerController.Instance.transform;
        StartCoroutine(EventLoop());
    }

    IEnumerator EventLoop()
    {
        while (true)
        {
            // 1) Wait a random interval
            float delay = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(delay);

            // 2) Spawn ring of spiders
            SpawnSpiderRing();

            // 3) Wait until all spiders are dead
            yield return new WaitUntil(() => spidersContainer.childCount == 0);

            // 4) Award chest
            SpawnRewardChest();
        }
    }

    void SpawnSpiderRing()
    {
        Vector2 center = _player.position;
        for (int i = 0; i < spiderCount; i++)
        {
            float angle = i * Mathf.PI * 2f / spiderCount;
            Vector2 pos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
            Instantiate(eventSpiderPrefab, pos, Quaternion.identity, spidersContainer);
        }
    }

    void SpawnRewardChest()
    {
        Vector3 chestPos = _player.position + Vector3.up * 1f; // next to player
        var chest = Instantiate(chestPrefab, chestPos, Quaternion.identity, chestContainer)
                    .GetComponent<Chest>();
        chest.minCoins = minGold;
        chest.maxCoins = maxGold;
    }
}
