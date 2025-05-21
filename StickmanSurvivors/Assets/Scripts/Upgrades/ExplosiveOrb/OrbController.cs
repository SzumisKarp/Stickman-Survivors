using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class OrbController : MonoBehaviour
{
    // --- orbit parameters (ustawiane przez ExplosiveOrbsUpgrade) ---
    private Transform _center;
    private float _radius;
    private float _angle;
    private float _speed;
    private int _damage;

    // --- nowe pola dla eksplozji i respawnu ---
    [Header("Explosion Settings")]
    [Tooltip("Warstwa, na której s¹ Twoi wrogowie")]
    public LayerMask enemyLayer;
    [Tooltip("Zasiêg odpychaj¹cej eksplozji")]
    public float explosionRadius = 2f;
    [Tooltip("Si³a odpychania (Impulse)")]
    public float explosionForce = 5f;
    [Tooltip("Czas ukrycia orba przed powrotem")]
    public float respawnDelay = 3f;

    // referencje
    private SpriteRenderer _sr;
    private Collider2D _col;

    public void Initialize(Transform center, float radius, float startAngle, float speed, int damage)
    {
        _center = center;
        _radius = radius;
        _angle = startAngle;
        _speed = speed;
        _damage = damage;
    }

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();
    }

    void Update()
    {
        // orb orbituje ca³y czas (nawet gdy jest ukryty)
        _angle += _speed * Time.deltaTime;
        if (_angle >= 360f) _angle -= 360f;
        float rad = _angle * Mathf.Deg2Rad;
        transform.position = _center.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * _radius;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var enemyHit = other.GetComponent<Enemy>();
        if (enemyHit == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (var h in hits)
        {
            Vector2 dir = (h.transform.position - transform.position).normalized;

            // 1) jeœli to Wolf:
            if (h.TryGetComponent<Wolf>(out var wolf))
            {
                wolf.ApplyKnockback(dir, explosionForce);
            }
            // 2) jeœli to Snake:
            else if (h.TryGetComponent<Snake>(out var snake))
            {
                snake.ApplyKnockback(dir, explosionForce);
            }
            // 3) jeœli to EventWolf:
            else if (h.TryGetComponent<EventWolf>(out var eventWolf))
            {
                eventWolf.ApplyKnockback(dir, explosionForce);
            }
            // 4) fallback dla innych Enemy bez ApplyKnockback:
            else if (h.TryGetComponent<Rigidbody2D>(out var targetRb))
            {
                targetRb.AddForce(dir * explosionForce, ForceMode2D.Impulse);
            }

            // zadajemy damage ka¿demu Enemy
            if (h.TryGetComponent<Enemy>(out var e))
                e.TakeDamage(_damage);
        }

        // hide & respawn jak wczeœniej…
        _sr.enabled = false;
        _col.enabled = false;
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);

        // przywracamy collider + sprite
        _sr.enabled = true;
        _col.enabled = true;
    }
}
