using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EventWolf : Enemy
{
    [Tooltip("Normalized direction the wolf will move in")]
    public Vector2 moveDirection;

    [Tooltip("Damage dealt to the player on contact")]
    public int damage = 1;

    [Header("Map Boundary Settings")]
    [Tooltip("CompositeCollider2D defining the playable map limits. Assign at runtime in EventManager.")]
    public CompositeCollider2D boundsCollider;

    [Header("Knockback Settings")]
    [Tooltip("How quickly knockback velocity decays (higher = faster)")]
    public float knockbackDecay = 5f;

    private Rigidbody2D _rb;
    private Vector2 _knockbackVelocity;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void FixedUpdate()
    {
        // Combine straight movement with any knockback
        Vector2 velocity = moveDirection * speed + _knockbackVelocity;
        _rb.velocity = velocity;

        // Decay knockback over time
        _knockbackVelocity = Vector2.Lerp(_knockbackVelocity, Vector2.zero, Time.fixedDeltaTime * knockbackDecay);

        // Destroy when outside map bounds
        if (boundsCollider != null)
        {
            var b = boundsCollider.bounds;
            Vector2 pos = _rb.position;
            if (pos.x < b.min.x || pos.x > b.max.x || pos.y < b.min.y || pos.y > b.max.y)
                Destroy(gameObject);
        }
    }

    /// <summary>Called by OrbController when hit.</summary>
    public void ApplyKnockback(Vector2 direction, float force)
    {
        _knockbackVelocity += direction * force;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            var playerHealth = collision.collider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
    }

    protected override void Die()
    {
        // Optional: spawn VFX here
        Destroy(gameObject);
    }
}
