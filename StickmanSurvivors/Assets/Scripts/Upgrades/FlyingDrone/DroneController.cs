using UnityEngine;

/// <summary>
/// Very lightweight homing projectile.  Finds the nearest Enemy on spawn and chases it.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class DroneController : MonoBehaviour
{
    private int _damage;
    private float _speed;
    private System.Action _onDestroyed;    // callback to ask parent upgrade for respawn

    private Enemy _target;
    private Rigidbody2D _rb;

    public void Initialize(int damage, float speed, System.Action onDestroyed)
    {
        _damage = damage;
        _speed = speed;
        _onDestroyed = onDestroyed;
    }

    void Awake() => _rb = GetComponent<Rigidbody2D>();

    void Start() => AcquireTarget();

    void FixedUpdate()
    {
        if (_target == null) { AcquireTarget(); return; }

        Vector2 dir = ((Vector2)_target.transform.position - _rb.position).normalized;
        _rb.velocity = dir * _speed;
    }

    private void AcquireTarget()
    {
        float sqrBest = float.MaxValue;
        Enemy best = null;

        foreach (var e in FindObjectsOfType<Enemy>())
        {
            float sqr = ((Vector2)e.transform.position - _rb.position).sqrMagnitude;
            if (sqr < sqrBest) { sqrBest = sqr; best = e; }
        }

        _target = best;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        enemy.TakeDamage(_damage);
        Explode();
    }

    private void Explode()
    {
        _onDestroyed?.Invoke();
        Destroy(gameObject);
    }

    void OnDisable()  // safety net
    {
        _rb.velocity = Vector2.zero;
    }
}
