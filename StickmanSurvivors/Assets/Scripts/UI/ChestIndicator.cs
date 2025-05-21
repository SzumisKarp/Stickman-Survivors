using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Chest))]  // lub po prostu zostaw bez – zale¿y od Ciebie
public class ChestIndicator : MonoBehaviour
{
    [Header("References")]
    public GameObject arrowPrefab;
    public Canvas targetCanvas;          // Canvas w trybie Screen Space – Overlay
    public float edgePadding = 40f;      // margines od krawêdzi w px

    Transform _player;
    Camera _cam;
    RectTransform _arrowRT;
    RectTransform _canvasRT;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _cam = Camera.main;
        _canvasRT = targetCanvas.GetComponent<RectTransform>();

        // instancjonujemy strza³kê jako dziecko canvasa
        GameObject go = Instantiate(arrowPrefab, targetCanvas.transform);
        _arrowRT = go.GetComponent<RectTransform>();
    }
    void Awake()
    {
        if (targetCanvas == null)
        {
            // Szuka pierwszego canvasa w trybie Screen Space - Overlay
            foreach (var c in FindObjectsOfType<Canvas>())
                if (c.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    targetCanvas = c;
                    break;
                }
        }
    }

    void Update()
    {
        if (!_player) return;

        Vector3 worldDir = transform.position - _player.position;
        Vector3 viewport = _cam.WorldToViewportPoint(transform.position);

        bool onScreen = viewport.x > 0f && viewport.x < 1f &&
                        viewport.y > 0f && viewport.y < 1f &&
                        viewport.z > 0f;              // przed kamer¹

        _arrowRT.gameObject.SetActive(!onScreen);

        if (onScreen) return;

        // 1. kierunek od œrodka ekranu do skrzyni
        Vector2 dir = new Vector2(viewport.x - 0.5f, viewport.y - 0.5f).normalized;

        // 2. punkt na krawêdzi canvasu (uwzglêdniamy padding)
        float w = _canvasRT.rect.width / 2f - edgePadding;
        float h = _canvasRT.rect.height / 2f - edgePadding;
        Vector2 pos = new Vector2(dir.x * w, dir.y * h);

        _arrowRT.anchoredPosition = pos;
        _arrowRT.rotation = Quaternion.FromToRotation(Vector3.up, dir);
    }

    void OnDestroy()
    {
        if (_arrowRT) Destroy(_arrowRT.gameObject);
    }
}
