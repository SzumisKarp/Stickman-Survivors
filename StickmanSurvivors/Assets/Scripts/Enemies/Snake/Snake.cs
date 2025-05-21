using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class Snake : Enemy
{
    private Rigidbody2D rb;
    private Transform player;

    [Header("Snake Settings")]
    public GameObject deathVFX;

    [Header("Combat")]
    public int contactDamage = 1;
    public float attackInterval = 1f;
    private float nextAttackTime = 0f;

    private Vector2 knockbackVelocity;

    [Header("Zigzag Movement")]
    [Tooltip("Czêstotliwoœæ oscylacji (rad/s)")]
    public float zigzagFrequency = 2f;
    [Tooltip("Amplituda oscylacji")]
    public float zigzagAmplitude = 1f;

    [Header("Poison Puddle Spawning")]
    [Tooltip("Prefab plamy, która zadaje obra¿enia")]
    public GameObject puddlePrefab;
    [Tooltip("Co ile sekund tworzyæ now¹ plamê")]
    public float spawnInterval = 0.5f;
    private float spawnTimer;

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

        // 1) pod¹¿anie + zigzag
        Vector2 toPlayer = ((Vector2)player.position - rb.position).normalized;
        Vector2 chaseVel = toPlayer * speed;
        Vector2 perp = new Vector2(-toPlayer.y, toPlayer.x);
        Vector2 zigzagVel = perp * (Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude);

        // 2) ruch i knockback
        rb.velocity = chaseVel + knockbackVelocity + zigzagVel;
        knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, Time.fixedDeltaTime * 5f);

        // 3) spawn plam trucizny przez mened¿era
        spawnTimer += Time.fixedDeltaTime;
        if (spawnTimer >= spawnInterval && puddlePrefab != null)
        {
            PoisonPuddleManager.Instance.SpawnPuddle(puddlePrefab, transform.position);
            spawnTimer = 0f;
        }
    }

    /// <summary>Wywo³ywane z OrbController przy eksplozji</summary>
    public void ApplyKnockback(Vector2 dir, float force)
    {
        knockbackVelocity += dir * force;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        var ph = collision.collider.GetComponent<PlayerHealth>();
        if (ph == null) return;

        if (Time.time < nextAttackTime) return;
        ph.TakeDamage(contactDamage);
        nextAttackTime = Time.time + attackInterval;
    }

    public override void TakeDamage(int amount)
    {
        Debug.Log($"[Snake.TakeDamage] HP before: {currentHP}, dmg: {amount}", this);
        base.TakeDamage(amount);
    }

    protected override void Die()
    {
        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);

        DropExpCrystal();
        Destroy(gameObject);
    }

    void OnDisable()
    {
        if (rb != null) rb.velocity = Vector2.zero;
    }
}
