using UnityEngine;

public class ExpCrystal : MonoBehaviour
{
    public int expValue = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelSystem.Instance.AddExp(expValue);
            Destroy(gameObject);
        }
    }
}
