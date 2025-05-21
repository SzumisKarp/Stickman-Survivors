using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Spider : Enemy
{
    [Header("Movement")]
    [Tooltip("How close the spider gets before starting its jump.")]
    public float detectionRange = 3f;

    [Tooltip("Horizontal chase speed.")]
    public float chaseSpeed = 2f;     // renamed from 'speed'


    [Header("Jump Settings")]
    [Tooltip("How long to wait once in range before leaping.")]
    public float jumpDelay = 1f;
    [Tooltip("Impulse strength applied on jump.")]
    public float jumpForce = 5f;

    [Header("VFX")]
    [Tooltip("Prefab to spawn on death.")]
    public GameObject deathVFX;

    [Header("Combat")]
    [Tooltip("Damage dealt on contact.")]
    public int contactDamage = 1;
    [Tooltip("Minimum seconds between contact hits.")]
    public float attackInterval = 1f;

    private Rigidbody2D rb;
    private Transform player;
    private bool isPreparingJump = false;
    private float nextAttackTime = 0f;
    private bool isAlive = true;

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
        if (!isAlive || player == null || isPreparingJump)
            return;

        float dist = Vector2.Distance(rb.position, player.position);

        if (dist > detectionRange)
        {
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            rb.velocity = dir * chaseSpeed;   // ← use chaseSpeed here
        }

        else
        {
            // 2) In range → jump sequence
            StartCoroutine(JumpSequence());
        }
    }

    private IEnumerator JumpSequence()
    {
        isPreparingJump = true;

        // wind-up pause
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(jumpDelay);

        if (player != null)
        {
            // leap impulse
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            rb.AddForce(dir * jumpForce, ForceMode2D.Impulse);
        }

        // small cooldown before resuming chase
        yield return new WaitForSeconds(0.1f);
        isPreparingJump = false;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        var ph = col.collider.GetComponent<PlayerHealth>();
        if (ph == null) return;

        if (Time.time < nextAttackTime) return;
        ph.TakeDamage(contactDamage);
        nextAttackTime = Time.time + attackInterval;
    }

    public override void TakeDamage(int amount)
    {
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
        isAlive = false;
        if (rb != null) rb.velocity = Vector2.zero;
    }
}
