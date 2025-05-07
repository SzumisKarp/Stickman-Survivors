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

    // Wywo�aj, gdy wr�g ma dosta� obra�enia
    public virtual void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
            Die();
    }

    // Metoda do nadpisania przez podrz�dne klasy
    protected abstract void Die();
}
