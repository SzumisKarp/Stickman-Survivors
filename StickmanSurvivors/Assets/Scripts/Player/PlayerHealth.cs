using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 5;
    [HideInInspector] public int currentHP;

    [Header("Events")]
    public UnityEvent onDamaged;
    public UnityEvent onDied;

    public static PlayerHealth Instance { get; private set; }
    void Awake()
    {
        // singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // original initialization
        currentHP = maxHP;
    }
    public void TakeDamage(int amount)
    {
        if (currentHP <= 0) return;
        currentHP -= amount;
        Debug.Log($"[PlayerHealth] HP = {currentHP}/{maxHP}");
        onDamaged?.Invoke();

        if (currentHP <= 0)
            Die();
    }
    public void Heal(int amount)
    {
        // clamp to maxHP
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        Debug.Log($"[PlayerHealth] Healed to {currentHP}/{maxHP}");
        // optionally invoke a onHealed UnityEvent here if you want UI feedback
    }

    void Die()
    {
        Debug.Log("[PlayerHealth] Gracz umar³");
        onDied?.Invoke();
        var ctrl = GetComponent<PlayerController>();
        if (ctrl != null) ctrl.enabled = false;
        var atk = GetComponent<PlayerAttack>();
        if (atk != null) atk.enabled = false;
    }
}
