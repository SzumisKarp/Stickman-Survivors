using UnityEngine;

public class Wolf : Enemy
{
    private Rigidbody2D rb;
    private Transform player;

    [Header("Wolf Settings")]
    public GameObject deathVFX;

    [Header("Combat")]
    public int contactDamage = 1;
    public float attackInterval = 1f;
    private float nextAttackTime = 0f;

    private Vector2 knockbackVelocity;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        player = PlayerController.Instance?.transform;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 chaseDir = ((Vector2)player.position - rb.position).normalized;
        Vector2 chaseVelocity = chaseDir * speed;

        // combine chase + any knockback
        rb.velocity = chaseVelocity + knockbackVelocity;

        // decay knockback over time
        knockbackVelocity = Vector2.Lerp(knockbackVelocity,
                                         Vector2.zero,
                                         Time.fixedDeltaTime * 5f);
    }

    /// <summary>Called by OrbController when hit.</summary>
    public void ApplyKnockback(Vector2 dir, float force)
    {
        knockbackVelocity += dir * force;
    }

    public override void TakeDamage(int amount)
    {
        Debug.Log($"[TakeDamage] before: {currentHP}, dmg: {amount}", this);
        base.TakeDamage(amount);
    }

    protected override void Die()
    {
        // 1) death VFX
        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);

        // 2) drop exp crystal (automatically parented under EXP)
        DropExpCrystal();

        // 3) destroy
        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        var ph = collision.collider.GetComponent<PlayerHealth>();
        if (ph == null) return;

        if (Time.time < nextAttackTime) return;
        ph.TakeDamage(contactDamage);
        nextAttackTime = Time.time + attackInterval;
    }

    void OnDisable()
    {
        if (rb != null) rb.velocity = Vector2.zero;
    }
}
