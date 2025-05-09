using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 3;
    protected int currentHP;
    public float speed = 2f;

    [Header("Rewards")]
    [Tooltip("Prefab kryszta³u do upuszczenia po œmierci")]
    public GameObject expCrystalPrefab;

    // cached container for all enemies
    private static Transform _expContainer;

    protected virtual void Awake()
    {
        currentHP = maxHP;

        // find & cache the EXP holder once
        if (_expContainer == null)
        {
            // first try by tag:
            var go = GameObject.FindGameObjectWithTag("EXP");
            if (go != null)
                _expContainer = go.transform;
            else
            {
                // fallback: find by name
                go = GameObject.Find("EXP");
                if (go != null)
                    _expContainer = go.transform;
                else
                    Debug.LogWarning("Enemy: no GameObject tagged or named 'EXP' found in scene!");
            }
        }
    }

    /// <summary>Call this when this Enemy should take damage.</summary>
    public virtual void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
            Die();
    }

    /// <summary>Override in subclasses to play VFX, drop rewards, then destroy.</summary>
    protected abstract void Die();

    /// <summary>Use this in Die() to spawn a crystal under the EXP container.</summary>
    protected void DropExpCrystal()
    {
        if (expCrystalPrefab != null && _expContainer != null)
        {
            Instantiate(expCrystalPrefab,
                        transform.position,
                        Quaternion.identity,
                        _expContainer);
        }
    }
}
