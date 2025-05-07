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
    protected override void Die()
    {
        // opcjonalnie: efekt œmierci
        if (deathVFX != null)
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
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
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
    }

    public override void TakeDamage(int amount)
    {
        Debug.Log($"[TakeDamage] przed: {currentHP}, obra¿enia: {amount}", this);
        base.TakeDamage(amount);
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
