using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals damage-per-second to every Enemy that stays inside the circle collider
/// and handles the puddle’s fade-out width / lifetime on its TrailRenderer.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(TrailRenderer))]
public class InkSplatter : MonoBehaviour
{
    private int _damagePerSecond;
    private float _lifetime;

    // per-enemy timers so each one is hurt only once per second
    private readonly Dictionary<Enemy, float> _nextTick = new Dictionary<Enemy, float>();

    void Awake()
    {
        // One-time width setup (damage radius == visual radius)
        var col = GetComponent<CircleCollider2D>();
        var trail = GetComponent<TrailRenderer>();
        trail.widthMultiplier = col.radius * 2f;
    }

    /// <summary>Called immediately after Instantiate by InkBulletsUpgrade.</summary>
    public void Initialize(int dps, float life)
    {
        _damagePerSecond = dps;
        _lifetime = life;

        // make the TrailRenderer fade for exactly the lifetime
        GetComponent<TrailRenderer>().time = _lifetime;

        Destroy(gameObject, _lifetime);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        if (!_nextTick.ContainsKey(enemy)) _nextTick.Add(enemy, 0f);

        if (Time.time >= _nextTick[enemy])
        {
            enemy.TakeDamage(_damagePerSecond);
            _nextTick[enemy] = Time.time + 1f;   // hurt again in 1 s
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (enemy) _nextTick.Remove(enemy);
    }
}
