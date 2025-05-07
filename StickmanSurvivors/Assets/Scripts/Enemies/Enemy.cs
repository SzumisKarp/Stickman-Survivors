using UnityEngine;
public abstract class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 3;
    protected int currentHP;
    public float speed = 2f;

    protected virtual void Awake()
    {
        currentHP = maxHP;
    }

    // Wywo³aj, gdy wróg ma dostaæ obra¿enia
    public virtual void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
            Die();
    }

    // Metoda do nadpisania przez podrzêdne klasy
    protected abstract void Die();
}
