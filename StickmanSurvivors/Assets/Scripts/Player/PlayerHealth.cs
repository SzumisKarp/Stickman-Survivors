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

    void Awake()
    {
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
