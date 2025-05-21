using UnityEngine;
public class PickupMagnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float pullSpeed = 5f;  // jednostek na sekundê

    private Transform _player;
    void Start() => _player = transform.parent;
    private void OnTriggerStay2D(Collider2D other)
    {
        // 1) moneta?
        var coin = other.GetComponent<CoinPickup>();
        if (coin != null)
        {
            PullTowards(other);
            return;
        }

        // 2) kryszta³ EXP?
        var crystal = other.GetComponent<ExpCrystal>();
        if (crystal != null)
        {
            PullTowards(other);
        }
    }

    private void PullTowards(Collider2D other)
    {
        // 1) oblicz kierunek do magnetu (czyli transform.parent)
        Vector2 targetPos = transform.parent.position;
        Rigidbody2D rb = other.attachedRigidbody;

        if (rb != null)
        {
            // 2a) jeœli jest Rigidbody2D, przesuñ przez movePosition
            Vector2 newPos = rb.position + ((targetPos - rb.position).normalized * pullSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
        }
        else
        {
            // 2b) w przeciwnym razie zmieñ transform
            other.transform.position =
                Vector2.MoveTowards(other.transform.position, targetPos, pullSpeed * Time.deltaTime);
        }
    }
}
