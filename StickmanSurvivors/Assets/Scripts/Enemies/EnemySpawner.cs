using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CompositeCollider2D))]
public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Lista prefabów wrogów: Wolf, Spider, Skeleton itd.")]
    public List<GameObject> enemyPrefabs;

    [Tooltip("Collider definiujący granice całej mapy")]
    public CompositeCollider2D boundsCollider;

    [Tooltip("Jeśli chcesz użyć innej kamery niż Camera.main, przeciągnij ją tutaj")]
    public Camera spawnCamera;

    [Header("Spawn Settings")]
    [Tooltip("Dodatkowy margines poza krawędzią ekranu (w jednostkach world)")]
    public float offscreenMargin = 1f;

    [Header("Spawn Rate Curve")]
    [Tooltip("X = czas od startu (s), Y = spawnów na sekundę")]
    public AnimationCurve spawnRateCurve = AnimationCurve.Linear(0, 0.5f, 60, 2f);

    // --- prywatne ---
    private float elapsedTime = 0f;
    private float timer = 0f;
    private Vector2 mapMin, mapMax;

    void Awake()
    {
        if (boundsCollider == null)
            boundsCollider = GetComponent<CompositeCollider2D>();

        // Cache granice mapy
        Bounds mb = boundsCollider.bounds;
        mapMin = mb.min;
        mapMax = mb.max;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        timer += Time.deltaTime;

        // ile spawnów/s w tej chwili?
        float rate = spawnRateCurve.Evaluate(elapsedTime);
        float interval = (rate > 0f) ? (1f / rate) : float.MaxValue;

        if (timer >= interval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return;

        // Wybór losowego prefab'a
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        if (!prefab) return;

        // Ustal bieżącą “kamerę spawnu”
        Camera cam = spawnCamera != null ? spawnCamera : Camera.main;
        if (cam == null) return;

        // Oblicz granice widoku kamery w world-space
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;
        Vector3 cpos = cam.transform.position;

        float camMinX = cpos.x - halfW - offscreenMargin;
        float camMaxX = cpos.x + halfW + offscreenMargin;
        float camMinY = cpos.y - halfH - offscreenMargin;
        float camMaxY = cpos.y + halfH + offscreenMargin;

        // Zdefiniuj 4 strefy spawnu: lewa, prawa, dolna, górna
        // (tylko tam, gdzie mają sens w obrębie mapy)
        var zones = new List<Rect>();

        // lewa
        if (mapMin.x < camMinX)
            zones.Add(new Rect(
                mapMin.x,
                mapMin.y,
                camMinX - mapMin.x,
                mapMax.y - mapMin.y));

        // prawa
        if (mapMax.x > camMaxX)
            zones.Add(new Rect(
                camMaxX,
                mapMin.y,
                mapMax.x - camMaxX,
                mapMax.y - mapMin.y));

        // dół
        if (mapMin.y < camMinY)
            zones.Add(new Rect(
                Mathf.Max(mapMin.x, camMinX),
                mapMin.y,
                Mathf.Min(mapMax.x, camMaxX) - Mathf.Max(mapMin.x, camMinX),
                camMinY - mapMin.y));

        // góra
        if (mapMax.y > camMaxY)
            zones.Add(new Rect(
                Mathf.Max(mapMin.x, camMinX),
                camMaxY,
                Mathf.Min(mapMax.x, camMaxX) - Mathf.Max(mapMin.x, camMinX),
                mapMax.y - camMaxY));

        if (zones.Count == 0) return;

        // wybierz losową strefę, a w niej losową pozycję
        Rect zone = zones[Random.Range(0, zones.Count)];
        float x = Random.Range(zone.xMin, zone.xMax);
        float y = Random.Range(zone.yMin, zone.yMax);

        Vector3 spawnPos = new Vector3(x, y, 0f);
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    // dla wizualnego debugu w edytorze
    void OnDrawGizmosSelected()
    {
        if (boundsCollider != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(boundsCollider.bounds.center,
                                boundsCollider.bounds.size);
        }
        Camera cam = spawnCamera != null ? spawnCamera : Camera.main;
        if (cam != null)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;
            Vector3 cpos = cam.transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(cpos, new Vector3(halfW * 2, halfH * 2, 0));
        }
    }
}
