using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public GameObject eventWolfPrefab;     // EventWolf prefab
    public Transform wolvesContainer;      // Parent for spawned wolves
    public int packSize = 10;
    public float minInterval = 20f;
    public float maxInterval = 40f;
    public float minSpawnDist = 8f;
    public float maxSpawnDist = 12f;

    [Header("Runtime Settings")]
    [Tooltip("Assign your map's CompositeCollider2D here at runtime.")]
    public CompositeCollider2D mapBounds;

    [Header("UI & Audio")]
    public CanvasGroup overlay;            // EventOverlay CanvasGroup
    public float overlayFadeTime = 0.5f;
    public float overlayTargetAlpha = 0.5f;
    public AudioSource howlSource;         // Howl AudioSource

    void Start()
    {
        StartCoroutine(EventLoop());
    }

    IEnumerator EventLoop()
    {
        while (true)
        {
            // Wait a random interval
            float delay = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(delay);

            // Darken screen & play howl
            yield return FadeOverlay(0f, overlayTargetAlpha);
            howlSource.Play();
            yield return new WaitForSeconds(howlSource.clip.length);

            // Spawn the wolf pack
            SpawnWave();

            // Lighten screen
            yield return FadeOverlay(overlayTargetAlpha, 0f);
        }
    }

    IEnumerator FadeOverlay(float from, float to)
    {
        float t = 0f;
        while (t < overlayFadeTime)
        {
            t += Time.deltaTime;
            overlay.alpha = Mathf.Lerp(from, to, t / overlayFadeTime);
            yield return null;
        }
        overlay.alpha = to;
    }

    void SpawnWave()
    {
        Vector2 playerPos = PlayerController.Instance.transform.position;
        Camera cam = Camera.main;

        for (int i = 0; i < packSize; i++)
        {
            Vector2 spawnPos;
            Vector3 vp;
            do
            {
                Vector2 dir = Random.insideUnitCircle.normalized;
                float dist = Random.Range(minSpawnDist, maxSpawnDist);
                spawnPos = playerPos + dir * dist;
                vp = cam.WorldToViewportPoint(spawnPos);
            } while (vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f);

            Vector2 moveDir = (playerPos - spawnPos).normalized;
            var go = Instantiate(eventWolfPrefab, spawnPos, Quaternion.identity, wolvesContainer);
            var mover = go.GetComponent<EventWolf>();
            mover.moveDirection = moveDir;
            mover.boundsCollider = mapBounds;
        }
    }
}
