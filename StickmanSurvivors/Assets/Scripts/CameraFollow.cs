using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Obiekt, za którym ma podążać kamera")]
    public Transform target;
    [Tooltip("Przesunięcie kamery względem targeta")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Follow Settings")]
    [Tooltip("Czas (w sekundach) w jakim kamera dogania target")]
    public float smoothTime = 0.3f;
    private Vector3 currentVelocity = Vector3.zero;

    [Header("Bounds Settings")]
    [Tooltip("Czy trzymać kamerę w granicach Tilemapy?")]
    public bool useBounds = true;
    [Tooltip("Tilemap, której granice wyznaczą obszar ruchu kamery")]
    public Tilemap tilemapBounds;

    private float minX, maxX, minY, maxY;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        if (useBounds && tilemapBounds != null)
        {
            // pobranie granic w local-space...
            Bounds b = tilemapBounds.localBounds;
            // ...i przeliczenie ich na world-space
            Vector3 origin = tilemapBounds.transform.position;
            minX = b.min.x + origin.x;
            maxX = b.max.x + origin.x;
            minY = b.min.y + origin.y;
            maxY = b.max.y + origin.y;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // pozycja, do której dążymy
        Vector3 desiredPos = target.position + offset;
        // wygładzone przejście
        Vector3 smoothPos = Vector3.SmoothDamp(transform.position, desiredPos, ref currentVelocity, smoothTime);

        if (useBounds && tilemapBounds != null)
        {
            // obliczenie zasięgów kamery w world-space
            float vertExtent = cam.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;

            // ograniczenie smoothPos do granic mapy
            smoothPos.x = Mathf.Clamp(smoothPos.x, minX + horzExtent, maxX - horzExtent);
            smoothPos.y = Mathf.Clamp(smoothPos.y, minY + vertExtent, maxY - vertExtent);
        }

        // ustawienie kamery
        transform.position = smoothPos;
    }
}
