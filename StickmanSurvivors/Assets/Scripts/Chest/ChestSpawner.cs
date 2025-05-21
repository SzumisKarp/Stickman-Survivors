using UnityEngine;

/// <summary>
/// „Heartbeat” – co <see cref="spawnInterval"/> s losuje punkt w pierścieniu
/// (min/max-distance od gracza) i próbuje tam postawić skrzynię.
/// </summary>
public class ChestSpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Prefab skrzyni z komponentem Chest.cs")]
    public GameObject chestPrefab;
    [Tooltip("Transform gracza (jeśli puste – skrypt znajdzie tag 'Player')")]
    public Transform player;
    [Tooltip("Rodzic nowych skrzyń (np. pusty GO ‘Chests’)")]
    public Transform chestParent;

    [Header("Spawn parametry")]
    [Min(0.1f)] public float spawnInterval = 30f;
    public float minDistance = 8f;
    public float maxDistance = 18f;
    public LayerMask obstacleMask;
    public int maxAttemptsPerTick = 15;
    public int maxActiveChests = 4;

    private float _timer;

    void Awake()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (chestParent == null)
        {
            var go = GameObject.Find("Chests");
            if (go) chestParent = go.transform;
        }
    }

    void Update()
    {
        if (!player || !chestPrefab) return;

        _timer += Time.deltaTime;
        if (_timer < spawnInterval) return;
        _timer = 0f;

        // limit aktywnych skrzyń
        if (chestParent && chestParent.childCount >= maxActiveChests) return;

        TrySpawn();
    }

    private void TrySpawn()
    {
        for (int i = 0; i < maxAttemptsPerTick; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(minDistance, maxDistance);
            Vector2 pos = (Vector2)player.position + dir * dist;

            // kolizja z przeszkodą → losuj dalej
            if (Physics2D.OverlapCircle(pos, 0.4f, obstacleMask)) continue;

            Instantiate(chestPrefab, pos, Quaternion.identity, chestParent);
            return;   // sukces
        }
        // po maxAttempts nie udało się – spróbujemy w następnym ticku
    }
}
